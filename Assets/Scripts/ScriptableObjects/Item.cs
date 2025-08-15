using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "Item", menuName = "Scriptable Objects/Item")]
public class Item : ScriptableObject
{
    public string itemName;
    public int useFlag = 0; //corresponds to inputhandler enum; 0 = None, 1 = Tool, 2 = Block, 3 = Consumable
    public Sprite icon;
    public TileBase tileThisPlaces;

    [TextArea(3, 10)]
    public string[] information;
}