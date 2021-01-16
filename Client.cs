using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;


public class Client : MonoBehaviour {
    public static Client instance;
    public static int dataBufferSize = 4096;
    public string ip = "127.0.0.1";
    public int port = 26950;
    public int ClientId { get; set; }
    public TCP tcp;

    private delegate void PacketHandler(Packet packet);

    private static Dictionary<int, PacketHandler> _packetHandlers;

    private void Awake() {
        print("Thread manager init");
        if (instance == null) {
            instance = this;
        } else if(instance != this) {
            Debug.Log("Instance already exists, destroying object");
            Destroy(this);
        }
    }

    private void Start() {
        tcp = new TCP();
    }

    public void ConnectToServer() {
        InitClientData();
        tcp.Connect();
    }

    public class TCP {
        private Packet receivedData;
        public TcpClient socket;
        private NetworkStream stream;
        private byte[] receiveBuffer;

        public void Connect() {
            receiveBuffer = new byte[dataBufferSize];
            socket = new TcpClient {
                ReceiveBufferSize = dataBufferSize,
                SendBufferSize = dataBufferSize
            };
            socket.BeginConnect(instance.ip, instance.port, ConnectCallBack, socket);
        }

        private void ConnectCallBack(IAsyncResult result) {
            socket.EndConnect(result);
            if (!socket.Connected) {
                return;
            }

            stream = socket.GetStream();
            receivedData = new Packet();
            stream.BeginRead(receiveBuffer, 0, dataBufferSize, ReceiveCallBack, null);
        }

        private void ReceiveCallBack(IAsyncResult result) {
            try {
                int byteLength = stream.EndRead(result);
                if (byteLength <= 0) {
                    // Todo : Disconnect the client
                    return;
                }
                byte[] data = new byte[byteLength];
                Array.Copy(receiveBuffer, data, byteLength);
                    
                // Handle the data
                receivedData.Reset(HandleData(data));
                stream.BeginRead(receiveBuffer,0,  dataBufferSize, ReceiveCallBack, null);
            } catch (Exception exception) {
                Console.WriteLine($"An error occured : {exception}");
                // Todo : Disconnect the client
            }
        }

        private bool HandleData(byte[] data) {
            int packetLength = 0;
            receivedData.SetBytes(data);
            if (receivedData.UnreadLength() > 4) {
                packetLength = receivedData.ReadInt();
                if (packetLength <= 0) return true;
            }

            while (packetLength > 0 && packetLength <= receivedData.UnreadLength()) {
                byte[] packetBytes = receivedData.ReadBytes(packetLength);
                ThreadManager.ExecuteOnMainThread(() => {
                    using (Packet packet = new Packet(packetBytes)) {
                        int packetId = packet.ReadInt();
                        _packetHandlers[packetId](packet);
                    }
                });
                packetLength = 0;
                if (receivedData.UnreadLength() > 4) {
                    packetLength = receivedData.ReadInt();
                    if (packetLength <= 0) return true;
                }
            }
            if(packetLength <= 1) return true;
            return false;
        }
    }

    private void InitClientData() {
        _packetHandlers = new Dictionary<int, PacketHandler>()  {
            {(int) ServerPacketsEnum.Welcome, ClientHandle.Welcome}
        };
        Debug.Log("Initialised package handlers, packet handler length : " + _packetHandlers.Count);
    }
}
