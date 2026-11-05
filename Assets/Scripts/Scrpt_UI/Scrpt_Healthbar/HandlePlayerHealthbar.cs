using UnityEngine;
using UnityEngine.UI;

public class HandlePlayerHealthbar : MonoBehaviour
{
    // this script is attached to the healthbar ui element parent
    const int maskWidth = 412; //i do not know why this value is different from the width of the mask gameobject. 

    RectMask2D Mask;
    Health playerHealth;
    void Start()
    {
        Mask = transform.Find("HealthFilledMask").gameObject.GetComponent<RectMask2D>();
    }
    //listen for player
    private void OnEnable()
    {
        EventManager.PlayerHurt += updateHealth;
    }

    private void OnDisable()
    {
        EventManager.PlayerHurt -= updateHealth;
    }

    void updateHealth(GameObject Player)
    {
        //called when player takes damage
        //Debug.Log("HANDLE PLYER HEALTHBAR");
        if (playerHealth == null)
        {
            playerHealth = Player.GetComponent<Health>();
        }

        //get fraction of health left
        float healthPercentRemaining = (float)playerHealth.currentHealth / (float)playerHealth.maxHealth;
        Debug.Log($"player health percent remaining: {healthPercentRemaining}");

        //adjust healthbar mask appropriately
        //mask mask buffer right goes between 8 (full health) and 420 (no health)
        Mask.padding = new Vector4(0f, 0f, ((maskWidth * Mathf.Abs(1 - healthPercentRemaining) + 8f)), 0f); //left, bottom, right, top

    }
}
