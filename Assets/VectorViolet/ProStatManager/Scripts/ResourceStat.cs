using System;
using UnityEngine;

namespace VectorViolet.Core.Stats
{
    [Serializable]
    public class ResourceStat : StatBase
    {
        [SerializeField] private float _currentValue;
        
        // MaxValue genelde bir Attribute'a (Vitality) bağlı olabilir ama şimdilik manuel kalsın.
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

        // Use the setter for inspector changes to clamp value and invoke event
        public override void OnInspectorChanged()
        {
            CurrentValue = _currentValue; 
        }

        public override float GetValue() => _currentValue;
        
        public void RestoreFully() => CurrentValue = MaxValue;
        public void Deplete(float amount) => CurrentValue -= amount;
        public void Restore(float amount) => CurrentValue += amount;
    }
}