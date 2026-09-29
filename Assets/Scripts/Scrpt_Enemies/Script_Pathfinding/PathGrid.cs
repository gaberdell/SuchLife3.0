using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class PathGrid 
{
    //Container for the data grid used by pathfinding scripts
    //Mobs will have their own local grid used to path to the player

    //IMPORTANT - ON THIS GRID, THE 0,0 POINT IS IN THE BOTTOM LEFT, JUST LIKE UNITY TILEMAPS.

    public List<List<PathCell>> grid = new List<List<PathCell>>();
    private int width = -1;
    private int height = -1;

    private int worldX, worldY; //position of 0,0 cell of grid in the world.

    public PathGrid(int w, int h, int wx, int wy)
    {
        //init path grid
        width = w;
        height = h;
        worldX = wx;
        worldY = wy;

        //init inner lists
        for(int i = 0; i < height;i++)
        {
            grid.Add(new List<PathCell>());
        }
    }

    //add a cell to the grid, used when initializing
    public void addCell(int y, PathCell cell)
    {
        grid[y].Add(cell);
    }

    //give every cell its neighbors, called on instantiation
    public void assignNeighbors()
    {
        for (int i = 0; i < grid.Count; i++)
        {
            for (int j = 0; j < grid[i].Count; j++)
            {
                PathCell cell = grid[i][j];
                //cell.printCell();
                cell.neighbors = getNeighbors(cell.pathX, cell.pathY);
          
                //cell.printNeighbors();
            }
        }
        
    }


    //get a cell's neighbors (within this pathgrid) using pathgrid coordinates
    public List<PathCell> getNeighbors(int x, int y)
    {
        List<PathCell> myNeighbors = new List<PathCell>();

        //handle all possible cases; 8 tiles surrounding target, and (literal) edge cases where some tiles would not exist
        //should never be cases of null values; null tiles are added into the grid as walls.

        //start with top middle tile, going clockwise
        if (y != (height - 1)) myNeighbors.Add(grid[y + 1][x]); //top middle
        if ((y != (height - 1)) && x != (width - 1)) myNeighbors.Add(grid[y + 1][x + 1]); //top right
        if (x != (width - 1)) myNeighbors.Add(grid[y][x + 1]); //middle right
        if ((y != 0) && x != (width - 1)) myNeighbors.Add(grid[y - 1][x + 1]); // bottom right
        if (y != 0) myNeighbors.Add(grid[y - 1][x]); //bottom middle
        if ((y != 0) && (x != 0)) myNeighbors.Add(grid[y - 1][x - 1]); // bottom left
        if (x != 0) myNeighbors.Add(grid[y][x - 1]); //middle left
        if ((y != (height - 1)) && (x != 0)) myNeighbors.Add(grid[y + 1][x - 1]); //top left

        return myNeighbors;
    }


    //print cells within grid; for debugging
    public void printGrid()
    {
        for (int i = 0; i < grid.Count; i++)
        {
            for(int j = 0; j < grid[i].Count; j++)
            {
                grid[i][j].printCell();
                //grid[i][j].printNeighbors();
            }
        }
    }

    //get A* distance between cells by finding the x and y distances; 10 is for single direction cell distances while 14 is for diagonal distances
    public int getDistance(PathCell c1, PathCell c2)
    {
        int dx = Mathf.Abs(c1.pathX - c2.pathX);
        int dy = Mathf.Abs(c1.pathY - c2.pathY);
        if (dx > dy)
        {
            return 14 * dy + 10 * (dx - dy);
        }
        return 14 * dx + 10 * (dy - dx);
    }

    //get world position of pathcell
    public Vector3 getWorldPos(PathCell c)
    {
        return new Vector3(c.tileX, c.tileY, 0);
    }

}
