using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using static DataService;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using TMPro;
/* Foundation of the UI used for the World Creator scene. Involves the start of saving logic
 * being implemented into the functionality of the buttons in this scene. Ideally, save will be
 *  called once a world is created. Also, eventually the script will pull a list of saved worlds to 
 * populate a list on the scene. Scene utilizes a TMPro input field to get the name of the world
 * so that it can be properly saved. Create World button currently calls save after loading the test scene.
*/
public class NewWorldUi : MonoBehaviour
{

    [SerializeField] 
    private static string inputText;
    [SerializeField]
    private GameObject loadCanvas;

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
    //Takes the text entered into the TMPro input field
    public void GrabFromInputField(string input)
    {
        inputText = input;
        Debug.Log(inputText);

    }


    public void CreateWorldButton()
    {
        loadCanvas.SetActive(true);

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
