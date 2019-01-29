using Ether.Network.Packets;
using Kermalis.PokemonBattleEngine.Battle;
using Kermalis.PokemonBattleEngine.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Kermalis.PokemonBattleEngine.Packets
{
    public sealed class PBEItemPacket : INetPacket
    {
        public const short Code = 0x16;
        public IEnumerable<byte> Buffer { get; }

        public PBEFieldPosition ItemHolder { get; }
        public PBETeam ItemHolderTeam { get; }
        public PBEFieldPosition Pokemon2 { get; }
        public PBETeam Pokemon2Team { get; }
        public PBEItem Item { get; }
        public PBEItemAction ItemAction { get; }

        public PBEItemPacket(PBEPokemon itemHolder, PBEPokemon pokemon2, PBEItem item, PBEItemAction itemAction)
        {
            var bytes = new List<byte>();
            bytes.AddRange(BitConverter.GetBytes(Code));
            bytes.Add((byte)(ItemHolder = itemHolder.FieldPosition));
            bytes.Add((ItemHolderTeam = itemHolder.Team).Id);
            bytes.Add((byte)(Pokemon2 = pokemon2.FieldPosition));
            bytes.Add((Pokemon2Team = pokemon2.Team).Id);
            bytes.AddRange(BitConverter.GetBytes((ushort)(Item = item)));
            bytes.Add((byte)(ItemAction = itemAction));
            Buffer = BitConverter.GetBytes((short)bytes.Count).Concat(bytes);
        }
        public PBEItemPacket(byte[] buffer, PBEBattle battle)
        {
            Buffer = buffer;
            using (var r = new BinaryReader(new MemoryStream(buffer)))
            {
                r.ReadInt16(); // Skip Code
                ItemHolder = (PBEFieldPosition)r.ReadByte();
                ItemHolderTeam = battle.Teams[r.ReadByte()];
                Pokemon2 = (PBEFieldPosition)r.ReadByte();
                Pokemon2Team = battle.Teams[r.ReadByte()];
                Item = (PBEItem)r.ReadUInt16();
                ItemAction = (PBEItemAction)r.ReadByte();
            }
        }

        public void Dispose() { }
    }
}
