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
using System.Net;
using System.Net.Sockets;

namespace Kermalis.PokemonBattleEngine.Network
{
    public sealed class PBEClient : IDisposable
    {
        private Socket _socket;
        private PBEEncryption _encryption;
        private ushort _maxDataSize;
        private byte[] _buffer;

        public event EventHandler<PBEMessage> MessageReceived;
        public event EventHandler<PBEClient> Disconnected;
        public event EventHandler<Exception> Error;

        public bool Connect(IPAddress ip, ushort port, TimeSpan timeout, PBEEncryption encryption = null, ushort maxDataSize = 1024)
        {
            if (ip == null)
            {
                throw new ArgumentNullException(nameof(ip));
            }
            if (port <= 0)
            {
                throw new ArgumentException($"\"{nameof(port)}\" must be greater than 0.");
            }
            if (timeout.Ticks <= 0)
            {
                throw new ArgumentException($"\"{nameof(timeout)}\" is invalid.");
            }
            if (maxDataSize <= 0)
            {
                throw new ArgumentException($"\"{nameof(maxDataSize)}\" must be greater than 0.");
            }

            Disconnect(true);
            _socket = new Socket(ip.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                _socket.BeginConnect(ip, port, null, null).AsyncWaitHandle.WaitOne(timeout);
                _encryption = encryption;
                _maxDataSize = maxDataSize;
                BeginReceive(_socket);
                return true;
            }
            catch
            {
                Disconnect(false);
                return false;
            }
        }
        public void Disconnect(bool notifyOnDisconnect)
        {
            if (_socket == null)
            {
                return;
            }
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
            if (notifyOnDisconnect)
            {
                Disconnected?.Invoke(this, this);
            }
        }

        public void Send(byte[] data, bool encrypt = false)
        {
            if (_socket == null)
            {
                throw new InvalidOperationException("Socket not connected.");
            }
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }
            if (encrypt)
            {
                if (_encryption == null)
                {
                    throw new ArgumentNullException(nameof(_encryption));
                }
                data = _encryption.Encrypt(data);
            }
            NetworkUtils.Send(data, _socket);
        }

        private void BeginReceive(Socket socket)
        {
            socket.BeginReceive(_buffer = new byte[2], 0, 2, SocketFlags.None, OnReceiveLength, socket);
        }
        // TODO: Endianness
        private void OnReceiveLength(IAsyncResult ar)
        {
            var socket = ar.AsyncState as Socket;
            try
            {
                if (socket.Poll(0, SelectMode.SelectRead) && socket.Available <= 0)
                {
                    Disconnect(true);
                }
                else
                {
                    ushort dataLength = BitConverter.ToUInt16(_buffer, 0);
                    if (dataLength <= 0 || dataLength > _maxDataSize)
                    {
                        Disconnect(true);
                    }
                    else
                    {
                        socket.BeginReceive(_buffer = new byte[dataLength], 0, dataLength, SocketFlags.None, OnReceiveData, socket);
                    }
                }
            }
            catch (Exception ex)
            {
                NotifyError(ex);
                Disconnect(true);
            }
        }
        private void OnReceiveData(IAsyncResult ar)
        {
            var socket = ar.AsyncState as Socket;
            try
            {
                if (socket.Poll(0, SelectMode.SelectRead) && socket.Available <= 0)
                {
                    Disconnect(true);
                }
                else
                {
                    MessageReceived?.Invoke(this, new PBEMessage(_buffer, socket, _encryption));
                    BeginReceive(socket);
                }
            }
            catch (Exception ex)
            {
                NotifyError(ex);
                Disconnect(true);
            }
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
            Disconnect(true);
        }
    }
}
