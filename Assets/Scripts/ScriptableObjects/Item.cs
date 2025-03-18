using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


[CreateAssetMenu(fileName = "Item", menuName = "Scriptable Objects/Item")]
[System.Serializable]
public class Item : ScriptableObject
{
    public string itemName; 
    public Sprite icon; 

    [TextArea(3, 10)]
    public string[] information; 


}
