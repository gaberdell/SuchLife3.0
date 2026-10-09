using Codice.Client.BaseCommands.WkStatus.Printers;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

public class PathingController
{
    //every entity with pathfinding will have a PathingController attached to them to facilitate this.


    Tilemap groundMap = ChunkManager.GetGroundTilemap();
    Tilemap wallMap = ChunkManager.GetWallTilemap();

    Transform controlled;
    Transform target;
    PathCell startCell;
    PathCell endCell;

    List<PathCell> currentPath;

    //bounds of where we search the world for tiles
    public BoundsInt pathBounds;

    const int gridSize = 30; //const value for now, replace with input values later when necessary
    PathGrid pgrid;

    private int gridBoundX = 0, gridBoundY = 0;
    private int startX, startY;

    bool isActive = false;

    

    public PathingController(int sX, int sY, Transform t, Transform c)
    {
        //only search for tiles within range of the enemy location.
        startX = sX;
        startY = sY;
        target = t;
        controlled = c;
        pathBounds = new BoundsInt(startX-gridSize/2, startY-gridSize/2, 0, gridSize, gridSize, 1); //create a bounds of length/width gridSize centered at startX, startY
        pgrid = new PathGrid(gridSize, gridSize, pathBounds.min.x, pathBounds.min.y);
        
    }

    public void toggleActive()
    {
        isActive = !isActive;
    }

    public void moveAlongPath(float speed)
    {
        if (isActive && currentPath.Count > 0)
        {
            //step towards the next world pos in the path
            Vector3 nextPos = pgrid.getWorldPos(currentPath[0]);
            float step = speed * Time.deltaTime;
            controlled.position = Vector3.MoveTowards(controlled.position, nextPos, step);
            //face towards next position
            Quaternion finalRotation = Quaternion.LookRotation(controlled.position - nextPos);
            float nextAngle = Mathf.LerpAngle(controlled.rotation.z, finalRotation.z, Time.deltaTime);
            controlled.eulerAngles = new Vector3(0, 0, nextAngle);
 
            if(Vector3.Distance(controlled.position, nextPos) < 0.01f)
            {
                Debug.Log("MOVING TO NEXT PATH STEP AT " + nextPos.ToString() + "");
                currentPath.RemoveAt(0);
            }
        }
    }

    public void createPathGrid()
    {
        //iterate through all corresponding tiles in the world within the range and add them to the grid

        //use getTilesBlock on the live tilemap instead for performance (definitely not cause i dont want to write an equivalent for the chunkman...)

        TileBase[] groundTiles = groundMap.GetTilesBlock(pathBounds);
        TileBase[] wallTiles = wallMap.GetTilesBlock(pathBounds);

        //check positions (debugging)
        //foreach (var point in pathBounds.allPositionsWithin) 
        //{
        //    Debug.Log(point.ToString());
        //}


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

        //assign startcell and endcell
        startCell = pgrid.getCellFromWorld(startX, startY);
        //Debug.Log("target " + target.position.ToString());
        //Debug.Log("Start: (" + startX + ", " + startY + ")");
        startCell.printCell();
        //Debug.Log("End: (" + (int)target.position.x + ", " + (int)target.position.y + ")");
        endCell = pgrid.getCellFromWorld((int)target.position.x, (int)target.position.y);
        //Debug.Log("End: (" + (int)target.position.x + ", " + (int)target.position.y + ")");
        endCell.printCell();
        //now assign neighbors
        assignNeighbors();

    }

    //called after grid is created; give every pathcell its neighbor
    public void assignNeighbors()
    {
        pgrid.assignNeighbors();
    }

    public void printPath()
    {
        //Debug.Log("printing path");
        pgrid.printGrid();
    }

    //retraces path from targetcell to startcell
    public List<PathCell> retracePath()
    {
        List<PathCell> path = new List<PathCell>();
        PathCell current = endCell;
        int c = 0;
        while ((current != startCell) && (c < 100))
        {
            current.printCell();
            path.Add(current);
            current = current.parent;
            c++;
        }

        path.Reverse();
        return path;
    }


    //main A* pathfinding script

    public void findPath()
    {
        //using start/end cells already established
        List<PathCell> openSet = new List<PathCell>();
        List<PathCell> closeSet = new List<PathCell>();
        openSet.Add(startCell);
        startCell.gCost = 0;

        //main loop
        while (openSet.Count > 0)
        {
            //look for an eligible cell from the open set
            PathCell currentCell = openSet[0];
            //currentCell.printCell();
            for (int i = 0; i < openSet.Count; i++)
            {
                if (openSet[i].fCost() < currentCell.fCost() || openSet[i].fCost() == currentCell.fCost() && openSet[i].hCost < currentCell.hCost)
                {
                    currentCell = openSet[i];
                }
            }
            //
            openSet.Remove(currentCell);
            closeSet.Add(currentCell);
            if (currentCell == endCell)
            {
                currentPath = retracePath();
                return;
            }

            //look through neighbors for a better cell to go to next
            foreach (PathCell neighbor in currentCell.neighbors)
            {
                if (!neighbor.walkable || closeSet.Contains(neighbor)) continue;

                int newMovementCostToNeighbor = currentCell.gCost + pgrid.getDistance(currentCell, neighbor);
                if (newMovementCostToNeighbor < neighbor.gCost || !openSet.Contains(neighbor))
                {
                    neighbor.gCost = newMovementCostToNeighbor;
                    neighbor.hCost = pgrid.getDistance(neighbor, endCell);
                    neighbor.parent = currentCell;

                    if (!openSet.Contains(neighbor)) openSet.Add(neighbor);
                }
            }
        }
    }

    
}
