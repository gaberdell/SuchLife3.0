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
    const int offset = 500; //add this number to world positions read into and from this manager; used to offset the 0,0 point of the manager so negative cases don't need to be considered.
    static Vector3Int worldPos = new Vector3Int(0, 0, 0); //position of chunk manager in world (should match the tilemaps'
    static List<List<Chunk>> chunkGrid = new List<List<Chunk>>(); //container for world chunks
    static HashSet<Chunk> loadedChunks = new HashSet<Chunk>(); //keep track of loaded chunks to unload them when player renders new chunks.

    static Tilemap wallTilemap = GameObject.Find("PlaceableTileMap").GetComponent<Tilemap>();
    static Tilemap groundTilemap = GameObject.Find("GroundTilemap").GetComponent<Tilemap>();

    // INSTANTIATION METHODS



     /*
     * requires: tilePos, isWall not null; tilePos >= 0,0,0 (no negative nums)
     * modifies: chunkGrid, wallTilemap, groundTilemap
     * effects: updates the chunk manager by setting the tile at the given position to match the given TileBase. may create new chunks if necessary. essentially mimics the behavior of tilemap.setTile.
     * will update currently rendered chunks with changes as well.
     * returns: none
     * throws: none
     */
    static public void SetTile(Vector3Int tilePos, TileBase tile, bool isWall)
    {
        //figure out what chunk to place the tile into
        //0th 0th chunk starts at bottom left corner? top left?
        //input is the position on the tilemap

        Vector2 cPos = getChunkPosFromWorld(tilePos);
        int chunkX = (int)cPos.x;
        int chunkY = (int)cPos.y;
        int xInChunk = (tilePos.x + offset) % chunkSize;
        int yInChunk = (tilePos.y + offset) % chunkSize;

        //if out of bounds then add empty chunks
        while(chunkY+1 > chunkGrid.Count)
        {
            chunkGrid.Add(new List<Chunk>());
        }
        if (chunkY < 0) return;
        while(chunkX+1 > chunkGrid[chunkY].Count)
        {
            chunkGrid[chunkY].Add(new Chunk(chunkSize, offset, chunkGrid[chunkY].Count, chunkY));
        }
        //fillChunkInfo();

        //Chunk targetChunk = chunkGrid[chunkY][chunkX]; //index out of range?
        Chunk targetChunk = getChunkFromWorld(tilePos);

        //place in chunk
        if (targetChunk != null)
        {
            targetChunk.insertTile(xInChunk, yInChunk, tile, isWall);
            //if this chunk is already rendered then make the change directly on the tilemap as well.
            if (loadedChunks.Contains(targetChunk))
            {
                if (isWall)
                {
                    wallTilemap.SetTile(tilePos, tile);
                }
                else
                {
                    groundTilemap.SetTile(tilePos, tile);
                }
            }
        }
    }

    /*
     * requires: chunkX, chunkY not null and > 0
     * modifies: chunkGrid
     * effects: adds a null chunk to the grid at the given index; only really used when attempting to render chunks that dont exist on the grid
     * returns: none
     * throws: none
     */
    static private void addChunk(int chunkX, int chunkY)
    {
        while (chunkY + 1 > chunkGrid.Count)
        {
            chunkGrid.Add(new List<Chunk>());
        }
        if (chunkY < 0) return;
        while (chunkX + 1 > chunkGrid[chunkY].Count)
        {
            chunkGrid[chunkY].Add(new Chunk(chunkSize, offset, chunkGrid[chunkY].Count, chunkY));
        }
        //Chunk newC = chunkGrid[chunkY][chunkX];
        //fill chunk with stuff
        
    }

    /*
    * requires: x, y, width, height not null
    * modifies: chunkGrid
    * effects: invokes the setFillInfo method on every chunk within the width and height. this 'fill' refers to all of the tiles in a chunk that are not previously set to another tile. For example, dungeon grids have a fill attribute set to the wall tile.
    * This is done to prevent unnecessary setTile calls when instantiating dungeons into chunks. May not be applicable for overworld generation.
    * returns: none
    * throws: none
    */
    static public void setChunksFill(int x, int y, int width, int height, TileBase fillTile, TileBase[] groundTiles)
    {
        for (int i = x; i < x + width; i+= chunkSize)
        {
            for(int j = y; j < y + height; j+= chunkSize)
            {
                Chunk currChunk = getChunkFromWorld(new Vector3(i, j, 0));
                if (currChunk != null)
                {
                    currChunk.setFillInfo(fillTile, groundTiles);
                } else
                {
                    //Vector2 pos = getChunkPosFromWorld(new Vector3(i, j, 0));
                    //if chunk is null then create a chunk at that index to be filled completely
                    SetTile(new Vector3Int(i, j, 0), fillTile, true);
                    Chunk newChunk = getChunkFromWorld(new Vector3(i, j, 0));
                    newChunk.setFillInfo(fillTile, groundTiles);
                }
                
            }
        }

    }

    /*
    * requires: entity, worldPos not null
    * modifies: chunkGrid
    * effects: places an entity gameObject onto a chunk. sets prefab objects to set active when the chunk is rendered, and instantiates them immediately. - suboptimal?
    * returns: none
    * throws: none
    */
    static public void addEntityToChunk(GameObject entity, Vector3Int worldPos)
    {
        //get chunk
        Chunk targetChunk = getChunkFromWorld(worldPos);
        //place entity in chunk
        entity = GameObject.Instantiate(entity, worldPos, Quaternion.identity);
        entity.SetActive(false);
        targetChunk.addEntity(entity);
    }

    /*
    * requires: prevChunk, currChunk, entity not null
    * modifies: chunkGrid
    * effects: removes a given entity from prevChunk, and adds it to currChunk using chunk class methods. 
    * returns: none
    * throws: none
    */
    static public void updateEntityPos(Vector2 prevChunk, Vector2 currChunk, GameObject entity)
    {
        Debug.Log("updating enemy position");
        chunkGrid[(int)prevChunk.y][(int)prevChunk.x].removeEntity(entity);
        chunkGrid[(int)currChunk.y][(int)currChunk.x].addEntity(entity);
    }



    // HELPERS

    /*
    * requires: tilePos, isWall not null.
    * modifies: none
    * effects: returns the tile set at the given position; isWall is necessary to know which tilemap to check.
    * returns: the tile at the chunk at the input tilePos position; null otherwise
    * throws: none
    */
    static public TileBase GetTile(Vector3Int tilePos, bool isWall)
    {
        Vector2 cPos = getChunkPosFromWorld(tilePos);
        int chunkX = (int)cPos.x;
        int chunkY = (int)cPos.y;
        int xInChunk = (tilePos.x + offset) % chunkSize;
        int yInChunk = (tilePos.y + offset) % chunkSize;

        Chunk targetChunk = getChunkFromWorld(tilePos);

        //place in chunk
        if (targetChunk == null) return null;
        return targetChunk.getTile(xInChunk, yInChunk, isWall);
    }



    

    /*
    * requires: input not null
    * modifies: none
    * effects: translates a Vector3 input into a chunk coordinate.
    * returns: Vector2 representing the chunk coordinate of the given Vector3 position
    * throws: none
    */
    static public Vector2 getChunkPosFromWorld(Vector3 input)
    {
        //increase position by offset to prevent negative cases
        input += new Vector3(offset, offset, 0);
        int chunkX = (int)input.x / chunkSize;
        int chunkY = (int)input.y / chunkSize;
        return new Vector2(chunkX, chunkY);
    }

    /*
    * requires: input not null
    * modifies: none
    * effects: returns the chunk at the input vector3 coordinate.
    * returns: Chunk c in chunkGrid at the given vector3 input position.
    * throws: none
    */
    static public Chunk getChunkFromWorld(Vector3 input)
    {
        Vector2 cPos = getChunkPosFromWorld(input);
        int chunkX = (int)cPos.x;
        int chunkY = (int)cPos.y;
        int xInChunk = (int) (input.x + offset) % chunkSize;
        int yInChunk = (int) (input.y + offset) % chunkSize;

        //if out of bounds return null
        if (chunkGrid.Count <= chunkY || chunkGrid[chunkY].Count <= chunkX)
        {
            //Debug.Log("getting out of bounds chunk from world!");
            return null;
        }


        return chunkGrid[chunkY][chunkX];
    }

    //RENDERING

    //called whenever the player moves into a new chunk
    /*
    * requires: playerPos not null; playerPos > (0,0,0) 
    * modifies: loadedChunks, wallTilemap, groundTilemap, chunkGrid
    * effects: gets chunks around player within renderDistance radius and attempts to render them (set chunk tile data onto the scene tilemap) by adding them to a list of loaded chunks.
    * then, remove the chunks that are now too far away from the player and unload them with their unload methods after the new chunks are rendered.
    * returns: none
    * throws: none
    */
    static public void renderPlayerChunks(Vector3 playerPos)
    {
        if(playerPos.x + offset < 0 || playerPos.y + offset < 0) { Debug.Log("PLAYER IS NEGATIVE EVEN WITH OFFSET"); return; } //how to handle negative pos?
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
                    //Debug.Log("tried rendering a chunk out of bounds! what now?");
                    //if a null chunk is rendered then create a new empty chunk at that coordinate... this shouldn't happen in the real game though 
                    addChunk(cornerPos.x+j, cornerPos.y+i);
                    loadedChunks.Add(chunkGrid[cornerPos.y + i][cornerPos.x + j]);
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
