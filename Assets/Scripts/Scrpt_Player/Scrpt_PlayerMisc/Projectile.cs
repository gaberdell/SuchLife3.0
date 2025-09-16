using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed;
    public float lifeTime;    //if we want a lifetime on arrows
    public float distance;
    public int damage;

    public LayerMask whatIsSolid;

    public GameObject destroyEffectEnemy;
    public GameObject destroyEffectWall;


    private void Start()
    {
        //if we want a lifetime on arrows
        Invoke("DestroyProjectileDueToLifetime", lifeTime);
    }

    // Update is called once per frame
    private void Update()
    {

        RaycastHit2D hitInfo = Physics2D.Raycast(transform.position, transform.up, distance, whatIsSolid);
        if (hitInfo.collider != null)
        {
            bool hitWall = true;
            bool hitEnemy = false;
            if (hitInfo.collider.CompareTag("Enemy"))
            {
                Debug.Log("ENEMY MUST TAKE DAMAGE !");
                hitInfo.collider.GetComponent<Health>().TakeDamage(damage);
                
                hitWall = false;
                hitEnemy = true;
            }
            DestroyProjectile(hitEnemy, hitWall);
        }
        transform.Translate(Vector2.up * speed * Time.deltaTime);
    }

    void DestroyProjectile(bool hitEnemy = false, bool hitWall = false)
    {
        if(hitEnemy)
        {
            Debug.Log("Destroying projectile; Hit Enemy");
            GameObject destroyEffectInScene = Instantiate(destroyEffectEnemy, transform.position, Quaternion.identity);
            destroyEffectInScene.GetComponent<ParticleSystem>().Play();
        }
        else if(hitWall)
        {
            Debug.Log("Destroying projectile; Hit Wall");
            GameObject destroyEffectInScene = Instantiate(destroyEffectWall, transform.position, Quaternion.identity);
            destroyEffectInScene.GetComponent<ParticleSystem>().Play();
        }
        Destroy(gameObject);
    }

    void DestroyProjectileDueToLifetime()
    {
        Debug.Log("Destroying projectile; Lifetime ran out");
        DestroyProjectile(false, false);
    }
}
