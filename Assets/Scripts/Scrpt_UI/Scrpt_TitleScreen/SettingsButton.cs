using UnityEngine;
using UnityEngine.SceneManagement;

public class Settings : MonoBehaviour
{
    [SerializeField]
    private string optionsSceneMenu = "Options";

    public void OpenSettings()
    {
        SceneManager.LoadScene(optionsSceneMenu, LoadSceneMode.Additive);
    }
}
