using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using UnityEngine.UI;

public class MenuButton : MonoBehaviour
{
    [Header("References")]
    [SerializeField] Transform player;

    [Header("Pause Menu Camera Focus Settings")]
    [SerializeField] private Vector2 _cameraOffset = new Vector2(0f, 2f);
    [SerializeField] private float _zoomAmount = 0.5f;

    private InputAction exitAction;
    private ButtonState buttonState;
    
    private void Awake()
    {
        buttonState = GetComponent<ButtonState>();
        exitAction = new InputAction("Exit", binding: "<Gamepad>/buttonNorth");
        exitAction.AddBinding("<Keyboard>/escape");
        exitAction.performed += _ => HandlePress();
    }

    private void OnEnable()
    {
        exitAction.Enable();
        GameManager.Instance.OnStateChanged += HandleGameStateChange;
    }

    private void OnDisable()
    {
        exitAction.Disable();
        GameManager.Instance.OnStateChanged -= HandleGameStateChange;
    }

    private void HandlePress()
    {
        switch (GameManager.Instance.CurrentState)
        {
            case GameState.Gameplay:
                PauseGame();
                break;
            case GameState.Paused:
                ResumeGame();
                break;
            case GameState.Interaction:
                Deinteract();
                break;
        }
    }

    private void PauseGame()
    {
        CutSceneManager.Play(PauseMenuCutscene(), GameState.Paused);
    }

    private void ResumeGame()
    {
        CutSceneManager.Play(ResumeGameCutscene(), GameState.Gameplay);
    }

    private void Deinteract()
    {
        EventBus.PlayerInteraction.RequestDeinteraction();
    }

    private void HandleGameStateChange(GameState newState)
    {
        switch (newState)
        {
            case GameState.Gameplay:
                buttonState.SetState("menu");
                break;
            case GameState.Paused:
                buttonState.SetState("close");
                break;
            case GameState.Interaction:
                buttonState.SetState("close");
                break;
        }
    }

    private IEnumerator PauseMenuCutscene()
    {
        player.GetComponent<EntityMovement>().FaceDirection(Vector2.down);
        yield return EventBus.Camera.TriggerFocus(player.position, _cameraOffset, _zoomAmount, 0.5f);
    }

    private IEnumerator ResumeGameCutscene()
    {
        yield return EventBus.Camera.TriggerDefocus(0.5f);
    }
}
