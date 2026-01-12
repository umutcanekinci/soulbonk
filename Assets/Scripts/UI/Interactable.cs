using UnityEngine;
using UnityEngine.Events;

public class Interactable : MonoBehaviour
{
    [SerializeField] private GameObject interactionUI;

    public void Interact()
    {
        EventBus.TriggerPlayerInteraction();

    }
    public void Deinteract()
    {
        EventBus.TriggerPlayerDeinteraction();
    }

    public void ShowInteractionUI()
    {
        interactionUI.SetActive(true);
    }

    public void HideInteractionUI()
    {
        interactionUI.SetActive(false);
    }
}