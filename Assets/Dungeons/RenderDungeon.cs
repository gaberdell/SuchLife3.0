
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using UnityEngine.UIElements;
using static UnityEngine.GraphicsBuffer;

public class RenderDungeon : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] Tilemap groundTilemap;
    [SerializeField] Tilemap wallTilemap;
    [SerializeField] TextAsset roomsFile;
    [SerializeField] Tile grassTile;
    [SerializeField] RuleTile wallTile;
    [SerializeField] Tile bonusTile;
    [SerializeField] GameObject dungeonExit;
    [SerializeField] GameObject bombEnemy;
    [SerializeField] Tile[] groundTiles;
    RoomContainer rooms;
    Tilemap dungeonTilemap; //deprecated; replace with ground tilemap and wall tilemap instead
    EventManager eventManager;
    DungeonOptions opts;
    

    int dungeonOffsetX;
    int dungeonOffsetY;

    // TOREVIEW: macros for grass and wall tile sprite names
    string GRASS_SPRITE_NAME;
    string WALL_SPRITE_NAME;

    private void Start()
    {
        eventManager = EventManager.Instance;
    }
    public void StartRender(GameObject dungeonEntrance, int offsetX, int offsetY, DungeonOptions dopts){
        dungeonOffsetX = offsetX;
        dungeonOffsetY = offsetY;
        opts = dopts;

        //set colors
        switch (dopts.generationStyle)
        {
            case "cave":
                foreach (Tile t in groundTiles)
                {
                    t.color = Color.gray;
                }
                break;

            case "default":
                foreach (Tile t in groundTiles)
                {
                    t.color = Color.white;
                }
                break;
        }

        //create dungeon graph
        DungeonGraph nodeGraph = new DungeonGraph(opts);

       
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
        createStartingRoom(nodeGraph, dungeonEntrance);
        
        //now branch off of the starting room, following the edges and creating connecting rooms one at a time
        //recursively?

        // TOREVIEW: saving sprite name in a macro for saving/loading
        GRASS_SPRITE_NAME = grassTile.sprite.name;
        //WALL_SPRITE_NAME = wallTile.sprite.name;
        WALL_SPRITE_NAME = "blueBricks";

        //after dungeon is generated
        fillBackground(nodeGraph);
    }

    void fillBackground(DungeonGraph nodeGraph)
    {

        //find left-most, top-most, right-most, bottom-most nodes
        int xmin = nodeGraph.layout[2].drawXPos;
        int xmax = nodeGraph.layout[2].drawXPos;
        int ymin = nodeGraph.layout[2].drawYPos;
        int ymax = nodeGraph.layout[2].drawYPos;
        
        for (int i = 0; i < nodeGraph.layout.Count; i++)
        {
            DungeonNode node = nodeGraph.layout[i];
            if (node.drawXPos < xmin) xmin = node.drawXPos;
            if (node.drawXPos + node.roomInfo.width > xmax) xmax = node.drawXPos + node.roomInfo.width;
            if (node.drawYPos < ymin) ymin = node.drawYPos;
            if (node.drawYPos + node.roomInfo.height > ymax) ymax = node.drawYPos + node.roomInfo.height;
        }
        xmin += opts.dungeonOffsetX - 10;
        xmax += opts.dungeonOffsetX + 10;
        ymin += opts.dungeonOffsetY - 10;
        ymax += opts.dungeonOffsetY + 10;



        //REMOVE
        EventManager.SetPlayerEnterDungeon(opts.dungeonOffsetX, opts.dungeonOffsetY, xmax - xmin, ymax - ymin);
        return;



        for (int x = xmin; x < xmax; x++)
        {
            for (int y = ymin; y < ymax; y++)
            {
                Tile tile = (Tile)groundTilemap.GetTile(new Vector3Int(x, y, 0));

                if (tile == null)
                {
                    //place a wall if theres no ground tile
                    wallTilemap.SetTile(new Vector3Int(x, y, 0), wallTile);
                }
            }
        }
        //now fill ground tilemap so player doesnt see void when they break walls
        float scale = .01f;
        for (int x = xmin; x < xmax; x++)
        {
            for (int y = ymin; y < ymax; y++)
            {
                //Tile tile = (Tile)groundTilemap.GetTile(new Vector3Int(x, y, 0));

                //if (tile == null)
                //{
                //    //place a ground tile if theres no ground tile
                //    groundTilemap.SetTile(new Vector3Int(x, y, 0), grassTile);
                //}
                //use perlin noise to pick ground tiles
                float perlinNum = Mathf.PerlinNoise(x/(xmax*scale), y/(ymax*scale));
                //Debug.Log(perlinNum);
                if (perlinNum < .2)
                {
                    groundTilemap.SetTile(new Vector3Int(x, y, 0), groundTiles[0]);
                } else if(perlinNum < .4)
                {
                    groundTilemap.SetTile(new Vector3Int(x, y, 0), groundTiles[1]);
                } else if (perlinNum < .6)
                {
                    groundTilemap.SetTile(new Vector3Int(x, y, 0), groundTiles[2]);
                } else
                {
                    groundTilemap.SetTile(new Vector3Int(x, y, 0), groundTiles[3]);
                }
            }
        }
        //finish dungeon render
        EventManager.SetPlayerEnterDungeon(opts.dungeonOffsetX, opts.dungeonOffsetY, xmax - xmin, ymax - ymin);
        //foreach (Tile t in groundTiles)
        //{
        //    t.color = Color.white;
        //}
    }

    void createStartingRoom(DungeonGraph nodeGraph, GameObject entrance)
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
        drawRoomOnTilemap(0, 0, startingNode.roomInfo, startingNode);
        //create entities
        //Debug.Log("entities");
        //Debug.Log(startingNode.roomInfo.entities.Length);
        //Debug.Log(startingNode.roomInfo.entities[0]);
        for(int i = 0; i < startingNode.roomInfo.entities.Length; i++)
        {
            EntityInfo entity = startingNode.roomInfo.entities[i];
            if (entity.entityName == "DungeonExit")
            {
                //create dungeon exit from prefab
                GameObject exitInstance = Instantiate(dungeonExit);
                exitInstance.transform.position = groundTilemap.transform.position + new Vector3Int(startingNode.drawXPos + startingNode.roomInfo.width/2 + dungeonOffsetX, startingNode.drawYPos + startingNode.roomInfo.width/2 + dungeonOffsetY, 0);
                exitInstance.GetComponent<HandleDungeonExit>().setEntrance(entrance);
            }
        }
        //for each edge, create the room
        for (int i = 0; i < startingNode.edges.Count; i++)
        {
            createRoom(startingNode.edges[i], startingNode);
        }
    }
    
    void createRoom(Tuple<DungeonNode, string> roomInfo, DungeonNode prevRoom)
    {
        DungeonNode roomNode = roomInfo.Item1;
        string prevEdge = roomInfo.Item2;
        //read in room info based on room type
        if(opts.generationStyle == "cave" && roomNode.type == "Default")
        {
            roomNode.roomInfo = rooms.getRoom("Cave");
        } else
        {
            roomNode.roomInfo = rooms.getRoom(roomNode.type);
        }
        
        List<string> roomLayout = roomNode.roomInfo.tileLayout;

        //calculate offset based on location of previous room
        int offset  = (int)UnityEngine.Random.Range(opts.minRoomDist, opts.maxRoomDist);
        //int offset = 0;
        int offsetX = 0;
        int offsetY = 0;
        switch (prevEdge) 
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

        //draw room in function for clarity
        drawRoomOnTilemap(offsetX, offsetY, roomNode.roomInfo, prevRoom);


        //create entities
        for (int i = 0; i < roomNode.roomInfo.entities.Length; i++)
        {
            EntityInfo entity = roomNode.roomInfo.entities[i];
            if (entity.entityName == "Bomb")
            {
                Instantiate(bombEnemy, new Vector3Int(roomNode.drawXPos + entity.relativeX + dungeonOffsetX, roomNode.drawYPos + entity.relativeY + dungeonOffsetY, 0), Quaternion.identity);
                //create trackable entity info somewhere?
            }
        }

        //connect rooms
        if (opts.createHallways == false)
        {
            //dont connect rooms for noHallways style
        } else
        {
            connectRooms(prevRoom, roomNode, prevEdge);
        }
        
        //create rooms edges; 
        //for each edge, create room and connect if not starting room
        for (int i = 0; i < roomNode.edges.Count; i++)
        {
            //if room already exists, then check if the edge between them has been drawn already
            //if (roomNode.edges[i].Item1.drawXPos != 0) { connectRooms(roomNode, roomNode.edges[i].Item1, roomNode.edges[i].Item2); }
            //draw room if it hasnt been drawn yet
            if (roomNode.edges[i].Item1.drawXPos == 0 && roomNode.edges[i].Item1.drawYPos == 0 && roomNode.edges[i].Item1.type != "Start")
            {;
                createRoom(roomNode.edges[i], roomNode);
            }

        }
    }

    void drawRoomOnTilemap(int offsetX, int offsetY, RoomInfo roomInfo, DungeonNode prevRoom)
    {
        List<string> roomLayout = roomInfo.tileLayout;
        //drawing starts from bottom left corner of room; draw room relative to prev room
        for (int i = 0; i < roomLayout.Count; i++) //column index
        {
            char[] row = roomLayout[i].ToCharArray();
            for (int j = 0; j < row.Length; j++) //row index
            {
                //draw room
                switch (roomLayout[i][j])
                {
                    case 'W':
                        //wallTilemap.SetTile(new Vector3Int(prevRoom.drawXPos + j + offsetX + dungeonOffsetX, prevRoom.drawYPos + i + offsetY + dungeonOffsetY, 0), wallTile);
                        ChunkManager.SetTile(new Vector3Int(prevRoom.drawXPos + j + offsetX + dungeonOffsetX, prevRoom.drawYPos + i + offsetY + dungeonOffsetY, 0), wallTile);
                        break;

                    case 'F':
                        //groundTilemap.SetTile(new Vector3Int(prevRoom.drawXPos + j + offsetX + dungeonOffsetX, prevRoom.drawYPos + i + offsetY + dungeonOffsetY, 0), grassTile);
                        ChunkManager.SetTile(new Vector3Int(prevRoom.drawXPos + j + offsetX + dungeonOffsetX, prevRoom.drawYPos + i + offsetY + dungeonOffsetY, 0), grassTile);
                        break;
                    case 'X':
                        //groundTilemap.SetTile(new Vector3Int(prevRoom.drawXPos + j + offsetX + dungeonOffsetX, prevRoom.drawYPos + i + offsetY + dungeonOffsetY, 0), null);
                        ChunkManager.SetTile(new Vector3Int(prevRoom.drawXPos + j + offsetX + dungeonOffsetX, prevRoom.drawYPos + i + offsetY + dungeonOffsetY, 0), null);
                        break;
                }
            }
        }
        //randomly modify rooms to make layout look more cavernous/natural
        if(opts.generationStyle == "cave")
        {
            //number of iterations
            int iterations = 3;
            for (int i = 1; i <= iterations; i++)
            {
                //iterate through border of room and randomly decide whether to spread or not
                for (int x = 0; x < roomInfo.width + i*2; x++) 
                {
                    for(int y = 0; y < roomInfo.height + i*2; y++)
                    {
                        //for every tile, check if tile has a null neightbor to check if it is on the border
                        //expensive but this can handle any shape of room
                        int tileX = prevRoom.drawXPos + offsetX + x + dungeonOffsetX - i;
                        int tileY = prevRoom.drawYPos + offsetY + y + dungeonOffsetY - i;
                        //skip null tiles
                        if (groundTilemap.GetTile(new Vector3Int(tileX, tileY, 0)) == null) continue;
                        if (hasSpecificNeighbor(groundTilemap, tileX, tileY, null))
                        {
                            //tile has null neighbor; tile is on the border
                            //decide whether to spread
                            if ((int)UnityEngine.Random.Range(0, 10) < 3) { spreadToNeighbors(groundTilemap, tileX, tileY, grassTile); }
                        }
                    }
                }
            }
        }
    }

    void spreadToNeighbors(Tilemap tilemap, int tileX, int tileY, Tile T)
    {
        //given an xPos, yPos, tilemap, and tile, change all adjacent tiles to match the given tile
        tilemap.SetTile(new Vector3Int(tileX + 1, tileY, 0), T);
        tilemap.SetTile(new Vector3Int(tileX - 1, tileY, 0), T);
        tilemap.SetTile(new Vector3Int(tileX, tileY + 1, 0), T);
        tilemap.SetTile(new Vector3Int(tileX, tileY - 1, 0), T);
    }

    bool hasSpecificNeighbor(Tilemap tilemap, int tileX, int tileY, Tile targetT)
    {
        if (tilemap.GetTile(new Vector3Int(tileX + 1, tileY, 0)) == targetT) return true;
        if (tilemap.GetTile(new Vector3Int(tileX - 1, tileY, 0)) == targetT) return true;
        if (tilemap.GetTile(new Vector3Int(tileX, tileY + 1, 0)) == targetT) return true;
        if (tilemap.GetTile(new Vector3Int(tileX, tileY - 1, 0)) == targetT) return true;
        return false;
    }

    void connectRooms(DungeonNode prevNode, DungeonNode newNode, string prevEdge)
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
        int roomLeftBounds = newNode.drawXPos;
        int roomRightBounds = newNode.drawXPos + newNode.roomInfo.width;
        int roomTopBounds = newNode.drawYPos + newNode.roomInfo.height;
        int roomBottomBounds = newNode.drawYPos;
        switch (prevEdge)
        {
            case "bottom":
                Hwidth = hallWaySpace;
                Hheight = prevNode.drawYPos - (newNode.drawYPos + newNode.roomInfo.height);
                x = prevNode.drawXPos + 1;
                y = newNode.drawYPos + newNode.roomInfo.height;
                drawHallway(Hwidth, Hheight, xStart, y, "vertical", roomLeftBounds, roomRightBounds, roomBottomBounds, roomTopBounds);
                break;
            case "top":
                Hwidth = hallWaySpace;
                Hheight = newNode.drawYPos - (prevNode.drawYPos + prevNode.roomInfo.height);
                x = newNode.drawXPos + 1;
                y = prevNode.drawYPos + prevNode.roomInfo.height;
                drawHallway(Hwidth, Hheight, xStart, y, "vertical", roomLeftBounds, roomRightBounds, roomBottomBounds, roomTopBounds);
                break;
            case "left":
                Hwidth = prevNode.drawXPos - (newNode.drawXPos + newNode.roomInfo.width);
                Hheight = hallWaySpace;
                x = newNode.drawXPos + newNode.roomInfo.width;
                y = prevNode.drawYPos + 1;
                drawHallway(Hwidth, Hheight, x, yStart, "horizontal", roomLeftBounds, roomRightBounds, roomBottomBounds, roomTopBounds);
                break;
            case "right":
                Hwidth = newNode.drawXPos - (prevNode.drawXPos + prevNode.roomInfo.width);
                Hheight = hallWaySpace;
                x = prevNode.drawXPos + prevNode.roomInfo.width;
                y = prevNode.drawYPos + 1;
                drawHallway(Hwidth, Hheight, x, yStart, "horizontal", roomLeftBounds, roomRightBounds, roomBottomBounds, roomTopBounds);
                break;
        }
        
        
    }

    void drawHallway(int width, int height, int startX, int startY, string type, int leftBounds, int rightBounds, int bottomBounds, int topBounds)
    {
        int varianceOffset = 0;
        int varianceIncrementer = 3;
        int shiftInterval = opts.hallwayVariance;
        List<Tuple<int, int>> hallwayTiles = new List<Tuple<int, int>>();
        List<Tuple<int, int>> hallwayEndPos = new List<Tuple<int, int>>();
        if(type == "vertical")
        {
 
            //vertical hallways; draw one row at a time
            for(int i = 0; i < height; i++) //y
            {
                //draw row

                //before row is drawn, decide whether to increment variance offset
                float rand = UnityEngine.Random.Range(0f, 1f); //0 - 1
                //check hallway bounds before moving offset
                if (rand < .333 && varianceIncrementer % shiftInterval == 0)
                {
                    if (startX + varianceOffset + width >= rightBounds) { }//dont
                    else varianceOffset += 1;
                }
                else if (rand < .666 && varianceIncrementer % shiftInterval == 0)
                {
                    if (startX + varianceOffset <= leftBounds) { }//dont
                    else varianceOffset -= 1;
                }
                else
                {
                    //dont change
                }
                varianceIncrementer++;

                for (int j = 0; j < width; j++) //x
                {
                    //groundTilemap.SetTile(new Vector3Int(j + startX + dungeonOffsetX + varianceOffset, i + startY + dungeonOffsetY, 0), grassTile);
                    ChunkManager.SetTile(new Vector3Int(j + startX + dungeonOffsetX + varianceOffset, i + startY + dungeonOffsetY, 0), grassTile);

                }

            }
        } else
        {
            //horizontal hallways
            for (int i = 0; i < width; i++) //x
            {
                //draw row

                //before row is drawn, decide whether to increment variance offset
                float rand = UnityEngine.Random.Range(0f, 1f); //0 - 1
                //check hallway bounds before moving offset
                if (rand < .333 && varianceIncrementer % shiftInterval == 0)
                {
                    if (startY + varianceOffset + height >= topBounds) { }//dont
                    else varianceOffset += 1;
                }
                else if (rand < .666 && varianceIncrementer % shiftInterval == 0)
                {
                    if (startY + varianceOffset <= bottomBounds) { }//dont
                    else varianceOffset -= 1;
                }
                else
                {
                    //dont change
                }
                varianceIncrementer++;

                for (int j = 0; j < height; j++) //y
                {
                    //groundTilemap.SetTile(new Vector3Int(i + startX + dungeonOffsetX, j + startY + dungeonOffsetY + varianceOffset, 0), grassTile);
                    ChunkManager.SetTile(new Vector3Int(i + startX + dungeonOffsetX, j + startY + dungeonOffsetY + varianceOffset, 0), grassTile);

                }

            }
        }
     

        //check if hallway actually reached target destination (neighboring tiles arent background), otherwise draw another hallway
        //for each final tile
        //for (int i = 0; i < hallwayEndPos.Count; i++)
        //{
        //    //check if tile is neighbored by a background tile
        //    Tile tile1 = (Tile)dungeonTilemap.GetTile(new Vector3Int(hallwayEndPos[i].Item1 + 1, hallwayEndPos[i].Item2, 0));
        //    Tile tile2 = (Tile)dungeonTilemap.GetTile(new Vector3Int(hallwayEndPos[i].Item1 - 1, hallwayEndPos[i].Item2, 0));
        //    Tile tile3 = (Tile)dungeonTilemap.GetTile(new Vector3Int(hallwayEndPos[i].Item1, hallwayEndPos[i].Item2 + 1, 0));
        //    Tile tile4 = (Tile)dungeonTilemap.GetTile(new Vector3Int(hallwayEndPos[i].Item1, hallwayEndPos[i].Item2 - 1, 0));
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

    // DEPRECATED
    /*
    public Tilemap getDungeonTilemap() { return dungeonTilemap; }

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
    */

}
