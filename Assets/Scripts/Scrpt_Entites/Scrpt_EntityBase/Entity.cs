using UnityEngine;

/*
 * Public Entity class every entity 
 * Every MonoBehavior has transform which handles position, and rotation
 */

public abstract class Entity : MonoBehaviour, ISaveable
{
    protected Vector2 velocity;
    protected Vector2 angularVelocity;

    abstract public SaveFileFormat Save();
    abstract public void Load(SaveFileFormat saveFile);

    virtual public void SetVelocity(float x, float y)
    {
        velocity.x = x;
        velocity.y = y;
    }

    virtual public void SetAngularVelocity(float x, float y)
    {
        angularVelocity.x = x;
        angularVelocity.y = y;
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
