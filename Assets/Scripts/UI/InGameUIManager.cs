using UnityEngine;
using UnityEngine.InputSystem;

public class InGameUIManager : MonoBehaviour
{

    [Header("References")]
    [SerializeField] GameObject interactionUI;
    [SerializeField] GameObject controlUI;

    [Header("Settings")]
    [SerializeField] private bool hideControlOnStart = true;

    private InputAction exitAction;

    private void Awake()
    {
        HideInteractionUI();
        if (Application.isMobilePlatform)
            ShowControlUI();
        else
            if (hideControlOnStart)
                HideControlUI();

        exitAction = new InputAction("Exit", binding: "<Gamepad>/buttonNorth");
        exitAction.AddBinding("<Keyboard>/escape");
        exitAction.performed += _ => SceneLoader.Load(SceneType.Menu);

    }

    private void OnEnable()
    {
        exitAction.Enable();
        GameManager.Instance.OnStateChanged += ChangeUIState;
    }

    private void OnDisable()
    {
        exitAction.Disable();
        GameManager.Instance.OnStateChanged -= ChangeUIState;
    }

    private void ChangeUIState(GameState newState)
    {
        switch (newState)
        {
            case GameState.Gameplay:
                ShowControlUI();
                HideInteractionUI();
                break;
            case GameState.Cutscene:
                HideControlUI();
                HideInteractionUI();
                break;
            case GameState.Interaction:
                HideControlUI();
                ShowInteractionUI();
                break;
        }
    }

    private void ShowControlUI() => ShowUIElement(controlUI);
    private void HideControlUI() => HideUIElement(controlUI);
    private void ShowInteractionUI() => ShowUIElement(interactionUI);
    private void HideInteractionUI() => HideUIElement(interactionUI);
    private void ShowUIElement(GameObject uiElement) => uiElement?.SetActive(true);
    private void HideUIElement(GameObject uiElement) => StartCoroutine(DisableUIEndOfFrame(uiElement));

    private System.Collections.IEnumerator DisableUIEndOfFrame(GameObject uiElement)
    {
        yield return new WaitForEndOfFrame();    
        uiElement.SetActive(false);
    }

}
