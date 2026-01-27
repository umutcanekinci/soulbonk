using UnityEngine;
using VectorViolet.Core.Stats;
using System;
using VectorViolet.Core.Audio;

[RequireComponent(typeof(StatHolder))]
[RequireStat("Health")]
public class EntityHP : MonoBehaviour, IDamageable
{
    [Header("Audio Settings")]
    [SerializeField] private SoundPack deathSounds;
    private ResourceStat _healthStat;

    public bool IsDead => _healthStat != null && _healthStat.CurrentValue <= 0;

    public event Action<float, bool> OnTakeDamage;
    public event Action OnDeath;
    public event Action<float> OnHeal;
    public event Action<float> OnHealthChanged;

    private void Awake()
    {
        var statHolder = GetComponent<StatHolder>();
        if (statHolder != null)
            _healthStat = statHolder.GetStat("Health") as ResourceStat;
    }

    private void OnEnable()
    {
        if (_healthStat != null)
        {
            _healthStat.OnValueChanged += OnHpChange;
            OnHpChange(_healthStat); 
        }
    }

    private void OnDisable() 
    {
        if (_healthStat != null)
            _healthStat.OnValueChanged -= OnHpChange;
    }
 
    private void OnHpChange(StatBase stat)
    {
        if (stat is ResourceStat res)
        {
            if (res.MaxValue > 0)
            {
                float healthPercent = res.CurrentValue / res.MaxValue;
                OnHealthChanged?.Invoke(healthPercent);
            }
            else
            {
                OnHealthChanged?.Invoke(0);
            }
        }
    }

    public void TakeDamage(float damageAmount, bool isCritical = false)
    {
        if (_healthStat == null || IsDead || Mathf.Approximately(damageAmount, 0) || !GameManager.IsGameplay) 
            return;

        _healthStat.CurrentValue -= damageAmount;
        OnTakeDamage?.Invoke(damageAmount, isCritical);

        if (_healthStat.CurrentValue <= 0)
        {
            OnDeath?.Invoke();
            SoundManager.Instance.PlaySFX(deathSounds);
        }
    }

    public void HealToFull() => Heal(_healthStat.MaxValue);

    public void Heal(float healAmount)
    {
        if (IsDead || healAmount <= 0 || _healthStat == null) 
            return;
        
        _healthStat.CurrentValue += healAmount;
        OnHeal?.Invoke(healAmount);
    }
    
}