using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitToTitle : MonoBehaviour
{
    const string TITLE_SCREEN_NAME = "TitleScreen";

    GameObject localPlayer = null;

    private void OnEnable() {
        EventManager.LocalGameObjectPlayerAddedToScene += addLocalPlayer;
    }

    private void OnDisable() {
        EventManager.LocalGameObjectPlayerAddedToScene -= addLocalPlayer;
    }
    void addLocalPlayer(GameObject newLocalPlayer) {
        localPlayer = newLocalPlayer;
    }

    public void Home()
    {
        EventManager.SetLocalGameObjectPlayerLeftScene(localPlayer);
        SceneManager.LoadScene(TITLE_SCREEN_NAME);
    }
}
