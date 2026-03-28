using UnityEngine;

public class PauseTimeHandler
{
    static bool isPaused = false;
    static float UNPAUSED_TIME = 1.0f;
    static float PAUSED_TIME = 0.0f;
    static float previousTime = 1.0f;

    public static void TogglePauseGame() {
        if (isPaused) {
            UnPauseGame();
        }
        else {
            PauseGame();
        }
    }
    public static void PauseGame() {
        if (!DataService.IsMultiplayer) {
            previousTime = Time.timeScale;
            Time.timeScale = PAUSED_TIME;
            isPaused = true;
        }
    }

    public static void UnPauseGame() {
        if (!DataService.IsMultiplayer) {
            Time.timeScale = UNPAUSED_TIME;
            isPaused = false;
        }
    }
}
