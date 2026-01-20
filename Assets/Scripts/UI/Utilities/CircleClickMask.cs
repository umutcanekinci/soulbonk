using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform), typeof(Image))]
public class CircleClickMask : MonoBehaviour, ICanvasRaycastFilter
{
    private RectTransform _rectTransform;

    [Range(0f, 1f)]
    [SerializeField] private float radiusPercentage = 1f;

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
    }

    public bool IsRaycastLocationValid(Vector2 screenPoint, Camera eventCamera)
    {
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            _rectTransform, 
            screenPoint, 
            eventCamera, 
            out localPoint
        );

        float width = _rectTransform.rect.width;
        float height = _rectTransform.rect.height;
        float radius = Mathf.Min(width, height) / 2f * radiusPercentage;

        return localPoint.sqrMagnitude <= (radius * radius);
    }
}