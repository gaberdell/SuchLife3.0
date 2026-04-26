using UnityEngine;
using System.Net.Sockets;
using System.Net;
using System.Collections;
using UnityEngine.SceneManagement;
using System.IO;

//If player is using a network application load into it
//Uses a JSON file for the build in Saves called ServerSettings

struct ServerSettings {
    public string PathOfLoadInto;
    public ushort Port;
    public int MaxFPS; //Okay so apperently I think the game breaks if you let the fps stay at the default
//    public ushort
}

public class NetworkLoadInto : MonoBehaviour
{

#if UNITY_SERVER
    //UNITY_STANDALONE is opposite of UNITY_SERVER
    static string SETTINGS_POSITION_DIRECTORY;
    static ushort DEFAULT_SUCH_LIFE_PORT = 8376;
    static int DEFAULT_SUCH_LIFE_FPS = 120;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SETTINGS_POSITION_DIRECTORY = Application.persistentDataPath+"/Saves/ServerSettings.json";

        if (!File.Exists(SETTINGS_POSITION_DIRECTORY)) {
            ServerSettings newServerSettings = new ServerSettings();
            SaveInfo newSaveWorld = DataService.NewSave("Server World", 0);
            newServerSettings.PathOfLoadInto = newSaveWorld.path;
            newServerSettings.Port = DEFAULT_SUCH_LIFE_PORT;
            newServerSettings.MaxFPS = DEFAULT_SUCH_LIFE_FPS;

            DataService.PortOfServerWeAreHosting = newServerSettings.Port;
            File.WriteAllText(SETTINGS_POSITION_DIRECTORY, JsonUtility.ToJson(newServerSettings));
        }


        Debug.Log(SETTINGS_POSITION_DIRECTORY);

        ServerSettings basicSaveInfo = JsonUtility.FromJson<ServerSettings>(File.ReadAllText(SETTINGS_POSITION_DIRECTORY));
        //Ignore native refresh rate the game doesn't even use the camera anyway
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = basicSaveInfo.MaxFPS;

        DataService.PortOfServerWeAreHosting = basicSaveInfo.Port;

        if (!Directory.Exists(basicSaveInfo.PathOfLoadInto)) {
            Debug.Log("Save doesn't exsist attemtping to create new one");
            DataService.NewSave(basicSaveInfo.PathOfLoadInto.Substring(basicSaveInfo.PathOfLoadInto.LastIndexOf('/') + 1), 0);
        }

        DataService.LoadServer(basicSaveInfo.PathOfLoadInto, true);
    }
#endif
}
