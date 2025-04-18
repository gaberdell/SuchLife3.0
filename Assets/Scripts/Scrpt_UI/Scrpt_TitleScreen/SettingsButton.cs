using UnityEngine;
using UnityEngine.SceneManagement;
/*
 * Switches between the title Screen and the Options menu. 
 */
public class Settings : MonoBehaviour
{
    //uses OnEnable() and OnDisable() to use OpenSettings()
    void OnEnable()
    {
        EventManager.Clicked += OpenSettings;
    }

    void OnDisable()
    {
        EventManager.Clicked -= OpenSettings;

    }

    // Uses the scene build index to switch to the Options scene at index 1
    public void OpenSettings()
    {
        SceneManager.LoadSceneAsync(1);
    }
}
