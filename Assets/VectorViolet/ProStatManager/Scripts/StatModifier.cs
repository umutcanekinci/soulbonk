using System;

namespace VectorViolet.Core.Stats
{
    public enum ModifierType
    {
        Flat = 100,      // Düz ekleme (Örn: +10 Defans)
        PercentAdd = 200 // Yüzdesel ekleme (Örn: +%10 Güç)
    }

    [Serializable]
    public class StatModifier
    {
        public float value;
        public ModifierType type;
        public object source; // Bu modifier kimden geldi? (Örn: "Iron Sword", "PoisonSpell")

        // Constructor - Hızlı oluşturmak için
        public StatModifier(float value, ModifierType type, object source = null)
        {
            this.value = value;
            this.type = type;
            this.source = source;
        }
    }
}