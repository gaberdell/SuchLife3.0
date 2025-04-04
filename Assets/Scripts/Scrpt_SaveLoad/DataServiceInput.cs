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
  private KeyCode inputDetector() {
    if (Input.GetKey(LC)) {
           if (Input.GetKey(F)) return F;
      else if (Input.GetKey(X)) return X;
      else if (Input.GetKey(V)) return V;
    }

    return KeyCode.None; // no key has been pressed
  }

  // returns whether a valid key press has been released
  private bool keyReleased() {
    return Input.GetKeyUp(LC) || Input.GetKeyUp(F) || Input.GetKeyUp(X) || Input.GetKeyUp(V);
  }

  // testing DataService based on key input
  private void testDataService(KeyCode keyPress) {
    Time.timeScale = 0f; // pausing game
    string savePath = DataService.GetSavePath() + "save";

    // fetch save files
    if (keyPress == F) DataService.Fetch();

    // save game state
    else if (keyPress == X) DataService.SaveCurr();

    // load game 
    else if (keyPress == V) {
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

    KeyCode keyPress = inputDetector();
    if (!blocking && keyPress != KeyCode.None) {
      blocking = true;
      testDataService(keyPress);
    }

    else if (blocking && keyReleased()) { blocking = false; }

  }
}
