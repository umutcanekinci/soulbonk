using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InteractionDetector : MonoBehaviour
{
    private Interactable nearestInteractable = null;
    private List<Interactable> nearbyInteractables = new List<Interactable>();
    private InputAction interactAction;

    private void Awake()
    {
        interactAction = new InputAction("Interact", binding: "<Gamepad>/buttonWest");
        interactAction.AddBinding("<Keyboard>/e");
        interactAction.performed += _ => OnInteractPressed();
    }

    private void OnEnable() {
        interactAction.Enable();
    }

    private void OnDisable() {
        interactAction.Disable();
    }

    public void OnInteractPressed()
    {
        if (nearestInteractable == null || GameManager.IsCutscene)
            return;

        if (GameManager.IsInteraction)
            EventBus.PlayerInteraction.TriggerDeinteraction(nearestInteractable, gameObject);
        else
            EventBus.PlayerInteraction.TriggerRequest(nearestInteractable, gameObject);
    }

    private void Update()
    {
        HandleNearestCalculation();
    }

    private void HandleNearestCalculation()
    {
        if (nearbyInteractables.Count == 0)
        {
            if (nearestInteractable != null)
                HighlightInteractable(null);
            return;
        }

        Interactable closest = null;
        float minDst = float.MaxValue;

        for (int i = nearbyInteractables.Count - 1; i >= 0; i--)
        {
            Interactable item = nearbyInteractables[i];
            if (item == null || !item.gameObject.activeSelf) 
            {
                nearbyInteractables.RemoveAt(i);
                continue;
            }

            float dst = Vector2.Distance(transform.position, item.transform.position);
            if (dst < minDst)
            {
                minDst = dst;
                closest = item;
            }
        }

        if (closest != nearestInteractable)
        {
            HighlightInteractable(closest);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!GameManager.IsGameplay)
            return;

        Interactable interactable = collision.GetComponent<Interactable>();
        if (interactable == null || nearbyInteractables.Contains(interactable))
            return;

        nearbyInteractables.Add(interactable);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        Interactable interactable = collision.GetComponent<Interactable>();
        nearbyInteractables.Remove(interactable);
        if (GameManager.IsInteraction && nearestInteractable == interactable)
        {
            EventBus.PlayerInteraction.TriggerDeinteraction(interactable, gameObject);
        }
        //     return;

        // if (GameManager.IsInteraction)
        //     EventBus.PlayerInteraction.TriggerDeinteraction(nearestInteractable, gameObject);

        // HighlightInteractable(null);
    }

    private void HighlightInteractable(Interactable interactable)
    {
        if (nearestInteractable != null)
            nearestInteractable.HideInteractionUI();

        nearestInteractable = interactable;

        if (nearestInteractable != null)
            nearestInteractable.ShowInteractionUI();
    }

}
