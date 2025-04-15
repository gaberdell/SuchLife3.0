using System;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

public class GraphUpdater : MonoBehaviour
{
    // private bool dungeonCreated = false;

    void OnEnable()
    {
        // EventManager.PlayerEnterDungeon += OnDungeonCreated;
        Tilemap.tilemapTileChanged += OnTilemapChanged;
    }

    void OnDisable()
    {
        // EventManager.PlayerEnterDungeon -= OnDungeonCreated;
        Tilemap.tilemapTileChanged -= OnTilemapChanged;
    }

    // void OnDungeonCreated()
    // {
    //     Invoke("InitialScan", 1);
    // }

    // void InitialScan()
    // {
    //     AstarPath.active.Scan();
    //     dungeonCreated = true;
    // }

    void OnTilemapChanged(Tilemap tilemap, Tilemap.SyncTile[] ts)
    {
        // if (dungeonCreated) {
        Bounds tileBounds = new Bounds(ts[0].position, new Vector2(3, 3));
        AstarPath.active.UpdateGraphs(tileBounds);
        // }
    }

}
