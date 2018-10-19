using Ether.Network.Packets;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Kermalis.PokemonBattleEngine.Network
{
    public sealed class PSubmitActionsPacket : INetPacketStream
    {
        public const int Code = 0x8;
        byte[] buf;
        public byte[] Buffer => (byte[])buf.Clone();

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
            Actions = actions;
            var bytes = new List<byte>();
            bytes.AddRange(BitConverter.GetBytes(Code));
            bytes.Add((byte)Actions.Length);
            foreach (var a in Actions)
                bytes.AddRange(a.ToBytes());
            buf = BitConverter.GetBytes(bytes.Count).Concat(bytes).ToArray();
        }
        public PSubmitActionsPacket(byte[] buffer)
        {
            using (var r = new BinaryReader(new MemoryStream(buf = buffer)))
            {
                r.ReadInt32(); // Skip Code
                byte numActions = r.ReadByte();
                Actions = new Action[numActions];
                for (int i = 0; i < numActions; i++)
                    Actions[i] = Action.FromBytes(r);
            }
        }

        public int Size => throw new NotImplementedException();
        public long Position => throw new NotImplementedException();
        public T Read<T>() => throw new NotImplementedException();
        public T[] ReadArray<T>(int amount) => throw new NotImplementedException();
        public void Write<T>(T value) => throw new NotImplementedException();
        public void Dispose() { }
    }
}
