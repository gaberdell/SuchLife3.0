using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using UnityEditor.PackageManager;
using UnityEngine;

public class ServerNetworkManager : MonoBehaviour
{

    private static int MAX_TCP_BUFFER_ALLOCATION = 4096;

    private static ServerNetworkManager instance;

    private static TcpListener tcpListener;
    private static UdpClient udpListener;

    private static Dictionary<int, TcpClientData> tcpClients;
    private static int nextClientId = 1;
    private static object clientsLock;

    private static bool isActivated;

    private static Dictionary<int, GameObject> players;

    //TODO : Decouple this from the other thing but for rn just getting it working
    private SaveObjectsManager currentSaveManager;

    private static ConcurrentQueue<NetworkMainThreadStruct> ExecuteOtherThreadRequestQueue;

    public static void StartServer(ushort port) {
        Debug.Log(String.Format("Starting server with port {0}", port));
        tcpListener = new TcpListener(IPAddress.Any, port);
        tcpListener.Start();
        tcpListener.BeginAcceptSocket(onAcceptTcpClient, null);

        udpListener = new UdpClient(port);
        udpListener.BeginReceive(onRecieveDataUdp, null);

        isActivated = true;
    }

    public static void StopServer() {
        isActivated = false;

        lock (clientsLock) {
            foreach (var client in tcpClients.Values) {
                //Closes TcpClient
                client.Client.Close();
            }
            //Clear Tcp listener if previously in use
            tcpClients.Clear();
        }
        udpListener?.Close();
        tcpListener?.Stop();

        Application.Quit();
    }

    static void onAcceptTcpClient(IAsyncResult result) {
        try {
            TcpClient newClient = tcpListener.EndAcceptTcpClient(result);
            NetworkStream newClientStream = newClient.GetStream();

            int clientId;
            lock (clientsLock) {
                clientId = nextClientId++;
            }

            IPEndPoint endPoint = (IPEndPoint)newClient.Client.RemoteEndPoint;

            Debug.Log("New Ip adress dont mean to dox you : " + endPoint.Address + ":" + endPoint.Port);

            TcpClientData newClientData = new TcpClientData();

            newClientData.ClientId = clientId;
            newClientData.Stream = newClientStream;
            newClientData.Buffer = new byte[MAX_TCP_BUFFER_ALLOCATION];
            newClientData.Client = newClient;
            newClientData.EndPoint = null; //Thats what guide says maybe change it to endPoint if it makes more sense

            tcpClients.Add(clientId, newClientData);

            newClientStream.BeginRead(newClientData.Buffer, 0, MAX_TCP_BUFFER_ALLOCATION, onRecieveDataTcp, newClientData);

            //using end accept client will end it sob emoji gotta call it again
            tcpListener.BeginAcceptTcpClient(onAcceptTcpClient, null);

            NetworkMainThreadStruct newCommand = new NetworkMainThreadStruct();

            newCommand.networkOpCode = NetworkOpCodeEnum.ADD_LOCAL_PLAYER;
            newCommand.prefabPosition = Vector3.zero;
            newCommand.prefabRotation = Vector3.zero;
            newCommand.tcpOriginId = clientId; //Gets overrided with the clientID in this case
            newCommand.idOfPrefab = null;

            ExecuteOtherThreadRequestQueue.Enqueue(newCommand);

            foreach (var saveObject in SaveablePrefabManager.SaveablePrefabs) {
                NetworkMainThreadStruct addForEachSavePrefabBefore = new NetworkMainThreadStruct();
                PrefabSaveInfo saveInfo = saveObject.GetComponent<PrefabSaveInfo>();

                addForEachSavePrefabBefore.networkOpCode = NetworkOpCodeEnum.ADD_LOCAL_PLAYER;
                addForEachSavePrefabBefore.prefabPosition = saveObject.transform.position;
                addForEachSavePrefabBefore.prefabRotation = saveObject.transform.rotation.eulerAngles;
                addForEachSavePrefabBefore.networkId = saveInfo.NetworkId;
                addForEachSavePrefabBefore.idOfPrefab = saveInfo.PrefabId;
                addForEachSavePrefabBefore.tcpOriginId = clientId; //Gets overrided with the clientID in this case

                ExecuteOtherThreadRequestQueue.Enqueue(addForEachSavePrefabBefore);
            }
        }
        catch (Exception e) {
            Debug.LogError("Error handeling tcpAccept lmao : " + e.Message);
        }
    }

    static void onRecieveDataTcp(IAsyncResult result) {
        TcpClientData data = (TcpClientData)result.AsyncState;

        try {
            int bytesRead = data.Stream.EndRead(result);
            if (bytesRead > 0) {

                //Clip data to bytes we want
                byte[] tcpBytes = new byte[bytesRead];
                for (int i = 0; i < bytesRead; i++) {
                    tcpBytes[i] = data.Buffer[i];
                }

                using (PacketWrapper packet = new PacketWrapper(tcpBytes)) {
                    //TODO : Text processing suff ig
                    //Make me print a pretty message to da screen
                    Debug.Log("Packet recieved! First packet byte is : " + packet.GetBytes()[0]);
                }

                data.Buffer = new byte[MAX_TCP_BUFFER_ALLOCATION];
                data.Stream.BeginRead(data.Buffer, 0, MAX_TCP_BUFFER_ALLOCATION, onRecieveDataTcp, data);
            }
            else {
                removeTcpClient(data.ClientId);
                IPEndPoint clientEndPoint = (IPEndPoint)data.Client.Client.RemoteEndPoint;

                Debug.Log("Peacefully lost client : " + clientEndPoint.Address + ":" + clientEndPoint.Port);
                SaveablePrefabManager.DeletePrefab(players[data.ClientId]);
                data.Client.Close();
            }
        }
        catch (ObjectDisposedException) {
            //Object gone ig
        }
        catch (IOException) {
            //Client left unexpectedly 
            removeTcpClient(data.ClientId);
            IPEndPoint clientEndPoint = (IPEndPoint)data.Client.Client.RemoteEndPoint;

            Debug.LogError("Unexpectedly lost client : " + clientEndPoint.Address + ":" + clientEndPoint.Port);
            SaveablePrefabManager.DeletePrefab(players[data.ClientId]);
            data.Client.Close();
        }
    }

    static void onRecieveDataUdp(IAsyncResult result) {
        try {

            IPEndPoint udpEndPoint = new IPEndPoint(IPAddress.Any, 0);

            byte[] recievedData = udpListener.EndReceive(result, ref udpEndPoint);

            TcpClientData clientData = null;

            foreach (var client in tcpClients.Values) {
                if (client.EndPoint != null) {
                    continue;
                }
                IPEndPoint tcpEndPoint = (IPEndPoint)client.Client.Client.RemoteEndPoint;
                if (tcpEndPoint.Address.Equals(udpEndPoint.Address)) {
                    //Found matching client by IP adress. set udp endpoint
                    clientData = client;
                    client.EndPoint = udpEndPoint;
                    break;
                }
            }

            using (PacketWrapper packet = new PacketWrapper(recievedData)) {
                //TODO : make this less jank oml
                //Make me print a pretty message to da screen
                Debug.Log("UDP Packet recieved! First packet byte is : " + packet.GetBytes()[0]);
                players[clientData.ClientId].GetComponent<PlayerNetworkDataToMovement>().SetFromNetworkBytes(packet.GetBytes());
            }


            udpListener.BeginReceive(onRecieveDataUdp, null);
        }
        catch (ObjectDisposedException) {
            //Object gone ig
        }
        catch (Exception e) {
            Debug.LogError("Unexpectedly got error : " + e.Message);

            if (isActivated) {
                //Keep going either way!!
                udpListener.BeginReceive(onRecieveDataUdp, null);
            }
        }
    }

    static void SendToTcp(int client, byte[] bytesToAdd) {
        using (PacketWrapper packet = new PacketWrapper()) {
            packet.AddBytes(bytesToAdd);
            byte[] data = packet.GetBytes();
            Debug.Log(String.Format("(TCP) Sending packets to {0} clients", tcpClients.Count));
            tcpClients[client].Stream.Write(data, 0, data.Length);
        }
    }


    static void SendToAllWithTcp(byte[] bytesToAdd) {
        using (PacketWrapper packet = new PacketWrapper()) {
            packet.AddBytes(bytesToAdd);
            byte[] data = packet.GetBytes();
            Debug.Log(String.Format("(TCP) Sending packets to {0} clients", tcpClients.Count));
            foreach (var client in tcpClients.Values) {
                client.Stream.Write(data, 0, data.Length);
            }
        }
    }

    static void SendToAllWithUdp(byte[] bytes) {
        using (PacketWrapper packet = new PacketWrapper()) {
            packet.AddBytes(bytes);
            byte[] data = packet.GetBytes();

            Debug.Log(String.Format("(UDP) Sending packets to {0} clients", tcpClients.Count));
            foreach (var client in tcpClients.Values) {
                if (client.EndPoint != null) {
                    udpListener.BeginSend(data, data.Length, client.EndPoint, null, null);
                }
            }
        }
    }

    static void removeTcpClient(int clientId) {
        lock (clientsLock) { 
            if (tcpClients.Remove(clientId)) {
                Debug.Log("Removed client id : " + clientId);
            }
        }
    }

    private void Awake() {
        currentSaveManager = GameObject.FindFirstObjectByType<SaveObjectsManager>();
        ExecuteOtherThreadRequestQueue = new ConcurrentQueue<NetworkMainThreadStruct>();
    }

    private void OnEnable() {
        if (DataService.IsMultiplayer) {
            EventManager.PrefabAddedToScene += addTcpObject;
            EventManager.PrefabRemovedFromScene += removeTcpObject;
        }
    }

    private void OnDisable() {
        if (DataService.IsMultiplayer) {
            EventManager.PrefabAddedToScene -= addTcpObject;
            EventManager.PrefabRemovedFromScene -= removeTcpObject;
        }
    }

    void Start()
    {
        if (instance == null && DataService.IsMultiplayer && DataService.IsLocalSave) {
            instance = this;
            tcpClients = new Dictionary<int, TcpClientData>();
            players = new Dictionary<int, GameObject>();
            clientsLock = new object();

            StartServer(DataService.PortOfServerWeAreHosting);
        }
        else {
            Destroy(instance);
        }
    }

    byte[] addTcpBasic(GameObject prefabToAdd) {
        PrefabSaveInfo saveInfo = prefabToAdd.GetComponent<PrefabSaveInfo>();
        byte[] networkIdType = new byte[1] { (byte)NetworkOpCodeEnum.ADD_PREFAB };
        byte[] networkBytes = ConvertToByteArray.ConvertValueToBytes(saveInfo.NetworkId);
        byte[] posBytes = ConvertToByteArray.ConvertValueToBytes(prefabToAdd.transform.position);
        byte[] rotBytes = ConvertToByteArray.ConvertValueToBytes(prefabToAdd.transform.rotation.eulerAngles);
        return networkIdType.Concat(networkBytes).Concat(posBytes).Concat(rotBytes).Concat(saveInfo.PrefabId).ToArray();
    }

    void addTcpObject(GameObject prefabToAdd) {
        SendToAllWithTcp(addTcpBasic(prefabToAdd));
    }

    void removeTcpObject(GameObject prefabToRemove) {
        PrefabSaveInfo saveInfo = prefabToRemove.GetComponent<PrefabSaveInfo>();
        byte[] networkIdType = new byte[1] { (byte)NetworkOpCodeEnum.REMOVE_PREFAB };
        byte[] networkBytes = ConvertToByteArray.ConvertValueToBytes(saveInfo.NetworkId);
        SendToAllWithTcp(networkIdType.Concat(networkBytes).ToArray());
    }

    void DispatchCommandsOnMainThread() {
        while (ExecuteOtherThreadRequestQueue.Count > 0) {
            bool isNotBlocked = ExecuteOtherThreadRequestQueue.TryDequeue(out NetworkMainThreadStruct newCommand);
            if (isNotBlocked) {
                switch (newCommand.networkOpCode) {
                    case NetworkOpCodeEnum.ADD_LOCAL_PLAYER:
                        GameObject newPlayer = SaveablePrefabManager.CreatePrefab("OtherPlayer", newCommand.prefabPosition, Quaternion.Euler(newCommand.prefabRotation));
                        players.Add(newCommand.tcpOriginId, newPlayer);

                        foreach (var clientPair in tcpClients) {
                            if (clientPair.Key == newCommand.tcpOriginId) {
                                byte[] bytesToSendToTellLocalClient = addTcpBasic(newPlayer);
                                bytesToSendToTellLocalClient[0] = (byte)NetworkOpCodeEnum.ADD_LOCAL_PLAYER;
                                SendToTcp(clientPair.Key, bytesToSendToTellLocalClient);
                            }
                            else {
                                byte[] bytesToSendToTellLocalClient = addTcpBasic(newPlayer);
                                SendToTcp(clientPair.Key, bytesToSendToTellLocalClient);
                            }
                        }
                        break;
                    case NetworkOpCodeEnum.ADD_PREFAB:
                        byte[] networkIdType = new byte[1] { (byte)NetworkOpCodeEnum.ADD_PREFAB };
                        byte[] networkBytes = ConvertToByteArray.ConvertValueToBytes(newCommand.networkId);
                        byte[] posBytes = ConvertToByteArray.ConvertValueToBytes(newCommand.prefabPosition);
                        byte[] rotBytes = ConvertToByteArray.ConvertValueToBytes(newCommand.prefabRotation);
                        byte[] sendBytes = networkIdType.Concat(networkBytes).Concat(posBytes).Concat(rotBytes).Concat(newCommand.idOfPrefab).ToArray();
                        SendToTcp(newCommand.tcpOriginId, sendBytes);
                        break;
                }
            }
        }
    }

    void Update()
    {
        if (isActivated) {

            if (Input.GetKeyDown(KeyCode.X)) {
                StopServer();
            }
            else if (Input.GetKeyDown(KeyCode.Y)) {
                Debug.Log("Summoning Scrombolo Bombolo");
                SaveablePrefabManager.CreatePrefab("Scrombolo_Bombolo", Vector3.zero, Quaternion.identity);
            }
            else if (Input.GetKeyDown(KeyCode.U)) {
            }

            foreach (KeyValuePair<uint, GameObject> item in SaveablePrefabManager.NetworkIdsPrefabs) {
                List<byte> serializedData = currentSaveManager.PrefabGetByteArray(item.Value, false);
                byte[] uintByte = ConvertToByteArray.ConvertValueToBytes(item.Key);
                SendToAllWithUdp(uintByte.Concat(serializedData).ToArray());
            }

            DispatchCommandsOnMainThread();
        }
    }
}
