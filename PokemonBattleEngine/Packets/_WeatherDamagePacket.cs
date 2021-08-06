using Kermalis.EndianBinaryIO;
using Kermalis.PokemonBattleEngine.Battle;
using System.Collections.ObjectModel;
using System.IO;

namespace Kermalis.PokemonBattleEngine.Packets
{
    public sealed class PBEWeatherDamagePacket : IPBEPacket
    {
        public const ushort Code = 0x40;
        public ReadOnlyCollection<byte> Data { get; }

        public PBEWeather Weather { get; }
        public PBETrainer DamageVictimTrainer { get; }
        public PBEFieldPosition DamageVictim { get; }

        internal PBEWeatherDamagePacket(PBEWeather weather, PBEBattlePokemon damageVictim)
        {
            using (var ms = new MemoryStream())
            using (var w = new EndianBinaryWriter(ms, encoding: EncodingType.UTF16))
            {
                w.Write(Code);
                w.Write(Weather = weather);
                w.Write((DamageVictimTrainer = damageVictim.Trainer).Id);
                w.Write(DamageVictim = damageVictim.FieldPosition);
                Data = new ReadOnlyCollection<byte>(ms.ToArray());
            }
        }
        internal PBEWeatherDamagePacket(byte[] data, EndianBinaryReader r, PBEBattle battle)
        {
            Data = new ReadOnlyCollection<byte>(data);
            Weather = r.ReadEnum<PBEWeather>();
            DamageVictimTrainer = battle.Trainers[r.ReadByte()];
            DamageVictim = r.ReadEnum<PBEFieldPosition>();
        }
    }
}
