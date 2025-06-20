using Codice.CM.Common;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Mob : MonoBehaviour //base class for living entities
{
    private bool isInKnockback = false;
    private float knockbackForce = 0f;
    private float knockbackAcceleration = 0f;
    private Vector3 knockbackPos;

    // Update is called once per frame
    //item drop below
    public GameObject itemDropPrefab; // Drag your Apple prefab here
    public float dropChance = 0.5f; // 25% drop chance

    private void Start()
    {
        // Hook up the death event from Health to our custom Die function
        Health health = GetComponent<Health>();
        if (health != null)
        {
            health.onDeath.AddListener(Die);
        }
    }

    public void applyKnockback(Vector3 IknockbackVector, float IknockbackForce)
    {
        //set knockback vars which this mob will update to
        isInKnockback = true;
        knockbackForce = IknockbackForce;
        knockbackPos = IknockbackVector + gameObject.transform.position;
        knockbackAcceleration = IknockbackForce / 10;
    }


    public void updateKnockback()
    {
        //Debug.Log(isInKnockback);
        //update player if in knockback
        if (isInKnockback)
        {
            var step = (knockbackForce + knockbackAcceleration) * Time.deltaTime; // calculate distance to move
            knockbackForce += knockbackAcceleration;
            gameObject.transform.position = Vector3.MoveTowards(gameObject.transform.position, knockbackPos, step);

            // check if knockback is finished
            if (Vector3.Distance(gameObject.transform.position, knockbackPos) < 0.001f)
            {
                isInKnockback = false;
            }
        }
    }
    
    private void Die()
    {
        // Roll for item drop
        if (itemDropPrefab != null && Random.value <= dropChance)
        {
            Instantiate(itemDropPrefab, transform.position, Quaternion.identity);
        }
    }
}
