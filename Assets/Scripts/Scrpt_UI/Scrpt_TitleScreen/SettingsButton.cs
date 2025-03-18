using UnityEngine;
using UnityEngine.SceneManagement;

public class Settings : MonoBehaviour
{

    void OnEnable()
    {
        EventManager.Clicked += OpenSettings;
    }

    void OnDisable()
    {
        EventManager.Clicked -= OpenSettings;

    }

    public void OpenSettings()
    {
        SceneManager.LoadSceneAsync(1);
    }
}
