using Kermalis.EndianBinaryIO;
using Kermalis.PokemonBattleEngine.Battle;
using Kermalis.PokemonBattleEngine.Data;
using System;
using System.Collections.ObjectModel;
using System.IO;

namespace Kermalis.PokemonBattleEngine.Packets
{
    public interface IPBEPkmnSwitchOutPacket : IPBEPacket
    {
        PBEBattlePokemon Pokemon { get; }
        PBEFieldPosition OldPosition { get; }
        bool Forced { get; }
        PBEBattlePokemon ForcedByPokemon { get; }
    }
    public sealed class PBEPkmnSwitchOutPacket : IPBEPkmnSwitchOutPacket
    {
        public const ushort Code = 0x0C;
        public ReadOnlyCollection<byte> Data { get; }

        public PBEBattlePokemon Pokemon { get; }
        public PBEBattlePokemon DisguisedAsPokemon { get; }
        public PBEFieldPosition OldPosition { get; }
        public bool Forced { get; }
        public PBEBattlePokemon ForcedByPokemon { get; }

        internal PBEPkmnSwitchOutPacket(PBEBattlePokemon pokemon, PBEBattlePokemon disguisedAsPokemon, PBEFieldPosition oldPosition, PBEBattlePokemon forcedByPokemon = null)
        {
            using (var ms = new MemoryStream())
            using (var w = new EndianBinaryWriter(ms, encoding: EncodingType.UTF16))
            {
                w.Write(Code);
                (Pokemon = pokemon).ToBytes_Id(w, DisguisedAsPokemon = disguisedAsPokemon);
                w.Write(OldPosition = oldPosition);
                w.Write(Forced = forcedByPokemon != null);
                if (Forced)
                {
                    (ForcedByPokemon = forcedByPokemon).ToBytes_Position(w);
                }
                Data = new ReadOnlyCollection<byte>(ms.ToArray());
            }
        }
        internal PBEPkmnSwitchOutPacket(byte[] data, EndianBinaryReader r, PBEBattle battle)
        {
            Data = new ReadOnlyCollection<byte>(data);
            (Pokemon, DisguisedAsPokemon) = battle.GetPokemon_DisguisedId(r);
            OldPosition = r.ReadEnum<PBEFieldPosition>();
            Forced = r.ReadBoolean();
            if (Forced)
            {
                ForcedByPokemon = battle.GetPokemon_Position(r);
            }
        }
    }
    public sealed class PBEPkmnSwitchOutPacket_Hidden : IPBEPkmnSwitchOutPacket
    {
        public const ushort Code = 0x37;
        public ReadOnlyCollection<byte> Data { get; }

        public PBEBattlePokemon Pokemon { get; }
        public PBEFieldPosition OldPosition { get; }
        public bool Forced { get; }
        public PBEBattlePokemon ForcedByPokemon { get; }

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
                (Pokemon = other.Pokemon).ToBytes_Position(w);
                w.Write(OldPosition = other.OldPosition);
                w.Write(Forced = other.Forced);
                if (Forced)
                {
                    (ForcedByPokemon = other.ForcedByPokemon).ToBytes_Position(w);
                }
                Data = new ReadOnlyCollection<byte>(ms.ToArray());
            }
        }
        internal PBEPkmnSwitchOutPacket_Hidden(byte[] data, EndianBinaryReader r, PBEBattle battle)
        {
            Data = new ReadOnlyCollection<byte>(data);
            Pokemon = battle.GetPokemon_Position(r);
            OldPosition = r.ReadEnum<PBEFieldPosition>();
            Forced = r.ReadBoolean();
            if (Forced)
            {
                ForcedByPokemon = battle.GetPokemon_Position(r);
            }
        }
    }
}
