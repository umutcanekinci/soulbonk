using Unity.VisualScripting;
using UnityEngine;

public class InGameUIManager : MonoBehaviour
{
    [SerializeField] GameObject interactionUI;
    [SerializeField] GameObject controlUI;

    private void OnEnable()
    {
        GameManager.Instance.OnGameStateChanged += ChangeUIState;
    }

    private void OnDisable()
    {
        GameManager.Instance.OnGameStateChanged -= ChangeUIState;
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

    public void OnMenuButtonPressed() => SceneLoader.Load(SceneType.Menu);

    private void ShowControlUI() => ShowUIElement(controlUI);
    private void HideControlUI() => HideUIElement(controlUI);
    private void ShowInteractionUI() => ShowUIElement(interactionUI);
    private void HideInteractionUI() => HideUIElement(interactionUI);
    private void ShowUIElement(GameObject uiElement) => uiElement?.SetActive(true);
    private void HideUIElement(GameObject uiElement) => uiElement?.SetActive(false);

}
