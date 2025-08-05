using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class PlayerBuffUI : MonoBehaviour
{
    public GameObject mgBoostIcon;        // Drag the MgBoostIcon GameObject here in Inspector
    public TextMeshProUGUI timerText;     // Drag the TimerText (child of mgBoostIcon) here

    private Coroutine activeTimerRoutine;

    private void Start()
    {
        if (mgBoostIcon != null)
            mgBoostIcon.SetActive(false);  // Hide on start

        if (timerText != null)
            timerText.text = "";           // Clear text on start
    }

    public void ShowMgBoostIcon(float duration)
    {
        if (mgBoostIcon == null) return;

        // Stop any previous timer
        if (activeTimerRoutine != null)
            StopCoroutine(activeTimerRoutine);

        mgBoostIcon.SetActive(true);
        activeTimerRoutine = StartCoroutine(Countdown(duration));
    }

    private IEnumerator Countdown(float duration)
    {
        float remaining = duration;

        while (remaining > 0)
        {
            if (timerText != null)
                timerText.text = Mathf.Ceil(remaining).ToString();  // Show seconds left

            yield return new WaitForSeconds(1f);
            remaining -= 1f;
        }

        if (timerText != null)
            timerText.text = "";

        if (mgBoostIcon != null)
            mgBoostIcon.SetActive(false);
    }
}