/*using UnityEngine;

public class PickupItem : MonoBehaviour
{
    public Item itemData; // Assign your health_potion asset in Inspector

    private bool canBePickedUp = false;

    private void Start()
    {
        Invoke(nameof(EnablePickup), 0.5f); // Delay to prevent instant pickup
    }

    private void EnablePickup()
    {
        canBePickedUp = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!canBePickedUp) return;

        if (other.CompareTag("Player"))
        {
            bool added = PlayerInventory.Instance.AddItem(itemData);
            if (added)
            {
                Debug.Log("Picked up: " + itemData.itemName);
                Destroy(gameObject);
            }
            else
            {
                Debug.Log("Inventory full!");
            }
        }
    }
}
*/