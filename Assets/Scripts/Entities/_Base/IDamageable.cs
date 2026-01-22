using UnityEngine;
using System;

public interface IDamageable 
{   
    void TakeDamage(float amount, bool isCritical = false);
    event Action<float, bool> OnTakeDamage;
    event Action OnDeath;
    public event Action<float> OnHeal;
    event Action<float> OnHealthChanged; // Percentage health change event
}