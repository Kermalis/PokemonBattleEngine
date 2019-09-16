using Ether.Network.Packets;
using Kermalis.PokemonBattleEngine.Battle;
using Kermalis.PokemonBattleEngine.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;

namespace Kermalis.PokemonBattleEngine.Packets
{
    public sealed class PBEWeatherPacket : INetPacket
    {
        public const short Code = 0x14;
        public ReadOnlyCollection<byte> Buffer { get; }

        public PBEWeather Weather { get; }
        public PBEWeatherAction WeatherAction { get; }
        public PBEFieldPosition DamageVictim { get; } // PBEFieldPosition.None means no victim
        public PBETeam DamageVictimTeam { get; } // null means no victim

        internal PBEWeatherPacket(PBEWeather weather, PBEWeatherAction weatherAction, PBEPokemon damageVictim)
        {
            var bytes = new List<byte>();
            bytes.AddRange(BitConverter.GetBytes(Code));
            bytes.Add((byte)(Weather = weather));
            bytes.Add((byte)(WeatherAction = weatherAction));
            bytes.Add((byte)(DamageVictim = damageVictim == null ? PBEFieldPosition.None : damageVictim.FieldPosition));
            bytes.Add((DamageVictimTeam = damageVictim?.Team) == null ? byte.MaxValue : damageVictim.Team.Id);
            bytes.InsertRange(0, BitConverter.GetBytes((short)bytes.Count));
            Buffer = new ReadOnlyCollection<byte>(bytes);
        }
        internal PBEWeatherPacket(ReadOnlyCollection<byte> buffer, BinaryReader r, PBEBattle battle)
        {
            Buffer = buffer;
            Weather = (PBEWeather)r.ReadByte();
            WeatherAction = (PBEWeatherAction)r.ReadByte();
            DamageVictim = (PBEFieldPosition)r.ReadByte();
            byte teamId = r.ReadByte();
            DamageVictimTeam = teamId == byte.MaxValue ? null : battle.Teams[teamId];
        }

        public void Dispose() { }
    }
}
