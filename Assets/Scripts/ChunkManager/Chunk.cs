using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Chunk //: MonoBehaviour
{
    private int worldX; //location in the unity scene
    private int worldY;
    private int chunkX; //location relative to other chunks (in the grid of chunks)
    private int chunkY;
    public int size;
    List<List<TileBase>> floorTiles = new List<List<TileBase>>();
    List<List<TileBase>> wallTiles = new List<List<TileBase>>();

    public Chunk(int csize)
    {
        size = csize;
        //fill chunk with null tiles
        List<TileBase> emptyList = new List<TileBase>();
        for (int j = 0; j < size; j++)
        {
            emptyList.Add(null);
        }
        for (int i = 0; i < size; i++)
        {
            floorTiles.Add(emptyList);
            wallTiles.Add(emptyList);
        }
    }

    //

    public void insertTile(int x, int y, TileBase tile)
    {
        //determine whether tile is a ruletile or not (add rule tiles to wall grid)
        //chunkTiles[y][x] = tile;
    }

    public TileBase getTile(int x, int y, bool isFloor)
    {
        if (isFloor)
        {
            return floorTiles[x][y];
        }
        else
        {
            return wallTiles[x][y];
        }
    }

    public void fillInfo(int cx, int cy, int wx, int wy)
    {
        chunkX = cx;
        chunkY = cy;
        worldX = wx;
        worldY = wy;
    }

    public void render(Tilemap groundTilemap, Tilemap wallTilemap)
    {
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                TileBase ftile = floorTiles[i][j];
                TileBase wtile = wallTiles[i][j];
                groundTilemap.SetTile(new Vector3Int(worldX + i, worldY + j, 0), ftile);
                wallTilemap.SetTile(new Vector3Int(worldX + i, worldY + j, 0), wtile);
            }
        }
    }

}
