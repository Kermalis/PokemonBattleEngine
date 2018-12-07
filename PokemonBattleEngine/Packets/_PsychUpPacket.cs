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

        public byte CulpritId { get; }
        public byte VictimId { get; }
        public sbyte AttackChange { get; }
        public sbyte DefenseChange { get; }
        public sbyte SpAttackChange { get; }
        public sbyte SpDefenseChange { get; }
        public sbyte SpeedChange { get; }
        public sbyte AccuracyChange { get; }
        public sbyte EvasionChange { get; }

        public PBEPsychUpPacket(PBEPokemon culprit, PBEPokemon victim)
        {
            var bytes = new List<byte>();
            bytes.AddRange(BitConverter.GetBytes(Code));
            bytes.Add(CulpritId = culprit.Id);
            bytes.Add(VictimId = victim.Id);
            bytes.Add((byte)(AttackChange = victim.AttackChange));
            bytes.Add((byte)(DefenseChange = victim.DefenseChange));
            bytes.Add((byte)(SpAttackChange = victim.SpAttackChange));
            bytes.Add((byte)(SpDefenseChange = victim.SpDefenseChange));
            bytes.Add((byte)(SpeedChange = victim.SpeedChange));
            bytes.Add((byte)(AccuracyChange = victim.AccuracyChange));
            bytes.Add((byte)(EvasionChange = victim.EvasionChange));
            Buffer = BitConverter.GetBytes((short)bytes.Count).Concat(bytes);
        }
        public PBEPsychUpPacket(byte[] buffer)
        {
            Buffer = buffer;
            using (var r = new BinaryReader(new MemoryStream(buffer)))
            {
                r.ReadInt16(); // Skip Code
                CulpritId = r.ReadByte();
                VictimId = r.ReadByte();
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
