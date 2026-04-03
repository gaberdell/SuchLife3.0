using UnityEngine;
using System.Net.Sockets;
using System.Net;

//Boiler plate from Developers Club youtube channel ty!
public class TcpClientData
{
    public int ClientId;
    public NetworkStream Stream;
    public byte[] Buffer;
    public TcpClient Client;
    public IPEndPoint EndPoint;
}
