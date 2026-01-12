using UnityEngine;
using UnityEngine.InputSystem;
using VectorViolet.Core.Stats;
using VectorViolet.Core.Audio;

[RequireComponent(typeof(StatHolder))]
[RequireStat("AttackDamage", "AttackRange")]
public class Sword : WeaponBase
{
    [Header("References")]
    [SerializeField] private LayerMask enemyLayers;
    [SerializeField] private EntityMovement entityMovement;
    [SerializeField] private EntityAttack entityAttack;

    [Header("Camera Shake Settings")]
    [SerializeField] private CameraShaker cameraShaker;
    [SerializeField] private float cameraShakeDuration = 0.2f;
    [SerializeField] private float cameraShakeMagnitude = 0.1f;

    [Header("Audio Settings")]
    [SerializeField] private SoundPack missSounds;
    [SerializeField] private SoundPack hitSounds;
    [SerializeField] private SoundPack killSounds;

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
        Vector2 attackPointPosition = (Vector2)transform.position + entityMovement.LastFacingDirection * attackRangeStat.GetValue();
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPointPosition, attackRangeStat.GetValue(), enemyLayers);

        bool anyKill = false;
        bool anyHit = hitEnemies.Length > 0;
        foreach (Collider2D enemy in hitEnemies)
        {
            EntityHP enemyHP = enemy.GetComponent<EntityHP>();
            
            if (enemyHP != null)
            {
                enemyHP.TakeDamage(attackStat.GetValue());
                if (enemyHP.IsDead)
                    anyKill = true;
            }
        }
        PlaySFX(anyKill, anyHit);
    }

    private void PlaySFX(bool anyKill, bool anyHit)
    {
        SoundManager.Instance.PlaySFX(anyKill ? killSounds : anyHit ? hitSounds : missSounds);
    }

    void ShakeCamera()
    {
        if (cameraShaker == null)
            return;

        cameraShaker.Shake(cameraShakeDuration, cameraShakeMagnitude);
    }
}