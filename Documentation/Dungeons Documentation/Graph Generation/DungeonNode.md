# DungeonNode.cs
> Found in DungeonGraph.cs
> Takes in DungeonOptions as an argument
> Does not inherit Monobehavior

## Description
### Use
Class representing a node on the dungeon graph (an individual 'room' in the dungeon). Has attributes storing its x & y pos, depth, room type, and a list of its edges. Initialized with default values of 0 to everything.
### How it works
- Pretty much a class to store data relating to each node/room
- has an enum with its 4 possible edge directions, so they can be randomly generated on branching
- has a helper function to create a new branch off of itself, which allows it to create an edge in a random direction that it doesnt already have an edge at.
