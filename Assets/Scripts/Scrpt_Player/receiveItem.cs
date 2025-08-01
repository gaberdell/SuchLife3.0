using UnityEngine;
using UnityEngine.Rendering.Universal;

public static class receiveItem
{

    //scuffed placeholder script to assign type to playerinfo; should be moved into player info when assembly definitions are created for items 
    //but i didnt want to debug the web of dependencies yet
    public static PlayerItemHoldView heldItemView = GameObject.Find("ViewInHand").GetComponent<PlayerItemHoldView>();
    private static Item heldItem;
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
            playerInfo.heldType = heldItem.useFlag;
            //when held item is changed, change the renderer accordingly
            heldItemView.playerHandUpdate(heldItem);
        }
    }
}
