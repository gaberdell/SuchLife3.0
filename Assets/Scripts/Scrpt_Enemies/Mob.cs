using UnityEngine;
using System.Collections;
using UnityEngine.Tilemaps;     // For IEnumerator

public class Mob : MonoBehaviour
{
    protected GameObject objectInScene; // Can be optional if you just use transform
    protected int health;
    protected Vector3 worldPos;
    protected Vector2 chunkPos;

    //pathing
    protected enum AggroType
    {
        Passive, //will never attack player; only moves randomly
        Reactive, //passive but turns hostile when attacked
        Hostile //has a detection radius; move towards player if enters detection radius;
    }
    [SerializeField] protected AggroType Atype;
    [SerializeField] protected float aggroDistance = 1;
    [SerializeField] protected CircleCollider2D enemyCollider;
    [SerializeField] protected int damage = 10;
    protected Rigidbody2D rb;
    protected PathingController pathController;
    protected Transform target;
    float speed = 1;

    protected EnemyAttack enemyAttack;

    protected Tilemap blockTilemap;
    protected string tileMapName = "PlaceableTileMap";

    private bool isKnockedBack = false;
    protected bool isDead = false;

    private void Awake()
    {
        target = GameObject.Find("Sign").transform;
        rb = GetComponent<Rigidbody2D>();
        pathController = new PathingController((int)transform.position.x, (int)transform.position.y, this.transform, null, 0);
        pathController.setTarget(target);
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
        //progress pathfinding
        pathController.moveAlongPath(speed);

        if (rb != null)
        {
            //Debug.Log($"Current velocity: {rb.linearVelocity}");
        }
    }

    public void updateChunkPos()
    {
        Vector3 currentPos = objectInScene != null ? objectInScene.transform.position : transform.position;
        //Debug.Log("current mob pos: " + currentPos);
        //Debug.Log("object in scene: " + objectInScene);
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