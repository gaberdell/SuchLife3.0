using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.SceneManagement;

/*
NOTE These are only for the scripts that send it over
    so this script won't have the bytes it recieves on it

Note Protocols are specified inside of BaseNetwork send packet
 */

public class ClientNetworkManager : MonoBehaviour
{
    private static int MAX_TCP_RECIEVE_BUFFER = 4096;

    private float MAX_TIME_BEFORE_WE_RESYNC_PLAYER = 0.0f;

    //TODO : Decouple this from the other thing but for rn just getting it working
    private SaveObjectsManager currentSaveManager;
    private InitialWorldSetup initialWorldSetup;

    private UdpClient udpClient;
    private TcpClient tcpClient;
    private NetworkStream tcpStream;
    private bool isActivated;
    private byte[] recieveBuffer;

    bool playerCreated;

    private uint localPlayerPrefabNetworkId;

    private ConcurrentQueue<BaseNetworkSendPacket> ExecuteOtherThreadRequestQueue;

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

            Debug.Log("Attempting to send to tcp");
            float timeSenseAttemptionAttempt = Time.time;
            while (!tcpClient.Connected) {
                if (Time.time - timeSenseAttemptionAttempt > 25.0f) {
                    Debug.Log("Wasn't able to connect in 25 seconds rip");
                }
            }

            
            sendDataWithTcp(new PlayerAddPacket(DataService.localPlayerUUID, DataService.LoadGlobalPlayerAvatarData()).GetPacketBytes());
        }
        catch (Exception e) {
            Debug.LogError(e.Message);
            StopClient();
            //TODO: Put actual connecting screen here instead of just dumping the player out to the main menu
            //if they fail and also explain why
            SceneManager.LoadScene("TitleScreen");
        }
    }

    public void OnApplicationQuit() {
        StopClient();
    }
    public void OnDisable() {
        StopClient();
    }

    public void StopClient() {
        udpClient?.Close();
        tcpStream?.Close();
        tcpClient?.Close();
        isActivated = false;
        Debug.Log("Client Shut Down (if it was running to begin with..)");
    }


    private void Awake() {
        currentSaveManager = FindAnyObjectByType<SaveObjectsManager>();
        initialWorldSetup = FindAnyObjectByType<InitialWorldSetup>();
    }

    void Start()
    {
        if (!DataService.IsLocalSave && DataService.IsMultiplayer) {
            ExecuteOtherThreadRequestQueue = new ConcurrentQueue<BaseNetworkSendPacket>();
            initialWorldSetup = FindAnyObjectByType<InitialWorldSetup>();
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
            //TCP stream reads everything including multiple requests at once sometimes
            int bytesRead = tcpStream.EndRead(result);
            if (bytesRead > 0) {
                byte[] data = new byte[bytesRead];
                for (int i = 0; i < bytesRead; i++) {
                    data[i] = recieveBuffer[i];
                    //Debug.Log(String.Format("Packet index i {0} : {1}", i, data[i]));
                }
                int bytesLeft = data.Length;
                do {
                    using (PacketWrapper packet = new PacketWrapper(data)) {
                    //TODO : Make Client processing stuff
                    byte[] packetBytes = packet.GetBytes();

                    if (packetBytes.Count() == 0) { break; }

                    BaseNetworkSendPacket packetToSendOver = null;
                    byte[] cBSkipOne = packetBytes.Skip(1).ToArray();

                    switch (packetBytes[0]) {
                        case (byte)NetworkOpCodeEnum.ADD_LOCAL_PLAYER:
                            packetToSendOver = new AddPlayerFromNetwork(cBSkipOne, 
                                currentSaveManager, initialWorldSetup);
                            break;
                        case (byte)NetworkOpCodeEnum.ADD_PREFAB:
                            packetToSendOver = new AddNetworkGameObject(cBSkipOne, 
                                currentSaveManager);
                            break;
                        case (byte)NetworkOpCodeEnum.REMOVE_PREFAB:
                            packetToSendOver = new RemoveNetworkGameObject(cBSkipOne);
                            break;
                    }

                    if (packetToSendOver != null) {
                        ExecuteOtherThreadRequestQueue.Enqueue(packetToSendOver);
                        bytesLeft -= packetToSendOver.BytesUsed;
                        data = data.Skip(packetToSendOver.BytesUsed).ToArray();
                        Debug.Log("Bytes left : " + bytesLeft);
                    }
                    else {
                        break;
                    }


                    }
                } while (bytesLeft > 0);
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
                if (packet.GetBytes() == null || packet.GetBytes().Length == 0) {
                    Debug.Log("Something bad has happened in the Data recieve UDP as we have a udp of zero byte!?");
                    return;
                }
                byte[] packetBytes = packet.GetBytes();

                UpdateNetworkGameObject updatePacket = new UpdateNetworkGameObject(packetBytes, 
                    currentSaveManager, localPlayerPrefabNetworkId, 
                    MAX_TIME_BEFORE_WE_RESYNC_PLAYER);

                ExecuteOtherThreadRequestQueue.Enqueue(updatePacket);
            }
        }
        catch (Exception e) {
            Debug.LogError(e.Message);
        }

        udpClient.BeginReceive(onRecieveDataUdp, null);
    }

    private void sendDataWithTcp(byte[] bytes) {
        try {
            using (PacketWrapper packet = new PacketWrapper()) {
                packet.AddBytes(bytes);
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


    void DispatchCommandsOnMainThread() {
        while (ExecuteOtherThreadRequestQueue.Count > 0) {
            bool isNotBlocked = ExecuteOtherThreadRequestQueue.TryDequeue(out BaseNetworkSendPacket newCommand);
            if (isNotBlocked) {
                newCommand.ExecuteSentBytes();
            }
        }
    }

    void Update()
    {
        if (isActivated) {
            float xValue = InputHandler.Instance.MoveInput.x;
            float yValue = InputHandler.Instance.MoveInput.y;
            float rot = InputHandler.Instance.GetMouseLookPos();
            
            sendDataWithUdp(new PlayerMovementPacket(xValue, yValue, rot).GetPacketBytes());
            // sendDataWithUdp(;
            if (Input.GetKeyDown(KeyCode.X)) {
                StopClient();
            }

            DispatchCommandsOnMainThread();
        }

    }
}
