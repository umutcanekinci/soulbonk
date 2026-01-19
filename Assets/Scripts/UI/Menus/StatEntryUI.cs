using TMPro;
using UnityEngine;
using VectorViolet.Core.Stats;
using UnityEngine.UI;

public class StatEntryUI : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private UpgradeDefinition _upgradeDefinition;

    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI costText;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private Image iconRenderer;
    [SerializeField] private Button upgradeButton;

    private StatHolder _targetHolder;

    private void OnEnable()
    {
        if (CoinManager.Instance != null)
        {
            CoinManager.Instance.OnCoinAmountChange -= OnCoinChanged;

            if (_upgradeDefinition != null)
                UpdateButtonState();
        }
            
    }

    private void OnDisable()
    {
        if (CoinManager.Instance != null)
        {
            CoinManager.Instance.OnCoinAmountChange -= OnCoinChanged;
        }
    }

    public void Setup(UpgradeDefinition definition, StatHolder target)
    {
        _upgradeDefinition = definition;
        _targetHolder = target;

        if (iconRenderer != null) iconRenderer.sprite = definition.icon;
        if (nameText != null) nameText.text = definition.DisplayName;
        
        gameObject.name = "UpgradeEntry_" + definition.DisplayName;

        UpdateUI();
    }

    private void OnCoinChanged(int amount)
    {
        if (_upgradeDefinition != null)
            UpdateButtonState();
    }

    private void UpdateUI()
    {
        if (_upgradeDefinition == null) return;

        int level = UpgradeManager.Instance.GetCurrentLevel(_upgradeDefinition);
        int cost = UpgradeManager.Instance.GetNextCost(_upgradeDefinition);

        if (levelText != null) 
            levelText.text = $"Lvl {level}";

        if (costText != null) 
            costText.text = cost.ToString();

        UpdateButtonState();
    }

    private void UpdateButtonState()
    {
        int cost = UpgradeManager.Instance.GetNextCost(_upgradeDefinition);
        bool canAfford = CoinManager.Instance.IsEnoughCoins(cost);

        if (upgradeButton != null)
        {
            upgradeButton.interactable = canAfford;
        }

        if (costText != null)
        {
            costText.color = canAfford ? Color.white : Color.red;
        }
    }

    public void OnClickUpgrade()
    {
        int cost = UpgradeManager.Instance.GetNextCost(_upgradeDefinition);

        if (CoinManager.Instance.SpendCoins(cost))
        {
            UpgradeManager.Instance.Upgrade(_upgradeDefinition, _targetHolder);
            UpdateUI();
        }
    }
}