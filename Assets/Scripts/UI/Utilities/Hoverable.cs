using UnityEngine;
using UnityEngine.EventSystems;

public class Hoverable : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [TextArea] 
    public string hoverTooltip;

    private bool _isMobile;

    private void Start()
    {
        _isMobile = Application.isMobilePlatform;
    }

    // --- PC MANTIĞI (HOVER) ---
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_isMobile) return; // Mobilde hover çalışmasın

        // PC'de mouse üstüne gelince sayacı başlat
        TooltipManager.Instance.StartHoverSequence(hoverTooltip);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (_isMobile) return;

        // PC'de mouse çıkınca sayacı veya tooltip'i kapat
        TooltipManager.Instance.CancelHoverSequence();
    }

    // --- MOBİL MANTIĞI (CLICK) ---
    public void OnPointerClick(PointerEventData eventData)
    {
        // Mobilde tıklanınca direkt göster
        if (_isMobile)
        {
            // Önce varsa diğerlerini kapat (Temizlik)
            TooltipManager.Instance.ForceHide();
            
            // Sonra bunu aç
            TooltipManager.Instance.ShowTooltipImmediate(hoverTooltip);
        }
    }
}