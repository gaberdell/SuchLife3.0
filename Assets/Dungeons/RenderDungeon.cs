using UnityEngine;
using UnityEngine.Tilemaps;

public class RenderDungeon : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] Tilemap dungeonTilemap;
    [SerializeField] Tile grassTile;
    [SerializeField] Tile wallTile;
    [SerializeField] Tile bonusTile;

    void Start(){
        DungeonOptions opts = new DungeonOptions();
        //create dungeon graph
        DungeonGraph nodeGraph = new DungeonGraph(opts);
        //for basic testing
        //for each node in the graph, set the tile with the same index to a grasstile
        for (int i = 0; i < nodeGraph.layout.Count; i++)
        {
            DungeonNode node = nodeGraph.layout[i];
            if(node.type == "Start")
            {
                dungeonTilemap.SetTile(new Vector3Int(node.xPos * 4, node.yPos * 4, 0), bonusTile);
            } else
            {
                dungeonTilemap.SetTile(new Vector3Int(node.xPos * 4, node.yPos * 4, 0), grassTile);
            }
            
            //draw edges
            for (int j = 0; j < node.edges.Count; j++)
            {
                //draw em somehow
                switch (node.edges[j].Item2)
                {
                    case "top":
                        dungeonTilemap.SetTile(new Vector3Int(node.xPos * 4, node.yPos * 4 - 2, 0), wallTile);
                        break;
                    case "bottom":
                        dungeonTilemap.SetTile(new Vector3Int(node.xPos * 4, node.yPos * 4 + 2, 0), wallTile);
                        break;
                    case "right":
                        dungeonTilemap.SetTile(new Vector3Int(node.xPos * 4 + 2, node.yPos * 4, 0), wallTile);
                        break;
                    case "left":
                        dungeonTilemap.SetTile(new Vector3Int(node.xPos * 4 - 2, node.yPos * 4, 0), wallTile);
                        break;
                }
                
            }
        }
    }
}
