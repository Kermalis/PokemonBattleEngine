using Ether.Network.Packets;
using Kermalis.PokemonBattleEngine.Battle;
using Kermalis.PokemonBattleEngine.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;

namespace Kermalis.PokemonBattleEngine.Packets
{
    public sealed class PBEPkmnHPChangedPacket : INetPacket
    {
        public const short Code = 0x0A;
        public ReadOnlyCollection<byte> Buffer { get; }

        public PBEFieldPosition Pokemon { get; }
        public PBETeam PokemonTeam { get; }
        public ushort OldHP { get; }
        public ushort NewHP { get; }
        public double OldHPPercentage { get; }
        public double NewHPPercentage { get; }

        internal PBEPkmnHPChangedPacket(PBEFieldPosition pokemon, PBETeam pokemonTeam, ushort oldHP, ushort newHP, double oldHPPercentage, double newHPPercentage)
        {
            var bytes = new List<byte>();
            bytes.AddRange(BitConverter.GetBytes(Code));
            bytes.Add((byte)(Pokemon = pokemon));
            bytes.Add((PokemonTeam = pokemonTeam).Id);
            bytes.AddRange(BitConverter.GetBytes(OldHP = oldHP));
            bytes.AddRange(BitConverter.GetBytes(NewHP = newHP));
            bytes.AddRange(BitConverter.GetBytes(OldHPPercentage = oldHPPercentage));
            bytes.AddRange(BitConverter.GetBytes(NewHPPercentage = newHPPercentage));
            bytes.InsertRange(0, BitConverter.GetBytes((short)bytes.Count));
            Buffer = new ReadOnlyCollection<byte>(bytes);
        }
        internal PBEPkmnHPChangedPacket(ReadOnlyCollection<byte> buffer, BinaryReader r, PBEBattle battle)
        {
            Buffer = buffer;
            Pokemon = (PBEFieldPosition)r.ReadByte();
            PokemonTeam = battle.Teams[r.ReadByte()];
            OldHP = r.ReadUInt16();
            NewHP = r.ReadUInt16();
            OldHPPercentage = r.ReadDouble();
            NewHPPercentage = r.ReadDouble();
        }

        public PBEPkmnHPChangedPacket MakeHidden()
        {
            return new PBEPkmnHPChangedPacket(Pokemon, PokemonTeam, ushort.MinValue, ushort.MinValue, OldHPPercentage, NewHPPercentage);
        }

        public void Dispose() { }
    }
}
