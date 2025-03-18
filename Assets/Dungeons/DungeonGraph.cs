
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;




public class DungeonGraph
{
    public List<DungeonNode> layout = new List<DungeonNode>(); //finalized layout
    private List<DungeonNode> availableNodes = new List<DungeonNode>(); //nodes that can be added to the graph
    private List<DungeonNode> nodesAwaitingEdges = new List<DungeonNode>(); //nodes that have an entrance edge but are elligible for further branches
    public DungeonGraph(DungeonOptions opts)
    {
        createLayout(opts);
    }

    //algorithm will start at the first node and branch outward
    //every node has four possible locations for an edge: top, bottom, left, right
    //a node can only have one edge in each location
    private void createLayout(DungeonOptions opts)
    {
        //generate amount of nodes between maxnodes and minnodes
        int totalNodes = (int) UnityEngine.Random.Range(opts.minNodes, opts.maxNodes);
        //fill graph with blank nodes
        for (int i = 0; i < totalNodes; i++)
        {
            DungeonNode newNode = new DungeonNode();
            availableNodes.Add(newNode);
        }
        availableNodes[0].type = "Start";
        //start from 0th node and branch until maxDepth is hit
        //node will generate 0-4 'away' edges (1-4 for start)
        //creating an edge will also create the node it goes to

        //create initial branch to depth minDepth
        //bool branchIncomplete = true;
        int tempDepth = 0;

        //create first node
        DungeonNode firstNode = availableNodes[0];
        firstNode.depth = tempDepth;
        //remove from available nodes
        availableNodes.RemoveAt(0);
        nodesAwaitingEdges.Add(firstNode);
        //create first edge 
        DungeonNode nodeCreated = createNewEdge(firstNode, opts);
        //add first node to nodes awaiting edges
        

        //finalize branch by creating more nodes until maxdepth is hit
        
        for (int i = 1; i < opts.maxDepth; i++)
        {
            nodeCreated = createNewEdge(nodeCreated, opts);
        }
        //printNodes(nodesAwaitingEdges);
        //now that main branch has been created, start adding branches to 'nodes awaiting edges' starting from index 0
        while( nodesAwaitingEdges.Count > 0  && availableNodes.Count > 0)
        {
            //depending on setting
            if(opts.branchType == "0th")
            {
                //branch off of the first node in the list; repeat until no more nodes are awaiting edges or max nodes hit
                DungeonNode newNode = createNewEdge(nodesAwaitingEdges[0], opts);
            }
            else if(opts.branchType == "random")
            {
                //branch off of a random node in the list; repeat until no more nodes are awaiting edges or max nodes hit
                DungeonNode newNode = createNewEdge(nodesAwaitingEdges[UnityEngine.Random.Range(0, nodesAwaitingEdges.Count)], opts);
            }

        }
        //after maxnodes have been placed, move all nodes awaiting edges into the layout
        while(nodesAwaitingEdges.Count > 0)
        {
            DungeonNode tempNode = nodesAwaitingEdges[0];
            nodesAwaitingEdges.RemoveAt(0);
            layout.Add(tempNode);
        }
        //printNodes(layout);
    }

    private DungeonNode createNewEdge(DungeonNode originNode, DungeonOptions opts)
    {
        //pick a direction
        string dir = originNode.getOpenDirection();
        string oppdir = "nowhere";
        int newX = 0;
        int newY = 0;
        //place edge based on direction
        switch (dir)
        {
            case "top":
                newX = originNode.xPos; newY = originNode.yPos - 1;
                oppdir = "bottom";
                break;
            case "bottom":
                newX = originNode.xPos; newY = originNode.yPos + 1;
                oppdir = "top";
                break;
            case "left":
                newX = originNode.xPos - 1; newY = originNode.yPos;
                oppdir = "right";
                break;
            case "right":
                newX = originNode.xPos + 1; newY = originNode.yPos;
                oppdir = "left";
                break;
        }
        //check if node exists in direction
        DungeonNode neighborNode = nodeAtIndex(newX, newY);
        if (neighborNode != null )
        {
            //before adding edge, check if edge will cause the neighboring node to have more than maxnodes
            if(neighborNode.edges.Count >= opts.maxEdges)
            {
                //if so, then dont add this edge.
                return neighborNode;
            }
            //add an edge between the nodes but do not change available nodes
            Tuple<DungeonNode, string> newEdge = new Tuple<DungeonNode, string>(neighborNode, dir);
            originNode.edges.Add(newEdge);
            Tuple<DungeonNode, string> revEdge = new Tuple<DungeonNode, string>(originNode, oppdir);
            neighborNode.edges.Add(revEdge);
            //check if either node now has maxedges
            if(originNode.edges.Count == opts.maxEdges)
            {
                nodesAwaitingEdges.Remove(originNode);
                layout.Add(originNode);
            }
            if(neighborNode.edges.Count == opts.maxEdges)
            {
                nodesAwaitingEdges.Remove(neighborNode);
                layout.Add(neighborNode);
            }
            return neighborNode;
        } else
        {
            //grab a node from availablenodes, update it, and add an edge between the two
            //though first check if any nodes are available
            if (availableNodes.Count <= 0)
            {
                //no nodes are available; dont add node
                return null;
            }
            DungeonNode newNode = availableNodes[0];
            newNode.depth = originNode.depth + 1;
            newNode.xPos = newX; newNode.yPos = newY;
            Tuple<DungeonNode, string> newEdge = new Tuple<DungeonNode, string>(newNode, dir);
            originNode.edges.Add(newEdge);
            Tuple<DungeonNode, string> revEdge = new Tuple<DungeonNode, string>(originNode, oppdir);
            newNode.edges.Add(revEdge);

            //if origin node now has maxEdges edges, then move it to layout
            if (originNode.edges.Count == opts.maxEdges)
            {
                nodesAwaitingEdges.Remove(originNode);
                layout.Add(originNode);
            }

            //if new node has maxEdges edges, then move node to layout instead of nodesawaitingedges - this shouldnt happen unless maxedges is 1
            if (newNode.edges.Count == opts.maxEdges)
            {
                layout.Add(newNode);
                availableNodes.RemoveAt(0);
                return layout[layout.Count - 1];
            } else
            {
                nodesAwaitingEdges.Add(newNode);
                availableNodes.RemoveAt(0);
                return nodesAwaitingEdges[nodesAwaitingEdges.Count - 1];
            }

        }
    }
    private DungeonNode nodeAtIndex(int inputX, int inputY)
    {
        for(int i=0; i < nodesAwaitingEdges.Count; i++)
        {
            DungeonNode tempNode = nodesAwaitingEdges[i];
            if(tempNode.xPos == inputX && tempNode.yPos == inputY)
            {
                return tempNode;
            }
        }
        return null;
    }

    private void printNodes(List<DungeonNode> toPrint)
    {
        for(int i=0;i<toPrint.Count;i++)
        {
            Debug.Log(i);
            Debug.Log("edges of node");
            for (int j = 0; j < toPrint[i].edges.Count; j++)
            {
                
                Debug.Log(toPrint[i].edges[j]);
            }
            
        }
    }
}
