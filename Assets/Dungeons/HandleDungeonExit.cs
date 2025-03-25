using System;
using UnityEngine;

public class HandleDungeonExit : MonoBehaviour
{
    GameObject entrance;
    InputHandler inputHandler;
    bool interact;
    bool playerInRange;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void setEntrance(GameObject entrance)
    {
        this.entrance = entrance;
        inputHandler = InputHandler.Instance;
        interact = false;
        playerInRange = false;
    }

    private void Update()
    {
        //trigger on true -> false (when key is let go)
        if (interact == true && inputHandler.InteractTriggered == false && playerInRange == true)
        {
            //move player back to entrance
            //newDungeon.StartRender();
            //gameObject.SetActive(false);
            GameObject.Find("Player").transform.position = entrance.transform.position;
        }
        interact = inputHandler.InteractTriggered;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        playerInRange = true;
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        playerInRange = false;
    }
}
