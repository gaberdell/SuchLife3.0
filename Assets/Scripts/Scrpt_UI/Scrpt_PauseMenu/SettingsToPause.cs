using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public class SettingsToPause : MonoBehaviour
{
    [SerializeField]
    private string sceneName = "Options";

    public void returnToGame()
    {
        SceneManager.UnloadSceneAsync(sceneName);
    }
}
