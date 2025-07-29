using System.Net.Sockets;
using UnityEngine;

public static class playerInfo 
{
    //data class to store attributes concerning the player so multiple scripts can access
    public static PlayerItemHoldView heldItemView = GameObject.Find("ViewInHand").GetComponent<PlayerItemHoldView>();
    private static Item heldItem;
    //put attributes like health in here instead of the health class



    public static Item HeldItem
    { 
        get
        {
            return heldItem;
        }

        set
        {
            Debug.Log("change held item!");
            heldItem = value;
            //when held item is changed, change the renderer accordingly
            heldItemView.playerHandUpdate(heldItem);
        }
    }

}
