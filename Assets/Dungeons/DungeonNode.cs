using System;
using UnityEngine;
using System.Collections.Generic;


public class DungeonNode 
{
    //node has 4 potential edges: top, bottom, left, right
    [SerializeField] public enum EdgeDirection{top, bottom, left, right};
    [SerializeField] public List<Tuple<DungeonNode, string>> edges; //other nodes that this node is connected to 
    [SerializeField] public int xPos;
    [SerializeField] public int yPos;
    [SerializeField] public int depth;
    [SerializeField] public string type;
    //vals used in drawing nodes
    [SerializeField] public List<List<string>> layout;
    [SerializeField] public int width;
    [SerializeField] public int height;
    [SerializeField] public int drawXPos;
    [SerializeField] public int drawYPos;
    // ^ Jason was here

    public DungeonNode()
    {
        //default vals
        depth = 0;
        xPos = 0;
        yPos = 0;
        type = "Default";
        edges = new List<Tuple<DungeonNode, string>>();
        layout = new List<List<string>>();
        width = 10;
        height = 10;
        drawXPos = 0;
        drawYPos = 0;
}

    public string getOpenDirection()
    {
        List<string> Available = new List<string> { "top", "bottom", "left", "right"};
        //loop through edges and mark whats not available
        for(int i = 0; i < edges.Count; i++)
        {
            string tempEdgeDir = edges[i].Item2;
            if (Available.Contains(tempEdgeDir))
            {
                Available.Remove(tempEdgeDir);
            }
            
        }
        //get random edge from available directions
        string dir = Available[(int)UnityEngine.Random.Range(0,  Available.Count)];
        return dir;
    }


}
