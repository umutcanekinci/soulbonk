using UnityEngine;

public class MeleeWeapon : WeaponBase
{
    [Header("Combat Settings")]
    [SerializeField] private LayerMask targetLayers;

    [Header("Camera Shake Settings")]
    [SerializeField] private float cameraShakeDuration = 0.2f;
    [SerializeField] private float cameraShakeMagnitude = 0.1f;

    private Vector2 attackDirection;

    public override void Attack(Vector3 direction)
    {
        attackDirection = direction;
        DamageEnemiesInRange();
        ShakeCamera();
    }

    void DamageEnemiesInRange()
    {
        float range = _attackRangeStat != null ? _attackRangeStat.Value : 0f;
        Vector2 attackPointPosition = (Vector2)transform.position + attackDirection * range;
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPointPosition, range, targetLayers);

        bool anyHit = hitEnemies.Length > 0;
        foreach (Collider2D enemy in hitEnemies)
        {
            IDamageable damageable = enemy.GetComponent<IDamageable>();
            
            if (damageable != null)
            {
                float damageAmount = CalculateDamage(out bool isCritical);
                damageable.TakeDamage(damageAmount, isCritical);
            }
        }
        PlaySFX(anyHit);
    }

    void ShakeCamera()
    {
        EventBus.Camera.TriggerShake(cameraShakeDuration, cameraShakeMagnitude);
    }

    private void OnDrawGizmosSelected()
    {
        if (_attackRangeStat == null)
            return;

        float range = _attackRangeStat.Value;
        Vector2 attackPointPosition = (Vector2)transform.position + attackDirection * range;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPointPosition, range);
    }
}