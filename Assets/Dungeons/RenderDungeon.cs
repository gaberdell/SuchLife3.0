
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class RenderDungeon : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] Tilemap dungeonTilemap;
    [SerializeField] TextAsset roomsFile;
    [SerializeField] Tile grassTile;
    [SerializeField] Tile wallTile;
    [SerializeField] Tile bonusTile;
    RoomContainer rooms;
    

    void Start(){
        DungeonOptions opts = new DungeonOptions();
        //create dungeon graph
        DungeonGraph nodeGraph = new DungeonGraph(opts);
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

        //load rooms
        Debug.Log(roomsFile.text);
        rooms = JsonUtility.FromJson<RoomContainer>(roomsFile.text);
        foreach(RoomInfo room in rooms.rooms)
        {
            Debug.Log("Found room " + room.type);
        }
        rooms.createIndexes();
        //start by drawing the starting room, which will always be at the 0th index(?)
        createStartingRoom(nodeGraph, opts);
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
        //List<List<string>> roomLayout = createRoomLayout("Start");
        startingNode.roomInfo = rooms.getRoom(startingNode.type);
        List<string> roomLayout = startingNode.roomInfo.tileLayout;
        for (int i = 0; i < roomLayout.Count; i++) //column index
        {
            char[] row = roomLayout[i].ToCharArray();
            for (int j = 0; j < row.Length; j++) //row index
            {
                //draw room on tilemap based on layout string
                switch (roomLayout[i][j])
                {
                    case 'W':
                        dungeonTilemap.SetTile(new Vector3Int(j, i, 0), wallTile);
                        break;

                    case 'F':
                        dungeonTilemap.SetTile(new Vector3Int(j, i, 0), grassTile);
                        break;
                }
            }
        }
        //for each edge, create the room
        for (int i = 0; i < startingNode.edges.Count; i++)
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
        roomNode.roomInfo = rooms.getRoom(roomNode.type);
        List<string> roomLayout = roomNode.roomInfo.tileLayout;

        //calculate offset based on location of previous room
        int offset  = (int)UnityEngine.Random.Range(opts.minRoomDist, opts.maxRoomDist);
        //int offset = 0;
        int offsetX = 0;
        int offsetY = 0;
        switch (prevEdge) //POSITIVE Y DIRECTION IS DOWN???
            //?????
        {
            case "bottom":
                offsetY -= (offset + roomNode.roomInfo.height);
                break;
            case "top":
                offsetY += (offset + prevRoom.roomInfo.height);
                //offsetY = 0;
                break;
            case "left":
                offsetX -= (offset + roomNode.roomInfo.width);
                break;
            case "right":
                offsetX += (offset + prevRoom.roomInfo.width);
                break;
        }
        roomNode.drawXPos = prevRoom.drawXPos + offsetX;
        roomNode.drawYPos = prevRoom.drawYPos + offsetY;


        //drawing starts from bottom left corner of room; draw room relative to prev room
        //for each edge, create room and connect if not starting room
        for (int i = 0; i < roomLayout.Count; i++) //column index
        {
            char[] row = roomLayout[i].ToCharArray();
            for (int j = 0; j < row.Length; j++) //row index
            {
                //draw room
                switch (roomLayout[i][j])
                {
                    case 'W':
                        dungeonTilemap.SetTile(new Vector3Int(prevRoom.drawXPos + j + offsetX, prevRoom.drawYPos + i + offsetY, 0), wallTile);
                        break;

                    case 'F':
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
            //if room already exists, then check if the edge between them has been drawn already
            if (roomNode.edges[i].Item1.drawXPos == 0 && roomNode.edges[i].Item1.drawYPos == 0 && roomNode.edges[i].Item1.type != "Start")
            {;
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
                Hheight = prevNode.drawYPos - (newNode.drawYPos + newNode.roomInfo.height);
                x = prevNode.drawXPos + 1;
                y = newNode.drawYPos + newNode.roomInfo.height;
                drawHallway(Hwidth, Hheight, x, y, "vertical");
                break;
            case "top":
                Hwidth = 3;
                Hheight = newNode.drawYPos - (prevNode.drawYPos + prevNode.roomInfo.height);
                x = newNode.drawXPos + 1;
                y = prevNode.drawYPos + prevNode.roomInfo.height;
                drawHallway(Hwidth, Hheight, x, y, "vertical");
                break;
            case "left":
                Hwidth = prevNode.drawXPos - (newNode.drawXPos + newNode.roomInfo.width);
                Hheight = 3;
                x = newNode.drawXPos + newNode.roomInfo.width;
                y = prevNode.drawYPos + 1;
                drawHallway(Hwidth, Hheight, x, y, "horizontal");
                break;
            case "right":
                Hwidth = newNode.drawXPos - (prevNode.drawXPos + prevNode.roomInfo.width);
                Hheight = 3;
                x = prevNode.drawXPos + prevNode.roomInfo.width;
                y = prevNode.drawYPos + 1;
                drawHallway(Hwidth, Hheight, x, y, "horizontal");
                break;
        }
        
    }

    void drawHallway(int width, int height, int startX, int startY, string type)
    {
        for (int i = 0; i < width; i++) //x
        {
            for (int j = 0; j < height; j++) //y
            {
                //handle wall placement for vertical and horizontal hallways
                if(type == "vertical")
                {
                    if (i == 0 || i == width - 1)
                    {
                        dungeonTilemap.SetTile(new Vector3Int(i + startX, j + startY, 0), wallTile);
                    }
                    else
                    {
                        dungeonTilemap.SetTile(new Vector3Int(i + startX, j + startY, 0), grassTile);
                    }
                } else
                {
                    if (j == 0 || j == height - 1)
                    {
                        dungeonTilemap.SetTile(new Vector3Int(i + startX, j + startY, 0), wallTile);
                    }
                    else
                    {
                        dungeonTilemap.SetTile(new Vector3Int(i + startX, j + startY, 0), grassTile);
                    }
                }

                
            }
        }
    }
    
}
