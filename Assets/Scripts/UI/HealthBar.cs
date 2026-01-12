using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [Header("UI References")]
    public Slider slider;
    public EntityHP playerHP;

    void Start()
    {
        slider.maxValue = 1f;

        if (playerHP != null)
        {
            playerHP.OnHealthChanged += UpdateHealthBar;    
        } else
        {
            Debug.LogError("PlayerHP reference is missing in HealthBar!");
        }
        
    }

    void OnDestroy()
    {
        if (playerHP != null)
        {
            playerHP.OnHealthChanged -= UpdateHealthBar;
        }
    }

    void UpdateHealthBar(float pct)
    {
        slider.value = pct;
    }
}