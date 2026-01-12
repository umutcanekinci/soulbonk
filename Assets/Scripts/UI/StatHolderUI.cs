using UnityEngine;
using UnityEngine.UI;
using VectorViolet.Core.Stats;
using TMPro;
public class StatHolderUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private StatHolder statHolder;
    [SerializeField] private GameObject statEntryPrefab;

    private void Start()
    {
        if (statEntryPrefab == null)
            return;

        foreach (var stat in statHolder.statMap.Values)
        {
            CreateStatEntry(stat);
        }
    }

    private void CreateStatEntry(StatBase stat)
    {
        titleText.text = statHolder.gameObject.name + " Stats";
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
}
