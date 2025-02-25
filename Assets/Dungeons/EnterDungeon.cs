using UnityEngine;

public class EnterDungeon : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        DungeonOptions opts = new DungeonOptions();
        //create dungeon graph
        DungeonGraph myGraph = new DungeonGraph(opts);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
