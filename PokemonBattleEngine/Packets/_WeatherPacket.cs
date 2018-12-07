using Ether.Network.Packets;
using Kermalis.PokemonBattleEngine.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Kermalis.PokemonBattleEngine.Packets
{
    public sealed class PBEWeatherPacket : INetPacket
    {
        public const short Code = 0x14;
        public IEnumerable<byte> Buffer { get; }

        public PBEWeather Weather { get; }
        public PBEWeatherAction WeatherAction { get; }

        public PBEWeatherPacket(PBEWeather weather, PBEWeatherAction weatherAction)
        {
            var bytes = new List<byte>();
            bytes.AddRange(BitConverter.GetBytes(Code));
            bytes.Add((byte)(Weather = weather));
            bytes.Add((byte)(WeatherAction = weatherAction));
            Buffer = BitConverter.GetBytes((short)bytes.Count).Concat(bytes);
        }
        public PBEWeatherPacket(byte[] buffer)
        {
            Buffer = buffer;
            using (var r = new BinaryReader(new MemoryStream(buffer)))
            {
                r.ReadInt16(); // Skip Code
                Weather = (PBEWeather)r.ReadByte();
                WeatherAction = (PBEWeatherAction)r.ReadByte();
            }
        }

        public void Dispose() { }
    }
}
