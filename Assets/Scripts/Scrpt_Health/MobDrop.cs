using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class DropItem
{
    public GameObject itemPrefab;
    [Range(0f, 1f)] public float dropChance; // This is a relative weight, not strict probability.
}

public class MobDrop : MonoBehaviour
{
    public List<DropItem> dropItems;

    void Start()
    {
        Health health = GetComponent<Health>();
        if (health != null)
        {
            health.onDeath.AddListener(AttemptDrop);
            Debug.Log($"[MobDrop] Subscribed to onDeath on {gameObject.name}");
        }
        else
        {
            Debug.LogError($"[MobDrop] No Health component on {gameObject.name}");
        }
    }

    public void AttemptDrop()
    {
        Debug.Log($"[MobDrop] AttemptDrop called on {gameObject.name}");

        if (dropItems == null || dropItems.Count == 0)
        {
            Debug.LogWarning("No drop items assigned!");
            return;
        }

        // Calculate total weight
        float totalWeight = 0f;
        foreach (var dropItem in dropItems)
        {
            totalWeight += dropItem.dropChance;
        }

        if (totalWeight <= 0f)
        {
            Debug.LogWarning("Total drop chance is zero or negative.");
            return;
        }

        // Pick a random value in range [0, totalWeight)
        float roll = Random.Range(0f, totalWeight);

        float cumulative = 0f;
        foreach (var dropItem in dropItems)
        {
            cumulative += dropItem.dropChance;
            if (roll <= cumulative)
            {
                if (dropItem.itemPrefab != null)
                {
                    Instantiate(dropItem.itemPrefab, transform.position, Quaternion.identity);
                    Debug.Log($"[MobDrop] Dropped {dropItem.itemPrefab.name} from {gameObject.name}");
                }
                else
                {
                    Debug.LogWarning("[MobDrop] Drop item prefab is null.");
                }
                return; // Exit after dropping one item
            }
        }
    }
}