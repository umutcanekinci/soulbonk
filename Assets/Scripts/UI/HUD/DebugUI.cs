using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class DebugUI : MonoBehaviour
{
    [SerializeField] private bool showOnStart = true;

    [Header("UI References")]
    [SerializeField] private GameObject uiContainer;
    [SerializeField] private TMP_Text gameStateText;
    [SerializeField] private WeaponController playerWeaponController;

    private bool isEnabled = false;
    
    private void OnEnable()
    {
        GameManager.Instance.OnStateChanged += UpdateGameStateText;
        SetEnabled(showOnStart);
        UpdateGameStateText(GameManager.Instance.CurrentState);
    }

    private void OnDisable()
    {
        GameManager.Instance.OnStateChanged -= UpdateGameStateText;
    }

    private void UpdateGameStateText(GameState newState)
    {
        if (!isEnabled)
            return;

        UpdateText();
    }

    private void Update()
    {
        if (Keyboard.current.f1Key.wasPressedThisFrame)
        {
            ToggleDebugMode();
        }
    }

    private void ToggleDebugMode()
    {
        SetEnabled(!isEnabled);
    }

    private void SetEnabled(bool enabled)
    {
        isEnabled = enabled;
        uiContainer.SetActive(isEnabled);
        UpdateText();
    }

    private void UpdateText()
    {
        if (gameStateText == null || !isEnabled)
            return;

        gameStateText.text = $"Debug Mode: ON" + 
                             $"\nGame State: {GameManager.Instance.CurrentState}";
        
        if (playerWeaponController != null)
        {
            gameStateText.text += $"\nPlayer Weapon: {playerWeaponController.CurrentWeapon?.name ?? "None"}" +
                                  $"\nWeapon Damage: {playerWeaponController.CurrentWeapon?.AttackDamage ?? 0}" +
                                  $"\nWeapon Range: {playerWeaponController.CurrentWeapon?.AttackRange ?? 0}" +
                                  $"\nWeapon Speed: {playerWeaponController.CurrentWeapon?.AttackSpeed ?? 0}";
        }

    }
}
