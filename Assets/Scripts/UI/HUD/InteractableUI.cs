using UnityEngine;
using TMPro;
using VectorViolet.Core.Utilities;

public class InteractableUI : Singleton<InteractableUI>
{
    [SerializeField] private GameObject interactionUI;
    [SerializeField] private TextMeshProUGUI interactionText;
    protected override bool DontDestroy => false;

    protected override void Awake()
    {
        base.Awake();
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
