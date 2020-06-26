using Kermalis.EndianBinaryIO;
using Kermalis.PokemonBattleEngine.Battle;
using System;
using System.Collections.ObjectModel;
using System.IO;

namespace Kermalis.PokemonBattleEngine.Packets
{
    public sealed class PBEPkmnHPChangedPacket : IPBEPacket
    {
        public const ushort Code = 0x0A;
        public ReadOnlyCollection<byte> Data { get; }

        public PBEBattlePokemon Pokemon { get; }
        public ushort OldHP { get; }
        public ushort NewHP { get; }
        public double OldHPPercentage { get; }
        public double NewHPPercentage { get; }

        internal PBEPkmnHPChangedPacket(PBEBattlePokemon pokemon, ushort oldHP, double oldHPPercentage)
        {
            using (var ms = new MemoryStream())
            using (var w = new EndianBinaryWriter(ms, encoding: EncodingType.UTF16))
            {
                w.Write(Code);
                (Pokemon = pokemon).ToBytes_Position(w);
                w.Write(OldHP = oldHP);
                w.Write(NewHP = pokemon.HP);
                w.Write(OldHPPercentage = oldHPPercentage);
                w.Write(NewHPPercentage = pokemon.HPPercentage);
                Data = new ReadOnlyCollection<byte>(ms.ToArray());
            }
        }
        internal PBEPkmnHPChangedPacket(byte[] data, EndianBinaryReader r, PBEBattle battle)
        {
            Data = new ReadOnlyCollection<byte>(data);
            Pokemon = battle.GetPokemon_Position(r);
            OldHP = r.ReadUInt16();
            NewHP = r.ReadUInt16();
            OldHPPercentage = r.ReadDouble();
            NewHPPercentage = r.ReadDouble();
        }
    }
    public sealed class PBEPkmnHPChangedPacket_Hidden : IPBEPacket
    {
        public const ushort Code = 0x35;
        public ReadOnlyCollection<byte> Data { get; }

        public PBEBattlePokemon Pokemon { get; }
        public double OldHPPercentage { get; }
        public double NewHPPercentage { get; }

        public PBEPkmnHPChangedPacket_Hidden(PBEPkmnHPChangedPacket other)
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
                w.Write(OldHPPercentage = other.OldHPPercentage);
                w.Write(NewHPPercentage = other.NewHPPercentage);
                Data = new ReadOnlyCollection<byte>(ms.ToArray());
            }
        }
        internal PBEPkmnHPChangedPacket_Hidden(byte[] data, EndianBinaryReader r, PBEBattle battle)
        {
            Data = new ReadOnlyCollection<byte>(data);
            Pokemon = battle.GetPokemon_Position(r);
            OldHPPercentage = r.ReadDouble();
            NewHPPercentage = r.ReadDouble();
        }
    }
}
