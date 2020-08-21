using Kermalis.EndianBinaryIO;
using Kermalis.PokemonBattleEngine.Battle;
using Kermalis.PokemonBattleEngine.Data;
using System.Collections.ObjectModel;
using System.IO;

namespace Kermalis.PokemonBattleEngine.Packets
{
    public sealed class PBEItemTurnPacket : IPBEPacket
    {
        public const ushort Code = 0x3A;
        public ReadOnlyCollection<byte> Data { get; }

        public PBETrainer ItemUserTrainer { get; }
        public PBEFieldPosition ItemUser { get; }
        public PBEItem Item { get; }
        public PBEItemTurnAction ItemAction { get; }

        internal PBEItemTurnPacket(PBEBattlePokemon itemUserHolder, PBEItem item, PBEItemTurnAction itemAction)
        {
            using (var ms = new MemoryStream())
            using (var w = new EndianBinaryWriter(ms, encoding: EncodingType.UTF16))
            {
                w.Write(Code);
                w.Write((ItemUserTrainer = itemUserHolder.Trainer).Id);
                w.Write(ItemUser = itemUserHolder.FieldPosition);
                w.Write(Item = item);
                w.Write(ItemAction = itemAction);
                Data = new ReadOnlyCollection<byte>(ms.GetBuffer());
            }
        }
        internal PBEItemTurnPacket(byte[] data, EndianBinaryReader r, PBEBattle battle)
        {
            Data = new ReadOnlyCollection<byte>(data);
            ItemUserTrainer = battle.Trainers[r.ReadByte()];
            ItemUser = r.ReadEnum<PBEFieldPosition>();
            Item = r.ReadEnum<PBEItem>();
            ItemAction = r.ReadEnum<PBEItemTurnAction>();
        }
    }
}
