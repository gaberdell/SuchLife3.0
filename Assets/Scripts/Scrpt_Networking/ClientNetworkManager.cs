using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR;


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

    private ConcurrentQueue<NetworkMainThreadStruct> ExecuteOtherThreadRequestQueue;

    private float lastTimeUpdatedPlayerPos = 0;

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

            sendDataWithTcp(DataService.localPlayerUUID.ToByteArray());
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

    public void StopClient() {
        udpClient?.Close();
        tcpStream?.Close();
        tcpClient?.Close();
        isActivated = false;
        Debug.Log("Client Shut Down");
    }


    private void Awake() {
        currentSaveManager = GameObject.FindFirstObjectByType<SaveObjectsManager>();
        initialWorldSetup = GameObject.FindFirstObjectByType<InitialWorldSetup>();
    }

    void Start()
    {
        if (!DataService.IsLocalSave && DataService.IsMultiplayer) {
            ExecuteOtherThreadRequestQueue = new ConcurrentQueue<NetworkMainThreadStruct>();
            initialWorldSetup = GameObject.FindAnyObjectByType<InitialWorldSetup>();
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
                    //Debug.Log(String.Format("Packet index i {0} : {1}", i, data[i]));
                }

                using (PacketWrapper packet = new PacketWrapper(data)) {
                    //TODO : Make Client processing stuff
                    //Make me print a pretty message to da screen
                    byte[] packetBytes = packet.GetBytes();

                    NetworkMainThreadStruct newCommand = new NetworkMainThreadStruct();
                    newCommand.networkOpCode = (NetworkOpCodeEnum)packetBytes[0];

                    uint newPrefabNetworkId = (uint)ConvertToByteArray.ConvertBytesToValue(typeof(uint), packetBytes.Skip(1).ToArray(), out int bytesForUInt);

                    Vector3 prefabPosition = Vector3.zero;
                    Vector3 eulerRotation = Vector3.zero;
                    int totalBytesUsed = 0;

                    if (newCommand.networkOpCode != NetworkOpCodeEnum.REMOVE_PREFAB) {
                        prefabPosition = (Vector3)ConvertToByteArray.ConvertBytesToValue(typeof(Vector3), packetBytes.Skip(1+ bytesForUInt).ToArray(), out int bytesUsedForPos);
                        eulerRotation = (Vector3)ConvertToByteArray.ConvertBytesToValue(typeof(Vector3), packetBytes.Skip(1 + bytesForUInt + bytesUsedForPos).ToArray(), out int rotBytesUsed);
                        totalBytesUsed = 1 + bytesForUInt + bytesUsedForPos + rotBytesUsed;
                    }
                    
                    newCommand.prefabPosition = prefabPosition;
                    newCommand.prefabRotation = eulerRotation;
                    newCommand.networkId = newPrefabNetworkId;
                    
                    if (newCommand.networkOpCode == NetworkOpCodeEnum.ADD_PREFAB) {
                        int zeroByte = 0;
                        for (int i = totalBytesUsed; i < packetBytes.Length; i++) {
                            if (packetBytes[i] == 0) {
                                zeroByte = i;
                                break;
                            }
                        }

                        newCommand.idOfPrefab = packetBytes.Skip(totalBytesUsed).Take(zeroByte - totalBytesUsed - 1).ToArray();

                    }
                    else {
                        newCommand.idOfPrefab = null;
                    }

                    ExecuteOtherThreadRequestQueue.Enqueue(newCommand);
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
                if (packet.GetBytes() == null || packet.GetBytes().Length == 0) {
                    Debug.Log("Something bad has happened in the Data recieve UDP as we have a udp of zero byte!?");
                    return;
                }
                byte[] packetBytes = packet.GetBytes();

                NetworkMainThreadStruct newCommand = new NetworkMainThreadStruct();
                newCommand.networkOpCode = NetworkOpCodeEnum.UPDATE_PREFAB;

                uint objectUIntIdToUpdate = (uint)ConvertToByteArray.ConvertBytesToValue(typeof(uint), packetBytes, out int skipAmount);

                Vector3 prefabPosition = (Vector3)ConvertToByteArray.ConvertBytesToValue(typeof(Vector3), packetBytes.Skip(skipAmount).ToArray(), out int bytesUsed);
                Vector3 eulerRotation = (Vector3)ConvertToByteArray.ConvertBytesToValue(typeof(Vector3), packetBytes.Skip(skipAmount + bytesUsed).ToArray(), out int rotBytesUsed);

                newCommand.networkId = objectUIntIdToUpdate;
                newCommand.prefabPosition = prefabPosition;
                newCommand.prefabRotation = eulerRotation;

                newCommand.idOfPrefab = packetBytes.Skip(skipAmount + bytesUsed + rotBytesUsed).ToArray();//Overcharged here to instead be the values to set it too

                ExecuteOtherThreadRequestQueue.Enqueue(newCommand);
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
            bool isNotBlocked = ExecuteOtherThreadRequestQueue.TryDequeue(out NetworkMainThreadStruct newCommand);
            if (isNotBlocked) {
                switch (newCommand.networkOpCode) {
                    case NetworkOpCodeEnum.ADD_LOCAL_PLAYER:
                        if (SaveablePrefabManager.NetworkIdsPrefabs.ContainsKey(newCommand.networkId)) {
                            SaveablePrefabManager.DeletePrefab(newCommand.networkId);
                        }

                        localPlayerPrefabNetworkId = newCommand.networkId;
                        GameObject newPlayer = SaveablePrefabManager.CreatePrefab("Player", newCommand.prefabPosition, Quaternion.Euler(newCommand.prefabRotation), newCommand.networkId);
                        initialWorldSetup.AddPlayerToGame(newPlayer);
                        break;
                    case NetworkOpCodeEnum.ADD_PREFAB:
                        GameObject newPrefab = SaveablePrefabManager.CreatePrefab(newCommand.idOfPrefab, newCommand.prefabPosition, Quaternion.Euler(newCommand.prefabRotation), newCommand.networkId);
                        break;
                    case NetworkOpCodeEnum.REMOVE_PREFAB:
                        SaveablePrefabManager.DeletePrefab(newCommand.networkId);
                        break;
                    case NetworkOpCodeEnum.UPDATE_PREFAB:
                        if (SaveablePrefabManager.NetworkIdsPrefabs.TryGetValue(newCommand.networkId, out GameObject gO)) {

                            if (newCommand.networkId != localPlayerPrefabNetworkId) {
                                gO.transform.position = newCommand.prefabPosition;
                                gO.transform.rotation = Quaternion.Euler(newCommand.prefabRotation);

                                currentSaveManager.SetAllBytesInAPrefab(SaveablePrefabManager.NetworkIdsPrefabs[newCommand.networkId], null, newCommand.idOfPrefab, out int _);
                            }
                            else {
                                if (Time.time - lastTimeUpdatedPlayerPos > MAX_TIME_BEFORE_WE_RESYNC_PLAYER) {
                                    lastTimeUpdatedPlayerPos = Time.time;
                                    gO.transform.position = newCommand.prefabPosition;
                                    //TODO : Sync rotation
                                    //gO.transform.rotation = Quaternion.Euler(newCommand.prefabRotation);
                                }
                            }
                        }
                        else {
                            Debug.Log("UDP Packet recieved! However this object seems to not exsist yet sorry!");
                            return;
                        }
                        break;
                }
            }
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
            if (Input.GetKeyDown(KeyCode.X)) {
                StopClient();
            }/*
            else if (Input.GetKeyDown(KeyCode.T)) {
                sendDataWithTcp();
            }
            else if (Input.GetKeyDown(KeyCode.U)) {
              //  sendDataWithUdp();
            }*/
            DispatchCommandsOnMainThread();
        }

    }
}
