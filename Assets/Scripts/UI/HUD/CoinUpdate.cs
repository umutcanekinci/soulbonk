using UnityEngine;

[RequireComponent(typeof(TMPro.TextMeshProUGUI))]
public class CoinUpdate : MonoBehaviour
{
    private TMPro.TextMeshProUGUI coinText;

    private void Start()
    {
        coinText = GetComponent<TMPro.TextMeshProUGUI>();
        if (CoinManager.Instance != null)
        {
            CoinManager.Instance.OnCoinCollected += UpdateCoinText;
            UpdateCoinText(0);
        }
    }

    private void OnDestroy()
    {
        if (CoinManager.Instance != null)
        {
            CoinManager.Instance.OnCoinCollected -= UpdateCoinText;
        }
    }

    private void UpdateCoinText(int amount)
    {
        coinText.text = amount.ToString();
    }

}