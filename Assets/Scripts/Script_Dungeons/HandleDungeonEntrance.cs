using UnityEditor.Profiling.Memory.Experimental;
using UnityEngine;

public class HandleDungeonEntrance : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    InputHandler inputHandler;
    bool interact;
    bool rendered;
    GameObject dungeonGrid;
    [SerializeField] TextAsset optionsFile;
    DungeonOptions opts;
    GameObject grid;
    GameObject nearbyPlayer;

    private void Start()
    {
        inputHandler = InputHandler.Instance;
        interact = false;
        opts = JsonUtility.FromJson<DungeonOptions>(optionsFile.text);
        rendered = false;
        grid = GameObject.FindGameObjectWithTag("Grid");

    }
    private void Update()
    {
        //trigger on true -> false (when key is let go)
        if(interact == true && inputHandler.InteractTriggered == false && nearbyPlayer != null){
            //generate dungeon if none exists 
            if (!rendered)
            {
                Debug.Log("Sus46 : " + dungeonGrid);
                grid.GetComponent<RenderDungeon>().StartRender(gameObject, opts.dungeonOffsetX, opts.dungeonOffsetY, opts);
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
        if(nearbyPlayer == null)
        {
            Debug.LogError("trying to enter dungeon while no player is detected nearby!");
            return;
        }
        //triggered from event manager
        //move player to starting room in dungeon (0,0 point)
        nearbyPlayer.transform.position = grid.transform.position + new Vector3Int(opts.dungeonOffsetX + 5, opts.dungeonOffsetY + 5, 0);
        //ChunkManager.renderChunks(GameObject.Find("Player").transform.position);
        //ChunkManager.renderAll();
        //ChunkManager.renderPlayerChunks();
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
