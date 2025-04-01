using System;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GraphUpdater : MonoBehaviour
{
    public bool jankScanGraphEveryTime;
    bool dungeonCreated = false;

    void OnEnable()
    {
        EventManager.PlayerEnterDungeon += OnDungeonCreated;
        Tilemap.tilemapTileChanged += OnTilemapChanged;
    }

    void OnDisable()
    {
        EventManager.PlayerEnterDungeon -= OnDungeonCreated;
        Tilemap.tilemapTileChanged -= OnTilemapChanged;
    }

    void OnDungeonCreated()
    {
        Invoke("InitialScan", 1);
        
        Debug.Log("TILEMAP CHANGED");
    }

    void InitialScan()
    {
        AstarPath.active.Scan();
        dungeonCreated = true;
    }

    void OnTilemapChanged(Tilemap tilemap, Tilemap.SyncTile[] arg2)
    {
        if (dungeonCreated && jankScanGraphEveryTime)
            AstarPath.active.Scan();
    }

}
