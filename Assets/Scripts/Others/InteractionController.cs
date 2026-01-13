using System.Collections;
using UnityEngine;

public class InteractionController : MonoBehaviour
{
    [SerializeField] private EntityMovement player;
    
    private void OnEnable()
    {
        EventBus.OnPlayerInteraction += StartInteractionSequence;
        EventBus.OnPlayerDeinteraction += StartDeinteractionSequence;
    }

    private void OnDisable()
    {
        EventBus.OnPlayerInteraction -= StartInteractionSequence;
        EventBus.OnPlayerDeinteraction -= StartDeinteractionSequence;
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
        Vector2 targetPos = target.GetClosestInteractionPoint(player.transform.position);

        Coroutine movementRoutine = player.StartCoroutine(player.MoveToPositionCoroutine(targetPos));
        Coroutine zoomRoutine = StartCoroutine(EventBus.TriggerZoomIn(0.5f, 1.5f)); 

        yield return movementRoutine;
        player.Sit(); 
        yield return zoomRoutine;

        UIManager.Instance.ShowInteractionUI();
    }

    private IEnumerator DeinteractionRoutine()
    {
        if (player == null)
            yield break;

        yield return EventBus.TriggerResetZoom(1f);
        player.StandUp();
        GameManager.Instance.SetState(GameState.Gameplay);
    }

}