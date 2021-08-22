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
using System.IO;
using System.Security.Cryptography;

namespace Kermalis.PokemonBattleEngine.Network
{
    public sealed class PBEEncryption
    {
        private readonly SymmetricAlgorithm _algorithm;

        public PBEEncryption(SymmetricAlgorithm algorithm)
        {
            _algorithm = algorithm;
        }

        public byte[] Encrypt(byte[] data)
        {
            _algorithm.GenerateIV();
            using (var ms = new MemoryStream())
            {
                ms.Write(_algorithm.IV, 0, _algorithm.IV.Length);
                using (var cs = new CryptoStream(ms, _algorithm.CreateEncryptor(_algorithm.Key, _algorithm.IV), CryptoStreamMode.Write))
                {
                    cs.Write(data, 0, data.Length);
                    cs.FlushFinalBlock();
                }
                return ms.ToArray();
            }
        }
        public byte[] Decrypt(byte[] data)
        {
            using (var ms = new MemoryStream(data))
            {
                byte[] iv = new byte[_algorithm.IV.Length];
                ms.Read(iv, 0, iv.Length);
                _algorithm.IV = iv;
                using (var cs = new CryptoStream(ms, _algorithm.CreateDecryptor(_algorithm.Key, _algorithm.IV), CryptoStreamMode.Read))
                {
                    byte[] decrypted = new byte[data.Length];
                    int byteCount = cs.Read(decrypted, 0, decrypted.Length);
                    byte[] ret = new byte[byteCount];
                    Buffer.BlockCopy(decrypted, 0, ret, 0, byteCount);
                    return ret;
                }
            }
        }
    }
}
