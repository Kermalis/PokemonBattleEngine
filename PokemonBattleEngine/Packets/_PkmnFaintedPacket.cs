using Ether.Network.Packets;
using Kermalis.PokemonBattleEngine.Battle;
using Kermalis.PokemonBattleEngine.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;

namespace Kermalis.PokemonBattleEngine.Packets
{
    public sealed class PBEPkmnFaintedPacket : INetPacket
    {
        public const short Code = 0x0E;
        public ReadOnlyCollection<byte> Buffer { get; }

        public byte PokemonId { get; }
        public PBEFieldPosition PokemonPosition { get; }
        public PBETeam PokemonTeam { get; }

        internal PBEPkmnFaintedPacket(byte pokemonId, PBEFieldPosition pokemonPosition, PBETeam pokemonTeam)
        {
            var bytes = new List<byte>();
            bytes.AddRange(BitConverter.GetBytes(Code));
            bytes.Add(PokemonId = pokemonId);
            bytes.Add((byte)(PokemonPosition = pokemonPosition));
            bytes.Add((PokemonTeam = pokemonTeam).Id);
            bytes.InsertRange(0, BitConverter.GetBytes((short)bytes.Count));
            Buffer = new ReadOnlyCollection<byte>(bytes);
        }
        internal PBEPkmnFaintedPacket(ReadOnlyCollection<byte> buffer, BinaryReader r, PBEBattle battle)
        {
            Buffer = buffer;
            PokemonId = r.ReadByte();
            PokemonPosition = (PBEFieldPosition)r.ReadByte();
            PokemonTeam = battle.Teams[r.ReadByte()];
        }

        public PBEPkmnFaintedPacket MakeHidden()
        {
            return new PBEPkmnFaintedPacket(byte.MaxValue, PokemonPosition, PokemonTeam);
        }

        public void Dispose() { }
    }
}
