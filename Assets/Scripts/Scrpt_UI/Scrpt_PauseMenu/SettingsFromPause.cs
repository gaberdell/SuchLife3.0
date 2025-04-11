using UnityEngine;
using UnityEngine.SceneManagement;

public class SettingsFromPause : MonoBehaviour
{

    void OnEnable()
    {
        EventManager.Clicked += Options;
    }

    void OnDisable()
    {
        EventManager.Clicked -= Options;

    }

    public void Options()
    { 
        SceneManager.LoadScene("OptionsCopy", LoadSceneMode.Additive);
    }
    
}
