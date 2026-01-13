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

    private void OnEnable()
    {
        EventBus.OnZoomIn += ZoomInCoroutine;
        EventBus.OnResetZoom += ResetZoomCoroutine;
    }

    private void OnDisable()
    {
        EventBus.OnZoomIn -= ZoomInCoroutine;
        EventBus.OnResetZoom -= ResetZoomCoroutine;
    }

    private void Start()
    {
        _defaultSize = zoomCamera.orthographicSize;
    }

    /// <summary>
    /// Stops any ongoing zoom coroutine.
    /// </summary>
    private void Stop()
    {
        if (_activeZoomCoroutine != null)
            StopCoroutine(_activeZoomCoroutine);
    }

    /// <summary>
    /// Zooms the camera to a specific size over a given duration.
    /// </summary>
    /// <param name="targetSize">The target orthographic size.</param>
    /// <param name="duration">How long the transition should take.</param>
    public void ZoomIn(float amount, float duration)
    {
        Stop();
        _activeZoomCoroutine = StartCoroutine(ZoomCoroutine(_defaultSize - amount, duration));
    }

    /// <summary>
    /// Zooms the camera out by a specific amount over a given duration.
    /// </summary>
    /// <param name="amount">The amount to zoom out.</param>
    /// <param name="duration">How long the transition should take.</param>
    public void ZoomOut(float amount, float duration)
    {
        Stop();
        _activeZoomCoroutine = StartCoroutine(ZoomCoroutine(_defaultSize + amount, duration));
    }

    /// <summary>
    /// Resets the camera to its original size.
    /// </summary>
    /// <param name="duration">Duration of the reset transition.</param>
    public void ResetZoom(float duration)
    {
        Stop();
        _activeZoomCoroutine = StartCoroutine(ZoomCoroutine(_defaultSize, duration));
    }

    /// <summary>
    /// Coroutine version of ZoomIn for use in sequential flows (e.g. InteractionController).
    /// </summary>
    public IEnumerator ZoomInCoroutine(float targetSize, float duration)
    {
        yield return ZoomCoroutine(_defaultSize - targetSize, duration );
    }

    /// <summary>
    /// Coroutine version of ResetZoom for use in sequential flows (e.g. InteractionController).
    /// </summary>
    public IEnumerator ResetZoomCoroutine(float duration)
    {
        yield return ZoomCoroutine(_defaultSize, duration);
    }

    /// <summary>
    /// Core coroutine that smoothly transitions the camera's orthographic size.
    /// </summary>
    /// <param name="targetSize">The target orthographic size.</param>
    /// <param name="duration">Duration of the transition.</param>
    private IEnumerator ZoomCoroutine(float targetSize, float duration)
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