using UnityEngine;
using System.Collections;
using VectorViolet.Core.Stats;
using System;
using System.Collections.Generic;

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
    [SerializeField] private Animator animator;
    [SerializeField] private WeaponBase activeWeapon;
    [SerializeField] private List<WeaponBase> passiveWeapons = new List<WeaponBase>();
    
    [Header("Attack Settings")]
    [SerializeField, Range(0f, 1f)] private float attackImpactPoint = 0.5f; 

    private static readonly int AttackSpeedHash = Animator.StringToHash("AttackSpeed");
    private static readonly int AttackTriggerHash = Animator.StringToHash("Attack");

    private float lastAttackTime;
    private bool isAttacking = false;
    private EntityMovement entityMovement;
    private StatHolder entityStats;

    private void Start()
    {
        entityMovement = GetComponent<EntityMovement>();
        entityStats = GetComponent<StatHolder>();

        if (activeWeapon != null)
            EquipWeapon(activeWeapon);
    }

    private void OnValidate()
    {
        EquipWeapon(activeWeapon);    
    }

    public void EquipWeapon(WeaponBase newWeapon)
    {
        if (newWeapon == null)
            throw new ArgumentNullException(nameof(newWeapon), "Cannot equip a null weapon.");

        if (activeWeapon != null)
        {
            UnequipWeapon();
        }

        activeWeapon = newWeapon;
        activeWeapon.gameObject.SetActive(true);
        activeWeapon?.OnEquip(entityStats);
    }

    private void UnequipWeapon()
    {
        activeWeapon?.OnUnequip();
        activeWeapon.gameObject.SetActive(false);
        activeWeapon = null;
    }

    public void ActiveAttack()
    {
        if (activeWeapon == null)
            return;

        float speed = activeWeapon.AttackSpeed;

        if (speed <= 0f)
            return;

        if (Time.time < lastAttackTime + (1f / speed))
            return;

        StartCoroutine(PerformAttackRoutine(speed));
    }

    IEnumerator PerformAttackRoutine(float speed)
    {
        isAttacking = true;
        lastAttackTime = Time.time;
        
        if (target != null)
        {
            targetDirection = (target.transform.position - transform.position).normalized;
            entityMovement.FaceDirection(targetDirection);
            entityMovement.Stop();
        }
        else
        {
            targetDirection = entityMovement.LastFacingDirection;
        }
        
        if (animator != null)
        {
            animator.SetFloat(AttackSpeedHash, speed);
            animator.SetTrigger(AttackTriggerHash);
        }

        float delayUntilHit = (1f / speed) * attackImpactPoint; 
        
        yield return new WaitForSeconds(delayUntilHit);

        activeWeapon?.Attack(targetDirection);

        float remainingTime = (1f / speed) * (1f - attackImpactPoint);
        yield return new WaitForSeconds(remainingTime);

        isAttacking = false;
    }
}