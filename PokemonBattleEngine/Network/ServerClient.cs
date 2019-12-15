using Kermalis.PokemonBattleEngine.Packets;
using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;

namespace Kermalis.PokemonBattleEngine.Network
{
    public sealed class PBEServerClient
    {
        internal readonly Socket Socket;
        internal byte[] Buffer;

        private readonly PBEEncryption _encryption;

        public IPEndPoint IP { get; }
        public bool IsConnected { get; internal set; }

        public EventHandler<IPBEPacket> MessageReceived;

        internal PBEServerClient(Socket socket, PBEEncryption encryption)
        {
            Socket = socket;
            IP = (IPEndPoint)socket.RemoteEndPoint;
            _encryption = encryption;
        }

        public void Send(IPBEPacket packet)
        {
            if (packet == null)
            {
                throw new ArgumentNullException(nameof(packet));
            }
            byte[] data = packet.Data.ToArray();
            if (_encryption != null)
            {
                data = _encryption.Encrypt(data);
            }
            PBENetworkUtils.Send(data, Socket);
        }

        internal void FireMessageReceived(IPBEPacket packet)
        {
            MessageReceived?.Invoke(this, packet);
        }
    }
}
