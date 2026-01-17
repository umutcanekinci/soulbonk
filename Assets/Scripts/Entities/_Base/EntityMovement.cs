using UnityEngine;
using System.Collections;
using VectorViolet.Core.Stats;

[RequireComponent(typeof(Rigidbody2D), typeof(StatHolder))]
[RequireStat("MoveSpeed")]
public class EntityMovement : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Animator animator;

    // --- OPTIMIZATION: Hashed Animator Parameters ---
    private static readonly int SpeedHash = Animator.StringToHash("Speed");
    private static readonly int HorizontalHash = Animator.StringToHash("Horizontal");
    private static readonly int VerticalHash = Animator.StringToHash("Vertical");
    // ------------------------------------------------

    /// <summary>
    /// Gets the last non-zero direction vector the entity faced.
    /// Default is Vector2.down.
    /// </summary>
    public Vector2 LastFacingDirection { get; private set; } = Vector2.down;
    
    [Tooltip("If false, FixedUpdate movement logic is skipped (useful for cutscenes and navmesh).")]
    public bool usePhysicsMovement = true;

    private Rigidbody2D _rb;
    private StatBase _speedStat;
    private Vector2 _moveInput;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {        
        StatHolder StatHolder = GetComponent<StatHolder>();

        if (StatHolder == null)
        {
            Debug.LogError($"StatHolder component not found on {gameObject.name}.");
            return;
        }

        _speedStat = StatHolder.GetStat("MoveSpeed");
    }

    private void Update()
    {
        UpdateAnimator();
    }

    private void UpdateAnimator()
    {
        if (animator == null ||animator.runtimeAnimatorController == null)
            return;

        // Optimization: Use Hash IDs instead of Strings
        animator.SetFloat(SpeedHash, _moveInput.sqrMagnitude);
        if (_moveInput.sqrMagnitude > 0.01f) // When not moving, don't update direction params and remember last direction
        {
            animator.SetFloat(HorizontalHash, _moveInput.x);
            animator.SetFloat(VerticalHash, _moveInput.y);
        }
    }

    public void Stop()
    {
        SetMoveInput(Vector2.zero);

        if (_rb == null)
            return;
        _rb.linearVelocity = Vector2.zero;
    }

    /// <summary>
    /// Sets the movement input vector. Should be called from an Input Manager or AI Script.
    /// </summary>
    /// <param name="input">Normalized or raw input vector.</param>
    public void SetMoveInput(Vector2 input)
    {
        //moveInput = input.normalized; // This causes diagonal speed boost
        // Clamping prevents diagonal speed boost while keeping analog control
        _moveInput = Vector2.ClampMagnitude(input, 1f); // Same as Mathf.Clamp(input.magnitude, 0f, 1f) * input.normalized;
        if (_moveInput.sqrMagnitude > 0.01f)
        {
            LastFacingDirection = _moveInput.normalized;
        }
    }

    void FixedUpdate()
    {
        if (!usePhysicsMovement)
            return;

        _rb.linearVelocity = _moveInput * _speedStat.GetValue();
    }

    /// <summary>
    /// Forces the entity to face a specific direction without moving physically.
    /// Useful for attacking towards a target while standing still.
    /// </summary>
    /// <param name="direction">The direction to face.</param>
    public void FaceDirection(Vector2 direction)
    {
        if (direction.sqrMagnitude < 0.01f) return;

        LastFacingDirection = direction.normalized;

        if (animator != null)
        {
            animator.SetFloat(HorizontalHash, LastFacingDirection.x);
            animator.SetFloat(VerticalHash, LastFacingDirection.y);
        }
    }

    /// <summary>
    /// Coroutine that handles automated movement to a point (for cutscenes/interactions).
    /// </summary>
    public IEnumerator MoveToPositionCoroutine(Vector2 targetPosition)
    {
        usePhysicsMovement = true;
        float stoppingTreshold = 0.1f;

        while (Vector2.Distance(transform.position, targetPosition) > stoppingTreshold)
        {
            Vector2 dir = (targetPosition - _rb.position).normalized;
            SetMoveInput(dir);
            yield return null;
        }
        Stop();
        _rb.MovePosition(targetPosition);
    }

    public void Sit()
    {
        animator.SetTrigger("Sit");
    }

    public void StandUp()
    {
        animator.SetTrigger("StandUp");
    }

}
