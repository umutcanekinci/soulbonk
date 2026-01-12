using UnityEngine;
using VectorViolet.Core.Stats;
using System;

[RequireComponent(typeof(StatHolder))]
[RequireStat("Health")]
public class EntityHP : MonoBehaviour, IDamageable
{
    private ResourceStat _healthStat;

    public bool IsDead => _healthStat != null && _healthStat.CurrentValue <= 0;

    public event Action<float> OnTakeDamage;
    public event Action OnDeath;
    public event Action<float> OnHealthChanged;

    private void Start() 
    {
        var statHolder = GetComponent<StatHolder>();
        if (statHolder != null)
        {
            _healthStat = statHolder.GetStat("Health") as ResourceStat;

            if (_healthStat != null)
            {
                _healthStat.OnValueChanged += OnHpChange;
                OnHpChange(_healthStat); 
            } 
        }
        else
        {
            Debug.LogError($"{name} does not have a StatHolder component!");
        }
    }

    private void OnDestroy() 
    {
        // DEĞİŞİKLİK 4: Artık temiz bir şekilde abonelikten çıkabiliriz.
        if (_healthStat != null)
        {
            _healthStat.OnValueChanged -= OnHpChange;
        }
    }

    // Event Dinleyici Metodu
    // StatResource gönderildiği için parametre tipi StatResource olmalı
    private void OnHpChange(StatBase stat) // Parametre artık StatBase
        {
            // Gelen stat gerçekten bir Resource mu? Kontrol edip dönüştürüyoruz (Casting)
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
        }
    }
}