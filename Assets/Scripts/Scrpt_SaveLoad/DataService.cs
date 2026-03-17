using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;


// used by fetch; simple packet that contains a save's path, name, and last modified date and time
public struct SaveInfo {
    public string path;
    public string name;
    public uint order;
    public uint saveVersion;
    public DateTime lastModified;
}
public class DataService {

    const string SAVES_FOLDER_NAME = "Saves";
    const string BASIC_SAVE_NAME = "Basic.json";
    const string WORLD_DATA_SAVE_NAME = "WorldData.save";
    const string ENTITY_SAVE_NAME = "Entity.save";

    private static string WORLD_SCENE_NAME = "MainGameplayScene";

    const uint currentSaveVersion = 0; //idk deal with 

    private static string savePath = Application.persistentDataPath + "/" + SAVES_FOLDER_NAME + "/";

    private static string saveName = null; // name of the current world's save file
    private static string worldName = null; // name of the current world
    private static string currentSavePath = null; // path of current world's save file

    private static SaveInfo SAVEINFO_NULL; // blank save info struct


    public static int SaveInfoOrder(SaveInfo order, SaveInfo order2)
    {
        //in a perfect world imagine order.order <=> order2.order
        //however this is not a perfect world
        return order.order > order2.order ? 1 : (order.order < order2.order ? -1 : 0);
    }

    // returns basic info : name, date (in that order) on all saves
    public static List<SaveInfo> Fetch() {
        Directory.CreateDirectory(savePath); // automatically create Saves\ directory if it doesn't exist

        List<SaveInfo> allBasicSaveData = new List<SaveInfo>();
        string[] files = Directory.GetDirectories(savePath);
    
        foreach (string filePath in files)
        {
            string fileText = File.ReadAllText(filePath + "/" + BASIC_SAVE_NAME);

            SaveInfo basicSaveInfo = JsonUtility.FromJson<SaveInfo>(fileText);

            allBasicSaveData.Add(basicSaveInfo);
        }

        allBasicSaveData.Sort(SaveInfoOrder);    

        return allBasicSaveData;
    }

    private static string toFileFormat(string name) {
        return name.ToLower().Replace(' ', '-');
    }

    // given a world name and directory ensures that the name is unique
    public static string EnsureUniqueName(string directory, string name)
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
    public static SaveInfo NewSave(string newWorldName, uint order) {
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

        saveName = EnsureUniqueName(savePath, saveName);

        saveFolder = saveName;

        //saveName += ".json";

        //bool success = SaveCurr();
        //if (!success)
        //    return SAVEINFO_NULL; // returns blank save info

        string almostFinishedPath = savePath + saveFolder + "/";
        Directory.CreateDirectory(almostFinishedPath);

        basicSave = almostFinishedPath + BASIC_SAVE_NAME;

        worldSave = almostFinishedPath + WORLD_DATA_SAVE_NAME;

        entitySave = almostFinishedPath + ENTITY_SAVE_NAME;

        SaveInfo saveInfo = new SaveInfo();
        saveInfo.path = savePath + saveFolder;
        saveInfo.name = worldName;
        saveInfo.order = order;
        saveInfo.saveVersion = currentSaveVersion;
        saveInfo.lastModified = Directory.GetLastWriteTime(saveInfo.path);
        
        File.WriteAllText(basicSave, JsonUtility.ToJson(saveInfo));
        File.WriteAllText(worldSave, "");
        File.WriteAllText(entitySave, "");


        Debug.Log("DataService: New world saved!");

        return saveInfo;
    }

    public static SaveInfo CloneSaveData(SaveInfo saveInfo) {
        string pathName = saveInfo.path.Substring(saveInfo.path.LastIndexOf('/')+1);

        pathName = EnsureUniqueName(savePath, pathName);

        string almostFinishedPath = savePath + pathName;

        Directory.CreateDirectory(almostFinishedPath);

        foreach (string directory in Directory.GetDirectories(saveInfo.path + "/", "*", SearchOption.AllDirectories)) {
            Directory.CreateDirectory(directory.Replace(saveInfo.path, almostFinishedPath));
            Debug.Log(directory.Replace(saveInfo.path, almostFinishedPath));
        }

        foreach (string filePath in Directory.GetFiles(saveInfo.path + "/", "*.*", SearchOption.AllDirectories)) {
            File.Copy(filePath, filePath.Replace(saveInfo.path, almostFinishedPath), true);
        }

        SaveInfo newSaveInfo = new SaveInfo();
        newSaveInfo = saveInfo;
        newSaveInfo.path = savePath + pathName;

        File.WriteAllText(almostFinishedPath + "/" + BASIC_SAVE_NAME, JsonUtility.ToJson(newSaveInfo));

        return newSaveInfo;
    }

    public static void DeleteSaveData(SaveInfo saveInfo) {
        Directory.Delete(saveInfo.path,true);
    }


    public static void ResaveBasicSaveInfo(SaveInfo saveInfoToResave) {
        string basicSave = saveInfoToResave.path + "/" + BASIC_SAVE_NAME;

        File.WriteAllText(basicSave, JsonUtility.ToJson(saveInfoToResave));
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

    public static AsyncOperation LoadAsync(string path) {
        if (!Directory.Exists(path)) {
            Debug.LogError("DataService: \"" + savePath + "\" does not exist!");
            return null;
        }

        currentSavePath = path;

        SaveInfo saveInfo = JsonUtility.FromJson<SaveInfo>(File.ReadAllText(path));

        worldName = saveInfo.name; // currently loaded world name
        Debug.Log("DataService: World loaded!");

        return SceneManager.LoadSceneAsync(WORLD_SCENE_NAME);
    }

    //Loads file through pathname
    public static bool Load(string path) {

        if (!Directory.Exists(path)) {
            Debug.LogError("DataService: \"" + savePath + "\" does not exist!");
            return false;
        }

        currentSavePath = path;

        SaveInfo saveInfo = JsonUtility.FromJson<SaveInfo>(File.ReadAllText(path));

        worldName = saveInfo.name; // currently loaded world name
        Debug.Log("DataService: World loaded!");

        SceneManager.LoadScene(WORLD_SCENE_NAME);

        return true;
    }

    // TOFIX: loads game at savePath, returns success through a boolean
    public static bool Load(SaveInfo saveInfo) {
        string path = saveInfo.path;

        currentSavePath = path;

        if (!Directory.Exists(path)) {
            Debug.LogError("DataService: \"" + savePath + "\" does not exist!"); 
            return false;
        }

        worldName = saveInfo.name; // currently loaded world name
        Debug.Log("DataService: World loaded!");

        SceneManager.LoadScene(WORLD_SCENE_NAME);

        return true;
    }
  
    // retuns the save directory, which ends with a \
    public static string GetSavePath() { return savePath; }

}

