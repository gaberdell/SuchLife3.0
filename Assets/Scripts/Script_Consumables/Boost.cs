using UnityEngine;

public class Boost : MonoBehaviour
{
    public float damageBoostAmount = 10f;
    public float boostDuration = 5f;

    private bool canBePickedUp = false;  // Add this flag

    private void Start()
    {
        // Delay before player can pick it up (0.5 seconds, adjust as needed)
        Invoke(nameof(EnablePickup), 0.5f);
    }

    private void EnablePickup()
    {
        canBePickedUp = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!canBePickedUp) return;  // Ignore collisions until delay ends

        PlayerAttack playerAttack = other.GetComponent<PlayerAttack>();
        if (playerAttack != null)
        {
            // Apply the damage boost to the player
            playerAttack.ApplyDamageBoost(damageBoostAmount, boostDuration);

            Destroy(gameObject);
        }
    }
}