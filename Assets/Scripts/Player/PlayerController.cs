using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(EntityMovement))]
public class PlayerController : MonoBehaviour
{
    private EntityMovement entityMovement;
    [SerializeField] private EntityAttack entityAttack;
    private InputAction moveAction, attackAction;

    private void OnEnable() {
        moveAction.Enable();
        attackAction.Enable();
    }

    private void OnDisable() {
        moveAction.Disable();
        attackAction.Disable();
    }

    private void Awake()
    {
        entityMovement = GetComponent<EntityMovement>();
        SetupInput();
    }

    private void SetupInput()
    {
        moveAction = new InputAction("Move", binding: "<Gamepad>/leftStick");
        attackAction = new InputAction("Attack", binding: "<Gamepad>/buttonSouth");
        
        moveAction.AddCompositeBinding("Dpad")
            .With("Up", "<Keyboard>/w")
            .With("Down", "<Keyboard>/s")
            .With("Left", "<Keyboard>/a")
            .With("Right", "<Keyboard>/d");

        
        attackAction.AddBinding("<Keyboard>/space");
        attackAction.performed += _ => PerformAttack();
    }
    
    private void PerformAttack()
    {
        if (entityAttack != null)
        {
            entityAttack.AttackLogic();
        }
    }
    
    void Update()
    {
        if (GameManager.Instance != null && !GameManager.IsGameplay)
            return;

        entityMovement.SetMoveInput(moveAction.ReadValue<Vector2>());
    }
    
}