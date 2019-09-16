using Ether.Network.Packets;
using Kermalis.PokemonBattleEngine.Battle;
using Kermalis.PokemonBattleEngine.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;

namespace Kermalis.PokemonBattleEngine.Packets
{
    public sealed class PBEItemPacket : INetPacket
    {
        public const short Code = 0x16;
        public ReadOnlyCollection<byte> Buffer { get; }

        public PBEFieldPosition ItemHolder { get; }
        public PBETeam ItemHolderTeam { get; }
        public PBEFieldPosition Pokemon2 { get; }
        public PBETeam Pokemon2Team { get; }
        public PBEItem Item { get; }
        public PBEItemAction ItemAction { get; }

        internal PBEItemPacket(PBEPokemon itemHolder, PBEPokemon pokemon2, PBEItem item, PBEItemAction itemAction)
        {
            var bytes = new List<byte>();
            bytes.AddRange(BitConverter.GetBytes(Code));
            bytes.Add((byte)(ItemHolder = itemHolder.FieldPosition));
            bytes.Add((ItemHolderTeam = itemHolder.Team).Id);
            bytes.Add((byte)(Pokemon2 = pokemon2.FieldPosition));
            bytes.Add((Pokemon2Team = pokemon2.Team).Id);
            bytes.AddRange(BitConverter.GetBytes((ushort)(Item = item)));
            bytes.Add((byte)(ItemAction = itemAction));
            bytes.InsertRange(0, BitConverter.GetBytes((short)bytes.Count));
            Buffer = new ReadOnlyCollection<byte>(bytes);
        }
        internal PBEItemPacket(ReadOnlyCollection<byte> buffer, BinaryReader r, PBEBattle battle)
        {
            Buffer = buffer;
            ItemHolder = (PBEFieldPosition)r.ReadByte();
            ItemHolderTeam = battle.Teams[r.ReadByte()];
            Pokemon2 = (PBEFieldPosition)r.ReadByte();
            Pokemon2Team = battle.Teams[r.ReadByte()];
            Item = (PBEItem)r.ReadUInt16();
            ItemAction = (PBEItemAction)r.ReadByte();
        }

        public void Dispose() { }
    }
}
