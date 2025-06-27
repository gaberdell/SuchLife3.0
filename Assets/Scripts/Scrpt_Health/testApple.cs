using UnityEngine;

public class Apple : MonoBehaviour
{
    public int healAmount = 50;

    private bool canBePickedUp = false;  // **Add this flag**

    private void Start()
    {
        // Delay before player can pick it up (0.5 seconds, you can adjust)
        Invoke(nameof(EnablePickup), 0.5f);
    }

    private void EnablePickup()
    {
        canBePickedUp = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!canBePickedUp) return;  // **Ignore collisions until delay ends**

        Health health = other.GetComponent<Health>();
        if (health != null)
        {
            health.Heal(healAmount);
            Destroy(gameObject);
        }
    }
}