using Kermalis.EndianBinaryIO;
using Kermalis.PokemonBattleEngine.Battle;
using System.Collections.ObjectModel;
using System.IO;

namespace Kermalis.PokemonBattleEngine.Packets
{
    public sealed class PBEStatus1Packet : IPBEPacket
    {
        public const ushort Code = 0x11;
        public ReadOnlyCollection<byte> Data { get; }

        public PBETrainer Status1ReceiverTrainer { get; }
        public PBEFieldPosition Status1Receiver { get; }
        public PBETrainer Pokemon2Trainer { get; }
        public PBEFieldPosition Pokemon2 { get; }
        public PBEStatus1 Status1 { get; }
        public PBEStatusAction StatusAction { get; }

        internal PBEStatus1Packet(PBEBattlePokemon status1Receiver, PBEBattlePokemon pokemon2, PBEStatus1 status1, PBEStatusAction statusAction)
        {
            using (var ms = new MemoryStream())
            using (var w = new EndianBinaryWriter(ms, encoding: EncodingType.UTF16))
            {
                w.Write(Code);
                w.Write((Status1ReceiverTrainer = status1Receiver.Trainer).Id);
                w.Write(Status1Receiver = status1Receiver.FieldPosition);
                w.Write((Pokemon2Trainer = pokemon2.Trainer).Id);
                w.Write(Pokemon2 = pokemon2.FieldPosition);
                w.Write(Status1 = status1);
                w.Write(StatusAction = statusAction);
                Data = new ReadOnlyCollection<byte>(ms.GetBuffer());
            }
        }
        internal PBEStatus1Packet(byte[] data, EndianBinaryReader r, PBEBattle battle)
        {
            Data = new ReadOnlyCollection<byte>(data);
            Status1ReceiverTrainer = battle.Trainers[r.ReadByte()];
            Status1Receiver = r.ReadEnum<PBEFieldPosition>();
            Pokemon2Trainer = battle.Trainers[r.ReadByte()];
            Pokemon2 = r.ReadEnum<PBEFieldPosition>();
            Status1 = r.ReadEnum<PBEStatus1>();
            StatusAction = r.ReadEnum<PBEStatusAction>();
        }
    }
}
