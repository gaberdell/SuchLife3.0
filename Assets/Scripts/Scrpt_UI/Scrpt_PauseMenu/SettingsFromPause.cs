using UnityEngine;
using UnityEngine.SceneManagement;
/* Used for testing purposes of switching between scenes loaded additively. 
 * Script switches to the copy of the Options scene. The goal is to not have
 * to reload the game play scene every time Options is accessed from the pause menu.
 */
public class SettingsFromPause : MonoBehaviour
{
    //use onEnable() and onDisable()
    void OnEnable()
    {
        EventManager.Clicked += Options;
    }

    void OnDisable()
    {
        EventManager.Clicked -= Options;

    }

    //Loads a new scene using "LoadSceneMode.Additive" - keep two scenes active at once to return to gameplay after
    public void Options()
    { 
        SceneManager.LoadScene("OptionsCopy", LoadSceneMode.Additive);
    }
    
}
