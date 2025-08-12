using UnityEngine;

public class Crossbow : MonoBehaviour
{
    public float offset; //allows weapon to align regardless
    //of the weapon sprite's orientation 
    //Default for weapon facing up = -90

    public GameObject projectile;
    public Transform shotPoint;

    public float timeToFullCharge;
    private float timeHeld;
    private float holdStartTime;
    private bool isHolding;
    private bool isCharged;

    //3 different states of transition for crossbow charge
    public Sprite idleSprite;
    public Sprite chargingSprite;
    public Sprite chargedSprite;

    private SpriteRenderer sr;

    private void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        if (sr != null && idleSprite != null) sr.sprite = idleSprite;
    }

    // Update is called once per frame
    private void Update()
    {
        //Rotates weapon to follow mouse cursor
        Vector3 difference = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        float rotZ = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, rotZ + offset);

        if (Input.GetMouseButtonDown(0)) //if mouse button is pressed at all
        {
            //if crossbow already fully charged, fire on this click
            if (isCharged)
            {
                Instantiate(projectile, shotPoint.position, transform.rotation);

                isCharged = false;
                if (sr && idleSprite) sr.sprite = idleSprite;
            }
            else //if crossbow isn't fully charged
            {
                isHolding = true;
                isCharged = false;
                holdStartTime = Time.time;
                if (sr && idleSprite) sr.sprite = idleSprite;
            }
        }


        if (Input.GetMouseButton(0) && isHolding) //if mouse button is being held
        {
            float held = Time.time - holdStartTime;

            if (held >= timeToFullCharge)
            {
                isCharged = true;
                if (sr && chargedSprite) sr.sprite = chargedSprite;
            }
            else if (held >= timeToFullCharge * 0.5f)
            {
                if (sr && chargingSprite) sr.sprite = chargingSprite;
            }
            else
            {
                if (sr && idleSprite) sr.sprite = idleSprite;
            }
        }

        if (Input.GetMouseButtonUp(0) && isHolding) //if mouse button is released 
        {
            isHolding = false;

            //if released before full charge, reset to idle
            if (!isCharged && sr && idleSprite) sr.sprite = idleSprite;

        }

        if (isCharged && Input.GetMouseButtonDown(0)) //if crossbow fully charged and mouse is clicked
        {
            Instantiate(projectile, shotPoint.position, transform.rotation);

            isCharged = false;
            if (sr && idleSprite) sr.sprite = idleSprite;
        }

    }
}
