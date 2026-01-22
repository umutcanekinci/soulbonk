using System.Collections.Generic;
using UnityEngine;
using VectorViolet.Core.Attributes;

namespace VectorViolet.Core.Stats
{
    [CreateAssetMenu(menuName = "Pro Stat Manager/Upgrade Definition")]
    public class UpgradeDefinition : ScriptableObject
    {
        [Header("Shop Settings")]
        public string upgradeID => name;      
        [SerializeField] private string displayName;    
        public string DisplayName => string.IsNullOrEmpty(displayName) ? name : displayName;
        [SpritePreview]
        [SerializeField] private Sprite icon;           
        public Sprite Icon => icon != null ? icon : modifiers.Count > 0 && modifiers[0].targetStat != null ? modifiers[0].targetStat.icon : null;
        public int baseCost = 100;    
        public float costMultiplier = 1.5f; 

        [Header("Stat Modifiers")]
        
        public List<ModifierEntry> modifiers = new List<ModifierEntry>();

        [System.Serializable]
        public class ModifierEntry
        {   
            public StatDefinition targetStat; 
            public ModifierType type = ModifierType.Flat;
            public float baseBonus;
            public float growthPerLevel;  
        }

        public int GetCost(int currentLevel)
        {
            float multiplier = Mathf.Pow(costMultiplier, currentLevel);
            return Mathf.RoundToInt(baseCost * multiplier);
        }

        public List<StatModifier> GenerateModifiersForLevel(int level)
        {
            var result = new List<StatModifier>();

            if (level <= 0)
                return result;

            foreach (var entry in modifiers)
            {
                float value = entry.baseBonus + ((level - 1) * entry.growthPerLevel);
                StatModifier mod = new StatModifier(value, entry.type, this);
                result.Add(mod);
            }
            return result;
        }
    }
}