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
|          ITEM_POS_Y_FLOAT         |          ITEM_ROT_Z_FLOAT         |
+--------+--------+--------+--------+--------+--------+--------+--------+
|      REST_OF_SERIALIZED_DATA 
+--------+--------+--------+--------+-- . .  .
*/

public class UpdateNetworkGameObject : BaseNetworkSendPacket {
    //Vars used by both
    private SaveObjectsManager saveLoad;
    private uint itemId;

    //Vars needed by sender
    private GameObject gameObjectToSend;
    private PrefabSaveInfo goPrefabSaveInfo;

    //Vars needed by reciever
    private float gameObjectPosX;
    private float gameObjectPosY;
    private float gameObjectRotation;
    private byte[] specificBytes;
    private uint playerID;
    private static float lastUpdatedPlayer; //OMEGA jank but no ref types in this version of C#
    private float maxTimeBeforeResyncPlayer;

    private int skipAmount;

    public override int RequiredBytesToUse { get; protected set; } = 20;

    public override bool IsCompatible(SetValuesInPacketParams pP) {
        if (pP.Packet.Length >= RequiredBytesToUse) {
            itemId = (uint)ConvertToByteArray.ConvertBytesToValue(typeof(uint), pP.Packet, out skipAmount);
            return SaveablePrefabManager.NetworkIdsPrefabs.ContainsKey(itemId);
        }
        return false;
    }

    public UpdateNetworkGameObject() { }

    public UpdateNetworkGameObject(SaveObjectsManager sl, uint iI, GameObject goTS) {
        saveLoad = sl;
        itemId = iI;
        gameObjectToSend = goTS;
    }

    public override void SetValuesInPacket(SetValuesInPacketParams pP) {
        //Item ID is already gonna be called by now cuz of skip ID
        gameObjectPosX = (float)ConvertToByteArray.ConvertBytesToValue(typeof(float), pP.Packet.Skip(skipAmount).ToArray(), out int bytesUsedX);
        gameObjectPosY = (float)ConvertToByteArray.ConvertBytesToValue(typeof(float), pP.Packet.Skip(skipAmount + bytesUsedX).ToArray(), out int bytesUsedY);
        gameObjectRotation = (float)ConvertToByteArray.ConvertBytesToValue(typeof(float), pP.Packet.Skip(skipAmount + bytesUsedX + bytesUsedY).ToArray(), out int rotBytesUsed);

        specificBytes = pP.Packet.Skip(skipAmount + bytesUsedX + bytesUsedY + rotBytesUsed).ToArray();

        gameObjectToSend = SaveablePrefabManager.NetworkIdsPrefabs[itemId];
        goPrefabSaveInfo = SaveablePrefabManager.NetworkIdsSaveInfoCache[itemId];
        BytesUsed = skipAmount + bytesUsedX + bytesUsedY + rotBytesUsed + pP.SaveLoad.GetAmountOfBytesUsedInPrefab(goPrefabSaveInfo.PrefabId,specificBytes); //Disregared extra bytes till better solution found to process them
        playerID = pP.NetworkID;
        maxTimeBeforeResyncPlayer = pP.PlayerResyncTime;
    }

    public override bool IsTcp => false;

    public override byte[] GetPacketBytes() {
        List<byte> serializedData = saveLoad.PrefabGetByteArray(gameObjectToSend, false, true, true);
        byte[] uintByte = ConvertToByteArray.ConvertValueToBytes(itemId);
        return uintByte.Concat(serializedData).ToArray();
    }

    public override void ExecuteSentBytes() {

        if (SaveablePrefabManager.NetworkIdsPrefabs.TryGetValue(itemId, out GameObject gO)) {

            if (itemId != playerID) {
                gO.transform.position = new Vector3(gameObjectPosX, gameObjectPosY, gO.transform.position.z);
                gO.transform.rotation = Quaternion.Euler(gO.transform.rotation.x, 
                                                            gO.transform.rotation.y,
                                                            gameObjectRotation);


                saveLoad.SetAllBytesInAPrefabSafe(gameObjectToSend, goPrefabSaveInfo.PrefabId, 
                                                       specificBytes, out int _, true);
            }
            else {
                //Constantly redoing players update causes jitter so do it like this
                if (Time.time - lastUpdatedPlayer > maxTimeBeforeResyncPlayer) {
                    lastUpdatedPlayer = Time.time;
                    gO.transform.position = new Vector3(gameObjectPosX, gameObjectPosY, gO.transform.position.z);
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
