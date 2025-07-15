using Codice.CM.Common;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Mob : MonoBehaviour //base class for living entities
{
    private bool isInKnockback = false;
    private float knockbackForce = 0f;
    private float knockbackAcceleration = 0f;
    private Vector3 knockbackPos;
    protected GameObject objectInScene; //gameobject associated with this instance of an entity
    private int health;
    protected Vector3 worldPos;
    protected Vector2 chunkPos;

    // Update is called once per frame

    //public Mob(Vector3 pos)
    //{
    //    worldPos = pos;
    //}

    //places the mob into the scene
    //public void instantiate(GameObject enemyPrefab)
    //{
    //    if (objectInScene == null)
    //    {
    //        objectInScene = Instantiate(enemyPrefab, worldPos, Quaternion.identity);
    //    }
    //    else
    //    {
    //        Instantiate(objectInScene);
    //    }
    //}

    public void applyKnockback(Vector3 IknockbackVector, float IknockbackForce)
    {
        //set knockback vars which this mob will update to
        isInKnockback = true;
        knockbackForce = IknockbackForce;
        knockbackPos = IknockbackVector + gameObject.transform.position;
        knockbackAcceleration = IknockbackForce / 10;
    }

    public void updateChunkPos()
    {
        Vector2 currChunkPos = ChunkManager.getChunkPosFromWorld(objectInScene.transform.position);
        //Debug.Log(currChunkPos);
        //if detect a change in chunks
        if (currChunkPos != chunkPos)
        {
            //ChunkManager.renderPlayerChunks(transform.position);
            ChunkManager.updateEntityPos(chunkPos, currChunkPos, objectInScene);
            chunkPos = currChunkPos;
        }
    }

    public void updateKnockback()
    {
        //update player if in knockback
        if (isInKnockback)
        {
            var step = (knockbackForce + knockbackAcceleration) * Time.deltaTime; // calculate distance to move
            knockbackForce += knockbackAcceleration;
            gameObject.transform.position = Vector3.MoveTowards(gameObject.transform.position, knockbackPos, step);

            // check if knockback is finished
            if (Vector3.Distance(gameObject.transform.position, knockbackPos) < 0.001f)
            {
                isInKnockback = false;
            }
        }
    }
}
