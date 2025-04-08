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

    [SerializeField] private static string inputText;

    void Start()
    {
        List<SaveInfo> info = DataService.Fetch();
        print(info); //debugging print statement
        foreach(SaveInfo s in info)
        {
           string worldPath = s.path; //protection level on path and name are restricted
           string worldName = s.name;
           DateTime worldDate = s.lastModified; 
        
        //use the above to populate a clickable button corresponding to a saved world

        }

    }

    public void GrabFromInputField(string input)
    {
        inputText = input;
        Debug.Log(inputText);


        //DisplayReactionToInput();

    }


    public void CreateWorldButton()
    {
        Debug.Log("Creating world...");

        //load into world before saving
        SceneManager.LoadScene("TestScene");

        //use inputField text to create world name
        string name = inputText;


        Debug.Log("Input text: " + name); //debugging print statement

        //save game after creating world
        DataService.NewSave(name);
    }

    
    void Update()
    {

    }
}
