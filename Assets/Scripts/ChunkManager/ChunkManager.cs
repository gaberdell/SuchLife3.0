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

    const int chunkSize = 10; //width & height of chunk in tiles;
    const int renderDistance = 2; //amount of chunks away from player's chunk to render (e.g. if 1, then render 8 chunks around player's chunk)
    static Vector3 worldPos = new Vector3(0, 0, 0); //position of chunk manager in world (should match the tilemaps'
    static List<List<Chunk>> chunkGrid = new List<List<Chunk>>(); //container for world chunks
    static List<Chunk> loadedChunks; //keep track of loaded chunks to unload them when player renders new chunks.

    static Tilemap wallTilemap = GameObject.Find("PlaceableTileMap").GetComponent<Tilemap>();
    static Tilemap groundTilemap = GameObject.Find("GroundTilemap").GetComponent<Tilemap>();

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

        Chunk targetChunk = chunkGrid[chunkY][chunkX];

        

        //place in chunk

        targetChunk.insertTile(xInChunk, yInChunk, tile, isWall);
    }

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



    // when the player moves, the manager will check if a chunk needs to be loaded (rendered) or unloaded (unrendered from scene)


    //debug function to render every chunk
    static public void renderAll()
    {
        for (int i = 0; i < chunkGrid.Count; i++)
        {
            for(int j = 0;j< chunkGrid[i].Count; j++)
            {
                chunkGrid[i][j].render(groundTilemap, wallTilemap);
            }
        }
    }

    //mainly for player
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
        if(chunkGrid.Count <= chunkY || chunkGrid[chunkY].Count <= chunkX)
        {
            return null;
        }

        return chunkGrid[chunkY][chunkX];
    }

    //call these whenever the player moves into a new chunk
    static public void renderPlayerChunks(Vector3 playerPos)
    {
        
        //Vector3 playerPos = GameObject.Find("Player").transform.position;
        //get chunk player is on
        Chunk playerChunk = getChunkFromWorld(playerPos);
        //render chunks around player
        if (renderDistance == 1)
        {
            playerChunk.render(groundTilemap, wallTilemap);
        }
        else if (renderDistance == 2)
        {
            Vector2 p = playerChunk.getPos();
            int cx = (int)p.x;
            int cy = (int)p.y;
            
            List<Chunk> chunksToRender = new List<Chunk>();
            chunksToRender.Add(playerChunk);
            chunksToRender.Add(chunkGrid[cy][cx+1]);
            chunksToRender.Add(chunkGrid[cy][cx-1]);
            chunksToRender.Add(chunkGrid[cy+1][cx]);
            chunksToRender.Add(chunkGrid[cy-1][cx]);
            chunksToRender.Add(chunkGrid[cy+1][cx+1]);
            chunksToRender.Add(chunkGrid[cy-1][cx-1]);
            chunksToRender.Add(chunkGrid[cy+1][cx-1]);
            chunksToRender.Add(chunkGrid[cy-1][cx+1]);

            //unload chunks
            //for (int i = 0; i < loadedChunks.Count; i++)
            //{
            //    Chunk currChunk = loadedChunks[i];
            //    //if already rendered chunk not in list of chunks to load then unload it
            //    if (chunksToRender.Contains(currChunk))
            //    {
                    
            //    } else
            //    {
            //        currChunk.unload(groundTilemap, wallTilemap);
            //        loadedChunks.Remove(currChunk);
            //    }
            //}

            for(int i = 0;i < chunksToRender.Count; i++)
            {
                Chunk currChunk = chunksToRender[i];
                currChunk.render(groundTilemap, wallTilemap);
                //if (!loadedChunks.Contains(currChunk)){ loadedChunks.Add(currChunk); }

            }

        }

    }


    
}
