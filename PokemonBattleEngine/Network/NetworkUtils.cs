using System;
using System.Collections.Generic;
using System.Net.Sockets;

namespace Kermalis.PokemonBattleEngine.Network
{
    internal static class NetworkUtils
    {
        public static void Send(byte[] data, Socket socket)
        {
            using (SocketAsyncEventArgs e = CreateArgs(data))
            {
                socket.SendAsync(e);
            }
        }
        public static void Send(byte[] data, IEnumerable<Socket> sockets)
        {
            using (SocketAsyncEventArgs e = CreateArgs(data))
            {
                foreach (Socket socket in sockets)
                {
                    socket.SendAsync(e);
                }
            }
        }

        // TODO: Endianness
        private static SocketAsyncEventArgs CreateArgs(byte[] data)
        {
            byte[] message = new byte[data.Length + 2];
            Buffer.BlockCopy(BitConverter.GetBytes((ushort)data.Length), 0, message, 0, 2);
            Buffer.BlockCopy(data, 0, message, 2, data.Length);
            var e = new SocketAsyncEventArgs();
            e.SetBuffer(message, 0, message.Length);
            return e;
        }
    }
}
