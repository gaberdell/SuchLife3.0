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
    bool rendered = false;
    List<List<TileBase>> floorTiles = new List<List<TileBase>>();
    List<List<TileBase>> wallTiles = new List<List<TileBase>>();

    public Chunk(int csize)
    {
        size = csize;

        for (int i = 0; i < size; i++)
        {
            //fill chunk with null tiles
            List<TileBase> emptyList1 = new List<TileBase>();
            List<TileBase> emptyList2 = new List<TileBase>();
            for (int j = 0; j < size; j++)
            {
                emptyList1.Add(null);
                emptyList2.Add(null);
            }
            floorTiles.Add(emptyList1);
            wallTiles.Add(emptyList2);
        }
    }

    //

    public void insertTile(int x, int y, TileBase tile, bool isWall)
    {
        if(isWall)
        {
            wallTiles[y][x] = tile;
        } else
        {
            floorTiles[y][x] = tile;
        }
    }

    public TileBase getTile(int x, int y, bool isWall)
    {
        if (!isWall)
        {
            return floorTiles[y][x];
        }
        else
        {
            return wallTiles[y][x];
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
        //if already rendered, then don't. 
        if (rendered)
        {
            return;
        }
        rendered = true;
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                TileBase ftile = floorTiles[i][j];
                TileBase wtile = wallTiles[i][j];
                groundTilemap.SetTile(new Vector3Int(worldX + j, worldY + i, 0), ftile);
                wallTilemap.SetTile(new Vector3Int(worldX + j, worldY + i, 0), wtile);
            }
        }
    }

    public void unload(Tilemap groundTilemap, Tilemap wallTilemap)
    {
        rendered = false;
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                groundTilemap.SetTile(new Vector3Int(worldX + j, worldY + i, 0), null);
                wallTilemap.SetTile(new Vector3Int(worldX + j, worldY + i, 0), null);
            }
        }
    }

    public Vector2 getPos()
    {
        return new Vector2(chunkX, chunkY);
    }

}
