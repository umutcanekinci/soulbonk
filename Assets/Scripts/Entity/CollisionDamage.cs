using UnityEngine;
using VectorViolet.Core.Stats;

[RequireStat("CollisionDamage")]
public class CollisionDamage : MonoBehaviour
{
    private StatBase collisionDamageStat;

    private void Start()
    {
        var statHolder = GetComponent<StatHolder>();
        if (statHolder != null)
        {
            collisionDamageStat = statHolder.GetStat("CollisionDamage");
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collisionDamageStat == null) return;

        EntityHP entityHP = collision.gameObject.GetComponent<EntityHP>();
        if (entityHP == null)
            return;
        
        if (entityHP.CompareTag(this.tag))
            return;
        
        entityHP.TakeDamage(collisionDamageStat.GetValue());
    }
}