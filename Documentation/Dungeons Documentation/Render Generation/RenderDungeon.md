# RenderDungeon.cs
> Initializes DungeonOptions and DungeonGraph a


## Description
### Use
Script used to generate the initial dungeon. Will probably be reworked into some kind of event call later. Just initializes a graph and renders the nodes and edges on the tilemap based on their x and y positions.
### How it works
- Doesnt do much at the moment, renders the graph by looping through every node and edge and drawing a tile at their positions


#### Future Plans
- Render graph one node at a time starting from the start node
- will draw the room, draw a neighboring node at a random distance away (set in opts), and draw a hallway connecting them (and add the new node to a queue)
- Rooms are read from static files and chosen randomly, with bias on room layouts not already existing
- Repeat for every edge remaining on the current node, and then move on to the next node in the queue
- If neighboring node already exists on the map then just draw a hallway between
- Randomize position of hallway on the sides

##### Future Future Plans
- Add more randomization to hallway generation
- Failsafe for impossible generations? maybe just slap the room on somewhere else
