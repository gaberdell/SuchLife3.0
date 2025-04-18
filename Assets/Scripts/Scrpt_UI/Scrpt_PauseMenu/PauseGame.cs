using Unity.VisualScripting;
using UnityEngine;
/* Handles pausing the game to bring up the pause menu. The pause menu is now a canvas in the
 * same scene as the game play. The scene is set to inactive, and made active by pressing the
 * escape key. The update() uses togglePause(), which simply freezes and unfreezes game time.
 * Added a feature to click on a return button in the pause menu, which behaves just like hitting 
 * escape does. */ 
public class PauseGame : MonoBehaviour
{
    //Made pause canvas object Serialize Field for convenience.
    [SerializeField] 
    private GameObject pauseCanvas;
    private bool isPaused = false;
    private bool switching = false;

    // uses InputHanlder to deal with the escape key press
    InputHandler inputhandler;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        inputhandler = InputHandler.Instance;
        
    }
    
    // Update checks for escape key input and checks isPaused to determine what action to take
    void Update()
    {
        if (!switching && inputhandler.EscapeTriggered)
        {
            switching = true;
            //pause or unpause the game regardless
            togglePause();

            if (!isPaused)
            {
                //activate the pause menu if it is inactive
                pauseCanvas.SetActive(true);
                isPaused = true;

            }
            else
            {
                // deactivate the pause menu if it was active
                pauseCanvas.SetActive(false);
                isPaused = false;
            }
            


        }
        else if (switching && Input.GetKeyUp(KeyCode.Escape))
        {
            switching = false;
        }

}   //Mimics the escape key functionality for players who want to use the mouse to unpause
    public void clickUnpause()
    {
        togglePause();
        pauseCanvas.SetActive(false);
        isPaused = false;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
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


