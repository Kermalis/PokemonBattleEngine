using System;
using System.Collections.Generic;
using System.Net.Sockets;

namespace Kermalis.PokemonBattleEngine.Network
{
    internal static class PBENetworkUtils
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

        private static SocketAsyncEventArgs CreateArgs(byte[] data)
        {
            int len = data.Length;
            if (len <= 0 || len > ushort.MaxValue)
            {
                throw new ArgumentException($"Data length must be greater than 0 bytes and must not exceed {ushort.MaxValue} bytes.");
            }
            byte[] message = new byte[len + 2];
            message[0] = (byte)(len & 0xFF); // Convert length to little endian each time regardless of system endianness
            message[1] = (byte)(len >> 8);
            Buffer.BlockCopy(data, 0, message, 2, len);
            var e = new SocketAsyncEventArgs();
            e.SetBuffer(message, 0, message.Length);
            return e;
        }
    }
}
