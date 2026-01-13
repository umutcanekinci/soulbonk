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
        GameManager.Instance.SetState(GameState.Cutscene);
        Vector2 targetPos = GetTargetPosition(target);

        Coroutine movementRoutine = player.StartCoroutine(player.MoveToPositionCoroutine(targetPos));
        Coroutine zoomRoutine = StartCoroutine(CameraZoomer.Instance.ZoomInCoroutine(0.5f, 1.5f)); 

        yield return movementRoutine;
        player.Sit(); 
        yield return zoomRoutine;

        UIManager.Instance.ShowInteractionUI();
    }

    private IEnumerator DeinteractionRoutine()
    {
        UIManager.Instance.HideInteractionUI();

        yield return CameraZoomer.Instance.ResetZoomCoroutine(1f);
        player.StandUp();
        GameManager.Instance.SetState(GameState.Gameplay);
    }

    private Vector2 GetTargetPosition(Interactable interactable)
    {
        Vector2 direction = (interactable.transform.position - player.transform.position).normalized;
        return (Vector2)interactable.transform.position - direction * .1f; // Biraz mesafe bırak
    }
}