using System.Collections;
using UnityEngine;

public class InteractionManager : MonoBehaviour
{
    [Header("Interaction Settings")]
    [SerializeField] private InteractableUI interactableUI;
    [SerializeField] private Vector2 cameraOffset = new Vector2(0f, 2f);
    [SerializeField] private float zoomAmount = 0.5f;
    [SerializeField] private float interactionDuration = 1.5f;
    [SerializeField] private float deinteractionDuration = 1f;
    
    private void OnEnable()
    {
        EventBus.PlayerInteraction.OnInteractionRequest += StartInteractionSequence;
        EventBus.PlayerInteraction.OnDeinteractionRequest += StartDeinteractionSequence;
    }

    private void OnDisable()
    {
        EventBus.PlayerInteraction.OnInteractionRequest -= StartInteractionSequence;
        EventBus.PlayerInteraction.OnDeinteractionRequest -= StartDeinteractionSequence;
    }

    private void StartInteractionSequence(Interactable interactable, GameObject player)
    {
        if (interactable == null || player == null)
            return;

        CutSceneManager.StartCutscene(InteractionRoutine(interactable, player), GameState.Interaction);
    }

    private void StartDeinteractionSequence(Interactable interactable, GameObject player)
    {
        if (interactable == null || player == null)
            return;

        CutSceneManager.StartCutscene(DeinteractionRoutine(interactable, player), GameState.Gameplay);
    }

    private IEnumerator InteractionRoutine(Interactable interactable, GameObject player)
    {
        Vector2 cameraPosition = (Vector2)interactable.transform.position + cameraOffset;

        Coroutine cameraMoveRoutine  = StartCoroutine(EventBus.Camera.TriggerMove(cameraPosition, interactionDuration));
        Coroutine zoomRoutine        = StartCoroutine(EventBus.Camera.TriggerZoomIn(zoomAmount, interactionDuration)); 
        Coroutine interactionRoutine = StartCoroutine(interactable.OnInteractSequence(player));

        yield return interactionRoutine;
        yield return zoomRoutine;
        yield return cameraMoveRoutine;
        interactableUI.SetInteractionText(interactable.deinteractionText);
    }

    private IEnumerator DeinteractionRoutine(Interactable interactable, GameObject player)
    {
        yield return EventBus.Camera.TriggerResetZoom(deinteractionDuration);
        yield return EventBus.Camera.TriggerMove(player.transform.position, deinteractionDuration);
        Coroutine deinteractionRoutine  = StartCoroutine(interactable.OnDeinteractSequence(player));
        yield return deinteractionRoutine;
        interactableUI.SetInteractionText(interactable.interactionText);
    }

}