using System.Collections;
using NUnit.Framework.Constraints;
using Pathfinding;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Bomb : Mob
{
    [SerializeField] private CircleCollider2D enemyCollider;
    [SerializeField] private AIPath path;
    [SerializeField] private Animator animator;
    [SerializeField] private ParticleSystem particles;

    private Transform target;
    [SerializeField] private float aggroDistance;
    [SerializeField] private float explodeAtDistance;
    [SerializeField] private float explosionRadius;
    [SerializeField] private float explosionDelay;
    [SerializeField] private int damage;

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
        target = GameObject.Find("Player").transform;
        objectInScene = gameObject;
        //set starting chunk
        chunkPos = ChunkManager.getChunkPosFromWorld(objectInScene.transform.position);

    }

    void Update()
    {
        updateKnockback(); //inherited from mob parent
        updateChunkPos(); //inherited from mob parent; ideally put in update method shared by all mobs

        distance = Vector2.Distance(transform.position, target.position);

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
            StartCoroutine(Explode());
        }
    }

    IEnumerator Explode()
    {
        animator.SetBool("Explode", true);

        yield return new WaitForSeconds(explosionDelay);

        
        enemyCollider.isTrigger = true;
        path.enabled = false;
        animator.Play("scrombolo_bombolo_exploding");
        // collider.radius = explosionRadius;

        yield return new WaitForSeconds(.35f);

        particles.Play();
        //damage nearby entities on explosion
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, explosionRadius);
        foreach (var hit in hits)
        {
            Health health = hit.GetComponent<Health>();
            if (health != null)
            {
                health.TakeDamage(damage); 
            }
        }

        //destroy nearby wall tiles on explosion
        // int explosionBlockRadius = 2;
        //get pos of tile entity is standing on
        int tileX = (int)gameObject.transform.position.x;
        int tileY = (int)gameObject.transform.position.y;
        Vector3Int bombTilePos = blockTilemap.WorldToCell(new Vector3Int(tileX, tileY, 0));
        //erase tiles T around P
        //XXXXTXXXX
        //XXXTTTXXX
        //XXTTPTTXX 
        //XXXTTTXXX
        //XXXXTXXXX
        int explosionBlockRadius = (int)Mathf.Ceil(explosionRadius);
        spreadToNeighbors(blockTilemap, bombTilePos.x, bombTilePos.y, null, explosionBlockRadius);

        // Trigger death event manually so any listeners (like MobDrop) react:
        Health mobHealth = GetComponent<Health>();
        if (mobHealth != null)
        {
            Debug.Log($"[Bomb] Invoking onDeath on {gameObject.name}");
            mobHealth.onDeath?.Invoke();
        }

        yield return new WaitForSeconds(3f);


        // onExplode?.Invoke(collider);


        Destroy(gameObject);
    }

    //ported and modified from renderDungeon
    void spreadToNeighbors(Tilemap tilemap, int tileX, int tileY, Tile T, int iterations)
    {
        //given an xPos, yPos, tilemap, and tile, change all adjacent tiles to match the given tile
        tilemap.SetTile(new Vector3Int(tileX + 1, tileY, 0), T);
        tilemap.SetTile(new Vector3Int(tileX - 1, tileY, 0), T);
        tilemap.SetTile(new Vector3Int(tileX, tileY + 1, 0), T);
        tilemap.SetTile(new Vector3Int(tileX, tileY - 1, 0), T);
        //recurse outwards; a little redundant but fine for small scale
        if (iterations > 0)
        {
            spreadToNeighbors(tilemap, tileX + 1, tileY, T, iterations - 1);
            spreadToNeighbors(tilemap, tileX - 1, tileY, T, iterations - 1);
            spreadToNeighbors(tilemap, tileX, tileY + 1, T, iterations - 1);
            spreadToNeighbors(tilemap, tileX, tileY - 1, T, iterations - 1);
        }

    }
}
