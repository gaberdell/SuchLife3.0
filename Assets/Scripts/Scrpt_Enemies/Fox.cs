using System.Collections;
using System.Collections.Generic;
using NUnit.Framework.Constraints;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Fox : Mob
{    
    void Start()
    {
        blockTilemap = blockTilemap != null ? blockTilemap : GameObject.Find(tileMapName).GetComponent<Tilemap>();
        //pathSetter = GetComponent<AIDestinationSetter>();
        objectInScene = gameObject;
        enemyAttack = GetComponent<EnemyAttack>(); // later set target if get attacked
        //set starting chunk
        chunkPos = ChunkManager.getChunkPosFromWorld(objectInScene.transform.position);

    }
    
}
