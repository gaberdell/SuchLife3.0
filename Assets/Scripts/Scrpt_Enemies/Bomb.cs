using System.Collections;
using System.Collections.Generic;
using NUnit.Framework.Constraints;
using UnityEngine;
using UnityEngine.Tilemaps;

[SaveableComponent("Scrombolo Bombolo")]
public class Bomb : Mob
{
    [SerializeField] private CircleCollider2D enemyCollider;
    //[SerializeField] private AIPath path;
    [SerializeField] private Animator animator;
    [SerializeField] private ParticleSystem particles;

    private List<Transform> targets;
    private Transform target;
    //private AIDestinationSetter pathSetter;
    [SerializeField] private float aggroDistance;
    [SerializeField] private float explodeAtDistance;
    [SerializeField] private float explosionRadius;
    [SerializeField] private float explosionDelay;
    [SerializeField] private int damage;

    public void testFunction() { }

    [Saveable("Distance")]
    private float distance;
    [Saveable("IsExploding")]
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
        targets = new List<Transform>();
        blockTilemap = blockTilemap != null ? blockTilemap : GameObject.Find(tileMapName).GetComponent<Tilemap>();
        //pathSetter = GetComponent<AIDestinationSetter>();
        target = GameObject.Find("Player").transform;
        //pathSetter.target = target;
        objectInScene = gameObject;
        //set starting chunk
        chunkPos = ChunkManager.getChunkPosFromWorld(objectInScene.transform.position);
    }

    private void OnEnable() {
        EventManager.LocalGameObjectPlayerAddedToScene += addPlayerTarget;
        EventManager.OnlinePlayerJoined += addPlayerTarget;
        EventManager.OnlinePlayerLeft += removePlayerTarget;
    }

    private void OnDisable() {
        EventManager.LocalGameObjectPlayerAddedToScene -= addPlayerTarget;
        EventManager.OnlinePlayerJoined -= addPlayerTarget;
        EventManager.OnlinePlayerLeft -= removePlayerTarget;
    }

    void addPlayerTarget(GameObject player) {
        targets.Add(player.transform);
    }

    void removePlayerTarget(GameObject player) {
        targets.Remove(player.transform);
    }

    void Update()
    {
        //updateKnockback(); //inherited from mob parent
        updateChunkPos(); //inherited from mob parent; ideally put in update method shared by all mobs


        float smallestDistance = float.MaxValue;
        foreach (Transform t in targets) {
            float newDistance = Vector2.Distance(transform.position, t.position);
            if (newDistance < smallestDistance) {
                target = t;
                smallestDistance = newDistance;
            }
        }

        //distance = Vector2.Distance(transform.position, target.position);

        if (smallestDistance <= aggroDistance && !isExploding)
        {
            //path.enabled = true;
        }

        /*if (path.desiredVelocity.magnitude > 0) {
            animator.SetBool("Walk", true);
        }*/

        if (smallestDistance <= explodeAtDistance && !isExploding)
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
        //path.enabled = false;
        animator.Play("scrombolo_bombolo_exploding");

        yield return new WaitForSeconds(.35f);

        particles.Play();

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, explosionRadius);
        foreach (var hit in hits)
        {
            if (hit.gameObject == gameObject) continue; // skip self, won't knockback self first.
            Health health = hit.GetComponent<Health>();
            if (health != null)
            {
                health.TakeDamage(damage, false);
            }
        }

        // Destroy nearby wall tiles on explosion
        int tileX = (int)gameObject.transform.position.x;
        int tileY = (int)gameObject.transform.position.y;
        Vector3Int bombTilePos = blockTilemap.WorldToCell(new Vector3Int(tileX, tileY, 0));
        int explosionBlockRadius = (int)Mathf.Ceil(explosionRadius);
        spreadToNeighbors(blockTilemap, bombTilePos.x, bombTilePos.y, null, explosionBlockRadius);

        // Trigger death event manually so any listeners (like MobDrop) react:
        Health mobHealth = GetComponent<Health>();
        if (mobHealth != null)
        {
            Debug.Log($"[Bomb] Invoking onDeath on {gameObject.name}");
            mobHealth.onDeath?.Invoke();

            OnDeath();

            // Hide health bar if it still exists
            Transform healthBar = transform.Find("HealthBar");
            if (healthBar != null)
                healthBar.gameObject.SetActive(false);
        }

        yield return new WaitForSeconds(3f);

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
