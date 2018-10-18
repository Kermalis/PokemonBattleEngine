using Ether.Network.Packets;
using Kermalis.PokemonBattleEngine.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Kermalis.PokemonBattleEngine.Network
{
    public sealed class PRequestTeamPacket : INetPacketStream
    {
        public const int Code = 0x4;
        byte[] buf;
        public byte[] Buffer => (byte[])buf.Clone();

        public readonly PTeamShell Team;

        public PRequestTeamPacket(PTeamShell team)
        {
            Team = team ?? throw new ArgumentNullException(nameof(team));
            var bytes = new List<byte>();
            bytes.AddRange(BitConverter.GetBytes(Code));
            bytes.AddRange(Team.ToBytes());
            buf = BitConverter.GetBytes(bytes.Count).Concat(bytes).ToArray();
        }
        public PRequestTeamPacket(byte[] buffer)
        {
            using (var r = new BinaryReader(new MemoryStream(buf = buffer)))
            {
                r.ReadInt32(); // Skip Code
                Team = PTeamShell.FromBytes(r);
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
