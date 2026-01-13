using UnityEngine;

public enum GameState {
    Gameplay,
    Cutscene,
    Paused
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public event System.Action<GameState> OnGameStateChanged;
    public GameState CurrentState { get; private set; }
    public bool IsGameplay() => CurrentState == GameState.Gameplay;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void SetState(GameState newState)
    {
        CurrentState = newState;
        OnGameStateChanged?.Invoke(newState);
    }
}