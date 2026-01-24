using UnityEngine;
using System.Collections;
using System.Numerics;
using Vector3 = UnityEngine.Vector3;
using Unity.VisualScripting;

public class CameraMovement : MonoBehaviour
{
    private Coroutine _activeMoveCoroutine;
    private Vector3 _defaultPosition;

    private void Awake()
    {
        _defaultPosition = transform.localPosition;
    }

    private void OnEnable()
    {
        EventBus.Camera.OnMove += MoveTo;
        EventBus.Camera.OnResetPosition += ResetPosition;
    }

    private void OnDisable()
    {
        EventBus.Camera.OnMove -= MoveTo;
        EventBus.Camera.OnResetPosition -= ResetPosition;
    }

    /// <summary>
    /// Moves the camera to a target position over a given duration.
    /// </summary>
    /// <param name="targetPosition">The target position to move to.</param>
    /// <param name="duration">How long the movement should take.</param>
    private IEnumerator MoveTo(Vector3 targetPosition, float duration)
    {
        Stop();
        _activeMoveCoroutine = StartCoroutine(MoveToRoutine(targetPosition, duration));
        yield return _activeMoveCoroutine;
        Stop();
    }

    private IEnumerator ResetPosition(float duration)
    {
        if (_defaultPosition == null || transform.parent == null)
            yield break;

        Stop();

        Vector3 position = transform.parent.TransformPoint(_defaultPosition);
        _activeMoveCoroutine = StartCoroutine(MoveToRoutine(position, duration));
        yield return _activeMoveCoroutine;
        Stop();
    }

    /// <summary>
    /// Stops any ongoing move coroutine.
    /// </summary>
    private void Stop()
    {
        if (_activeMoveCoroutine != null)
        {
            StopCoroutine(_activeMoveCoroutine);
            _activeMoveCoroutine = null;
        }
    }

    private IEnumerator MoveToRoutine(Vector3 targetPosition, float duration)
    {
        Vector3 startingPosition = transform.position;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            transform.position = Vector3.Lerp(startingPosition, targetPosition, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPosition;
    }
}