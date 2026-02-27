using PlasticPipe.PlasticProtocol.Messages;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class HandleTileCollisions : MonoBehaviour
{
    GameObject Player;
    Tilemap groundTilemap;
    //player will trigger certain effects when coming into contact with specific tiles. To handle this, we keep a dictionary of tiles paired with their corresponding effect
    /* Effects:
     - Hurt the player
     - Fall down a pit (different behaviors for player/enemy/items; player is put back up and the others fall down
     - Tiles that apply some status while player is on it (water that slows, lava that damages and slows, ect)
    */
    Dictionary<TileBase, StatusEffect> effectTiles = new Dictionary<TileBase, StatusEffect>();
     
    void Start()
    {
        Player = GameObject.Find("Player");
        groundTilemap = GameObject.Find("GroundTilemap").GetComponent<Tilemap>();
        //create dictionary with tile effects.
        effectTiles = GameObject.Find("GroundTilemap").GetComponent<TileEffectsDict>().getDict();
    }

    // Update is called once per frame
    void Update()
    {
        //get all tiles the player is in contact with
        Vector3Int playerCellCenter = new Vector3Int((int)gameObject.transform.position.x, (int)gameObject.transform.position.y, 0);
        int playerRadius = 1; //get real radius
        BoundsInt bounds = new BoundsInt(
            playerCellCenter.x - playerRadius,
            playerCellCenter.y - playerRadius,
            0,
            playerRadius * 2,
            playerRadius * 2,
            1
        );
        foreach (Vector3Int cellp in bounds.allPositionsWithin)
        {
            if (groundTilemap.HasTile(cellp))
            {
                Vector3 tileWorldPos = groundTilemap.GetCellCenterWorld(cellp);
                //Debug.Log(groundTilemap.GetTile(cellp).name + Vector3.Distance(this.gameObject.transform.position, tileWorldPos));
                if(Vector3.Distance(this.gameObject.transform.position, tileWorldPos) < playerRadius)
                {
                    TileBase t = groundTilemap.GetTile(cellp);
                    //Debug.Log("player made contact with tile " + t.name);
                    if (effectTiles.ContainsKey(t))
                    {
                        Debug.Log("player made contact with interactive tile " + t.name + ". Applying associated effect " );
                        effectTiles[t].ApplyEffects(Player);
                    }
                }

            }
        }
    }
}
