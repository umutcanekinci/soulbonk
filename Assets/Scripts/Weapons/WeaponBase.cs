using UnityEngine;
using VectorViolet.Core.Stats;

[RequireComponent(typeof(StatHolder))]
[RequireStat("AttackDamage", "AttackSpeed", "AttackRange")] 
public abstract class WeaponBase : MonoBehaviour, IWeapon
{
    protected StatBase attackDamageStat;
    protected StatBase attackRangeStat;
    
    private bool _isInitialized = false;
    private StatModifier _scalingModifier;

    public void Initialize()
    {
        if (_isInitialized) return;

        StatHolder statHolder = GetComponent<StatHolder>();
        if (statHolder != null)
        {
            attackDamageStat = statHolder.GetStat("AttackDamage");
            attackRangeStat = statHolder.GetStat("AttackRange");
            
            _isInitialized = true;
        }
    }

    public virtual void OnEquip(WeaponController weaponController)
    {
        Initialize(); 

        StatHolder playerStats = weaponController.GetComponent<StatHolder>();
        
        if (playerStats != null)
        {
            StatBase playerStrength = playerStats.GetStat("Strength");

            if (playerStrength != null && attackDamageStat != null)
            {
                CleanUpModifier();

                float scalingFactor = 0.5f; 
                float bonusDamage = playerStrength.GetValue() * scalingFactor;

                _scalingModifier = new StatModifier(bonusDamage, ModifierType.Flat, playerStats);
                
                if (attackDamageStat is AttributeStat attrDamage)
                {
                    attrDamage.AddModifier(_scalingModifier);
                }
            }
        }
    }

    public virtual void OnUnequip()
    {
        CleanUpModifier();
    }

    private void CleanUpModifier()
    {
        if (_scalingModifier != null && attackDamageStat is AttributeStat attrDamage)
        {
            attrDamage.RemoveModifier(_scalingModifier);
            _scalingModifier = null;
        }
    }

    public StatBase GetAttackRangeStat() => attackRangeStat;
    public abstract void Attack(Vector3 direction);
}