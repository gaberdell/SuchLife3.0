using JetBrains.Annotations;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UIElements.Experimental;

public class DataService : MonoBehaviour {

  // TODO: implement multiple saves
  private static string savePath = System.IO.Directory.GetCurrentDirectory() + "\\Saves\\";
  private static bool saving = false;
  private static bool loading = false;

  // used by fetch; simple packet that contains a save's path, name, and last modified date and time
  public struct SaveInfo {
    string path;
    string name;
    DateTime lastModified;
  }

  // TODO: returns name, date (in that order) on all saves
  public static List<SaveInfo> Fetch() {
    string[] paths = GetFiles(savePath);

    List<SaveInfo> info = new List<SaveInfo>();
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

  public static string GetSavePath() { return savePath; } // retuns the save directory, ended with a \
  
  // TOREMOVE: basic input handling for testing
  void Update() {

    // saving

    if (Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.X) && !saving) {
      Time.timeScale = 0f;
      saving = true;
      Save();
      Time.timeScale = 1f;
    }

    else if (saving && (Input.GetKeyUp(KeyCode.LeftControl) || Input.GetKeyUp(KeyCode.X))) {
      saving = false;
    }

    // loading

    if (Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.V) && !loading) {
      Time.timeScale = 0f;
      loading = true;
      Load();
      Time.timeScale = 1f;
    }

    else if (loading && (Input.GetKeyUp(KeyCode.LeftControl) || Input.GetKeyUp(KeyCode.V))) {
      loading = false;
    }

  }

}

/*
 * TODO:
 *  - implement functions
 *  - encrypt saves
*/