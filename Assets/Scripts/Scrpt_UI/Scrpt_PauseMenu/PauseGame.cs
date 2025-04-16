using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseGame : MonoBehaviour
{
    [SerializeField] 
    private GameObject pauseCanvas;
    [SerializeField]
    private float diffsecs;
    private bool isPaused = false;
    private bool switching = false;
    private bool isOptionsMenuLoaded = false;

    [SerializeField]
    private string titleScreen = "TitleScreen";

    [SerializeField]
    private string optionsSceneMenu = "Options";

    [SerializeField]
    private string optionsSceneMenuExitButton = "ExitButtonForOptionsMenu";

    InputHandler inputhandler;

    private void OnEnable()
    {
        SceneManager.sceneLoaded += onPauseMenuDoneLoading;
        SceneManager.sceneUnloaded += onPauseMenuDoneUnloading;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= onPauseMenuDoneLoading;
        SceneManager.sceneUnloaded -= onPauseMenuDoneUnloading;
    }

    void onPauseMenuDoneLoading(Scene pauseMenu, LoadSceneMode loadSceneMode)
    {
        if (pauseMenu.name == optionsSceneMenu)
        {
            isOptionsMenuLoaded = true;
        }
    }

    void onPauseMenuDoneUnloading(Scene pauseMenu)
    {
        if (pauseMenu.name == optionsSceneMenu)
        {
            isOptionsMenuLoaded = false;
        }
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        inputhandler = InputHandler.Instance;  
    }
    
    // Update is called once per frame
    void Update()
    {
        if (!switching && inputhandler.EscapeTriggered)
        {
            switching = true;
            togglePause();

            if (!isPaused)
            {
                
                pauseCanvas.SetActive(true);
                isPaused = true;

            }
            else
            {
                pauseCanvas.SetActive(false);
                isPaused = false;

                if (isOptionsMenuLoaded)
                {
                    SceneManager.UnloadSceneAsync(optionsSceneMenu);
                }
            }
            


        }
        else if (switching && !inputhandler.EscapeTriggered)
        {
            switching = false;
        }

    }

    public void OpenSettings()
    {
        SceneManager.LoadScene(optionsSceneMenu, LoadSceneMode.Additive);

    }

    public void ExitToTitle()
    {
        if (isPaused)
        {
            isPaused = togglePause();
        }

        SceneManager.LoadScene(titleScreen);
    }

    public bool togglePause()
    {
        if (Time.timeScale == 0f)
        {
            Time.timeScale = 1f;
            return (false);
        }
        else
        {
            Time.timeScale = 0f;
            return (true);
        }
    }

}


