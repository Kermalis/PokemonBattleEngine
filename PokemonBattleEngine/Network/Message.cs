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
using System.Net.Sockets;

namespace Kermalis.PokemonBattleEngine.Network
{
    public sealed class PBEMessage
    {
        public byte[] Data { get; }
        public Socket Socket { get; }
        private readonly PBEEncryption _encryption;

        internal PBEMessage(byte[] data, Socket socket, PBEEncryption encryption)
        {
            Data = data;
            Socket = socket;
            _encryption = encryption;
        }

        public void Reply(byte[] data, bool encrypt = false)
        {
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
            NetworkUtils.Send(data, Socket);
        }
    }
}
