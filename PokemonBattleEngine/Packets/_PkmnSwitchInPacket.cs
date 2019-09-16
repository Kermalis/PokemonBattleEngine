using Ether.Network.Packets;
using Kermalis.PokemonBattleEngine.Battle;
using Kermalis.PokemonBattleEngine.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;

namespace Kermalis.PokemonBattleEngine.Packets
{
    public sealed class PBEPkmnSwitchInPacket : INetPacket
    {
        public const short Code = 0x06;
        public ReadOnlyCollection<byte> Buffer { get; }

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
            internal PBESwitchInInfo(BinaryReader r)
            {
                PokemonId = r.ReadByte();
                DisguisedAsId = r.ReadByte();
                Species = (PBESpecies)r.ReadUInt32();
                Nickname = PBEUtils.StringFromBytes(r);
                Level = r.ReadByte();
                Shiny = r.ReadBoolean();
                Gender = (PBEGender)r.ReadByte();
                HP = r.ReadUInt16();
                MaxHP = r.ReadUInt16();
                HPPercentage = r.ReadDouble();
                Status1 = (PBEStatus1)r.ReadByte();
                FieldPosition = (PBEFieldPosition)r.ReadByte();
            }

            internal void ToBytes(List<byte> bytes)
            {
                bytes.Add(PokemonId);
                bytes.Add(DisguisedAsId);
                bytes.AddRange(BitConverter.GetBytes((uint)Species));
                PBEUtils.StringToBytes(bytes, Nickname);
                bytes.Add(Level);
                bytes.Add((byte)(Shiny ? 1 : 0));
                bytes.Add((byte)Gender);
                bytes.AddRange(BitConverter.GetBytes(HP));
                bytes.AddRange(BitConverter.GetBytes(MaxHP));
                bytes.AddRange(BitConverter.GetBytes(HPPercentage));
                bytes.Add((byte)Status1);
                bytes.Add((byte)FieldPosition);
            }
        }

        public PBETeam Team { get; }
        public ReadOnlyCollection<PBESwitchInInfo> SwitchIns { get; }
        public bool Forced { get; }

        internal PBEPkmnSwitchInPacket(PBETeam team, IList<PBESwitchInInfo> switchIns, bool forced)
        {
            var bytes = new List<byte>();
            bytes.AddRange(BitConverter.GetBytes(Code));
            bytes.Add((Team = team).Id);
            bytes.Add((byte)(SwitchIns = new ReadOnlyCollection<PBESwitchInInfo>(switchIns)).Count);
            for (int i = 0; i < (sbyte)SwitchIns.Count; i++)
            {
                SwitchIns[i].ToBytes(bytes);
            }
            bytes.Add((byte)((Forced = forced) ? 1 : 0));
            bytes.InsertRange(0, BitConverter.GetBytes((short)bytes.Count));
            Buffer = new ReadOnlyCollection<byte>(bytes);
        }
        internal PBEPkmnSwitchInPacket(ReadOnlyCollection<byte> buffer, BinaryReader r, PBEBattle battle)
        {
            Buffer = buffer;
            Team = battle.Teams[r.ReadByte()];
            var switches = new PBESwitchInInfo[r.ReadSByte()];
            for (int i = 0; i < switches.Length; i++)
            {
                switches[i] = new PBESwitchInInfo(r);
            }
            SwitchIns = new ReadOnlyCollection<PBESwitchInInfo>(switches);
            Forced = r.ReadBoolean();
        }

        public PBEPkmnSwitchInPacket MakeHidden()
        {
            var hiddenSwitchIns = new PBESwitchInInfo[SwitchIns.Count];
            for (int i = 0; i < hiddenSwitchIns.Length; i++)
            {
                PBESwitchInInfo s = SwitchIns[i];
                hiddenSwitchIns[i] = new PBESwitchInInfo(byte.MaxValue, byte.MaxValue, s.Species, s.Nickname, s.Level, s.Shiny, s.Gender, ushort.MinValue, ushort.MinValue, s.HPPercentage, s.Status1, s.FieldPosition);
            }
            return new PBEPkmnSwitchInPacket(Team, hiddenSwitchIns, Forced);
        }

        public void Dispose() { }
    }
}
