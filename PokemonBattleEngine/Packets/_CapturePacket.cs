using Kermalis.EndianBinaryIO;
using Kermalis.PokemonBattleEngine.Battle;
using Kermalis.PokemonBattleEngine.Data;
using System.Collections.ObjectModel;
using System.IO;

namespace Kermalis.PokemonBattleEngine.Packets
{
    public sealed class PBECapturePacket : IPBEPacket
    {
        public const ushort Code = 0x3B;
        public ReadOnlyCollection<byte> Data { get; }

        public PBETrainer PokemonTrainer { get; }
        public PBEFieldPosition Pokemon { get; }
        public PBEItem Ball { get; }
        public byte NumShakes { get; }
        public bool Success { get; }
        public bool Critical { get; }

        internal PBECapturePacket(PBEBattlePokemon pokemon, PBEItem ball, byte numShakes, bool success, bool critical)
        {
            using (var ms = new MemoryStream())
            using (var w = new EndianBinaryWriter(ms, encoding: EncodingType.UTF16))
            {
                w.Write(Code);
                w.Write((PokemonTrainer = pokemon.Trainer).Id);
                w.Write(Pokemon = pokemon.FieldPosition);
                w.Write(Ball = ball);
                w.Write(NumShakes = numShakes);
                w.Write(Success = success);
                w.Write(Critical = critical);
                Data = new ReadOnlyCollection<byte>(ms.ToArray());
            }
        }
        internal PBECapturePacket(byte[] data, EndianBinaryReader r, PBEBattle battle)
        {
            Data = new ReadOnlyCollection<byte>(data);
            PokemonTrainer = battle.Trainers[r.ReadByte()];
            Pokemon = r.ReadEnum<PBEFieldPosition>();
            Ball = r.ReadEnum<PBEItem>();
            NumShakes = r.ReadByte();
            Success = r.ReadBoolean();
            Critical = r.ReadBoolean();
        }
    }
}
