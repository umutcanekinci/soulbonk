using UnityEngine;
using TMPro;
public class InteractableUI : MonoBehaviour
{
    [SerializeField] private GameObject interactionUI;
    [SerializeField] private TextMeshProUGUI interactionText;

    public static InteractableUI Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }
        Hide();
    }

    public void Show(Interactable interactable)
    {
        interactionUI.SetActive(true);
        interactionText.text = interactable.interactionText;
        transform.position = interactable.GetUIPosition();
    }

    public void Hide()
    {
        interactionUI.SetActive(false);
    }

    public void SetInteractionText(string text)
    {
        interactionText.text = text;
    }

}
