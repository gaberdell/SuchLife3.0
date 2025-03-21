using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using static DataService;
using UnityEngine.UI;

public class NewWorldUi : MonoBehaviour
{
    
    void Start()
    {
        List<SaveInfo> info = DataService.Fetch();
        print(info); //debugging print statement

    }
    static void CreateWorldButton(string name, string path )
    {

    }

    void Update()
    {
        
    }
}
