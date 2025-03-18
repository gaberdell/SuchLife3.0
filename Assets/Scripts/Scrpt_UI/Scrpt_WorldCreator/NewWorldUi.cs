using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using static DataService;
using UnityEngine.UI;

public class NewWorldUi : MonoBehaviour
{
    
    void Start()
    {
        GameObject datamgnt = GameObject.Find("DataMgnt");
        DataService dataservice = datamgnt.GetComponent<DataService>(); 
        List<SaveInfo> info = dataservice.Fetch();
        print(info); //debugging print statement

    }
    static void CreateWorldButton(string name, string path )
    {

    }

    void Update()
    {
        
    }
}
