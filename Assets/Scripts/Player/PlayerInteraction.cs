using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteraction : MonoBehaviour
{
    private Interactable nearestInteractable = null;
    private float nearestInteractableDistance = Mathf.Infinity;
    private bool isInteracting = false;

    public void ToggleInteraction()
    {
        if (nearestInteractable == null)
            return;

        if (isInteracting)
        {
            nearestInteractable.TriggerDeinteract();
            isInteracting = false;
        }
        else
        {
            nearestInteractable.TriggerInteract();
            isInteracting = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Interactable interactable = collision.GetComponent<Interactable>();
        if (interactable == null)
            return;

        if (isInteracting)
            return;
        
        if (nearestInteractable == interactable)
            return;

        float distance = Vector2.Distance(transform.position, interactable.transform.position);

        if (nearestInteractable != null && distance >= nearestInteractableDistance)
            return;

        if (nearestInteractable != null)
            nearestInteractable.HideInteractionUI();

        nearestInteractable = interactable;
        nearestInteractableDistance = distance;
        interactable.ShowInteractionUI();
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        Interactable interactable = collision.GetComponent<Interactable>();
        if (interactable == null)
            return;

        if (nearestInteractable != interactable)
            return;

        if (isInteracting)
        {
            nearestInteractable.TriggerDeinteract();
            isInteracting = false;
        }
        nearestInteractable.HideInteractionUI();
        nearestInteractable = null;
        nearestInteractableDistance = Mathf.Infinity;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        Interactable interactable = collision.GetComponent<Interactable>();
        if (interactable == null)
            return;

        if (isInteracting)
            return;

        float distance = Vector2.Distance(transform.position, interactable.transform.position);
        if (nearestInteractable == interactable) {
            nearestInteractableDistance = distance;
        }
        else {
            if (distance < nearestInteractableDistance)
            {
                if (nearestInteractable != null)
                    nearestInteractable.HideInteractionUI();

                nearestInteractable = interactable;
                nearestInteractableDistance = distance;
                interactable.ShowInteractionUI();
            }
        }
    }
}
