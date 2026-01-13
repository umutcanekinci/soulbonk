using UnityEngine;
using System.Collections;

class EntityTakeDamageEffect : MonoBehaviour
{
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
        }
    }

    private void OnDisable()
    {
        if (entityHP != null)
        {
            entityHP.OnTakeDamage -= OnTakeDamage;
        }
    }

    public void OnTakeDamage(float damageAmount)
    {
        if (spriteRenderer != null && !isFlashing)
        {
            StartCoroutine(DamageFlashRoutine());
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
}