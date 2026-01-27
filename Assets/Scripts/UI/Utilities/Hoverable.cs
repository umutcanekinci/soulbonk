using UnityEngine;
using UnityEngine.EventSystems;

public class Hoverable : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [TextArea] 
    public string hoverTooltip;

    private bool _isMobile;

    private void OnDisable()
    {
        TooltipManager.Instance.HideTooltip();
        TooltipManager.Instance.CancelHoverSequence();
    }

    private void Start()
    {
        _isMobile = Application.isMobilePlatform;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_isMobile)
            return;

        TooltipManager.Instance.StartHoverSequence(hoverTooltip);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (_isMobile)
            return;

        TooltipManager.Instance.CancelHoverSequence();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (_isMobile)
        {
            TooltipManager.Instance.ForceHide();
            TooltipManager.Instance.ShowTooltipImmediate(hoverTooltip);
        }
    }
}