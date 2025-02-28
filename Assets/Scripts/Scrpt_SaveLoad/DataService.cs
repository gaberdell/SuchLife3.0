using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UIElements.Experimental;

public class DataService : MonoBehaviour {

  // TODO: implement multiple saves
  private static string savePath = System.IO.Directory.GetCurrentDirectory() + "\\Saves\\save";
  private static bool saving = false;
  private static bool loading = false;

  // used by fetch; simple packet that contains a save's path, name, and last modified date and time
  public struct SaveInfo {
    string path;
    string name;
    DateTime lastModified;
  }

  // TODO: returns name, date (in that order) on all saves
  public List<SaveInfo> Fetch() {
    List<SaveInfo> info = new List<SaveInfo>();
    return info;
  }

  // TODO: saves current game into given path with given name, returns success through a boolean
  // public bool Save(string path, string name) {
  public bool Save() {

    // Dungeon
    GameObject dungeonGrid = GameObject.Find("DungeonGrid");
    if (dungeonGrid == null) {
      Debug.Log("Unable to find dungeon!");
      return false;
    }
    RenderDungeon dungeonRenderer = dungeonGrid.GetComponent<RenderDungeon>();
    DungeonGraph dungeonGraph = dungeonRenderer.nodeGraph;
    dungeonGraph.Save();
    string renderDungeonJSON = JsonUtility.ToJson(dungeonRenderer);
    string dungeonJSON = JsonUtility.ToJson(dungeonGraph);
    Debug.Log("Debug: dungeonSize: " + dungeonGraph.layout.Count);
    Debug.Log("Debug: dungeonJSON: " + dungeonJSON);

    // writing to file
    Debug.Log("Saving to \"" + savePath + "\"...");
    FileStream saveFile = new FileStream(savePath, FileMode.Create);
    BinaryWriter writer = new BinaryWriter(saveFile);
    writer.Write(renderDungeonJSON);
    writer.Write(dungeonJSON);

    // cleaning up
    writer.Close();
    saveFile.Close();
    
    Debug.Log("Saved!");

    return true;
  }

  // TODO: loads game at given path, returns success through a boolean
  public bool Load() {
    return true;
  }

  // TODO: deletes game at given path, returns success through a boolean
  public bool Delete(string path) {
    return true;
  }
  
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