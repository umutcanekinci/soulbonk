using UnityEngine;

[RequireComponent(typeof(TMPro.TextMeshProUGUI))]
public class CoinUpdate : MonoBehaviour
{
    private TMPro.TextMeshProUGUI coinText;

    private void Awake()
    {
        coinText = GetComponent<TMPro.TextMeshProUGUI>();
        if (CoinManager.Instance != null)
        {
            UpdateCoinText(CoinManager.CurrentCoins);
        }
    }

    private void OnEnable()
    {
        if (CoinManager.Instance != null)
        {
            CoinManager.Instance.OnCoinAmountChange += UpdateCoinText;
        }
    }

    private void OnDisable()
    {
        if (CoinManager.Instance != null)
        {
            CoinManager.Instance.OnCoinAmountChange -= UpdateCoinText;
        }
    }

    private void UpdateCoinText(int amount)
    {
        coinText.text = amount.ToString();
    }

}