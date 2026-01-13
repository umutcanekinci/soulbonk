using UnityEngine;
using System.Collections;

public class CameraZoomer : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;

    private float targetZoom;

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
        targetZoom = mainCamera.orthographicSize;
    }

    public void ZoomIn(float amount, float duration)
    {
        StopAllCoroutines();
        StartCoroutine(ZoomCoroutine(targetZoom - amount, duration));
    }

    public void ZoomOut(float amount, float duration)
    {
        StopAllCoroutines();
        StartCoroutine(ZoomCoroutine(targetZoom + amount, duration));
    }

    public void ResetZoom(float duration)
    {
        StopAllCoroutines();
        StartCoroutine(ZoomCoroutine(targetZoom, duration));
    }

    public IEnumerator ZoomInCoroutine(float amount, float duration)
    {
        yield return ZoomCoroutine(targetZoom - amount, duration );
    }

    public IEnumerator ResetZoomCoroutine(float duration)
    {
        yield return ZoomCoroutine(targetZoom, duration);
    }

    private IEnumerator ZoomCoroutine(float amount, float duration)
    {
        float startZoom = mainCamera.orthographicSize;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            mainCamera.orthographicSize = Mathf.Lerp(startZoom, amount, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        mainCamera.orthographicSize = amount;
    }
}