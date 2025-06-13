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
    bool hasBeenRenderedOnce = false;
    TileBase wallTile; //tile used to fill walls; leave empty if not needed (not dungeon chunk)
    TileBase[] floorTile; //tiles used to fill floor beneath filled walls; leave empty if not dungeon
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
        //if has not been rendered before then perform fill operations
        if (!hasBeenRenderedOnce && wallTile != null)
        {
            fillChunkWalls(wallTilemap);
            hasBeenRenderedOnce = true;
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


    public void setFillInfo(TileBase wallT)
    {
        wallTile = wallT;
    }

    //fill walls when rendering chunk (for the first time) so it doesnt have to be done all at once
    public void fillChunkWalls(Tilemap wallTilemap)
    {
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                //if no floor tile then set a wall
                if (floorTiles[i][j] == null)
                {
                    wallTiles[i][j] = wallTile;
                }
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

    public int distanceFromChunk(Chunk otherChunk)
    {
        Vector2 c2pos = otherChunk.getPos();
        //diagonals are counted as 1 distance; take the max between x and y distance
        int distance = (int) Mathf.Max(Mathf.Abs(chunkX - c2pos.x), Mathf.Abs(chunkY - c2pos.y));
        return distance;
    }

}
