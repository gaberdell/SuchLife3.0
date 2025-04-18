using UnityEngine;
using UnityEngine.SceneManagement;
/* Handles the switching of scenes between the TitleScreen and the test scene / Options. 
 * Also handles going back to the titlescreen from the Options scene.
 * 
 */
public class PlayButton : MonoBehaviour
{
    //uses onEnable() and onDisable() for PlayGame()
    void OnEnable()
    {
        EventManager.Clicked += PlayGame;
    }

    void OnDisable()
    {
        EventManager.Clicked -= PlayGame;

    }
    //Keep the scene build index with test scene in index 2
    public void PlayGame()
    {
        SceneManager.LoadSceneAsync(2);
    }
    // loads scene by string name to get back to the Title Screen
    public void Home()
    {
        SceneManager.LoadScene("TitleScreen");
    }
}
