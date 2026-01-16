using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HoldEffectUI : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private float animationSpeed = 15f;

    [Header("Scale Settings")]
    [SerializeField] private bool changeScale = false;
    [Tooltip("Values less than 1 shrink (e.g., 0.8), values greater than 1 enlarge.")]
    [SerializeField] Transform targetObject;
    [SerializeField] private bool useDefaultScale = true;
    [SerializeField] private Vector3 originalScale = Vector3.one;
    [SerializeField] private float activeScale = 0.85f;

    [Header("Alpha Settings")]
    [SerializeField] private bool changeAlpha = false;
    [SerializeField] private CanvasGroup targetCanvasGroup;
    [SerializeField] private Image targetImage;
    [SerializeField] private bool useDefaultAlpha = true;
    [SerializeField] private float originalAlpha = 1f;
    [SerializeField] private float activeAlpha = 0.6f;
    
    private Vector3 targetScaleVector;
    private float targetAlpha;

    private void Start()
    {
        if (changeScale && useDefaultScale && targetObject != null)
        {
            originalScale = targetObject.localScale;
        }
        
        if (changeAlpha && useDefaultAlpha)
        {
            if (targetCanvasGroup != null)
                originalAlpha = targetCanvasGroup.alpha;
            if (targetImage != null)
                originalAlpha = targetImage.color.a;
        }
        targetScaleVector = originalScale;
        targetAlpha = originalAlpha;
    }

    private void Update()
    {
        if (changeScale && targetObject != null)
            targetObject.localScale = Vector3.Lerp(targetObject.localScale, targetScaleVector, Time.deltaTime * animationSpeed);
        
        if (changeAlpha)
        {
            if (targetCanvasGroup != null)
                targetCanvasGroup.alpha = Mathf.Lerp(targetCanvasGroup.alpha, targetAlpha, Time.deltaTime * animationSpeed);
            if (targetImage != null)
                targetImage.color = new Color(targetImage.color.r, targetImage.color.g, targetImage.color.b, Mathf.Lerp(targetImage.color.a, targetAlpha, Time.deltaTime * animationSpeed));    
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        targetScaleVector = originalScale * activeScale;
        targetAlpha = activeAlpha;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        targetScaleVector = originalScale;
        targetAlpha = originalAlpha;
    }
}