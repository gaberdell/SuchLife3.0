using System.Collections;
using Pathfinding;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    public AIDestinationSetter destination;
    public Animator animator;
    public ParticleSystem particles;

    public Transform target;
    public float explodeDistance = 3f;
    public float explosionRadius = 4f;
    public float explosionDelay = 2f;

    private float distance;

    public delegate void OnExplode();
    public static event OnExplode onExplode;

    void Start()
    {
        target = GameObject.Find("Player").transform;
    }

    void Update()
    {
        distance = Vector2.Distance(transform.position, target.position);
        if (distance <= explodeDistance) {
            StartCoroutine(Explode());
        }
    }

    IEnumerator Explode()
    {
        destination.target = null;
        animator.enabled = true;
        
        yield return new WaitForSeconds(explosionDelay);

        particles.Play();
        onExplode?.Invoke();

        yield return new WaitForSeconds(0.5f);

        // Debug.Log("explode");

        // Collider2D collider = Physics2D.OverlapCircle(transform.position, explosionRadius);
        // I NEED TO CHECK FOR ALL COLLIDERS MOST LIKELY SO THAT OTHER COLLIDERS DON'T GET IN THE WAY AND FOR SURE THE PLAYER IS HIT OR NOT
        // or make this broadcast an event lowkey that sounds better so this code is not tagged to "Player" specifically
        // if (collider.gameObject.name == "Player") {
        //     Debug.Log("player in explosion radius");
        // } else {
        //     Debug.Log("player safe");
        // }

        Destroy(gameObject);
    }
}
