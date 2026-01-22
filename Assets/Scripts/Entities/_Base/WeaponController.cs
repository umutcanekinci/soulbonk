using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using VectorViolet.Core.Stats;
using System;

[RequireComponent(typeof(EntityMovement))]
[RequireComponent(typeof(StatHolder))]
public class WeaponController : MonoBehaviour
{
    public WeaponBase CurrentWeapon => activeWeapon;

    public bool IsAttacking => isAttacking;
    public float AttackDamage => activeWeapon ? activeWeapon.AttackDamage : 0f;
    public float AttackRange => activeWeapon ? activeWeapon.AttackRange : 0f;
    public float AttackSpeed => activeWeapon ? activeWeapon.AttackSpeed : 0f;

    [HideInInspector] public Transform target;
    [HideInInspector] public Vector3 targetDirection;

    [Header("References")]
    [SerializeField] private WeaponBase activeWeapon;
    [SerializeField] private List<WeaponBase> passiveWeapons = new List<WeaponBase>();
    
    [Header("Attack Settings")]
    [SerializeField, Range(0f, 1f)] private float attackImpactPoint = 0.5f; 

    private float lastAttackTime;
    private bool isAttacking = false;
    
    private EntityAnimator _animator;
    private EntityMovement _movement;
    private StatHolder _stats;
    
    private Dictionary<WeaponBase, Coroutine> passiveRoutines = new Dictionary<WeaponBase, Coroutine>();

    private void Start()
    {
        _animator = GetComponent<EntityAnimator>();
        _movement = GetComponent<EntityMovement>();
        _stats = GetComponent<StatHolder>();

        if (activeWeapon != null)
            EquipWeapon(activeWeapon);

        
        foreach (var weapon in passiveWeapons)
        {
            EquipPassiveWeapon(weapon);
        }
    }

    public void EquipWeapon(WeaponBase newWeapon)
    {
        if (newWeapon == null) return;

        if (activeWeapon != null)
            UnequipWeapon();

        activeWeapon = newWeapon;
        activeWeapon.gameObject.SetActive(true);
        activeWeapon.Initialize(); 
        activeWeapon.OnEquip(_stats); 
    }

    private void UnequipWeapon()
    {
        if (activeWeapon != null)
        {
            activeWeapon.OnUnequip();
            activeWeapon.gameObject.SetActive(false);
            activeWeapon = null;
        }
    }

    public void ActiveAttack()
    {
        if (activeWeapon == null || isAttacking) return;

        float speed = activeWeapon.AttackSpeed; 
        if (speed <= 0f) return;

        if (Time.time < lastAttackTime + (1f / speed)) return;

        StartCoroutine(PerformActiveAttackRoutine(speed));
    }

    private IEnumerator PerformActiveAttackRoutine(float speed)
    {
        isAttacking = true;
        lastAttackTime = Time.time;
        
        
        if (target != null)
        {
            targetDirection = (target.transform.position - transform.position).normalized;
            _movement.FaceDirection(targetDirection);
            _movement.Stop();
        }
        else
        {
            
            targetDirection = _movement.LastFacingDirection; 
        }

        
        if (_animator != null)
        {
            _animator.TriggerAttack(speed);
        }

        
        float delayUntilHit = (1f / speed) * attackImpactPoint; 
        yield return new WaitForSeconds(delayUntilHit);

        
        activeWeapon.Attack(targetDirection);

        
        float remainingTime = (1f / speed) * (1f - attackImpactPoint);
        yield return new WaitForSeconds(remainingTime);

        isAttacking = false;
    }

    public void EquipPassiveWeapon(WeaponBase passiveWeapon)
    {
        if (passiveWeapon == null) return;
        
        if (!passiveWeapons.Contains(passiveWeapon))
        {
            passiveWeapons.Add(passiveWeapon);
        }

        passiveWeapon.Initialize();
        passiveWeapon.OnEquip(_stats);
        passiveWeapon.gameObject.SetActive(true);

        
        if (!passiveRoutines.ContainsKey(passiveWeapon))
        {
            passiveRoutines[passiveWeapon] = StartCoroutine(PassiveAttackLoop(passiveWeapon));
        }
    }

    public void UnequipPassiveWeapon(WeaponBase passiveWeapon)
    {
        if (passiveWeapons.Contains(passiveWeapon))
        {
            
            if (passiveRoutines.ContainsKey(passiveWeapon))
            {
                StopCoroutine(passiveRoutines[passiveWeapon]);
                passiveRoutines.Remove(passiveWeapon);
            }

            passiveWeapon.OnUnequip();
            passiveWeapon.gameObject.SetActive(false);
            passiveWeapons.Remove(passiveWeapon);
        }
    }
    
    private IEnumerator PassiveAttackLoop(WeaponBase weapon)
    {
        while (true)
        {
            if (!GameManager.IsGameplay)
            {
                yield return null;
                continue;
            }

            float speed = weapon.AttackSpeed;
            
            if (speed <= 0.1f)
                speed = 1f;

            float cooldown = 1f / speed;

            yield return new WaitForSeconds(cooldown);

            Vector3 fireDirection = _movement.LastFacingDirection; 
            
            if(target != null) 
                fireDirection = (target.transform.position - transform.position).normalized;

            weapon.Attack(fireDirection);
        }
    }
}