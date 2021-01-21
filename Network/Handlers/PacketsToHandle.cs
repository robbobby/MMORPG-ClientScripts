using System.Collections.Generic;

namespace Network.Handlers {
    /// <summary>
    /// Add new packages to the dictionary in GetPackages(),
    /// Add the package to the ClientPackets Enum,
    /// In the ServerHandle.cs write the handler for that package
    /// Done - the listener will now be able to handle the package if it arrives 
    /// </summary>
    public static class PacketsToHandle {
        #region PacketIds enum

        private enum PacketId {
            Welcome = 1,
            SpawnPlayer,
            PlayerPosition,
            PlayerRotation
        }
        #endregion
        // ######################################################################################################## //
        #region Packets
        public static Dictionary<int, Client.PacketHandler> GetPackages() {
            return new Dictionary<int, Client.PacketHandler> {
                {(int) PacketId.Welcome, ClientHandle.Welcome},
                {(int) PacketId.SpawnPlayer, ClientHandle.SpawnPlayer},
                {(int) PacketId.PlayerPosition, ClientHandle.PlayerPosition},
                {(int) PacketId.PlayerRotation, ClientHandle.PlayerRotation}
            };
        }
        #endregion

    }
}