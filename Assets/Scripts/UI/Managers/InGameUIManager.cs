using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using VectorViolet.Core.Utilities;

public class InGameUIManager : Singleton<InGameUIManager>
{
    [Header("References")]
    [SerializeField] GameObject statUI;
    [SerializeField] GameObject statUpgradeUI;  
    [SerializeField] GameObject controlUI;
    
    List<GameObject> uiElements;

    [Header("Settings")]
    [SerializeField] private bool hideControlsOnPC = true;
    
    protected override void Awake()
    {
        base.Awake();
        uiElements = new List<GameObject> { statUI, statUpgradeUI, controlUI };
        ShowOnly(controlUI);
    }

    private void ChangeUIState(GameState newState)
    {
        switch (newState)
        {
            case GameState.Gameplay:
                ShowOnly(controlUI);
                break;
            case GameState.Cutscene:
                HideAll();
                break;
            case GameState.Interaction:
                ShowOnly(statUpgradeUI);
                break;
            case GameState.Paused:
                ShowOnly(statUI);
                break;
        }
    }

    private void OnEnable()
    {
        GameManager.Instance.OnStateChanged += ChangeUIState;
    }

    private void OnDisable()
    {
        GameManager.Instance.OnStateChanged -= ChangeUIState;
    }
    
    private void ShowOnly(GameObject uiElement)
    {
        HideAll();
        if (uiElement == controlUI && !Application.isMobilePlatform && hideControlsOnPC)
            return;

        Show(uiElement);
    }

    private void HideAll()
    {
        uiElements.ForEach(element => Hide(element));
    }

    private void Show(GameObject uiElement) => StartCoroutine(EnableUINextFrame(uiElement));
    private void Hide(GameObject uiElement) => StartCoroutine(DisableUIEndOfFrame(uiElement));

    private IEnumerator EnableUINextFrame(GameObject uiElement)
    {
        yield return null;    
        uiElement.SetActive(true);
    }

    private IEnumerator DisableUIEndOfFrame(GameObject uiElement)
    {
        yield return new WaitForEndOfFrame();    
        uiElement.SetActive(false);
    }

}
