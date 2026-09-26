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
    public int tileX = -1, tileY = -1; //corresponds to position on tilemap;
    public int pathX = -1, pathY = -1;
    bool walkable = false;
    public List<PathCell> neighbors; //for finding the path
    public PathCell parent; //for path purposes


    public PathCell(int tx, int ty, int px, int py, bool w)
    {
        tileX = tx; 
        tileY = ty;
        pathX = px;
        pathY = py;
        walkable = w;
    }

    public int getfCost()
    {
        return gCost + hCost;
    }

    public void printCell()
    {
        //print cell as a string
        Debug.Log("Cell: Tile (" + tileX + ", " + tileY + ") | Path (" + pathX + ", " + pathY + ") | Walkable: " + walkable + " | fgh(" + fCost + ", " + gCost + ", " + hCost + ")");
    }
}
