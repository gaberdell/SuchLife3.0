using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class PathCell 
{
    //Utility Cell data structure for pathfinding algo
    //public cause i cant be bothered otherwise

    public int gCost = -1; //-1 corresponds to uninitialized
    public int hCost = -1;
    public int fCost = -1;
    public int tileX, tileY; //corresponds to position on tilemap;
    public int pathX, pathY;
    bool walkable = false;
    public List<PathCell> neighbors; //for finding the path
    public PathCell parent; //for path purposes


    public PathCell(int tx, int ty, bool w)
    {
        tileX = tx; 
        tileY = ty;
        walkable = w;
    }

    public int getfCost()
    {
        return gCost + hCost;
    }
}
