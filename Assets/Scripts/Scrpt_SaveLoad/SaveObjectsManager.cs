using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Reflection;
using System.Collections.Generic;

public class SaveObjectsManager : MonoBehaviour
{
    public bool joe = false;

    List<Tuple<object,string>> stuffToKeepTrackOff;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        stuffToKeepTrackOff = new List<Tuple<object, string>>();
        AllObjects();
    }


    void AllObjects()
    {
        Scene curScene = SceneManager.GetActiveScene();
        GameObject[] rootGameObjects = curScene.GetRootGameObjects();

        foreach (GameObject rootGameObject in rootGameObjects)
        {
            foreach (MonoBehaviour component in rootGameObject.GetComponents<MonoBehaviour>())
            {
                MemberInfo[] member = component.GetType().GetMembers(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance);
                for (int i = 0; i < member.Length; i++)
                {
                    Saveable newSavableAttribute = (Saveable) Attribute.GetCustomAttribute(member[i], typeof(Saveable));

                    if (newSavableAttribute != null && member[i].MemberType == MemberTypes.Field)
                    {
                        Debug.Log(member[i].Name);

                        Debug.Log(member[i].MemberType);

                        object propertyValue = ((FieldInfo)member[i]).GetValue(component);
                        stuffToKeepTrackOff.Add(new Tuple<object, string>(propertyValue, rootGameObject.name));
                        Debug.Log("Value of this object : " + propertyValue.ToString());

                        Debug.Log("Name of parent : " + rootGameObject.name);
                    }
                }

                //newSavableAttribute.
            }
        }

        //gameObject[] allGameObjects = GameObject.FindObjectsByType(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        foreach (var item in stuffToKeepTrackOff)
        {
            Debug.Log("Value of this object : " + item.Item1);

            Debug.Log("Name of parent : " + item.Item2);
        }
    }
}
