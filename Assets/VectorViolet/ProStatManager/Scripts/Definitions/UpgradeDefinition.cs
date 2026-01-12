using System.Collections.Generic;
using UnityEngine;

namespace VectorViolet.Core.Stats
{
    [CreateAssetMenu(menuName = "Game/Upgrade Definition")]
    public class UpgradeDefinition : ScriptableObject
    {
        [Header("Shop Settings")]
        public string upgradeID;      // Save için ID (örn: "DamageUpg")
        public string displayName;    // Ekranda görünecek isim
        public Sprite icon;           // Shop İkonu
        public int baseCost = 100;    // Başlangıç fiyatı
        public float costMultiplier = 1.5f; // Fiyat artış çarpanı

        [Header("Stat Modifiers")]
        // Senin yazdığın yapı, buraya entegre edildi
        public List<ModifierEntry> modifiers = new List<ModifierEntry>();

        [System.Serializable]
        public class ModifierEntry
        {   
            public StatDefinition targetStat; // Hangi stat değişecek?
            public ModifierType type = ModifierType.Flat;
            
            [Tooltip("Level 1'de verilecek bonus")]
            public float baseBonus;       
            
            [Tooltip("Her levelde üstüne eklenecek miktar")]
            public float growthPerLevel;  
        }

        // --- YARDIMCI STRUCT ---
        // Modifier'ı ve hedefini paketleyip döndürmek için
        public struct AppliedModifierInfo
        {
            public StatDefinition TargetStatDef;
            public StatModifier Modifier;
        }

        public List<AppliedModifierInfo> GenerateModifiers(int level, object source)
        {
            var result = new List<AppliedModifierInfo>();

            // Eğer level 0 ise hiç modifier verme
            if (level <= 0) return result;

            foreach (var entry in modifiers)
            {
                // Formül: BaseBonus + ((Level - 1) * Growth)
                float value = entry.baseBonus + ((level - 1) * entry.growthPerLevel);
                
                // Modifier'ı oluştur
                StatModifier mod = new StatModifier(value, entry.type, source);
                
                // Hedef stat ile birlikte paketle
                result.Add(new AppliedModifierInfo 
                { 
                    TargetStatDef = entry.targetStat, 
                    Modifier = mod 
                });
            }
            return result;
        }
    }
}