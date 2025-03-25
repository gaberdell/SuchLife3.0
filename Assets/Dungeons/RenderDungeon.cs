
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
    [SerializeField] Tilemap groundTilemap;
    [SerializeField] Tilemap wallTilemap;
    [SerializeField] TextAsset roomsFile;
    [SerializeField] Tile grassTile;
    [SerializeField] RuleTile wallTile;
    [SerializeField] Tile bonusTile;
    [SerializeField] GameObject dungeonExit;
    RoomContainer rooms;
    Tilemap dungeonTilemap; //deprecated; replace with ground tilemap and wall tilemap instead
    EventManager eventManager;

    // TOREVIEW: macros for grass and wall tile sprite names
    string GRASS_SPRITE_NAME;
    string WALL_SPRITE_NAME;

    private void Start()
    {
        eventManager = EventManager.Instance;
    }
    public void StartRender(GameObject dungeonEntrance){
        
        DungeonOptions opts = new DungeonOptions();
        //create dungeon graph
        DungeonGraph nodeGraph = new DungeonGraph(opts);
        //for basic testing
        //for each node in the graph, set the tile with the same index to a grasstile
        //for (int i = 0; i < nodeGraph.layout.Count; i++)
        //{
        //    DungeonNode node = nodeGraph.layout[i];
        //    if(node.type == "Start")
        //    {
        //        groundTilemap.SetTile(new Vector3Int(node.xPos * 4 + 50, node.yPos * 4, 0), bonusTile);
        //    } else
        //    {
        //        groundTilemap.SetTile(new Vector3Int(node.xPos * 4 + 50, node.yPos * 4, 0), grassTile);
        //    }
            
        //    //draw edges
        //    for (int j = 0; j < node.edges.Count; j++)
        //    {
        //        //draw em somehow
        //        switch (node.edges[j].Item2)
        //        {
        //            case "top":
        //                wallTilemap.SetTile(new Vector3Int(node.xPos * 4 + 50, node.yPos * 4 - 2, 0), wallTile);
        //                break;
        //            case "bottom":
        //                wallTilemap.SetTile(new Vector3Int(node.xPos * 4 + 50, node.yPos * 4 + 2, 0), wallTile);
        //                break;
        //            case "right":
        //                wallTilemap.SetTile(new Vector3Int(node.xPos * 4 + 2 + 50, node.yPos * 4, 0), wallTile);
        //                break;
        //            case "left":
        //                wallTilemap.SetTile(new Vector3Int(node.xPos * 4 - 2 + 50, node.yPos * 4, 0), wallTile);
        //                break;
        //        }
                
        //    }
        //}
        // Render graph one node at a time starting from the start node
        //will draw the room, draw a neighboring node at a random distance away(set in opts), and draw a hallway connecting them(and add the new node to a queue)
        //Rooms are read from static files and chosen randomly, with bias on room layouts not already existing
        //Repeat for every edge remaining on the current node, and then move on to the next node in the queue
        //If neighboring node already exists on the map then just draw a hallway between
        //Randomize position of hallway on the sides

        //load rooms
        rooms = JsonUtility.FromJson<RoomContainer>(roomsFile.text);
        rooms.createIndexes();
        //start by drawing the starting room, which will always be at the 0th index(?)
        createStartingRoom(nodeGraph, opts, dungeonEntrance);
        
        //now branch off of the starting room, following the edges and creating connecting rooms one at a time
        //recursively?

        // TOREVIEW: saving sprite name in a macro for saving/loading
        GRASS_SPRITE_NAME = grassTile.sprite.name;
        //WALL_SPRITE_NAME = wallTile.sprite.name;
        WALL_SPRITE_NAME = "blueBricks";

        //after dungeon is generated
        fillBackground();
        //trigger player enter dungeon after dungeon is generated
        EventManager.SetPlayerEnterDungeon();

    }

    void fillBackground()
    {
        
        int x_min = groundTilemap.cellBounds.min.x-10;
        int x_max = groundTilemap.cellBounds.max.x+10;
        int y_min = groundTilemap.cellBounds.min.y-10;
        int y_max = groundTilemap.cellBounds.max.y+10;

        for (int x = x_min; x < x_max; x++)
        {
            for (int y = y_min; y < y_max; y++)
            {
                Tile tile = (Tile)groundTilemap.GetTile(new Vector3Int(x, y, 0));

                if (tile == null)
                {
                    //place a wall if theres no ground tile
                    wallTilemap.SetTile(new Vector3Int(x, y, 0), wallTile);
                }
            }
        }
    }

    void createStartingRoom(DungeonGraph nodeGraph, DungeonOptions opts, GameObject entrance)
    {
        DungeonNode startingNode = new DungeonNode();
        //find starting room in nodegraph
        for(int i = 0; i < nodeGraph.layout.Count; i++)
        {
            if(nodeGraph.layout[i].type == "Start")
            {
                startingNode = nodeGraph.layout[i];
                //print(i);
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
                        wallTilemap.SetTile(new Vector3Int(j, i, 0), wallTile);
                        break;

                    case 'F':
                        groundTilemap.SetTile(new Vector3Int(j, i, 0), grassTile);
                        break;
                }
            }
        }
        //handle entities
        for(int i = 0; i < startingNode.roomInfo.entities.Count; i++)
        {
            string entity = startingNode.roomInfo.entities[i];
            if (entity == "DungeonExit")
            {
                //create dungeon exit from prefab
                GameObject exitInstance = Instantiate(dungeonExit);
                exitInstance.transform.position = groundTilemap.transform.position + new Vector3Int(startingNode.drawXPos + startingNode.roomInfo.width/2, startingNode.drawYPos + startingNode.roomInfo.width/2, 0);
                exitInstance.GetComponent<HandleDungeonExit>().setEntrance(entrance);
            }
        }
        //for each edge, create the room
        for (int i = 0; i < startingNode.edges.Count; i++)
        {
            createRoom(startingNode.edges[i], opts, startingNode);
        }
        groundTilemap.SetTile(new Vector3Int(0, 0, 0), bonusTile);
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
                        wallTilemap.SetTile(new Vector3Int(prevRoom.drawXPos + j + offsetX, prevRoom.drawYPos + i + offsetY, 0), wallTile);
                        break;

                    case 'F':
                        groundTilemap.SetTile(new Vector3Int(prevRoom.drawXPos + j + offsetX, prevRoom.drawYPos + i + offsetY, 0), grassTile);
                        break;
                }
            }
        }
        //connect rooms
        connectRooms(prevRoom, roomNode, prevEdge, opts);
        //create rooms edges; 
        for (int i = 0; i < roomNode.edges.Count; i++)
        {
            //if room already exists, then check if the edge between them has been drawn already
            //if (roomNode.edges[i].Item1.drawXPos != 0) { connectRooms(roomNode, roomNode.edges[i].Item1, roomNode.edges[i].Item2); }
            //draw room if it hasnt been drawn yet
            if (roomNode.edges[i].Item1.drawXPos == 0 && roomNode.edges[i].Item1.drawYPos == 0 && roomNode.edges[i].Item1.type != "Start")
            {;
                createRoom(roomNode.edges[i], opts, roomNode);
            }

        }
    }

    void connectRooms(DungeonNode prevNode, DungeonNode newNode, string prevEdge, DungeonOptions opts)
    {
        //draw a straight hallway between the two rooms

        //decide on width of hallway (uniform?)
        int hallWaySpace = opts.hallwayOpening; //width/height of hallway
        int Hheight = 0;
        int Hwidth = 0;
        int x = 0; //rect draws from bottom left
        int y = 0;

        //decide on where to start the hallway
        //for vertical hallways, select a width and a 'left' point to start the hallway from
        int xStart = prevNode.drawXPos + (int)UnityEngine.Random.Range(0, Math.Min(prevNode.roomInfo.width - hallWaySpace, newNode.roomInfo.width - hallWaySpace));
        //for horizontal hallways, select a width and a  'bottom' point to start the hallway from
        int yStart = prevNode.drawYPos + (int)UnityEngine.Random.Range(0, Math.Min(prevNode.roomInfo.height - hallWaySpace, newNode.roomInfo.height - hallWaySpace));
        switch (prevEdge)
        {
            case "bottom":
                Hwidth = hallWaySpace;
                Hheight = prevNode.drawYPos - (newNode.drawYPos + newNode.roomInfo.height);
                x = prevNode.drawXPos + 1;
                y = newNode.drawYPos + newNode.roomInfo.height;
                drawHallway(Hwidth, Hheight, xStart, y, "vertical");
                break;
            case "top":
                Hwidth = hallWaySpace;
                Hheight = newNode.drawYPos - (prevNode.drawYPos + prevNode.roomInfo.height);
                x = newNode.drawXPos + 1;
                y = prevNode.drawYPos + prevNode.roomInfo.height;
                drawHallway(Hwidth, Hheight, xStart, y, "vertical");
                break;
            case "left":
                Hwidth = prevNode.drawXPos - (newNode.drawXPos + newNode.roomInfo.width);
                Hheight = hallWaySpace;
                x = newNode.drawXPos + newNode.roomInfo.width;
                y = prevNode.drawYPos + 1;
                drawHallway(Hwidth, Hheight, x, yStart, "horizontal");
                break;
            case "right":
                Hwidth = newNode.drawXPos - (prevNode.drawXPos + prevNode.roomInfo.width);
                Hheight = hallWaySpace;
                x = prevNode.drawXPos + prevNode.roomInfo.width;
                y = prevNode.drawYPos + 1;
                drawHallway(Hwidth, Hheight, x, yStart, "horizontal");
                break;
        }
        
        
    }

    void drawHallway(int width, int height, int startX, int startY, string type)
    {
        List<Tuple<int, int>> hallwayEndPos = new List<Tuple<int, int>>();
        for (int i = 0; i < width; i++) //x
        {
            for (int j = 0; j < height; j++) //y
            {
                //handle wall placement for vertical and horizontal hallways
                if(type == "vertical")
                {
                    if (i == 0 || i == width - 1)
                    {
                        //wallTilemap.SetTile(new Vector3Int(i + startX, j + startY, 0), wallTile);
                    }
                    else
                    {
                        if(j == 0 || j == height - 1)
                        {
                            hallwayEndPos.Add(new Tuple<int, int>(i, j));
                            //CLEAR WALLS THAT OVERLAP WITH HALLWAY
                            wallTilemap.SetTile(new Vector3Int(i + startX, j + startY, 0), null);
                            wallTilemap.SetTile(new Vector3Int(i + startX, j + startY-1, 0), null);
                            wallTilemap.SetTile(new Vector3Int(i + startX, j + startY+1, 0), null);
                        }
                        groundTilemap.SetTile(new Vector3Int(i + startX, j + startY, 0), grassTile);

                    }
                } else
                {
                    if (j == 0 || j == height - 1)
                    {
                        //wallTilemap.SetTile(new Vector3Int(i + startX, j + startY, 0), wallTile);
                    }
                    else
                    {
                        if (i == 0 || i == width - 1)
                        {
                            hallwayEndPos.Add(new Tuple<int, int>(i, j));
                            //CLEAR WALLS THAT OVERLAP WITH HALLWAY
                            wallTilemap.SetTile(new Vector3Int(i + startX, j + startY, 0), null);
                            wallTilemap.SetTile(new Vector3Int(i + startX+1, j + startY, 0), null);
                            wallTilemap.SetTile(new Vector3Int(i + startX-1, j + startY, 0), null);

                        }
                        groundTilemap.SetTile(new Vector3Int(i + startX, j + startY, 0), grassTile);
                        
                    }
                }

                
            }
        }




        //check if hallway actually reached target destination (neighboring tiles arent background), otherwise draw another hallway
        //for each final tile
        //for (int i = 0; i < hallwayEndPos.Count; i++) {
        //    //check if tile is neighbored by a background tile
        //    Tile tile1 = (Tile)dungeonTilemap.GetTile(new Vector3Int(hallwayEndPos[i].Item1+1, hallwayEndPos[i].Item2, 0));
        //    Tile tile2 = (Tile)dungeonTilemap.GetTile(new Vector3Int(hallwayEndPos[i].Item1-1, hallwayEndPos[i].Item2, 0));
        //    Tile tile3 = (Tile)dungeonTilemap.GetTile(new Vector3Int(hallwayEndPos[i].Item1, hallwayEndPos[i].Item2+1, 0));
        //    Tile tile4 = (Tile)dungeonTilemap.GetTile(new Vector3Int(hallwayEndPos[i].Item1, hallwayEndPos[i].Item2-1, 0));
        //    //based on the location of the background tile, redraw the hallway
        //    if (tile1 == null || tile2 == null || tile3 == null || tile4 == null)
        //    {
        //        print("null neighbor");
        //    }

        //    //if (tile1 == null) { drawHallway(width, height, startX + width, startY, type); return; }
        //    //if (tile2 == null) { drawHallway(width, height, startX - width, startY, type); return; }
        //    //if (tile3 == null) { drawHallway(width, height, startX, startY + height, type); return; }
        //    //if (tile4 == null) { drawHallway(width, height, startX, startY - height, type); return; }
        //}
    }

    public Tilemap getDungeonTilemap() { return dungeonTilemap; } // TOREVIEW: needed for use by DataService to clear tilemap before loading
    
    
    
    public string Save() {
        int x_min = dungeonTilemap.cellBounds.min.x;
        int x_max = dungeonTilemap.cellBounds.max.x;
        int y_min = dungeonTilemap.cellBounds.min.y;
        int y_max = dungeonTilemap.cellBounds.max.y;

        string json = "";
        for (int x = x_min; x < x_max; x++) {
            for (int y = y_min; y < y_max; y++) {
                Tile tile = (Tile)dungeonTilemap.GetTile(new Vector3Int(x, y, 0));
                
                if (tile != null) {
                    TileCache tileCache = new TileCache(tile.sprite.name, x, y); // saving tile data in a temporary object

                    string spriteName = tileCache.spriteName;
                    if (spriteName.Equals(GRASS_SPRITE_NAME) || spriteName.Equals(WALL_SPRITE_NAME))
                    json += tileCache.Save() + "\n";
                }
            }
        }
        
        return json;
    }
    
    public void Load(string json) {
        TileCache tileCache = new TileCache();
        tileCache.Load(json);
            
        if (tileCache.spriteName.Equals(GRASS_SPRITE_NAME)) // 
            dungeonTilemap.SetTile(new Vector3Int(tileCache.x, tileCache.y, 0), grassTile);
        if (tileCache.spriteName.Equals(WALL_SPRITE_NAME))
            dungeonTilemap.SetTile(new Vector3Int(tileCache.x, tileCache.y, 0), wallTile);
    }
    
}
