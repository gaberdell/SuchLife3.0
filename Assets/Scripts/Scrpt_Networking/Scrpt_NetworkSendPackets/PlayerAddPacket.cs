using System;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

/*
Sent from Client to Server
TCP : Send over player UUID to let it know player is joining
 0      7 8     15 16    23 24    31 32    39 40    47 48    55 56    63
+--------+
|ENUM_DWT|
+--------+--------+--------+--------+--------+--------+--------+--------+
+                                                                       +
|                                                                       |
+                              PLAYER_GUID                              +
|                                                                       |
+--------+--------+--------+--------+--------+--------+--------+--------+
|                       STRING_FOR_PLAYER_USER_NAME                      
+--------+--------+--------+--------+--------+--------+--------+. .  .   
|   00   |
+--------+
*/

public class PlayerAddPacket : BaseNetworkSendPacket {
    private Guid playerGUID;
    private string playerUsername;

    private int playerNetworkID;

    private SaveObjectsManager saveLoad;

    public PlayerAddPacket(Guid plrGUID, string userName) {
        playerGUID = plrGUID;
        playerUsername = userName;
    }

    public PlayerAddPacket(byte[] plrBytes, SaveObjectsManager sl, int plrNwID) {
        playerGUID = (Guid)ConvertToByteArray.ConvertBytesToValue(typeof(Guid), plrBytes, out int bytesUsed);
        playerUsername = (string)ConvertToByteArray.ConvertBytesToValue(typeof(string), 
                                                        plrBytes.Skip(bytesUsed).ToArray(), out int stringLen);
        BytesUsed = bytesUsed + stringLen;

        saveLoad = sl;
        playerNetworkID = plrNwID;
    }

    public override bool IsTcp => true;

    public override byte[] GetPacketBytes() {
        Debug.Log("Player sent packet sent over");
        byte[] serializedData = ConvertToByteArray.ConvertValueToBytes(playerGUID);
        byte[] plrByteUsername = ConvertToByteArray.ConvertValueToBytes(playerUsername);
        return (new byte[1] { (byte)NetworkOpCodeEnum.DEAL_WITH_TCP_CONNECTION }).Concat(serializedData).Concat(plrByteUsername).ToArray();
    }

    //A lot of jank to work around not being able to grab stuff directly
    public override void ExecuteSentBytes() {
        //Send over added gameobjects to new player
        bool isPlayerAlreadyInGame = false;
        foreach (GameObject saveObject in SaveablePrefabManager.SaveablePrefabs) {
            BaseNetworkSendPacket packetToSend = null;

            PlayerGUIDInfo isOurPlayer = saveObject.GetComponent<PlayerGUIDInfo>();
            PrefabSaveInfo saveInfo = saveObject.GetComponent<PrefabSaveInfo>();
            if (isOurPlayer != null && isOurPlayer.guid == playerGUID) {
                isPlayerAlreadyInGame = true;

                packetToSend = new AddPlayerFromNetwork(saveInfo.NetworkId,
                    saveObject.transform.position, saveObject.transform.eulerAngles.z);

                //Tell server our player info
                ServerNetworkManager.AddPlayer(playerGUID, saveObject);
                ServerNetworkManager.AddClientIDToPlayer(playerNetworkID, saveObject.GetComponent<PlayerNetworkDataToMovement>());
                Debug.Log("Exiting Early due to player already exsisting");
            }
            else {
                packetToSend = new AddNetworkGameObject(saveInfo.NetworkId,
            saveInfo.PrefabId, saveObject.transform.position, saveObject.transform.rotation.eulerAngles.z,
             saveLoad.PrefabGetByteArray(saveObject, false, false).ToArray());
            }
            ServerNetworkManager.SendToTcp(playerNetworkID, packetToSend.GetPacketBytes());
        }

        if (isPlayerAlreadyInGame) {
            return;
        }
        
        GameObject newPlayer = SaveablePrefabManager.CreatePrefab("ServerPlayer", Vector3.zero, 0f, null, true);
        ServerNetworkManager.AddPlayer(playerGUID, newPlayer);
        ServerNetworkManager.AddClientIDToPlayer(playerNetworkID, newPlayer.GetComponent<PlayerNetworkDataToMovement>());
        PlayerGUIDInfo playerInfo = newPlayer.GetComponent<PlayerGUIDInfo>();
        playerInfo.guid = playerGUID;
        playerInfo.username = playerUsername;
        PrefabSaveInfo prefabSaveInfo = newPlayer.GetComponent<PrefabSaveInfo>();

        foreach (var clientPair in ServerNetworkManager.GetTcpClientData()) {
            BaseNetworkSendPacket packetToSendOver;
            if (clientPair.Key == playerNetworkID) {
                packetToSendOver = new AddPlayerFromNetwork(prefabSaveInfo.NetworkId, Vector3.zero, 0f);
            }
            else {
                packetToSendOver = new AddNetworkGameObject(prefabSaveInfo.NetworkId, prefabSaveInfo.PrefabId, Vector3.zero, 0f, null);
            }
            ServerNetworkManager.SendToTcp(clientPair.Key, packetToSendOver.GetPacketBytes());
        }
    }
}
