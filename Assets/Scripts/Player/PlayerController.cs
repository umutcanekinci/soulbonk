using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(EntityMovement))]
public class PlayerController : MonoBehaviour
{
    private EntityMovement entityMovement;
    private PlayerInteraction playerInteraction;
    [SerializeField] private EntityAttack entityAttack;
    private InputAction moveAction, attackAction, interactAction;

    private void OnEnable() {
        moveAction.Enable();
        attackAction.Enable();
        interactAction.Enable();
    }

    private void OnDisable() {
        moveAction.Disable();
        attackAction.Disable();
        interactAction.Disable();
    }

    private void Awake()
    {
        entityMovement = GetComponent<EntityMovement>();
        playerInteraction = GetComponent<PlayerInteraction>();
        SetupInput();
    }

    private void SetupInput()
    {
        moveAction = new InputAction("Move", binding: "<Gamepad>/leftStick");
        attackAction = new InputAction("Attack", binding: "<Gamepad>/buttonSouth");
        interactAction = new InputAction("Interact", binding: "<Gamepad>/buttonWest");

        moveAction.AddCompositeBinding("Dpad")
            .With("Up", "<Keyboard>/w")
            .With("Down", "<Keyboard>/s")
            .With("Left", "<Keyboard>/a")
            .With("Right", "<Keyboard>/d");

        
        attackAction.AddBinding("<Keyboard>/space");
        interactAction.AddBinding("<Keyboard>/e");

        attackAction.performed += _ => PerformAttack();
        interactAction.performed += _ => ToggleInteraction();
    }
    
    private void PerformAttack()
    {
        if (entityAttack != null)
        {
            entityAttack.AttackLogic();
        }
    }
    
    public void ToggleInteraction()
    {
        if (playerInteraction != null)
        {
            playerInteraction.ToggleInteraction();
        }
    }

    void Update()
    {
        if (GameManager.Instance != null && !GameManager.Instance.IsGameplay())
            return;

        entityMovement.SetMoveInput(moveAction.ReadValue<Vector2>());
    }
    
}