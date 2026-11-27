using System;
using System.Linq;
using UnityEngine;
using UnityEngine.XR;

/*
Sent from Server to Clients
TCP : Add our player to the scene
 0      7 8     15 16    23 24    31 32    39 40    47 48    55 56    63
+--------+--------+--------+--------+--------+--------+--------+--------+
|ENUM_PLR|             NETWORK_ID            |              PLAYER_POS_X
+--------+--------+--------+--------+--------+--------+--------+--------+
_FLOAT   |        PLAYER_POS_Y_FLOAT         |              PLAYER_POS_Z
+--------+--------+--------+--------+--------+--------+--------+--------+
_FLOAT   |        PLAYER_ROT_Z_FLOAT         |
+--------+--------+--------+--------+--------+
*/

public class AddPlayerFromNetwork : BaseNetworkSendPacket {
    //Vars used by both
    private uint itemId;
    private Vector3 gameObjectPos;
    private float gameObjectRotation;

    //Vars needed by sender

    //Vars needed by reciever
    private InitialWorldSetup initWorldSetup;

    public override int RequiredBytesToUse { get; protected set; } = 21;

    public override bool IsCompatible(SetValuesInPacketParams pP) {
        return BasicCompatibleChecker(pP.Packet, NetworkOpCodeEnum.ADD_LOCAL_PLAYER);
    }

    public AddPlayerFromNetwork() { }

    public AddPlayerFromNetwork(uint iI, Vector3 goPos, float rot) {
        itemId = iI;
        gameObjectPos = goPos;
        gameObjectRotation = rot;
    }

    public override void SetValuesInPacket(SetValuesInPacketParams pP) {
        byte[] bytes = pP.Packet.Skip(1).ToArray();
        initWorldSetup = pP.InitWorldSetup;
        ByteArrayPrint.PrintByteArray(bytes, "Printing bytes from network saying its us : ");
        itemId = (uint)ConvertToByteArray.ConvertBytesToValue(typeof(uint), bytes, out int skipAmount);
        gameObjectPos = (Vector3)ConvertToByteArray.ConvertBytesToValue(typeof(Vector3), bytes.Skip(skipAmount).ToArray(), out int bytesUsed);
        gameObjectRotation = (float)ConvertToByteArray.ConvertBytesToValue(typeof(float), bytes.Skip(skipAmount + bytesUsed).ToArray(), out int rotBytesUsed);
        BytesUsed = skipAmount + bytesUsed + rotBytesUsed;
    }

    public override bool IsTcp => true;

    public override byte[] GetPacketBytes() {
        byte[] uintByte = ConvertToByteArray.ConvertValueToBytes(itemId);
        byte[] gameObjectPosByte = ConvertToByteArray.ConvertValueToBytes(gameObjectPos);
        byte[] gameObjectRotationByte = ConvertToByteArray.ConvertValueToBytes(gameObjectRotation);
        return (new byte[1] {(byte)NetworkOpCodeEnum.ADD_LOCAL_PLAYER}).Concat(uintByte)
                .Concat(gameObjectPosByte).Concat(gameObjectRotationByte).ToArray();
    }

    public override void ExecuteSentBytes() {
        GameObject player = SaveablePrefabManager.CreatePrefab("Player", gameObjectPos, gameObjectRotation, itemId);
        initWorldSetup.AddPlayerToGame(player);
    }
}
