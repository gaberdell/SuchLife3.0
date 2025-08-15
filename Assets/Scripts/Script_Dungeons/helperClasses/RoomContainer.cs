using UnityEngine;
using System.Collections.Generic;
using System;

[System.Serializable]
public class RoomContainer 
{
    public RoomInfo[] rooms;
    //stores indexes of rooms by type so we can get random index
    private Dictionary<string, List<int>> roomTypeIndexes = new Dictionary<string, List<int>>(); 

    public void createIndexes()
    {
        //loop through rooms
        for (int i = 0; i < rooms.Length; i++)
        {
            RoomInfo room = rooms[i];
            //for each room type, check if already exists in dictionary.
            if (roomTypeIndexes.ContainsKey(room.type))
            {
                //if it does, then add current index to the entry of indexes
                roomTypeIndexes[room.type].Add(i);
            } else
            {
                //if it doesnt, then create the entry
                roomTypeIndexes.Add(room.type, new List<int>());
                roomTypeIndexes[room.type].Add(i);
            }
        }
    }

    public RoomInfo getRoom(string type)
    {
        //return a random room layout based on type (pick one out of the rooms of that type, unweighted)
        int maxIndexes = roomTypeIndexes[type].Count;
        int randomIndex = (int)UnityEngine.Random.Range(0, maxIndexes);
        int roomIndex = roomTypeIndexes[type][randomIndex];
        return rooms[roomIndex];

    }
}
