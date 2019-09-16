using Ether.Network.Packets;
using Kermalis.PokemonBattleEngine.Battle;
using Kermalis.PokemonBattleEngine.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;

namespace Kermalis.PokemonBattleEngine.Packets
{
    public sealed class PBEPkmnStatChangedPacket : INetPacket
    {
        public const short Code = 0x10;
        public ReadOnlyCollection<byte> Buffer { get; }

        public PBEFieldPosition Pokemon { get; }
        public PBETeam PokemonTeam { get; }
        public PBEStat Stat { get; }
        public sbyte OldValue { get; }
        public sbyte NewValue { get; }

        internal PBEPkmnStatChangedPacket(PBEPokemon pokemon, PBEStat stat, sbyte oldValue, sbyte newValue)
        {
            var bytes = new List<byte>();
            bytes.AddRange(BitConverter.GetBytes(Code));
            bytes.Add((byte)(Pokemon = pokemon.FieldPosition));
            bytes.Add((PokemonTeam = pokemon.Team).Id);
            bytes.Add((byte)(Stat = stat));
            bytes.Add((byte)(OldValue = oldValue));
            bytes.Add((byte)(NewValue = newValue));
            bytes.InsertRange(0, BitConverter.GetBytes((short)bytes.Count));
            Buffer = new ReadOnlyCollection<byte>(bytes);
        }
        internal PBEPkmnStatChangedPacket(ReadOnlyCollection<byte> buffer, BinaryReader r, PBEBattle battle)
        {
            Buffer = buffer;
            Pokemon = (PBEFieldPosition)r.ReadByte();
            PokemonTeam = battle.Teams[r.ReadByte()];
            Stat = (PBEStat)r.ReadByte();
            OldValue = r.ReadSByte();
            NewValue = r.ReadSByte();
        }

        public void Dispose() { }
    }
}
