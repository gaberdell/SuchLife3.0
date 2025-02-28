
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class RenderDungeon : MonoBehaviour, Saveable
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] Tilemap dungeonTilemap;
    [SerializeField] Tile grassTile;
    [SerializeField] Tile wallTile;
    [SerializeField] Tile bonusTile;
    
    // v Jason was here
    public DungeonGraph nodeGraph;

    void Start(){
        DungeonOptions opts = new DungeonOptions();
        //create dungeon graph
        nodeGraph = new DungeonGraph(opts);
        //for basic testing
        //for each node in the graph, set the tile with the same index to a grasstile
        for (int i = 0; i < nodeGraph.layout.Count; i++)
        {
            DungeonNode node = nodeGraph.layout[i];
            if(node.type == "Start")
            {
                dungeonTilemap.SetTile(new Vector3Int(node.xPos * 4 + 50, node.yPos * 4, 0), bonusTile);
            } else
            {
                dungeonTilemap.SetTile(new Vector3Int(node.xPos * 4 + 50, node.yPos * 4, 0), grassTile);
            }
            
            //draw edges
            for (int j = 0; j < node.edges.Count; j++)
            {
                //draw em somehow
                switch (node.edges[j].Item2)
                {
                    case "top":
                        dungeonTilemap.SetTile(new Vector3Int(node.xPos * 4 + 50, node.yPos * 4 - 2, 0), wallTile);
                        break;
                    case "bottom":
                        dungeonTilemap.SetTile(new Vector3Int(node.xPos * 4 + 50, node.yPos * 4 + 2, 0), wallTile);
                        break;
                    case "right":
                        dungeonTilemap.SetTile(new Vector3Int(node.xPos * 4 + 2 + 50, node.yPos * 4, 0), wallTile);
                        break;
                    case "left":
                        dungeonTilemap.SetTile(new Vector3Int(node.xPos * 4 - 2 + 50, node.yPos * 4, 0), wallTile);
                        break;
                }
                
            }
        }
        // Render graph one node at a time starting from the start node
        //will draw the room, draw a neighboring node at a random distance away(set in opts), and draw a hallway connecting them(and add the new node to a queue)
        //Rooms are read from static files and chosen randomly, with bias on room layouts not already existing
        //Repeat for every edge remaining on the current node, and then move on to the next node in the queue
        //If neighboring node already exists on the map then just draw a hallway between
        //Randomize position of hallway on the sides



        //start by drawing the starting room, which will always be at the 0th index(?)
        createStartingRoom(nodeGraph, opts);

        //now branch off of the starting room, following the edges and creating connecting rooms one at a time
        //recursively?

    }

    void createStartingRoom(DungeonGraph nodeGraph, DungeonOptions opts)
    {
        DungeonNode startingNode = new DungeonNode();
        //find starting room in nodegraph
        for(int i = 0; i < nodeGraph.layout.Count; i++)
        {
            if(nodeGraph.layout[i].type == "Start")
            {
                startingNode = nodeGraph.layout[i];
                print(i);
            }
        }
        //draw starting room
        List<List<string>> roomLayout = createRoomLayout("Start");
        for (int i = 0; i < roomLayout.Count; i++) //column index
        {
            for (int j = 0; j < roomLayout[i].Count; j++) //row index
            {
                //draw room on tilemap based on layout string
                switch (roomLayout[i][j])
                {
                    case "W":
                        dungeonTilemap.SetTile(new Vector3Int(j, i, 0), wallTile);
                        break;

                    case "F":
                        dungeonTilemap.SetTile(new Vector3Int(j, i, 0), grassTile);
                        break;
                }
            }
        }
        //update layout
        startingNode.layout = roomLayout;
        startingNode.width = roomLayout[0].Count;
        startingNode.height = roomLayout.Count;
        //for each edge, create the room
        for(int i = 0; i < startingNode.edges.Count; i++)
        {
            createRoom(startingNode.edges[i], opts, startingNode);
        }
        dungeonTilemap.SetTile(new Vector3Int(0, 0, 0), bonusTile);
    }

    void createRoom(Tuple<DungeonNode, string> roomInfo, DungeonOptions opts, DungeonNode prevRoom)
    {
        DungeonNode roomNode = roomInfo.Item1;
        string prevEdge = roomInfo.Item2;
        //read in room info based on room type
        List<List<string>> roomLayout = createRoomLayout(roomNode.type);
        roomNode.width = roomLayout[0].Count;
        roomNode.height = roomLayout.Count;

        //calculate offset based on location of previous room
        int offset  = (int)UnityEngine.Random.Range(opts.minRoomDist, opts.maxRoomDist);
        //int offset = 0;
        int offsetX = 0;
        int offsetY = 0;
        switch (prevEdge) //POSITIVE Y DIRECTION IS DOWN???
            //?????
        {
            case "bottom":
                offsetY -= (offset + roomNode.height);
                break;
            case "top":
                offsetY += (offset + prevRoom.height);
                //offsetY = 0;
                break;
            case "left":
                offsetX -= (offset + roomNode.width);
                break;
            case "right":
                offsetX += (offset + prevRoom.width);
                break;
        }
        roomNode.drawXPos = prevRoom.drawXPos + offsetX;
        roomNode.drawYPos = prevRoom.drawYPos + offsetY;


        //drawing starts from bottom left corner of room; draw room relative to prev room
        //for each edge, create room and connect if not starting room
        for (int i = 0; i < roomLayout.Count; i++) //column index
        {
            for(int j = 0; j < roomLayout[i].Count; j++) //row index
            {
                //draw room
                switch (roomLayout[i][j])
                {
                    case "W":
                        dungeonTilemap.SetTile(new Vector3Int(prevRoom.drawXPos + j + offsetX, prevRoom.drawYPos + i + offsetY, 0), wallTile);
                        break;

                    case "F":
                        dungeonTilemap.SetTile(new Vector3Int(prevRoom.drawXPos + j + offsetX, prevRoom.drawYPos + i + offsetY, 0), grassTile);
                        break;
                }
            }
        }
        //connect rooms
        connectRooms(prevRoom, roomNode, prevEdge);
        //create rooms edges; 
        for (int i = 0; i < roomNode.edges.Count; i++)
        {
            //skip if already created (only draw nodes that havent already been placed)
            if (roomNode.edges[i].Item1.drawXPos == 0 && roomNode.edges[i].Item1.drawYPos == 0)
            {
                print("Create Room");
                createRoom(roomNode.edges[i], opts, roomNode);
            }

        }
    }

    void connectRooms(DungeonNode prevNode, DungeonNode newNode, string prevEdge)
    {
        //draw a straight hallway between the two rooms

        //decide on width of hallway (uniform?)
        int Hheight = 0;
        int Hwidth = 0;
        int x = 0; //rect draws from bottom left
        int y = 0;
        
        //decide on where to start the hallway
        switch (prevEdge)
        {
            case "bottom":
                Hwidth = 3;
                Hheight = prevNode.drawYPos - (newNode.drawYPos + newNode.height);
                x = prevNode.drawXPos + 1;
                y = newNode.drawYPos + newNode.height;
                drawHallway(Hwidth, Hheight, x, y);
                break;
            case "top":
                Hwidth = 3;
                Hheight = newNode.drawYPos - (prevNode.drawYPos + prevNode.height);
                x = newNode.drawXPos + 1;
                y = prevNode.drawYPos + prevNode.height;
                drawHallway(Hwidth, Hheight, x, y);
                break;
            case "left":
                Hwidth = prevNode.drawXPos - (newNode.drawXPos + newNode.width);
                Hheight = 3;
                x = newNode.drawXPos + newNode.width;
                y = prevNode.drawYPos + 1;
                drawHallway(Hwidth, Hheight, x, y);
                break;
            case "right":
                Hwidth = newNode.drawXPos - (prevNode.drawXPos + prevNode.width);
                Hheight = 3;
                x = prevNode.drawXPos + prevNode.width;
                y = prevNode.drawYPos + 1;
                drawHallway(Hwidth, Hheight, x, y);
                break;
        }
        
    }

    void drawHallway(int width, int height, int startX, int startY)
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                dungeonTilemap.SetTile(new Vector3Int(i + startX, j + startY, 0), wallTile);
            }
        }
    }
    
    List<List<string>> createRoomLayout(string type)
    {
        //read in starting room info from json file (right now am creating manually to test)

        //create a room layout for testing
        //read in room info based on room type
        List<List<string>> roomLayout = new List<List<string>>();
        List<string> roomRow1 = new List<string>();
        List<string> roomRow2 = new List<string>();
        int roomLength = 5;
        int roomHeight = 5;
        switch (type)
        {
            case "Start":
                roomLength = 10;
                roomHeight = 10;
                for (int i = 0; i < roomLength; i++)
                {
                    roomRow1.Add("W");
                    if (i == 0 || i == roomLength-1)
                    {
                        roomRow2.Add("W");
                    }
                    else
                    {
                        roomRow2.Add("F");
                    }
                }
                for (int i = 0; i < roomHeight; i++)
                {
                    if (i == 0 || i == roomHeight-1)
                    {
                        roomLayout.Add(roomRow1);
                    }
                    else
                    {
                        roomLayout.Add(roomRow2);
                    }
                }
                break;

            case "Default":
                for (int i = 0; i < roomLength; i++)
                {
                    roomRow1.Add("W");
                    if (i == 0 || i == roomLength - 1)
                    {
                        roomRow2.Add("W");
                    }
                    else
                    {
                        roomRow2.Add("F");
                    }
                }
                for (int i = 0; i < roomHeight; i++)
                {
                    if (i == 0 || i == roomHeight - 1)
                    {
                        roomLayout.Add(roomRow1);
                    }
                    else
                    {
                        roomLayout.Add(roomRow2);
                    }
                }
                break;
        }
        return roomLayout;
    }
    
    public string Save() { return nodeGraph.Save(); }
    public void Load(string json) { nodeGraph.Load(json); }
}
