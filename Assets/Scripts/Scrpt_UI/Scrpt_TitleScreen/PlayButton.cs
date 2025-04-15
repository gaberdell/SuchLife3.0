using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayButton : MonoBehaviour
{
    [SerializeField]
    private string titleScreenName = "TitleScreen";

    public void PlayGame()
    {
        SceneManager.LoadSceneAsync(2);
    }

    public void Home()
    {
        SceneManager.LoadScene(titleScreenName);
    }
}
