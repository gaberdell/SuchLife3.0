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
}

public class NetworkLoadInto : MonoBehaviour
{
    //UNITY_STANDALONE is opposite of UNITY_SERVER
    static string SETTINGS_POSITION_DIRECTORY = Application.persistentDataPath+"/Saves/ServerSettings.json";
    static ushort DEFAULT_SUCH_LIFE_PORT = 8376;
//#if UNITY_SERVER
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (!File.Exists(SETTINGS_POSITION_DIRECTORY)) {
            ServerSettings newServerSettings = new ServerSettings();
            SaveInfo newSaveWorld = DataService.NewSave("Server World", 0);
            newServerSettings.PathOfLoadInto = newSaveWorld.path;
            newServerSettings.Port = DEFAULT_SUCH_LIFE_PORT;

            File.WriteAllText(SETTINGS_POSITION_DIRECTORY, JsonUtility.ToJson(newServerSettings));
        }

        ServerSettings basicSaveInfo = JsonUtility.FromJson<ServerSettings>(SETTINGS_POSITION_DIRECTORY);

        StartCoroutine(LoadIntoServerScene(basicSaveInfo));
    }
//#endif

    IEnumerator LoadIntoServerScene(ServerSettings serverSettings) {
        AsyncOperation asyncLoad = DataService.LoadAsync(serverSettings.PathOfLoadInto);

        if (asyncLoad == null) {
            Debug.LogError("Probably a bad path type can't load into it");
        }
        else {
            while (!asyncLoad.isDone) {
                yield return null;
            }

            ServerNetworkManager.StartServer(serverSettings.Port);
            Destroy(FindFirstObjectByType<ClientNetworkManager>());
        }

    }
}
