using Ether.Network.Packets;
using Kermalis.PokemonBattleEngine.Battle;
using Kermalis.PokemonBattleEngine.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Kermalis.PokemonBattleEngine.Packets
{
    public sealed class PBEPkmnHPChangedPacket : INetPacket
    {
        public const short Code = 0x0A;
        public IEnumerable<byte> Buffer { get; }

        public PBEFieldPosition Pokemon { get; }
        public PBETeam PokemonTeam { get; }
        public ushort OldHP { get; }
        public ushort NewHP { get; }
        public double OldHPPercentage { get; }
        public double NewHPPercentage { get; }

        public PBEPkmnHPChangedPacket(PBEFieldPosition pokemon, PBETeam pokemonTeam, ushort oldHP, ushort newHP, double oldHPPercentage, double newHPPercentage)
        {
            var bytes = new List<byte>();
            bytes.AddRange(BitConverter.GetBytes(Code));
            bytes.Add((byte)(Pokemon = pokemon));
            bytes.Add((PokemonTeam = pokemonTeam).Id);
            bytes.AddRange(BitConverter.GetBytes(OldHP = oldHP));
            bytes.AddRange(BitConverter.GetBytes(NewHP = newHP));
            bytes.AddRange(BitConverter.GetBytes(OldHPPercentage = oldHPPercentage));
            bytes.AddRange(BitConverter.GetBytes(NewHPPercentage = newHPPercentage));
            Buffer = BitConverter.GetBytes((short)bytes.Count).Concat(bytes);
        }
        public PBEPkmnHPChangedPacket(byte[] buffer, PBEBattle battle)
        {
            Buffer = buffer;
            using (var r = new BinaryReader(new MemoryStream(buffer)))
            {
                r.ReadInt16(); // Skip Code
                Pokemon = (PBEFieldPosition)r.ReadByte();
                PokemonTeam = battle.Teams[r.ReadByte()];
                OldHP = r.ReadUInt16();
                NewHP = r.ReadUInt16();
                OldHPPercentage = r.ReadDouble();
                NewHPPercentage = r.ReadDouble();
            }
        }

        public void Dispose() { }
    }
}
