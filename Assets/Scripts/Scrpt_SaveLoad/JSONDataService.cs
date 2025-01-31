using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class JSONDataService {

  // used by fetch; simple packet that contains a save's path, name, and last modified date and time
  public struct SaveInfo {
    string path;
    string name;
    DateTime lastModified;
  }

  // TODO: deletes game at given path, returns success through a boolean
  public bool Delete(string path) {
    return true;
  }

  // TODO: returns name, date (in that order) on all saves
  public List<SaveInfo> Fetch() {
    List<SaveInfo> info = new List<SaveInfo>();
    return info;
  }

  // TODO: loads game at given path, returns success through a boolean
  public bool Load(string path) {
    return true;
  }

  // TODO: saves current game into given path with given name, returns success through a boolean
  public bool Save(string path, string name) {
    return true;
  }
}

/*
 * TODO:
 *  - implement functions
 *  - encrypt saves
*/