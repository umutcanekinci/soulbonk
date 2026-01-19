using NaughtyAttributes;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HoldEffectUI : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private float animationSpeed = 15f;

    [Header("Scale Settings")]
    [SerializeField] private bool changeScale = false;
    [ShowIf("changeScale")]
    [SerializeField] private Transform targetObject;
    [ShowIf("changeScale")]
    [SerializeField] private bool customScale = false;
    [ShowIf("changeScale"), EnableIf("customScale")]
    [SerializeField] private Vector3 originalScale = Vector3.one;
    [ShowIf("changeScale")]
    [SerializeField] private float activeScale = 0.85f;

    [Header("Alpha Settings")]
    [SerializeField] private bool changeAlpha = false;
    [ShowIf("changeAlpha")]
    [SerializeField] private CanvasGroup targetCanvasGroup;
    [ShowIf("changeAlpha"), HideIf("targetCanvasGroup")]
    [SerializeField] private Image targetImage;
    [ShowIf("changeAlpha")]
    [SerializeField] private bool customAlpha = false;
    [ShowIf("changeAlpha"), EnableIf("customAlpha")]
    [SerializeField] private float originalAlpha = 1f;
    [ShowIf("changeAlpha")]
    [SerializeField] private float activeAlpha = 0.6f;
    
    private Vector3 targetScaleVector;
    private float currentAlphaTarget;

    private void Start()
    {
        if (changeScale && targetObject != null)
        {
            if (!customScale) originalScale = targetObject.localScale;
            targetScaleVector = originalScale;
        }
        
        if (changeAlpha)
        {
            if (!customAlpha)
            {
                if (targetCanvasGroup != null) originalAlpha = targetCanvasGroup.alpha;
                else if (targetImage != null) originalAlpha = targetImage.color.a;
            }
            currentAlphaTarget = originalAlpha;
        }
    }

    private void Update()
    {
        if (changeScale && targetObject != null)
        {
            targetObject.localScale = Vector3.Lerp(targetObject.localScale, targetScaleVector, Time.deltaTime * animationSpeed);
        }
    
        if (changeAlpha)
        {
            if (targetCanvasGroup != null)
            {
                targetCanvasGroup.alpha = Mathf.Lerp(targetCanvasGroup.alpha, currentAlphaTarget, Time.deltaTime * animationSpeed);
            }
            else if (targetImage != null)
            {
                Color c = targetImage.color;
                float newAlpha = Mathf.Lerp(c.a, currentAlphaTarget, Time.deltaTime * animationSpeed);
                targetImage.color = new Color(c.r, c.g, c.b, newAlpha);
            }
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (changeScale) targetScaleVector = originalScale * activeScale;
        if (changeAlpha) currentAlphaTarget = activeAlpha;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (changeScale) targetScaleVector = originalScale;
        if (changeAlpha) currentAlphaTarget = originalAlpha;
    }
}