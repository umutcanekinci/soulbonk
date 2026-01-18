using UnityEngine;
using System.Collections;

class EntityHpChangeEffect : MonoBehaviour
{
    [Tooltip("When damaged, the sprite renderer to apply flash effect on.")]
    [SerializeField] private SpriteRenderer spriteRenderer;

    [Tooltip("Material to use for the flash effect.")]
    [SerializeField] private Material flashMaterial;

    [SerializeField] private Vector2 floatingTextOffset = new Vector2(0, 0.1f);

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

    public void OnTakeDamage(float damageAmount)
    {
        if (spriteRenderer != null && !isFlashing)
        {
            StartCoroutine(DamageFlashRoutine());
        }

        ShowFloatingText(-damageAmount);
    }

    public void OnHeal(float healAmount)
    {
        ShowFloatingText(healAmount);
    }

    private void ShowFloatingText(float amount)
    {
        if (FloatingTextManager.Instance != null)
        {
            string text = (amount > 0 ? "+" : "") + amount.ToString();
            Color color = amount > 0 ? Color.green : Color.red;
            Vector2 position = (Vector2)transform.position + floatingTextOffset;
            FloatingTextManager.Instance.ShowFloatingText(text, position, color, amount);
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
        Gizmos.DrawSphere((Vector2)transform.position + floatingTextOffset, 0.05f);
    }
}