using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;


// used by fetch; simple packet that contains a save's path, name, and last modified date and time
public struct SaveInfo {
    public string path;
    public string name;
    public DateTime lastModified;
}
public class DataService {

    const string SAVES_FOLDER_NAME = "Saves";
    const string BASIC_SAVE_NAME = "Basic.json";
    const string WORLD_DATA_SAVE_NAME = "WorldData.save";
    const string ENTITY_SAVE_NAME = "Entity.save";

    private static string savePath = Application.persistentDataPath + "/" + SAVES_FOLDER_NAME + "/";
    private static int savePathLen = savePath.Length;

    private static string saveName = null; // name of the current world's save file
    private static string worldName = null; // name of the current world

    private static SaveInfo SAVEINFO_NULL; // blank save info struct

    // returns basic info : name, date (in that order) on all saves
    public static List<SaveInfo> Fetch() {
        Directory.CreateDirectory(savePath); // automatically create Saves\ directory if it doesn't exist

        List<SaveInfo> allBasicSaveData = new List<SaveInfo>();
        string[] files = Directory.GetDirectories(savePath);
    
        foreach (string filePath in files)
        {
            string fileText = File.ReadAllText(filePath + "/" + BASIC_SAVE_NAME);

            SaveInfo basicSaveInfo = JsonUtility.FromJson<SaveInfo>(fileText);
        }

        return allBasicSaveData;
    }

  // converts and returns the lowercase character of c
    private static char toLower(char c) {
        if ('A' <= c && c <= 'Z')
            return (char)('a' - 'A' + c);

        return c; // not uppercase letter; just return c
    }

    private static string toFileFormat(string name) {
        string saveName = "";

        // converting...
        int len = worldName.Length;
        for (int i = 0; i < len; i++)
        {
            if (worldName[i] == ' ')
                saveName += '_';
            else
                saveName += toLower(worldName[i]);
        }

        return saveName;
    }

  // given a world name, converts all characters to lowercase and replaces spaces with underscores
    private static string toSaveName(string worldName) {
        return toFileFormat(worldName) + ".save";
    }

    // given a world name and directory ensures that the name is unique
    private static string ensureUniqueName(string directory, string name)
    {

        if (Directory.Exists(directory + name))
        {
            uint addI = 0;
            string newName = name + addI.ToString();
            while (Directory.Exists(directory + newName))
            {
                addI++;

                newName = name + addI.ToString();

                if (addI == uint.MaxValue)
                    name += "0";
            }

        return newName;
        }

        return name;
    }

  // creates a new save file with the given name, returns new SaveInfo struct representing the new save file
  // NOTE: will overwrite worlds with the same name
    public static SaveInfo NewSave(string newWorldName) {
        Directory.CreateDirectory(savePath);
        string saveFolder;
        string basicSave;
        string worldSave;
        string entitySave;

        if (newWorldName == null || newWorldName.Length == 0) {
            saveName = "unnamed";
            worldName = "Unnamed World";
        }
        else {
            saveName = toFileFormat(newWorldName);
            worldName = newWorldName;
        }

        saveName = ensureUniqueName(savePath, saveName);

        saveFolder = saveName;

        //saveName += ".json";

        //bool success = SaveCurr();
        //if (!success)
        //    return SAVEINFO_NULL; // returns blank save info

        string almostFinishedPath = savePath + saveFolder + "/";

        basicSave = almostFinishedPath + BASIC_SAVE_NAME;

        worldSave = almostFinishedPath + WORLD_DATA_SAVE_NAME;

        entitySave = almostFinishedPath + ENTITY_SAVE_NAME;

        SaveInfo saveInfo = new SaveInfo();
        saveInfo.path = basicSave;
        saveInfo.name = worldName;
        saveInfo.lastModified = Directory.GetLastWriteTime(saveInfo.path);
        
        File.WriteAllText(basicSave, JsonUtility.ToJson(saveInfo));
        File.WriteAllText(worldSave, "");
        File.WriteAllText(entitySave, "");


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
        // GridSaveLoad.LoadGrid(gridData); // TODO

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

