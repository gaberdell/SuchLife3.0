using UnityEngine;

public class HandleDungeonEntrance : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    InputHandler inputHandler;
    bool interact;
    bool playerInRange;
    GameObject dungeonGrid;
    int dungeonOffsetX = 100;
    int dungeonOffsetY = 100;
    private void Start()
    {
        inputHandler = InputHandler.Instance;
        interact = false;
        playerInRange = false;
        //subscribe to enter dungeon event
        EventManager.PlayerEnterDungeon += enterDungeon;

    }
    private void Update()
    {

        //trigger on true -> false (when key is let go)
        if(interact == true && inputHandler.InteractTriggered == false && playerInRange == true){
            //generate dungeon if none exists 
            GameObject dungeonGrid = GameObject.Find("DungeonGrid");
            dungeonGrid.GetComponent<RenderDungeon>().StartRender(gameObject, dungeonOffsetX, dungeonOffsetY);
        }
        interact = inputHandler.InteractTriggered;
    }

    private void enterDungeon()
    {
        //triggered from event manager
        //move player to starting room in dungeon (0,0 point)
        GameObject.Find("Player").transform.position = GameObject.Find("Grid").transform.position + new Vector3Int(dungeonOffsetX + 5, dungeonOffsetY + 5, 0);
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
