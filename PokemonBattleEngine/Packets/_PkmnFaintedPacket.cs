using Kermalis.EndianBinaryIO;
using Kermalis.PokemonBattleEngine.Battle;
using Kermalis.PokemonBattleEngine.Data;
using System.Collections.ObjectModel;
using System.IO;

namespace Kermalis.PokemonBattleEngine.Packets
{
    public sealed class PBEPkmnFaintedPacket : IPBEPacket
    {
        public const ushort Code = 0x0E;
        public ReadOnlyCollection<byte> Data { get; }

        public byte PokemonId { get; }
        public PBEFieldPosition PokemonPosition { get; }
        public PBETeam PokemonTeam { get; }

        private PBEPkmnFaintedPacket(byte pokemonId, PBEFieldPosition pokemonPosition, PBETeam pokemonTeam)
        {
            using (var ms = new MemoryStream())
            using (var w = new EndianBinaryWriter(ms, encoding: EncodingType.UTF16))
            {
                w.Write(Code);
                w.Write(PokemonId = pokemonId);
                w.Write(PokemonPosition = pokemonPosition);
                w.Write((PokemonTeam = pokemonTeam).Id);
                Data = new ReadOnlyCollection<byte>(ms.ToArray());
            }
        }
        internal PBEPkmnFaintedPacket(PBEBattlePokemon pokemon, PBEFieldPosition oldPosition)
            : this(pokemon.Id, oldPosition, pokemon.Team) { }
        internal PBEPkmnFaintedPacket(byte[] data, EndianBinaryReader r, PBEBattle battle)
        {
            Data = new ReadOnlyCollection<byte>(data);
            PokemonId = r.ReadByte();
            PokemonPosition = r.ReadEnum<PBEFieldPosition>();
            PokemonTeam = battle.Teams[r.ReadByte()];
        }

        public PBEPkmnFaintedPacket MakeHidden()
        {
            return new PBEPkmnFaintedPacket(byte.MaxValue, PokemonPosition, PokemonTeam);
        }
    }
}
