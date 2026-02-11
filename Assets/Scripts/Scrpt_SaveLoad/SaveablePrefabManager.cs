using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

//https://stackoverflow.com/questions/4854207/get-a-specific-bit-from-byte
//Gotten from this with a lil modification


//https://stackoverflow.com/questions/10966331/two-way-bidirectional-dictionary-in-c
//Got the flip Dictionary part from this Hasan Baidoun

public class SaveablePrefabManager : MonoBehaviour
{
    static private SaveablePrefabManager instance;

    static private string RESOURCE_LOCATION = "SaveablePrefabs/";

    static private string PLAYER_PREFAB_NAME = "Player";
    static private string SCROMBOLO_BOMBOLO_NAME = "Bomb";
    static private string ITEM_NAME = "Item";

    static MonoBehaviour PLAYER_PREFAB;
    static MonoBehaviour SCROMBOLO_BOMBOLO_PREFAB;

    //Maybe make this into a two way dictionary type?
    static Dictionary<byte[], MonoBehaviour> ByteToPrefabKey;
    static Dictionary<MonoBehaviour, byte[]> PrefabToByteKey;

    static Dictionary<string, MonoBehaviour> StringToPrefabKey;

    static uint MAX_BYTES_FOR_ENTITIES = 1024;

    uint numberOfActiveEntities = 0;
    static byte[] currentlyActiveEntities;

    static bool retrieveIsActiveEntity(uint location)
    {
        byte exactByte = currentlyActiveEntities[location / 8];

        byte byteLocation = (byte)(location % 8);

        byte oneByte = 1;

        /*Debug.Log("Exact Byte : " + Convert.ToString(exactByte,2));
        Debug.Log("ByteLocation : " + Convert.ToString(byteLocation, 2));
        Debug.Log("Post-Shift : " + Convert.ToString(oneByte << byteLocation, 2));
        Debug.Log("After And : " + Convert.ToString(exactByte & (oneByte << byteLocation), 2));*/
        //Bit shift black magic
        return (exactByte & (oneByte << byteLocation)) != 0;
    }

    static void removeLocation(uint location)
    {
        byte byteComplement = (byte) ~(1 << ((byte)location % 8));

        currentlyActiveEntities[location / 8] &= byteComplement;
    }

    static void addLocation(uint location)
    {
        byte byteLocation = (byte) (1 << ((byte)location % 8));

        currentlyActiveEntities[location / 8] |= byteLocation;
    }

    static uint getUnoccupiedId()
    {
        uint unoccupiedId = 0;
        while (retrieveIsActiveEntity(unoccupiedId))
        {
            unoccupiedId++;
            if (unoccupiedId >= 8*currentlyActiveEntities.Length)
            {
                Array.Resize<byte>(ref currentlyActiveEntities, currentlyActiveEntities.Length + 1);
            }
        }

        return unoccupiedId;
    }

    static public void DeletePrefab(MonoBehaviour prefab)
    {
        PrefabSaveInfo entity = prefab.GetComponent<PrefabSaveInfo>();

        if (entity != null)
        {
            removeLocation(entity.EntityId);

        }
        Destroy(prefab);
    }

    static public MonoBehaviour CreatePrefab(MonoBehaviour prefab, Vector3 position, Quaternion rotation)
    {
        //Maybe use object pooling
        MonoBehaviour newObject = Instantiate(prefab, position, rotation);

        PrefabSaveInfo entity = newObject.gameObject.AddComponent<PrefabSaveInfo>();
        entity.PrefabId = PrefabToByteKey[prefab];
        entity.EntityId = getUnoccupiedId();


        return newObject;
    }

    static public MonoBehaviour CreatePrefab(string prefabName)
    {
        return CreatePrefab(StringToPrefabKey[prefabName], Vector3.zero, Quaternion.identity);
    }

    public static MonoBehaviour CreatePrefab(string prefabName, Vector3 position, Quaternion rotation)
    {
        return CreatePrefab(StringToPrefabKey[prefabName], position, rotation);
    }

    public static MonoBehaviour CreatePrefab(byte[] prefabId, Vector3 position, Quaternion rotation)
    {
        return CreatePrefab(ByteToPrefabKey[prefabId], position, rotation);
    }


    void OnSuccessfulStart()
    {
        currentlyActiveEntities = new byte[2] { 1,1 };
        Debug.Log(Convert.ToString(currentlyActiveEntities[0],2));

        for (uint i = 0; i < 9; i++) {
            if (!retrieveIsActiveEntity(i))
            {
                addLocation(i);
                break;
            }
        }
        Debug.Log(currentlyActiveEntities[0]);

        PLAYER_PREFAB = Resources.Load<MonoBehaviour>(RESOURCE_LOCATION + PLAYER_PREFAB_NAME);
        SCROMBOLO_BOMBOLO_PREFAB = Resources.Load<MonoBehaviour>(RESOURCE_LOCATION + SCROMBOLO_BOMBOLO_NAME);

        ByteToPrefabKey = new Dictionary<byte[], MonoBehaviour>();
        ByteToPrefabKey.Add(new byte[1] { 1 }, PLAYER_PREFAB);
        ByteToPrefabKey.Add(new byte[1] { 2 }, SCROMBOLO_BOMBOLO_PREFAB);

        PrefabToByteKey = ByteToPrefabKey.ToDictionary((i) => i.Value, (i) => i.Key);

        StringToPrefabKey = new Dictionary<string, MonoBehaviour>();
        StringToPrefabKey.Add(PLAYER_PREFAB_NAME, PLAYER_PREFAB);
        StringToPrefabKey.Add(SCROMBOLO_BOMBOLO_NAME, SCROMBOLO_BOMBOLO_PREFAB);
    }
    void Start()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            OnSuccessfulStart();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
