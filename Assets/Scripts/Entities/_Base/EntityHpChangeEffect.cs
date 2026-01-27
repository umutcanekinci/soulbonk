using UnityEngine;
using System.Collections;

class EntityHpChangeEffect : MonoBehaviour
{
    [Header("Floating Text Settings")]
    [SerializeField] private Vector2 offset = new Vector2(0, 0.1f);
    [SerializeField] private Color damageColor = Color.white;
    [SerializeField] private Color healColor = Color.green;
    [SerializeField] private Color critColor = Color.red;
    [SerializeField, Range(1f, 3f)] private float critScale = 1.5f;

    [Header("Flash Effect Settings")]
    [Tooltip("When damaged, the sprite renderer to apply flash effect on.")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [Tooltip("Material to use for the flash effect.")]
    [SerializeField] private Material flashMaterial;

    private EntityHP entityHP;
    private Material originalMaterial;
    private bool isFlashing = false;

    private void Awake()
    {
        entityHP = GetComponent<EntityHP>();
        if (spriteRenderer != null)
        {
            originalMaterial = spriteRenderer.material;
        }
    }

    private void OnEnable()
    {
        if (entityHP != null)
        {
            entityHP.OnTakeDamage += OnTakeDamage;
            entityHP.OnHeal += OnHeal;
        }
    }

    private void OnDisable()
    {
        if (entityHP != null)
        {
            entityHP.OnTakeDamage -= OnTakeDamage;
            entityHP.OnHeal -= OnHeal;
        }
    }

    public void OnTakeDamage(float damageAmount, bool isCritical = false)
    {
        if (spriteRenderer != null && !isFlashing)
        {
            StartCoroutine(DamageFlashRoutine());
        }

        ShowFloatingText(-damageAmount, isCritical);
    }

    public void OnHeal(float healAmount)
    {
        ShowFloatingText(healAmount);
    }

    private void ShowFloatingText(float amount, bool isCritical = false)
    {
        if (FloatingTextManager.Instance != null)
        {
            int amountInt = Mathf.CeilToInt(Mathf.Abs(amount));
            string text = (amount > 0 ? "+" : "") + amountInt.ToString();
            Vector2 position = (Vector2)transform.position + offset;
            Color color = amount > 0 ? healColor : isCritical ? critColor : damageColor;
            float scale = (isCritical ? critScale : 1f) * amount;

            FloatingTextManager.Instance.ShowFloatingText(text, position, color, scale);
        }
    }
    
    private IEnumerator DamageFlashRoutine()
    {
        isFlashing = true;
        spriteRenderer.material = flashMaterial;
        yield return new WaitForSeconds(0.1f); 
        spriteRenderer.material = originalMaterial;
        isFlashing = false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere((Vector2)transform.position + offset, 0.05f);
    }
}