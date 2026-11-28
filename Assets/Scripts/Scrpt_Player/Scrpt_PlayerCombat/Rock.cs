using UnityEngine;

public class Rock : MonoBehaviour
{
    public float offset; //allows weapon to align regardless
    //of the weapon sprites orientation (adjust offset according to 
    // sprite starting orientation)
    //of the weapon sprite's orientation 
    //Default for weapon facing up = -90

    public GameObject projectile;
    public Transform shotPoint;

    private float timeBtwShots;
    public float startTimeBtwShots;

    // Update is called once per frame
    private void Update()
    {
        //Rotates rock to follow mouse cursor
        Vector3 difference = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        float rotZ = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, rotZ + offset);

        if(timeBtwShots <= 0){
            if(Input.GetMouseButtonDown(0)) {
                Instantiate(projectile, shotPoint.position, transform.rotation);
                timeBtwShots = startTimeBtwShots;
            }
        }
        else{
            timeBtwShots -= Time.deltaTime;
        }

    }
}


