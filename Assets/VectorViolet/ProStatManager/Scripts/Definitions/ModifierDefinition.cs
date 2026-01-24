using System;
using System.Collections.Generic;
using UnityEngine;

namespace VectorViolet.Core.Stats
{
    [CreateAssetMenu(menuName = "Pro Stat Manager/Modifier Package")]
    public class ModifierDefinition : ScriptableObject
    {
        [Serializable]
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
            return entries.ConvertAll(entry => 
                new StatModifier(
                    entry.baseValue + ((level - 1) * entry.growthPerLevel), 
                    entry.type, 
                    source
                )
            );
        }
    }
}