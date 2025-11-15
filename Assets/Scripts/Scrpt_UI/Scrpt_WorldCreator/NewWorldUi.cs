using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using static DataService;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using TMPro;

public class NewWorldUi : MonoBehaviour
{

    [SerializeField] private static uint maxSize;
    [SerializeField]
    TextMeshProUGUI newNameText;

    List<SaveInfo> info;

    void Start()
    {
        info = DataService.Fetch();
        print(info); //debugging print statement
        foreach(SaveInfo s in info)
        {
           string worldPath = s.path; //protection level on path and name are restricted
           string worldName = s.name;
           DateTime worldDate = s.lastModified; 
        
        //use the above to populate a clickable button corresponding to a saved world

        }

        maxSize = (uint) info.Count;

    }

    private void OnEnable()
    {
        EventManager.UpdateSlotPosition += UpdateSlotPosition;
    }
    private void OnDisable()
    {
        EventManager.UpdateSlotPosition -= UpdateSlotPosition;
    }


    public void UpdateSlotPosition(string nameOfPath, uint newSlotPosition)
    {
        int findIndex = -1;
        for (int i = 0; i < info.Count; i++)
        {
            if (info[i].path == nameOfPath)
            {
                findIndex = i;
                break;
            }
        }
        if (findIndex < 0)
        {
            return;
        }
        SaveInfo newSave = info[findIndex];
        newSave.order = newSlotPosition;
        info[findIndex] = newSave;

        info.RemoveAt(findIndex);

        int insertPosition = findIndex > (int)newSlotPosition ? 1 : 0;

        info.Insert((int) newSlotPosition + insertPosition, newSave);


        //Recalc slot positions
        for (int i = insertPosition+1; i < info.Count; i++)
        {
            SaveInfo newSave2 = info[findIndex];
            newSave2.order = newSave2.order+1;
            info[findIndex] = newSave2;
        }
    }

    public void CreateWorldButton()
    {
        Debug.Log("Creating world...");

        //load into world before saving
        //SceneManager.LoadScene("TestScene");

        //use inputField text to create world name
        string name = newNameText.text;

        Debug.Log("Input text: " + name); //debugging print statement

        //save game after creating world
        DataService.NewSave(name, ++maxSize);
    }

    public void LoadIntoWorld()
    {

    }
    
    void Update()
    {

    }
}
