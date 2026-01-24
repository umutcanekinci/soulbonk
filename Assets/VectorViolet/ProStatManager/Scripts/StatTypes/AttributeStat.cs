using System;
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
        
        public override float BaseValue 
        {
            get => _baseValue;
            protected set 
            {
                _baseValue = value;
                SetDirty();
            }
        }

        private void SetDirty()
        {
            _isDirty = true;
            InvokeValueChanged();
        }

        public override void OnInspectorChanged()
        {
            SetDirty();
        }

        public override float Value
        {
            get
            {
                if (_isDirty)
                {
                    _cachedValue = CalculateFinalValue();
                    _isDirty = false;
                }
                return _cachedValue;
            }
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
            SetDirty();
        }

        public bool RemoveModifier(StatModifier modifier)
        {
            if (modifiers.Remove(modifier))
            {
                SetDirty();
                return true;
            }
            return false;
        }

        public bool RemoveAllModifiersFromSource(object source)
        {
            int numRemoved = modifiers.RemoveAll(mod => mod.source == source);
            if (numRemoved > 0)
            {
                SetDirty();
                return true;
            }
            return false;
        }

    }
}