using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class ClientHandle : MonoBehaviour {
    public static void Welcome(Packet packet) {
        string message = packet.ReadString();
        int id = packet.ReadInt();
        
        Debug.Log($"Data from server : {message}");
        Client.singletonInstance.clientId = id;
        ClientSend.WelcomeReceived();
        Client.singletonInstance.udp.Connect(((IPEndPoint)Client.singletonInstance.tcp.socket.Client.LocalEndPoint).Port);
    }

    public static void UdpTest(Packet packet) {
        string message = packet.ReadString();
        Debug.Log($"RECEIVED PACKAGE VIA UDP. Contains message: {message}");
        ClientSend.UdpTestReceived();
    }
}
