using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PathGrid 
{
    //Container for the data grid used by pathfinding scripts
    //Mobs will have their own local grid used to path to the player

    //IMPORTANT - ON THIS GRID, THE 0,0 POINT IS IN THE BOTTOM LEFT, JUST LIKE UNITY TILEMAPS.

    public List<List<PathCell>> grid = new List<List<PathCell>>();
    private int width = -1;
    private int height = -1;

    public PathGrid(int w, int h)
    {
        //init path grid
        width = w;
        height = h;

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


    //get a cell's neighbors (within this pathgrid) using pathgrid coordinates
    public List<PathCell> getNeighbors(int x, int y)
    {
        List<PathCell> myNeighbors = new List<PathCell>();

        //handle all possible cases; 8 tiles surrounding target, and (literal) edge cases where some tiles would not exist
        //should never be cases of null values; null tiles are added into the grid as walls.

        //start with top middle tile, going clockwise
        if (y != (height - 1)) myNeighbors.Add(grid[y + 1][x]); //top middle
        if ((y != (height - 1)) && x != (width - 1)) myNeighbors.Add(grid[y + 1][x + 1]);
        if (x != (width - 1)) myNeighbors.Add(grid[y][x + 1]); //middle right
        if ((y != 0) && x != (width - 1)) myNeighbors.Add(grid[y - 1][x + 1]);
        if (y != 0) myNeighbors.Add(grid[y - 1][x]); //bottom middle
        if ((y != 0) && (x != 0)) myNeighbors.Add(grid[y - 1][x - 1]);
        if (x != 0) myNeighbors.Add(grid[y][x - 1]); //middle left
        if ((y != (height - 1)) && (x != 0)) myNeighbors.Add(grid[y - 1][x - 1]);

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
            }
        }
    }

}
