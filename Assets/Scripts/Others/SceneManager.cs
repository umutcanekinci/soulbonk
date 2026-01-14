using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using UnityEngine.InputSystem;
public class SceneManager : MonoBehaviour
{
    // [SerializeField] private FlashEffect flashEffect;
    [SerializeField] private GameObject menuContainer;
    [SerializeField] private int mainMenuIndex = 0;
    [SerializeField] private int pauseMenuIndex = 1;
    public static SceneManager Instance;
    private int menuIndex = 0;
    private List<GameObject> menus;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            if (menuContainer != null)
            {
                menus = new List<GameObject>();
                foreach (Transform child in menuContainer.transform)
                {
                    menus.Add(child.gameObject);
                }
            }

        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            HandleBackButton();
        }
    }
    
    private void HandleBackButton()
    {
        string currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;

        if (currentScene == "GameScene")
        {
            OpenMenu(pauseMenuIndex);
        }
        else if (currentScene == "MainMenu")
        {
            if (menuIndex == 0)
            {
                Quit();
            }
            else
            {
                OpenPreviousMenu();
            }
        }
    }
    
    public void OpenPreviousMenu()
    {
        if (IsMenuOpen(mainMenuIndex))
        {
            Quit();
        }
        else if (IsMenuOpen(pauseMenuIndex))
        {
            CloseAllMenus();
        }
        else
        {
            OpenMenu(mainMenuIndex);
        }
        
    }

    public bool IsSceneLoaded(string sceneName)
    {
        return UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == sceneName;
    }

    public bool IsMenuOpen(int index)
    {
        return menuIndex == index;
    }

    public void OpenMenu(int index)
    {
        if (menus == null)
            return;

        if (index >= 0 && index < menus.Count)
        {
            for (int i = 0; i < menus.Count; i++)
            {
                menus[i].SetActive(i == index);
            }
            menuIndex = index;
        }
        else
        {
            Debug.LogError("Menu index out of range.");
        }
    }

    public void LoadScene(string sceneName)
    {
        if (IsSceneLoaded(sceneName))
            return;

        if (Instance != null)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
        }
        else
        {
            Debug.LogError("SceneLoader not found in the scene.");
        }
    }

    public void LoadMainMenu()
    {
        LoadScene("MenuScene");
        OpenMenu(mainMenuIndex);
    }

    public void LoadGameScene()
    {
        // flashEffect.StartFlash(() => {
            LoadScene("GameScene");
            CloseAllMenus();
        // });
    }

    public void CloseAllMenus()
    {
        if (menus == null)
            return;

        foreach (var menu in menus)
        {
            menu.SetActive(false);
        }
        menuIndex = 0;
    }

    public void GameOver()
    {
        LoadMainMenu();
    }

    public void Quit()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }
}
