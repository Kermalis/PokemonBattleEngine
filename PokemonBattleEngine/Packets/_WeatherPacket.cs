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
        public PBETrainer DamageVictimTrainer { get; }
        public PBEFieldPosition DamageVictim { get; }

        internal PBEWeatherPacket(PBEWeather weather, PBEWeatherAction weatherAction, PBEBattlePokemon damageVictim = null)
        {
            using (var ms = new MemoryStream())
            using (var w = new EndianBinaryWriter(ms, encoding: EncodingType.UTF16))
            {
                w.Write(Code);
                w.Write(Weather = weather);
                w.Write(WeatherAction = weatherAction);
                w.Write(damageVictim != null);
                if (damageVictim != null)
                {
                    w.Write((DamageVictimTrainer = damageVictim.Trainer).Id);
                    w.Write(DamageVictim = damageVictim.FieldPosition);
                }
                Data = new ReadOnlyCollection<byte>(ms.GetBuffer());
            }
        }
        internal PBEWeatherPacket(byte[] data, EndianBinaryReader r, PBEBattle battle)
        {
            Data = new ReadOnlyCollection<byte>(data);
            Weather = r.ReadEnum<PBEWeather>();
            WeatherAction = r.ReadEnum<PBEWeatherAction>();
            if (r.ReadBoolean())
            {
                DamageVictimTrainer = battle.Trainers[r.ReadByte()];
                DamageVictim = r.ReadEnum<PBEFieldPosition>();
            }
        }
    }
}
