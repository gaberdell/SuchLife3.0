using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed;
    public float lifeTime;    //if we want a lifetime on arrows
    public float distance;
    public int damage;

    public LayerMask whatIsSolid;

    public GameObject destroyEffect;

    //if we want a lifetime on arrows
    private void Start()
    {
        Invoke("DestroyProjectile", lifeTime);
    }

    // Update is called once per frame
    private void Update()
    {

        RaycastHit2D hitInfo = Physics2D.Raycast(transform.position, transform.up, distance, whatIsSolid);
        if (hitInfo.collider != null){
            if (hitInfo.collider.CompareTag("Enemy")) {
                Debug.Log("ENEMY MUST TAKE DAMAGE !");
                hitInfo.collider.GetComponent<Health>().TakeDamage(damage);
            }
            destroyProjectile();
        }



        transform.Translate(Vector2.up * speed * Time.deltaTime);
    }

    void destroyProjectile(){
        Instantiate(destroyEffect, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}
