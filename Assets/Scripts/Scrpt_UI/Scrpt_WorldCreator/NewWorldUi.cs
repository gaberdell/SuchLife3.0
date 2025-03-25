using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using static DataService;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class NewWorldUi : MonoBehaviour
{
    private string input;

    void Start()
    {
        List<SaveInfo> info = DataService.Fetch();
        print(info); //debugging print statement
      /*  foreach(SaveInfo s in info)
        {
           string worldPath = s.path;
           string worldName = s.name;
           DateTime worldDate = s.lastModified;
        }*/

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
       //enter world name
   
        //load into world before saving
        SceneManager.LoadScene("TestScene");

        //save game after creating world
        
        string savePath = DataService.GetSavePath() + name.ToLower();
        DataService.Save(savePath);
        
    }
    public void InputToString(string s)
    {
        input = s;
    }
     private void SubmitName(string arg0)
     {
            Debug.Log(arg0);
     }
    void Update()
    {
        
    }
}
