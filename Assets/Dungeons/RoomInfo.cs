
using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class RoomInfo { 

    public string type;
    public List<string> tileLayout;
    public int width;
    public int height;

    RoomInfo()
    {
        //default values, these should never be seen
        type = "default";
        tileLayout = new List<string>();
        width = 0;
        height = 0;
    }


}
