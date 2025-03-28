using System.Collections;
using Pathfinding;
using UnityEngine;

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

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, explosionRadius);
        foreach (var hit in hits)
        {
            Health health = hit.GetComponent<Health>();
            if (health != null)
            {
                health.TakeDamage(damage); 
            }
        }

        yield return new WaitForSeconds(0.5f);

        // onExplode?.Invoke(collider);

        Destroy(gameObject);
    }
}
