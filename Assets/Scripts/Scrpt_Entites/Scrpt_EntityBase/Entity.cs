using UnityEngine;

/*
 * Public Entity class every entity 
 */

public abstract class Entity : MonoBehaviour
{
    Vector2 velocity;


    abstract 

    virtual protected void setVelocity(float x, float y)
    {
        velocity.x = x;
        velocity.y = y;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
