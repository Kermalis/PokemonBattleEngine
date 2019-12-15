using Kermalis.EndianBinaryIO;
using Kermalis.PokemonBattleEngine.Battle;
using Kermalis.PokemonBattleEngine.Data;
using System.Collections.ObjectModel;
using System.IO;

namespace Kermalis.PokemonBattleEngine.Packets
{
    public sealed class PBEPkmnSwitchOutPacket : IPBEPacket
    {
        public const ushort Code = 0x0C;
        public ReadOnlyCollection<byte> Data { get; }

        public byte PokemonId { get; }
        public byte DisguisedAsPokemonId { get; }
        public PBEFieldPosition PokemonPosition { get; }
        public PBETeam PokemonTeam { get; }
        public bool Forced { get; }
        public PBEFieldPosition? ForcedByPokemonPosition { get; }
        public PBETeam ForcedByPokemonTeam { get; }

        private PBEPkmnSwitchOutPacket(byte pokemonId, byte disguisedAsPokemonId, PBEFieldPosition pokemonPosition, PBETeam pokemonTeam, bool forced = false, PBEFieldPosition? forcedByPokemonPosition = null, PBETeam forcedByPokemonTeam = null)
        {
            using (var ms = new MemoryStream())
            using (var w = new EndianBinaryWriter(ms, encoding: EncodingType.UTF16))
            {
                w.Write(Code);
                w.Write(PokemonId = pokemonId);
                w.Write(DisguisedAsPokemonId = disguisedAsPokemonId);
                w.Write(PokemonPosition = pokemonPosition);
                w.Write((PokemonTeam = pokemonTeam).Id);
                w.Write(Forced = forced);
                if (Forced)
                {
                    w.Write((ForcedByPokemonPosition = forcedByPokemonPosition).Value);
                    w.Write((ForcedByPokemonTeam = forcedByPokemonTeam).Id);
                }
                Data = new ReadOnlyCollection<byte>(ms.ToArray());
            }
        }
        internal PBEPkmnSwitchOutPacket(PBEPokemon pokemon, PBEPokemon disguisedAsPokemon, PBEFieldPosition oldPosition, PBEPokemon forcedByPokemon = null)
            : this(pokemon.Id, disguisedAsPokemon.Id, oldPosition, pokemon.Team, forcedByPokemon != null, forcedByPokemon?.FieldPosition, forcedByPokemon?.Team) { }
        internal PBEPkmnSwitchOutPacket(byte[] data, EndianBinaryReader r, PBEBattle battle)
        {
            Data = new ReadOnlyCollection<byte>(data);
            PokemonId = r.ReadByte();
            DisguisedAsPokemonId = r.ReadByte();
            PokemonPosition = r.ReadEnum<PBEFieldPosition>();
            PokemonTeam = battle.Teams[r.ReadByte()];
            Forced = r.ReadBoolean();
            if (Forced)
            {
                ForcedByPokemonPosition = r.ReadEnum<PBEFieldPosition>();
                ForcedByPokemonTeam = battle.Teams[r.ReadByte()];
            }
        }

        public PBEPkmnSwitchOutPacket MakeHidden()
        {
            return new PBEPkmnSwitchOutPacket(byte.MaxValue, byte.MaxValue, PokemonPosition, PokemonTeam, Forced, ForcedByPokemonPosition, ForcedByPokemonTeam);
        }
    }
}
