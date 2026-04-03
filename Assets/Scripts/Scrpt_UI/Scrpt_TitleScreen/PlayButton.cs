using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayButton : MonoBehaviour {
    private const string titleScreenName = "TitleScreen";

    private const string singlePlayerWorldName = "WorldCreator";

    private const string multiPlayerWorldName = "ServerMenu";

    public void StartSingleplayer() {
        SceneManager.LoadScene(singlePlayerWorldName);
    }

    public void StartMultiplayer() {
        SceneManager.LoadScene(multiPlayerWorldName);
    }

    public void Home() {
        SceneManager.LoadScene(titleScreenName);
    }
}
