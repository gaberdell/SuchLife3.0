using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using UnityEngine;

/*
Sent from Server to Clients
TCP : Remove game object from the scene
+--------+--------+--------+--------+--------+
|ENUM_DEL|             NETWORK_ID            |
+--------+--------+--------+--------+--------+
*/

public class RemoveNetworkGameObject : BaseNetworkSendPacket {
    //Vars used by both
    private uint itemId;

    //Vars needed to send over

    //Vars needed for send back
    
    

    public RemoveNetworkGameObject(uint iI) {
        itemId = iI;
    }

    public RemoveNetworkGameObject(byte[] bytes) {
        itemId = (uint)ConvertToByteArray.ConvertBytesToValue(typeof(uint), bytes, out int bytesUsed);
        BytesUsed = bytesUsed;
    }

    public override bool IsTcp => true;

    public override byte[] GetPacketBytes() {
        byte[] uintByte = ConvertToByteArray.ConvertValueToBytes(itemId);
        return (new byte[1] { (byte)NetworkOpCodeEnum.REMOVE_PREFAB })
                .Concat(uintByte).ToArray();
    }

    public override void ExecuteSentBytes() {
        SaveablePrefabManager.DeletePrefab(itemId);
    }
}
