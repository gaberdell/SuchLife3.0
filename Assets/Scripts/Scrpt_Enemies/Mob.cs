using Codice.CM.Common;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;
using System.Collections;     // For IEnumerator
using Pathfinding;

public class Mob : MonoBehaviour
{
    protected GameObject objectInScene; // Can be optional if you just use transform
    private int health;
    protected Vector3 worldPos;
    protected Vector2 chunkPos;

    private Rigidbody2D rb;
    private AIPath path;
    private bool isKnockedBack = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        path = GetComponent<AIPath>();  // get AIPath attached to same object
        if (rb == null)
            Debug.LogError("Rigidbody2D missing on Mob!");
        if (path == null)
            Debug.LogWarning("AIPath missing on Mob!");
    }

    public void applyKnockback(Vector3 knockbackVector, float knockbackForce)
    {
        if (rb == null) return;

        if (path != null)
            path.enabled = false;  // Disable AIPath during knockback

        isKnockedBack = true;

        rb.linearVelocity = Vector2.zero;
        Vector2 knockbackDir = new Vector2(knockbackVector.x, knockbackVector.y).normalized;
        rb.AddForce(knockbackDir * knockbackForce, ForceMode2D.Impulse);

        // Start coroutine to re-enable AIPath after knockback
        StartCoroutine(ReenableAIPathAfterDelay(0.5f)); // adjust duration as needed
    }
    private IEnumerator ReenableAIPathAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        isKnockedBack = false;
        if (path != null)
            path.enabled = true;
    }
    private void FixedUpdate()
    {
        if (rb != null)
        {
            Debug.Log($"Current velocity: {rb.linearVelocity}");
        }
    }

    public void updateChunkPos()
    {
        Vector3 currentPos = objectInScene != null ? objectInScene.transform.position : transform.position;

        Vector2 currChunkPos = ChunkManager.getChunkPosFromWorld(currentPos);

        if (currChunkPos != chunkPos)
        {
            ChunkManager.updateEntityPos(chunkPos, currChunkPos, objectInScene != null ? objectInScene : gameObject);
            chunkPos = currChunkPos;
        }
    }
}



/*
using Codice.CM.Common;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Mob : MonoBehaviour //base class for living entities
{
    private bool isInKnockback = false;
    private float knockbackForce = 0f;
    private float knockbackAcceleration = 0f;
    private Vector3 knockbackPos;
    protected GameObject objectInScene; //gameobject associated with this instance of an entity
    private int health;
    protected Vector3 worldPos;
    protected Vector2 chunkPos;

    // Update is called once per frame

    //public Mob(Vector3 pos)
    //{
    //    worldPos = pos;
    //}

    //places the mob into the scene
    //public void instantiate(GameObject enemyPrefab)
    //{
    //    if (objectInScene == null)
    //    {
    //        objectInScene = Instantiate(enemyPrefab, worldPos, Quaternion.identity);
    //    }
    //    else
    //    {
    //        Instantiate(objectInScene);
    //    }
    //}

    public void applyKnockback(Vector3 IknockbackVector, float IknockbackForce)
    {
        //set knockback vars which this mob will update to
        isInKnockback = true;
        knockbackForce = IknockbackForce;
        knockbackPos = IknockbackVector + gameObject.transform.position;
        knockbackAcceleration = IknockbackForce / 10;
    }

    public void updateChunkPos()
    {
        Vector2 currChunkPos = ChunkManager.getChunkPosFromWorld(objectInScene.transform.position);
        //Debug.Log(currChunkPos);
        //if detect a change in chunks
        if (currChunkPos != chunkPos)
        {
            //ChunkManager.renderPlayerChunks(transform.position);
            ChunkManager.updateEntityPos(chunkPos, currChunkPos, objectInScene);
            chunkPos = currChunkPos;
        }
    }

    public void updateKnockback()
    {
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
}
*/