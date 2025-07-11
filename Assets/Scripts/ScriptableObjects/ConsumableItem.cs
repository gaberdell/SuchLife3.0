using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "Consumable Item", menuName = "Scriptable Objects/Potion")]
public class ConsumableItem : Item
{
    public int healAmount;
    public string description;
}