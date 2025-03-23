using UnityEngine;

public class HandleDungeonEntrance : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    InputHandler inputHandler;
    bool interact;
    bool playerInRange;
    private void Start()
    {
        inputHandler = InputHandler.Instance;
        interact = false;
        playerInRange = false;
    }
    private void Update()
    {

        //trigger on true -> false (when key is let go)
        if(interact == true && inputHandler.InteractTriggered == false && playerInRange == true){
            Debug.Log("eeeee");
            //generate dungeon if none exists 
            GameObject dungeonGrid = GameObject.Find("DungeonGrid");
            dungeonGrid.GetComponent<RenderDungeon>().StartRender(gameObject);
            //gameObject.SetActive(false);
            //move player to starting room in dungeon (0,0 point)
            GameObject.Find("Player").transform.position = dungeonGrid.transform.position + new Vector3Int(5, 5, 0);
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
