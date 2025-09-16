using NUnit.Framework;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Chunk
{
    private int worldX; //location in the unity scene
    private int worldY;
    private int chunkX; //location relative to other chunks (in the grid of chunks)
    private int chunkY;
    private int offset; //offset of chunk pos vs world pos
    public int size; //size of chunk
    bool rendered = false; //bools for determining whether the chunk has been rendered in the scene or not
    bool hasBeenRenderedOnce = false; //bool used to determine whether to fill the chunk with walls or not, only needs to be done on initial render
    TileBase wallTile; //tile used to fill walls; leave empty if not needed (not dungeon chunk)
    TileBase[] floorTile; //tiles used to fill floor beneath filled walls; leave empty if not dungeon
    List<List<TileBase>> floorTiles = new List<List<TileBase>>(); //grid of placed tiles for floor tilemap
    List<List<TileBase>> wallTiles = new List<List<TileBase>>(); //grid of placed tiles for wall tilemap
    List<GameObject> chunkMobs = new List<GameObject>();//container for enemies present in this chunk

    /*
    * requires: csize not null
    * modifies: size, floorTiles, wallTiles
    * effects: instantiates the chunk's size and positions and fills its floor and wall tile lists with null tiles
    * returns: none
    * throws: none
    */
    public Chunk(int csize, int coffset, int cx, int cy)
    {
        size = csize;
        offset = coffset;
        chunkX = cx;
        chunkY = cy;
        worldX = cx * csize;
        worldY = cy * csize;

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


    // INFO METHODS
    /*
    * requires: x > 0, y > 0
    * modifies: wallTiles, floorTiles
    * effects: updates the chunk's tilegrid at input position with input tile
    * returns: none
    * throws: IndexOutOfBoundsException when  y > size or x > size or x or y is negative 
    */
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

    /*
    * requires: x > 0, y > 0
    * modifies: none
    * effects: returns the tile in the chunk's tilegrid at the input position
    * returns: returns a TileBase object at the input position from either the floor or wall tileMap, depending on arguments.
    * throws: IndexOutOfBoundsException when  y > size or x > size or x or y is negative 
    */
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

    /*
    * requires: cx, cy, wx, wy not null
    * modifies: chunkX, chunkY, worldX, worldY
    * effects: instantiates this chunk's position properties to the argument values. ... can probably be moved into the initialization method
    * returns: none
    * throws: none
    */
    public void fillInfo(int cx, int cy, int wx, int wy)
    {
        chunkX = cx;
        chunkY = cy;
        worldX = wx;
        worldY = wy;
    }

    // HELPER METHODS

    /*
    * requires: none
    * modifies: none
    * effects:  returns this chunk's position in the chunkGrid
    * returns: returns a Vector2 representing this chunk's position in the chunkGrid
    * throws: none
    */
    public Vector2 getPos()
    {
        return new Vector2(chunkX, chunkY);
    }

    /*
    * requires: otherChunk not null
    * modifies: none
    * effects: returns the distance between this chunk and the input chunk, in terms of coordinates on the chunkGrid
    * returns: int distance where distance is obtained by subtracting the 2 chunks' positions from each other and getting the absolute value.
    * throws: none
    */
    public int distanceFromChunk(Chunk otherChunk)
    {
        Vector2 c2pos = otherChunk.getPos();
        //diagonals are counted as 1 distance; take the max between x and y distance
        int distance = (int)Mathf.Max(Mathf.Abs(chunkX - c2pos.x), Mathf.Abs(chunkY - c2pos.y));
        return distance;
    }

    /*
    * requires: mob not null
    * modifies: chunkMobs
    * effects:  adds/removes the input mob gameobject from the chunk's chunkMobs list. Used for moving mobs between chunks.
    * returns: none
    * throws: none
    */
    public void addEntity(GameObject mob)
    {
        chunkMobs.Add(mob);
    }

    public void removeEntity(GameObject mob)
    {
        chunkMobs.Remove(mob);
    }



    // RENDERING METHODS

    /*
    * requires: groundTilemap, wallTilemap
    * modifies: hasBeenRenderedOnce, groundTilemap, wallTilemap, chunkMobs
    * effects: sets all of the tiles on the chunk onto the input tilemaps (which are present in the scene). If this is the first time, then fill the chunk with tiles matching its wallTile/floorTile respectively.
    * Also set entities present in the chunk to active.
    * returns: none
    * throws: none
    */
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
        //fill chunk tiles from data
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                TileBase ftile = floorTiles[i][j];
                TileBase wtile = wallTiles[i][j];
                groundTilemap.SetTile(new Vector3Int(worldX + j - offset, worldY + i - offset, 0), ftile);
                wallTilemap.SetTile(new Vector3Int(worldX + j - offset, worldY + i - offset, 0), wtile);
            }
        }
        //activate chunk entities 
        for (int i = 0; i < chunkMobs.Count; i++)
        {
            GameObject currMob = chunkMobs[i];
            
            if (currMob != null)
            {
                currMob.SetActive(true);
            } else
            {
                //enemy is null; remove it 
                chunkMobs.Remove(currMob);
                i--;
            }
            
            
        }
    }

    /*
    * requires: none
    * modifies: none
    * effects:  sets this chunk's wallTile attribute to the input tile. Only sets wall fill tiles now, will likely have a floor fill later too.
    * returns: none
    * throws: none
    */
    public void setFillInfo(TileBase wallT, TileBase[] groundTiles)
    {
        wallTile = wallT;
        floorTile = groundTiles;
    }

    /*
    * requires: wallTilemap not null
    * modifies: wallTiles
    * effects:  sets all tiles on the wallTiles grid above null floors to be the fill Tile. Does this when the chunk is rendered for the first time so it doesn't all ahve to be done at runtime.
    * returns: none 
    * throws: none
    */
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
                //set floor tile after
                floorTiles[i][j] = floorTile[0];
            }
        }
    }

    /*
    * requires: groundTilemap, wallTilemap not null
    * modifies: chunkMobs, groundTilemap, wallTilemap
    * effects: unloads the current chunk by setting all of its tiles in the scene to be null and cleans out mobs by removing null enemies and setting the rest to be inactive
    * returns: none
    * throws: none
    */
    public void unload(Tilemap groundTilemap, Tilemap wallTilemap)
    {
        rendered = false;
        //deactivate chunk entities
        for (int i = 0; i < chunkMobs.Count; i++)
        {
            GameObject currMob = chunkMobs[i];
            if (currMob != null)
            {
                currMob.SetActive(false);
            }
            else
            {
                //enemy is null; remove it 
                chunkMobs.Remove(currMob);
                i--;
            }
        }
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                groundTilemap.SetTile(new Vector3Int(worldX + j - offset, worldY + i - offset, 0), null);
                wallTilemap.SetTile(new Vector3Int(worldX + j - offset, worldY + i - offset, 0), null);
            }
        }
    }



}
