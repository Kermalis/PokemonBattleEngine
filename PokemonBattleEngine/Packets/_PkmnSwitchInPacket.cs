using Kermalis.EndianBinaryIO;
using Kermalis.PokemonBattleEngine.Battle;
using Kermalis.PokemonBattleEngine.Data;
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
            public string Nickname { get; }
            public byte Level { get; }
            public bool Shiny { get; }
            public PBEGender Gender { get; }
            public ushort HP { get; }
            public ushort MaxHP { get; }
            public double HPPercentage { get; }
            public PBEStatus1 Status1 { get; }
            public PBEFieldPosition FieldPosition { get; }

            internal PBESwitchInInfo(byte pkmnId, byte disguisedAsId, PBESpecies species, string nickname, byte level, bool shiny, PBEGender gender, ushort hp, ushort maxHP, double hpPercentage, PBEStatus1 status1, PBEFieldPosition fieldPosition)
            {
                PokemonId = pkmnId;
                DisguisedAsId = disguisedAsId;
                Species = species;
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
            internal PBESwitchInInfo(EndianBinaryReader r)
            {
                PokemonId = r.ReadByte();
                DisguisedAsId = r.ReadByte();
                Species = r.ReadEnum<PBESpecies>();
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

        private PBEPkmnSwitchInPacket(PBETeam team, IList<PBESwitchInInfo> switchIns, bool forced = false, PBEFieldPosition? forcedByPokemonPosition = null, PBETeam forcedByPokemonTeam = null)
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
                w.Write(Forced = forced);
                if (Forced)
                {
                    w.Write((ForcedByPokemonPosition = forcedByPokemonPosition).Value);
                    w.Write((ForcedByPokemonTeam = forcedByPokemonTeam).Id);
                }
                Data = new ReadOnlyCollection<byte>(ms.ToArray());
            }
        }
        internal PBEPkmnSwitchInPacket(PBETeam team, IList<PBESwitchInInfo> switchIns, PBEPokemon forcedByPokemon = null)
            : this(team, switchIns, forcedByPokemon != null, forcedByPokemon?.FieldPosition, forcedByPokemon?.Team) { }
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

        public PBEPkmnSwitchInPacket MakeHidden()
        {
            var hiddenSwitchIns = new PBESwitchInInfo[SwitchIns.Count];
            for (int i = 0; i < hiddenSwitchIns.Length; i++)
            {
                PBESwitchInInfo s = SwitchIns[i];
                hiddenSwitchIns[i] = new PBESwitchInInfo(byte.MaxValue, byte.MaxValue, s.Species, s.Nickname, s.Level, s.Shiny, s.Gender, ushort.MinValue, ushort.MinValue, s.HPPercentage, s.Status1, s.FieldPosition);
            }
            return new PBEPkmnSwitchInPacket(Team, hiddenSwitchIns, Forced, ForcedByPokemonPosition, ForcedByPokemonTeam);
        }
    }
}
