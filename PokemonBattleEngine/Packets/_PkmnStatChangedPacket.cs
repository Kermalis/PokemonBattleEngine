using Ether.Network.Packets;
using Kermalis.PokemonBattleEngine.Battle;
using Kermalis.PokemonBattleEngine.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Kermalis.PokemonBattleEngine.Packets
{
    public sealed class PBEPkmnStatChangedPacket : INetPacket
    {
        public const short Code = 0x10;
        public IEnumerable<byte> Buffer { get; }

        public PBEFieldPosition Pokemon { get; }
        public PBETeam PokemonTeam { get; }
        public PBEStat Stat { get; }
        public sbyte OldValue { get; }
        public sbyte NewValue { get; }

        public PBEPkmnStatChangedPacket(PBEPokemon pokemon, PBEStat stat, sbyte oldValue, sbyte newValue)
        {
            var bytes = new List<byte>();
            bytes.AddRange(BitConverter.GetBytes(Code));
            bytes.Add((byte)(Pokemon = pokemon.FieldPosition));
            bytes.Add((PokemonTeam = pokemon.Team).Id);
            bytes.Add((byte)(Stat = stat));
            bytes.Add((byte)(OldValue = oldValue));
            bytes.Add((byte)(NewValue = newValue));
            Buffer = BitConverter.GetBytes((short)bytes.Count).Concat(bytes);
        }
        public PBEPkmnStatChangedPacket(byte[] buffer, PBEBattle battle)
        {
            Buffer = buffer;
            using (var r = new BinaryReader(new MemoryStream(buffer)))
            {
                r.ReadInt16(); // Skip Code
                Pokemon = (PBEFieldPosition)r.ReadByte();
                PokemonTeam = battle.Teams[r.ReadByte()];
                Stat = (PBEStat)r.ReadByte();
                OldValue = r.ReadSByte();
                NewValue = r.ReadSByte();
            }
        }

        public void Dispose() { }
    }
}
