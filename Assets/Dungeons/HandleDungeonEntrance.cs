using UnityEngine;

public class HandleDungeonEntrance : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    InputHandler inputHandler;
    private void Start()
    {
        inputHandler = InputHandler.Instance;
    }
    private void Update()
    {
        bool interact = inputHandler.InteractTriggered;
        //Debug.Log(interact);
        if(interact){
            Debug.Log("eeeee");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //generate dungeon if none exists 
        Debug.Log("interact triggered");
        GameObject dungeonGrid = GameObject.Find("DungeonGrid");
        dungeonGrid.GetComponent<RenderDungeon>().StartRender(gameObject);
        //newDungeon.StartRender();
        //gameObject.SetActive(false);
        //move player to starting room in dungeon (0,0 point)
        GameObject.Find("Player").transform.position = dungeonGrid.transform.position + new Vector3Int(5, 5, 0);
    }



}
