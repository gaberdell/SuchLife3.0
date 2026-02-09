using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveObjectsManager : MonoBehaviour
{
    Dictionary<Type, byte[]> objectTypeDictionary;

    List<Tuple<FieldInfo, MonoBehaviour>> fieldInfos;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        objectTypeDictionary = new Dictionary<Type, byte[]>();

        fieldInfos = new List<Tuple<FieldInfo, MonoBehaviour>>();
        AllObjects();
    }

    /// <summary>
    /// Creates errors if the savable byte id doesn't
    /// follow the rule of no zero bytes or
    /// throws an error if there already is that byte string in the arrow
    /// </summary>
    /// <param name="component"></param>
    /// <param name="saveableIdToAdd"></param>
    void SuperErrorChecking(MonoBehaviour component, byte[] saveableIdToAdd)
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
            SaveableId saveableId = (SaveableId)Attribute.GetCustomAttribute(component.GetType(), typeof(SaveableId));

            FieldInfo[] field = component.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance);

            byte[] saveableIdToAdd = null;

            if (saveableId != null)
            {
                saveableIdToAdd = saveableId.Id;
            }

            bool wasSaveableField = false;

            for (int i = 0; i < field.Length; i++)
            {

                FieldInfo fieldToCheck = field[i];

                Saveable newSavableAttribute = (Saveable)Attribute.GetCustomAttribute(fieldToCheck, typeof(Saveable));

                if (newSavableAttribute != null && saveableIdToAdd == null)
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

            if (saveableIdToAdd != null)
            {
                if (!wasSaveableField)
                {
                    Debug.LogError("Saveable object '" + component.name + "' has no fields to save");
                }
                else
                {
                    SuperErrorChecking(component, saveableIdToAdd);

                    objectTypeDictionary.TryAdd(component.GetType(), saveableIdToAdd);
                }
            }
        }
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
