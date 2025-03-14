# DungeonOptions.cs
> Found in DungeonGraph.cs & RenderDungeon.cs
> (Will) Take in arguments for all of its parameters
> Does not inherit Monobehavior

## Description
### Use
Storage class to hold values for options concerning a dungeon's generation
Eventually dungeon types will have their own static option values that give them unique generation qualities
### How it works
Parameters are:
- int maxNodes: max amount of 'rooms'
- int minNodes: min amount of 'rooms' 
- int maxEdges: max edges(connections) per node 
- maxDepth: tree will go this deep before creating branches (rename later)
#### Future
Not implemented but planned parameters
- int maxHallwayLength
- int minHallwayLength
- some kind of bias for room types?
- custom generation traits; e.g. mazelike, blobby, horizontally inclined, vertically inclined