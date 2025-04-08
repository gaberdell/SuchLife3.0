using Unity.VisualScripting;
using UnityEngine;

public class PauseGame : MonoBehaviour
{
    [SerializeField] 
    private GameObject pauseCanvas;
    [SerializeField]
    private float diffsecs;
    private bool isPaused = false;
    private bool switching = false;

    InputHandler inputhandler;

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
            }
            


        }
        else if (switching && Input.GetKeyUp(KeyCode.Escape))
        {
            switching = false;
        }

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


