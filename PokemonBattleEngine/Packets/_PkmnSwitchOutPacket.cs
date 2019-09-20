using Ether.Network.Packets;
using Kermalis.PokemonBattleEngine.Battle;
using Kermalis.PokemonBattleEngine.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;

namespace Kermalis.PokemonBattleEngine.Packets
{
    public sealed class PBEPkmnSwitchOutPacket : INetPacket
    {
        public const short Code = 0x0C;
        public ReadOnlyCollection<byte> Buffer { get; }

        public byte PokemonId { get; }
        public byte DisguisedAsPokemonId { get; }
        public PBEFieldPosition PokemonPosition { get; }
        public PBETeam PokemonTeam { get; }
        public bool Forced { get; }
        public PBEFieldPosition? ForcedByPokemonPosition { get; }
        public PBETeam ForcedByPokemonTeam { get; }

        private PBEPkmnSwitchOutPacket(byte pokemonId, byte disguisedAsPokemonId, PBEFieldPosition pokemonPosition, PBETeam pokemonTeam, bool forced = false, PBEFieldPosition? forcedByPokemonPosition = null, PBETeam forcedByPokemonTeam = null)
        {
            var bytes = new List<byte>();
            bytes.AddRange(BitConverter.GetBytes(Code));
            bytes.Add(PokemonId = pokemonId);
            bytes.Add(DisguisedAsPokemonId = disguisedAsPokemonId);
            bytes.Add((byte)(PokemonPosition = pokemonPosition));
            bytes.Add((PokemonTeam = pokemonTeam).Id);
            bytes.Add((byte)((Forced = forced) ? 1 : 0));
            if (Forced)
            {
                bytes.Add((byte)(ForcedByPokemonPosition = forcedByPokemonPosition));
                bytes.Add((ForcedByPokemonTeam = forcedByPokemonTeam).Id);
            }
            bytes.InsertRange(0, BitConverter.GetBytes((short)bytes.Count));
            Buffer = new ReadOnlyCollection<byte>(bytes);
        }
        internal PBEPkmnSwitchOutPacket(byte pokemonId, byte disguisedAsPokemonId, PBEFieldPosition pokemonPosition, PBETeam pokemonTeam, PBEPokemon forcedByPokemon = null)
            : this(pokemonId, disguisedAsPokemonId, pokemonPosition, pokemonTeam, forcedByPokemon != null, forcedByPokemon?.FieldPosition, forcedByPokemon?.Team) { }
        internal PBEPkmnSwitchOutPacket(ReadOnlyCollection<byte> buffer, BinaryReader r, PBEBattle battle)
        {
            Buffer = buffer;
            PokemonId = r.ReadByte();
            DisguisedAsPokemonId = r.ReadByte();
            PokemonPosition = (PBEFieldPosition)r.ReadByte();
            PokemonTeam = battle.Teams[r.ReadByte()];
            Forced = r.ReadBoolean();
            if (Forced)
            {
                ForcedByPokemonPosition = (PBEFieldPosition)r.ReadByte();
                ForcedByPokemonTeam = battle.Teams[r.ReadByte()];
            }
        }

        public PBEPkmnSwitchOutPacket MakeHidden()
        {
            return new PBEPkmnSwitchOutPacket(byte.MaxValue, byte.MaxValue, PokemonPosition, PokemonTeam, Forced, ForcedByPokemonPosition, ForcedByPokemonTeam);
        }

        public void Dispose() { }
    }
}
