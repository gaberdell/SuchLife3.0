using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

//Small helper class to encapsualte stuff
public class MonoBehaviourSaveFields {

    public MonoBehaviourSaveFields(Type saveType) {
        MonobehaviorSaveType = saveType;
        SaveFields = new List<FieldInfo> ();
    }


    public Type MonobehaviorSaveType;
    public List<FieldInfo> SaveFields;
}

//Thx for this for how to use the OnApplicationQuit and stuff
//https://gamedev.stackexchange.com/questions/191550/saving-settings-when-exiting-the-application
public class SaveObjectsManager : MonoBehaviour
{
    //Note not a reliable way to get local player only when is loaded in through save data
    public static GameObject LocalPlayerFromSaveData { get; private set; }


    Dictionary<GameObject, List<MonoBehaviour>> saveableMonoBehaviorQuickRetriveDictionary;

    Dictionary<byte[], List<MonoBehaviourSaveFields>> PrefabToMonoBehaviorFieldOrder;

    private static float MAX_AUTOSAVE_TIME_IN_SECONDS = 300;
    private static float autoSaveTime;

    void Start()
    {
        autoSaveTime = MAX_AUTOSAVE_TIME_IN_SECONDS;

        saveableMonoBehaviorQuickRetriveDictionary = new Dictionary<GameObject, List<MonoBehaviour>>();

        PrefabToMonoBehaviorFieldOrder = new Dictionary<byte[], List<MonoBehaviourSaveFields>>(new ByteArrayComparer());


        foreach (KeyValuePair<byte[], GameObject> pair in SaveablePrefabManager.ByteToPrefabKey) {
            if (pair.Value == null)
                Debug.LogError("Resource probably improperly loaded");
            PrefabToMonoBehaviorFieldOrder.Add(pair.Key, AddPrefabSavableType(pair.Value));
        }


        //Lets get loading!
        if (DataService.IsLocalSave) {
#if UNITY_EDITOR
            byte[] localSaveData = DataService.LoadEntitySaveData();
            for (int i = 0; i < localSaveData.Length; i++) {
                Debug.Log("Loaded Byte index (" + i + ") : " + localSaveData[i]);
            }
#endif

            LoadAllEntityDataToPrefabs(DataService.LoadEntitySaveData());
        }

    }

    void OnApplicationPause(bool isPaused) {
        if (isPaused) {
            SaveAllPrefabData();
        }

    }

    void OnApplicationQuit() {
        SaveAllPrefabData();
    }

    private void OnEnable() {
        EventManager.PrefabAddedToScene += GetAllSaveableMonoBehaviors;
        EventManager.PrefabRemovedFromScene += RemoveSaveableMonoBehavior;

        EventManager.LocalGameObjectPlayerLeftScene += savePrefabOnLocalPlayerLeave;
    }

    private void OnDisable() {
        EventManager.PrefabAddedToScene -= GetAllSaveableMonoBehaviors;
        EventManager.PrefabRemovedFromScene -= RemoveSaveableMonoBehavior;

        EventManager.LocalGameObjectPlayerLeftScene -= savePrefabOnLocalPlayerLeave;
    }


    void GetAllSaveableMonoBehaviors(GameObject prefabAdded) {

        List<MonoBehaviour> listToAdd = new List<MonoBehaviour>();

        foreach (MonoBehaviour component in prefabAdded.GetComponents<MonoBehaviour>()) {
            SaveableComponent saveableId = (SaveableComponent)Attribute.GetCustomAttribute(component.GetType(), typeof(SaveableComponent));

            if (saveableId != null) {
                listToAdd.Add(component);
            }
        }

        listToAdd = listToAdd.OrderBy(comp => ((SaveableComponent)Attribute.GetCustomAttribute(comp.GetType(), typeof(SaveableComponent))).SaveClassName).ToList();

        saveableMonoBehaviorQuickRetriveDictionary.Add(prefabAdded, listToAdd);
    }

    void RemoveSaveableMonoBehavior(GameObject prefabRemoved) {
        saveableMonoBehaviorQuickRetriveDictionary.Remove(prefabRemoved);
    }

    void LoadAllEntityDataToPrefabs(byte[] byteData) {

#if UNITY_EDITOR
        for (int i = 0; i < byteData.Length; i++) {
            Debug.Log("Loaded Byte index ("+ i + ") : " + byteData[i]);
        }
#endif

        while (byteData.Length > 0) {
            PrefabCreateFromByteArray(byteData, out int readBytes);
            byteData = byteData.Skip(readBytes).ToArray();
        }
    }

    void savePrefabOnLocalPlayerLeave(GameObject player) {
        SaveAllPrefabData();
    }

    void SaveAllPrefabData() {
        Debug.Log("Attempting to save game!");
        if (DataService.IsLocalSave) {
            byte[] saveData = GetAllBytesForPrefabData();
            
            Debug.Log("Save byte length : " + saveData.Length);

            DataService.SaveEntitySaveData(saveData);

#if UNITY_EDITOR
            for (int i = 0; i < saveData.Length; i++) {
                Debug.Log("Save Byte index (" + i + ") : " + saveData[i]);
            }
#endif
        }
    }

    byte[] GetAllBytesForPrefabData() {
        List<byte> savePrefab = new List<byte>();

        Debug.Log("Saveable prefab length : " + SaveablePrefabManager.SaveablePrefabs.Count);

        foreach (GameObject loadedPefab in SaveablePrefabManager.SaveablePrefabs) {

            savePrefab.AddRange(PrefabGetByteArray(loadedPefab));
        }
        return savePrefab.ToArray();
    }

    public void SetAllBytesInAPrefab(GameObject prefab, byte[] prefabId, byte[] dataToSetTo, out int readBytes) {
        readBytes = 0;

        if (prefabId == null) {
            prefabId = prefab.GetComponent<PrefabSaveInfo>().PrefabId;
        }

        List<MonoBehaviourSaveFields> monoBehaviorAndTypeOrder = PrefabToMonoBehaviorFieldOrder[prefabId];

        for (int i = 0; i < monoBehaviorAndTypeOrder.Count; i++) {
            MonoBehaviourSaveFields componentType = monoBehaviorAndTypeOrder[i];

            MonoBehaviour component;
            //ARE THESE SUPPOSED TO BE ZERO CHANGE IT BACK IF IM WRONG :SKULL:
            if (saveableMonoBehaviorQuickRetriveDictionary.TryGetValue(prefab, out List<MonoBehaviour> saveableMonobehaviors)) {
                component = saveableMonobehaviors[i];
            }
            else {
                GetAllSaveableMonoBehaviors(prefab);

                component = saveableMonoBehaviorQuickRetriveDictionary[prefab][i];
            }

            

            foreach (FieldInfo field in componentType.SaveFields) {
                field.SetValue(component, ConvertToByteArray.ConvertBytesToValue(field.FieldType, dataToSetTo.Skip(readBytes).ToArray(), out int bytesUsed));
                readBytes += bytesUsed;
            }

        }
    }

    GameObject PrefabCreateFromByteArray(byte[] dataToSetTo, out int readBytes) {
        readBytes = 0;

        for (; readBytes < dataToSetTo.Length; readBytes++) {
            if (dataToSetTo[readBytes] == 0) {
                break;
            }
        }

        byte[] id = dataToSetTo.Take(readBytes).ToArray();

#if UNITY_EDITOR
        foreach (byte idNum in id) {
            Debug.Log("Byte id : " + idNum);
        }
#endif

        readBytes++;


        Vector3 prefabPosition = (Vector3)ConvertToByteArray.ConvertBytesToValue(typeof(Vector3), dataToSetTo.Skip(readBytes).ToArray(), out int bytesUsed);
        Vector3 eulerRotation = (Vector3)ConvertToByteArray.ConvertBytesToValue(typeof(Vector3), dataToSetTo.Skip(readBytes + bytesUsed).ToArray(), out int rotBytesUsed);

        GameObject prefab = SaveablePrefabManager.CreatePrefab(id, prefabPosition, Quaternion.Euler(eulerRotation.x, eulerRotation.y, eulerRotation.z));

        //TODO : Player might not be id one so maybe change later?
        if (id.Length == 1 && id[0] == 1) {
            LocalPlayerFromSaveData = prefab;
        }
        //newStartPos += idDelinatorPosition; Idk this makes more sense to me idk tho
        readBytes += bytesUsed + rotBytesUsed;

        SetAllBytesInAPrefab(prefab, id, dataToSetTo.Skip(readBytes).ToArray(), out int newBytesUsed);
        readBytes += newBytesUsed;

        return prefab;
    }



    public List<byte> PrefabGetByteArray(GameObject prefab, bool addId = true) {
        PrefabSaveInfo prefabSaveInfo = prefab.GetComponent<PrefabSaveInfo>();

        if (prefabSaveInfo == null) {
            Debug.LogError("Was not able to retrieve PrefabSaveInfo for : " + prefab.name);
        }

        List<byte> returnArray = new List<byte>();

        if (addId) {
            returnArray = prefabSaveInfo.PrefabId.ToList();

            returnArray.Add(0);
        }

        byte[] prefabPosition = ConvertToByteArray.ConvertValueToBytes(prefab.transform.position);
        byte[] prefabRotation = ConvertToByteArray.ConvertValueToBytes(prefab.transform.rotation.eulerAngles);

        returnArray.AddRange(prefabPosition);
        returnArray.AddRange(prefabRotation);
        
        List<MonoBehaviourSaveFields> monoBehaviorAndTypeOrder = PrefabToMonoBehaviorFieldOrder[prefabSaveInfo.PrefabId];

        for (int i = 0; i < monoBehaviorAndTypeOrder.Count; i++) {
            MonoBehaviourSaveFields componentType = monoBehaviorAndTypeOrder[i];

            MonoBehaviour component;
            if (saveableMonoBehaviorQuickRetriveDictionary.TryGetValue(prefab, out List<MonoBehaviour> saveableMonobehaviors)) {
                component = saveableMonobehaviors[0];
            }
            else {
                GetAllSaveableMonoBehaviors(prefab);

                component = saveableMonoBehaviorQuickRetriveDictionary[prefab][0];
            }

            //MonoBehaviour component = saveableMonoBehaviorQuickRetriveDictionary[prefab][0];

            foreach (FieldInfo field in componentType.SaveFields) {
                returnArray.AddRange(ConvertToByteArray.ConvertValueToBytes(field.GetValue(component)));
            }
        }

        return returnArray;
    }

    List<MonoBehaviourSaveFields> AddPrefabSavableType(GameObject prefab) {
        List<MonoBehaviourSaveFields> monoToSort = new List<MonoBehaviourSaveFields>();

        foreach (MonoBehaviour component in prefab.GetComponents<MonoBehaviour>()) {
            SaveableComponent saveableId = (SaveableComponent)Attribute.GetCustomAttribute(component.GetType(), typeof(SaveableComponent));

            if (saveableId == null) { continue; }

            FieldInfo[] field = component.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance);


            for (int i = 0; i < field.Length; i++) {

                FieldInfo fieldToCheck = field[i];

                Saveable newSavableAttribute = (Saveable)Attribute.GetCustomAttribute(fieldToCheck, typeof(Saveable));


                if (newSavableAttribute != null) {
                    MonoBehaviourSaveFields ourTuple = monoToSort.Find(i => i.MonobehaviorSaveType == component.GetType());


                    if (ourTuple != null) {
                        ourTuple.SaveFields.Add(fieldToCheck);
                    }
                    else {
                        ourTuple = new MonoBehaviourSaveFields(component.GetType());
                        ourTuple.SaveFields.Add(fieldToCheck);
                    }

                    monoToSort.Add(ourTuple);
                }

            }


            MonoBehaviourSaveFields ourMono = monoToSort.Find(i => i.MonobehaviorSaveType == component.GetType());

            if (ourMono != null) {
                IOrderedEnumerable<FieldInfo> orderedField = ourMono.SaveFields.OrderBy(fieldInfo => ((Saveable)Attribute.GetCustomAttribute(fieldInfo, typeof(Saveable))).SaveName); ;

                ourMono.SaveFields = orderedField.ToList();


                //ourMono.SaveFields = (List<FieldInfo>) ourMono.SaveFields.OrderBy(fieldInfo => ((Saveable)Attribute.GetCustomAttribute(fieldInfo, typeof(Saveable))).SaveName);
            }
        }

        IOrderedEnumerable<MonoBehaviourSaveFields> orderedObjects = monoToSort.OrderBy(tuple => ((SaveableComponent)Attribute.GetCustomAttribute(tuple.MonobehaviorSaveType, typeof(SaveableComponent))).SaveClassName);

        monoToSort = orderedObjects.ToList();

        return monoToSort;
    }

    // Update is called once per frame
    void Update()
    {
        autoSaveTime -= Time.deltaTime;
        //Test input to spit something out
        if (Input.GetKeyDown(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.S)) {
            Debug.Log("Manually saving prefabs");
            SaveAllPrefabData();
            autoSaveTime = MAX_AUTOSAVE_TIME_IN_SECONDS;
        }


        if (autoSaveTime < 0) {
            SaveAllPrefabData();
            autoSaveTime = MAX_AUTOSAVE_TIME_IN_SECONDS;
        }
    }
}
