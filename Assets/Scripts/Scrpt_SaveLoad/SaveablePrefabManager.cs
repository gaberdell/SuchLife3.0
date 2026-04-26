using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

//https://stackoverflow.com/questions/4854207/get-a-specific-bit-from-byte
//Gotten from this with a lil modification


//https://stackoverflow.com/questions/10966331/two-way-bidirectional-dictionary-in-c
//Got the flip Dictionary part from this Hasan Baidoun

public class SaveablePrefabManager : MonoBehaviour {
    static private SaveablePrefabManager instance;

    static private string RESOURCE_LOCATION = "SaveablePrefabs/";

    static private string ENEMY_FOLDER = "SaveableEnemyPrefabs/";

    static private string PLAYER_PREFAB_NAME = "Player";
    static private string OTHER_PLAYER_PREFAB_NAME = "OtherPlayer";
    static private string SERVER_PLAYER_PREFAB_NAME = "ServerPlayer";
    static private string SCROMBOLO_BOMBOLO_NAME = ENEMY_FOLDER + "Scrombolo_Bombolo";
    static private string ITEM_NAME = "Item";

    static GameObject PLAYER_PREFAB;
    static GameObject OTHER_PLAYER_PREFAB;
    static GameObject SCROMBOLO_BOMBOLO_PREFAB;
    static GameObject SERVER_PLAYER_PREFAB;

    //Maybe make this into a two way dictionary type?
    static public Dictionary<byte[], GameObject> ByteToPrefabKey { get; private set; }
    static Dictionary<GameObject, byte[]> PrefabToByteKey;

    static public Dictionary<string, GameObject> StringToPrefabKey { get; private set; }

    static public List<GameObject> SaveablePrefabs { get; private set; }

    static public Dictionary<uint, GameObject> NetworkIdsPrefabs {get; private set;}

    static uint MAX_BYTES_FOR_ENTITIES = 1024;

    uint numberOfActiveEntities = 0;
    static byte[] currentlyActiveEntities;

    static bool retrieveIsActiveEntity(uint location)
    {
        byte exactByte = currentlyActiveEntities[location / 8];

        byte byteLocation = (byte)(location % 8);

        byte oneByte = 1;

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

    static public void DeletePrefab(uint id) {
        GameObject prefab = NetworkIdsPrefabs[id];
        DeletePrefab(prefab);
    }

    static public void DeletePrefab(GameObject prefab)
    {
        PrefabSaveInfo entity = prefab.GetComponent<PrefabSaveInfo>();

        if (entity != null)
        {
            removeLocation(entity.NetworkId);

        }

        SaveablePrefabs.Remove(prefab);
        NetworkIdsPrefabs.Remove(entity.NetworkId);

        EventManager.SetPrefabRemovedFromScene(prefab);

        Destroy(prefab);
    }

    static public GameObject CreatePrefab(GameObject prefab, Vector3 position, Quaternion rotation, uint? id = null, bool isQuiet = false)
    {
        //Maybe use object pooling
        GameObject newObject = Instantiate(prefab, position, rotation);

        PrefabSaveInfo entity = newObject.gameObject.AddComponent<PrefabSaveInfo>();
        entity.PrefabId = PrefabToByteKey[prefab];

        if (id == null) {
            entity.NetworkId = getUnoccupiedId();
        }
        else {
            uint idReal = (uint)id;

            if (retrieveIsActiveEntity(idReal)) {
                throw new Exception("Already exsists exception :sob:");
            }
            addLocation(idReal);
            entity.NetworkId = idReal;
        }

        SaveablePrefabs.Add(newObject);
        NetworkIdsPrefabs.Add(entity.NetworkId, newObject);

        if (!isQuiet) {
            EventManager.SetPrefabAddedToScene(newObject);
        }

        return newObject;
    }

    static public GameObject CreatePrefab(string prefabName, uint? id = null, bool isQuiet = false)
    {
        return CreatePrefab(StringToPrefabKey[prefabName], Vector3.zero, Quaternion.identity, id, isQuiet);
    }

    public static GameObject CreatePrefab(string prefabName, Vector3 position, Quaternion rotation, uint? id = null, bool isQuiet = false)
    {
        return CreatePrefab(StringToPrefabKey[prefabName], position, rotation, id, isQuiet);
    }

    public static GameObject CreatePrefab(byte[] prefabId, Vector3 position, Quaternion rotation, uint? id = null, bool isQuiet = false)
    {
        return CreatePrefab(ByteToPrefabKey[prefabId], position, rotation, id, isQuiet);
    }

#if UNITY_EDITOR
    //Only used for testing creating stuff through the editor
    public void EDITOR_SETUP() {
        OnSuccessfulAwake();
    }

#endif

    //wtf this here?!
    /*private void OnEnable() {
        EventManager.LocalGameObjectPlayerAddedToScene += clearSaveablePrefabs;
    }
    */
    private void OnDisable() {
        //EventManager.LocalGameObjectPlayerAddedToScene -= clearSaveablePrefabs;
        SaveablePrefabs.Clear();
    }

    void clearSaveablePrefabs(GameObject player) {
        SaveablePrefabs.Clear();
    }

    void OnSuccessfulAwake()
    {
        SaveablePrefabs = new List<GameObject>();
        currentlyActiveEntities = new byte[1] { 0 };

        PLAYER_PREFAB = Resources.Load<GameObject>(RESOURCE_LOCATION + PLAYER_PREFAB_NAME);
        OTHER_PLAYER_PREFAB = Resources.Load<GameObject>(RESOURCE_LOCATION + OTHER_PLAYER_PREFAB_NAME);
        SCROMBOLO_BOMBOLO_PREFAB = Resources.Load<GameObject>(RESOURCE_LOCATION + SCROMBOLO_BOMBOLO_NAME);
        SERVER_PLAYER_PREFAB = Resources.Load<GameObject>(RESOURCE_LOCATION + SERVER_PLAYER_PREFAB_NAME);


        ByteToPrefabKey = new Dictionary<byte[], GameObject>(new ByteArrayComparer());
        ByteToPrefabKey.Add(new byte[1] { 1 }, PLAYER_PREFAB);
        ByteToPrefabKey.Add(new byte[1] { 2 }, OTHER_PLAYER_PREFAB);
        ByteToPrefabKey.Add(new byte[1] { 3 }, SERVER_PLAYER_PREFAB);
        ByteToPrefabKey.Add(new byte[1] { 4 }, SCROMBOLO_BOMBOLO_PREFAB);

        PrefabToByteKey = ByteToPrefabKey.ToDictionary((i) => i.Value, (i) => i.Key);

        //TODO : Automate creation of this too prone to error otherwise
        StringToPrefabKey = new Dictionary<string, GameObject>();
        StringToPrefabKey.Add(PLAYER_PREFAB_NAME, PLAYER_PREFAB);
        StringToPrefabKey.Add(OTHER_PLAYER_PREFAB_NAME, OTHER_PLAYER_PREFAB);
        StringToPrefabKey.Add(SERVER_PLAYER_PREFAB_NAME, SERVER_PLAYER_PREFAB);
        StringToPrefabKey.Add(SCROMBOLO_BOMBOLO_NAME, SCROMBOLO_BOMBOLO_PREFAB);

        NetworkIdsPrefabs = new Dictionary<uint, GameObject>();
    }
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            OnSuccessfulAwake();
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
