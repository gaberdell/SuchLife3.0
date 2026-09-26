using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PathingController 
{
    //every entity with pathfinding will have a PathingController attached to them to facilitate this.


    Tilemap groundMap = ChunkManager.GetGroundTilemap();
    Tilemap wallMap = ChunkManager.GetWallTilemap();

    const int gridSize = 30; //const value for now, replace with input values later when necessary
    PathGrid pgrid = new PathGrid(gridSize, gridSize);

    Transform target;

    //bounds of where we search the world for tiles
    public int pathStartX, pathStartY;
    public BoundsInt pathBounds;

    private int gridBoundX = 0, gridBoundY = 0;

    bool isActive = false;

    public PathingController(int startX, int startY, Transform t)
    {
        //only search for tiles within range of the enemy location.
        pathStartX = startX;
        pathStartY = startY;
        target = t;
        pathBounds = new BoundsInt(startX-gridSize/2, startY-gridSize/2, 0, gridSize, gridSize, 1); //create a bounds of length/width gridSize centered at startX, startY
    }

    private void FixedUpdate()
    {
        //update path every x length of time
    }

    public void createPathGrid()
    {
        //iterate through all corresponding tiles in the world within the range and add them to the grid

        //use getTilesBlock on the live tilemap instead for performance (definitely not cause i dont want to write an equivalent for the chunkman...)

        TileBase[] groundTiles = groundMap.GetTilesBlock(pathBounds);
        TileBase[] wallTiles = wallMap.GetTilesBlock(pathBounds);

        //check positions (debugging)
        foreach (var point in pathBounds.allPositionsWithin) 
        {
            Debug.Log(point.ToString());
        }


        int pathx = 0, pathy = 0, i = 0;
        foreach (TileBase t in groundTiles)
        {
            PathCell newCell;
            //assuming the lists contain tiles starting from the minX, minY position, compressed into a 1D list 
            if (t == null)
            {
                //if tile is null then there is no wall tile OR chunk is unloaded
                if(groundTiles[i] == null)
                {
                    //chunk is unloaded; treat this tile as a wall because we do not want enemies pathing through unloaded chunks
                    newCell = new PathCell(pathx + pathBounds.min.x, pathy + pathBounds.min.y, pathx, pathy, true);
                }
                newCell = new PathCell(pathx + pathBounds.min.x, pathy + pathBounds.min.y, pathx, pathy, false);
            } else
            {
                newCell = new PathCell(pathx + pathBounds.min.x, pathy + pathBounds.min.y, pathx, pathy, true);
            }

            //Debug.Log("pathx: " + pathx + " | pathy: " + pathy + " | i: " + i );
            pgrid.addCell(pathy, newCell);

            i++;
            if (pathx !=0 && pathx % (gridSize-1) == 0) //cause starts at 0
            {
                pathy++;
                pathx = 0;
            } else
            {
                pathx++;
            }
            
            
        }


    }

    public void printPath()
    {
        Debug.Log("printing path");
        pgrid.printGrid();
    }

    
}
