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

public struct ServerInfo {
    public string path;
    public string name;
    public string ip;
    public Guid uuid;
    public uint order;
    public uint saveVersion;
}

public struct  ServerClientsDataInfo {
    public string[] uuids;
    public string[] externalId;
}

public class DataService {

    public static bool IsLocalSave = true;
    public static bool IsMultiplayer = false;
    public static Guid localPlayerUUID = Guid.Empty;

    const string SAVES_FOLDER_NAME = "Saves";
    const string BASIC_SAVE_NAME = "Basic.json";
    const string WORLD_DATA_SAVE_NAME = "WorldData.save";
    const string ENTITY_SAVE_NAME = "Entity.save";

    const string SERVER_SAVES_FOLDER_NAME = "ServerSaves";

    private static string WORLD_SCENE_NAME = "MainGameplayScene";

    const uint currentSaveVersion = 0; //idk deal with 

    private static string savePath = Application.persistentDataPath + "/" + SAVES_FOLDER_NAME + "/";

    private static string serverSavePath = Application.persistentDataPath + "/" + SERVER_SAVES_FOLDER_NAME + "/";

    private static string saveName = null; // name of the current world's save file
    private static string worldName = null; // name of the current world
    private static string currentSavePath = null; // path of current world's save file

    public static string IpOfServer { get; private set; } = null;
    public static ushort PortOfServerWeAreHosting { get; set; } = 0;

    private static SaveInfo SAVEINFO_NULL; // blank save info struct


    public static int SaveInfoOrder(SaveInfo order, SaveInfo order2)
    {
        //in a perfect world imagine order.order <=> order2.order
        //however this is not a perfect world
        return order.order > order2.order ? 1 : (order.order < order2.order ? -1 : 0);
    }

    public static int ServerInfoOrder(ServerInfo order, ServerInfo order2) {
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

    public static List<ServerInfo> FetchServers() {
        Directory.CreateDirectory(serverSavePath); // automatically create Saves\ directory if it doesn't exist

        List<ServerInfo> allBasicSaveData = new List<ServerInfo>();
        string[] files = Directory.GetFiles(serverSavePath);

        foreach (string filePath in files) {
            string fileText = File.ReadAllText(filePath);

            ServerInfo basicSaveInfo = JsonUtility.FromJson<ServerInfo>(fileText);

            allBasicSaveData.Add(basicSaveInfo);
        }

        allBasicSaveData.Sort(ServerInfoOrder);

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

    public static ServerInfo NewServerSave(string newWorldName, uint order) {
        Directory.CreateDirectory(serverSavePath);

        if (newWorldName == null || newWorldName.Length == 0) {
            saveName = "unnamed";
            worldName = "Unnamed Server";
        }
        else {
            saveName = toFileFormat(newWorldName);
            worldName = newWorldName;
        }

        saveName = EnsureUniqueName(serverSavePath, saveName);


        string finishedPath = serverSavePath+saveName+".json";

        ServerInfo saveInfo = new ServerInfo();
        saveInfo.path = finishedPath;
        saveInfo.name = worldName;
        saveInfo.ip = "127.0.0.1";
        saveInfo.uuid = Guid.NewGuid();
        saveInfo.order = order;
        saveInfo.saveVersion = currentSaveVersion;

        File.WriteAllText(finishedPath, JsonUtility.ToJson(saveInfo));


        Debug.Log("DataService: New server saved!");

        return saveInfo;
    }

    public static ServerInfo CloneServerSaveData(ServerInfo saveInfo) {
        string fileName = saveInfo.path.Substring(saveInfo.path.LastIndexOf('/') + 1);

        string newName = EnsureUniqueName(serverSavePath, fileName);

        string finishedPath = serverSavePath + newName;

        File.Copy(saveInfo.path, finishedPath, true);

        ServerInfo newCloneServer = JsonUtility.FromJson<ServerInfo>(finishedPath);
        newCloneServer.order++;
        newCloneServer.path = fileName;

        File.WriteAllText(finishedPath, JsonUtility.ToJson(newCloneServer));

        return newCloneServer;
    }
    public static void DeleteServerSaveData(ServerInfo saveInfo) {
        Directory.Delete(saveInfo.path);
    }


    public static void ResaveBasicServerSaveInfo(ServerInfo saveInfoToResave) {
        File.WriteAllText(saveInfoToResave.path, JsonUtility.ToJson(saveInfoToResave));
    }


    public static byte[] LoadEntitySaveData() {
        return File.ReadAllBytes(currentSavePath + "/" + ENTITY_SAVE_NAME);
    }

    public static void SaveEntitySaveData(byte[] saveData) {
        File.WriteAllBytes(currentSavePath + "/" + ENTITY_SAVE_NAME, saveData);
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

    public static AsyncOperation LoadAsync(string path, bool isLocalSave = false) {
        IsLocalSave = isLocalSave;

        IsMultiplayer = false;

        //If Data is gotten from server no point in trying to grab local save
        if (IsLocalSave) {
            if (!Directory.Exists(path)) {
                Debug.LogError("DataService: \"" + savePath + "\" does not exist!");
                return null;
            }

            currentSavePath = path;

            SaveInfo saveInfo = JsonUtility.FromJson<SaveInfo>(File.ReadAllText(path));

            worldName = saveInfo.name; // currently loaded world name
            Debug.Log("DataService: World loaded!");
        }

        return SceneManager.LoadSceneAsync(WORLD_SCENE_NAME);
    }

    public static bool localSaveHandler(string path, bool isLocalSave = true) {
        if (isLocalSave) {
            if (!Directory.Exists(path)) {
                Debug.LogError("DataService local save string path: \"" + path + "\" does not exist!");
                return false;
            }

            currentSavePath = path;

            SaveInfo saveInfo = JsonUtility.FromJson<SaveInfo>(File.ReadAllText(path + "/" + BASIC_SAVE_NAME));

            worldName = saveInfo.name; // currently loaded world name
            Debug.Log("DataService: World loaded!");
        }
        else {
            if (!File.Exists(path)) {
                Debug.LogError("DataService server info string path: \"" + path + "\" does not exist!");
                return false;
            }

            ServerInfo serverInfo = JsonUtility.FromJson<ServerInfo>(File.ReadAllText(path));

            IpOfServer = serverInfo.ip;
            localPlayerUUID = serverInfo.uuid;
        }

        return true;
    }

    //Loads file through pathname
    public static bool Load(string path, bool isLocalSave = true) {
        IsLocalSave = isLocalSave;

        IsMultiplayer = false;

        if (!localSaveHandler(path, isLocalSave)) {
            return false;
        }


        SceneManager.LoadScene(WORLD_SCENE_NAME);

        return true;
    }

    // TOFIX: loads game at savePath, returns success through a boolean
    public static bool Load(SaveInfo saveInfo, bool isLocalSave = true) {
        if (isLocalSave) {
            string path = saveInfo.path;

            if (!Directory.Exists(path)) {
                Debug.LogError("DataService: \"" + path + "\" does not exist!"); 
                return false;
            }
        }

        return Load(saveInfo.path, isLocalSave);
    }

    public static bool LoadServer(string path, bool isLocalSave = true) {
        IsLocalSave = isLocalSave;
        IsMultiplayer = true;

        if (!localSaveHandler(path, isLocalSave)) {
            return false;
        }

        SceneManager.LoadScene(WORLD_SCENE_NAME);

        return true;

        //if local save grab local save path. Else grab ServerInfo
    }
  
    // retuns the save directory, which ends with a \
    public static string GetSavePath() { return savePath; }

}

