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
using System.Linq;
using System.Net;
using System.Net.Sockets;

namespace Kermalis.PokemonBattleEngine.Network
{
    public sealed class PBEClient : IDisposable
    {
        public PBEBattle Battle { get; set; }

        private Socket _socket;
        private PBEEncryption _encryption;
        private byte[] _buffer;

        public IPEndPoint RemoteIP { get; private set; }
        public bool IsConnected { get; private set; }

        public event EventHandler Disconnected;
        public event EventHandler<Exception> Error;
        public event EventHandler<IPBEPacket> PacketReceived;

        public bool Connect(IPEndPoint ip, int millisecondsTimeout, PBEEncryption encryption = null)
        {
            if (ip == null)
            {
                throw new ArgumentNullException(nameof(ip));
            }
            if (millisecondsTimeout < -1)
            {
                throw new ArgumentException($"\"{nameof(millisecondsTimeout)}\" is invalid.");
            }

            Disconnect(true);
            _socket = new Socket(ip.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                if (_socket.BeginConnect(ip, null, null).AsyncWaitHandle.WaitOne(millisecondsTimeout))
                {
                    IsConnected = true;
                    RemoteIP = ip;
                    _encryption = encryption;
                    BeginReceive();
                    return true;
                }
            }
            catch (Exception ex)
            {
                NotifyError(ex);
            }
            Disconnect(false);
            return false;
        }
        public void Disconnect(bool notify)
        {
            if (IsConnected)
            {
                IsConnected = false;
                RemoteIP = null;
                _encryption = null;
                try
                {
                    _socket.Shutdown(SocketShutdown.Both);
                }
                catch (Exception ex)
                {
                    NotifyError(ex);
                }
                _socket.Dispose();
                _socket = null;
                if (notify)
                {
                    Disconnected?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        public void Send(IPBEPacket packet)
        {
            if (!IsConnected)
            {
                throw new InvalidOperationException("Socket not connected.");
            }
            if (packet == null)
            {
                throw new ArgumentNullException(nameof(packet));
            }
            byte[] data = packet.Data.ToArray();
            if (_encryption != null)
            {
                data = _encryption.Encrypt(data);
            }
            PBENetworkUtils.Send(data, _socket);
        }

        private void BeginReceive()
        {
            _socket.BeginReceive(_buffer = new byte[2], 0, 2, SocketFlags.None, OnReceiveLength, null);
        }
        private void OnReceiveLength(IAsyncResult ar)
        {
            if (IsConnected)
            {
                try
                {
                    if (_socket.Poll(0, SelectMode.SelectRead) && _socket.Available <= 0)
                    {
                        Disconnect(true);
                    }
                    else
                    {
                        ushort dataLength = (ushort)(_buffer[0] | (_buffer[1] << 8));
                        if (dataLength <= 0)
                        {
                            Disconnect(true);
                        }
                        else
                        {
                            _socket.BeginReceive(_buffer = new byte[dataLength], 0, dataLength, SocketFlags.None, OnReceiveData, null);
                        }
                    }
                }
                catch (Exception ex)
                {
                    NotifyError(ex);
                    Disconnect(true);
                }
            }
        }
        private void OnReceiveData(IAsyncResult ar)
        {
            if (IsConnected)
            {
                try
                {
                    if (_socket.Poll(0, SelectMode.SelectRead) && _socket.Available <= 0)
                    {
                        Disconnect(true);
                    }
                    else
                    {
                        byte[] data = _buffer;
                        if (_encryption != null)
                        {
                            data = _encryption.Decrypt(data);
                        }
                        PacketReceived?.Invoke(this, PBEPacketProcessor.CreatePacket(Battle, data));
                        BeginReceive();
                    }
                }
                catch (Exception ex)
                {
                    NotifyError(ex);
                    Disconnect(true);
                }
            }
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
            Disconnect(true);
        }
    }
}
