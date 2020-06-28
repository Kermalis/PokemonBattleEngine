using Kermalis.EndianBinaryIO;
using Kermalis.PokemonBattleEngine.Battle;
using Kermalis.PokemonBattleEngine.Data;
using System.Collections.ObjectModel;
using System.IO;

namespace Kermalis.PokemonBattleEngine.Packets
{
    public sealed class PBEMoveMissedPacket : IPBEPacket
    {
        public const ushort Code = 0x0D;
        public ReadOnlyCollection<byte> Data { get; }

        public PBETrainer MoveUserTrainer { get; }
        public PBEFieldPosition MoveUser { get; }
        public PBETrainer Pokemon2Trainer { get; }
        public PBEFieldPosition Pokemon2 { get; }

        internal PBEMoveMissedPacket(PBEBattlePokemon moveUser, PBEBattlePokemon pokemon2)
        {
            using (var ms = new MemoryStream())
            using (var w = new EndianBinaryWriter(ms, encoding: EncodingType.UTF16))
            {
                w.Write(Code);
                w.Write((MoveUserTrainer = moveUser.Trainer).Id);
                w.Write(MoveUser = moveUser.FieldPosition);
                w.Write((Pokemon2Trainer = pokemon2.Trainer).Id);
                w.Write(Pokemon2 = pokemon2.FieldPosition);
                Data = new ReadOnlyCollection<byte>(ms.ToArray());
            }
        }
        internal PBEMoveMissedPacket(byte[] data, EndianBinaryReader r, PBEBattle battle)
        {
            Data = new ReadOnlyCollection<byte>(data);
            MoveUserTrainer = battle.Trainers[r.ReadByte()];
            MoveUser = r.ReadEnum<PBEFieldPosition>();
            Pokemon2Trainer = battle.Trainers[r.ReadByte()];
            Pokemon2 = r.ReadEnum<PBEFieldPosition>();
        }
    }
}
