using UnityEngine;

namespace VectorViolet.Core.Stats
{
    public enum StatType { Attribute, Resource }
    
    [CreateAssetMenu(menuName = "Pro Stat Manager/Stat Definition")]
    public class StatDefinition : ScriptableObject
    {
        public string ID => this.name; 
        [SerializeField] private string displayName;
        [TextArea] public string description;
        public Sprite icon;
        public bool isRangedStat = false;
        public Color gizmosColor = Color.white;

        public StatType type = StatType.Attribute;

        public string DisplayName 
        {
            get 
            {
                if (string.IsNullOrEmpty(displayName))
                    return ID;
                return displayName;
            }
        }
        
        // Editörde kolaylık olsun diye, oluşturulduğunda otomatik doldur
        private void Reset()
        {
            displayName = this.name;
        }
        
        // Editör kolaylığı: Dosya oluşturulunca otomatik isimlendirme denemesi
        private void OnValidate()
        {
            // Dosya isminde "Stat_" gibi prefixler varsa onları temizleyebilirsin
            // Örn dosya adı: "Stat_Health" -> Görünen ad: "Health" olsun istersen:
            /*
            if (string.IsNullOrEmpty(_statNameOverride))
            {
                _statNameOverride = this.name.Replace("Stat_", "");
            }
            */
        }
    }
}