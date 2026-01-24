using UnityEngine;
using System.Collections;

public class CameraFocus : MonoBehaviour
{
    private Coroutine _activeFocusCoroutine;

    private void OnEnable()
    {
        EventBus.Camera.OnFocus += FocusOn;
        EventBus.Camera.OnDefocus += Defocus;
    }

    private void OnDisable()
    {
        EventBus.Camera.OnFocus -= FocusOn;
        EventBus.Camera.OnDefocus -= Defocus;
    }

    private IEnumerator FocusOn(Vector3 targetPosition, Vector2 cameraOffset, float zoomAmount, float duration)
    {
        Stop();
        _activeFocusCoroutine = StartCoroutine(FocusOnRoutine(targetPosition, cameraOffset, zoomAmount, duration));
        yield return _activeFocusCoroutine;
        Stop();
    }

    private IEnumerator Defocus(float duration)
    {
        Stop();
        _activeFocusCoroutine = StartCoroutine(DefocusRoutine(duration));
        yield return _activeFocusCoroutine;
        Stop();
    }

    private void Stop()
    {
        if (_activeFocusCoroutine != null)
        {
            StopCoroutine(_activeFocusCoroutine);
            _activeFocusCoroutine = null;
        }
    }

    private IEnumerator FocusOnRoutine(Vector3 targetPosition, Vector2 cameraOffset, float zoomAmount, float duration)
    {
        Vector3 cameraPosition = targetPosition + (Vector3)cameraOffset;
        Coroutine moveRoutine = StartCoroutine(EventBus.Camera.TriggerMove(cameraPosition, duration));
        Coroutine zoomRoutine = StartCoroutine(EventBus.Camera.TriggerZoomIn(zoomAmount, duration));

        yield return moveRoutine;
        yield return zoomRoutine;
    }

    private IEnumerator DefocusRoutine(float duration)
    {
        Coroutine resetZoomRoutine = StartCoroutine(EventBus.Camera.TriggerResetZoom(duration));
        Coroutine resetPositionRoutine = StartCoroutine(EventBus.Camera.TriggerResetPosition(duration));

        yield return resetZoomRoutine;
        yield return resetPositionRoutine;
    }
}