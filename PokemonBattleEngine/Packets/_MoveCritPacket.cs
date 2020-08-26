using Kermalis.EndianBinaryIO;
using Kermalis.PokemonBattleEngine.Battle;
using System.Collections.ObjectModel;
using System.IO;

namespace Kermalis.PokemonBattleEngine.Packets
{
    public sealed class PBEMoveCritPacket : IPBEPacket
    {
        public const ushort Code = 0x0F;
        public ReadOnlyCollection<byte> Data { get; }

        public PBETrainer VictimTrainer { get; }
        public PBEFieldPosition Victim { get; }

        internal PBEMoveCritPacket(PBEBattlePokemon victim)
        {
            using (var ms = new MemoryStream())
            using (var w = new EndianBinaryWriter(ms, encoding: EncodingType.UTF16))
            {
                w.Write(Code);
                w.Write((VictimTrainer = victim.Trainer).Id);
                w.Write(Victim = victim.FieldPosition);
                Data = new ReadOnlyCollection<byte>(ms.GetBuffer());
            }
        }
        internal PBEMoveCritPacket(byte[] data, EndianBinaryReader r, PBEBattle battle)
        {
            Data = new ReadOnlyCollection<byte>(data);
            VictimTrainer = battle.Trainers[r.ReadByte()];
            Victim = r.ReadEnum<PBEFieldPosition>();
        }
    }
}
