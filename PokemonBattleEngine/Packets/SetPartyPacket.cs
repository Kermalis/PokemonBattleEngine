using Ether.Network.Packets;
using Kermalis.PokemonBattleEngine.Battle;
using Kermalis.PokemonBattleEngine.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

namespace Kermalis.PokemonBattleEngine.Packets
{
    public sealed class PBESetPartyPacket : INetPacket
    {
        public const short Code = 0x05;
        public IEnumerable<byte> Buffer { get; }

        public PBETeam Team { get; }
        public ReadOnlyCollection<PBEPokemon> Party { get; }

        public PBESetPartyPacket(PBETeam team)
        {
            var bytes = new List<byte>();
            bytes.AddRange(BitConverter.GetBytes(Code));
            bytes.Add((Team = team).Id);
            bytes.Add((byte)(Party = Team.Party.AsReadOnly()).Count);
            foreach (PBEPokemon pkmn in Party)
            {
                bytes.AddRange(pkmn.ToBytes());
            }
            Buffer = BitConverter.GetBytes((short)bytes.Count).Concat(bytes);
        }
        public PBESetPartyPacket(byte[] buffer, PBEBattle battle)
        {
            Buffer = buffer;
            using (var r = new BinaryReader(new MemoryStream(buffer)))
            {
                r.ReadInt16(); // Skip Code
                Team = battle.Teams[r.ReadByte()];
                var party = new List<PBEPokemon>(r.ReadByte());
                for (int i = 0; i < party.Capacity; i++)
                {
                    party.Add(PBEPokemon.FromBytes(r, Team));
                }
                Party = party.AsReadOnly();
            }
        }

        public void Dispose() { }
    }
}
