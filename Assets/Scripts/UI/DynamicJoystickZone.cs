using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.OnScreen;

public class DynamicJoystickZone : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    [Header("References")]
    [Tooltip("The RectTransform of the joystick to be moved.")]
    [SerializeField] private RectTransform joystickTransform;
    [SerializeField] private HoldEffectUI holdEffectUI;
    
    [Tooltip("The OnScreenStick component controlling the joystick behavior.")]
    [SerializeField] private OnScreenStick onScreenStick;

    [SerializeField] private bool resetWhenReleased = true;

    private Vector2 initialJoystickPosition;

    private void Start()
    {
        initialJoystickPosition = joystickTransform.anchoredPosition;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        joystickTransform.position = eventData.position;

        onScreenStick.OnPointerDown(eventData);

        if (holdEffectUI != null)
            holdEffectUI.OnPointerDown(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        onScreenStick.OnDrag(eventData);

        if (holdEffectUI != null)
            holdEffectUI.OnPointerDown(eventData);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        onScreenStick.OnPointerUp(eventData);

        if (holdEffectUI != null)
            holdEffectUI.OnPointerUp(eventData);

        if (resetWhenReleased)
        {
            joystickTransform.anchoredPosition = initialJoystickPosition;
        }
    }
}