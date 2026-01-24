using UnityEngine;
using VectorViolet.Core.Stats;
using VectorViolet.Core.Audio;
using System.Collections.Generic;

// 1. ADIM: Scaling kuralını tanımlayan küçük bir sınıf oluşturuyoruz.
[System.Serializable]
public class WeaponScalingConfig
{
    [Tooltip("Which Player Stat to base the scaling on? (e.g., Strength)")]
    public StatDefinition sourceStatDef;

    [Tooltip("Which Weapon Stat to increase? (e.g., AttackDamage)")]
    public StatDefinition targetStatDef;

    [Tooltip("Multiplier factor (e.g., 0.5 means 10 Str -> +5 Dmg)")]
    public float factor = 0.5f;
}

[RequireComponent(typeof(StatHolder))]
[RequireStat("AttackDamage", "AttackSpeed", "AttackRange")] 
public abstract class WeaponBase : MonoBehaviour, IWeapon
{
    [Header("Audio Settings")]
    [SerializeField] private SoundPack missSounds;
    [SerializeField] private SoundPack hitSounds;

    [Header("Dynamic Scaling Settings")]
    // 2. ADIM: Tek bir scaling yerine liste kullanıyoruz.
    [SerializeField] private List<WeaponScalingConfig> scalings = new List<WeaponScalingConfig>();

    protected StatBase _attackDamageStat, _attackRangeStat, _attackSpeedStat, _critRateStat, _critDamageStat;
    private bool _isInitialized = false;

    // Aktif modifierları ve hangi player statına bağlı olduklarını takip etmek için bir liste
    // (Unequip ederken eventlerden çıkmak için gerekli)
    private class ActiveScalingLink
    {
        public StatBase SourceStat;      
        public AttributeStat TargetWeaponStat; 
        public StatModifier ActiveModifier; 
        public WeaponScalingConfig Config;
        public object SourceObject; // Modifier kaynağı (WeaponBase instance)

        public void OnSourceStatChanged(StatBase updatedStat)
        {
            if (ActiveModifier != null)
            {
                TargetWeaponStat.RemoveModifier(ActiveModifier);
            }

            float bonus = SourceStat.Value * Config.factor;

            if (Mathf.Abs(bonus) > 0.01f)
            {
                ActiveModifier = new StatModifier(bonus, ModifierType.Flat, SourceObject);
                TargetWeaponStat.AddModifier(ActiveModifier);
            }
            else
            {
                ActiveModifier = null;
            }
        }

        public void Clear()
        {
            if (ActiveModifier != null)
            {
                if (SourceStat != null)
                    SourceStat.OnValueChanged -= OnSourceStatChanged;
                
                if (TargetWeaponStat != null && ActiveModifier != null)
                    TargetWeaponStat.RemoveModifier(ActiveModifier);
                ActiveModifier = null;
            }
        }
    }
    
    private List<ActiveScalingLink> _activeLinks = new List<ActiveScalingLink>();

    // Propertyler aynı kalabilir...
    public float AttackDamage => _isInitialized ? _attackDamageStat.Value : 0f;
    public float AttackRange => _isInitialized ? _attackRangeStat.Value : 0f;
    public float AttackSpeed => _isInitialized ? _attackSpeedStat.Value : 0f;

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
        if (entityStats == null) return;

        foreach (var config in scalings)
        {
            if (config.sourceStatDef == null || config.targetStatDef == null) continue;

            StatBase playerStat = entityStats.GetStat(config.sourceStatDef.ID);
            StatBase weaponStat = GetComponent<StatHolder>().GetStat(config.targetStatDef.ID);

            if (playerStat != null && weaponStat is AttributeStat attrWeaponStat)
            {
                var link = new ActiveScalingLink
                {
                    SourceStat = playerStat,
                    TargetWeaponStat = attrWeaponStat,
                    Config = config,
                    SourceObject = this
                };

                _activeLinks.Add(link);

                link.OnSourceStatChanged(playerStat);
                playerStat.OnValueChanged += link.OnSourceStatChanged;
            }
        }
    }

    public virtual void OnUnequip()
    {
        _activeLinks.ForEach(link => link.Clear());
        _activeLinks.Clear();
    }

    public StatBase GetAttackRangeStat() => _attackRangeStat;
    public abstract void Attack(Vector3 direction);
    
    protected void PlaySFX(bool anyHit)
    {
        SoundManager.Instance.PlaySFX(anyHit ? hitSounds : missSounds);
    }

    protected virtual float CalculateDamage(out bool isCritical)
    {
        float baseDamage = _attackDamageStat != null ? _attackDamageStat.Value : 0f;
        float critRate = _critRateStat != null ? _critRateStat.Value : 0f;
        float critDamage = _critDamageStat != null ? _critDamageStat.Value : 0f;

        isCritical = Random.value * 100 < critRate;
        if (isCritical)
        {
            return baseDamage * (1f + (critDamage / 100f));
        }
        return baseDamage;
    }
}