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
            public StatDefinition targetStat; 
            public ModifierType type = ModifierType.Flat;
            
            [Header("Values")]
            public float baseValue;       
            public float growthPerLevel;  
        }

        
        public List<ModifierEntry> entries = new List<ModifierEntry>();

        
        public List<StatModifier> CreateModifiers(int level, object source)
        {
            List<StatModifier> result = new List<StatModifier>();

            foreach (var entry in entries)
            {
                
                float val = entry.baseValue + ((level - 1) * entry.growthPerLevel);
                
                
                result.Add(new StatModifier(val, entry.type, source));
            }

            return result;
        }
    }
}