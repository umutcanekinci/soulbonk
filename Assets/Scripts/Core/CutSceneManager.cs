using UnityEngine;
using System.Collections;
using VectorViolet.Core.Utilities;

public class CutSceneManager : Singleton<CutSceneManager>
{
    public static void Play(IEnumerator cutsceneRoutine, GameState stateAfterCutscene = GameState.Gameplay)
    {
        if(Instance == null || cutsceneRoutine == null)
            return;

        Instance.StartCoroutine(Instance.CutSceneCoroutine(cutsceneRoutine, stateAfterCutscene));
    }

    private IEnumerator CutSceneCoroutine(IEnumerator cutsceneRoutine, GameState endState)
    {
        GameManager.Instance.SetState(GameState.Cutscene);
        yield return StartCoroutine(cutsceneRoutine);
        GameManager.Instance.SetState(endState);
    }
}