using System.Collections;
using Pathfinding;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Bomb : MonoBehaviour
{
    [SerializeField] private CircleCollider2D collider;
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

    // public delegate void OnExplode(Collider2D explosionCollider);
    // public static event OnExplode onExplode;

    void Start()
    {
        target = GameObject.Find("Player").transform;
    }

    void Update()
    {
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
        animator.SetBool("Wick", true);

        yield return new WaitForSeconds(explosionDelay);

        particles.Play();
        collider.isTrigger = true;
        path.enabled = false;
        animator.Play("scrombolo_bombolo_exploding");
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
        // int explosionBlockRadius = 2;
        //get pos of tile entity is standing on
        int tileX = (int)gameObject.transform.position.x;
        int tileY = (int)gameObject.transform.position.y;
        //Tile t = blockTilemap.WorldToCell(new Vector3Int(tileX, tileY, 0));

        yield return new WaitForSeconds(1f);

        // onExplode?.Invoke(collider);

        Destroy(gameObject);
    }
}
