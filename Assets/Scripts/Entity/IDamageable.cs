using UnityEngine;
using System;

// Bu interface, canı olan her şeyin uyması gereken kuralları belirler.
public interface IDamageable 
{   
    void TakeDamage(float amount);
    event Action<float> OnTakeDamage;
    event Action OnDeath;
    event Action<float> OnHealthChanged; // Percentage health change event
}