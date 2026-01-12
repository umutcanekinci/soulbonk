using UnityEngine;
using UnityEngine.InputSystem;
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
        UpdateSpriteDirection();
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

    void UpdateSpriteDirection()
    {
        /* 
        // We got three options here:

        // 1- Flip the sprite based on horizontal movement
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.flipX = moveInput.x < 0;
        
        // 2- Change scale to face direction
        Vector3 direction = transform.localScale;
        if (moveInput.x > 0)
            direction.x = Mathf.Abs(direction.x);
        else if (moveInput.x < 0)
            direction.x = -Mathf.Abs(direction.x);
        transform.localScale = direction;

        // 3- Or use blend tree with horizontal and vertical parameters. (current)
        */
    }

    public void UpdateAnimator()
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

    void FixedUpdate()
    {
        if (!usePhysicsMovement)
            return;

        rb.linearVelocity = moveInput * speedStat.GetValue();
    }

    // Hareket etmeden sadece yöne dönmek için (Örn: Saldırı anı)
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
}