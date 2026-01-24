using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InteractionDetector : MonoBehaviour
{
    [SerializeField] private InteractableUI interactableUI;
    private Interactable nearestInteractable = null;
    private List<Interactable> nearbyInteractables = new List<Interactable>();
    private InputAction interactAction;

    private void Awake()
    {
        interactAction = new InputAction("Interact", binding: "<Gamepad>/buttonWest");
        interactAction.AddBinding("<Keyboard>/e");
        interactAction.performed += _ => ToggleInteract();
    }

    private void OnEnable() {
        interactAction.Enable();
        EventBus.PlayerInteraction.OnDeinteractionRequest += OnDeinteractPressed;
    }

    private void OnDisable() {
        interactAction.Disable();
        EventBus.PlayerInteraction.OnDeinteractionRequest -= OnDeinteractPressed;
    }

    public void ToggleInteract()
    {
        if (GameManager.IsGameplay)
            OnInteractPressed();
        else if (GameManager.IsInteraction)
            OnDeinteractPressed();
    }

    public void OnInteractPressed()
    {
        if (nearestInteractable == null || !GameManager.IsGameplay)
            return;

        EventBus.PlayerInteraction.TriggerInteractionWith(nearestInteractable, gameObject);
    }

    public void OnDeinteractPressed()
    {
        if (nearestInteractable == null || !GameManager.IsInteraction)
            return;
        
        EventBus.PlayerInteraction.TriggerDeinteractionWith(nearestInteractable, gameObject);
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
            OnDeinteractPressed();
        }
    }

    private void HighlightInteractable(Interactable interactable)
    {
        if (nearestInteractable != null)
            InteractableUI.Instance.Hide();

        nearestInteractable = interactable;

        if (nearestInteractable != null)
            InteractableUI.Instance.Show(nearestInteractable);
    }

}
