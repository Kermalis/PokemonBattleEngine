using Kermalis.EndianBinaryIO;
using Kermalis.PokemonBattleEngine.Battle;
using Kermalis.PokemonBattleEngine.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;

namespace Kermalis.PokemonBattleEngine.Packets
{
    public sealed class PBEPkmnSwitchInPacket : IPBEPacket
    {
        public const ushort Code = 0x06;
        public ReadOnlyCollection<byte> Data { get; }

        public sealed class PBESwitchInInfo
        {
            public byte PokemonId { get; }
            public byte DisguisedAsId { get; }
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

            private PBESwitchInInfo(byte pkmnId, byte disguisedAsId, PBESpecies species, PBEForm form, string nickname, byte level, bool shiny, PBEGender gender, ushort hp, ushort maxHP, double hpPercentage, PBEStatus1 status1, PBEFieldPosition fieldPosition)
            {
                PokemonId = pkmnId;
                DisguisedAsId = disguisedAsId;
                Species = species;
                Form = form;
                Nickname = nickname;
                Level = level;
                Shiny = shiny;
                Gender = gender;
                HP = hp;
                MaxHP = maxHP;
                HPPercentage = hpPercentage;
                Status1 = status1;
                FieldPosition = fieldPosition;
            }
            internal PBESwitchInInfo(PBEBattlePokemon pkmn)
                : this(pkmn.Id, pkmn.DisguisedAsPokemon != null ? pkmn.DisguisedAsPokemon.Id : pkmn.Id, pkmn.KnownSpecies, pkmn.KnownForm, pkmn.KnownNickname, pkmn.Level, pkmn.KnownShiny, pkmn.KnownGender, pkmn.HP, pkmn.MaxHP, pkmn.HPPercentage, pkmn.Status1, pkmn.FieldPosition) { }
            internal PBESwitchInInfo(EndianBinaryReader r)
            {
                PokemonId = r.ReadByte();
                DisguisedAsId = r.ReadByte();
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
                w.Write(PokemonId);
                w.Write(DisguisedAsId);
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

        public PBETeam Team { get; }
        public ReadOnlyCollection<PBESwitchInInfo> SwitchIns { get; }
        public bool Forced { get; }
        public PBEFieldPosition? ForcedByPokemonPosition { get; }
        public PBETeam ForcedByPokemonTeam { get; }

        internal PBEPkmnSwitchInPacket(PBETeam team, IList<PBESwitchInInfo> switchIns, PBEBattlePokemon forcedByPokemon = null)
        {
            using (var ms = new MemoryStream())
            using (var w = new EndianBinaryWriter(ms, encoding: EncodingType.UTF16))
            {
                w.Write(Code);
                w.Write((Team = team).Id);
                sbyte count = (sbyte)(SwitchIns = new ReadOnlyCollection<PBESwitchInInfo>(switchIns)).Count;
                w.Write(count);
                for (int i = 0; i < count; i++)
                {
                    SwitchIns[i].ToBytes(w);
                }
                w.Write(Forced = forcedByPokemon != null);
                if (Forced)
                {
                    w.Write((ForcedByPokemonPosition = forcedByPokemon.FieldPosition).Value);
                    w.Write((ForcedByPokemonTeam = forcedByPokemon.Team).Id);
                }
                Data = new ReadOnlyCollection<byte>(ms.ToArray());
            }
        }
        internal PBEPkmnSwitchInPacket(byte[] data, EndianBinaryReader r, PBEBattle battle)
        {
            Data = new ReadOnlyCollection<byte>(data);
            Team = battle.Teams[r.ReadByte()];
            var switches = new PBESwitchInInfo[r.ReadSByte()];
            for (int i = 0; i < switches.Length; i++)
            {
                switches[i] = new PBESwitchInInfo(r);
            }
            SwitchIns = new ReadOnlyCollection<PBESwitchInInfo>(switches);
            Forced = r.ReadBoolean();
            if (Forced)
            {
                ForcedByPokemonPosition = r.ReadEnum<PBEFieldPosition>();
                ForcedByPokemonTeam = battle.Teams[r.ReadByte()];
            }
        }
    }
    public sealed class PBEPkmnSwitchInPacket_Hidden : IPBEPacket
    {
        public const ushort Code = 0x36;
        public ReadOnlyCollection<byte> Data { get; }

        public sealed class PBESwitchInInfo
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

        public PBETeam Team { get; }
        public ReadOnlyCollection<PBESwitchInInfo> SwitchIns { get; }
        public bool Forced { get; }
        public PBEFieldPosition? ForcedByPokemonPosition { get; }
        public PBETeam ForcedByPokemonTeam { get; }

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
                w.Write((Team = other.Team).Id);
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
                    w.Write((ForcedByPokemonPosition = other.ForcedByPokemonPosition).Value);
                    w.Write((ForcedByPokemonTeam = other.ForcedByPokemonTeam).Id);
                }
                Data = new ReadOnlyCollection<byte>(ms.ToArray());
            }
        }
        internal PBEPkmnSwitchInPacket_Hidden(byte[] data, EndianBinaryReader r, PBEBattle battle)
        {
            Data = new ReadOnlyCollection<byte>(data);
            Team = battle.Teams[r.ReadByte()];
            var switches = new PBESwitchInInfo[r.ReadSByte()];
            for (int i = 0; i < switches.Length; i++)
            {
                switches[i] = new PBESwitchInInfo(r);
            }
            SwitchIns = new ReadOnlyCollection<PBESwitchInInfo>(switches);
            Forced = r.ReadBoolean();
            if (Forced)
            {
                ForcedByPokemonPosition = r.ReadEnum<PBEFieldPosition>();
                ForcedByPokemonTeam = battle.Teams[r.ReadByte()];
            }
        }
    }
}
