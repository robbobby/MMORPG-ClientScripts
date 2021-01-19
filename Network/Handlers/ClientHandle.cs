﻿using System.Collections;
using System.Collections.Generic;
using System.Net;
using Network.Crud;
using UnityEngine;

public class ClientHandle : MonoBehaviour {
    public static void Welcome(Packet packet) {
        string message = packet.ReadString();
        int id = packet.ReadInt();
        
        Debug.Log($"Data from server : {message}");
        Client.SingletonInstance.clientId = id;
        ClientSendPacket.WelcomeReceived();
        Client.SingletonInstance.Udp.Connect(((IPEndPoint)Client.SingletonInstance.Tcp.Socket.Client.LocalEndPoint).Port);
    }

    public static void UdpTest(Packet packet) {
        string message = packet.ReadString();
        Debug.Log($"RECEIVED PACKAGE VIA UDP. Contains message: {message}");
        ClientSendPacket.UdpTestReceived();
    }
}