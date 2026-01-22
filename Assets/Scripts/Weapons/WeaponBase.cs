using UnityEngine;
using VectorViolet.Core.Stats;
using VectorViolet.Core.Audio;

[RequireComponent(typeof(StatHolder))]
[RequireStat("AttackDamage", "AttackSpeed", "AttackRange")] 
public abstract class WeaponBase : MonoBehaviour, IWeapon
{
    [Header("Audio Settings")]
    [SerializeField] private SoundPack missSounds;
    [SerializeField] private SoundPack hitSounds;

    [Header("Scaling Settings")]
    [SerializeField] private float scalingFactor = 0.5f; 

    protected StatBase _attackDamageStat, _attackRangeStat, _attackSpeedStat, _critRateStat, _critDamageStat;
    
    private bool _isInitialized = false;
    private StatModifier _scalingModifier;

    public float AttackDamage => _isInitialized ? _attackDamageStat.GetValue() : 0f;
    public float AttackRange => _isInitialized ? _attackRangeStat.GetValue() : 0f;
    public float AttackSpeed => _isInitialized ? _attackSpeedStat.GetValue() : 0f;

    public void Initialize()
    {
        if (_isInitialized) return;

        StatHolder statHolder = GetComponent<StatHolder>();
        if (statHolder != null)
        {
            _attackDamageStat = statHolder.GetStat("AttackDamage");
            _attackRangeStat = statHolder.GetStat("AttackRange");
            _attackSpeedStat = statHolder.GetStat("AttackSpeed");
            _critRateStat = statHolder.GetStat("CritRate");
            _critDamageStat = statHolder.GetStat("CritDamage");
            _isInitialized = true;
        }
    }

    public virtual void OnEquip(StatHolder entityStats)
    {
        Initialize(); 
        
        if (entityStats != null)
        {
            // Attack Damage = Base Damage + (Player Strength * Scaling Factor)
            StatBase playerStrength = entityStats.GetStat("Strength");
            if (playerStrength != null && _attackDamageStat != null)
            {
                CleanUpModifier();

                float bonusDamage = playerStrength.GetValue() * scalingFactor;

                _scalingModifier = new StatModifier(bonusDamage, ModifierType.Flat, entityStats);
                
                if (_attackDamageStat is AttributeStat attrDamage)
                {
                    attrDamage.AddModifier(_scalingModifier);
                }
            }
        }
    }

    private void CleanUpModifier()
    {
        if (_scalingModifier != null && _attackDamageStat is AttributeStat attrDamage)
        {
            attrDamage.RemoveModifier(_scalingModifier);
            _scalingModifier = null;
        }
    }

    public virtual void OnUnequip()
    {
        CleanUpModifier();
    }

    public StatBase GetAttackRangeStat() => _attackRangeStat;
    
    public abstract void Attack(Vector3 direction);

    protected void PlaySFX(bool anyHit)
    {
        SoundManager.Instance.PlaySFX(anyHit ? hitSounds : missSounds);
    }

    protected virtual float CalculateDamage(out bool isCritical)
    {
        float baseDamage = _attackDamageStat != null ? _attackDamageStat.GetValue() : 0f;
        float critRate   = _critRateStat     != null ? _critRateStat.GetValue()     : 0f;
        float critDamage = _critDamageStat   != null ? _critDamageStat.GetValue()   : 0f;

        isCritical = Random.value * 100 < critRate;
        if (isCritical)
        {
            return baseDamage * (1f + (critDamage / 100f));
        }
        return baseDamage;
    }

}