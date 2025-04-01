using System.Collections;
using Pathfinding;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Bomb : MonoBehaviour
{
    public SpriteRenderer renderer;
    public CircleCollider2D collider;
    public AIPath path;
    public Animator animator;
    public ParticleSystem particles;

    public Transform target;
    public float aggroDistance;
    public float explodeAtDistance;
    public float explosionRadius;
    public float explosionDelay;
    public int damage;

    private float distance;
    private bool isExploding = false;

    public Tilemap blockTilemap;

    // public delegate void OnExplode(Collider2D explosionCollider);
    // public static event OnExplode onExplode;

    void Start()
    {
        target = GameObject.Find("Player").transform;
        blockTilemap = GameObject.Find("PlaceableTileMap").GetComponent<Tilemap>();
    }

    void Update()
    {
        distance = Vector2.Distance(transform.position, target.position);

        if (distance <= aggroDistance && !isExploding)
        {
            path.enabled = true;
        }

        if (distance <= explodeAtDistance && !isExploding)
        {
            isExploding = true;  
            StartCoroutine(Explode());
        }
    }

    IEnumerator Explode()
    {
        animator.enabled = true;
        yield return new WaitForSeconds(explosionDelay);

        particles.Play();
        path.enabled = false;
        renderer.enabled = false;
        collider.isTrigger = true;
        // collider.radius = explosionRadius;

        yield return new WaitForSeconds(0.05f);
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
        //int explosionBlockRadius = 2;
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
        int explosionBlockRadius = (int) Mathf.Ceil(explosionRadius);
        spreadToNeighbors(blockTilemap, bombTilePos.x, bombTilePos.y, null, explosionBlockRadius);
        

        yield return new WaitForSeconds(0.5f);

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
        if(iterations > 0)
        {
            spreadToNeighbors(tilemap, tileX + 1, tileY, T, iterations - 1);
            spreadToNeighbors(tilemap, tileX - 1, tileY, T, iterations - 1);
            spreadToNeighbors(tilemap, tileX, tileY + 1, T, iterations - 1);
            spreadToNeighbors(tilemap, tileX, tileY - 1, T, iterations - 1);
        }
        
    }

}
