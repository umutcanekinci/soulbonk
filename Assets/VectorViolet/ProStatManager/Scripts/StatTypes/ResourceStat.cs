using System;
using UnityEngine;

namespace VectorViolet.Core.Stats
{
    [Serializable]
    public class ResourceStat : StatBase
    {
        [SerializeField] private float _currentValue;
        
        public float MaxValue = 100f; 

        public float CurrentValue
        {
            get => _currentValue;
            set
            {
                _currentValue = Mathf.Clamp(value, 0, MaxValue);
                InvokeValueChanged();
            }
        }

        public override void OnInspectorChanged()
        {
            CurrentValue = _currentValue; 
        }

        public override float Value => _currentValue;
        public override float BaseValue
        {
            get => MaxValue;
            protected set => MaxValue = value;
        }
        public void RestoreFully() => CurrentValue = MaxValue;
        public void Deplete(float amount) => CurrentValue -= amount;
        public void Restore(float amount) => CurrentValue += amount;
    }
}