using Ether.Network.Packets;
using Kermalis.PokemonBattleEngine.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Kermalis.PokemonBattleEngine.Packets
{
    public sealed class PWeatherPacket : INetPacket
    {
        public const short Code = 0x14;
        public IEnumerable<byte> Buffer { get; }
        
        public readonly PWeather Weather;
        public readonly PWeatherAction Action;

        public PWeatherPacket(PWeather weather, PWeatherAction action)
        {
            var bytes = new List<byte>();
            bytes.AddRange(BitConverter.GetBytes(Code));
            bytes.Add((byte)(Weather = weather));
            bytes.Add((byte)(Action = action));
            Buffer = BitConverter.GetBytes((short)bytes.Count).Concat(bytes);
        }
        public PWeatherPacket(byte[] buffer)
        {
            Buffer = buffer;
            using (var r = new BinaryReader(new MemoryStream(buffer)))
            {
                r.ReadInt16(); // Skip Code
                Weather = (PWeather)r.ReadByte();
                Action = (PWeatherAction)r.ReadByte();
            }
        }

        public void Dispose() { }
    }
}
