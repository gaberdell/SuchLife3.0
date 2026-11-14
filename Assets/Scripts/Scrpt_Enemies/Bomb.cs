using System.Collections;
using System.Collections.Generic;
using NUnit.Framework.Constraints;
using UnityEngine;
using UnityEngine.Tilemaps;

[SaveableComponent("Scrombolo Bombolo")]
public class Bomb : Mob
{   
    private List<Transform> targets;
    //private AIDestinationSetter pathSetter;
    [SerializeField] private float explodeAtDistance;
    [SerializeField] private float explosionRadius;
    [SerializeField] private float explosionDelay;

    [Saveable("Distance")]
    private float distance;
    [Saveable("IsExploding")]
    private bool isExploding = false;

    void Start()
    {
        blockTilemap = blockTilemap != null ? blockTilemap : GameObject.Find(tileMapName).GetComponent<Tilemap>();
        //pathController.setAnimator(animator);
        pathController.setSize(1);
        //pathController.toggleActive();
        targets = new List<Transform>();

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
        
        //explosion check
        if (pathController.isActive && (Mathf.Abs(Vector3.Distance(pathController.getTarget().position, transform.position))) <= explodeAtDistance && !isExploding)
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
