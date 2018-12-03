using Ether.Network.Packets;
using Kermalis.PokemonBattleEngine.Battle;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Kermalis.PokemonBattleEngine.Packets
{
    public sealed class PActionsResponsePacket : INetPacket
    {
        public const short Code = 0x08;
        public IEnumerable<byte> Buffer { get; }

        public readonly PAction[] Actions;

        public PActionsResponsePacket(IEnumerable<PAction> actions)
        {
            var bytes = new List<byte>();
            bytes.AddRange(BitConverter.GetBytes(Code));
            bytes.Add((byte)(Actions = actions.ToArray()).Length);
            foreach (var a in Actions)
                bytes.AddRange(a.ToBytes());
            Buffer = BitConverter.GetBytes((short)bytes.Count).Concat(bytes);
        }
        public PActionsResponsePacket(byte[] buffer)
        {
            Buffer = buffer;
            using (var r = new BinaryReader(new MemoryStream(buffer)))
            {
                r.ReadInt16(); // Skip Code
                byte numActions = r.ReadByte();
                Actions = new PAction[numActions];
                for (int i = 0; i < numActions; i++)
                    Actions[i] = PAction.FromBytes(r);
            }
        }

        public void Dispose() { }
    }
}
