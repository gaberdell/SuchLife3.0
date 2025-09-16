using System.Net.Sockets;
using UnityEngine;

public static class playerInfo 
{
    //data class to store attributes concerning the player so multiple scripts can access
    //put attributes like health in here instead of the health class

    //held item info
    public static GameObject player = GameObject.Find("Player"); //reference to player object so only has to be done once
    public static string heldType;
    private static Item heldItem;
    public static int heldItemIndex = 0;
    public static PlayerItemHoldView heldItemView = GameObject.Find("ViewInHand").GetComponent<PlayerItemHoldView>();
    
    public static Item HeldItem
    {
        get
        {
            return heldItem;
        }

        set
        {
            //Debug.Log("change held item!");
            heldItem = value;
            //when held item is changed, change the renderer accordingly
            heldItemView.playerHandUpdate(heldItem);
        }
    }


}
