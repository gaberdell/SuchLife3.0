using System.Collections.Concurrent;
using UnityEngine;


//TODO : Most of them have a lot of repeat code omg :sob:
/// <summary>
/// Class used to contain common functionality between
/// Server and Client network managers!
/// </summary>
public abstract class NetworkManager : MonoBehaviour
{
    protected static ConcurrentQueue<BaseNetworkSendPacket> ExecuteOtherThreadRequestQueue;

    protected static void ProcessThroughByteStream(byte[] rawBytes, NetworkSenderFactory factory,
                                                   SetValuesInPacketParams paramValues) {
        using (PacketWrapper packet = new PacketWrapper(rawBytes)) {
            while (packet.BytesLeftToRead() > 0) {
                //TODO KEEP THIS UPDATED
                BaseNetworkSendPacket action = factory.BuildNewPacket(new SetValuesInPacketParams(
                            packet.GetBytes(), paramValues.SaveLoad, paramValues.InitWorldSetup,
                            paramValues.PlayerClientID, paramValues.NetworkID, paramValues.PlayerResyncTime));

                if (action != null) {
                    packet.AdvanceReadPos(action.BytesUsed);

                    ExecuteOtherThreadRequestQueue.Enqueue(action);
                }
                else {
                    break;
                }
            }
        }
    }
}
