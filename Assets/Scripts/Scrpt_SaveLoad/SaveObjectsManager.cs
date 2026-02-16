using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ByteArrayCompare : IComparer<byte[]> {
    public int Compare(byte[] x, byte[] y) {
        if (x.Length < y.Length)
            return -1;
        else if (x.Length > y.Length)
            return 1;
        else {
            for (int i = 0; i < x.Length; i++) {
                if (x[i] < y[i]) {
                    return -1;
                }
                else if (x[i] > y[i]) {
                    return 1;
                }
            }
            return 0;
        }
    }
}

public class SaveObjectsManager : MonoBehaviour
{
    Dictionary<Type, byte[]> objectTypeDictionary;

    List<(FieldInfo, MonoBehaviour)> fieldInfos;

    Dictionary<byte[], List<(Type, List<FieldInfo>)>> PrefabToMonoBehaviorFieldOrder;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        fieldInfos = new List<(FieldInfo, MonoBehaviour)>();

        AllObjects();
    }

    /// <summary>
    /// Creates errors if the savable byte id doesn't
    /// follow the rule of no zero bytes or
    /// throws an error if there already is that byte string in the arrow
    /// </summary>
    /// <param name="component"></param>
    /// <param name="saveableIdToAdd"></param>
    void SuperErrorChecking(MonoBehaviour component, string saveableIdToAdd)
    {
        bool isIllegalByte = false;
        for (int i = 0; i < saveableIdToAdd.Length; i++)
        {
            if (saveableIdToAdd[i] == 0)
            {
                isIllegalByte = true;
                Debug.LogError("Saveable object '" + component.name + "' id byte number " + i + " has an illegal byte number of " + saveableIdToAdd[i] +
                                " this number is reseved for special operations.");
            }
        }

        if (!isIllegalByte)
        {
            if (objectTypeDictionary.ContainsKey(component.GetType()))
            {

                if (objectTypeDictionary[component.GetType()].Length !=
                    saveableIdToAdd.Length)
                    Debug.LogError("Saveable object '" + component.name + "' contains two different ids in dictionairy");
                else
                {
                    for (int i = 0; i < objectTypeDictionary[component.GetType()].Length; i++)
                    {
                        if (objectTypeDictionary[component.GetType()][i] != saveableIdToAdd[i])
                        {
                            Debug.LogError("Saveable object '" + component.name + "' contains two different ids in dictionairy");
                            break;
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// Checks through the gameobjects components
    /// to see if anything needs to be saved
    /// </summary>
    void AddGameObjectToSave(GameObject gOToSave)
    {

        foreach (MonoBehaviour component in gOToSave.GetComponents<MonoBehaviour>())
        {
            SaveableComponent saveableId = (SaveableComponent)Attribute.GetCustomAttribute(component.GetType(), typeof(SaveableComponent));

            FieldInfo[] field = component.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance);

            string saveableClassName = null;

            if (saveableId != null)
            {
                saveableClassName = saveableId.SaveClassName;
            }

            bool wasSaveableField = false;

            for (int i = 0; i < field.Length; i++)
            {

                FieldInfo fieldToCheck = field[i];

                Saveable newSavableAttribute = (Saveable)Attribute.GetCustomAttribute(fieldToCheck, typeof(Saveable));

                if (newSavableAttribute != null && saveableClassName == null)
                {
                    Debug.LogError("Saveable field '" + fieldToCheck.Name + "' yet it's parent class '" +
                                    component.name + "' has no saveable id attribute.");
                }


                if (newSavableAttribute != null && fieldToCheck.MemberType == MemberTypes.Field)
                {
                    wasSaveableField = true;
                    FieldInfo saveableField = field[i];
                    fieldInfos.Add(new Tuple<FieldInfo, MonoBehaviour>(saveableField, component));

                    Debug.Log("Value of '" + field[i].Name + "' : " + saveableField.GetValue(component).ToString());
                }
            }

            if (saveableClassName != null)
            {
                if (!wasSaveableField)
                {
                    Debug.LogError("Saveable object '" + component.name + "' has no fields to save");
                }
                else
                {
                    SuperErrorChecking(component, saveableClassName);

                    //objectTypeDictionary.TryAdd(component.GetType(), saveableClassName);
                }
            }
        }
    }

    void PrefabSetToByteArray(GameObject prefab, byte[] dataToSetTo) {

        int idDelinatorPosition = 0;
        for (; idDelinatorPosition < dataToSetTo.Length; idDelinatorPosition++) {
            if (dataToSetTo[idDelinatorPosition] == 0) {
                break;
            }
        }

        byte[] id = (byte[]) dataToSetTo.Take(idDelinatorPosition);

        List<Tuple<Type, List<FieldInfo>>> monoBehaviorAndTypeOrder = PrefabToMonoBehaviorFieldOrder[id];

        foreach (Tuple<Type, List<FieldInfo>> componentType in monoBehaviorAndTypeOrder) {
            MonoBehaviour component = (MonoBehaviour) prefab.GetComponent(componentType.Item1.Name);

            foreach (FieldInfo field in componentType.Item2) {
                field.SetValue(component, )
            }
        }
    }

    void PrefabGetByteArray(GameObject prefab, byte[] dataToSetTo) {

    }

    List<Tuple<Type, List<FieldInfo>>> AddPrefabSavableType(GameObject prefab) {

        List<Tuple<Type, List<FieldInfo>>> monoToSort = new List<Tuple<Type, List<FieldInfo>>>();

        foreach (MonoBehaviour component in prefab.GetComponents<MonoBehaviour>()) {
            SaveableComponent saveableId = (SaveableComponent)Attribute.GetCustomAttribute(component.GetType(), typeof(SaveableComponent));

            if (saveableId == null) { continue; }

            FieldInfo[] field = component.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance);


            for (int i = 0; i < field.Length; i++) {

                FieldInfo fieldToCheck = field[i];

                Saveable newSavableAttribute = (Saveable)Attribute.GetCustomAttribute(fieldToCheck, typeof(Saveable));


                if (newSavableAttribute != null) {
                    Tuple<Type, List<FieldInfo>> ourTuple = monoToSort.Find(i => i.Item1 == component.GetType());

                    if (ourTuple != null) {
                        ourTuple = new Tuple<Type, List<FieldInfo>>(component.GetType(), new List<FieldInfo>());
                        ourTuple.Item2.Add(fieldToCheck);
                    }
                    else {
                        ourTuple.Item2.Add(fieldToCheck);
                    }
                }
            }


            Tuple<Type, List<FieldInfo>> ourMono = monoToSort.Find(i => i.Item1 == component.GetType());

            if (ourMono != null) {
                ourMono.Item2 = ourMono.Item2.OrderBy(fieldInfo => ((Saveable)Attribute.GetCustomAttribute(fieldInfo, typeof(Saveable))).SaveName);
            }
        }

        monoToSort.OrderBy(tuple => ((SaveableComponent)Attribute.GetCustomAttribute(tuple.Item1,typeof(SaveableComponent))).SaveClassName);

        return monoToSort;
    }

    void AllObjects()
    {
        Scene curScene = SceneManager.GetActiveScene();
        GameObject[] rootGameObjects = curScene.GetRootGameObjects();

        foreach (GameObject rootGameObject in rootGameObjects)
        {
            AddGameObjectToSave(rootGameObject);
        }

    }

    // Update is called once per frame
    void Update()
    {
        /*foreach (var item in fieldInfos)
        { 
            Debug.Log("Value of this object : " + item.Item1.GetValue(item.Item2));

            Debug.Log("Name of parent : " + item.Item2.name);
        }*/

        /*foreach (var item in objectTypeDictionary.Keys)
        {
            Debug.Log("Name of item in dictionairy '" + item.Name + "' byte number it equals '" + objectTypeDictionary[item][0] + "'");
        }*/
    }
}
