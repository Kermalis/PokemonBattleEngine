using Kermalis.EndianBinaryIO;
using Kermalis.PokemonBattleEngine.Battle;
using Kermalis.PokemonBattleEngine.Data;
using System;
using System.Collections.ObjectModel;
using System.IO;

namespace Kermalis.PokemonBattleEngine.Packets
{
    public sealed class PBEPkmnFaintedPacket : IPBEPacket
    {
        public const ushort Code = 0x0E;
        public ReadOnlyCollection<byte> Data { get; }

        public byte PokemonId { get; }
        public byte DisguisedAsPokemonId { get; }
        public PBEFieldPosition PokemonPosition { get; }
        public PBETeam PokemonTeam { get; }

        internal PBEPkmnFaintedPacket(PBEBattlePokemon pokemon, PBEBattlePokemon disguisedAsPokemon, PBEFieldPosition oldPosition)
        {
            using (var ms = new MemoryStream())
            using (var w = new EndianBinaryWriter(ms, encoding: EncodingType.UTF16))
            {
                w.Write(Code);
                w.Write(PokemonId = pokemon.Id);
                w.Write(DisguisedAsPokemonId = disguisedAsPokemon.Id);
                w.Write(PokemonPosition = oldPosition);
                w.Write((PokemonTeam = pokemon.Team).Id);
                Data = new ReadOnlyCollection<byte>(ms.ToArray());
            }
        }
        internal PBEPkmnFaintedPacket(byte[] data, EndianBinaryReader r, PBEBattle battle)
        {
            Data = new ReadOnlyCollection<byte>(data);
            PokemonId = r.ReadByte();
            DisguisedAsPokemonId = r.ReadByte();
            PokemonPosition = r.ReadEnum<PBEFieldPosition>();
            PokemonTeam = battle.Teams[r.ReadByte()];
        }
    }
    public sealed class PBEPkmnFaintedPacket_Hidden : IPBEPacket
    {
        public const ushort Code = 0x2F;
        public ReadOnlyCollection<byte> Data { get; }

        public PBEFieldPosition PokemonPosition { get; }
        public PBETeam PokemonTeam { get; }

        public PBEPkmnFaintedPacket_Hidden(PBEPkmnFaintedPacket other)
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
                Data = new ReadOnlyCollection<byte>(ms.ToArray());
            }
        }
        internal PBEPkmnFaintedPacket_Hidden(byte[] data, EndianBinaryReader r, PBEBattle battle)
        {
            Data = new ReadOnlyCollection<byte>(data);
            PokemonPosition = r.ReadEnum<PBEFieldPosition>();
            PokemonTeam = battle.Teams[r.ReadByte()];
        }
    }
}
