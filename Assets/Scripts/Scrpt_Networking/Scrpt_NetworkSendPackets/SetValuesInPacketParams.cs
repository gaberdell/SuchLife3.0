using UnityEngine;

/// <summary>
/// Record so if we change params for class we don't have to change double
/// </summary>
public struct SetValuesInPacketParams
{
    //Not C# 11 so we gonna have to do this manual but whatever
    public byte[] Packet { get; set; }
    public SaveObjectsManager SaveLoad { get; set; }
    public InitialWorldSetup InitWorldSetup { get; set; }
    public int PlayerClientID { get; set; }
    public uint NetworkID { get; set; }
    public float PlayerResyncTime { get; set; }

    public SetValuesInPacketParams(byte[] packet, SaveObjectsManager sl = null,
                                        InitialWorldSetup iws = null, int plrNwID = 0, 
                                        uint ntWorkID = 0, float resync = 0f) {
        Packet = packet;
        SaveLoad = sl;
        InitWorldSetup = iws;
        PlayerClientID = plrNwID;
        NetworkID = ntWorkID;
        PlayerResyncTime = resync;
    }
}
