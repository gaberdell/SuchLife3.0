using System;
using System.Linq;
using UnityEngine;
using UnityEngine.XR;

/*
Sent from Server to Clients
TCP : Add game object to the scene
 0      7 8     15 16    23 24    31 32    39 40    47 48    55         
+--------+--------+--------+--------+--------+--------+-. .  . +--------+
|ENUM_ADD|             NETWORK_ID            |  ID_OF_PREFAB   |   00   |
+--------+--------+--------+--------+--------+--------+--------+--------+
|        PLAYER_POS_X_FLOAT         |        PLAYER_POS_Y_FLOAT         |
+--------+--------+--------+--------+--------+--------+--------+--------+
|        PLAYER_POS_Z_FLOAT         |        PLAYER_ROT_Z_FLOAT         |
+--------+--------+--------+--------+--------+--------+--------+--------+
|      REST_OF_SERIALIZED_DATA      
+--------+--------+--------+-- . .  .
*/

public class AddNetworkGameObject : BaseNetworkSendPacket {
    //Vars used by both
    private SaveObjectsManager saveLoad;
    private uint itemId;
    private byte[] prefabId;
    private Vector3 gameObjectPos;
    private float gameObjectRotation;
    private byte[] specificBytes;

    //Vars needed to send over

    //Vars needed for send back

    public override int RequiredBytesToUse { get; protected set; } = 23;

    public override bool IsCompatible(SetValuesInPacketParams pP) {
        return BasicCompatibleChecker(pP.Packet, NetworkOpCodeEnum.ADD_PREFAB);
    }

    public AddNetworkGameObject() { }

    public AddNetworkGameObject(uint iI, byte[] pId, 
                                Vector3 goPos, float rot, byte[] extraInfoBytes) {
        itemId = iI;
        prefabId = pId;
        gameObjectPos = goPos;
        gameObjectRotation = rot;
        specificBytes = extraInfoBytes;
    }


    public override void SetValuesInPacket(SetValuesInPacketParams pP) {
        byte[] packet = pP.Packet.Skip(1).ToArray();
        itemId = (uint)ConvertToByteArray.ConvertBytesToValue(typeof(uint), packet, out int skipAmount);
        int bytesUsedAfterIdOfPrefab = Array.IndexOf(packet.Skip(skipAmount).ToArray(), (byte)0) + 1;
        prefabId = packet.Skip(skipAmount).Take(bytesUsedAfterIdOfPrefab - 1).ToArray();
        gameObjectPos = (Vector3)ConvertToByteArray.ConvertBytesToValue(typeof(Vector3), packet.Skip(bytesUsedAfterIdOfPrefab).ToArray(), out int bytesUsed);
        gameObjectRotation = (float)ConvertToByteArray.ConvertBytesToValue(typeof(float), packet.Skip(bytesUsedAfterIdOfPrefab + bytesUsed).ToArray(), out int rotBytesUsed);

        specificBytes = packet.Skip(bytesUsedAfterIdOfPrefab + bytesUsed + rotBytesUsed).ToArray();//Overcharged here to instead be the values to set it too

        ByteArrayPrint.PrintByteArray(packet, "Network Add Game object equals : ");
        Debug.Log($"{bytesUsedAfterIdOfPrefab}");
        ByteArrayPrint.PrintByteArray(prefabId, "Network Add Game object equals : ");

        BytesUsed = skipAmount + bytesUsed + rotBytesUsed + pP.SaveLoad.GetAmountOfBytesUsedInPrefab(prefabId, specificBytes); //Disregared extra bytes till better solution found to process them
        saveLoad = pP.SaveLoad;
    }

    public override bool IsTcp => true;

    public override byte[] GetPacketBytes() {
        byte[] uintByte = ConvertToByteArray.ConvertValueToBytes(itemId);
        byte[] gameObjectPosByte = ConvertToByteArray.ConvertValueToBytes(gameObjectPos);
        byte[] gameObjectRotationByte = ConvertToByteArray.ConvertValueToBytes(gameObjectRotation);
        return (new byte[1] {(byte)NetworkOpCodeEnum.ADD_PREFAB})
                .Concat(uintByte).Concat(prefabId).Concat(new byte[1] { 0 })
                .Concat(gameObjectPosByte).Concat(gameObjectRotationByte).ToArray();
    }

    public override void ExecuteSentBytes() {
        GameObject newPrefab = SaveablePrefabManager.CreatePrefab(prefabId, gameObjectPos, gameObjectRotation, itemId);
        //Not totally accurate but gives us a good guestimate

        saveLoad.SetAllBytesInAPrefabSafe(newPrefab, prefabId, specificBytes, out int _, true);
    }
}
