using Ether.Network.Packets;
using Kermalis.PokemonBattleEngine.Battle;
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
        public PBEFieldPosition DamageVictim { get; } // PBEFieldPosition.None means no victim
        public PBETeam DamageVictimTeam { get; } // null means no victim

        public PBEWeatherPacket(PBEWeather weather, PBEWeatherAction weatherAction, PBEPokemon damageVictim)
        {
            var bytes = new List<byte>();
            bytes.AddRange(BitConverter.GetBytes(Code));
            bytes.Add((byte)(Weather = weather));
            bytes.Add((byte)(WeatherAction = weatherAction));
            bytes.Add((byte)(DamageVictim = damageVictim == null ? PBEFieldPosition.None : damageVictim.FieldPosition));
            bytes.Add((DamageVictimTeam = damageVictim?.Team) == null ? byte.MaxValue : damageVictim.Team.Id);
            Buffer = BitConverter.GetBytes((short)bytes.Count).Concat(bytes);
        }
        public PBEWeatherPacket(byte[] buffer, PBEBattle battle)
        {
            Buffer = buffer;
            using (var r = new BinaryReader(new MemoryStream(buffer)))
            {
                r.ReadInt16(); // Skip Code
                Weather = (PBEWeather)r.ReadByte();
                WeatherAction = (PBEWeatherAction)r.ReadByte();
                DamageVictim = (PBEFieldPosition)r.ReadByte();
                byte teamId = r.ReadByte();
                DamageVictimTeam = teamId == byte.MaxValue ? null : battle.Teams[teamId];
            }
        }

        public void Dispose() { }
    }
}
