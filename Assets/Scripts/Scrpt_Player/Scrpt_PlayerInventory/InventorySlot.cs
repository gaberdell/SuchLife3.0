using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    public int quantity;
    public bool isEmpty; 
    public Item item; 
  
    void Awake(){
        //Set all of this to zero when you initiate.
        isEmpty = true; 
        quantity = 0; 
        item = null; 

    }


}
