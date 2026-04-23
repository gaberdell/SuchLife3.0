using UnityEngine;

/*Data to be sent to main thread
 * As multi threads can not use the Instantiate function of Unity
 * or other core unity functions for that matter we must
 * move this functionality to the main thread in this case
 * using a concurrent enque class to store this data
 * This struct handles the data to be moved
 */
public struct NetworkMainThreadStruct
{
    public NetworkOpCodeEnum networkOpCode;
    public Vector3 prefabPosition;
    public Vector3 prefabRotation;
    public uint networkId;
    public byte[] idOfPrefab;
    public int tcpOriginId;
}
