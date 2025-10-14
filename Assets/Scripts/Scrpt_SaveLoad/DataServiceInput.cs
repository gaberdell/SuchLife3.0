using NUnit.Framework;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

// README: this is a component exclusively for detecting key inputs to test DataService
public class DataServiceInput : MonoBehaviour
{

  private bool blocking = false; // a boolean to block further DataService calls when one is in process

  // key shortcuts
  private KeyCode LC = KeyCode.LeftControl;
  private KeyCode F = KeyCode.F; // fetch
  private KeyCode X = KeyCode.X; // save
  private KeyCode V = KeyCode.V; // load

  // Start is called once before the first execution of Update after the MonoBehaviour is created
  void Start() { }

  // for valid key presses has been entered, returns KeyCode X from CTRL+X, KeyCode.None for invalid key presses
  private bool inputDetector() {
    return Input.GetKey(LC) && (Input.GetKeyDown(F) || Input.GetKeyDown(X) || Input.GetKeyDown(V));
  }

  // returns whether a valid key press has been released
  private bool keyReleased() {
    return Input.GetKeyUp(LC) || Input.GetKeyUp(F) || Input.GetKeyUp(X) || Input.GetKeyUp(V);
  }

  // testing DataService based on key input
  private void testDataService() {
        Debug.Log("Hello!!");
    Time.timeScale = 0f; // pausing game
    string savePath = DataService.GetSavePath() + "save";

    // fetch save files
    if (Input.GetKeyDown(F)) DataService.Fetch();

    // save game state
    else if (Input.GetKeyDown(X)) DataService.SaveCurr();

    // load game 
    else if (Input.GetKeyDown(V)) {
      List<SaveInfo> list = DataService.Fetch();
      if (list.Count == 0) {
        Debug.LogError("DataServiceInput: No save files found!");
        return;
      }
      DataService.Load(list[0]);
    }
    
    Time.timeScale = 1f; // unpausing game
  }

  void Update() {

    if (inputDetector()) {
        testDataService();
    }
  }
}
