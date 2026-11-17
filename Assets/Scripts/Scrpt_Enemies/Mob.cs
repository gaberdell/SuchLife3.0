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
        Reactive, //passive but starts chasing the player when attacked
        Hostile, //has a detection collider; move towards player if enters detection radius;
        None //doesnt move at all
    }
    [SerializeField] protected AggroType Atype = AggroType.None;
    //[SerializeField] protected float aggroDistance = 1; - DETERMINED BY RADIUS OF AGGROCOLLIDER
    [SerializeField] protected CircleCollider2D enemyCollider;
    [SerializeField] protected CircleCollider2D aggroCollider; //for hostile enemy types
    [SerializeField] protected int damage = 10;
    [SerializeField] protected private Animator animator;
    [SerializeField] protected private ParticleSystem particles;
    protected Rigidbody2D rb;
    protected PathingController pathController;
    [SerializeField] float speed = 1;
    protected Transform target = null; //keep track of target because target moves 

    protected EnemyAttack enemyAttack;

    protected Tilemap blockTilemap;
    protected string tileMapName = "PlaceableTileMap";

    private bool isKnockedBack = false;
    protected bool isDead = false;

    private void Awake()
    {        
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        pathController = new PathingController((int)transform.position.x, (int)transform.position.y, this.transform, null, 0);
        pathController.setAnimator(animator);
        pathController.setActive(false);
        
        //set chunk pos on spawn
        chunkPos = ChunkManager.getChunkPosFromWorld(objectInScene != null ? objectInScene.transform.position : transform.position);

        //for pathfinding
        Health health = GetComponent<Health>();
        

        //pathfinding init states
        if(Atype == AggroType.Reactive)
        {
            health.onDamageTaken.AddListener(OnDamageTaken);
        }
        if(Atype == AggroType.Reactive || Atype == AggroType.Passive)
        {
            InvokeRepeating(nameof(randomTargetPos), 0.1f, 3f);
        }
    }

    //for reactive mobs
    void OnDamageTaken(int damageAmount, GameObject attacker)
    {
        bool isNotNull = attacker != null;
        bool isPlayer = isNotNull && attacker.CompareTag("Player");

        Debug.Log($"[Fox] OnDamageTaken: damage={damageAmount}, attacker!=null={isNotNull}, attackerName={(attacker?.name ?? "NULL")}, CompareTag(Player)={isPlayer}");
        if (attacker != null && attacker.CompareTag("Player"))
        {
            Debug.Log($"[Fox] OnDamageTaken called! Damage: {damageAmount}, Attacker: {attacker?.name}");
            //stop random movement and start chasing player
            CancelInvoke(nameof(randomTargetPos));
            target = attacker.transform;
            pathController.setTarget(attacker.transform.position);
            pathController.setActive(true);

            // Set the attack target flag in EnemyAttack
            if (enemyAttack != null)
            {
                enemyAttack.hasAttackTarget = true;
            }
        }
    }

    //for reactive/passive mobs
    void randomTargetPos()
    {
        Vector3 pos = new Vector3();
        pos.x = Random.Range(-1, 2);
        pos.y = Random.Range(-1, 2);
        pos.z = 0f;
        Debug.Log(pos);
        pathController.setTarget(pos + this.transform.position);
        pathController.setActive(true);
        //target.position = pos;

    }

    //for hostile mobs

    private void OnTriggerEnter2D(Collider2D other)
    {
        //if player enters collider while mob is inactive then start chasing that player
        if (Atype == AggroType.Hostile && !target)
        {
            if (other.CompareTag("Player"))
            {
                target = other.transform;
                pathController.setTarget(other.transform.position);
                pathController.setActive(true);
            }
        }

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
    }

    //pathfinding methods



    private void FixedUpdate()
    {
        //check if path needs to be updated
        if(target && pathController.getTarget() != target.position)
        {
            pathController.setTarget(target.position);
            pathController.setActive(true);
        }
        if (pathController.isActive)
        {
            //progress pathfinding
            pathController.moveAlongPath(speed);
        }
        updateChunkPos();


        if (rb != null)
        {
            //Debug.Log($"Current velocity: {rb.linearVelocity}");
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