using Ether.Network.Packets;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Kermalis.PokemonBattleEngine.Packets
{
    public sealed class PSubmitActionsPacket : INetPacket
    {
        public const short Code = 0x08;
        public IEnumerable<byte> Buffer { get; }

        public sealed class Action
        {
            public readonly Guid PokemonId;
            // TODO: Action (switch, forfeit, move)
            public readonly byte Param;

            public Action(Guid pkmnId, byte param)
            {
                PokemonId = pkmnId;
                Param = param;
            }

            internal byte[] ToBytes()
            {
                var bytes = new List<byte>();
                bytes.AddRange(PokemonId.ToByteArray());
                bytes.Add(Param);
                return bytes.ToArray();
            }
            internal static Action FromBytes(BinaryReader r)
            {
                return new Action(new Guid(r.ReadBytes(16)), r.ReadByte());
            }
        }

        public readonly Action[] Actions;

        public PSubmitActionsPacket(Action[] actions)
        {
            var bytes = new List<byte>();
            bytes.AddRange(BitConverter.GetBytes(Code));
            bytes.Add((byte)(Actions = actions).Length);
            foreach (var a in Actions)
                bytes.AddRange(a.ToBytes());
            Buffer = BitConverter.GetBytes((short)bytes.Count).Concat(bytes);
        }
        public PSubmitActionsPacket(byte[] buffer)
        {
            Buffer = buffer;
            using (var r = new BinaryReader(new MemoryStream(buffer)))
            {
                r.ReadInt16(); // Skip Code
                byte numActions = r.ReadByte();
                Actions = new Action[numActions];
                for (int i = 0; i < numActions; i++)
                    Actions[i] = Action.FromBytes(r);
            }
        }

        public void Dispose() { }
    }
}
