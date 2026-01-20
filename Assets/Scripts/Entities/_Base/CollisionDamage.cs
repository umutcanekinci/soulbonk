using UnityEngine;
using VectorViolet.Core.Stats;

[RequireStat("CollisionDamage")]
public class CollisionDamage : MonoBehaviour
{
    private StatBase collisionDamageStat;
    private float lastDamageTime = 0f;
    private float damageCooldown = 0.5f;

    private void Start()
    {
        var statHolder = GetComponent<StatHolder>();
        if (statHolder != null)
        {
            collisionDamageStat = statHolder.GetStat("CollisionDamage");
        }
    }
    
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collisionDamageStat == null) return;

        EntityHP entityHP = collision.gameObject.GetComponent<EntityHP>();
        if (entityHP == null) return;

        if (Time.time - lastDamageTime >= damageCooldown)
        {
            entityHP.TakeDamage(collisionDamageStat.GetValue());
            lastDamageTime = Time.time;
        }
    }
}