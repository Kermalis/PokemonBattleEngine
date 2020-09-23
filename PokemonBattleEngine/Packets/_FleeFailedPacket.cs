using Kermalis.EndianBinaryIO;
using Kermalis.PokemonBattleEngine.Battle;
using System.Collections.ObjectModel;
using System.IO;

namespace Kermalis.PokemonBattleEngine.Packets
{
    public sealed class PBEFleeFailedPacket : IPBEPacket
    {
        public const ushort Code = 0x39;
        public ReadOnlyCollection<byte> Data { get; }

        public PBETrainer PokemonTrainer { get; }
        public PBEFieldPosition Pokemon { get; }

        internal PBEFleeFailedPacket(PBEBattlePokemon pokemon)
        {
            using (var ms = new MemoryStream())
            using (var w = new EndianBinaryWriter(ms, encoding: EncodingType.UTF16))
            {
                w.Write(Code);
                w.Write((PokemonTrainer = pokemon.Trainer).Id);
                w.Write(Pokemon = pokemon.FieldPosition);
                Data = new ReadOnlyCollection<byte>(ms.ToArray());
            }
        }
        internal PBEFleeFailedPacket(byte[] data, EndianBinaryReader r, PBEBattle battle)
        {
            Data = new ReadOnlyCollection<byte>(data);
            PokemonTrainer = battle.Trainers[r.ReadByte()];
            Pokemon = r.ReadEnum<PBEFieldPosition>();
        }
    }
}
