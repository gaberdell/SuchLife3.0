using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

/*
NOTE These are only for the scripts that send it over
    so this script won't have the bytes it recieves on it

Note Protocols are specified inside of BaseNetwork send packet
 */

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

    private static Dictionary<Guid, GameObject> players;
    private static Dictionary<int, PlayerNetworkDataToMovement> clientIdToPlayerMovement;

    //TODO : Decouple this from the other thing but for rn just getting it working
    private static SaveObjectsManager currentSaveManager;

    private static ConcurrentQueue<BaseNetworkSendPacket> ExecuteOtherThreadRequestQueue;

    public static void AddClientIDToPlayer(int clientId, PlayerNetworkDataToMovement playerMov) {
        clientIdToPlayerMovement.Add(clientId, playerMov);
    }

    public static void AddPlayer(Guid playerGUID, GameObject player) {
        players.Add(playerGUID, player);
    }

    public static void EnqueMainThreadRequest(BaseNetworkSendPacket bNSP) {
        ExecuteOtherThreadRequestQueue.Enqueue(bNSP);
    }

    public static ReadOnlyDictionary<int, TcpClientData> GetTcpClientData() {
        return new ReadOnlyDictionary<int, TcpClientData>(tcpClients);
    }

    public static ReadOnlyDictionary<int, PlayerNetworkDataToMovement> GetClientIdToPlayerGO() {
        return new ReadOnlyDictionary<int, PlayerNetworkDataToMovement>(clientIdToPlayerMovement);
    }

    public static void StartServer(ushort port) {
        Debug.Log($"Starting server with port {port}");
        tcpListener = new TcpListener(IPAddress.Any, port);
        tcpListener.Start();
        tcpListener.BeginAcceptSocket(onAcceptTcpClient, null);

        udpListener = new UdpClient(port);
        udpListener.BeginReceive(onRecieveDataUdp, null);
        Debug.Log("Estabilished udp listener");
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
        }
        catch (Exception e) {
            Debug.LogError("Error handeling tcpAccept lmao : " + e.Message);
        }
    }

    static void onRecieveDataTcp(IAsyncResult result) {
        TcpClientData data = (TcpClientData)result.AsyncState;
        try {
            int bytesRead = data.Stream.EndRead(result);

            Debug.Log("Bytes read from the amogus : " + bytesRead);

            if (bytesRead > 0) {
                //Clip data to bytes we want
                byte[] tcpBytes = new byte[bytesRead];
                for (int i = 0; i < bytesRead; i++) {
                    tcpBytes[i] = data.Buffer[i];
                }

                using (PacketWrapper packet = new PacketWrapper(tcpBytes)) {
                    //TODO : Account for processing requiring me to make this like the client
                            //where it parses through the bytes each time for each new request
                    bool doesPlayerAlreadyExsistInScene = false;

                    foreach (GameObject saveObject in SaveablePrefabManager.SaveablePrefabs) {
                        BaseNetworkSendPacket packetToSend = null;

                        PlayerGUIDInfo playerInfo = saveObject.GetComponent<PlayerGUIDInfo>();
                        PrefabSaveInfo saveInfo = saveObject.GetComponent<PrefabSaveInfo>();
                        if (playerInfo != null && playerInfo.guid == new Guid(packet.GetBytes().Take(16).ToArray())) {
                            packetToSend = new AddPlayerFromNetwork(saveInfo.NetworkId,
                                saveObject.transform.position, saveObject.transform.eulerAngles.z);
                            doesPlayerAlreadyExsistInScene = true;
                        }
                        else {
                            packetToSend = addTcpBasic(saveObject);
                        }

                        SendToTcp(data.ClientId, packetToSend.GetPacketBytes());
                    }

                    if (!doesPlayerAlreadyExsistInScene) {
                        ExecuteOtherThreadRequestQueue.Enqueue(new PlayerAddPacket(packet.GetBytes(), currentSaveManager, data.ClientId));
                    }
                }

                data.Buffer = new byte[MAX_TCP_BUFFER_ALLOCATION];
                data.Stream.BeginRead(data.Buffer, 0, MAX_TCP_BUFFER_ALLOCATION, onRecieveDataTcp, data);
            }
            else {
                removeTcpClient(data.ClientId);
                IPEndPoint clientEndPoint = (IPEndPoint)data.Client.Client.RemoteEndPoint;

                Debug.Log("Peacefully lost client : " + clientEndPoint.Address + ":" + clientEndPoint.Port);
                Debug.Log("Peacefully lost client");
                //Probably don't want to do this as we want player to persist between sessions
                //SaveablePrefabManager.DeletePrefab(players[data.ClientId]);
                //Dont think we need to remove this.. No probably keeping track of player who left
                //players.Remove(data.ClientId);
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
            Debug.Log("Attempting to remove client");
            //Probably don't want to do this as we want player to persist between sessions
            //Fix it so there's a time out so player will get auto disconnected..
            //SaveablePrefabManager.DeletePrefab(players[data.ClientId]);
            //players.Remove(data.ClientId);
            data.Client.Close();
        }
        catch (Exception e) {
            Debug.LogError("Error when trying to do the tcp stuff " + e.Message);
        }
    }

    static void onRecieveDataUdp(IAsyncResult result) {
        try {
            IPEndPoint udpEndPoint = new IPEndPoint(IPAddress.Any, 0);

            byte[] recievedData = udpListener.EndReceive(result, ref udpEndPoint);

            udpListener.BeginReceive(onRecieveDataUdp, null);

            TcpClientData clientData = null;

            foreach (var client in tcpClients.Values) {
                IPEndPoint tcpEndPoint = (IPEndPoint)client.Client.Client.RemoteEndPoint;
                if (tcpEndPoint.Address.Equals(udpEndPoint.Address)) {
                    //Found matching client by IP adress. set udp endpoint
                    clientData = client;
                    client.EndPoint = udpEndPoint;
                    break;
                }
            }

            using (PacketWrapper packet = new PacketWrapper(recievedData)) {

            byte[] packetBytes = packet.GetBytes();

            ExecuteOtherThreadRequestQueue.Enqueue(new PlayerMovementPacket(packetBytes, clientData.ClientId));
            }

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

    //Dont like exposing this but oh well
    public static void SendToTcp(int client, byte[] bytesToAdd) {
        using (PacketWrapper packet = new PacketWrapper()) {
            packet.AddBytes(bytesToAdd);
            byte[] data = packet.GetBytes();
            tcpClients[client].Stream.Write(data, 0, data.Length);
        }
    }


    static void SendToAllWithTcp(byte[] bytesToAdd) {
        using (PacketWrapper packet = new PacketWrapper()) {
            packet.AddBytes(bytesToAdd);
            byte[] data = packet.GetBytes();
            if (tcpClients != null) {
                foreach (var client in tcpClients.Values) {
                    client.Stream.Write(data, 0, data.Length);
                }
            }
        }
    }

    static void SendToAllWithUdp(byte[] bytes) {
        using (PacketWrapper packet = new PacketWrapper()) {
            packet.AddBytes(bytes);
            byte[] data = packet.GetBytes();

            //Debug.Log(String.Format("(UDP) Sending packets to {0} clients", tcpClients.Count));
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

    private void OnEnable() {
        if (DataService.IsMultiplayer && DataService.IsLocalSave) {
            EventManager.PrefabAddedToScene += addTcpObject;
            EventManager.PrefabRemovedFromScene += removeTcpObject;
        }
    }

    private void OnDisable() {
        if (DataService.IsMultiplayer && DataService.IsLocalSave) {
            EventManager.PrefabAddedToScene -= addTcpObject;
            EventManager.PrefabRemovedFromScene -= removeTcpObject;
        }
    }

    void Start()
    {
        if (instance == null && DataService.IsMultiplayer && DataService.IsLocalSave) {
            currentSaveManager = FindAnyObjectByType<SaveObjectsManager>();
            instance = this;
            tcpClients = new Dictionary<int, TcpClientData>();
            players = new Dictionary<Guid, GameObject>();
            ExecuteOtherThreadRequestQueue = new ConcurrentQueue<BaseNetworkSendPacket>();
            clientsLock = new object();

            StartServer(DataService.PortOfServerWeAreHosting);
        }
        else {
            Destroy(instance);
        }
    }

    static AddNetworkGameObject addTcpBasic(GameObject prefabToAdd) {
        PrefabSaveInfo curSaveInfo = prefabToAdd.GetComponent<PrefabSaveInfo>();
        AddNetworkGameObject addNewGO = new AddNetworkGameObject(curSaveInfo.NetworkId,
            curSaveInfo.PrefabId, prefabToAdd.transform.position, prefabToAdd.transform.rotation.eulerAngles.z,
             currentSaveManager.PrefabGetByteArray(prefabToAdd, false, false).ToArray());
        return addNewGO;
    }

    void addTcpObject(GameObject prefabToAdd) {
        SendToAllWithTcp(addTcpBasic(prefabToAdd).GetPacketBytes());
    }

    void removeTcpObject(GameObject prefabToRemove) {
        PrefabSaveInfo saveInfo = prefabToRemove.GetComponent<PrefabSaveInfo>();
        SendToAllWithTcp(new RemoveNetworkGameObject(saveInfo.NetworkId).GetPacketBytes());
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

            if (Input.GetKeyDown(KeyCode.X)) {
                StopServer();
            }
            else if (Input.GetKeyDown(KeyCode.Y)) {
                Debug.Log("Summoning Scrombolo Bombolo");
                SaveablePrefabManager.CreatePrefab("Scrombolo_Bombolo", Vector3.zero, 0f);
            }
            else if (Input.GetKeyDown(KeyCode.U)) {
            }

            foreach (KeyValuePair<uint, GameObject> item in SaveablePrefabManager.NetworkIdsPrefabs) {
                SendToAllWithUdp(new UpdateNetworkGameObject(currentSaveManager, item.Key, item.Value).GetPacketBytes());
            }

            DispatchCommandsOnMainThread();
        }
    }
}
