using UnityEngine;
using UnityEngine.UI;
using VectorViolet.Core.Stats;
using TMPro;
public class StatHolderUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private StatHolder[] statHolders;
    [SerializeField] private GameObject statEntryPrefab;

    private void Start()
    {
        if (statEntryPrefab == null)
            return;
        foreach (var statHolder in statHolders)
        {
            foreach (var stat in statHolder.statMap.Values)
            {
                CreateStatEntry(stat);
            }
        }
    }

    private void CreateStatEntry(StatBase stat)
    {
        //titleText.text = statHolder.gameObject.name + " Stats";
        GameObject entryObj = Instantiate(statEntryPrefab, transform);
        StatEntryUI entryUI = entryObj.GetComponent<StatEntryUI>();
        if (entryUI != null)
        {
            entryUI.Setup(stat);
        }
        else
        {
            Debug.LogError("StatEntryUI component not found on the instantiated prefab.");
        }
    }

    // TODO
    // public void UpgradeStat(string statName, float amount)
    // {
    //     foreach (var statHolder in statHolders)
    //     {
    //         if (statHolder.statMap.TryGetValue(statName, out StatBase stat))
    //         {
    //             stat.ModifyBaseValue(amount);
    //             return;
    //         }
    //     }
    //     Debug.LogWarning($"Stat '{statName}' not found in any StatHolder.");
    // }
}
