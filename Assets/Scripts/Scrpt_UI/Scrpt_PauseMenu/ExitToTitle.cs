using UnityEngine;
using UnityEngine.SceneManagement;
/* Handles saving and then returning to the Title Screen. This is intended to be used in-game, 
 * on the EXIT button in the pause menu. SaveCurr() is called to save the game state and then 
 * safetly return the player to the title screen.
 * 
 */
public class ExitToTitle : MonoBehaviour
{   
    //Uses OnEnable() and OnDisable() to access Home()
    void OnEnable()
    {
        EventManager.Clicked += Home;
    }

    void OnDisable()
    {
        EventManager.Clicked -= Home;

    }

    //Calls SaveCurr() and then returns to title
    public void Home()
    {
        DataService.SaveCurr();
        SceneManager.LoadScene("TitleScreen");
    }


    void Update()
    {
        
    }
}
