using System.Collections;
using TMPro;
using UnityEngine;

public class TextPopupWhenNear : MonoBehaviour
{
    // This class can be attached to an object to trigger text when a trigger is detected
    // This class is used in the playerTextCollider prefab to display text in the scene when the player comes nearby 
    TextMeshPro textToDisplay;
    [SerializeField] string message;
    int nearbyPlayerCount = 0; //keep track of players so the text will be visible by all players whenever one is nearby
    bool isActive = false;
    void Start()
    {
        //grab components
        textToDisplay = GetComponent<TextMeshPro>();
        textToDisplay.text = "";
    }

    private IEnumerator animateTextDisplay()
    {
        //display one letter at a time 
        float timePerChar = 0.05f;
        for (int i = 0; i <= message.Length; i++)
        {
            textToDisplay.text = message.Substring(0,i);
            yield return new WaitForSeconds(timePerChar);
        }
        
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            nearbyPlayerCount++;
            if (!isActive)
            {
                Debug.Log("display message");
                StartCoroutine(animateTextDisplay());
                isActive = true;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            nearbyPlayerCount--;
            if(nearbyPlayerCount <= 0)
            {
                isActive = false;
                textToDisplay.text = "";
            }
        }
    }
}
