using System;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

//attached to Astar, to update the graph in real time
public class GraphUpdater : MonoBehaviour
{
    void OnEnable()
    {
        Tilemap.tilemapTileChanged += OnTilemapChanged;
    }

    void OnDisable()
    {
        Tilemap.tilemapTileChanged -= OnTilemapChanged;
    }

    void OnTilemapChanged(Tilemap tilemap, Tilemap.SyncTile[] ts)
    {
        //updates Astar when unwalkable tilemap changed, in small box around each changed tile
        if (tilemap.gameObject.layer == 7) {
            foreach (Tilemap.SyncTile tile in ts) {
                Bounds tileBounds = new Bounds(tile.position, new Vector2(3, 3));
                AstarPath.active.UpdateGraphs(tileBounds);
            }
        }
    }

}
