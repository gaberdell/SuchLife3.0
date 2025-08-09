using UnityEngine;

public class Crossbow : MonoBehaviour
{
    public float offset; //allows weapon to align regardless
    //of the weapon sprite's orientation 
    //Default for weapon facing up = -90

    public GameObject projectile;
    public Transform shotPoint;

    private float timeBtwShots;
    public float startTimeBtwShots;

    public float timeToFullCharge;
    private float timeHeld;
    private bool isCharged;

    //3 different states of transition for crossbow charge



    // Update is called once per frame
    private void Update()
    {
        //Rotates weapon to follow mouse cursor
        Vector3 difference = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        float rotZ = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, rotZ + offset);

        if(Input.GetMouseButton(0)){
            timeHeld += Time.deltaTime;
        }

        if(timeHeld >= timeToFullCharge){
            isCharged = true;
        }

        if(timeBtwShots <= 0 && isCharged){
            if(Input.GetMouseButtonDown(0)) {
                Instantiate(projectile, shotPoint.position, transform.rotation);
                timeBtwShots = startTimeBtwShots;
                isCharged = false;
                timeHeld = 0f;
            }
        }
        else{
            timeBtwShots -= Time.deltaTime;
        }

    }
}
