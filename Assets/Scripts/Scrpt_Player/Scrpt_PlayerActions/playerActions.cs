using UnityEngine;

public class playerActions : MonoBehaviour
{
    double useCooldown = 0;


    public void useHeldItem()
    {
        Item heldItem = playerInfo.HeldItem;
        GameObject player = gameObject;

        //switch based on item metadata
        switch (heldItem.useFlag)
        {
            case "none":
                Debug.Log("default use case");
                break;
        }

    }
    
}
