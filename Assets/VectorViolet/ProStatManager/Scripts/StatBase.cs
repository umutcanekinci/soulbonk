using System;

namespace VectorViolet.Core.Stats
{
    [Serializable]
    public abstract class StatBase
    {
        public StatDefinition definition;
        public string StatName => definition != null ? definition.DisplayName : "Unknown";
        public event Action<StatBase> OnValueChanged;

        public abstract float GetValue();
        public abstract void OnInspectorChanged();
        protected void InvokeValueChanged()
        {
            OnValueChanged?.Invoke(this);
        }
        
    }
}