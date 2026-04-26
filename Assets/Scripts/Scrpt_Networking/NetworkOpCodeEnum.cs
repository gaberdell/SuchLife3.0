using UnityEngine;

/* Op codes that decide how the server talks to the client
 * 
 */
public enum NetworkOpCodeEnum : byte
{
    ADD_LOCAL_PLAYER,
    ADD_PREFAB,
    REMOVE_PREFAB,
    UPDATE_PREFAB,
    DEAL_WITH_TCP_CONNECTION
}
