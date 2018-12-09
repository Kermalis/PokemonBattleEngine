using Ether.Network.Packets;
using Kermalis.PokemonBattleEngine.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Kermalis.PokemonBattleEngine.Packets
{
    public sealed class PBETransformPacket : INetPacket
    {
        public const short Code = 0x18;
        public IEnumerable<byte> Buffer { get; }

        public byte CulpritId { get; }
        public byte VictimId { get; }
        public ushort TargetAttack { get; }
        public ushort TargetDefense { get; }
        public ushort TargetSpAttack { get; }
        public ushort TargetSpDefense { get; }
        public ushort TargetSpeed { get; }
        public PBEAbility TargetAbility { get; }
        public PBEType TargetType1 { get; }
        public PBEType TargetType2 { get; }
        public PBEMove[] TargetMoves { get; }

        public PBETransformPacket(PBEPokemon culprit, PBEPokemon victim, PBESettings settings)
        {
            var bytes = new List<byte>();
            bytes.AddRange(BitConverter.GetBytes(Code));
            bytes.Add(CulpritId = culprit.Id);
            bytes.Add(VictimId = victim.Id);
            bytes.AddRange(BitConverter.GetBytes(TargetAttack = victim.Attack));
            bytes.AddRange(BitConverter.GetBytes(TargetDefense = victim.Defense));
            bytes.AddRange(BitConverter.GetBytes(TargetSpAttack = victim.SpAttack));
            bytes.AddRange(BitConverter.GetBytes(TargetSpDefense = victim.SpDefense));
            bytes.AddRange(BitConverter.GetBytes(TargetSpeed = victim.Speed));
            bytes.Add((byte)(TargetAbility = victim.Ability));
            bytes.Add((byte)(TargetType1 = victim.Type1));
            bytes.Add((byte)(TargetType2 = victim.Type2));
            for (int i = 0; i < settings.NumMoves; i++)
            {
                bytes.AddRange(BitConverter.GetBytes((ushort)victim.Moves[i]));
            }
            Buffer = BitConverter.GetBytes((short)bytes.Count).Concat(bytes);
        }
        public PBETransformPacket(byte[] buffer, PBESettings settings)
        {
            Buffer = buffer;
            using (var r = new BinaryReader(new MemoryStream(buffer)))
            {
                r.ReadInt16(); // Skip Code
                CulpritId = r.ReadByte();
                VictimId = r.ReadByte();
                TargetAttack = r.ReadUInt16();
                TargetDefense = r.ReadUInt16();
                TargetSpAttack = r.ReadUInt16();
                TargetSpDefense = r.ReadUInt16();
                TargetSpeed = r.ReadUInt16();
                TargetAbility = (PBEAbility)r.ReadByte();
                TargetType1 = (PBEType)r.ReadByte();
                TargetType2 = (PBEType)r.ReadByte();
                TargetMoves = new PBEMove[settings.NumMoves];
                for (int i = 0; i < TargetMoves.Length; i++)
                {
                    TargetMoves[i] = (PBEMove)r.ReadUInt16();
                }
            }
        }

        public void Dispose() { }
    }
}
