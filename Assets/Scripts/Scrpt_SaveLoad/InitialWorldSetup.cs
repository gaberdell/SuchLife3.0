using System;
using System.Collections;
using System.IO;
using UnityEngine;

public class InitialWorldSetup : MonoBehaviour
{
    [SerializeField]
    GameObject localPlayerEffects;

    GameObject player;

    //TODO : Change this hackney solution lol
    static GameObject staticLocalPlayerEffects;

    private void Awake() {
        staticLocalPlayerEffects = localPlayerEffects;
    }

    //Note initial world set up needs to be called later in order to ensure all other things have been properly set up before its called
    void Start()
    {
        

        Debug.Log("IsLocalSave? : " + DataService.IsLocalSave);
        if (DataService.IsMultiplayer) {
            return;
        }

        byte[] entitySaveDataBytes = null;

        try {
            entitySaveDataBytes = DataService.LoadEntitySaveData();
        }
        catch (Exception e) {
            Debug.LogError("Error found with attempting to load entity data : " + e.Message);
        }



        StartCoroutine(LoadLocalPlayerThings());

        //Change to be if it actually cant find 
        if (entitySaveDataBytes == null || entitySaveDataBytes.Length == 0) {
            player = SaveablePrefabManager.CreatePrefab("Player", Vector3.zero, Quaternion.Euler(0,0,0));

        } else {
            player = SaveObjectsManager.LocalPlayerFromSaveData;
            Debug.Log("Player from save objects manager : " + player);
        }

#if UNITY_EDITOR
        if (entitySaveDataBytes != null) {
            Debug.Log(entitySaveDataBytes.Length);

            foreach (byte entitySaveData in entitySaveDataBytes) {
                Debug.Log(entitySaveData);
            }
        }
#endif
    }

    public void AddPlayerToGame(GameObject localPlayerPrefab) {
        player = localPlayerPrefab;
        StartCoroutine(LoadLocalPlayerThingsServer());
    }

    IEnumerator LoadLocalPlayerThingsServer() {
        AsyncInstantiateOperation asyncInstantiateOperation = InstantiateAsync(localPlayerEffects);
        while (!asyncInstantiateOperation.isDone) {
            yield return null;
        }
        EventManager.SetLocalGameObjectPlayerAddedToScene(player);
    }

    IEnumerator LoadLocalPlayerThings() {
        AsyncInstantiateOperation asyncInstantiateOperation = InstantiateAsync(localPlayerEffects);
        while (!asyncInstantiateOperation.isDone || player == null) {
            yield return null;
        }
        EventManager.SetLocalGameObjectPlayerAddedToScene(player);
    }
}
