using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Tilemaps;
using UnityEngine.WSA;
using Codice.CM.Common;

public static class ChunkManager 
{
    // all processes that want to add a tile to the scene will be redirected to the chunk manager
    // the chunk manager will store the changes made to the tilemap but will not display them;
    // instead, the manager will only render tiles that are within a vicinity of the player, using 'chunks' of tiles.

    // changes to the tilemap will be stored as a custom tileChange object
    // tileChanges will be sorted into 'chunks' of size n
    // dungeons will be handled as instances with their own chunks

    const int chunkSize = 4; //width & height of chunk in tiles;
    const int renderDistance = 4; //amount of chunks away from player's chunk to render (e.g. if 2, then render 8 chunks around player's chunk)
    //const int offset = 500; //add this number to world positions read into and from this manager; used to offset the 0,0 point of the manager so negative cases don't need to be considered.
    static Vector3Int worldPos = new Vector3Int(0, 0, 0); //position of chunk manager in world (should match the tilemaps'
    static List<List<Chunk>> chunkGrid = new List<List<Chunk>>(); //container for world chunks
    static HashSet<Chunk> loadedChunks = new HashSet<Chunk>(); //keep track of loaded chunks to unload them when player renders new chunks.

    static Tilemap wallTilemap = GameObject.Find("PlaceableTileMap").GetComponent<Tilemap>();
    static Tilemap groundTilemap = GameObject.Find("GroundTilemap").GetComponent<Tilemap>();

    // INSTANTIATION METHODS
    static public void SetTile(Vector3Int tilePos, TileBase tile, bool isWall)
    {
        //figure out what chunk to place the tile into
        //0th 0th chunk starts at bottom left corner? top left?
        //input is the position on the tilemap

        int chunkX =  tilePos.x / chunkSize;
        int chunkY = tilePos.y / chunkSize;
        int xInChunk = tilePos.x % chunkSize;
        int yInChunk = tilePos.y % chunkSize;

        //if out of bounds then add empty chunks
        while(chunkY+1 > chunkGrid.Count)
        {
            chunkGrid.Add(new List<Chunk>());
        }
        while(chunkX+1 > chunkGrid[chunkY].Count)
        {
            chunkGrid[chunkY].Add(new Chunk(chunkSize));
        }
        fillChunkInfo();

        //Chunk targetChunk = chunkGrid[chunkY][chunkX]; //index out of range?
        Chunk targetChunk = getChunkFromWorld(tilePos);

        //place in chunk
        if(targetChunk != null) targetChunk.insertTile(xInChunk, yInChunk, tile, isWall);
    }

    //set fill method for chunks
    static public void setChunksFill(int x, int y, int width, int height, TileBase fillTile)
    {
        for (int i = x; i < x + width; i+= chunkSize)
        {
            for(int j = y; j < y + height; j+= chunkSize)
            {
                Chunk currChunk = getChunkFromWorld(new Vector3(i, j, 0));
                if (currChunk != null)
                {
                    currChunk.setFillInfo(fillTile);
                } else
                {
                    //Vector2 pos = getChunkPosFromWorld(new Vector3(i, j, 0));
                    //if chunk is null then create a chunk at that index to be filled completely
                    SetTile(new Vector3Int(i, j, 0), fillTile, true);
                    Chunk newChunk = getChunkFromWorld(new Vector3(i, j, 0));
                    newChunk.setFillInfo(fillTile);
                }
                
            }
        }

    }

    static public void addEntityToChunk(Mob entity, Vector3Int worldPos)
    {
        //get chunk

        //place entity in chunk
    }



    // HELPERS
    static private void fillChunkInfo()
    {
        for (int i = 0; i < chunkGrid.Count; i++)
        {
            for (int j = 0; j < chunkGrid[i].Count; j++)
            {
                Chunk c = chunkGrid[i][j];
                c.fillInfo(j, i, j * chunkSize, i * chunkSize);
            }
        }
    }

    static public TileBase GetTile(Vector3Int tilePos, bool isWall)
    {
        int chunkX = tilePos.x / chunkSize;
        int chunkY = tilePos.y / chunkSize;
        int xInChunk = tilePos.x % chunkSize;
        int yInChunk = tilePos.y % chunkSize;

        Chunk targetChunk = getChunkFromWorld(tilePos);

        //place in chunk
        if (targetChunk == null) return null;
        return targetChunk.getTile(xInChunk, yInChunk, isWall);
    }



    // RENDERING
    static public Vector2 getChunkPosFromWorld(Vector3 input)
    {
        int chunkX = (int)input.x / chunkSize;
        int chunkY = (int)input.y / chunkSize;
        return new Vector2(chunkX, chunkY);
    }

    static public Chunk getChunkFromWorld(Vector3 input)
    {
        int chunkX = (int) input.x / chunkSize;
        int chunkY = (int) input.y / chunkSize;
        int xInChunk = (int) input.x % chunkSize;
        int yInChunk = (int) input.y % chunkSize;

        //if out of bounds return null
        if (chunkGrid.Count <= chunkY || chunkGrid[chunkY].Count <= chunkX)
        {
            return null;
        }


        return chunkGrid[chunkY][chunkX];
    }

    //call these whenever the player moves into a new chunk
    static public void renderPlayerChunks(Vector3 playerPos)
    {
        if(playerPos.x < 0 || playerPos.y < 0)return; //how to handle negative pos?
        //Vector3 playerPos = GameObject.Find("Player").transform.position;
        //get chunk player is on
        Chunk playerChunk = getChunkFromWorld(playerPos);
        //if player is not on a valid chunk then dont try to render it
        if (playerChunk == null) { return; }
        Vector2 p = playerChunk.getPos();
        int cx = (int)p.x;
        int cy = (int)p.y;
        //get chunks around player
        Vector2Int cornerPos = new Vector2Int(cx - renderDistance + 1, cy - renderDistance + 1);
        for (int i = 0; i < renderDistance * 2 - 1; i++)
        {
            for(int j = 0; j < renderDistance * 2 - 1; j++)
            {
                //player is located at center of grid
                try
                {
                    loadedChunks.Add(chunkGrid[cornerPos.y + i][cornerPos.x + j]);
                }
                catch
                {
                    Debug.Log("tried rendering a chunk out of bounds! what now?");
                }
            }
        }
        //unload old chunks and render new chunks
        List<Chunk> toRemove = new List<Chunk>();
        foreach(Chunk chunk in loadedChunks){
            //if chunk is too far from player then unload it
            if(chunk.distanceFromChunk(playerChunk) < renderDistance)
            {
                //within distance
                chunk.render(groundTilemap, wallTilemap);
            } else
            {
                //outside distance
                toRemove.Add(chunk);
                //loadedChunks.Remove(chunk);
                chunk.unload(groundTilemap, wallTilemap);
            }

        }
        for (int i = 0; i < toRemove.Count; i++)
        {
            loadedChunks.Remove(toRemove[i]);
        }

        

    }


    
}
