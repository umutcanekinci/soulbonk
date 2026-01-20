using UnityEngine;
using VectorViolet.Core.Audio;

public class MeleeWeapon : WeaponBase
{
    [Header("Combat Settings")]
    [SerializeField] private LayerMask targetLayers;

    [Header("Camera Shake Settings")]
    [SerializeField] private float cameraShakeDuration = 0.2f;
    [SerializeField] private float cameraShakeMagnitude = 0.1f;

    [Header("Audio Settings")]
    [SerializeField] private SoundPack missSounds;
    [SerializeField] private SoundPack hitSounds;

    private Vector2 attackDirection;

    public override void Attack(Vector3 direction)
    {
        attackDirection = direction;
        DamageEnemiesInRange();
        ShakeCamera();
    }

    void DamageEnemiesInRange()
    {
        float range = attackRangeStat != null ? attackRangeStat.GetValue() : 0f;
        Vector2 attackPointPosition = (Vector2)transform.position + attackDirection * range;
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPointPosition, range, targetLayers);

        bool anyHit = hitEnemies.Length > 0;
        foreach (Collider2D enemy in hitEnemies)
        {
            IDamageable damageable = enemy.GetComponent<IDamageable>();
            
            if (damageable != null)
            {
                damageable.TakeDamage(attackDamageStat.GetValue());
            }
        }
        PlaySFX(anyHit);
    }

    private void PlaySFX(bool anyHit)
    {
        SoundManager.Instance.PlaySFX(anyHit ? hitSounds : missSounds);
    }

    void ShakeCamera()
    {
        EventBus.Camera.TriggerShake(cameraShakeDuration, cameraShakeMagnitude);
    }

    private void OnDrawGizmosSelected()
    {
        if (attackRangeStat == null)
            return;

        float range = attackRangeStat.GetValue();
        Vector2 attackPointPosition = (Vector2)transform.position + attackDirection * range;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPointPosition, range);
    }
}