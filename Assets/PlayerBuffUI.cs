using UnityEngine;

public class PlayerBuffUI : MonoBehaviour
{
    public GameObject mgBoostIcon;
    public Transform playerHeadTransform;  // This is where the icon should hover (e.g., player's head or chest)

    private Camera mainCam;

    void Start()
    {
        mainCam = Camera.main;
        if (mgBoostIcon != null)
            mgBoostIcon.SetActive(false);
    }

    void Update()
    {
        if (mgBoostIcon != null && mgBoostIcon.activeSelf && playerHeadTransform != null)
        {
            // Convert world position to screen position
            Vector3 screenPos = mainCam.WorldToScreenPoint(playerHeadTransform.position + Vector3.up * 1.5f);
            mgBoostIcon.transform.position = screenPos;
        }
    }

    public void ShowMgBoostIcon(float duration)
    {
        if (mgBoostIcon != null)
        {
            mgBoostIcon.SetActive(true);
            StartCoroutine(HideAfterDelay(duration));
        }
    }

    private System.Collections.IEnumerator HideAfterDelay(float duration)
    {
        yield return new WaitForSeconds(duration);
        if (mgBoostIcon != null)
            mgBoostIcon.SetActive(false);
    }
}