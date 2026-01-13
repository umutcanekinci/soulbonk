using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] GameObject interactionUI;
    public static UIManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable()
    {
        EventBus.OnPlayerDeinteraction += HideInteractionUI;
    }

    private void OnDisable()
    {
        EventBus.OnPlayerDeinteraction -= HideInteractionUI;
    }

    public void ShowInteractionUI()
    {
        ShowUIElement(interactionUI);
    }

    private void HideInteractionUI()
    {
        HideUIElement(interactionUI);
    }

    public void ShowUIElement(GameObject uiElement)
    {
        if (uiElement != null)
        {
            uiElement.SetActive(true);
        }
    }

    public void HideUIElement(GameObject uiElement)
    {
        if (uiElement != null)
        {
            uiElement.SetActive(false);
        }
    }
}
