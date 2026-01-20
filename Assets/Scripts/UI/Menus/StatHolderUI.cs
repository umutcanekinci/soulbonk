using UnityEngine;
using UnityEngine.UI;
using VectorViolet.Core.Stats;
using TMPro;
using System.Collections.Generic;
using Unity.VisualScripting;

public class StatHolderUI : MonoBehaviour
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
        StatEntryUI entryUI = entryObj.GetComponent<StatEntryUI>();

        if (entryUI != null)
        {
            entryUI.Setup(upgrade, targetHolder);
        }
    }
}