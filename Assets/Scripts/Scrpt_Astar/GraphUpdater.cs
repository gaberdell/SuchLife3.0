using System;
using UnityEngine;
using UnityEngine.Tilemaps;

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

    void OnTilemapChanged(Tilemap tilemap, Tilemap.SyncTile[] arg2)
    {
        AstarPath.active.Scan();
    }
}
