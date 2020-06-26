using Kermalis.EndianBinaryIO;
using Kermalis.PokemonBattleEngine.Battle;
using Kermalis.PokemonBattleEngine.Data;
using System;
using System.Collections.ObjectModel;
using System.IO;

namespace Kermalis.PokemonBattleEngine.Packets
{
    public interface IPBEPkmnFaintedPacket : IPBEPacket
    {
        PBEBattlePokemon Pokemon { get; }
        PBEFieldPosition OldPosition { get; }
    }
    public sealed class PBEPkmnFaintedPacket : IPBEPkmnFaintedPacket
    {
        public const ushort Code = 0x0E;
        public ReadOnlyCollection<byte> Data { get; }

        public PBEBattlePokemon Pokemon { get; }
        public PBEBattlePokemon DisguisedAsPokemon { get; }
        public PBEFieldPosition OldPosition { get; }

        internal PBEPkmnFaintedPacket(PBEBattlePokemon pokemon, PBEBattlePokemon disguisedAsPokemon, PBEFieldPosition oldPosition)
        {
            using (var ms = new MemoryStream())
            using (var w = new EndianBinaryWriter(ms, encoding: EncodingType.UTF16))
            {
                w.Write(Code);
                (Pokemon = pokemon).ToBytes_Id(w, DisguisedAsPokemon = disguisedAsPokemon);
                w.Write(OldPosition = oldPosition);
                Data = new ReadOnlyCollection<byte>(ms.ToArray());
            }
        }
        internal PBEPkmnFaintedPacket(byte[] data, EndianBinaryReader r, PBEBattle battle)
        {
            Data = new ReadOnlyCollection<byte>(data);
            (Pokemon, DisguisedAsPokemon) = battle.GetPokemon_DisguisedId(r);
            OldPosition = r.ReadEnum<PBEFieldPosition>();
        }
    }
    public sealed class PBEPkmnFaintedPacket_Hidden : IPBEPkmnFaintedPacket
    {
        public const ushort Code = 0x2F;
        public ReadOnlyCollection<byte> Data { get; }

        public PBEBattlePokemon Pokemon { get; }
        public PBEFieldPosition OldPosition { get; }

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
                (Pokemon = other.Pokemon).ToBytes_Position(w);
                w.Write(OldPosition = other.OldPosition);
                Data = new ReadOnlyCollection<byte>(ms.ToArray());
            }
        }
        internal PBEPkmnFaintedPacket_Hidden(byte[] data, EndianBinaryReader r, PBEBattle battle)
        {
            Data = new ReadOnlyCollection<byte>(data);
            Pokemon = battle.GetPokemon_Position(r);
            OldPosition = r.ReadEnum<PBEFieldPosition>();
        }
    }
}
