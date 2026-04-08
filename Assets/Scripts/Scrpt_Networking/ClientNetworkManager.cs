using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

public class ClientNetworkManager : MonoBehaviour
{
    private static int MAX_TCP_RECIEVE_BUFFER = 4096;

    private UdpClient udpClient;
    private TcpClient tcpClient;
    private NetworkStream tcpStream;
    private bool isActivated;
    private byte[] recieveBuffer;

    bool playerCreated;
    public void StartClient(string ip, ushort port) {
        try {
            Debug.Log(String.Format("Joining server Ip of server : {0}, Port is {1}", ip, port));
            isActivated = true;

            tcpClient = new TcpClient(ip, port);
            tcpStream = tcpClient.GetStream();
            recieveBuffer = new byte[MAX_TCP_RECIEVE_BUFFER];

            tcpStream.BeginRead(recieveBuffer, 0, MAX_TCP_RECIEVE_BUFFER, onRecieveDataTcp, null);
            udpClient = new UdpClient(ip, port);
            udpClient.BeginReceive(onRecieveDataUdp, null);
        }
        catch (Exception e) {
            Debug.LogError(e.Message);
        }
    }

    public void OnApplicationQuit() {
        StopClient();
    }

    public void StopClient() {
        udpClient?.Close();
        tcpStream?.Close();
        tcpClient?.Close();
        isActivated = false;
        Debug.Log("Client Shut Down");
    }

    void Start()
    {
        if (!DataService.IsLocalSave && DataService.IsMultiplayer) {
            if (!Uri.TryCreate(DataService.IpOfServer, UriKind.Absolute, out Uri urlWithIpAndPort)) {
                Debug.Log("Try with http?");
                Debug.Log("http://" + DataService.IpOfServer);

                Uri.TryCreate("http://" + DataService.IpOfServer, UriKind.Absolute, out urlWithIpAndPort);
            }


            if (IPAddress.TryParse(urlWithIpAndPort.Host, out IPAddress ip)) {

                Debug.Log("Looking for host : " + urlWithIpAndPort.Host);
                Debug.Log("Looking for port : " + (ushort)urlWithIpAndPort.Port);

                StartClient(urlWithIpAndPort.Host, (ushort)urlWithIpAndPort.Port);
            }
            
        }
        else {
            Destroy(this);
        }
    }

    private void onRecieveDataTcp(IAsyncResult result) {
        try {
            int bytesRead = tcpStream.EndRead(result);
            if (bytesRead > 0) {
                byte[] data = new byte[bytesRead];
                for (int i = 0; i < bytesRead; i++) {
                    data[i] = recieveBuffer[i];
                }

                using (PacketWrapper packet = new PacketWrapper(data)) {
                    //TODO : Make Client processing stuff
                    //Make me print a pretty message to da screen
                    Debug.Log("UDP Packet recieved! First packet byte is : " + packet.GetBytes()[0]);
                }
            }
            else {
                Debug.Log("TCP connection ended");
                StopClient();
            }
            recieveBuffer = new byte[MAX_TCP_RECIEVE_BUFFER];
            tcpStream.BeginRead(recieveBuffer, 0, recieveBuffer.Length, onRecieveDataTcp, null);
        }
        catch (IOException) {
            Debug.LogError("TCP Connection was lost.");
            StopClient();
        }
        catch (ObjectDisposedException) {
            Debug.Log("Just means it was closed during read this");
        }
        catch (Exception e) {
            Debug.LogError("Error reading TCP message booting from server : " + e.Message);
            StopClient();
        }
    }

    //Maybe refactor cuz this stuff basically the same as the server code
    private void onRecieveDataUdp(IAsyncResult result) {
        try {
            IPEndPoint udpEndPoint = new IPEndPoint(IPAddress.Any, 0);

            byte[] recievedData = udpClient.EndReceive(result, ref udpEndPoint);

            using (PacketWrapper packet = new PacketWrapper(recievedData)) {
                //TODO : Text processing suff ig
                //Make me print a pretty message to da screen
                Debug.Log("UDP Packet recieved! First packet byte is : " + packet.GetBytes()[0]);
            }
        }
        catch (Exception e) {
            Debug.LogError(e.Message);
        }
    }

    private void sendDataWithTcp() {
        try {
            using (PacketWrapper packet = new PacketWrapper()) {
                packet.AddBytes(new byte[1] { 11 });
                byte[] data = packet.GetBytes();

                tcpStream.Write(data, 0, data.Length);
            }
        }
        catch (Exception e) {
            Debug.LogError(e.Message);
        }
    }

    private void sendDataWithUdp(byte[] bytes) {
        try {
            using (PacketWrapper packet = new PacketWrapper()) {
                packet.AddBytes(bytes);
                byte[] data = packet.GetBytes();

                udpClient.Send(data, data.Length);
            }
        }
        catch (Exception e) {
            Debug.LogError(e.Message);
        }
    }


    void Update()
    {
        if (isActivated) {
            float xValue = InputHandler.Instance.MoveInput.x;
            float yValue = InputHandler.Instance.MoveInput.y;
            byte[] xBytes = ConvertToByteArray.ConvertValueToBytes(xValue);
            byte[] yBytes = ConvertToByteArray.ConvertValueToBytes(yValue);

            sendDataWithUdp(xBytes.Concat(yBytes).ToArray());

            // sendDataWithUdp(;
            /*if (Input.GetKeyDown(KeyCode.X)) {
                StopClient();
            }
            else if (Input.GetKeyDown(KeyCode.T)) {
                sendDataWithTcp();
            }
            else if (Input.GetKeyDown(KeyCode.U)) {
              //  sendDataWithUdp();
            }*/
        }
    }
}
