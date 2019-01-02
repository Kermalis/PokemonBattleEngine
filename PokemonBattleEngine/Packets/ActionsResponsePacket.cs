using Ether.Network.Packets;
using Kermalis.PokemonBattleEngine.Battle;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

namespace Kermalis.PokemonBattleEngine.Packets
{
    public sealed class PBEActionsResponsePacket : INetPacket
    {
        public const short Code = 0x08;
        public IEnumerable<byte> Buffer { get; }

        public ReadOnlyCollection<PBEAction> Actions { get; }

        public PBEActionsResponsePacket(IEnumerable<PBEAction> actions)
        {
            var bytes = new List<byte>();
            bytes.AddRange(BitConverter.GetBytes(Code));
            bytes.Add((byte)(Actions = actions.ToList().AsReadOnly()).Count);
            foreach (PBEAction a in Actions)
            {
                bytes.AddRange(a.ToBytes());
            }
            Buffer = BitConverter.GetBytes((short)bytes.Count).Concat(bytes);
        }
        public PBEActionsResponsePacket(byte[] buffer, PBEBattle battle)
        {
            Buffer = buffer;
            using (var r = new BinaryReader(new MemoryStream(buffer)))
            {
                r.ReadInt16(); // Skip Code
                var actions = new PBEAction[r.ReadByte()];
                for (int i = 0; i < actions.Length; i++)
                {
                    actions[i] = PBEAction.FromBytes(r);
                }
                Actions = Array.AsReadOnly(actions);
            }
        }

        public void Dispose() { }
    }
}
