using UnityEngine;

public class Pickup : MonoBehaviour
{
    public Item itemData; // Or ConsumableItem if you're only doing consumables
    public float pullRadius = 2f;
    public float pickupRadius = 0.3f;
    public float moveSpeed = 5f;

    private Transform targetPlayer;
    private bool isBeingPulled = false;
    private bool canBePickedUp = false;



    private void Start()
    {
        // Ensure item appears below the player visually
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            sr.sortingOrder = -1;
        }

        // Prevent pickup for a brief moment after spawning
        Invoke(nameof(EnablePickup), 0.5f);
    }

    private void EnablePickup()
    {
        canBePickedUp = true;
    }

    private void Update()
    {
        if (!canBePickedUp) return;

        if (!isBeingPulled)
        {
            // Check for player in pull radius
            Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, pullRadius);
            foreach (var hit in hits)
            {
                if (hit.CompareTag("Player"))
                {
                    targetPlayer = hit.transform;
                    isBeingPulled = true;
                    break;
                }
            }
        }

        if (isBeingPulled && targetPlayer != null)
        {
            // Move item toward the player
            transform.position = Vector3.MoveTowards(
                transform.position,
                targetPlayer.position,
                moveSpeed * Time.deltaTime
            );

            // If close enough, pick up
            if (Vector3.Distance(transform.position, targetPlayer.position) <= pickupRadius)
            {
                PlayerInventory inventory = targetPlayer.GetComponent<PlayerInventory>();
                if (inventory != null)
                {
                    inventory.AddItem(itemData); // Add to inventory
                    Destroy(gameObject);         // Remove from world
                }
            }
        }
    }

    // Optional: show radius in scene view
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, pullRadius);
    }
}