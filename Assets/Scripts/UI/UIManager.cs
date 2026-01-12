using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] GameObject interactionUI;
    public static UIManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(this.gameObject);
    }

    public void ShowInteractionUI()
    {
        ShowUIElement(interactionUI);
    }

    public void HideInteractionUI()
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
