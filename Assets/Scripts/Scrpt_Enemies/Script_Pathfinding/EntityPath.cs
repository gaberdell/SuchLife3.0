using NUnit.Framework;
using System.Collections.Generic;
using Unity.Hierarchy.Editor;
using UnityEngine;
using UnityEngine.Tilemaps;

public class EntityPath : MonoBehaviour
{
    // Path Object to be given as part of a Mob class.
    // Will constantly move the gameobject towards a target (cell) 

    Transform target;
    Tilemap placeableTilemap; //placeable one so we know where walls are
    List<PathCell> openList = new List<PathCell>();
    List<PathCell> closedList = new List<PathCell>();


    private void pathfindingLoop()
    {
        //do a star stuff
    }

    public void setTarget(Transform target)
    {

    }
}
