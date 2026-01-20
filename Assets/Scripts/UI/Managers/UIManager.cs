using UnityEngine;

public class UIManager : MonoBehaviour
{
    public void OnPlayButtonPressed() => SceneLoader.Load(SceneType.Game);

    public void OnQuitButtonPressed() => SceneLoader.Quit();
}