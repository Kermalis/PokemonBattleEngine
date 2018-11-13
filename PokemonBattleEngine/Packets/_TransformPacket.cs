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

        public readonly Guid UserId, TargetId;
        public readonly ushort TargetAttack, TargetDefense, TargetSpAttack, TargetSpDefense, TargetSpeed;
        public readonly PAbility TargetAbility;
        public readonly PMove[] TargetMoves;

        public PTransformPacket(PPokemon user, PPokemon target)
        {
            var bytes = new List<byte>();
            bytes.AddRange(BitConverter.GetBytes(Code));
            bytes.AddRange((UserId = user.Id).ToByteArray());
            bytes.AddRange((TargetId = target.Id).ToByteArray());
            bytes.AddRange(BitConverter.GetBytes(TargetAttack = target.Attack));
            bytes.AddRange(BitConverter.GetBytes(TargetDefense = target.Defense));
            bytes.AddRange(BitConverter.GetBytes(TargetSpAttack = target.SpAttack));
            bytes.AddRange(BitConverter.GetBytes(TargetSpDefense = target.SpDefense));
            bytes.AddRange(BitConverter.GetBytes(TargetSpeed = target.Speed));
            bytes.Add((byte)(TargetAbility = target.Ability));
            for (int i = 0; i < PSettings.NumMoves; i++)
                bytes.AddRange(BitConverter.GetBytes((ushort)target.Moves[i]));
            Buffer = BitConverter.GetBytes((short)bytes.Count).Concat(bytes);
        }
        public PTransformPacket(byte[] buffer)
        {
            Buffer = buffer;
            using (var r = new BinaryReader(new MemoryStream(buffer)))
            {
                r.ReadInt16(); // Skip Code
                UserId = new Guid(r.ReadBytes(0x10));
                TargetId = new Guid(r.ReadBytes(0x10));
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
