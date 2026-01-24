using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using System.Collections;

public class TooltipManager : MonoBehaviour
{
    public static TooltipManager Instance { get; private set; }

    [Header("UI References")]
    [SerializeField] private GameObject tooltipContainer;
    [SerializeField] private TMP_Text tooltipText;
    
    [Header("Settings")]
    [SerializeField] private Vector2 tooltipOffset = new Vector2(15f, -15f);
    [SerializeField] private float hoverDelay = 0.5f; // PC için bekleme süresi

    private Coroutine _hoverCoroutine;
    private bool _isMobile;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        
        // Platform kontrolü (Editor'de test etmek için manuel bool yapabilirsin)
        _isMobile = Application.isMobilePlatform; 
        
        HideTooltip();
    }

    private void Update()
    {
        // 1. Tooltip Aktifse Pozisyonu Güncelle (Fareyi/Parmağı takip et)
        if (tooltipContainer.activeSelf)
        {
            FollowPointer();

            // 2. MOBİL ÖZEL: Boşluğa tıklanırsa kapat
            if (_isMobile)
            {
                CheckMobileOutsideClick();
            }
        }
    }

    private void FollowPointer()
    {
        if (Pointer.current != null)
        {
            Vector2 screenPos = Pointer.current.position.ReadValue();
            
            // Ekranın dışına taşmayı engellemek için Clamp eklenebilir (Opsiyonel)
            tooltipContainer.transform.position = screenPos + tooltipOffset;
        }
    }

    // --- MOBİL İÇİN BOŞLUĞA TIKLAMA KONTROLÜ ---
    private void CheckMobileOutsideClick()
    {
        // Ekrana dokunuldu mu? (WasPressedThisFrame)
        if (Pointer.current != null && Pointer.current.press.wasPressedThisFrame)
        {
            // Eğer Tooltip açıkken başka bir yere basılırsa kapatacağız.
            // Not: Tıklanan yer bir Hoverable ise, o kendi Click eventiyle bunu yönetecek,
            // biz sadece genel kapatma emrini veriyoruz. Ancak UI üstüne tıklamayı ayırt etmek gerekebilir.
            
            // Basit mantık: Her tıklamada kapat, eğer yeni bir hoverable'a tıklandıysa o zaten tekrar açacaktır.
            // Ancak bu "flicker" yapabilir. Daha sağlamı: Hoverable scripti açarken buraya "ben açtım" der.
        }
    }
    
    // --- PC İÇİN GECİKMELİ GÖSTERİM ---
    public void StartHoverSequence(string text)
    {
        // Önceki sayaç varsa durdur
        StopHoverSequence();
        
        // Yeni sayaç başlat
        _hoverCoroutine = StartCoroutine(HoverDelayRoutine(text));
    }

    public void CancelHoverSequence()
    {
        StopHoverSequence();
        HideTooltip();
    }

    private void StopHoverSequence()
    {
        if (_hoverCoroutine != null)
        {
            StopCoroutine(_hoverCoroutine);
            _hoverCoroutine = null;
        }
    }

    private IEnumerator HoverDelayRoutine(string text)
    {
        // Belirlenen süre kadar bekle
        yield return new WaitForSeconds(hoverDelay);
        
        // Süre dolduysa ve hala iptal edilmediyse göster
        ShowTooltipImmediate(text);
    }

    // --- GENEL METOTLAR ---

    // Mobilde direkt çağrılır, PC'de süre dolunca çağrılır
    public void ShowTooltipImmediate(string text)
    {
        if (tooltipText == null || tooltipContainer == null) return;

        tooltipText.text = text;
        tooltipContainer.SetActive(true);
        
        // İlk pozisyonu hemen ayarla ki zıplama yapmasın
        FollowPointer();
    }

    public void HideTooltip()
    {
        if (tooltipContainer != null)
        {
            tooltipContainer.SetActive(false);
        }
    }

    // Mobile'de dışarı tıklamayı yönetmek için basit bir "Kapat" metodu
    public void ForceHide()
    {
        HideTooltip();
    }
}