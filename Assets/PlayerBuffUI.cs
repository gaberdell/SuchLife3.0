using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerBuffUI : MonoBehaviour
{
    public GameObject mgBoostIcon;  // Drag the MgBoostIcon GameObject here in Inspector

    private void Start()
    {
        if (mgBoostIcon != null)
            mgBoostIcon.SetActive(false);  // Hide on start
    }

    public void ShowMgBoostIcon(float duration)
    {
        if (mgBoostIcon == null) return;

        mgBoostIcon.SetActive(true);
        StartCoroutine(HideAfterDelay(duration));
    }

    private IEnumerator HideAfterDelay(float duration)
    {
        yield return new WaitForSeconds(duration);
        if (mgBoostIcon != null)
            mgBoostIcon.SetActive(false);
    }
}