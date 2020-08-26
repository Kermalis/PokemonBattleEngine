using Kermalis.EndianBinaryIO;
using Kermalis.PokemonBattleEngine.Battle;
using Kermalis.PokemonBattleEngine.Data;
using System.Collections.ObjectModel;
using System.IO;

namespace Kermalis.PokemonBattleEngine.Packets
{
    public sealed class PBEItemPacket : IPBEPacket
    {
        public const ushort Code = 0x16;
        public ReadOnlyCollection<byte> Data { get; }

        public PBETrainer ItemHolderTrainer { get; }
        public PBEFieldPosition ItemHolder { get; }
        public PBETrainer Pokemon2Trainer { get; }
        public PBEFieldPosition Pokemon2 { get; }
        public PBEItem Item { get; }
        public PBEItemAction ItemAction { get; }

        internal PBEItemPacket(PBEBattlePokemon itemHolder, PBEBattlePokemon pokemon2, PBEItem item, PBEItemAction itemAction)
        {
            using (var ms = new MemoryStream())
            using (var w = new EndianBinaryWriter(ms, encoding: EncodingType.UTF16))
            {
                w.Write(Code);
                w.Write((ItemHolderTrainer = itemHolder.Trainer).Id);
                w.Write(ItemHolder = itemHolder.FieldPosition);
                w.Write((Pokemon2Trainer = pokemon2.Trainer).Id);
                w.Write(Pokemon2 = pokemon2.FieldPosition);
                w.Write(Item = item);
                w.Write(ItemAction = itemAction);
                Data = new ReadOnlyCollection<byte>(ms.GetBuffer());
            }
        }
        internal PBEItemPacket(byte[] data, EndianBinaryReader r, PBEBattle battle)
        {
            Data = new ReadOnlyCollection<byte>(data);
            ItemHolderTrainer = battle.Trainers[r.ReadByte()];
            ItemHolder = r.ReadEnum<PBEFieldPosition>();
            Pokemon2Trainer = battle.Trainers[r.ReadByte()];
            Pokemon2 = r.ReadEnum<PBEFieldPosition>();
            Item = r.ReadEnum<PBEItem>();
            ItemAction = r.ReadEnum<PBEItemAction>();
        }
    }
}
