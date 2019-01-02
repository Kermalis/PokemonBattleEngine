using Ether.Network.Packets;
using Kermalis.PokemonBattleEngine.Battle;
using Kermalis.PokemonBattleEngine.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Kermalis.PokemonBattleEngine.Packets
{
    public sealed class PBEPkmnSwitchOutPacket : INetPacket
    {
        public const short Code = 0x0C;
        public IEnumerable<byte> Buffer { get; }

        public byte Pokemon { get; }
        public bool Forced { get; }

        public PBEPkmnSwitchOutPacket(PBEPokemon pokemon, bool forced)
        {
            var bytes = new List<byte>();
            bytes.AddRange(BitConverter.GetBytes(Code));
            bytes.Add(Pokemon = pokemon.Id);
            bytes.Add((byte)((Forced = forced) ? 1 : 0));
            Buffer = BitConverter.GetBytes((short)bytes.Count).Concat(bytes);
        }
        public PBEPkmnSwitchOutPacket(byte[] buffer, PBEBattle battle)
        {
            Buffer = buffer;
            using (var r = new BinaryReader(new MemoryStream(buffer)))
            {
                r.ReadInt16(); // Skip Code
                Pokemon = r.ReadByte();
                Forced = r.ReadBoolean();
            }
        }

        public void Dispose() { }
    }
}
