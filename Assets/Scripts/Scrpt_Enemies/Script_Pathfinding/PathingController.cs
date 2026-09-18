using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PathingController : MonoBehaviour
{
    //every entity with pathfinding will have a PathingController attached to them to facilitate this.


    const int gridSize = 30; //const value for now, replace with input values later when necessary
    PathGrid pgrid = new PathGrid(gridSize, gridSize);

    GameObject target;


    //bounds of where we search the world for tiles
    public int pathStartX, pathStartY;

    private int gridBoundX = 0, gridBoundY = 0;

    PathingController(int startX, int startY, GameObject t)
    {
        //only search for tiles within range of the enemy location.
        pathStartX = startX;
        pathStartY = startY;
        target = t;
    }

    private void FixedUpdate()
    {
        //update path every x length of time
    }

    public void createPathGrid()
    {
        //iterate through all corresponding tiles in the world within the range and add them to the grid
        //get tile info from chunkmanager; enemy pathing will respect unloaded tile information (kind of)
        for (int i = 0; i < gridSizeX; i++)
        {
            for(int j = 0; j < gridSizeY; j++)
            {
                //add tile to data grid

            }
        }
    }

    public List<PathCell> getNeighbors(int x, int y, int width, int height)
    {
        List<PathCell> myNeighbours = new List<PathCell>();
        //handle all possible cases; 8 tiles surrounding target, and (literal) edge cases where some tiles would not exist
        if (x > 0 && x < width - 1)
        {
            if (y > 0 && y < height - 1)
            {

            }
            else if (y == 0)
            {

            }
            else if (y == height - 1)
            {

            }
        }
        else if (x == 0)
        {
            if (y > 0 && y < height - 1)
            {

            }
            else if (y == 0)
            {

            }
            else if (y == height - 1)
            {

            }
        }
        else if (x == width - 1)
        {
            if (y > 0 && y < height - 1)
            {

            }
            else if (y == 0)
            {

            }
            else if (y == height - 1)
            {

            }
        }
        else if (x == 0)
        {
            if (y > 0 && y < height - 1)
            {

            }
            else if (y == 0)
            {

            }
            else if (y == height - 1)
            {

            }
        }
        else if (x == width - 1)
        {
            if (y > 0 && y < height - 1)
            {

            }
            else if (y == 0)
            {

            }
            else if (y == height - 1)
            {

            }
        }
        return null;
    }
}
