using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class PathGrid 
{
    //Container for the data grid used by pathfinding scripts
    //Mobs will have their own local grid used to path to the player

    public List<List<PathCell>> grid = new List<List<PathCell>>();

    public PathGrid(int width, int height)
    {
        //init path grid

        //init inner lists
        for(int i = 0; i < height;i++)
        {
            grid[i] = new List<PathCell>();
        }
    }

}
