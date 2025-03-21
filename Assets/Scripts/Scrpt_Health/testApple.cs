using UnityEngine;

public class Apple : MonoBehaviour
{
    public int healAmount = 50; 

    private void OnTriggerEnter2D(Collider2D other)
    {
        Health health = other.GetComponent<Health>();
        if (health != null)
        {
            health.Heal(healAmount);  
            Destroy(gameObject);  
        }
    }

}
