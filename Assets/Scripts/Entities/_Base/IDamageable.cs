using UnityEngine;
using System;

public interface IDamageable 
{   
    void TakeDamage(float amount);
    event Action<float> OnTakeDamage;
    event Action OnDeath;
    event Action<float> OnHealthChanged; // Percentage health change event
}