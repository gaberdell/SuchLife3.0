using UnityEngine;

public class ConsumableItemPickup : MonoBehaviour
{
    public ConsumableItem itemData;  // Assign your asset here in Inspector

    private bool canBePickedUp = false;

    private void Start()
    {
        // Optional delay to prevent immediate pickup on spawn
        Invoke(nameof(EnablePickup), 0.5f);
    }

    private void EnablePickup()
    {
        canBePickedUp = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!canBePickedUp) return;

        PlayerInventory inventory = other.GetComponent<PlayerInventory>();
        if (inventory != null)
        {
            inventory.AddItem(itemData);  // Add the consumable item asset to the player's inventory
            Destroy(gameObject);      // Remove the pickup from the scene
        }
    }
}