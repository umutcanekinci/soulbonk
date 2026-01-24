using System;

namespace VectorViolet.Core.Stats
{
    [Serializable]
    public abstract class StatBase
    {
        public StatDefinition definition;
        public string StatName => definition != null ? definition.DisplayName : "Unknown";
        public event Action<StatBase> OnValueChanged;

        public abstract float Value { get;}

        /// <summary>
        /// For the attribute stats, it is the base value of the stat before any modifiers are applied.
        /// For resource stats, it represents the maximum value of the resource.
        /// </summary>
        public abstract float BaseValue { get; protected set; }
        public abstract void OnInspectorChanged();
        protected void InvokeValueChanged()
        {
            OnValueChanged?.Invoke(this);
        }
        
    }
}