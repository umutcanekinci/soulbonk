using UnityEngine;
using VectorViolet.Core.Stats;
using TMPro;
using System.Collections.Generic;

public class StatPanel : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private GameObject statEntryPrefab;
    [SerializeField] private Transform entriesContainer;
    [SerializeField] private WeaponController weaponController;
    
    public void OnEnable()
    {
        if (weaponController != null)
        {
            weaponController.OnWeaponSwitched += Redraw;
            Redraw(weaponController.GetStatHolders());    
        }
    }

    public void OnDisable()
    {
        if (weaponController != null)
        {
            weaponController.OnWeaponSwitched -= Redraw;    
        }
    }

    private void Redraw(List<StatHolder> holders)
    {
        ResetPanel();
        Initialize(holders);
    }

    private void ResetPanel()
    {
        foreach (Transform child in entriesContainer)
        {
            Destroy(child.gameObject);
        }
    }

    private void Initialize(List<StatHolder> holders)
    {
        if (statEntryPrefab == null) return;

        foreach (var statHolder in holders)
        {
            if (statHolder == null)
                continue;

            HashSet<StatDefinition> createdStats = new HashSet<StatDefinition>();
            
            foreach (var stat in statHolder.statMap.Values)
            {
                StatDefinition config = stat.definition;
                if (config != null && !createdStats.Contains(config))
                {
                    CreateStatEntry(config, statHolder);
                    createdStats.Add(config);
                }
            }
        }
    }

    private void CreateStatEntry(StatDefinition statDefinition, StatHolder targetHolder)
    {
        if (statEntryPrefab == null || statDefinition == null) return;

        GameObject entryObj = Instantiate(statEntryPrefab, entriesContainer);
        StatEntry entryUI = entryObj.GetComponent<StatEntry>();

        if (entryUI != null)
        {
            entryUI.Setup(statDefinition, targetHolder);
        }
    }
}