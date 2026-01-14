using System.Collections;
using UnityEngine;

public class InteractionController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private EntityMovement player;

    [Header("Interaction Settings")]
    [SerializeField] private Vector2 interactionCameraOffset = new Vector2(0f, 2f);
    [SerializeField] private float interactionZoomAmount = 0.5f;
    [SerializeField] private float interactionDuration = 1.5f;
    [SerializeField] private float deinteractionDuration = 1f;
    
    private void OnEnable()
    {
        EventBus.PlayerInteraction.OnInteraction += StartInteractionSequence;
        EventBus.PlayerInteraction.OnDeinteraction += StartDeinteractionSequence;
    }

    private void OnDisable()
    {
        EventBus.PlayerInteraction.OnInteraction -= StartInteractionSequence;
        EventBus.PlayerInteraction.OnDeinteraction -= StartDeinteractionSequence;
    }

    private void StartInteractionSequence(Interactable target)
    {
        StartCoroutine(InteractionRoutine(target));
    }

    private void StartDeinteractionSequence()
    {
        StartCoroutine(DeinteractionRoutine());
    }

    private IEnumerator InteractionRoutine(Interactable target)
    {
        if (target == null || player == null)
            yield break;

        GameManager.Instance.SetState(GameState.Cutscene);
        Vector2 targetPosition = target.GetClosestInteractionPoint(player.transform.position);
        Vector2 cameraPosition = (Vector2)target.transform.position + interactionCameraOffset;

        Coroutine movementRoutine = player.StartCoroutine(player.MoveToPositionCoroutine(targetPosition));
        Coroutine zoomRoutine = StartCoroutine(EventBus.Camera.TriggerZoomIn(interactionZoomAmount, interactionDuration)); 
        Coroutine cameraMoveRoutine = StartCoroutine(EventBus.Camera.TriggerCameraMove(cameraPosition, interactionDuration));

        yield return movementRoutine; // Wait for player to reach the interaction point
        player.Sit();  // Example action upon reaching the point
        yield return zoomRoutine; // Wait for zoom to complete
        yield return cameraMoveRoutine; // Wait for camera move to complete

        GameManager.Instance.SetState(GameState.Interaction);
    }

    private IEnumerator DeinteractionRoutine()
    {
        if (player == null)
            yield break;

        GameManager.Instance.SetState(GameState.Cutscene);
        yield return EventBus.Camera.TriggerResetZoom(deinteractionDuration);
        yield return EventBus.Camera.TriggerCameraMove(player.transform.position, deinteractionDuration);
        player.StandUp();
        GameManager.Instance.SetState(GameState.Gameplay);
    }

}