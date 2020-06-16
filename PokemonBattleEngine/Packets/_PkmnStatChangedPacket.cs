using Kermalis.EndianBinaryIO;
using Kermalis.PokemonBattleEngine.Battle;
using Kermalis.PokemonBattleEngine.Data;
using System.Collections.ObjectModel;
using System.IO;

namespace Kermalis.PokemonBattleEngine.Packets
{
    public sealed class PBEPkmnStatChangedPacket : IPBEPacket
    {
        public const ushort Code = 0x10;
        public ReadOnlyCollection<byte> Data { get; }

        public PBEFieldPosition Pokemon { get; }
        public PBETeam PokemonTeam { get; }
        public PBEStat Stat { get; }
        public sbyte OldValue { get; }
        public sbyte NewValue { get; }

        internal PBEPkmnStatChangedPacket(PBEBattlePokemon pokemon, PBEStat stat, sbyte oldValue, sbyte newValue)
        {
            using (var ms = new MemoryStream())
            using (var w = new EndianBinaryWriter(ms, encoding: EncodingType.UTF16))
            {
                w.Write(Code);
                w.Write(Pokemon = pokemon.FieldPosition);
                w.Write((PokemonTeam = pokemon.Team).Id);
                w.Write(Stat = stat);
                w.Write(OldValue = oldValue);
                w.Write(NewValue = newValue);
                Data = new ReadOnlyCollection<byte>(ms.ToArray());
            }
        }
        internal PBEPkmnStatChangedPacket(byte[] data, EndianBinaryReader r, PBEBattle battle)
        {
            Data = new ReadOnlyCollection<byte>(data);
            Pokemon = r.ReadEnum<PBEFieldPosition>();
            PokemonTeam = battle.Teams[r.ReadByte()];
            Stat = r.ReadEnum<PBEStat>();
            OldValue = r.ReadSByte();
            NewValue = r.ReadSByte();
        }
    }
}
