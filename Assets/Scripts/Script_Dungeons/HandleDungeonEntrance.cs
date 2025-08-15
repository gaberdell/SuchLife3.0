using UnityEngine;

public class HandleDungeonEntrance : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    InputHandler inputHandler;
    bool interact;
    bool playerInRange;
    bool rendered;
    GameObject dungeonGrid;
    [SerializeField] TextAsset optionsFile;
    DungeonOptions opts;

    private void Start()
    {
        inputHandler = InputHandler.Instance;
        interact = false;
        playerInRange = false;
        opts = JsonUtility.FromJson<DungeonOptions>(optionsFile.text);
        rendered = false;

    }
    private void Update()
    {
        //trigger on true -> false (when key is let go)
        if(interact == true && inputHandler.InteractTriggered == false && playerInRange == true){
            //generate dungeon if none exists 
            GameObject dungeonGrid = GameObject.Find("Grid");
            if (!rendered)
            {
                dungeonGrid.GetComponent<RenderDungeon>().StartRender(gameObject, opts.dungeonOffsetX, opts.dungeonOffsetY, opts);
                rendered = true;
                enterDungeon();
            } else
            {
                enterDungeon();
            }

        }
        interact = inputHandler.InteractTriggered;
    }

    private void enterDungeon()
    {
        //triggered from event manager
        //move player to starting room in dungeon (0,0 point)
        GameObject.Find("Player").transform.position = GameObject.Find("Grid").transform.position + new Vector3Int(opts.dungeonOffsetX + 5, opts.dungeonOffsetY + 5, 0);
        //ChunkManager.renderChunks(GameObject.Find("Player").transform.position);
        //ChunkManager.renderAll();
        //ChunkManager.renderPlayerChunks();
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
