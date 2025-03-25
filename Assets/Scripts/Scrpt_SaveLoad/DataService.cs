using JetBrains.Annotations;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UIElements.Experimental;

// used by fetch; simple packet that contains a save's path, name, and last modified date and time
public struct SaveInfo {
  public string path;
  public string name;
  public DateTime lastModified;
}

public class DataService {

  private static string savePath = System.IO.Directory.GetCurrentDirectory() + "\\Saves\\";
  private static int savePathLen = savePath.Length;

  // TODO: automatically create Save/ directory
  public static void SaveDir() {
# 
  }

  // returns name, date (in that order) on all saves
  public static List<SaveInfo> Fetch() {
    List<SaveInfo> info = new List<SaveInfo>();
    
    string[] paths = Directory.GetFiles(savePath);
    foreach (string path in paths) {
      SaveInfo saveInfo = new SaveInfo();
      
      saveInfo.path = path;
      saveInfo.name = path.Substring(savePathLen);
      saveInfo.lastModified = Directory.GetLastWriteTime(path);

      Debug.Log("saveInfo.path: " + saveInfo.path);
      Debug.Log("saveInfo.name: " + saveInfo.name);
      Debug.Log("saveInfo.lastModified: " + saveInfo.lastModified);

      info.Add(saveInfo);
    }

    // TODO: sort by time

    return info;
  }

  // saves current game into savePath, returns success through a boolean (TODO: saves into given path)
  public static bool Save(string savePath) {

    // dungeon
    GameObject dungeonGrid = GameObject.Find("DungeonGrid");
    if (dungeonGrid == null) {
      Debug.Log("Unable to find dungeon!");
      return false;
    }
    RenderDungeon dungeonRenderer = dungeonGrid.GetComponent<RenderDungeon>();
    string dungeonJSON = dungeonRenderer.Save();
    Debug.Log("dungeonJSON: " + dungeonJSON);

    // writing to file
    Debug.Log("Saving to \"" + savePath + "\"...");
    File.WriteAllText(savePath, dungeonJSON);
    
    Debug.Log("Saved!");

    return true;
  }

  // loads game at savePath, returns success through a boolean
  public static bool Load(string savePath) {

    // reading from file
    Debug.Log("Loading from \"" + savePath + "\"...");
    List<string> dungeonJsons = File.ReadLines(savePath).ToList();
    Debug.Log("dungeonJsons: " + dungeonJsons);

    // dungeon
    GameObject dungeonGrid = GameObject.Find("DungeonGrid");
    RenderDungeon dungeonRenderer = dungeonGrid.GetComponent<RenderDungeon>();
    dungeonRenderer.getDungeonTilemap().ClearAllTiles(); // clear current dungeon tilemap
    foreach (string json in dungeonJsons)
      dungeonRenderer.Load(json);

    Debug.Log("Loaded!");
    
    return true;
  }

  // TODO: deletes game at given path, returns success through a boolean
  public static bool Delete(string path) {
    return true;
  }

  public static string GetSavePath() { return savePath; } // retuns the save directory, which ends with a \

}