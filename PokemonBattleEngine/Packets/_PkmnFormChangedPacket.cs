using Ether.Network.Packets;
using Kermalis.PokemonBattleEngine.Battle;
using Kermalis.PokemonBattleEngine.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Kermalis.PokemonBattleEngine.Packets
{
    public sealed class PBEPkmnFormChangedPacket : INetPacket
    {
        public const short Code = 0x29;
        public IEnumerable<byte> Buffer { get; }

        public PBEFieldPosition Pokemon { get; }
        public PBETeam PokemonTeam { get; }
        public ushort NewAttack { get; }
        public ushort NewDefense { get; }
        public ushort NewSpAttack { get; }
        public ushort NewSpDefense { get; }
        public ushort NewSpeed { get; }
        public PBEAbility NewAbility { get; }
        public PBEAbility NewKnownAbility { get; }
        public PBESpecies NewSpecies { get; }
        public PBEType NewType1 { get; }
        public PBEType NewType2 { get; }
        public double NewWeight { get; }

        public PBEPkmnFormChangedPacket(PBEFieldPosition pokemonPosition, PBETeam pokemonTeam, ushort newAttack, ushort newDefense, ushort newSpAttack, ushort newSpDefense, ushort newSpeed, PBEAbility newAbility, PBEAbility newKnownAbility, PBESpecies newSpecies, PBEType newType1, PBEType newType2, double newWeight)
        {
            var bytes = new List<byte>();
            bytes.AddRange(BitConverter.GetBytes(Code));
            bytes.Add((byte)(Pokemon = pokemonPosition));
            bytes.Add((PokemonTeam = pokemonTeam).Id);
            bytes.AddRange(BitConverter.GetBytes(NewAttack = newAttack));
            bytes.AddRange(BitConverter.GetBytes(NewDefense = newDefense));
            bytes.AddRange(BitConverter.GetBytes(NewSpAttack = newSpAttack));
            bytes.AddRange(BitConverter.GetBytes(NewSpDefense = newSpDefense));
            bytes.AddRange(BitConverter.GetBytes(NewSpeed = newSpeed));
            bytes.Add((byte)(NewAbility = newAbility));
            bytes.Add((byte)(NewKnownAbility = newKnownAbility));
            bytes.AddRange(BitConverter.GetBytes((uint)(NewSpecies = newSpecies)));
            bytes.Add((byte)(NewType1 = newType1));
            bytes.Add((byte)(NewType2 = newType2));
            bytes.AddRange(BitConverter.GetBytes(NewWeight = newWeight));
            Buffer = BitConverter.GetBytes((short)bytes.Count).Concat(bytes);
        }
        public PBEPkmnFormChangedPacket(byte[] buffer, PBEBattle battle)
        {
            Buffer = buffer;
            using (var r = new BinaryReader(new MemoryStream(buffer)))
            {
                r.ReadInt16(); // Skip Code
                Pokemon = (PBEFieldPosition)r.ReadByte();
                PokemonTeam = battle.Teams[r.ReadByte()];
                NewAttack = r.ReadUInt16();
                NewDefense = r.ReadUInt16();
                NewSpAttack = r.ReadUInt16();
                NewSpDefense = r.ReadUInt16();
                NewSpeed = r.ReadUInt16();
                NewAbility = (PBEAbility)r.ReadByte();
                NewKnownAbility = (PBEAbility)r.ReadByte();
                NewSpecies = (PBESpecies)r.ReadUInt32();
                NewType1 = (PBEType)r.ReadByte();
                NewType2 = (PBEType)r.ReadByte();
                NewWeight = r.ReadDouble();
            }
        }

        public void Dispose() { }
    }
}
