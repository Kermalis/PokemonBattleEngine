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
        public PBESpecies NewSpecies { get; }

        public PBEPkmnFormChangedPacket(PBEPokemon pokemon, PBESpecies newSpecies)
        {
            var bytes = new List<byte>();
            bytes.AddRange(BitConverter.GetBytes(Code));
            bytes.Add((byte)(Pokemon = pokemon.FieldPosition));
            bytes.Add((PokemonTeam = pokemon.Team).Id);
            bytes.AddRange(BitConverter.GetBytes((uint)(NewSpecies = newSpecies)));
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
                NewSpecies = (PBESpecies)r.ReadUInt32();
            }
        }

        public void Dispose() { }
    }
}
