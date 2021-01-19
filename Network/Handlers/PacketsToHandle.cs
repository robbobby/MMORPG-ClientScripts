using System.Collections.Generic;

namespace Network.Handlers {
    /// <summary>
    /// Add new packages to the dictionary in GetPackages(),
    /// Add the package to the ClientPackets Enum,
    /// In the ServerHandle.cs write the handler for that package
    /// </summary>
    public static class PacketsToHandle {
        public static Dictionary<int, Client.PacketHandler> GetPackages() {
            return new Dictionary<int, Client.PacketHandler> {
                {(int) ServerPacketsEnum.Welcome, ClientHandle.Welcome},
                {(int) ServerPacketsEnum.UdpTest, ClientHandle.UdpTest}
            };
        }

        private enum ServerPacketsEnum {
            Welcome = 1,
            UdpTest
        }
    }
}