using Kermalis.EndianBinaryIO;
using Kermalis.PokemonBattleEngine.Battle;
using System.Collections.ObjectModel;
using System.IO;

namespace Kermalis.PokemonBattleEngine.Packets
{
    public sealed class PBESwitchInRequestPacket : IPBEPacket
    {
        public const ushort Code = 0x23;
        public ReadOnlyCollection<byte> Data { get; }

        public PBETrainer Trainer { get; }
        public byte Amount { get; }

        internal PBESwitchInRequestPacket(PBETrainer trainer)
        {
            using (var ms = new MemoryStream())
            using (var w = new EndianBinaryWriter(ms, encoding: EncodingType.UTF16))
            {
                w.Write(Code);
                w.Write((Trainer = trainer).Id);
                w.Write(Amount = trainer.SwitchInsRequired);
                Data = new ReadOnlyCollection<byte>(ms.GetBuffer());
            }
        }
        internal PBESwitchInRequestPacket(byte[] data, EndianBinaryReader r, PBEBattle battle)
        {
            Data = new ReadOnlyCollection<byte>(data);
            Trainer = battle.Trainers[r.ReadByte()];
            Amount = r.ReadByte();
        }
    }
}
