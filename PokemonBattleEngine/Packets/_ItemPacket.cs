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

        public PBEFieldPosition ItemHolder { get; }
        public PBETeam ItemHolderTeam { get; }
        public PBEFieldPosition Pokemon2 { get; }
        public PBETeam Pokemon2Team { get; }
        public PBEItem Item { get; }
        public PBEItemAction ItemAction { get; }

        internal PBEItemPacket(PBEPokemon itemHolder, PBEPokemon pokemon2, PBEItem item, PBEItemAction itemAction)
        {
            using (var ms = new MemoryStream())
            using (var w = new EndianBinaryWriter(ms, encoding: EncodingType.UTF16))
            {
                w.Write(Code);
                w.Write(ItemHolder = itemHolder.FieldPosition);
                w.Write((ItemHolderTeam = itemHolder.Team).Id);
                w.Write(Pokemon2 = pokemon2.FieldPosition);
                w.Write((Pokemon2Team = pokemon2.Team).Id);
                w.Write(Item = item);
                w.Write(ItemAction = itemAction);
                Data = new ReadOnlyCollection<byte>(ms.ToArray());
            }
        }
        internal PBEItemPacket(byte[] data, EndianBinaryReader r, PBEBattle battle)
        {
            Data = new ReadOnlyCollection<byte>(data);
            ItemHolder = r.ReadEnum<PBEFieldPosition>();
            ItemHolderTeam = battle.Teams[r.ReadByte()];
            Pokemon2 = r.ReadEnum<PBEFieldPosition>();
            Pokemon2Team = battle.Teams[r.ReadByte()];
            Item = r.ReadEnum<PBEItem>();
            ItemAction = r.ReadEnum<PBEItemAction>();
        }
    }
}
