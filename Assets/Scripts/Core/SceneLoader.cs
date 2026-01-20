using UnityEngine.SceneManagement;

public enum SceneType
{
    Menu,
    Game
}

public static class SceneLoader
{
    public static bool IsLoaded(string sceneName)
    {
        return SceneManager.GetActiveScene().name == sceneName;
    }

    public static void Load(SceneType sceneType)
    {
        Load(sceneType.ToString() + "Scene");
    }

    public static void Load(string sceneName)
    {
        if (IsLoaded(sceneName))
            return;

        SceneManager.LoadScene(sceneName);
    }

    public static void Quit()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }

}
