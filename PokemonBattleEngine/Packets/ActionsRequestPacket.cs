using Kermalis.EndianBinaryIO;
using Kermalis.PokemonBattleEngine.Battle;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

namespace Kermalis.PokemonBattleEngine.Packets
{
    public sealed class PBEActionsRequestPacket : IPBEPacket
    {
        public const ushort Code = 0x07;
        public ReadOnlyCollection<byte> Data { get; }

        public PBETrainer Trainer { get; }
        public ReadOnlyCollection<PBEFieldPosition> Pokemon { get; }

        internal PBEActionsRequestPacket(PBETrainer trainer)
        {
            using (var ms = new MemoryStream())
            using (var w = new EndianBinaryWriter(ms, encoding: EncodingType.UTF16))
            {
                w.Write(Code);
                w.Write((Trainer = trainer).Id);
                byte count = (byte)(Pokemon = new ReadOnlyCollection<PBEFieldPosition>(trainer.ActionsRequired.Select(p => p.FieldPosition).ToArray())).Count;
                w.Write(count);
                for (int i = 0; i < count; i++)
                {
                    w.Write(Pokemon[i]);
                }
                Data = new ReadOnlyCollection<byte>(ms.GetBuffer());
            }
        }
        internal PBEActionsRequestPacket(byte[] data, EndianBinaryReader r, PBEBattle battle)
        {
            Data = new ReadOnlyCollection<byte>(data);
            Trainer = battle.Trainers[r.ReadByte()];
            var pkmn = new PBEFieldPosition[r.ReadByte()];
            for (int i = 0; i < pkmn.Length; i++)
            {
                pkmn[i] = r.ReadEnum<PBEFieldPosition>();
            }
            Pokemon = new ReadOnlyCollection<PBEFieldPosition>(pkmn);
        }
    }
}
