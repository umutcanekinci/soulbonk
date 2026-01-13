using UnityEngine;
using UnityEngine.InputSystem;
using VectorViolet.Core.Stats;
using VectorViolet.Core.Audio;

public class MeleeWeapon : WeaponBase
{
    [Header("References")]
    [SerializeField] private EntityMovement entityMovement;

    [Header("Combat Settings")]
    [SerializeField] private LayerMask targetLayers;

    [Header("Camera Shake Settings")]
    [SerializeField] private float cameraShakeDuration = 0.2f;
    [SerializeField] private float cameraShakeMagnitude = 0.1f;

    [Header("Audio Settings")]
    [SerializeField] private SoundPack missSounds;
    [SerializeField] private SoundPack hitSounds;

    private StatBase attackStat, attackRangeStat;

    private void Start()
    {
        var statHolder = GetComponent<StatHolder>();
        if (statHolder != null)
        {
            attackStat = statHolder.GetStat("AttackDamage");
            attackRangeStat = statHolder.GetStat("AttackRange");
        }
    }
    
    public override void Attack(Vector3 direction)
    {
        DamageEnemiesInRange();
        ShakeCamera();
    }

    void DamageEnemiesInRange()
    {
        float range = attackRangeStat != null ? attackRangeStat.GetValue() : 0f;
        Vector2 attackPointPosition = (Vector2)transform.position + entityMovement.LastFacingDirection * range;
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPointPosition, range, targetLayers);

        bool anyHit = hitEnemies.Length > 0;
        foreach (Collider2D enemy in hitEnemies)
        {
            IDamageable damageable = enemy.GetComponent<IDamageable>();
            
            if (damageable != null)
            {
                damageable.TakeDamage(attackStat.GetValue());
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
        EventBus.TriggerShake(cameraShakeDuration, cameraShakeMagnitude);
    }
}