/* EasyTcp
 * 
 * Copyright (c) 2019 henkje
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 */

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace Kermalis.PokemonBattleEngine.Network
{
    public sealed class PBEServer : IDisposable
    {
        private class PBEClientTuple
        {
            public Socket Socket;
            public byte[] Buffer;
        }

        private Socket _listener;
        private PBEEncryption _encryption;
        private int _maxConnections;
        private ushort _maxDataSize;

        public bool IsRunning => _listener != null;
        private readonly HashSet<IPEndPoint> _bannedIPs = new HashSet<IPEndPoint>();
        private readonly HashSet<Socket> _connectedClients = new HashSet<Socket>();

        public event EventHandler<Socket> ClientConnected;
        public event EventHandler<Socket> ClientDisconnected;
        public event EventHandler<PBEMessage> MessageReceived;
        public event EventHandler<Exception> Error;
        public delegate void PBEClientRefusedEventHandler(object sender, IPEndPoint refusedClient, bool refusedForBan);
        public event PBEClientRefusedEventHandler ClientRefused;

        public void Start(IPAddress ip, ushort port, int maxConnections, PBEEncryption encryption = null, bool dualMode = false, ushort maxDataSize = 1024)
        {
            if (IsRunning)
            {
                throw new InvalidOperationException("Server is already running.");
            }
            if (ip == null)
            {
                throw new ArgumentNullException(nameof(ip));
            }
            if (port <= 0)
            {
                throw new ArgumentException($"\"{nameof(port)}\" must be greater than 0.");
            }
            if (maxConnections <= 0)
            {
                throw new ArgumentException($"\"{nameof(maxConnections)}\" must be greater than 0.");
            }
            if (maxDataSize <= 0)
            {
                throw new ArgumentException($"\"{nameof(maxDataSize)}\" must be greater than 0.");
            }

            _listener = new Socket(ip.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            if (ip.AddressFamily == AddressFamily.InterNetworkV6)
            {
                _listener.DualMode = dualMode;
            }
            _listener.Bind(new IPEndPoint(ip, port));
            _encryption = encryption;

            try
            {
                _maxConnections = maxConnections;
                _maxDataSize = maxDataSize;
                _listener.Listen(maxConnections);
                _listener.BeginAccept(OnClientConnected, null);
            }
            catch (Exception ex)
            {
                NotifyError(ex);
                Stop();
            }
        }
        public void Stop()
        {
            if (IsRunning)
            {
                try
                {
                    _listener.Shutdown(SocketShutdown.Both);
                }
                catch (Exception ex)
                {
                    NotifyError(ex);
                }
                _listener.Dispose();
                _listener = null;
            }
        }

        private void OnClientConnected(IAsyncResult ar)
        {
            if (!IsRunning)
            {
                return;
            }
            Socket client = null;
            try
            {
                client = _listener.EndAccept(ar);
                var clientIP = (IPEndPoint)client.RemoteEndPoint;
                bool isBanned;
                lock (_bannedIPs)
                {
                    isBanned = _bannedIPs.Contains(clientIP);
                }
                if (isBanned)
                {
                    RefuseClient(client, clientIP, true);
                }
                else
                {
                    int count;
                    lock (_connectedClients)
                    {
                        count = _connectedClients.Count;
                    }
                    if (count >= _maxConnections)
                    {
                        RefuseClient(client, clientIP, false);
                    }
                    else
                    {
                        lock (_connectedClients)
                        {
                            _connectedClients.Add(client);
                        }
                        ClientConnected?.Invoke(this, client);
                        BeginReceive(new PBEClientTuple { Socket = client });
                    }
                }
            }
            catch (Exception ex)
            {
                NotifyError(ex);
                if (client != null)
                {
                    CloseClient(client);
                }
            }
            _listener.BeginAccept(OnClientConnected, _listener);
        }

        private void BeginReceive(PBEClientTuple client)
        {
            byte[] buffer = new byte[2];
            client.Buffer = buffer;
            client.Socket.BeginReceive(buffer, 0, 2, SocketFlags.None, OnReceiveLength, client);
        }
        // TODO: Endianness
        private void OnReceiveLength(IAsyncResult ar)
        {
            var client = ar.AsyncState as PBEClientTuple;
            Socket socket = client.Socket;
            try
            {
                if (socket.Poll(0, SelectMode.SelectRead) && socket.Available <= 0)
                {
                    CloseClient(socket);
                }
                else
                {
                    ushort dataLength = BitConverter.ToUInt16(client.Buffer, 0);
                    if (dataLength <= 0 || dataLength > _maxDataSize)
                    {
                        CloseClient(socket);
                    }
                    else
                    {
                        socket.BeginReceive(client.Buffer = new byte[dataLength], 0, dataLength, SocketFlags.None, OnReceiveData, client);
                    }
                }
            }
            catch (Exception ex)
            {
                NotifyError(ex);
                CloseClient(socket);
            }
        }
        private void OnReceiveData(IAsyncResult ar)
        {
            var client = ar.AsyncState as PBEClientTuple;
            Socket socket = client.Socket;
            try
            {
                if (socket.Poll(0, SelectMode.SelectRead) && socket.Available <= 0)
                {
                    CloseClient(socket);
                }
                else
                {
                    MessageReceived?.Invoke(this, new PBEMessage(client.Buffer, socket, _encryption));
                    BeginReceive(client);
                }
            }
            catch (Exception ex)
            {
                NotifyError(ex);
                CloseClient(socket);
            }
        }

        private void RefuseClient(Socket client, IPEndPoint clientIP, bool isBanned)
        {
            client.Shutdown(SocketShutdown.Both);
            client.Dispose();
            ClientRefused?.Invoke(this, clientIP, isBanned);
        }
        private void CloseClient(Socket client)
        {
            lock (_connectedClients)
            {
                _connectedClients.Remove(client);
            }
            ClientDisconnected?.Invoke(this, client);
            client.Shutdown(SocketShutdown.Both);
            client.Dispose();
        }

        private void NotifyError(Exception ex)
        {
            if (Error != null)
            {
                Error(this, ex);
            }
            else
            {
                throw ex;
            }
        }

        public void Dispose()
        {
            Stop();
        }
    }
}
