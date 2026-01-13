using UnityEngine;
using VectorViolet.Core.Stats;
using System;
using System.Collections;
using VectorViolet.Core.Audio;

[RequireComponent(typeof(StatHolder))]
[RequireStat("Health")]
public class EntityHP : MonoBehaviour, IDamageable
{
    [SerializeField] private SoundPack deathSounds;
    private ResourceStat _healthStat;

    public bool IsDead => _healthStat != null && _healthStat.CurrentValue <= 0;

    public event Action<float> OnTakeDamage;
    public event Action OnDeath;
    public event Action<float> OnHealthChanged;
    

    private void Start()  // TODO rename to OnEnable, but it breaks things now
    {
        var statHolder = GetComponent<StatHolder>();
        if (statHolder != null)
        {
            _healthStat = statHolder.GetStat("Health") as ResourceStat;
            _healthStat.OnValueChanged += OnHpChange;
            OnHpChange(_healthStat); 
        }
    }

    private void OnDisable() 
    {
        if (_healthStat != null)
        {
            _healthStat.OnValueChanged -= OnHpChange;
        }
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

    public void TakeDamage(float damageAmount)
    {
        if (_healthStat == null)
            return;

        _healthStat.CurrentValue -= damageAmount;
        OnTakeDamage?.Invoke(damageAmount);

        if (_healthStat.CurrentValue <= 0)
        {
            OnDeath?.Invoke();
            SoundManager.Instance.PlaySFX(deathSounds);
        }
    }

}