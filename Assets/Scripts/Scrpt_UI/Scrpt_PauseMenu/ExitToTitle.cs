using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitToTitle : MonoBehaviour
{

    public void Home()
    {
        //DataService.SaveCurr();
        SceneManager.LoadScene("TitleScreen");
    }
}
