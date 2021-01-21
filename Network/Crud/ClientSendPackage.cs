using UnityEngine;

namespace Network.Crud {
    public abstract class ClientSendPackage : MonoBehaviour{
        protected static void SendTcpData(Packet packet) {
            packet.WriteLength();
            Debug.Log("Sending Tcp package");
            Client.SingletonInstance.Tcp.SendData(packet);
        }

        protected static void SendUdpData(Packet packet) {
            packet.WriteLength();
            Debug.Log("Sending Udp package");
            Client.SingletonInstance.Udp.SendData(packet);
        }
    }
}