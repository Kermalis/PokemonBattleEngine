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
    public sealed class PBEPartyResponsePacket : INetPacket
    {
        public const short Code = 0x04;
        public IEnumerable<byte> Buffer { get; }

        public ReadOnlyCollection<PBEPokemonShell> Party { get; }

        public PBEPartyResponsePacket(IEnumerable<PBEPokemonShell> party)
        {
            var bytes = new List<byte>();
            bytes.AddRange(BitConverter.GetBytes(Code));
            bytes.Add((byte)(Party = party.ToList().AsReadOnly()).Count);
            foreach (PBEPokemonShell pkmn in Party)
            {
                bytes.AddRange(pkmn.ToBytes());
            }
            Buffer = BitConverter.GetBytes((short)bytes.Count).Concat(bytes);
        }
        public PBEPartyResponsePacket(byte[] buffer, PBEBattle battle)
        {
            Buffer = buffer;
            using (var r = new BinaryReader(new MemoryStream(buffer)))
            {
                r.ReadInt16(); // Skip Code
                var party = new PBEPokemonShell[r.ReadByte()];
                for (int i = 0; i < party.Length; i++)
                {
                    party[i] = PBEPokemonShell.FromBytes(r);
                }
                Party = Array.AsReadOnly(party);
            }
        }

        public void Dispose() { }
    }
}
