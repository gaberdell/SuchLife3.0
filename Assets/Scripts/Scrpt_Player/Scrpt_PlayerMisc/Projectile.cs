using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed;

    // public float lifeTime;    //if we want a lifetime on arrows

    // public GameObject destroyEffect;

    // //if we want a lifetime on arrows
    // private void Start()
    // {
    //     Invoke("DestroyProjectile", lifeTime);
    // }

    // Update is called once per frame
    private void Update()
    {
        transform.Translate(Vector2.up * speed * Time.deltaTime);
    }

    // void destroyProjectile(){
    //     Instantiate(destroyEffect, transform.position, Quaternion.identity);
    //     Destroy(gameObject);
    // }
}
