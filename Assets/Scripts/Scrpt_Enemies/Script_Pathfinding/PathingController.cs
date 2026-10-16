using Codice.Client.BaseCommands.WkStatus.Printers;
using NUnit.Framework;
using System.Collections.Generic;
using System.Drawing;
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
    int controlledWallExtension = 0; //how many times to 'extend' walls when simulating the path so that the controlled object is big enough to fit; 1x1 is 0, 3x3 (bomb) is 1 
    PathGrid pgrid;

    private int gridBoundX = 0, gridBoundY = 0;
    private int startX, startY;

    private bool isActive = false;
    private Animator animator; //used to toggle walk cycle as necessary


    public PathingController(int sX, int sY, Transform c, Animator a, int s)
    {
        //only search for tiles within range of the enemy location.
        startX = sX;
        startY = sY;
        controlled = c;
        pathBounds = new BoundsInt(startX - gridSize / 2, startY - gridSize / 2, 0, gridSize, gridSize, 1); //create a bounds of length/width gridSize centered at startX, startY
        pgrid = new PathGrid(gridSize, gridSize, pathBounds.min.x, pathBounds.min.y);
        animator = a;
        controlledWallExtension = s;
        //instantiate the pathgrid on creation
        createPathGrid();
    }

    public void toggleActive()
    {
        isActive = !isActive;
        if (isActive)
        {
            //create a path when being set active
            findPath();
        }
    }

    //method is called every frame by the mob's update method
    public void moveAlongPath(float speed)
    {
        if (isActive && currentPath.Count > 0)
        {
            //step towards the next world pos in the path
            Vector3 nextPos = pgrid.getWorldPos(currentPath[0]);

            float step = speed * Time.deltaTime;
            controlled.position = Vector3.MoveTowards(controlled.position, nextPos, step);

            //face towards next position
            Vector3 moveDir = (controlled.position - nextPos);
            float nextAngle = Mathf.Atan2(moveDir.y, moveDir.x) * Mathf.Rad2Deg;
            controlled.rotation = Quaternion.Slerp(controlled.rotation, Quaternion.Euler(0, 0, nextAngle + 90), Time.deltaTime);

            //make sure walking animation is playing
            if (animator) animator.SetBool("Walk", true);

            if (Vector3.Distance(controlled.position, nextPos) < 0.01f)
            {
                Debug.Log("MOVING TO NEXT PATH STEP AT " + nextPos.ToString() + "");
                currentPath.RemoveAt(0);

                //if finished with the path then stop walking
                if (currentPath.Count <= 0)
                {
                    if (animator) animator.SetBool("Walk", false);
                }
            }
        }

        /*check if path needs to be recalculated;
         * path needs to be recalculated if:
         * - target position changes to a different tile (listen for target change)
         * - current path becomes no longer walkable (check when player places a block)
        */
       
    }

    private void createPathGrid()
    {
        //iterate through all corresponding tiles in the world within the range and add them to the grid
        //use getTilesBlock on the live tilemap instead for performance (definitely not cause i dont want to write an equivalent for the chunkman...)

        TileBase[] groundTiles = groundMap.GetTilesBlock(pathBounds);
        TileBase[] wallTiles = wallMap.GetTilesBlock(pathBounds);

        int pathx = 0, pathy = 0, i = 0;
        foreach (TileBase t in groundTiles)
        {
            PathCell newCell;
            //assuming the lists contain tiles starting from the minX, minY position, compressed into a 1D list 
            if (t == null)
            {
                //if ground tile is null then chunk is unloaded; treat this tile as a wall because we do not want enemies pathing through unloaded chunks
                newCell = new PathCell(pathx + pathBounds.min.x, pathy + pathBounds.min.y, pathx, pathy, false);
            } else
            {
                //ground tile is not null; check for a wall now
                if (wallTiles[i] == null)
                {
                    //no wall; tile is walkable
                    newCell = new PathCell(pathx + pathBounds.min.x, pathy + pathBounds.min.y, pathx, pathy, false);
                } else
                {
                    //wall present; tile is not walkable
                    newCell = new PathCell(pathx + pathBounds.min.x, pathy + pathBounds.min.y, pathx, pathy, true);
                    //newCell.printCell();
                }
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
        //now assign neighbors
        assignNeighbors();

        //perform simulated wall extensions
        for(int j = 0; j < controlledWallExtension; j++)
        {
            //for every tile, check if a neighbor is a wall; if so then mark it to be set as a wall
            List<PathCell> newWallCells = pgrid.getCellsWithWallNeighbors();
            //set marked tiles as walls; 2 passes to avoid everything chaining into a wall
            foreach(PathCell newCell in newWallCells)
            {
                newCell.walkable = false;
            }
        }
    }

    //used to set a target after initialization
    public void setTarget(Transform t)
    {
        target = t;
        //assign startcell and endcell
    }

    public void setSize(int s)
    {
        controlledWallExtension = s;
        createPathGrid();
    }

    //reset start and end cell locations; used to update path if target moves
    private void resetPath()
    {
        startCell = pgrid.getCellFromWorld(startX, startY);
        endCell = pgrid.getCellFromWorld((int)target.position.x, (int)target.position.y);
    }

    //called after grid is created; give every pathcell its neighbor
    private void assignNeighbors()
    {
        pgrid.assignNeighbors();
    }

    public void printPath()
    {
        //Debug.Log("printing path");
        pgrid.printGrid();
    }

    //retraces path from targetcell to startcell
    private List<PathCell> retracePath()
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

    private void findPath()
    {
        //reset path and start/end cells first
        resetPath();
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

    public void setAnimator(Animator a)
    {
        animator = a;
    }

    
}
