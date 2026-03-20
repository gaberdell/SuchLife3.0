using System.Net.Sockets;
using UnityEngine;

public class PlayerInfo : MonoBehaviour
{
    //data class to store attributes concerning the player so multiple scripts can access
    //put attributes like health in here instead of the health class

    //held item info
    public GameObject Player { get; private set; }
    private string username;
    private string heldType;
    private Item heldItem;
    public int heldItemIndex = 0;
    public PlayerItemHoldView heldItemView;

    private void Start()
    {
        Player = gameObject;
        //heldItemView = GameObject.Find("ViewInHand").GetComponent<PlayerItemHoldView>();
    }
    public Item HeldItem
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
