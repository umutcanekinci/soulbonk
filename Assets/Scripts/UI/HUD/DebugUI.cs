using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.UI;

public class DebugUI : MonoBehaviour
{
    [SerializeField] private bool showOnStart = true;

    [Header("UI References")]
    [SerializeField] private GameObject uiContainer;
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
        if (!showOnStart)
            return;

        UpdateText();
    }

    private void Update()
    {
        if (Keyboard.current.f2Key.wasPressedThisFrame)
        {
            ToggleDebugMode();
        }
    }

    private void ToggleDebugMode()
    {
        showOnStart = !showOnStart;
        uiContainer.SetActive(showOnStart);
        UpdateText();
    }

    private void UpdateText()
    {
        if (gameStateText == null || !showOnStart)
            return;

        gameStateText.text = $"Debug Mode: ON" + 
                             $"\nGame State: {GameManager.Instance.CurrentState}";
    }
}
