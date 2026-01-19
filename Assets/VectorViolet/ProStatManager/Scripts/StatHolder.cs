using System.Collections.Generic;
using UnityEngine;

namespace VectorViolet.Core.Stats
{
    public class StatHolder : MonoBehaviour
    {
        public StatCategory holderCategory;
        
        public List<AttributeStat> attributes = new List<AttributeStat>();
        public List<ResourceStat> resources = new List<ResourceStat>();
        
        public Dictionary<StatDefinition, StatBase> statMap = new Dictionary<StatDefinition, StatBase>();

        private void Awake()
        {
            InitializeStatMap();
        }

        private void InitializeStatMap()
        {
            statMap.Clear();

            foreach (var attr in attributes)
            {
                if (attr.definition != null && !statMap.ContainsKey(attr.definition))
                {
                    statMap.Add(attr.definition, attr);
                }
            }

            foreach (var res in resources)
            {
                if (res.definition != null && !statMap.ContainsKey(res.definition))
                {
                    statMap.Add(res.definition, res);
                }
            }
        }

        #if UNITY_EDITOR
        private void OnValidate()
        {
            if (!Application.isPlaying)
                return;

            if (attributes != null)
            {
                foreach (var attr in attributes)
                {
                    if (attr != null)
                        attr.OnInspectorChanged();
                }
            }

            if (resources != null)
            {
                foreach (var res in resources)
                {
                    if (res != null)
                        res.OnInspectorChanged();
                }
            }
        }
        #endif

        public StatBase GetStat(string statName)
        {
            if (statMap != null && statMap.Count > 0)
            {
                foreach (var pair in statMap)
                {
                    if (pair.Key.name == statName || pair.Key.DisplayName == statName)
                    {
                        return pair.Value;
                    }
                }
            }

            if (attributes != null)
            {
                foreach (var attr in attributes)
                {
                    if (attr.definition != null && 
                       (attr.definition.name == statName || attr.definition.DisplayName == statName))
                    {
                        return attr;
                    }
                }
            }

            if (resources != null)
            {
                foreach (var res in resources)
                {
                    if (res.definition != null && 
                       (res.definition.name == statName || res.definition.DisplayName == statName))
                    {
                        return res;
                    }
                }
            }

            return null;
        }

        public StatBase GetStat(StatDefinition def)
        {
            if (def != null && statMap.TryGetValue(def, out StatBase s))
            {
                Debug.Assert(s != null, $"StatHolder on {name} has a null stat for definition {def.name}!");
                return s;
            }
            return null;
        }

        public void AddModifier(StatDefinition def, StatModifier mod)
        {
            if (GetStat(def) is AttributeStat attr)
            {
                attr.AddModifier(mod);
            }
        }

        public bool RemoveModifier(StatDefinition def, StatModifier mod) 
        {
            return GetStat(def) is AttributeStat attr && attr.RemoveModifier(mod);
        }

        public bool RemoveAllModifiersFromSource(StatDefinition def, object source)
        {
            return GetStat(def) is AttributeStat attr && attr.RemoveAllModifiersFromSource(source);
        }

        public void ApplyModifierPackage(ModifierDefinition package, object source, int level = 1)
        {
            if (package == null)
            {
                return;
            }
                
            var newModifiers = package.CreateModifiers(level, source);

            for (int i = 0; i < package.entries.Count; i++)
            {
                if (i >= newModifiers.Count)
                {
                    break;
                }

                StatDefinition targetStat = package.entries[i].targetStat;
                StatModifier mod = newModifiers[i];

                AddModifier(targetStat, mod);
            }
        }

        public void RemoveModifierPackage(ModifierDefinition package, object source)
        {
            if (package == null)
            {
                return;
            }

            foreach (var entry in package.entries)
            {
                if (entry.targetStat != null)
                {
                    RemoveAllModifiersFromSource(entry.targetStat, source);
                }
            }
        }
        
        private void OnDrawGizmosSelected()
        {
            foreach (var attr in attributes)
            {
                if (attr.definition == null)
                    continue;
                
                StatDefinition def = attr.definition;
                if (!def.isRangedStat)
                    continue;
                    
                Gizmos.color = def.gizmosColor;
                Gizmos.DrawWireSphere(transform.position, attr.GetValue());
            }
        }
    }
}