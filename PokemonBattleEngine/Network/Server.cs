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

using Kermalis.PokemonBattleEngine.Battle;
using Kermalis.PokemonBattleEngine.Packets;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace Kermalis.PokemonBattleEngine.Network
{
    public sealed class PBEServer : IDisposable
    {
        public PBEBattle Battle { get; set; }

        private Socket _listener;
        private PBEEncryption _encryption;
        private int _maxConnections;

        public bool IsRunning => _listener != null;
        private readonly HashSet<IPEndPoint> _bannedIPs = new HashSet<IPEndPoint>();
        private readonly HashSet<PBEServerClient> _connectedClients = new HashSet<PBEServerClient>();

        public event EventHandler<PBEServerClient> ClientConnected;
        public event EventHandler<PBEServerClient> ClientDisconnected;
        public event EventHandler<Exception> Error;
        public delegate void PBEClientRefusedEventHandler(object sender, IPEndPoint refusedIP, bool refusedForBan);
        public event PBEClientRefusedEventHandler ClientRefused;

        public void Start(IPEndPoint ip, int maxConnections, PBEEncryption encryption = null, bool dualMode = false)
        {
            if (IsRunning)
            {
                throw new InvalidOperationException("Server is already running.");
            }
            if (ip == null)
            {
                throw new ArgumentNullException(nameof(ip));
            }
            if (maxConnections <= 0)
            {
                throw new ArgumentException($"\"{nameof(maxConnections)}\" must be greater than 0.");
            }

            _listener = new Socket(ip.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            if (ip.AddressFamily == AddressFamily.InterNetworkV6)
            {
                _listener.DualMode = dualMode;
            }
            _listener.Bind(ip);
            _encryption = encryption;

            try
            {
                _maxConnections = maxConnections;
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

        public void SendToAll(IPBEPacket packet)
        {
            foreach (PBEServerClient client in _connectedClients)
            {
                client.Send(packet);
            }
        }

        private void OnClientConnected(IAsyncResult ar)
        {
            if (!IsRunning)
            {
                return;
            }
            PBEServerClient client = null;
            try
            {
                Socket clientSocket = _listener.EndAccept(ar);
                client = new PBEServerClient(clientSocket, _encryption);
                var clientIP = (IPEndPoint)clientSocket.RemoteEndPoint;
                bool isBanned;
                lock (_bannedIPs)
                {
                    isBanned = _bannedIPs.Contains(clientIP);
                }
                if (isBanned)
                {
                    RefuseClient(clientSocket, clientIP, true);
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
                        RefuseClient(clientSocket, clientIP, false);
                    }
                    else
                    {
                        lock (_connectedClients)
                        {
                            _connectedClients.Add(client);
                        }
                        ClientConnected?.Invoke(this, client);
                        BeginReceive(client);
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

        private void BeginReceive(PBEServerClient client)
        {
            byte[] buffer = new byte[2];
            client.Buffer = buffer;
            client.Socket.BeginReceive(buffer, 0, 2, SocketFlags.None, OnReceiveLength, client);
        }
        private void OnReceiveLength(IAsyncResult ar)
        {
            var client = ar.AsyncState as PBEServerClient;
            Socket socket = client.Socket;
            try
            {
                if (socket.Poll(0, SelectMode.SelectRead) && socket.Available <= 0)
                {
                    CloseClient(client);
                }
                else
                {
                    ushort dataLength = (ushort)(client.Buffer[0] | (client.Buffer[1] << 8));
                    if (dataLength <= 0)
                    {
                        CloseClient(client);
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
                CloseClient(client);
            }
        }
        private void OnReceiveData(IAsyncResult ar)
        {
            var client = ar.AsyncState as PBEServerClient;
            Socket socket = client.Socket;
            try
            {
                if (socket.Poll(0, SelectMode.SelectRead) && socket.Available <= 0)
                {
                    CloseClient(client);
                }
                else
                {
                    byte[] data = client.Buffer;
                    if (_encryption != null)
                    {
                        data = _encryption.Decrypt(data);
                    }
                    client.FireMessageReceived(PBEPacketProcessor.CreatePacket(Battle, data));
                    BeginReceive(client);
                }
            }
            catch (Exception ex)
            {
                NotifyError(ex);
                CloseClient(client);
            }
        }

        private void RefuseClient(Socket socket, IPEndPoint clientIP, bool isBanned)
        {
            socket.Shutdown(SocketShutdown.Both);
            socket.Dispose();
            ClientRefused?.Invoke(this, clientIP, isBanned);
        }
        public bool CloseClient(PBEServerClient client)
        {
            bool b;
            lock (_connectedClients)
            {
                b = _connectedClients.Remove(client);
            }
            if (b)
            {
                ClientDisconnected?.Invoke(this, client);
                client.Socket.Shutdown(SocketShutdown.Both);
                client.Socket.Dispose();
            }
            return b;
        }

        private void NotifyError(Exception ex)
        {
            if (Error != null)
            {
                Error.Invoke(this, ex);
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
