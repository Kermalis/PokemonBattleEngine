using Kermalis.EndianBinaryIO;
using Kermalis.PokemonBattleEngine.Battle;
using Kermalis.PokemonBattleEngine.Data;
using System;
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

        internal PBEPkmnSwitchOutPacket(PBEBattlePokemon pokemon, PBEBattlePokemon disguisedAsPokemon, PBEFieldPosition oldPosition, PBEBattlePokemon forcedByPokemon = null)
        {
            using (var ms = new MemoryStream())
            using (var w = new EndianBinaryWriter(ms, encoding: EncodingType.UTF16))
            {
                w.Write(Code);
                w.Write(PokemonId = pokemon.Id);
                w.Write(DisguisedAsPokemonId = disguisedAsPokemon.Id);
                w.Write(PokemonPosition = oldPosition);
                w.Write((PokemonTeam = pokemon.Team).Id);
                w.Write(Forced = forcedByPokemon != null);
                if (Forced)
                {
                    w.Write((ForcedByPokemonPosition = forcedByPokemon.FieldPosition).Value);
                    w.Write((ForcedByPokemonTeam = forcedByPokemon.Team).Id);
                }
                Data = new ReadOnlyCollection<byte>(ms.ToArray());
            }
        }
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
    }
    public sealed class PBEPkmnSwitchOutPacket_Hidden : IPBEPacket
    {
        public const ushort Code = 0x37;
        public ReadOnlyCollection<byte> Data { get; }

        public PBEFieldPosition PokemonPosition { get; }
        public PBETeam PokemonTeam { get; }
        public bool Forced { get; }
        public PBEFieldPosition? ForcedByPokemonPosition { get; }
        public PBETeam ForcedByPokemonTeam { get; }

        public PBEPkmnSwitchOutPacket_Hidden(PBEPkmnSwitchOutPacket other)
        {
            if (other == null)
            {
                throw new ArgumentNullException(nameof(other));
            }
            using (var ms = new MemoryStream())
            using (var w = new EndianBinaryWriter(ms, encoding: EncodingType.UTF16))
            {
                w.Write(Code);
                w.Write(PokemonPosition = other.PokemonPosition);
                w.Write((PokemonTeam = other.PokemonTeam).Id);
                w.Write(Forced = other.Forced);
                if (Forced)
                {
                    w.Write((ForcedByPokemonPosition = other.ForcedByPokemonPosition).Value);
                    w.Write((ForcedByPokemonTeam = other.ForcedByPokemonTeam).Id);
                }
                Data = new ReadOnlyCollection<byte>(ms.ToArray());
            }
        }
        internal PBEPkmnSwitchOutPacket_Hidden(byte[] data, EndianBinaryReader r, PBEBattle battle)
        {
            Data = new ReadOnlyCollection<byte>(data);
            PokemonPosition = r.ReadEnum<PBEFieldPosition>();
            PokemonTeam = battle.Teams[r.ReadByte()];
            Forced = r.ReadBoolean();
            if (Forced)
            {
                ForcedByPokemonPosition = r.ReadEnum<PBEFieldPosition>();
                ForcedByPokemonTeam = battle.Teams[r.ReadByte()];
            }
        }
    }
}
