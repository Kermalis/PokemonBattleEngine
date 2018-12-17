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

        public byte UserId { get; }
        public byte TargetId { get; }
        public ushort TargetAttack { get; }
        public ushort TargetDefense { get; }
        public ushort TargetSpAttack { get; }
        public ushort TargetSpDefense { get; }
        public ushort TargetSpeed { get; }
        public sbyte TargetAttackChange { get; }
        public sbyte TargetDefenseChange { get; }
        public sbyte TargetSpAttackChange { get; }
        public sbyte TargetSpDefenseChange { get; }
        public sbyte TargetSpeedChange { get; }
        public sbyte TargetAccuracyChange { get; }
        public sbyte TargetEvasionChange { get; }
        public PBEAbility TargetAbility { get; }
        public PBEType TargetType1 { get; }
        public PBEType TargetType2 { get; }
        public double TargetWeight { get; }
        public PBEMove[] TargetMoves { get; }

        public PBETransformPacket(PBEPokemon user, PBEPokemon target, PBESettings settings)
        {
            var bytes = new List<byte>();
            bytes.AddRange(BitConverter.GetBytes(Code));
            bytes.Add(UserId = user.Id);
            bytes.Add(TargetId = target.Id);
            bytes.AddRange(BitConverter.GetBytes(TargetAttack = target.Attack));
            bytes.AddRange(BitConverter.GetBytes(TargetDefense = target.Defense));
            bytes.AddRange(BitConverter.GetBytes(TargetSpAttack = target.SpAttack));
            bytes.AddRange(BitConverter.GetBytes(TargetSpDefense = target.SpDefense));
            bytes.AddRange(BitConverter.GetBytes(TargetSpeed = target.Speed));
            bytes.Add((byte)(TargetAttackChange = target.AttackChange));
            bytes.Add((byte)(TargetDefenseChange = target.DefenseChange));
            bytes.Add((byte)(TargetSpAttackChange = target.SpAttackChange));
            bytes.Add((byte)(TargetSpDefenseChange = target.SpDefenseChange));
            bytes.Add((byte)(TargetSpeedChange = target.SpeedChange));
            bytes.Add((byte)(TargetAccuracyChange = target.AccuracyChange));
            bytes.Add((byte)(TargetEvasionChange = target.EvasionChange));
            bytes.Add((byte)(TargetAbility = target.Ability));
            bytes.Add((byte)(TargetType1 = target.Type1));
            bytes.Add((byte)(TargetType2 = target.Type2));
            bytes.AddRange(BitConverter.GetBytes(TargetWeight = target.Weight));
            for (int i = 0; i < settings.NumMoves; i++)
            {
                bytes.AddRange(BitConverter.GetBytes((ushort)target.Moves[i]));
            }
            Buffer = BitConverter.GetBytes((short)bytes.Count).Concat(bytes);
        }
        public PBETransformPacket(byte[] buffer, PBESettings settings)
        {
            Buffer = buffer;
            using (var r = new BinaryReader(new MemoryStream(buffer)))
            {
                r.ReadInt16(); // Skip Code
                UserId = r.ReadByte();
                TargetId = r.ReadByte();
                TargetAttack = r.ReadUInt16();
                TargetDefense = r.ReadUInt16();
                TargetSpAttack = r.ReadUInt16();
                TargetSpDefense = r.ReadUInt16();
                TargetSpeed = r.ReadUInt16();
                TargetAttackChange = r.ReadSByte();
                TargetDefenseChange = r.ReadSByte();
                TargetSpAttackChange = r.ReadSByte();
                TargetSpDefenseChange = r.ReadSByte();
                TargetSpeedChange = r.ReadSByte();
                TargetAccuracyChange = r.ReadSByte();
                TargetEvasionChange = r.ReadSByte();
                TargetAbility = (PBEAbility)r.ReadByte();
                TargetType1 = (PBEType)r.ReadByte();
                TargetType2 = (PBEType)r.ReadByte();
                TargetWeight = r.ReadDouble();
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
