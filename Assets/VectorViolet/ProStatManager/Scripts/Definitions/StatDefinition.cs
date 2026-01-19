using UnityEngine;
using VectorViolet.Core.Attributes;

namespace VectorViolet.Core.Stats
{
    public enum StatType { Attribute, Resource }
    
    [CreateAssetMenu(menuName = "Pro Stat Manager/Stat Definition")]
    public class StatDefinition : ScriptableObject
    {
        public string ID => name; 
        [SerializeField] private string displayName;
        [TextArea] public string description;

        [SpritePreview]
        public Sprite icon;
        public bool isRangedStat = false;
        public Color gizmosColor = Color.white;

        public StatType type = StatType.Attribute;

        public string DisplayName => string.IsNullOrEmpty(displayName) ? name : displayName;
        
        private void Reset()
        {
            displayName = this.name;
        }
        
        private void OnValidate()
        {
            /*
            if (string.IsNullOrEmpty(_statNameOverride))
            {
                _statNameOverride = this.name.Replace("Stat_", "");
            }
            */
        }
    }
}