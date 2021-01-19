using UnityEngine;

namespace Network.Crud {
    public class ClientSendPacket : ClientSend {

        #region Packets

        public static void WelcomeReceived() {
            using (Packet packet = new Packet((int) ClientPackets.WelcomeReceived)) {
                packet.Write(Client.SingletonInstance.clientId);
                packet.Write(UIManager.SingletonInstance.userNameField.text);
                SendTcpData(packet);
            }
        }
        public static void UdpTestReceived() {
            using (Packet packet = new Packet((int) ClientPackets.UdpTestReceived)) {
                packet.Write("Received a UDP Package.");
                SendUdpData(packet);
            }
        }

        #endregion
    }
};