using UnityEngine;

public class PauseGame : MonoBehaviour
{
    public GameObject PauseCanvas;
    public GameObject Camera;
    public bool IsPaused = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey("escape"))
        {
            togglePause();

            if (IsPaused)
            {

                PauseCanvas.SetActive(true);
                IsPaused = true;

            }
            else
            {
                PauseCanvas.SetActive(false);
                IsPaused = false;
            }


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


