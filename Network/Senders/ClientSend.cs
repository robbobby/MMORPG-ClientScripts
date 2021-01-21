using UnityEngine;

namespace Network.Crud {
    public class ClientSend : ClientSendPackage {
        /// <summary>
        /// Add the package to the PacketId Enum,
        /// Write the sender for that package in the ServerSendPacket class
        /// Done - call the send when you need it
        /// </summary>

        #region PacketIds enum
        private enum PacketId {
            WelcomeReceived = 1,
            PlayerMovement
        }
        #endregion
        // ######################################################################################################## //
        #region Packets

        public static void WelcomeReceived() {
            using (Packet packet = new Packet((int) PacketId.WelcomeReceived)) {
                packet.Write(Client.SingletonInstance.clientId);
                packet.Write(UIManager.SingletonInstance.userNameField.text);
                SendTcpData(packet);
            }
        }
        #endregion

        public static void PlayerMovement(bool[] inputs) {
            Debug.Log("Attempting to send player movement package");
            using (Packet packet = new Packet((int) PacketId.PlayerMovement)) {
                packet.Write(inputs.Length);
                for (int i = 0; i < inputs.Length; i++) {
                    packet.Write(inputs[i]);
                }
                packet.Write(GameManager.players[Client.SingletonInstance.clientId].transform.rotation);
                SendUdpData(packet);
            }
        }
    }
}