using UnityEngine;
using System.Collections;

public class CameraZoomer : MonoBehaviour
{
    [SerializeField] public static CameraZoomer Instance;
    [SerializeField] private Camera mainCamera;

    private float targetZoom;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        targetZoom = mainCamera.orthographicSize;
    }

    public void ZoomIn(float amount, float duration, System.Action onComplete = null)
    {
        StopAllCoroutines();
        StartCoroutine(ZoomCoroutine(targetZoom - amount, duration, onComplete));
    }

    public void ZoomOut(float amount, float duration, System.Action onComplete = null)
    {
        StopAllCoroutines();
        StartCoroutine(ZoomCoroutine(targetZoom + amount, duration, onComplete));
    }

    public void ResetZoom(float duration, System.Action onComplete = null)
    {
        StopAllCoroutines();
        StartCoroutine(ZoomCoroutine(targetZoom, duration, onComplete));
    }

    public IEnumerator ZoomInCoroutine(float amount, float duration, System.Action onComplete = null)
    {
        yield return ZoomCoroutine(targetZoom - amount, duration, onComplete);
    }

    public IEnumerator ResetZoomCoroutine(float duration, System.Action onComplete = null)
    {
        yield return ZoomCoroutine(targetZoom, duration, onComplete);
    }

    private IEnumerator ZoomCoroutine(float amount, float duration, System.Action onComplete = null)
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
        onComplete?.Invoke();
    }
}