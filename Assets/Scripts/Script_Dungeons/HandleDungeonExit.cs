using System;
using UnityEngine;

public class HandleDungeonExit : MonoBehaviour
{
    GameObject entrance;
    InputHandler inputHandler;
    bool interact;
    GameObject nearbyPlayer;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void setEntrance(GameObject entrance)
    {
        this.entrance = entrance;
        inputHandler = InputHandler.Instance;
        interact = false;
        
    }

    private void Update()
    {
        //trigger on true -> false (when key is let go)
        if (interact == true && inputHandler.InteractTriggered == false && nearbyPlayer != null)
        {
            //move player back to entrance
            //newDungeon.StartRender();
            //gameObject.SetActive(false);
            nearbyPlayer.transform.position = entrance.transform.position;
            EventManager.SetPlayerExitDungeon();
        }
        interact = inputHandler.InteractTriggered;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            nearbyPlayer = collision.gameObject;
        }
        
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            nearbyPlayer = null;
        }
    }
}
