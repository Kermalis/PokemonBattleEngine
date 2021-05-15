using Kermalis.EndianBinaryIO;
using Kermalis.PokemonBattleEngine.Battle;
using System.Collections.ObjectModel;
using System.IO;

namespace Kermalis.PokemonBattleEngine.Packets
{
    public sealed class PBEPkmnEXPChangedPacket : IPBEPacket
    {
        public const ushort Code = 0x3D;
        public ReadOnlyCollection<byte> Data { get; }

        public PBETrainer PokemonTrainer { get; }
        public byte Pokemon { get; }
        public uint OldEXP { get; }
        public uint NewEXP { get; }

        internal PBEPkmnEXPChangedPacket(PBEBattlePokemon pokemon, uint oldEXP)
        {
            using (var ms = new MemoryStream())
            using (var w = new EndianBinaryWriter(ms, encoding: EncodingType.UTF16))
            {
                w.Write(Code);
                w.Write((PokemonTrainer = pokemon.Trainer).Id);
                w.Write(Pokemon = pokemon.Id);
                w.Write(OldEXP = oldEXP);
                w.Write(NewEXP = pokemon.EXP);
                Data = new ReadOnlyCollection<byte>(ms.ToArray());
            }
        }
        internal PBEPkmnEXPChangedPacket(byte[] data, EndianBinaryReader r, PBEBattle battle)
        {
            Data = new ReadOnlyCollection<byte>(data);
            PokemonTrainer = battle.Trainers[r.ReadByte()];
            Pokemon = r.ReadByte();
            OldEXP = r.ReadUInt32();
            NewEXP = r.ReadUInt32();
        }
    }
}
