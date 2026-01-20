using UnityEngine;
using System.Collections;
using VectorViolet.Core.Stats;
using System;
using System.Collections.Generic;

public class WeaponController : MonoBehaviour
{
    [HideInInspector] public Transform target;
    [HideInInspector] public Vector3 targetDirection;

    [Header("References")]
    [SerializeField] private Animator animator;
    [SerializeField] private WeaponBase activeWeapon;
    [SerializeField] private List<WeaponBase> passiveWeapons = new List<WeaponBase>();
    

    [Header("Attack Settings")]
    [SerializeField, Range(0f, 1f)] private float attackImpactPoint = 0.5f; 
    
    // Animator parameter hashes
    private static readonly int AttackSpeedHash = Animator.StringToHash("AttackSpeed");
    private static readonly int AttackTriggerHash = Animator.StringToHash("Attack");

    public bool IsAttacking => isAttacking;
    private float lastAttackTime;
    private bool isAttacking = false;
    private EntityMovement entityMovement;
    public WeaponBase CurrentWeapon => activeWeapon;
    private StatBase attackSpeedStat;

    private void Start()
    {
        entityMovement = GetComponent<EntityMovement>();
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
 
        StatHolder statHolder = activeWeapon.GetComponent<StatHolder>();
        if (statHolder != null)
        {
            attackSpeedStat = statHolder.GetStat("AttackSpeed");
        }

        activeWeapon?.OnEquip(this);
    }

    private void UnequipWeapon()
    {
        activeWeapon?.OnUnequip();
        activeWeapon = null;
    }

    public void ActiveAttack()
    {
        if (activeWeapon == null || attackSpeedStat == null)
            return;

        if (Time.time < lastAttackTime + (1f / attackSpeedStat.GetValue()))
            return;

        StartCoroutine(PerformAttackRoutine());
    }

    IEnumerator PerformAttackRoutine()
    {
        isAttacking = true;
        lastAttackTime = Time.time;
        
        if (target != null)
        {
            targetDirection = (target.transform.position - transform.position).normalized;
            entityMovement.FaceDirection(targetDirection);
            entityMovement.Stop();
        }
        
        float currentAttackSpeed = attackSpeedStat.GetValue();
        if (animator != null)
        {
            animator.SetFloat(AttackSpeedHash, currentAttackSpeed);
            animator.SetTrigger(AttackTriggerHash);
        }

        float delayUntilHit = (1f / currentAttackSpeed) * attackImpactPoint; 
        
        yield return new WaitForSeconds(delayUntilHit);

        activeWeapon?.Attack(targetDirection);

        float remainingTime = (1f / currentAttackSpeed) * (1f - attackImpactPoint);
        yield return new WaitForSeconds(remainingTime);

        isAttacking = false;
    }
}