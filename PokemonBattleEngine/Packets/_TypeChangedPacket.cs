using Kermalis.EndianBinaryIO;
using Kermalis.PokemonBattleEngine.Battle;
using Kermalis.PokemonBattleEngine.Data;
using System.Collections.ObjectModel;
using System.IO;

namespace Kermalis.PokemonBattleEngine.Packets
{
    public sealed class PBETypeChangedPacket : IPBEPacket
    {
        public const ushort Code = 0x2B;
        public ReadOnlyCollection<byte> Data { get; }

        public PBEBattlePokemon Pokemon { get; }
        public PBEType Type1 { get; }
        public PBEType Type2 { get; }

        internal PBETypeChangedPacket(PBEBattlePokemon pokemon, PBEType type1, PBEType type2)
        {
            using (var ms = new MemoryStream())
            using (var w = new EndianBinaryWriter(ms, encoding: EncodingType.UTF16))
            {
                w.Write(Code);
                (Pokemon = pokemon).ToBytes_Position(w);
                w.Write(Type1 = type1);
                w.Write(Type2 = type2);
                Data = new ReadOnlyCollection<byte>(ms.ToArray());
            }
        }
        internal PBETypeChangedPacket(byte[] data, EndianBinaryReader r, PBEBattle battle)
        {
            Data = new ReadOnlyCollection<byte>(data);
            Pokemon = battle.GetPokemon_Position(r);
            Type1 = r.ReadEnum<PBEType>();
            Type2 = r.ReadEnum<PBEType>();
        }
    }
}
