using System.Collections.Generic;
using UnityEngine;

namespace VectorViolet.Core.Stats
{
    [CreateAssetMenu(menuName = "Game/Upgrade Definition")]
    public class UpgradeDefinition : ScriptableObject
    {
        [Header("Shop Settings")]
        public string upgradeID;      
        public string displayName;    
        public Sprite icon;           
        public int baseCost = 100;    
        public float costMultiplier = 1.5f; 

        [Header("Stat Modifiers")]
        
        public List<ModifierEntry> modifiers = new List<ModifierEntry>();

        [System.Serializable]
        public class ModifierEntry
        {   
            public StatDefinition targetStat; 
            public ModifierType type = ModifierType.Flat;
            
            [Tooltip("Level 1'de verilecek bonus")]
            public float baseBonus;       
            
            [Tooltip("Her levelde üstüne eklenecek miktar")]
            public float growthPerLevel;  
        }

        public struct AppliedModifierInfo
        {
            public StatDefinition TargetStatDef;
            public StatModifier Modifier;
        }

        public List<AppliedModifierInfo> GenerateModifiers(int level, object source)
        {
            var result = new List<AppliedModifierInfo>();

            if (level <= 0)
                return result;

            foreach (var entry in modifiers)
            {
                float value = entry.baseBonus + ((level - 1) * entry.growthPerLevel);
                StatModifier mod = new StatModifier(value, entry.type, source);
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