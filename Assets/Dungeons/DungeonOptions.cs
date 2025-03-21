using UnityEngine;

public class DungeonOptions 
{
    //class to hold options for dungeon creation
    //has graph (layout) options and generation options

    public int maxNodes; //max amount of 'rooms'
    public int minNodes; //min amount of 'rooms' 
    public int maxEdges; //max edges(connections) per node 
    public int maxDepth; //tree will go this deep before creating branches
    public string branchType; //how the tree will branch out; options are 0th;random
    //options for amount/frequency of room types
    //biases toward generation types? e.g. labyrinthine, blob, linear
    //maybe have a second generation phase that adds additional elements to the graph
    //boss/finish room at longest depth, treasure rooms, ect

    //options for dungeon rendering
    public int minRoomDist; //minimum amount of distance between two connected rooms
    public int maxRoomDist; //maximum amount of distance between two connected rooms
    public int hallwayOpening; //width/height of connections between rooms
    
    public DungeonOptions()
    {
        //initialize default values
        maxNodes = 10;
        minNodes = 10;
        maxEdges = 4;
        maxDepth = 3;
        minRoomDist = 4;
        maxRoomDist = 4;
        hallwayOpening = 6;
        branchType = "random";
    }
}
