// Health.cs
using UnityEngine;
using UnityEngine.Events;


public class Health : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth;

    public bool showHealthText = false;
    private TextMesh healthTextMesh;

    public UnityEvent onDeath;
    public UnityEvent<int> onDamageTaken;

    private void Awake()
    {

        if (showHealthText)
        {
            CreateHealthText();
            UpdateHealthText();
        }
    }

    public void TakeDamage(int amount, bool applyKnockback = true)
    {
        currentHealth -= amount;
        currentHealth = Mathf.Max(currentHealth, 0);

        onDamageTaken?.Invoke(amount);

        if (showHealthText)
        {
            UpdateHealthText();
        }

        // ✅ Only apply knockback if explicitly allowed
        if (applyKnockback)
        {
            Rigidbody2D rb = GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                Vector3 knockbackDir = Vector3.zero; // TODO: set a proper direction
                rb.AddForce(knockbackDir * 3f, ForceMode2D.Impulse);
            }
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Heal(int amount)
    {
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);

        if (showHealthText)
        {
            UpdateHealthText();
        }
    }

    private void Die()
    {
        Debug.Log($"Health.Die() called on {gameObject.name}");
        onDeath?.Invoke();

        // Destroy with delay instead of instantly
        Destroy(gameObject, 0.1f);
    }

    private void CreateHealthText()
    {
        GameObject textObj = new GameObject("HealthText");
        textObj.transform.SetParent(transform);
        //move health text away from entity according to its radius
        float textRadius = gameObject.GetComponent<CircleCollider2D>().radius + 0.5f;
        textObj.transform.localPosition = new Vector3(0, textRadius, 0); 

        healthTextMesh = textObj.AddComponent<TextMesh>();
        healthTextMesh.fontSize = 32;
        healthTextMesh.color = Color.red;
        healthTextMesh.alignment = TextAlignment.Center;
        healthTextMesh.anchor = TextAnchor.MiddleCenter;
        healthTextMesh.characterSize = 0.1f;
    }

    private void UpdateHealthText()
    {
        if (healthTextMesh != null)
        {
            healthTextMesh.text = currentHealth + " / " + maxHealth;
        }
    }

}
