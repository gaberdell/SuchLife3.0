using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using static DataService;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class NewWorldUi : MonoBehaviour
{
    private string input;
    public InputField inputfield;

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
    /*  void OnEnable()
      {
          EventManager.Clicked += CreateWorldButton;
      }

      void OnDisable()
      {
          EventManager.Clicked -= CreateWorldButton;

      }*/

    public void CreateWorldButton()
    {
        
        //load into world before saving
        SceneManager.LoadScene("TestScene");

        //use inputField text to create world name
        string name = inputfield.text;


        Debug.Log("Input text: " + name); //debugging print statement

        //save game after creating world
        string savePath = DataService.GetSavePath() + name.ToLower();
        DataService.Save(savePath);
    }

    
    void Update()
    {

    }
}
