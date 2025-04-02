using Codice.CM.Client.Differences;
using JetBrains.Annotations;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Experimental.AI;
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

  private static string worldName = null; // name of the current world

  // returns name, date (in that order) on all saves
  public static List<SaveInfo> Fetch() {
    Directory.CreateDirectory(savePath); // automatically create Saves\ directory if it doesn't exist


    List<SaveInfo> info = new List<SaveInfo>();
    
    string[] paths = Directory.GetFiles(savePath);

    
    Debug.Log("Fetched:");
    if (paths.Length == 0)
      Debug.Log("(none)"); // nothing in the Saves\ directory
    
    foreach (string path in paths) {
      SaveInfo saveInfo = new SaveInfo();
      
      saveInfo.path = path;
      saveInfo.name = path.Substring(savePathLen); // TODO: save world name separately
      saveInfo.lastModified = Directory.GetLastWriteTime(path);

      Debug.Log("saveInfo.path: " + saveInfo.path);
      Debug.Log("saveInfo.name: " + saveInfo.name);
      Debug.Log("saveInfo.lastModified: " + saveInfo.lastModified);

      info.Add(saveInfo);
    }

    // TODO: sort by time

    return info;
  }

  // creates a new save file with the given name, returns new SaveInfo struct representing the new save file
  // NOTE: will overwrite worlds with the same name
  public static SaveInfo NewSave(string name) {
    Directory.CreateDirectory(savePath);

    worldName = name;
    SaveCurr();

    SaveInfo saveInfo = new SaveInfo();
    saveInfo.path = savePath + worldName;
    saveInfo.name = worldName;
    saveInfo.lastModified = Directory.GetLastWriteTime(saveInfo.path);

    return saveInfo;
  }

  // saves current game into savePath + name, returns success through a boolean
  public static bool SaveCurr() {
    Directory.CreateDirectory(savePath);

    // checks if a world is currently loaded
    if (worldName == null) {
      Debug.Log("ERROR: World not loaded!");
      return false;
    }

    // dungeon
    GameObject dungeonGrid = GameObject.Find("DungeonGrid");
    if (dungeonGrid == null) {
      Debug.Log("ERROR: Unable to find dungeon!");
      return false;
    }
    RenderDungeon dungeonRenderer = dungeonGrid.GetComponent<RenderDungeon>();
    string dungeonJSON = dungeonRenderer.Save();
    Debug.Log("dungeonJSON: " + dungeonJSON);

    // writing to file
    string namedSavePath = savePath + worldName;
    Debug.Log("Saving to \"" + namedSavePath + "\"...");
    File.WriteAllText(namedSavePath, dungeonJSON);
    
    Debug.Log("Saved!");

    return true;
  }

  // loads game at savePath, returns success through a boolean
  public static bool Load(SaveInfo saveInfo) {
    String path = saveInfo.path;

    if (!File.Exists(path)) {
      Debug.Log("ERROR: \"" + savePath + "\" does not exist!"); 
      return false;
    }

    // reading from file
    Debug.Log("Loading from \"" + path + "\"...");
    List<string> dungeonJsons = File.ReadLines(path).ToList();
    Debug.Log("dungeonJsons: " + dungeonJsons);

    // dungeon
    GameObject dungeonGrid = GameObject.Find("DungeonGrid");
    RenderDungeon dungeonRenderer = dungeonGrid.GetComponent<RenderDungeon>();
    dungeonRenderer.getDungeonTilemap().ClearAllTiles(); // clear current dungeon tilemap
    foreach (string json in dungeonJsons)
      dungeonRenderer.Load(json);

    worldName = saveInfo.name; // currently loaded world name
    Debug.Log("Loaded!");
    
    return true;
  }

  // TODO: deletes game at given path, returns success through a boolean
  public static bool Delete(string path) {
    return false;
  }
  
  // retuns the save directory, which ends with a \
  public static string GetSavePath() { return savePath; }

}

