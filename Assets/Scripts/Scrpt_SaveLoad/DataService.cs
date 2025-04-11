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

  private static string saveName = null; // name of the current world's save file
  private static string worldName = null; // name of the current world

  private static SaveInfo SAVEINFO_NULL; // blank save info struct

  // returns name, date (in that order) on all saves
  public static List<SaveInfo> Fetch() {
    Directory.CreateDirectory(savePath); // automatically create Saves\ directory if it doesn't exist


    List<SaveInfo> info = new List<SaveInfo>();
    
    string[] paths = Directory.GetFiles(savePath);

    
    Debug.Log("DataService: Fetched:");
    if (paths.Length == 0)
      Debug.Log("DataService: (none)"); // nothing in the Saves\ directory
    
    foreach (string path in paths) {
      SaveInfo saveInfo = new SaveInfo();
      
      saveInfo.path = path;
      saveInfo.name = path.Substring(savePathLen); // TODO: save world name separately
      saveInfo.lastModified = Directory.GetLastWriteTime(path);

      Debug.Log("DataService: saveInfo.path: " + saveInfo.path);
      Debug.Log("DataService: saveInfo.name: " + saveInfo.name);
      Debug.Log("DataService: saveInfo.lastModified: " + saveInfo.lastModified);

      info.Add(saveInfo);
    }

    // TODO: sort by time

    return info;
  }

  // converts and returns the lowercase character of c
  private static char toLower(char c) {
    if ('A' <= c && c <= 'Z')
      return (char)('a' - 'A' + c);
    
    return c; // not uppercase letter; just return c
  }

  // given a world name, converts all characters to lowercase and replaces spaces with underscores
  private static string toSaveName(string worldName) {
    string saveName = "";
    
    // converting...
    int len = worldName.Length;
    for (int i = 0; i < len; i++) {
      if (worldName[i] == ' ')
        saveName += '_';
      else
        saveName += toLower(worldName[i]);
    }

    return saveName + ".save";
  }

  // creates a new save file with the given name, returns new SaveInfo struct representing the new save file
  // NOTE: will overwrite worlds with the same name
  public static SaveInfo NewSave(string worldName) {
    Directory.CreateDirectory(savePath);

    if (worldName == null || worldName.Length == 0) {
      DataService.saveName = "unnamed.save";
      DataService.worldName = "Unnamed World";
    }
    else {
      DataService.saveName = toSaveName(worldName);
      DataService.worldName = worldName;
    }

    bool success = SaveCurr();
    if (!success)
      return SAVEINFO_NULL; // returns blank save info

    SaveInfo saveInfo = new SaveInfo();
    saveInfo.path = savePath + saveName;
    saveInfo.name = worldName;
    saveInfo.lastModified = Directory.GetLastWriteTime(saveInfo.path);

    Debug.Log("DataService: New world saved!");

    return saveInfo;
  }

  // saves current scene into savePath + name, returns success through a boolean
  // NOTE: as of 2025-04-04, TestScene is what is saved
  public static bool SaveCurr() {
    Directory.CreateDirectory(savePath);

    // checks if a world is currently loaded
    if (worldName == null) {
      Debug.LogError("DataService: World not loaded!");
      return false;
    }

    // saving world grid/tilemap
    string gridData = GridSaveLoad.SaveGrid();
    if (gridData == null) {
      Debug.LogError("DataService: SaveGrid() failed!");
      return false;
    }

    // writing to file
    string namedSavePath = savePath + saveName;
    Debug.Log("DataService: Saving to \"" + namedSavePath + "\"...");
    File.WriteAllText(namedSavePath, gridData);
    
    Debug.Log("DataService: World saved!");

    return true;
  }

  // TOFIX: loads game at savePath, returns success through a boolean
  public static bool Load(SaveInfo saveInfo) {
    String path = saveInfo.path;

    if (!File.Exists(path)) {
      Debug.LogError("DataService: \"" + savePath + "\" does not exist!"); 
      return false;
    }

    // reading from file
    Debug.Log("DataService: Loading from \"" + path + "\"...");
    List<string> gridData = File.ReadLines(path).ToList();

    // loading world
    GridSaveLoad.LoadGrid(gridData);

    worldName = saveInfo.name; // currently loaded world name
    Debug.Log("DataService: World loaded!");

    return true;
  }

  // TODO: deletes game at given path, returns success through a boolean
  public static bool Delete(string path) {
    return false;
  }
  
  // retuns the save directory, which ends with a \
  public static string GetSavePath() { return savePath; }

}

