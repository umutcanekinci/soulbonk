using TMPro;
using UnityEngine;
using VectorViolet.Core.Stats;
using UnityEngine.UI;

public class StatEntry : MonoBehaviour
{
    [Header("Stat References")]
    [SerializeField] private StatHolder _targetHolder;
    [SerializeField] private StatDefinition _statDefinition;
    private StatBase _runtimeStat; // Store reference to the runtime stat

    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI valueText;
    [SerializeField] private Image iconRenderer;
    [SerializeField] private Hoverable hoverableComponent;

    private void OnEnable()
    {
        // If we already have a reference, update the UI
        if (_runtimeStat != null)
        {
            UpdateUI();
        }
    }

    private void OnDisable()
    {
        // Unsubscribe to prevent memory leaks
        if (_runtimeStat != null)
        {
            _runtimeStat.OnValueChanged -= OnStatChanged;
        }
    }

    public void Setup(StatDefinition definition, StatHolder target)
    {
        _statDefinition = definition;
        _targetHolder = target;

        if (iconRenderer != null) iconRenderer.sprite = definition.icon;
        if (nameText != null) nameText.text = definition.DisplayName;
        
        gameObject.name = "StatEntry_" + definition.DisplayName;

        if (hoverableComponent != null)
        {
            hoverableComponent.hoverTooltip = definition.DisplayName;
        }

        // Get the runtime stat instance
        _runtimeStat = _targetHolder.GetStat(_statDefinition);

        if (_runtimeStat != null)
        {
            // Subscribe to changes so UI updates automatically when stats change (e.g. equipping weapon)
            _runtimeStat.OnValueChanged += OnStatChanged;
            UpdateUI();
        }
    }

    private void OnStatChanged(StatBase stat)
    {
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (_runtimeStat == null) return;

        float finalValue = _runtimeStat.Value;
        float baseValue = _runtimeStat.BaseValue; // Ensure BaseValue is public in StatBase

        if (valueText != null)
        {
            // Check if there is a difference between base and final
            if (Mathf.Abs(finalValue - baseValue) > 0.01f)
            {
                float bonus = finalValue - baseValue;
                int baseInt = Mathf.RoundToInt(baseValue);
                int bonusInt = Mathf.RoundToInt(bonus);

                string colorHex = bonus > 0 ? "#00FF00" : "#FF0000"; // Green or Red
                string sign = bonus > 0 ? "+" : ""; // Negative numbers have sign automatically

                // Format: "10 (+5)"
                valueText.text = $"{baseInt} <color={colorHex}>({sign}{bonusInt})</color>";
            }
            else
            {
                // No bonus, just show the value
                valueText.text = Mathf.RoundToInt(finalValue).ToString();
            }
        }
    }
}