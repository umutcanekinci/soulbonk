using UnityEngine;
using VectorViolet.Core.Stats;
using TMPro;
using System.Collections.Generic;

public class StatUpgradePanel : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private StatHolder[] statHolders;
    [SerializeField] private GameObject statEntryPrefab;
    [SerializeField] private Transform entriesContainer;

    private void Start()
    {
        if (statEntryPrefab == null) return;

        foreach (var statHolder in statHolders)
        {
            HashSet<UpgradeDefinition> createdUpgrades = new HashSet<UpgradeDefinition>();

            foreach (var stat in statHolder.statMap.Values)
            {
                UpgradeDefinition config = UpgradeManager.Instance.GetUpgradeConfig(stat.definition);
                
                if (config != null && !createdUpgrades.Contains(config))
                {
                    CreateStatEntry(config, statHolder);
                    createdUpgrades.Add(config);
                }
            }
        }
    }

    private void CreateStatEntry(UpgradeDefinition upgrade, StatHolder targetHolder)
    {
        if (statEntryPrefab == null || upgrade == null) return;

        GameObject entryObj = Instantiate(statEntryPrefab, entriesContainer);
        StatUpgradeEntry entryUI = entryObj.GetComponent<StatUpgradeEntry>();

        if (entryUI != null)
        {
            entryUI.Setup(upgrade, targetHolder);
        }
    }
}