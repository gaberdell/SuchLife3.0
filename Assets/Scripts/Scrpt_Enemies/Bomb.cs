using System.Collections;
using Pathfinding;
using UnityEngine;
using System.Collections.Generic;

public class Bomb : MonoBehaviour
{
    public SpriteRenderer renderer;
    public CircleCollider2D collider;
    public AIPath path;
    public Animator animator;
    public ParticleSystem particles;

    public Transform target;
    public float explodeAtDistance = 3f;
    public float explosionRadius = 1f;
    public float explosionDelay = 2f;

    private float distance;
    private bool isExploding = false;  

    public delegate void OnExplode(Collider2D explosionCollider);
    public static event OnExplode onExplode;

    void Start()
    {
        target = GameObject.Find("Player").transform;
    }

    void Update()
    {
        distance = Vector2.Distance(transform.position, target.position);
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
        collider.radius = explosionRadius;

        yield return new WaitForSeconds(0.05f);

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, explosionRadius);
        foreach (var hit in hits)
        {
            Health health = hit.GetComponent<Health>();
            if (health != null)
            {
                health.TakeDamage(50); 
            }
        }

        yield return new WaitForSeconds(0.5f);

        onExplode?.Invoke(collider);

        Destroy(gameObject);
    }
}
