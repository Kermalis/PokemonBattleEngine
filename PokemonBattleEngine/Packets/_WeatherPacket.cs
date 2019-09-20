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
        public bool HasDamageVictim { get; }
        public PBEFieldPosition? DamageVictim { get; }
        public PBETeam DamageVictimTeam { get; }

        internal PBEWeatherPacket(PBEWeather weather, PBEWeatherAction weatherAction, PBEPokemon damageVictim = null)
        {
            var bytes = new List<byte>();
            bytes.AddRange(BitConverter.GetBytes(Code));
            bytes.Add((byte)(Weather = weather));
            bytes.Add((byte)(WeatherAction = weatherAction));
            bytes.Add((byte)((HasDamageVictim = damageVictim != null) ? 1 : 0));
            if (HasDamageVictim)
            {
                bytes.Add((byte)(DamageVictim = damageVictim.FieldPosition));
                bytes.Add((DamageVictimTeam = damageVictim.Team).Id);
            }
            bytes.InsertRange(0, BitConverter.GetBytes((short)bytes.Count));
            Buffer = new ReadOnlyCollection<byte>(bytes);
        }
        internal PBEWeatherPacket(ReadOnlyCollection<byte> buffer, BinaryReader r, PBEBattle battle)
        {
            Buffer = buffer;
            Weather = (PBEWeather)r.ReadByte();
            WeatherAction = (PBEWeatherAction)r.ReadByte();
            HasDamageVictim = r.ReadBoolean();
            if (HasDamageVictim)
            {
                DamageVictim = (PBEFieldPosition)r.ReadByte();
                DamageVictimTeam = battle.Teams[r.ReadByte()];
            }
        }

        public void Dispose() { }
    }
}
