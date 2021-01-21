using System.Net;
using Network.Crud;
using UnityEngine;

public class ClientHandle : MonoBehaviour {
    
    // Add new listener logic here
    public static void Welcome(Packet packet) {
        string message = packet.ReadString();
        int id = packet.ReadInt();
        
        Debug.Log($"Data from server : {message}");
        Client.SingletonInstance.clientId = id;
        ClientSend.WelcomeReceived();
        Client.SingletonInstance.Udp.Connect(((IPEndPoint)Client.SingletonInstance.Tcp.Socket.Client.LocalEndPoint).Port);
    }
    
    public static void SpawnPlayer(Packet packet) {
        int id = packet.ReadInt();
        string username = packet.ReadString();
        Vector3 position = packet.ReadVector3();
        Quaternion rotation = packet.ReadRotation();
        
        GameManager.singletonInstance.SpawnPlayer(id, username, position, rotation);
    }

    public static void PlayerPosition(Packet packet) {
        int id = packet.ReadInt();
        GameManager.players[id].transform.position = packet.ReadVector3();
    }

    public static void PlayerRotation(Packet packet) {
        int id = packet.ReadInt();
        GameManager.players[id].transform.rotation = packet.ReadRotation();
    }
}
