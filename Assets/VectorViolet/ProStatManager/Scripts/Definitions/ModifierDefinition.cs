using System;
using System.Collections.Generic;
using UnityEngine;

namespace VectorViolet.Core.Stats
{
    [CreateAssetMenu(menuName = "Pro Stat Manager/Modifier Package")]
    public class ModifierDefinition : ScriptableObject
    {
        [System.Serializable]
        public class ModifierEntry
        {   
            public StatDefinition targetStat; // Hangi stat değişecek?
            public ModifierType type = ModifierType.Flat;
            
            [Header("Values")]
            public float baseValue;       // Level 1 değeri
            public float growthPerLevel;  // Her levelda artış miktarı
        }

        // Bir paket birden fazla statı etkileyebilir (Örn: Heavy Armor -> +Def, -Speed)
        public List<ModifierEntry> entries = new List<ModifierEntry>();

        // Bu paketin içindeki tüm verileri, verilen level'a göre Runtime Modifier'a çevirir
        public List<StatModifier> CreateModifiers(int level, object source)
        {
            List<StatModifier> result = new List<StatModifier>();

            foreach (var entry in entries)
            {
                // Formül: Base + ((Level - 1) * Growth)
                float val = entry.baseValue + ((level - 1) * entry.growthPerLevel);
                
                // Runtime modifier oluştur
                result.Add(new StatModifier(val, entry.type, source));
            }

            return result;
        }
    }
}