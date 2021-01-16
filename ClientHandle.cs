using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientHandle : MonoBehaviour {
    public static void Welcome(Packet packet) {
        string message = packet.ReadString();
        int id = packet.ReadInt();
        
        Debug.Log($"Data from server : {message}");
        Client.instance.ClientId = id;
        // Todo: ### Reply welcome back to server ### //
    }
}
