using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public class SettingsToPause : MonoBehaviour
{
    private string sceneName;

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

    public void returnToGame()
    {
        Scene gameScene = SceneManager.GetSceneByName("RyanBackuptestScene");
        string name = gameScene.name;
        if (name == "RyanBackuptestScene")
        {
            SceneManager.SetActiveScene(gameScene);
        }
        else
        {
            SceneManager.LoadScene("TitleScene");
        }
    }
}
