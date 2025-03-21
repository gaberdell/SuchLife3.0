using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using static DataService;
using UnityEngine.UI;

public class NewWorldUi : MonoBehaviour
{
    static DataService dataservice = null;


    void Start()
    {
        GameObject datamgnt = GameObject.Find("DataMgnt");
        DataService dataservice = datamgnt.GetComponent<DataService>(); 
        List<SaveInfo> info = dataservice.Fetch();
        print(info); //debugging print statement

    }
    static void CreateWorldButton(string name)
    {
        //load into world before saving

        //save game after creating world
        if (dataservice != null){ 
            string savePath = dataservice.GetSavePath() + name.ToLower();
            dataservice.Save(savePath);
        }
    }

    void Update()
    {
        
    }
}
