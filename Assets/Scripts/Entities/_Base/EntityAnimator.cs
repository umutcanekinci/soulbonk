using UnityEngine;

public class EntityAnimator : MonoBehaviour
{
    private Animator animator;
    private bool IsAnimatorControllerAssigned => animator != null && animator.runtimeAnimatorController != null;

    // --- OPTIMIZATION: Hashed Animator Parameters ---
    private static readonly int SpeedHash = Animator.StringToHash("Speed");
    private static readonly int HorizontalHash = Animator.StringToHash("Horizontal");
    private static readonly int VerticalHash = Animator.StringToHash("Vertical");
    private static readonly int SitTriggerHash = Animator.StringToHash("Sit");
    private static readonly int StandUpTriggerHash = Animator.StringToHash("StandUp");
    private static readonly int AttackSpeedHash = Animator.StringToHash("AttackSpeed");
    private static readonly int AttackTriggerHash = Animator.StringToHash("Attack");
    // ------------------------------------------------

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void Sit() => SetTrigger(SitTriggerHash);
    public void StandUp() => SetTrigger(StandUpTriggerHash);
    public void SetSpeed(float speed) => SetFloat(SpeedHash, speed);
    
    public void SetFacingDirection(Vector2 direction)
    {
        SetFloat(HorizontalHash, direction.x);
        SetFloat(VerticalHash, direction.y);
    }
    
    public void TriggerAttack(float attackSpeed)
    {
        ResetTrigger(AttackTriggerHash);
        SetFloat(AttackSpeedHash, attackSpeed);
        SetTrigger(AttackTriggerHash);
    }

    private void SetFloat(int hash, float value)
    {
        if (animator == null || !IsAnimatorControllerAssigned)
            return;

        animator.SetFloat(hash, value);
    }

    private void SetTrigger(int hash)
    {
        if (animator == null || !IsAnimatorControllerAssigned)
            return;

        animator.SetTrigger(hash);
    }

    private void ResetTrigger(int hash)
    {
        if (animator == null || !IsAnimatorControllerAssigned)
            return;

        animator.ResetTrigger(hash);
    }

}