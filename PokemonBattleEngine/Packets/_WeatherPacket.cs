using Kermalis.EndianBinaryIO;
using Kermalis.PokemonBattleEngine.Battle;
using Kermalis.PokemonBattleEngine.Data;
using System.Collections.ObjectModel;
using System.IO;

namespace Kermalis.PokemonBattleEngine.Packets
{
    public sealed class PBEWeatherPacket : IPBEPacket
    {
        public const ushort Code = 0x14;
        public ReadOnlyCollection<byte> Data { get; }

        public PBEWeather Weather { get; }
        public PBEWeatherAction WeatherAction { get; }
        public bool HasDamageVictim { get; }
        public PBEFieldPosition? DamageVictim { get; }
        public PBETeam DamageVictimTeam { get; }

        internal PBEWeatherPacket(PBEWeather weather, PBEWeatherAction weatherAction, PBEPokemon damageVictim = null)
        {
            using (var ms = new MemoryStream())
            using (var w = new EndianBinaryWriter(ms, encoding: EncodingType.UTF16))
            {
                w.Write(Code);
                w.Write(Weather = weather);
                w.Write(WeatherAction = weatherAction);
                w.Write(HasDamageVictim = damageVictim != null);
                if (HasDamageVictim)
                {
                    w.Write((DamageVictim = damageVictim.FieldPosition).Value);
                    w.Write((DamageVictimTeam = damageVictim.Team).Id);
                }
                Data = new ReadOnlyCollection<byte>(ms.ToArray());
            }
        }
        internal PBEWeatherPacket(byte[] data, EndianBinaryReader r, PBEBattle battle)
        {
            Data = new ReadOnlyCollection<byte>(data);
            Weather = r.ReadEnum<PBEWeather>();
            WeatherAction = r.ReadEnum<PBEWeatherAction>();
            HasDamageVictim = r.ReadBoolean();
            if (HasDamageVictim)
            {
                DamageVictim = r.ReadEnum<PBEFieldPosition>();
                DamageVictimTeam = battle.Teams[r.ReadByte()];
            }
        }
    }
}
