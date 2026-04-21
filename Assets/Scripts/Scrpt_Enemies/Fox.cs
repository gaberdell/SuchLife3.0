using System.Collections;
using NUnit.Framework.Constraints;
using Pathfinding;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Fox : Mob
{
    [SerializeField] private CircleCollider2D enemyCollider;
    //[SerializeField] private AIPath path;
    [SerializeField] private Animator animator;
    [SerializeField] private ParticleSystem particles;
    private EnemyAttack enemyAttack;

    private Transform target;
    [SerializeField] private float aggroDistance;
    [SerializeField] private float explodeAtDistance;
    [SerializeField] private float explosionRadius;
    [SerializeField] private float explosionDelay;
    [SerializeField] private int damage;
    private AIDestinationSetter pathSetter;

    private float distance;
    private bool isExploding = false;

    [SerializeField] private Tilemap blockTilemap;
    [SerializeField] private string tileMapName = "PlaceableTileMap"; //Done by goat Kcrushel
    // public delegate void OnExplode(Collider2D explosionCollider);
    // public static event OnExplode onExplode;


    //public Bomb(Vector3 pos)
    //{
    //    //
    //    worldPos = pos;
    //}
    void Start()
    {
        blockTilemap = blockTilemap != null ? blockTilemap : GameObject.Find(tileMapName).GetComponent<Tilemap>();
        pathSetter = GetComponent<AIDestinationSetter>();
        objectInScene = gameObject;

        GameObject targetObj = new GameObject();
        target = targetObj.transform;
        target.position = new Vector3(0, 0, 0);
        pathSetter.target = target;
        enemyAttack = GetComponent<EnemyAttack>(); // later set target if get attacked
        //set starting chunk
        chunkPos = ChunkManager.getChunkPosFromWorld(objectInScene.transform.position);
        //add a clock to trigger randomMovement()
        InvokeRepeating(nameof(randomTargetPos),0.1f,3f);

        // Listen on health changes to trigger getAttacked()
        Health health = GetComponent<Health>();
        health.onDamageTaken.AddListener(OnDamageTaken); 
    }
    

    void Update()
    {
        //updateKnockback(); //inherited from mob parent
        updateChunkPos(); //inherited from mob parent; ideally put in update method shared by all mobs

        distance = Vector2.Distance(transform.position, target.position);

        if  (pathSetter.target != target)
        {
            pathSetter.target = target;
        }
        if (distance <= aggroDistance && !isExploding)
        {
            path.enabled = true;
        }

        if (path.desiredVelocity.magnitude > 0) {
            animator.SetBool("Walk", true);
        }

        if (distance <= explodeAtDistance && !isExploding)
        {
            isExploding = true;  
            //StartCoroutine(Explode());
        }
    }

    
    void OnDamageTaken(int damageAmount, GameObject attacker){
        bool isNotNull = attacker != null;
        bool isPlayer = isNotNull && attacker.CompareTag("Player");

        Debug.Log($"[Fox] OnDamageTaken: damage={damageAmount}, attacker!=null={isNotNull}, attackerName={(attacker?.name ?? "NULL")}, CompareTag(Player)={isPlayer}");
        if (attacker != null && attacker.CompareTag("Player"))
        {
            Debug.Log($"[Fox] OnDamageTaken called! Damage: {damageAmount}, Attacker: {attacker?.name}");
            //stop random movement and start chasing player
            CancelInvoke(nameof(randomTargetPos));
            target = attacker.transform;
            pathSetter.target = target;
            path.enabled = true;

            // Set the attack target flag in EnemyAttack
            if (enemyAttack != null)
            {
                enemyAttack.hasAttackTarget = true;
            }
        }
    }

    void randomTargetPos()
    {
        Vector3 pos = new Vector3();
        pos.x = Random.Range(-3f, 3f);
        pos.y = Random.Range(-3f, 3f);
        pos.z = 0f;
        target.position = pos;
        
    }

    void getAttacked()
    {
        
    }

    // IEnumerator Explode()
    // {
    //     animator.SetBool("Explode", true);

    //     yield return new WaitForSeconds(explosionDelay);

    //     enemyCollider.isTrigger = true;
    //     path.enabled = false;
    //     animator.Play("scrombolo_bombolo_exploding");

    //     yield return new WaitForSeconds(.35f);

    //     particles.Play();

        
    //     Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, explosionRadius);
    //     foreach (var hit in hits)
    //     {
    //         Health health = hit.GetComponent<Health>();
    //         if (health != null)
    //         {
    //             health.TakeDamage(damage, false);
    //         }
    //     }

    //     // Destroy nearby wall tiles on explosion
    //     int tileX = (int)gameObject.transform.position.x;
    //     int tileY = (int)gameObject.transform.position.y;
    //     Vector3Int bombTilePos = blockTilemap.WorldToCell(new Vector3Int(tileX, tileY, 0));
    //     int explosionBlockRadius = (int)Mathf.Ceil(explosionRadius);
    //     spreadToNeighbors(blockTilemap, bombTilePos.x, bombTilePos.y, null, explosionBlockRadius);

    //     // Trigger death event manually so any listeners (like MobDrop) react:
    //     Health mobHealth = GetComponent<Health>();
    //     if (mobHealth != null)
    //     {
    //         Debug.Log($"[Bomb] Invoking onDeath on {gameObject.name}");
    //         mobHealth.onDeath?.Invoke();

    //         OnDeath();

    //         // Hide health bar if it still exists
    //         Transform healthBar = transform.Find("HealthBar");
    //         if (healthBar != null)
    //             healthBar.gameObject.SetActive(false);
    //     }

    //     yield return new WaitForSeconds(3f);

    //     Destroy(gameObject);
    // }

    //ported and modified from renderDungeon
    // void spreadToNeighbors(Tilemap tilemap, int tileX, int tileY, Tile T, int iterations)
    // {
    //     //given an xPos, yPos, tilemap, and tile, change all adjacent tiles to match the given tile
    //     tilemap.SetTile(new Vector3Int(tileX + 1, tileY, 0), T);
    //     tilemap.SetTile(new Vector3Int(tileX - 1, tileY, 0), T);
    //     tilemap.SetTile(new Vector3Int(tileX, tileY + 1, 0), T);
    //     tilemap.SetTile(new Vector3Int(tileX, tileY - 1, 0), T);
    //     //recurse outwards; a little redundant but fine for small scale
    //     if (iterations > 0)
    //     {
    //         spreadToNeighbors(tilemap, tileX + 1, tileY, T, iterations - 1);
    //         spreadToNeighbors(tilemap, tileX - 1, tileY, T, iterations - 1);
    //         spreadToNeighbors(tilemap, tileX, tileY + 1, T, iterations - 1);
    //         spreadToNeighbors(tilemap, tileX, tileY - 1, T, iterations - 1);
    //     }

    // }
}
