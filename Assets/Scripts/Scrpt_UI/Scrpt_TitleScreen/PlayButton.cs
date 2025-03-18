using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayButton : MonoBehaviour
{

    void OnEnable()
    {
        EventManager.Clicked += PlayGame;
    }

    void OnDisable()
    {
        EventManager.Clicked -= PlayGame;

    }

    public void PlayGame()
    {
        SceneManager.LoadSceneAsync(2);
    }

    public void Home()
    {
        SceneManager.LoadScene("TitleScreen");
    }
}
