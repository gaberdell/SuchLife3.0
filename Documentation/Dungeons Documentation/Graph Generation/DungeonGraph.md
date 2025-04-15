# DungeonGraph.cs
> Found in RenderDungeon.cs
> Takes in DungeonOptions as an argument
> Does not inherit Monobehavior

## Description
### Use
Class representing a graph of nodes for a dungeon. When initialized, generates a graph based on the parameters defined. Graph is represented by a list of DungeonNodes, in a property called layout.
### How it works
- Contains three lists of DungeonNodes: one to represent the amount of nodes that will be added to the graph, one to represent nodes that can have more edges branch off of them, and one for 'finished' nodes.
- Starts by generating an amount of DungeonNodes between opts.minNodes and opts.maxNodes, and adds them all to a list
- Then, creates an initial branch of depth opts.maxDepth.
- After branch is created, these nodes are sequentially populated with adjacent nodes (starting from the oldest node) until it has opts.maxEdges edges or no more nodes are available to be placed.
- Nodes are added by 'branching' off of available nodes in a free direction, creating an edge with an accompanying node; edges will always lead to a node
- Generation can be pretty inconsistent with overall dungeon shape
- Does not take into account room types yet