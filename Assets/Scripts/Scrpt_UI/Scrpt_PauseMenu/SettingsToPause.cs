using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
/*
 * Test script for checking how switching between multiple loaded scenes will work. Currently, it
 * switches between test copies of scenes(Options, test, title) to keep game progress. 
 * Note: audio listener and Event Manager needed to be removed from the copied scene that gets
 * loaded additively in order to work.
 */
public class SettingsToPause : MonoBehaviour
{
    private string sceneName;

    //Uses onEnable() and onDisable() to use returnToGame()
    void OnEnable()
    {
        EventManager.Clicked += returnToGame;
    }

    void OnDisable()
    {
        EventManager.Clicked -= returnToGame;

    }
    private void Start()
    {
      
    }
    //Checks to see if the game play scene is active to determine which scene to switch to
    public void returnToGame()
    {
        Scene gameScene = SceneManager.GetSceneByName("RyanBackuptestScene");
        string name = gameScene.name;
        //checks to see if Options was loaded from the gameplay scene
        if (name == "RyanBackuptestScene")
        {
            Debug.Log("Hello1");
            SceneManager.UnloadSceneAsync("OptionsCopy");
        }
        else
        {
            //Switches back to the copy of the Title Screen if game play was not active
            SceneManager.LoadScene("TitleScreenCopy");
        }
    }
}
