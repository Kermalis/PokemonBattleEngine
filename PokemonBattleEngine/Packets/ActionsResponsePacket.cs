using Ether.Network.Packets;
using Kermalis.PokemonBattleEngine.Battle;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Kermalis.PokemonBattleEngine.Packets
{
    public sealed class PBEActionsResponsePacket : INetPacket
    {
        public const short Code = 0x08;
        public IEnumerable<byte> Buffer { get; }

        public PBEAction[] Actions { get; }

        public PBEActionsResponsePacket(IEnumerable<PBEAction> actions)
        {
            var bytes = new List<byte>();
            bytes.AddRange(BitConverter.GetBytes(Code));
            bytes.Add((byte)(Actions = actions.ToArray()).Length);
            foreach (var a in Actions)
            {
                bytes.AddRange(a.ToBytes());
            }
            Buffer = BitConverter.GetBytes((short)bytes.Count).Concat(bytes);
        }
        public PBEActionsResponsePacket(byte[] buffer)
        {
            Buffer = buffer;
            using (var r = new BinaryReader(new MemoryStream(buffer)))
            {
                r.ReadInt16(); // Skip Code
                byte numActions = r.ReadByte();
                Actions = new PBEAction[numActions];
                for (int i = 0; i < numActions; i++)
                {
                    Actions[i] = PBEAction.FromBytes(r);
                }
            }
        }

        public void Dispose() { }
    }
}
