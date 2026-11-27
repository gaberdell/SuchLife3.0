using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Factory class that checks if function is of type if so uses it
/// </summary>
public class NetworkSenderFactory
{
    public IReadOnlyList<BaseNetworkSendPacket> networkSendPacketTypes = new List<BaseNetworkSendPacket>();

    public NetworkSenderFactory(IEnumerable<BaseNetworkSendPacket> nTSPT) {
        networkSendPacketTypes = new List<BaseNetworkSendPacket>(nTSPT);
    }

    public BaseNetworkSendPacket BuildNewPacket(SetValuesInPacketParams pP) {
        foreach (BaseNetworkSendPacket netWorkType in networkSendPacketTypes) {
            if (netWorkType.IsCompatible(pP)) {
                Type objType = netWorkType.GetType();

                BaseNetworkSendPacket newPacketOfType = (BaseNetworkSendPacket)Activator.CreateInstance(objType);

                newPacketOfType.SetValuesInPacket(pP);

                return newPacketOfType;
            }
        }
        return null;
    }
}
