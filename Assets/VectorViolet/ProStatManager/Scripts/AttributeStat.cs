using System; // Math sınıfı için gerekli
using System.Collections.Generic;
using UnityEngine;

namespace VectorViolet.Core.Stats
{
    [Serializable]
    public class AttributeStat : StatBase
    {
        [SerializeField] private float _baseValue;
        private bool _isDirty = true;
        private float _cachedValue;
        private readonly List<StatModifier> modifiers = new List<StatModifier>();
        
        public float BaseValue 
        {
            get => _baseValue;
            set 
            {
                _baseValue = value;
                _isDirty = true;
                InvokeValueChanged();
            }
        }

        public override void OnInspectorChanged()
        {
            _isDirty = true;
            InvokeValueChanged();
        }

        public override float GetValue()
        {
            if (_isDirty)
            {
                _cachedValue = CalculateFinalValue();
                _isDirty = false;
            }
            return _cachedValue;
        }

        private float CalculateFinalValue()
        {
            float finalValue = _baseValue;
            float sumPercentAdd = 0;

            for (int i = 0; i < modifiers.Count; i++)
            {
                StatModifier mod = modifiers[i];
                if (mod.type == ModifierType.Flat) {
                    finalValue += mod.value;
                }
                else if (mod.type == ModifierType.PercentAdd) {
                    sumPercentAdd += mod.value;
                }
            }

            finalValue *= 1 + sumPercentAdd;

            return (float)Math.Round(finalValue, 2);
        }

        public void AddModifier(StatModifier modifier)
        {
            modifiers.Add(modifier);
            _isDirty = true;
        }

        public bool RemoveModifier(StatModifier modifier)
        {
            if (modifiers.Remove(modifier))
            {
                _isDirty = true;
                return true;
            }
            return false;
        }

        public bool RemoveAllModifiersFromSource(object source)
        {
            int numRemoved = modifiers.RemoveAll(mod => mod.source == source);
            if (numRemoved > 0)
            {
                _isDirty = true;
                return true;
            }
            return false;
        }

    }
}