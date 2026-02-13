using UnityEngine;
using UnityEngine.Tilemaps;

public class HandleTileCollisions : MonoBehaviour
{
    Tilemap groundTilemap;
    void Start()
    {
        groundTilemap = GameObject.Find("GroundTilemap").GetComponent<Tilemap>();
    }

    // Update is called once per frame
    void Update()
    {
        //get all tiles the player is in contact with
        Vector3Int center = new Vector3Int((int)gameObject.transform.position.x, (int)gameObject.transform.position.y, (int)gameObject.transform.position.z);
        int playerRadius = 3; //get real radius
        BoundsInt bounds = new BoundsInt(
            center.x - playerRadius,
            center.y - playerRadius,
            center.z - playerRadius,
            playerRadius * 2,
            playerRadius * 2,
            1
        );

        foreach (Vector3Int cellp in bounds.allPositionsWithin)
        {
            if (groundTilemap.HasTile(cellp))
            {
                Vector3 tileWorldPos = groundTilemap.GetCellCenterWorld(cellp);
                if(Vector3.Distance(center, tileWorldPos) < playerRadius)
                {
                    Debug.Log("player made contact with tile " + groundTilemap.GetTile(cellp).name);
                }

            }
        }
    }

    //private void OnTriggerEnter2D(Collider2D collision)
    //{
        
    //    if(collision.gameObject.CompareTag("Player"))
    //    {
    //        Debug.Log("player triggered with ground tilemap");
    //    }
    //}

    //private void OnCollisionEnter2D(Collision2D collision)
    //{
    //    if (collision.gameObject.CompareTag("Player"))
    //    {
    //        Debug.Log("player collided with ground tilemap");
    //    }
    //}
}
