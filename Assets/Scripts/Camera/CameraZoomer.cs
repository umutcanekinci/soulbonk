using UnityEngine;
using System.Collections;

/// <summary>
/// Manages camera zoom effects for cinematic moments or focus interactions.
/// Uses a singleton pattern for easy access.
/// </summary>
public class CameraZoomer : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Camera zoomCamera;

    private float _defaultSize;
    private Coroutine _activeZoomCoroutine;

    private void Awake()
    {
        _defaultSize = zoomCamera.orthographicSize;
    }

    private void OnEnable()
    {
        EventBus.Camera.OnZoomIn += ZoomIn;
        EventBus.Camera.OnResetZoom += ResetZoom;
    }

    private void OnDisable()
    {
        EventBus.Camera.OnZoomIn -= ZoomIn;
        EventBus.Camera.OnResetZoom -= ResetZoom;
    }

    /// <summary>
    /// Zooms the camera to a specific size over a given duration.
    /// </summary>
    /// <param name="targetSize">The target orthographic size.</param>
    /// <param name="duration">How long the transition should take.</param>
    public IEnumerator ZoomIn(float amount, float duration)
    {
        Stop();
        _activeZoomCoroutine = StartCoroutine(ZoomRoutine(_defaultSize - amount, duration));
        yield return _activeZoomCoroutine;
        Stop();
    }

    /// <summary>
    /// Zooms the camera out by a specific amount over a given duration.
    /// </summary>
    /// <param name="amount">The amount to zoom out.</param>
    /// <param name="duration">How long the transition should take.</param>
    public IEnumerator ZoomOut(float amount, float duration)
    {
        Stop();
        _activeZoomCoroutine = StartCoroutine(ZoomRoutine(_defaultSize + amount, duration));
        yield return _activeZoomCoroutine;
        Stop();
    }

    /// <summary>
    /// Resets the camera to its original size.
    /// </summary>
    /// <param name="duration">Duration of the reset transition.</param>
    public IEnumerator ResetZoom(float duration)
    {
        Stop();
        _activeZoomCoroutine = StartCoroutine(ZoomRoutine(_defaultSize, duration));
        yield return _activeZoomCoroutine;
        Stop();
    }
    
    /// <summary>
    /// Stops any ongoing zoom coroutine.
    /// </summary>
    private void Stop()
    {
        if (_activeZoomCoroutine != null)
        {
            StopCoroutine(_activeZoomCoroutine);
            _activeZoomCoroutine = null;
        }
    }

    /// <summary>
    /// Coroutine version of ZoomIn for use in sequential flows (e.g. InteractionController).
    /// </summary>
    public IEnumerator ZoomInRoutine(float targetSize, float duration)
    {
        yield return ZoomRoutine(targetSize, duration );
    }

    /// <summary>
    /// Coroutine version of ResetZoom for use in sequential flows (e.g. InteractionController).
    /// </summary>
    public IEnumerator ResetZoomRoutine(float duration)
    {
        yield return ZoomRoutine(_defaultSize, duration);
    }

    /// <summary>
    /// Core coroutine that smoothly transitions the camera's orthographic size.
    /// </summary>
    /// <param name="targetSize">The target orthographic size.</param>
    /// <param name="duration">Duration of the transition.</param>
    private IEnumerator ZoomRoutine(float targetSize, float duration)
    {
        float startZoom = zoomCamera.orthographicSize;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            zoomCamera.orthographicSize = Mathf.Lerp(startZoom, targetSize, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        zoomCamera.orthographicSize = targetSize;
        _activeZoomCoroutine = null;
    }
}