using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;


public class Client : MonoBehaviour {
    public static Client singletonInstance;
    public static int dataBufferSize = 4096;
    public string ip = "127.0.0.1";
    public int port = 26950;
    public int clientId;
    public TCP tcp;
    public UDP udp;

    private delegate void PacketHandler(Packet packet);

    private static Dictionary<int, PacketHandler> _packetHandlers;

    private void Awake() {
        print("Thread manager init");
        if (singletonInstance == null) {
            singletonInstance = this;
        } else if(singletonInstance != this) {
            Debug.Log("Instance already exists, destroying object");
            Destroy(this);
        }
    }

    private void Start() {
        tcp = new TCP();
        udp = new UDP();
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
            socket.BeginConnect(singletonInstance.ip, singletonInstance.port, ConnectCallBack, socket);
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

        public void SendData(Packet packet) {
            try {
                if (socket != null) {
                    stream.BeginWrite(packet.ToArray(), 0, packet.Length(), null, null);
                }
            } catch(Exception exception) {
                Debug.Log($"Error sending packet : -- Error:\n{exception}");
            }
        }
    }

    public class UDP {
        public UdpClient socket;
        public IPEndPoint endPoint;

        public UDP() {
            endPoint = new IPEndPoint(IPAddress.Parse(singletonInstance.ip), singletonInstance.port);
        }

        public void Connect(int localPort) {
            socket = new UdpClient(localPort);
            socket.Connect(endPoint);
            socket.BeginReceive(ReceiveCallback, null);

            using (Packet packet = new Packet()) {
                SendData(packet);
            }
        }

        public void SendData(Packet packet) {
            try {
                packet.InsertInt(singletonInstance.clientId);
                if (socket != null) {
                    socket.BeginSend(packet.ToArray(),
                        packet.Length(), null, null);
                }
            } catch {
                Debug.Log($"Error: sending UDP Data to server: -- Error \n");  
            }
        }

        
        private void ReceiveCallback(IAsyncResult result) {
            try {
                byte[] data = socket.EndReceive(result, ref endPoint);
                socket.BeginReceive(ReceiveCallback, null);

                if (data.Length < 4) return; // Todo : ### Disconnect ### //
                HandleData(data);
            } catch (Exception exception) {
                Debug.Log($"Error in UDP callback -- Error: \n{exception}");
                // Todo : ### Disconnect ### //
            }
        }

        private void HandleData(byte[] data) {
            using (Packet packet = new Packet(data)) {
                int packetLength = packet.ReadInt();
                data = packet.ReadBytes(packetLength);
            }
                
            ThreadManager.ExecuteOnMainThread(() => {
                using (Packet packet = new Packet(data)) {
                    int packetId = packet.ReadInt();
                    _packetHandlers[packetId](packet);
                }    
            });
        }
    }

    private void InitClientData() {
        _packetHandlers = new Dictionary<int, PacketHandler>()  {
            {(int) ServerPacketsEnum.Welcome, ClientHandle.Welcome},
            {(int) ServerPacketsEnum.UdpTest, ClientHandle.UdpTest}
        };
        Debug.Log("Initialised package handlers, packet handler length : " + _packetHandlers.Count);
    }
}
