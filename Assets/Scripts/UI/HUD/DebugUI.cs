using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.UI;

public class DebugUI : MonoBehaviour
{
    [SerializeField] private bool isDebugMode = true;

    [Header("UI References")]
    [SerializeField] private Image backgroundImage;
    [SerializeField] private TMP_Text gameStateText;
    
    private void OnEnable()
    {
        GameManager.Instance.OnStateChanged += UpdateGameStateText;
        UpdateGameStateText(GameManager.Instance.CurrentState);
    }

    private void OnDisable()
    {
        GameManager.Instance.OnStateChanged -= UpdateGameStateText;
    }

    private void UpdateGameStateText(GameState newState)
    {
        if (!isDebugMode)
            return;

        UpdateText();
    }

    private void Update()
    {
        if (Keyboard.current.f2Key.wasPressedThisFrame)
        {
            isDebugMode = !isDebugMode;
            gameStateText.gameObject.SetActive(isDebugMode);
            backgroundImage.enabled = isDebugMode;
            UpdateText();
        }
    }

    private void UpdateText()
    {
        if (gameStateText == null || !isDebugMode)
            return;

        gameStateText.text = $"Debug Mode: ON" + 
                             $"\nGame State: {GameManager.Instance.CurrentState}";
    }
}
