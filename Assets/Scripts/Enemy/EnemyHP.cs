using System.Collections;
using UnityEngine;
using VectorViolet.Core.Stats;

[RequireComponent(typeof(EntityHP))]
[RequireStat("CoinSource")]
public class EnemyHP : MonoBehaviour
{
    [SerializeField] private GameObject deathVFXPrefab;
    [SerializeField] private SpriteShatter spriteShatter;

    private StatBase coinSourceStat;
    private EntityHP entityHP;

    private void Awake() {
        entityHP = GetComponent<EntityHP>();
    }

    private void OnEnable() {
        if (entityHP != null) {
            entityHP.OnDeath += OnDeath;
        }
        if (spriteShatter != null) {
            spriteShatter.OnDestroy += Destroy;
        }
    }

    private void OnDisable() {
        if (entityHP != null) {
            entityHP.OnDeath -= OnDeath;
        }
        if (spriteShatter != null) {
            spriteShatter.OnDestroy -= Destroy;
        }
    }
   
    void Start()
    {
        var statHolder = GetComponent<StatHolder>();
        if (statHolder != null)
        {
            coinSourceStat = statHolder.GetStat("CoinSource");
        }
    } 
    private void Destroy()
    {
        Destroy(gameObject);
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