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
            ByteArrayPrint.PrintByteArray(DataService.LoadEntitySaveData(), "Loaded Byte index of total length : ");
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
        if (saveableMonoBehaviorQuickRetriveDictionary.ContainsKey(prefabAdded)) {
            return;
        }

        PrefabSaveInfo prefabSaveInfo = prefabAdded.GetComponent<PrefabSaveInfo>();
        List<MonoBehaviourSaveFields> monoBehaviorAndTypeOrder = PrefabToMonoBehaviorFieldOrder[prefabSaveInfo.PrefabId];

        List<MonoBehaviour> listToAdd = new List<MonoBehaviour>();

        int currentComponentIndex = 0;
        foreach (MonoBehaviour component in prefabAdded.GetComponents<MonoBehaviour>()) {
            SaveableComponent saveableId = (SaveableComponent)Attribute.GetCustomAttribute(component.GetType(), typeof(SaveableComponent));

            if (saveableId != null) {
                listToAdd.Add(component);
                if (component.GetType() != monoBehaviorAndTypeOrder[currentComponentIndex].MonobehaviorSaveType) {
                    Debug.Log($"{prefabAdded.name} has mis-match when adding {component.GetType().Name}");
                }
                currentComponentIndex++;
            }
        }

        listToAdd = listToAdd.OrderBy(comp => ((SaveableComponent)Attribute.GetCustomAttribute(comp.GetType(), typeof(SaveableComponent))).SaveClassName).ToList();

        saveableMonoBehaviorQuickRetriveDictionary.Add(prefabAdded, listToAdd);
    }

    void RemoveSaveableMonoBehavior(GameObject prefabRemoved) {
        saveableMonoBehaviorQuickRetriveDictionary.Remove(prefabRemoved);
    }

    void LoadAllEntityDataToPrefabs(byte[] byteData) {

        while (byteData.Length > 0) {
            PrefabCreateFromByteArray(byteData, out int readBytes);
            byteData = byteData.Skip(readBytes).ToArray();
        }
    }

    void savePrefabOnLocalPlayerLeave(GameObject player) {
        SaveAllPrefabData();
        EventManager.SetPlayerExitedGameSoSafeToLeave();
    }

    void SaveAllPrefabData() {
        if (DataService.IsLocalSave) {
            Debug.Log("Attempting to save game!");
            byte[] saveData = GetAllBytesForPrefabData();

            DataService.SaveEntitySaveData(saveData);

#if UNITY_EDITOR
            ByteArrayPrint.PrintByteArray(saveData, "Loaded Byte index of total length : ");
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

    public int GetAmountOfBytesUsedInPrefab(byte[] prefabId, byte[] dataToSetTo) {
        int readBytes = 0;

        List<MonoBehaviourSaveFields> monoBehaviorAndTypeOrder = PrefabToMonoBehaviorFieldOrder[prefabId];

        //Get a generic version to read through
        GameObject prefab = SaveablePrefabManager.ByteToPrefabKey[prefabId];

        for (int i = 0; i < monoBehaviorAndTypeOrder.Count; i++) {
            MonoBehaviourSaveFields componentType = monoBehaviorAndTypeOrder[i];
            foreach (FieldInfo field in componentType.SaveFields) {
                if (readBytes > dataToSetTo.Length) { return -1; }
                int bytesUsed = ConvertToByteArray.GetSizeOfValue(field.FieldType, dataToSetTo.Skip(readBytes).ToArray());
                if (bytesUsed == -1) { return -1; }
                readBytes += bytesUsed;
            }

        }

        return readBytes;
    }

    public void SetAllBytesInAPrefabSafe(GameObject prefab, byte[] prefabId, byte[] dataToSetTo, out int readBytes) {
        if (prefabId == null) {
            prefabId = prefab.GetComponent<PrefabSaveInfo>().PrefabId;
        }

        int bytesUsedForPrefabSave = GetAmountOfBytesUsedInPrefab(prefabId, dataToSetTo);

        if (bytesUsedForPrefabSave > 0) {
            SetAllBytesInAPrefab(prefab, prefabId, dataToSetTo, out readBytes);
        }
        else {
            readBytes = 0;
        }

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
            if (!saveableMonoBehaviorQuickRetriveDictionary.TryGetValue(prefab, out List<MonoBehaviour> saveableMonobehaviors)) {
                GetAllSaveableMonoBehaviors(prefab);
            }

            component = saveableMonobehaviors.Find((e) => e.GetType() == componentType.MonobehaviorSaveType);

            foreach (FieldInfo field in componentType.SaveFields) {
                field.SetValue(component, ConvertToByteArray.ConvertBytesToValue(field.FieldType, dataToSetTo.Skip(readBytes).ToArray(), out int bytesUsed));
                readBytes += bytesUsed;
            }

        }
    }

    GameObject PrefabCreateFromByteArray(byte[] dataToSetTo, out int readBytes) {
        readBytes = 0;

        readBytes = Array.IndexOf(dataToSetTo, (byte)0);
        /*for (; readBytes < dataToSetTo.Length; readBytes++) {
            if (dataToSetTo[readBytes] == 0) {
                break;
            }
        }*/

        byte[] id = dataToSetTo.Take(readBytes).ToArray();

#if UNITY_EDITOR
        foreach (byte idNum in id) {
            Debug.Log("Byte id : " + idNum);
        }
#endif

        readBytes++;


        ByteArrayPrint.PrintByteArray(dataToSetTo.Skip(readBytes).ToArray(), "Array Loaded before checking pos : ");

        Vector3 prefabPosition = (Vector3)ConvertToByteArray.ConvertBytesToValue(typeof(Vector3), dataToSetTo.Skip(readBytes).ToArray(), out int bytesUsed);
        float eulerRotationZ = (float)ConvertToByteArray.ConvertBytesToValue(typeof(float), dataToSetTo.Skip(readBytes + bytesUsed).ToArray(), out int rotBytesUsed);

        GameObject prefab = SaveablePrefabManager.CreatePrefab(id, prefabPosition, eulerRotationZ);

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



    public List<byte> PrefabGetByteArray(GameObject prefab, bool addId = true, bool addRotAndPos = true) {
        PrefabSaveInfo prefabSaveInfo = prefab.GetComponent<PrefabSaveInfo>();

        if (prefabSaveInfo == null) {
            Debug.LogError("Was not able to retrieve PrefabSaveInfo for : " + prefab.name);
        }

        List<byte> returnArray = new List<byte>();

        if (addId) {
            returnArray = prefabSaveInfo.PrefabId.ToList();

            returnArray.Add(0);
        }

        if (addRotAndPos) {
            byte[] prefabPosition = ConvertToByteArray.ConvertValueToBytes(prefab.transform.position);
            byte[] prefabRotation = ConvertToByteArray.ConvertValueToBytes(prefab.transform.rotation.eulerAngles.z);
            returnArray.AddRange(prefabPosition);
            returnArray.AddRange(prefabRotation);
        }

        List<MonoBehaviourSaveFields> monoBehaviorAndTypeOrder = PrefabToMonoBehaviorFieldOrder[prefabSaveInfo.PrefabId];

        List<MonoBehaviour> saveableMonobehaviors;
        if (!saveableMonoBehaviorQuickRetriveDictionary.TryGetValue(prefab, out saveableMonobehaviors)) {
            GetAllSaveableMonoBehaviors(prefab);
            saveableMonobehaviors = saveableMonoBehaviorQuickRetriveDictionary[prefab];
        }

        //Debug.Log($"MonoBehaviorTypeAndOrder.count {monoBehaviorAndTypeOrder.Count}\nsaveableMonobehaviors.Count {saveableMonobehaviors.Count}");
        for (int i = 0; i < monoBehaviorAndTypeOrder.Count; i++) {
            MonoBehaviourSaveFields componentType = monoBehaviorAndTypeOrder[i];
            MonoBehaviour component = saveableMonobehaviors[i];


            foreach (FieldInfo field in componentType.SaveFields) {
                //Debug.Log($"What is component?35 {component}\nField name {field.Name}\nField value {field.GetValue(component)}");
                returnArray.AddRange(ConvertToByteArray.ConvertValueToBytes(field.GetValue(component)));
            }
        }

        return returnArray;
    }

    //TODO : Possibly change the system so it saves stuff with strname allowing for updates :skull:
    /// <summary>
    /// Gets a ordered list of saveable MonoBehaviors with their saveable Fields under them.
    /// List is ordered based on order of the string SaveName (Ex. Save name "A" goes before
    /// "B").
    /// </summary>
    /// <param name="prefab">Prefab we want to save</param>
    /// <returns>Ordered MonoBehaviorSaveFields list</returns>
    List<MonoBehaviourSaveFields> AddPrefabSavableType(GameObject prefab) {
        List<MonoBehaviourSaveFields> monoToSort = new List<MonoBehaviourSaveFields>();

        foreach (MonoBehaviour component in prefab.GetComponents<MonoBehaviour>()) {
            SaveableComponent saveableId = (SaveableComponent)Attribute.GetCustomAttribute(component.GetType(), typeof(SaveableComponent));

            if (saveableId == null) { continue; }

            FieldInfo[] field = component.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance);

            MonoBehaviourSaveFields newSaveField = new MonoBehaviourSaveFields(component.GetType());
            
            for (int i = 0; i < field.Length; i++) {
                FieldInfo fieldToCheck = field[i];

                Saveable newSavableAttribute = (Saveable)Attribute.GetCustomAttribute(fieldToCheck, typeof(Saveable));
                if (newSavableAttribute != null) {
                    newSaveField.SaveFields.Add(fieldToCheck);
                }

            }

            if (newSaveField.SaveFields.Count > 0) {
                IOrderedEnumerable<FieldInfo> orderedField = newSaveField.SaveFields.OrderBy(fieldInfo => ((Saveable)Attribute.GetCustomAttribute(fieldInfo, typeof(Saveable))).SaveName);
                newSaveField.SaveFields = orderedField.ToList();
                monoToSort.Add(newSaveField);
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
        if (QuickInputWrapper.GetKeyPress("LeftCtrl") && QuickInputWrapper.GetKeyPress("S")) {
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
