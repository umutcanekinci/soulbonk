using UnityEngine;
using System.Collections;
using VectorViolet.Core.Stats;


[RequireComponent(typeof(Rigidbody2D), typeof(StatHolder))]
[RequireStat("MoveSpeed")]
public class EntityMovement : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Animator animator;
    public Vector2 LastFacingDirection { get; private set; } = Vector2.down;
    
    public bool usePhysicsMovement = true;
    private Rigidbody2D rb;
    private StatBase speedStat;
    private Vector2 moveInput;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {        
        StatHolder StatHolder = GetComponent<StatHolder>();

        if (StatHolder == null)
        {
            Debug.LogError("StatHolder component not found on Player.");
            return;
        }

        speedStat = StatHolder.GetStat("MoveSpeed");
    }

    private void Update()
    {
        UpdateAnimator();
    }

    private void UpdateAnimator()
    {
        if (animator == null ||animator.runtimeAnimatorController == null)
            return;

        animator.SetFloat("Speed", moveInput.sqrMagnitude);
        if (moveInput.sqrMagnitude > 0.01f) // When not moving, don't update direction params and remember last direction
        {
            animator.SetFloat("Horizontal", moveInput.x);
            animator.SetFloat("Vertical", moveInput.y);
        }
    }

    public void SetMoveInput(Vector2 input)
    {
        //moveInput = input.normalized; // This causes diagonal speed boost
        moveInput = Vector2.ClampMagnitude(input, 1f); // Same as Mathf.Clamp(input.magnitude, 0f, 1f) * input.normalized;
        if (moveInput.sqrMagnitude > 0.01f)
        {
            LastFacingDirection = moveInput.normalized;
        }
    }

    void FixedUpdate()
    {
        if (!usePhysicsMovement)
            return;

        rb.linearVelocity = moveInput * speedStat.GetValue();
    }

    public void FaceDirection(Vector2 direction)
    {
        // Yön vektörü çok küçükse işlem yapma (0,0 gelirse sapıtmasın)
        if (direction.sqrMagnitude < 0.01f) return;

        // 1. Yönü kaydet
        LastFacingDirection = direction.normalized;

        // 2. Animator'a sadece yön bilgisini ver (Speed vermiyoruz!)
        if (animator != null)
        {
            animator.SetFloat("Horizontal", LastFacingDirection.x);
            animator.SetFloat("Vertical", LastFacingDirection.y);
            
            // Speed parametresini ellemiyoruz veya 0 olduğundan emin olmak istersen:
            // animator.SetFloat("Speed", 0); 
        }
    }

    public IEnumerator MoveToPositionCoroutine(Vector2 targetPosition)
    {
        usePhysicsMovement = true;
        while (Vector2.Distance(transform.position, targetPosition) > 0.1f)
        {
            Vector2 dir = (targetPosition - (Vector2)transform.position).normalized;
            SetMoveInput(dir);
            yield return null; // Bir sonraki kareyi bekle
        }
        SetMoveInput(Vector2.zero);
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
