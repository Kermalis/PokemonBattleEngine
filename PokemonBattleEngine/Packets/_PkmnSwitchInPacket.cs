using Kermalis.EndianBinaryIO;
using Kermalis.PokemonBattleEngine.Battle;
using Kermalis.PokemonBattleEngine.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;

namespace Kermalis.PokemonBattleEngine.Packets
{
    public interface IPBEPkmnSwitchInInfo
    {
        PBESpecies Species { get; }
        PBEForm Form { get; }
        string Nickname { get; }
        byte Level { get; }
        bool Shiny { get; }
        PBEGender Gender { get; }
        double HPPercentage { get; }
        PBEStatus1 Status1 { get; }
        PBEFieldPosition FieldPosition { get; }
    }
    public interface IPBEPkmnSwitchInPacket : IPBEPacket
    {
        PBETrainer Trainer { get; }
        IReadOnlyList<IPBEPkmnSwitchInInfo> SwitchIns { get; }
        bool Forced { get; }
        PBEBattlePokemon ForcedByPokemon { get; }
    }
    public sealed class PBEPkmnSwitchInPacket : IPBEPkmnSwitchInPacket
    {
        public const ushort Code = 0x06;
        public ReadOnlyCollection<byte> Data { get; }

        public sealed class PBESwitchInInfo : IPBEPkmnSwitchInInfo
        {
            public PBEBattlePokemon Pokemon { get; }
            public PBEBattlePokemon DisguisedAsPokemon { get; }
            public PBESpecies Species { get; }
            public PBEForm Form { get; }
            public string Nickname { get; }
            public byte Level { get; }
            public bool Shiny { get; }
            public PBEGender Gender { get; }
            public ushort HP { get; }
            public ushort MaxHP { get; }
            public double HPPercentage { get; }
            public PBEStatus1 Status1 { get; }
            public PBEFieldPosition FieldPosition { get; }

            internal PBESwitchInInfo(PBEBattlePokemon pkmn)
            {
                Pokemon = pkmn;
                DisguisedAsPokemon = pkmn.Status2.HasFlag(PBEStatus2.Disguised) ? pkmn.DisguisedAsPokemon : pkmn;
                Species = pkmn.KnownSpecies;
                Form = pkmn.KnownForm;
                Nickname = pkmn.KnownNickname;
                Level = pkmn.Level;
                Shiny = pkmn.KnownShiny;
                Gender = pkmn.KnownGender;
                HP = pkmn.HP;
                MaxHP = pkmn.MaxHP;
                HPPercentage = pkmn.HPPercentage;
                Status1 = pkmn.Status1;
                FieldPosition = pkmn.FieldPosition;
            }
            internal PBESwitchInInfo(EndianBinaryReader r, PBEBattle battle)
            {
                (Pokemon, DisguisedAsPokemon) = battle.GetPokemon_DisguisedId(r);
                Species = r.ReadEnum<PBESpecies>();
                Form = r.ReadEnum<PBEForm>();
                Nickname = r.ReadStringNullTerminated();
                Level = r.ReadByte();
                Shiny = r.ReadBoolean();
                Gender = r.ReadEnum<PBEGender>();
                HP = r.ReadUInt16();
                MaxHP = r.ReadUInt16();
                HPPercentage = r.ReadDouble();
                Status1 = r.ReadEnum<PBEStatus1>();
                FieldPosition = r.ReadEnum<PBEFieldPosition>();
            }

            internal void ToBytes(EndianBinaryWriter w)
            {
                Pokemon.ToBytes_Id(w, DisguisedAsPokemon);
                w.Write(Species);
                w.Write(Form);
                w.Write(Nickname, true);
                w.Write(Level);
                w.Write(Shiny);
                w.Write(Gender);
                w.Write(HP);
                w.Write(MaxHP);
                w.Write(HPPercentage);
                w.Write(Status1);
                w.Write(FieldPosition);
            }
        }

        public PBETrainer Trainer { get; }
        public ReadOnlyCollection<PBESwitchInInfo> SwitchIns { get; }
        IReadOnlyList<IPBEPkmnSwitchInInfo> IPBEPkmnSwitchInPacket.SwitchIns => SwitchIns;
        public bool Forced { get; }
        public PBEBattlePokemon ForcedByPokemon { get; }

        internal PBEPkmnSwitchInPacket(PBETrainer trainer, IList<PBESwitchInInfo> switchIns, PBEBattlePokemon forcedByPokemon = null)
        {
            using (var ms = new MemoryStream())
            using (var w = new EndianBinaryWriter(ms, encoding: EncodingType.UTF16))
            {
                w.Write(Code);
                w.Write((Trainer = trainer).Id);
                sbyte count = (sbyte)(SwitchIns = new ReadOnlyCollection<PBESwitchInInfo>(switchIns)).Count;
                w.Write(count);
                for (int i = 0; i < count; i++)
                {
                    SwitchIns[i].ToBytes(w);
                }
                w.Write(Forced = forcedByPokemon != null);
                if (Forced)
                {
                    (ForcedByPokemon = forcedByPokemon).ToBytes_Position(w);
                }
                Data = new ReadOnlyCollection<byte>(ms.ToArray());
            }
        }
        internal PBEPkmnSwitchInPacket(byte[] data, EndianBinaryReader r, PBEBattle battle)
        {
            Data = new ReadOnlyCollection<byte>(data);
            Trainer = battle.Trainers[r.ReadByte()];
            var switches = new PBESwitchInInfo[r.ReadSByte()];
            for (int i = 0; i < switches.Length; i++)
            {
                switches[i] = new PBESwitchInInfo(r, battle);
            }
            SwitchIns = new ReadOnlyCollection<PBESwitchInInfo>(switches);
            Forced = r.ReadBoolean();
            if (Forced)
            {
                ForcedByPokemon = battle.GetPokemon_Position(r);
            }
        }
    }
    public sealed class PBEPkmnSwitchInPacket_Hidden : IPBEPkmnSwitchInPacket
    {
        public const ushort Code = 0x36;
        public ReadOnlyCollection<byte> Data { get; }

        public sealed class PBESwitchInInfo : IPBEPkmnSwitchInInfo
        {
            public PBESpecies Species { get; }
            public PBEForm Form { get; }
            public string Nickname { get; }
            public byte Level { get; }
            public bool Shiny { get; }
            public PBEGender Gender { get; }
            public double HPPercentage { get; }
            public PBEStatus1 Status1 { get; }
            public PBEFieldPosition FieldPosition { get; }

            internal PBESwitchInInfo(PBEPkmnSwitchInPacket.PBESwitchInInfo other)
            {
                Species = other.Species;
                Form = other.Form;
                Nickname = other.Nickname;
                Level = other.Level;
                Shiny = other.Shiny;
                Gender = other.Gender;
                HPPercentage = other.HPPercentage;
                Status1 = other.Status1;
                FieldPosition = other.FieldPosition;
            }
            internal PBESwitchInInfo(EndianBinaryReader r)
            {
                Species = r.ReadEnum<PBESpecies>();
                Form = r.ReadEnum<PBEForm>();
                Nickname = r.ReadStringNullTerminated();
                Level = r.ReadByte();
                Shiny = r.ReadBoolean();
                Gender = r.ReadEnum<PBEGender>();
                HPPercentage = r.ReadDouble();
                Status1 = r.ReadEnum<PBEStatus1>();
                FieldPosition = r.ReadEnum<PBEFieldPosition>();
            }

            internal void ToBytes(EndianBinaryWriter w)
            {
                w.Write(Species);
                w.Write(Form);
                w.Write(Nickname, true);
                w.Write(Level);
                w.Write(Shiny);
                w.Write(Gender);
                w.Write(HPPercentage);
                w.Write(Status1);
                w.Write(FieldPosition);
            }
        }

        public PBETrainer Trainer { get; }
        public ReadOnlyCollection<PBESwitchInInfo> SwitchIns { get; }
        IReadOnlyList<IPBEPkmnSwitchInInfo> IPBEPkmnSwitchInPacket.SwitchIns => SwitchIns;
        public bool Forced { get; }
        public PBEBattlePokemon ForcedByPokemon { get; }

        public PBEPkmnSwitchInPacket_Hidden(PBEPkmnSwitchInPacket other)
        {
            if (other == null)
            {
                throw new ArgumentNullException(nameof(other));
            }
            using (var ms = new MemoryStream())
            using (var w = new EndianBinaryWriter(ms, encoding: EncodingType.UTF16))
            {
                w.Write(Code);
                w.Write((Trainer = other.Trainer).Id);
                var switchIns = new PBESwitchInInfo[other.SwitchIns.Count];
                for (int i = 0; i < switchIns.Length; i++)
                {
                    switchIns[i] = new PBESwitchInInfo(other.SwitchIns[i]);
                }
                sbyte count = (sbyte)(SwitchIns = new ReadOnlyCollection<PBESwitchInInfo>(switchIns)).Count;
                w.Write(count);
                for (int i = 0; i < count; i++)
                {
                    SwitchIns[i].ToBytes(w);
                }
                w.Write(Forced = other.Forced);
                if (Forced)
                {
                    (ForcedByPokemon = other.ForcedByPokemon).ToBytes_Position(w);
                }
                Data = new ReadOnlyCollection<byte>(ms.ToArray());
            }
        }
        internal PBEPkmnSwitchInPacket_Hidden(byte[] data, EndianBinaryReader r, PBEBattle battle)
        {
            Data = new ReadOnlyCollection<byte>(data);
            Trainer = battle.Trainers[r.ReadByte()];
            var switches = new PBESwitchInInfo[r.ReadSByte()];
            for (int i = 0; i < switches.Length; i++)
            {
                switches[i] = new PBESwitchInInfo(r);
            }
            SwitchIns = new ReadOnlyCollection<PBESwitchInInfo>(switches);
            Forced = r.ReadBoolean();
            if (Forced)
            {
                ForcedByPokemon = battle.GetPokemon_Position(r);
            }
        }
    }
}
