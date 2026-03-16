using UnityEngine;
using System.Net.Sockets;
using System.Net;

//If player is using a network application load into it
//Uses a JSON file for the build in Saves called ServerSettings

struct ServerSettings {
    string pathOfLoadInto;
}

public class NetworkLoadInto : MonoBehaviour
{
    //UNITY_STANDALONE is opposite of UNITY_SERVER
    static string SETTINGS_POSITION_DIRECTORY = Application.persistentDataPath+"/Saves/ServerSettings.json";
#if UNITY_SERVER
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ServerSettings basicSaveInfo = JsonUtility.FromJson<ServerSettings>(SETTINGS_POSITION_DIRECTORY);

        DataService.Load(basicSaveInfo.pathOfLoadInto);
    }
#endif
}
