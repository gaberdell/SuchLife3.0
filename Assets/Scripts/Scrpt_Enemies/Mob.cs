using UnityEngine;
using System.Collections;     // For IEnumerator

public class Mob : MonoBehaviour
{
    protected GameObject objectInScene; // Can be optional if you just use transform
    private int health;
    protected Vector3 worldPos;
    protected Vector2 chunkPos;

    private Rigidbody2D rb;
    //protected AIPath path;
    private bool isKnockedBack = false;

    protected bool isDead = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        //path = GetComponent<AIPath>();  // get AIPath attached to same object
        if (rb == null)
            Debug.LogError("Rigidbody2D missing on Mob!");
        //if (path == null)
        //    Debug.LogWarning("AIPath missing on Mob!");
        //set chunk pos on spawn
        chunkPos = ChunkManager.getChunkPosFromWorld(objectInScene != null ? objectInScene.transform.position : transform.position);
    }

    public void applyKnockback(Vector3 knockbackVector, float knockbackForce)
    {
        if (rb == null || isDead) return;  // don't knockback dead mobs

        //if (path != null)
        //    path.enabled = false;  // Disable AIPath during knockback

        isKnockedBack = true;

        rb.linearVelocity = Vector2.zero;
        Vector2 knockbackDir = new Vector2(knockbackVector.x, knockbackVector.y).normalized;
        rb.AddForce(knockbackDir * knockbackForce, ForceMode2D.Impulse);

        StartCoroutine(ReenableAIPathAfterDelay(0.5f));
    }

    private IEnumerator ReenableAIPathAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        isKnockedBack = false;
        //if (path != null)
        //    path.enabled = true;
    }
    private void FixedUpdate()
    {
        if (rb != null)
        {
            //Debug.Log($"Current velocity: {rb.linearVelocity}");
        }
    }

    public void updateChunkPos()
    {
        Vector3 currentPos = objectInScene != null ? objectInScene.transform.position : transform.position;
        Debug.Log("current mob pos: " + currentPos);
        Debug.Log("object in scene: " + objectInScene);
        Vector2 currChunkPos = ChunkManager.getChunkPosFromWorld(currentPos);

        if (currChunkPos != chunkPos)
        {
            ChunkManager.updateEntityPos(chunkPos, currChunkPos, objectInScene != null ? objectInScene : gameObject);
            chunkPos = currChunkPos;
        }
    }

    public void OnDeath()
    {
        isDead = true;
        //if (path != null)
        //    path.enabled = false;
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
            rb.bodyType = RigidbodyType2D.Kinematic;  // freeze physics
        }
    }
}