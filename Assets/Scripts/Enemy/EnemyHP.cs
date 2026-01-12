using System.Collections;
using UnityEngine;
using VectorViolet.Core.Stats;

[RequireComponent(typeof(EntityHP))]
[RequireStat("CoinSource")]
public class EnemyHP : MonoBehaviour
{
    [Tooltip("When damaged, the sprite renderer to apply flash effect on.")]
    [SerializeField] private SpriteRenderer targetSpriteRenderer;

    [Tooltip("Prefab to instantiate upon death.")]
    [SerializeField] private GameObject deathVFXPrefab;
    [SerializeField] private Material flashMaterial;
    [SerializeField] private SpriteShatter spriteShatter;

    private StatBase coinSourceStat;
    private Material originalMaterial;
    private EntityHP entityHP;
    private bool isFlashing = false;

    private void Awake() {
        entityHP = GetComponent<EntityHP>();
        if (entityHP != null) {
            entityHP.OnTakeDamage += OnTakeDamage;
            entityHP.OnDeath += OnDeath;
        }
        if (spriteShatter != null) {
            spriteShatter.OnDestroy += () => {
                Destroy(gameObject);
            };
        }
    }

    private void OnDestroy() {
        if (entityHP != null) {
            entityHP.OnTakeDamage -= OnTakeDamage;  
            entityHP.OnDeath -= OnDeath;
        }
        if (spriteShatter != null) {
            spriteShatter.OnDestroy -= () => { Destroy(gameObject); };
        }
    }
    void Start()
    {
        originalMaterial = targetSpriteRenderer.material;
        var statHolder = GetComponent<StatHolder>();
        if (statHolder != null)
        {
            coinSourceStat = statHolder.GetStat("CoinSource");
        }
    }

    public void OnTakeDamage(float damageAmount)
    {
        if (targetSpriteRenderer != null && !isFlashing)
        {
            StartCoroutine(DamageFlashRoutine());
        }
    }
    private IEnumerator DamageFlashRoutine()
    {
        isFlashing = true;
        targetSpriteRenderer.material = flashMaterial;
        yield return new WaitForSeconds(0.1f); 
        targetSpriteRenderer.material = originalMaterial;
        isFlashing = false;
    }

    void OnDeath()
    {
        if (deathVFXPrefab != null)
        {
            Instantiate(deathVFXPrefab, transform.position, Quaternion.identity);
        }

        if (spriteShatter != null && spriteShatter.enabled)
        {
            spriteShatter.ShatterAndDie();
        }
        else
        {
            EventBus.TriggerEnemyDeath(transform.position, (int)coinSourceStat.GetValue());
            Destroy(gameObject);
        }
    }
}