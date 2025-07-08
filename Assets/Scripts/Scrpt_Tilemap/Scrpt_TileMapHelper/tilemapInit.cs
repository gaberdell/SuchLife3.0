using UnityEngine;
using UnityEngine.Tilemaps;

public class tilemapInit : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Tilemap groundTilemap = GameObject.Find("GroundTilemap").GetComponent<Tilemap>();
        Tilemap wallTilemap = GameObject.Find("PlaceableTileMap").GetComponent<Tilemap>();
        //when tilemap is initialized, load all of it into the chunk manager; allows player to interact with the hardcoded tilemap
        for (int x = groundTilemap.cellBounds.min.x; x < groundTilemap.cellBounds.max.x; x++)
        {
            for (int y = groundTilemap.cellBounds.min.y; y < groundTilemap.cellBounds.max.y; y++)
            {
                for (int z = groundTilemap.cellBounds.min.z; z < groundTilemap.cellBounds.max.z; z++)
                {
                    Vector3Int pos = new Vector3Int(x, y, z);
                    TileBase t = groundTilemap.GetTile(pos);
                    ChunkManager.SetTile(pos, t, false);
                }
            }
        }
        for (int x = wallTilemap.cellBounds.min.x; x < wallTilemap.cellBounds.max.x; x++)
        {
            for (int y = wallTilemap.cellBounds.min.y; y < wallTilemap.cellBounds.max.y; y++)
            {
                for (int z = wallTilemap.cellBounds.min.z; z < wallTilemap.cellBounds.max.z; z++)
                {
                    Vector3Int pos = new Vector3Int(x, y, z);
                    TileBase t = wallTilemap.GetTile(pos);
                    ChunkManager.SetTile(pos, t, true);
                }
            }
        }
    }

    
}
