using System;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

/*
Sent from Client to Server
UDP : Send over field UDP movement field
 0      7 8     15 16    23 24    31 32    39 40    47 48    55 56    63
+--------+--------+--------+--------+--------+--------+--------+--------+
|       PLAYER_POS_X_FLOAT          |       PLAYER_POS_Y_FLOAT          |
+--------+--------+--------+--------+--------+--------+--------+--------+
|   MOUSE_ROT_RELATIVE_TO_PLAYER    |
+--------+--------+--------+--------+
*/

public class PlayerMovementPacket : BaseNetworkSendPacket {
    private int playerNetworkID;

    private float playerX;
    private float playerY;
    private float playerRot;


    public PlayerMovementPacket(float pX, float pY, float pRot) {
        playerX = pX;
        playerX = pY;
        playerRot = pRot;
    }

    public PlayerMovementPacket(byte[] plrBytes, int plrNetworkId) {
        playerNetworkID = plrNetworkId;

        playerX = (float)ConvertToByteArray.ConvertBytesToValue(typeof(float), plrBytes, out int bytesUsedX);
        playerY = (float)ConvertToByteArray.ConvertBytesToValue(typeof(float), plrBytes.Skip(bytesUsedX).ToArray(), out int bytesUsedY);
        playerRot = (float)ConvertToByteArray.ConvertBytesToValue(typeof(float), plrBytes.Skip(bytesUsedX).ToArray(), out int bytesUsedRot);
        BytesUsed = bytesUsedX + bytesUsedY + bytesUsedRot;
    }

    public override bool IsTcp => false;

    public override byte[] GetPacketBytes() {
        byte[] playerXBytes = ConvertToByteArray.ConvertValueToBytes(playerX);
        byte[] playerYBytes = ConvertToByteArray.ConvertValueToBytes(playerY);
        byte[] playerRotBytes = ConvertToByteArray.ConvertValueToBytes(playerRot);
        return playerXBytes.Concat(playerYBytes).Concat(playerRotBytes).ToArray();
    }

    //A lot of jank to work around not being able to grab stuff directly
    public override void ExecuteSentBytes() {
        
        ReadOnlyDictionary<int, PlayerNetworkDataToMovement> plrGODict = ServerNetworkManager.GetClientIdToPlayerGO();

        //Cach this

        if (plrGODict.TryGetValue(playerNetworkID, out PlayerNetworkDataToMovement value)) {
            value.SetPlayerControlValues(playerX, playerY, playerRot);
        }
    }
}
