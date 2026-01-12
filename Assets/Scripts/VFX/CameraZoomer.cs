using UnityEngine;
using System.Collections;

public class CameraZoomer : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;

    private float targetZoom;

    private void Awake()
    {
        EventBus.OnPlayerInteraction += () => ZoomIn(1f, 1f, UIManager.Instance.ShowInteractionUI);
        EventBus.OnPlayerDeinteraction += () => UIManager.Instance.HideInteractionUI();
        EventBus.OnPlayerDeinteraction += () => ResetZoom(1f);
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