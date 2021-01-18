using UnityEngine;

public class ClientSend : MonoBehaviour{
    private static void SendTcpData(Packet packet) {
        packet.WriteLength();
        Debug.Log("Sending Tcp package");
        Client.singletonInstance.tcp.SendData(packet);
    }

    private static void SendUdpData(Packet packet) {
        packet.WriteLength();
        Debug.Log("Sending Udp package");
        Client.singletonInstance.udp.SendData(packet);
    }
    
    #region Packets

    public static void WelcomeReceived() {
        using (Packet packet = new Packet((int) ClientPackets.WelcomeReceived)) {
            packet.Write(Client.singletonInstance.clientId);
            packet.Write(UIManager.SingletonInstance.userNameField.text);
            SendTcpData(packet);
        }
    }
    #endregion

    public static void UdpTestReceived() {
        using (Packet packet = new Packet((int) ClientPackets.UdpTestReceived)) {
            packet.Write("Received a UDP Package.");
            SendUdpData(packet);
        }
    }
}
