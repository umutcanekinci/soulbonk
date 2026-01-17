using UnityEngine;
using TMPro;
using System.Collections;

public abstract class Interactable : MonoBehaviour
{
    [SerializeField] private GameObject interactionUI;
    [SerializeField] private TextMeshProUGUI interactionText;
    [SerializeField] private Vector2 interactionPoint;
    
    protected virtual Vector2 GetInteractionPoint(Vector2 fromPosition)
    {
        if (interactionPoint != null)
            return (Vector2)transform.position + interactionPoint;
        return (Vector2)transform.position;
    }

    public abstract IEnumerator OnInteractSequence(GameObject player);
    public abstract IEnumerator OnDeinteractSequence(GameObject player);

    public void OnInteraction()
    {
        interactionText.text = "Deinteract";
    }

    public void OnDeinteraction()
    {
        interactionText.text = "Interact";
    }

    public void ShowInteractionUI() => interactionUI.SetActive(true);
    public void HideInteractionUI() => interactionUI.SetActive(false);

}