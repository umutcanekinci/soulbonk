using UnityEngine;
using UnityEngine.EventSystems;

public class HoldEffectUI : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [Header("Ayarlar")]
    [SerializeField] Transform targetObject;
    [Tooltip("1'den küçük değerler küçültür (örn: 0.8), 1'den büyükler büyütür.")]
    [SerializeField] private float activeScale = 0.85f;
    [SerializeField] private float animationSpeed = 15f;

    private Vector3 originalScale;
    private Vector3 targetScaleVector;

    private void Start()
    {
        originalScale = targetObject.localScale;
        targetScaleVector = originalScale;
    }

    private void Update()
    {
        targetObject.localScale = Vector3.Lerp(targetObject.localScale, targetScaleVector, Time.deltaTime * animationSpeed);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        targetScaleVector = originalScale * activeScale;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        targetScaleVector = originalScale;
    }
}