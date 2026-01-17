using UnityEngine;
using System.Collections;

public class CutSceneManager : MonoBehaviour
{
    public static CutSceneManager Instance { get; private set; }

    public static void StartCutscene(IEnumerator cutsceneRoutine, GameState stateAfterCutscene = GameState.Gameplay)
    {
        if(Instance != null)
            Instance.StartCoroutine(Instance.CutSceneCoroutine(cutsceneRoutine, stateAfterCutscene));
    }

    private void Awake()
    {
        if (Instance == null) { Instance = this; DontDestroyOnLoad(gameObject); }
        else { Destroy(gameObject); }
    }

    private IEnumerator CutSceneCoroutine(IEnumerator cutsceneRoutine, GameState endState)
    {
        GameManager.Instance.SetState(GameState.Cutscene);
        yield return StartCoroutine(cutsceneRoutine);
        GameManager.Instance.SetState(endState);
    }
}