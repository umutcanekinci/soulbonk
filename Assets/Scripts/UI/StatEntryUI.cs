using TMPro;
using UnityEngine;
using VectorViolet.Core.Stats;
using UnityEngine.UI;

public class StatEntryUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI statNameText;
    [SerializeField] private TextMeshProUGUI statValueText;
    [SerializeField] private Image statIconRenderer;

    public void Setup(StatBase stat)
    {
        gameObject.name = "StatEntry_" + stat.definition.DisplayName;
        if (statNameText != null)
        {
            if (stat.definition != null)
                statNameText.text = stat.definition.DisplayName;
            else
                statNameText.text = "Unknown Stat";
        }
            

        if (statValueText != null)
            statValueText.text = stat.GetValue().ToString();

        if (statIconRenderer != null && stat.definition.icon != null)
             statIconRenderer.sprite = stat.definition.icon;
    }
}
