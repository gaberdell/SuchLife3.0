using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitToTitle : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void OnEnable()
    {
        EventManager.Clicked += Home;
    }

    void OnDisable()
    {
        EventManager.Clicked -= Home;

    }

    void Start()
    {
        //Call save game
    }
    public void Home()
    {
        SceneManager.LoadScene("TitleScreen");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
