using UnityEngine;

public enum GameState {
    Gameplay,
    Cutscene,
    Interaction
}

public class GameManager : MonoBehaviour
{
    [Header("Layer Names")]
    [SerializeField] private string playerLayerName = "Player";
    [SerializeField] private string enemyLayerName = "Enemy";

    public static GameManager Instance { get; private set; }
    public event System.Action<GameState> OnGameStateChanged;
    public GameState CurrentState { get; private set; }
    public static bool IsGameplay => Instance != null && Instance.CurrentState == GameState.Gameplay;
    public static bool IsCutscene => Instance != null && Instance.CurrentState == GameState.Cutscene;
    public static bool IsInteraction => Instance != null && Instance.CurrentState == GameState.Interaction;
    private void Awake()
    {
        if (Instance != null && Instance != this) {
            Destroy(gameObject); return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void SetState(GameState newState)
    {
        CurrentState = newState;
        UpdatePhysicsCollision(newState);
        OnGameStateChanged?.Invoke(newState);
    }

    private void UpdatePhysicsCollision(GameState state)
    {
        int pLayer = LayerMask.NameToLayer(playerLayerName);
        int eLayer = LayerMask.NameToLayer(enemyLayerName);

        if (pLayer == -1 || eLayer == -1) 
        {
            Debug.LogWarning("GameManager: Layer names not found.");
            return;
        }

        bool ignore = state == GameState.Cutscene;
        Physics2D.IgnoreLayerCollision(pLayer, eLayer, ignore);
    }
}