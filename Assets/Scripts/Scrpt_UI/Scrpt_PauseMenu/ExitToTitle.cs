using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitToTitle : MonoBehaviour
{
    const string TITLE_SCREEN_NAME = "TitleScreen";

    GameObject localPlayer = null;

    private void OnEnable() {
        EventManager.LocalGameObjectPlayerAddedToScene += addLocalPlayer;
        EventManager.PlayerExitedGameSoSafeToLeave += ExitGame;
    }

    private void OnDisable() {
        EventManager.LocalGameObjectPlayerAddedToScene -= addLocalPlayer;
        EventManager.PlayerExitedGameSoSafeToLeave -= ExitGame;
    }
    void addLocalPlayer(GameObject newLocalPlayer) {
        localPlayer = newLocalPlayer;
    }

    public void Home()
    {
        Debug.Log("Leaving to tittle screen");
        EventManager.SetLocalGameObjectPlayerLeftScene(localPlayer);
    }
    public void ExitGame() {
        SceneManager.LoadScene(TITLE_SCREEN_NAME);
    }
}
