using Ether.Network.Packets;
using Kermalis.PokemonBattleEngine.Battle;
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
            bytes.AddRange(Party.SelectMany(p => p.ToBytes()));
            Buffer = BitConverter.GetBytes((short)bytes.Count).Concat(bytes);
        }
        public PBESetPartyPacket(byte[] buffer, PBEBattle battle)
        {
            Buffer = buffer;
            using (var r = new BinaryReader(new MemoryStream(buffer)))
            {
                r.ReadInt16(); // Skip Code
                Team = battle.Teams[r.ReadByte()];
                var party = new PBEPokemon[r.ReadByte()];
                for (int i = 0; i < party.Length; i++)
                {
                    party[i] = new PBEPokemon(r, Team);
                }
                Party = Array.AsReadOnly(party);
            }
        }

        public void Dispose() { }
    }
}
