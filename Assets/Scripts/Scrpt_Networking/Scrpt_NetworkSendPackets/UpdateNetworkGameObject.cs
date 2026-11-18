using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/*
Sent from Server to Clients
UDP : Send over item update
 0      7 8     15 16    23 24    31 32    39 40    47 48    55 56    63
+--------+--------+--------+--------+--------+--------+--------+--------+
|             NETWORK_ID            |          ITEM_POS_X_FLOAT         |
+--------+--------+--------+--------+--------+--------+--------+--------+
|          ITEM_POS_Y_FLOAT         |          ITEM_POS_Z_FLOAT         |
+--------+--------+--------+--------+--------+--------+--------+--------+
|          ITEM_ROT_Z_FLOAT         |      REST_OF_SERIALIZED_DATA      
+--------+--------+--------+--------+--------+--------+--------+-- . .  .
*/

public class UpdateNetworkGameObject : BaseNetworkSendPacket {
    //Vars used by both
    private SaveObjectsManager saveLoad;
    private uint itemId;

    //Vars needed by sender
    private GameObject gameObjectToSend;
    private PrefabSaveInfo goPrefabSaveInfo;

    //Vars needed by reciever
    private Vector3 gameObjectPos;
    private float gameObjectRotation;
    private byte[] specificBytes;
    private uint playerID;
    private static float lastUpdatedPlayer; //OMEGA jank but no ref types in this version of C#
    private float maxTimeBeforeResyncPlayer;

    public UpdateNetworkGameObject(SaveObjectsManager sl, uint iI, GameObject goTS) {
        saveLoad = sl;
        itemId = iI;
        gameObjectToSend = goTS;
    }

    public UpdateNetworkGameObject(byte[] bytes, SaveObjectsManager sl, uint pID, float plrResync) {
        itemId = (uint)ConvertToByteArray.ConvertBytesToValue(typeof(uint), bytes, out int skipAmount);
        gameObjectPos = (Vector3)ConvertToByteArray.ConvertBytesToValue(typeof(Vector3), bytes.Skip(skipAmount).ToArray(), out int bytesUsed);
        gameObjectRotation = (float)ConvertToByteArray.ConvertBytesToValue(typeof(float), bytes.Skip(skipAmount + bytesUsed).ToArray(), out int rotBytesUsed);

        specificBytes = bytes.Skip(skipAmount + bytesUsed + rotBytesUsed).ToArray();
        gameObjectToSend = SaveablePrefabManager.NetworkIdsPrefabs[itemId];
        goPrefabSaveInfo = SaveablePrefabManager.NetworkIdsSaveInfoCache[itemId];
        BytesUsed = skipAmount + bytesUsed + rotBytesUsed + sl.GetAmountOfBytesUsedInPrefab(goPrefabSaveInfo.PrefabId,specificBytes); //Disregared extra bytes till better solution found to process them
        playerID = pID;
        maxTimeBeforeResyncPlayer = plrResync;
    }

    public override bool IsTcp => false;

    public override byte[] GetPacketBytes() {
        List<byte> serializedData = saveLoad.PrefabGetByteArray(gameObjectToSend, false);
        byte[] uintByte = ConvertToByteArray.ConvertValueToBytes(itemId);
        return uintByte.Concat(serializedData).ToArray();
    }

    public override void ExecuteSentBytes() {
        if (SaveablePrefabManager.NetworkIdsPrefabs.TryGetValue(itemId, out GameObject gO)) {

            if (itemId != playerID) {
                gO.transform.position = gameObjectPos;
                gO.transform.rotation = Quaternion.Euler(gO.transform.rotation.x, 
                                                            gO.transform.rotation.y,
                                                            gameObjectRotation);

                saveLoad.SetAllBytesInAPrefab(gameObjectToSend, goPrefabSaveInfo.PrefabId,
                                                        specificBytes, out int _);
            }
            else {
                //Constantly redoing players update causes jitter so do it like this
                if (Time.time - lastUpdatedPlayer > maxTimeBeforeResyncPlayer) {
                    lastUpdatedPlayer = Time.time;
                    gO.transform.position = gameObjectPos;
                    gO.transform.rotation = Quaternion.Euler(gO.transform.rotation.x,
                                                                gO.transform.rotation.y,
                                                                gameObjectRotation);
                }
            }
        }
        else {
            Debug.Log("UDP Packet recieved! However this object seems to not exsist yet sorry!");
            return;
        }
    }
}
