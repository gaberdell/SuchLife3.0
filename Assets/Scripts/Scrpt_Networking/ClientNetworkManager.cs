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

public class ClientNetworkManager : NetworkManager {

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

    private static NetworkSenderFactory udpPacketFactory;
    private static NetworkSenderFactory tcpPacketFactory;

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

            BaseNetworkSendPacket[] udpPacketTypes = { new UpdateNetworkGameObject() };
            udpPacketFactory = new NetworkSenderFactory(udpPacketTypes);

            BaseNetworkSendPacket[] tcpPacketTypes = { new AddPlayerFromNetwork(), 
                                                new AddNetworkGameObject(), new RemoveNetworkGameObject()};
            tcpPacketFactory = new NetworkSenderFactory(tcpPacketTypes);

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
        TcpClientData data = (TcpClientData)result.AsyncState;
        try {
            //TCP stream reads everything including multiple requests at once sometimes
            int bytesRead = tcpStream.EndRead(result);
            if (bytesRead > 0) {
                byte[] tcpBytes = data.Buffer.Take(bytesRead).ToArray();

                ProcessThroughByteStream(tcpBytes, tcpPacketFactory,
                    new SetValuesInPacketParams(null, currentSaveManager, initialWorldSetup));

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
            Debug.LogError($"Error reading TCP message booting from server : {e.Message}\nStacktrace equals : {e.StackTrace}");
            StopClient();
        }
    }

    //Maybe refactor cuz this stuff basically the same as the server code
    private void onRecieveDataUdp(IAsyncResult result) {
        try {
            IPEndPoint udpEndPoint = new IPEndPoint(IPAddress.Any, 0);

            byte[] recievedData = udpClient.EndReceive(result, ref udpEndPoint);

            ProcessThroughByteStream(recievedData, udpPacketFactory,
                new SetValuesInPacketParams(null, currentSaveManager, initialWorldSetup, 0, localPlayerPrefabNetworkId, MAX_TIME_BEFORE_WE_RESYNC_PLAYER));

        }
        catch (Exception e) {
            Debug.LogError($"UDP exception : {e.Message}\nUDP stacktrace : {e.StackTrace}");
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
