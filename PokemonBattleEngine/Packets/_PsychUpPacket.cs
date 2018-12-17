using Ether.Network.Packets;
using Kermalis.PokemonBattleEngine.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Kermalis.PokemonBattleEngine.Packets
{
    public sealed class PBEPsychUpPacket : INetPacket
    {
        public const short Code = 0x22;
        public IEnumerable<byte> Buffer { get; }

        public byte UserId { get; }
        public byte TargetId { get; }
        public sbyte AttackChange { get; }
        public sbyte DefenseChange { get; }
        public sbyte SpAttackChange { get; }
        public sbyte SpDefenseChange { get; }
        public sbyte SpeedChange { get; }
        public sbyte AccuracyChange { get; }
        public sbyte EvasionChange { get; }

        public PBEPsychUpPacket(PBEPokemon user, PBEPokemon target)
        {
            var bytes = new List<byte>();
            bytes.AddRange(BitConverter.GetBytes(Code));
            bytes.Add(UserId = user.Id);
            bytes.Add(TargetId = target.Id);
            bytes.Add((byte)(AttackChange = target.AttackChange));
            bytes.Add((byte)(DefenseChange = target.DefenseChange));
            bytes.Add((byte)(SpAttackChange = target.SpAttackChange));
            bytes.Add((byte)(SpDefenseChange = target.SpDefenseChange));
            bytes.Add((byte)(SpeedChange = target.SpeedChange));
            bytes.Add((byte)(AccuracyChange = target.AccuracyChange));
            bytes.Add((byte)(EvasionChange = target.EvasionChange));
            Buffer = BitConverter.GetBytes((short)bytes.Count).Concat(bytes);
        }
        public PBEPsychUpPacket(byte[] buffer)
        {
            Buffer = buffer;
            using (var r = new BinaryReader(new MemoryStream(buffer)))
            {
                r.ReadInt16(); // Skip Code
                UserId = r.ReadByte();
                TargetId = r.ReadByte();
                AttackChange = r.ReadSByte();
                DefenseChange = r.ReadSByte();
                SpAttackChange = r.ReadSByte();
                SpDefenseChange = r.ReadSByte();
                SpeedChange = r.ReadSByte();
                AccuracyChange = r.ReadSByte();
                EvasionChange = r.ReadSByte();
            }
        }

        public void Dispose() { }
    }
}
