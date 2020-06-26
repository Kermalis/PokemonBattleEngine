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

        public PBEBattlePokemon ItemHolder { get; }
        public PBEBattlePokemon Pokemon2 { get; }
        public PBEItem Item { get; }
        public PBEItemAction ItemAction { get; }

        internal PBEItemPacket(PBEBattlePokemon itemHolder, PBEBattlePokemon pokemon2, PBEItem item, PBEItemAction itemAction)
        {
            using (var ms = new MemoryStream())
            using (var w = new EndianBinaryWriter(ms, encoding: EncodingType.UTF16))
            {
                w.Write(Code);
                (ItemHolder = itemHolder).ToBytes_Position(w);
                (Pokemon2 = pokemon2).ToBytes_Position(w);
                w.Write(Item = item);
                w.Write(ItemAction = itemAction);
                Data = new ReadOnlyCollection<byte>(ms.ToArray());
            }
        }
        internal PBEItemPacket(byte[] data, EndianBinaryReader r, PBEBattle battle)
        {
            Data = new ReadOnlyCollection<byte>(data);
            ItemHolder = battle.GetPokemon_Position(r);
            Pokemon2 = battle.GetPokemon_Position(r);
            Item = r.ReadEnum<PBEItem>();
            ItemAction = r.ReadEnum<PBEItemAction>();
        }
    }
}
