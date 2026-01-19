using UnityEngine;
using System.Collections.Generic;

namespace VectorViolet.Core.Stats
{
    public class UpgradeManager : MonoBehaviour
    {
        public static UpgradeManager Instance;

        [SerializeField] private List<UpgradeDefinition> allUpgrades;

        private Dictionary<StatDefinition, UpgradeDefinition> _statUpgradeMap = new Dictionary<StatDefinition, UpgradeDefinition>();
        private Dictionary<UpgradeDefinition, int> _upgradeLevels = new Dictionary<UpgradeDefinition, int>();

        private void Awake()
        {
            if (Instance == null) { Instance = this; DontDestroyOnLoad(gameObject); }
            else { Destroy(gameObject); return; }

            LoadAllUpgrades();
            InitializeLookups();
        }

        private void LoadAllUpgrades()
        {
            if (allUpgrades == null || allUpgrades.Count == 0)
                allUpgrades = new List<UpgradeDefinition>(Resources.LoadAll<UpgradeDefinition>("Upgrades"));
        }

        private void InitializeLookups()
        {
            foreach (var upgrade in allUpgrades)
            {
                if (upgrade.modifiers.Count > 0)
                {
                    StatDefinition mainTarget = upgrade.modifiers[0].targetStat;
                    if (!_statUpgradeMap.ContainsKey(mainTarget))
                    {
                        _statUpgradeMap.Add(mainTarget, upgrade);
                    }
                }
            }
        }

        public UpgradeDefinition GetUpgradeConfig(StatDefinition statDef)
        {
            if (_statUpgradeMap.TryGetValue(statDef, out UpgradeDefinition upgrade)) return upgrade;
            return null;
        }

        public int GetCurrentLevel(UpgradeDefinition upgrade)
        {
            if (_upgradeLevels.ContainsKey(upgrade)) return _upgradeLevels[upgrade];
            return 0;
        }

        public int GetNextCost(UpgradeDefinition upgrade)
        {
            int currentLvl = GetCurrentLevel(upgrade);
            return upgrade.GetCost(currentLvl);
        }

        public void Upgrade(UpgradeDefinition upgrade, StatHolder target)
        {
            if (target == null) return;

            int currentLvl = GetCurrentLevel(upgrade);

            if (currentLvl > 0)
            {
                RemoveUpgradeModifiers(upgrade, target);
            }

            int newLevel = currentLvl + 1;
            _upgradeLevels[upgrade] = newLevel;

            ApplyUpgradeModifiers(upgrade, newLevel, target);
        }

        private void ApplyUpgradeModifiers(UpgradeDefinition upgrade, int level, StatHolder target)
        {
            List<StatModifier> newModifiers = upgrade.GenerateModifiersForLevel(level);

            for (int i = 0; i < upgrade.modifiers.Count; i++)
            {
                 var entry = upgrade.modifiers[i];
                 var modifierToApply = newModifiers[i];

                 var stat = target.GetStat(entry.targetStat.ID);
                 if (stat is AttributeStat attrStat)
                 {
                     attrStat.AddModifier(modifierToApply);
                 }
            }
        }

        private void RemoveUpgradeModifiers(UpgradeDefinition upgrade, StatHolder target)
        {
            foreach (var entry in upgrade.modifiers)
            {
                var stat = target.GetStat(entry.targetStat.ID);
                if (stat is AttributeStat attrStat)
                {
                    attrStat.RemoveAllModifiersFromSource(upgrade);
                }
            }
        }
    }
}