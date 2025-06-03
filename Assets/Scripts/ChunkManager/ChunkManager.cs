using UnityEngine;

public class ChunkManager : MonoBehaviour
{
    // all processes that want to add a tile to the scene will be redirected to the chunk manager
    // the chunk manager will store the changes made to the tilemap but will not display them;
    // instead, the manager will only render tiles that are within a vicinity of the player, using 'chunks' of tiles.

    // changes to the tilemap will be stored as a custom tileChange object
    // tileChanges will be sorted into 'chunks' of size n
    // dungeons will be handled as instances with their own chunks


    // when the player moves, the manager will check if a chunk needs to be loaded (rendered) or unloaded (unrendered from scene)
    
}
