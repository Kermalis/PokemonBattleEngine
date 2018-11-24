using Ether.Network.Packets;
using Kermalis.PokemonBattleEngine.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Kermalis.PokemonBattleEngine.Packets
{
    public sealed class PTransformPacket : INetPacket
    {
        public const short Code = 0x18;
        public IEnumerable<byte> Buffer { get; }

        public readonly byte CulpritId, VictimId;
        public readonly ushort TargetAttack, TargetDefense, TargetSpAttack, TargetSpDefense, TargetSpeed;
        public readonly PAbility TargetAbility;
        public readonly PMove[] TargetMoves;

        public PTransformPacket(PPokemon culprit, PPokemon victim)
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
            for (int i = 0; i < PSettings.NumMoves; i++)
                bytes.AddRange(BitConverter.GetBytes((ushort)victim.Moves[i]));
            Buffer = BitConverter.GetBytes((short)bytes.Count).Concat(bytes);
        }
        public PTransformPacket(byte[] buffer)
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
                TargetAbility = (PAbility)r.ReadByte();
                TargetMoves = new PMove[PSettings.NumMoves];
                for (int i = 0; i < PSettings.NumMoves; i++)
                    TargetMoves[i] = (PMove)r.ReadUInt16();
            }
        }

        public void Dispose() { }
    }
}
