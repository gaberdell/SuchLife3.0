using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

public class SaveObjectsManager : MonoBehaviour
{

    Dictionary<GameObject, List<MonoBehaviour>> saveableMonoBehaviorQuickRetriveDictionary;

    Dictionary<byte[], List<(Type, List<FieldInfo>)>> PrefabToMonoBehaviorFieldOrder;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        saveableMonoBehaviorQuickRetriveDictionary = new Dictionary<GameObject, List<MonoBehaviour>>();

        PrefabToMonoBehaviorFieldOrder = new Dictionary<byte[], List<(Type, List<FieldInfo>)>>(new ByteArrayComparer());


        foreach (KeyValuePair<byte[], GameObject> pair in SaveablePrefabManager.ByteToPrefabKey) {
            if (pair.Value == null)
                Debug.LogError("Resource probably improperly loaded");
            PrefabToMonoBehaviorFieldOrder.Add(pair.Key, AddPrefabSavableType(pair.Value));
        }
        
    }

    private void OnEnable() {
        EventManager.PrefabAddedToScene += GetAllSaveableMonoBehaviors;
        EventManager.PrefabRemovedFromScene -= RemoveSaveableMonoBehavior;
    }

    private void OnDisable() {
        EventManager.PrefabAddedToScene -= GetAllSaveableMonoBehaviors;
        EventManager.PrefabRemovedFromScene -= RemoveSaveableMonoBehavior;
    }


    void GetAllSaveableMonoBehaviors(GameObject prefabAdded) {

        List<MonoBehaviour> listToAdd = new List<MonoBehaviour>();

        foreach (MonoBehaviour component in prefabAdded.GetComponents<MonoBehaviour>()) {
            SaveableComponent saveableId = (SaveableComponent)Attribute.GetCustomAttribute(component.GetType(), typeof(SaveableComponent));

            if (saveableId != null) {
                listToAdd.Add(component);
            }
        }

        listToAdd = (List<MonoBehaviour>) listToAdd.OrderBy(comp => ((SaveableComponent)Attribute.GetCustomAttribute(comp.GetType(), typeof(SaveableComponent))).SaveClassName);

        saveableMonoBehaviorQuickRetriveDictionary.Add(prefabAdded, listToAdd);
    }

    void RemoveSaveableMonoBehavior(GameObject prefabRemoved) {
        saveableMonoBehaviorQuickRetriveDictionary.Remove(prefabRemoved);
    }

    void PrefabSetToByteArray(GameObject prefab, byte[] dataToSetTo) {

        int idDelinatorPosition = 0;
        for (; idDelinatorPosition < dataToSetTo.Length; idDelinatorPosition++) {
            if (dataToSetTo[idDelinatorPosition] == 0) {
                break;
            }
        }

        byte[] id = (byte[]) dataToSetTo.Take(idDelinatorPosition);

        int newStartPos = sizeof(float) * 3;
        prefab.transform.position = (Vector3) ByteConvertor.ConvertBytesToValue(typeof(Vector3), (byte[]) dataToSetTo.Skip(idDelinatorPosition).Take(newStartPos), out int bytesUsed);

        newStartPos += idDelinatorPosition;

        int positionToReadDataFrom = idDelinatorPosition;

        List<(Type, List<FieldInfo>)> monoBehaviorAndTypeOrder = PrefabToMonoBehaviorFieldOrder[id];


        for (int i = 0; i < monoBehaviorAndTypeOrder.Count; i++) {
            (Type, List<FieldInfo>) componentType = monoBehaviorAndTypeOrder[i];

            MonoBehaviour component = saveableMonoBehaviorQuickRetriveDictionary[prefab][0];

            foreach (FieldInfo field in componentType.Item2) {
                field.SetValue(component, ByteConvertor.ConvertBytesToValue(field.FieldType, (byte[])dataToSetTo.Skip(newStartPos), out bytesUsed));
                newStartPos += bytesUsed;
            }

        }
    }

    byte[] PrefabGetByteArray(GameObject prefab) {
        return new byte[1] {0};
    }

    List<(Type, List<FieldInfo>)> AddPrefabSavableType(GameObject prefab) {
        List<(Type, List<FieldInfo>)> monoToSort = new List<(Type, List<FieldInfo>)>();

        foreach (MonoBehaviour component in prefab.GetComponents<MonoBehaviour>()) {
            SaveableComponent saveableId = (SaveableComponent)Attribute.GetCustomAttribute(component.GetType(), typeof(SaveableComponent));

            if (saveableId == null) { continue; }

            FieldInfo[] field = component.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance);


            for (int i = 0; i < field.Length; i++) {

                FieldInfo fieldToCheck = field[i];

                Saveable newSavableAttribute = (Saveable)Attribute.GetCustomAttribute(fieldToCheck, typeof(Saveable));


                if (newSavableAttribute != null) {
                    (Type, List<FieldInfo>) ourTuple = monoToSort.Find(i => i.Item1 == component.GetType());

                    if (ourTuple.Item1 != null) {
                        ourTuple = (component.GetType(), new List<FieldInfo>());
                        ourTuple.Item2.Add(fieldToCheck);
                    }
                    else {
                        ourTuple.Item2.Add(fieldToCheck);
                    }
                }
            }


            (Type, List<FieldInfo>) ourMono = monoToSort.Find(i => i.Item1 == component.GetType());

            if (ourMono.Item1 != null) {
                ourMono.Item2 = (List<FieldInfo>) ourMono.Item2.OrderBy(fieldInfo => ((Saveable)Attribute.GetCustomAttribute(fieldInfo, typeof(Saveable))).SaveName);
            }
        }

        monoToSort.OrderBy(tuple => ((SaveableComponent)Attribute.GetCustomAttribute(tuple.Item1,typeof(SaveableComponent))).SaveClassName);

        return monoToSort;
    }

    // Update is called once per frame
    void Update()
    {

        //Test input to spit something out
        if (Input.GetKeyDown(KeyCode.RightAlt)) {
            //Implement dis stuff
        }
    }
}
