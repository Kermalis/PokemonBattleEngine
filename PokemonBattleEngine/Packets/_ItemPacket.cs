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

        public byte Culprit { get; } // Item holder
        public byte Victim { get; } // Victim of ItemAction
        public PBEItem Item { get; }
        public PBEItemAction ItemAction { get; }

        public PBEItemPacket(PBEPokemon culprit, PBEPokemon victim, PBEItem item, PBEItemAction itemAction)
        {
            var bytes = new List<byte>();
            bytes.AddRange(BitConverter.GetBytes(Code));
            bytes.Add(Culprit = culprit.Id);
            bytes.Add(Victim = victim.Id);
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
                Culprit = r.ReadByte();
                Victim = r.ReadByte();
                Item = (PBEItem)r.ReadUInt16();
                ItemAction = (PBEItemAction)r.ReadByte();
            }
        }

        public void Dispose() { }
    }
}
