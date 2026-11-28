# RenderDungeon.cs
> Initializes DungeonOptions and DungeonGraph a


## Description
### Use
Script used to generate the initial dungeon. Will probably be reworked into some kind of event call later. Just initializes a graph and renders the nodes and edges on the tilemap based on their x and y positions.
### How it works
Renders graph one node (room) at a time starting from the start node (room player enters in).
Draws the room, another room, and then a hallway between, then adds a new room to queue.
Room layouts are biased to ones not already generated. 
Repeats until all nodes (rooms) and edges (hallways) are created.

#### Future Plans
- Randomize where hallways start and end.

##### Future Future Plans
- Add more randomization to hallway generation
- Failsafe for impossible generations? maybe just slap the room on somewhere else
