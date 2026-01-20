using System;

namespace VectorViolet.Core.Stats
{
    public enum ModifierType
    {
        Flat = 100,     
        PercentAdd = 200
    }

    [Serializable]
    public class StatModifier
    {
        public float value;
        public ModifierType type;
        public object source;

    
        public StatModifier(float value, ModifierType type, object source = null)
        {
            this.value = value;
            this.type = type;
            this.source = source;
        }
    }
}