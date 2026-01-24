using UnityEngine;
using VectorViolet.Core.Attributes;
using System.Collections.Generic;
using NaughtyAttributes;

namespace VectorViolet.Core.Stats
{
    public enum StatType { Attribute, Resource }
    
    [CreateAssetMenu(menuName = "Pro Stat Manager/Stat Definition")]
    public class StatDefinition : ScriptableObject
    {
        [Header("General Info")]
        [SerializeField] private string displayName;
        [TextArea] public string description;
        public StatType type = StatType.Attribute;
        public List<StatCategory> categories = new List<StatCategory>();

        [Header("Visuals")]
        [SpritePreview]
        public Sprite icon;
        public bool isRangedStat = false;
        [ShowIf("isRangedStat")]
        public Color gizmosColor = Color.white;

        public string ID => name; 
        public string DisplayName => string.IsNullOrEmpty(displayName) ? name : displayName;
        
        private void Reset()
        {
            displayName = this.name;
        }
    }
}