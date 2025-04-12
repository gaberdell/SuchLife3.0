
using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class RoomInfo { 

    public string type;
    public List<string> tileLayout;
    public EntityInfo[] entities;
    public int width;
    public int height;

    RoomInfo()
    {
        //default values, these should never be seen
        type = "default";
        //entities = new EntityInfo[0];
        tileLayout = new List<string>();
        width = 0;
        height = 0;
    }


}
