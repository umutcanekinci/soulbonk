using System;
using System.Collections;
using UnityEngine;
using VectorViolet.Core.Utilities;

public class InteractionManager : Singleton<InteractionManager>
{
    [Header("Interaction Settings")]    
    [SerializeField] private InteractableUI interactableUI;
    [SerializeField] private float interactionDuration = 1.5f;
    [SerializeField] private float deinteractionDuration = 1f;
    
    [Header("Camera Focus Settings")]
    [SerializeField] private Vector2 _cameraOffset = new Vector2(0f, 2f);
    [SerializeField] private float _zoomAmount = 0.5f;

    private void OnEnable()
    {
        EventBus.PlayerInteraction.OnInteractionRequestWith += StartInteractionCutscene;
        EventBus.PlayerInteraction.OnDeinteractionRequestWith += StartDeinteractionCutscene;
    }

    private void OnDisable()
    {
        EventBus.PlayerInteraction.OnInteractionRequestWith -= StartInteractionCutscene;
        EventBus.PlayerInteraction.OnDeinteractionRequestWith -= StartDeinteractionCutscene;
    }

    private void StartInteractionCutscene(Interactable interactable, GameObject player)
    {
        if (interactable == null || player == null)
            return;

        CutSceneManager.Play(InteractionRoutine(interactable, player), GameState.Interaction);
    }

    private void StartDeinteractionCutscene(Interactable interactable, GameObject player)
    {
        if (interactable == null || player == null)
            return;

        CutSceneManager.Play(DeinteractionRoutine(interactable, player), GameState.Gameplay);
    }

    private IEnumerator InteractionRoutine(Interactable interactable, GameObject player)
    {
        Vector2 cameraPosition = (Vector2)interactable.transform.position;

        Coroutine focusRoutine = StartCoroutine(EventBus.Camera.TriggerFocus(cameraPosition, _cameraOffset, _zoomAmount, interactionDuration));
        Coroutine interactionRoutine = StartCoroutine(interactable.OnInteractSequence(player));

        yield return interactionRoutine;
        yield return focusRoutine;
        interactableUI.SetInteractionText(interactable.deinteractionText);
    }

    private IEnumerator DeinteractionRoutine(Interactable interactable, GameObject player)
    {
        yield return StartCoroutine(EventBus.Camera.TriggerDefocus(deinteractionDuration));
        Coroutine deinteractionRoutine  = StartCoroutine(interactable.OnDeinteractSequence(player));
        yield return deinteractionRoutine;
        interactableUI.SetInteractionText(interactable.interactionText);
    }

}