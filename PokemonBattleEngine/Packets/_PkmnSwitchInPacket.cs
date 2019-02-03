using Ether.Network.Packets;
using Kermalis.PokemonBattleEngine.Battle;
using Kermalis.PokemonBattleEngine.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

namespace Kermalis.PokemonBattleEngine.Packets
{
    public sealed class PBEPkmnSwitchInPacket : INetPacket
    {
        public const short Code = 0x06;
        public IEnumerable<byte> Buffer { get; }

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

            public PBESwitchInInfo(byte pkmnId, byte disguisedAsId, PBESpecies species, string nickname, byte level, bool shiny, PBEGender gender, ushort hp, ushort maxHP, double hpPercentage, PBEStatus1 status1, PBEFieldPosition fieldPosition)
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

            internal byte[] ToBytes()
            {
                var bytes = new List<byte>();
                bytes.Add(PokemonId);
                bytes.Add(DisguisedAsId);
                bytes.AddRange(BitConverter.GetBytes((uint)Species));
                bytes.AddRange(PBEUtils.StringToBytes(Nickname));
                bytes.Add(Level);
                bytes.Add((byte)(Shiny ? 1 : 0));
                bytes.Add((byte)Gender);
                bytes.AddRange(BitConverter.GetBytes(HP));
                bytes.AddRange(BitConverter.GetBytes(MaxHP));
                bytes.AddRange(BitConverter.GetBytes(HPPercentage));
                bytes.Add((byte)Status1);
                bytes.Add((byte)FieldPosition);
                return bytes.ToArray();
            }
            internal static PBESwitchInInfo FromBytes(BinaryReader r)
            {
                return new PBESwitchInInfo(r.ReadByte(), r.ReadByte(), (PBESpecies)r.ReadUInt32(), PBEUtils.StringFromBytes(r), r.ReadByte(), r.ReadBoolean(), (PBEGender)r.ReadByte(), r.ReadUInt16(), r.ReadUInt16(), r.ReadDouble(), (PBEStatus1)r.ReadByte(), (PBEFieldPosition)r.ReadByte());
            }
        }

        public PBETeam Team { get; }
        public ReadOnlyCollection<PBESwitchInInfo> SwitchIns { get; }
        public bool Forced { get; }

        public PBEPkmnSwitchInPacket(PBETeam team, IEnumerable<PBESwitchInInfo> switchIns, bool forced)
        {
            var bytes = new List<byte>();
            bytes.AddRange(BitConverter.GetBytes(Code));
            bytes.Add((Team = team).Id);
            bytes.Add((byte)(SwitchIns = switchIns.ToList().AsReadOnly()).Count);
            foreach (PBESwitchInInfo info in SwitchIns)
            {
                bytes.AddRange(info.ToBytes());
            }
            bytes.Add((byte)((Forced = forced) ? 1 : 0));
            Buffer = BitConverter.GetBytes((short)bytes.Count).Concat(bytes);
        }
        public PBEPkmnSwitchInPacket(byte[] buffer, PBEBattle battle)
        {
            using (var r = new BinaryReader(new MemoryStream(buffer)))
            {
                r.ReadInt16(); // Skip Code
                Team = battle.Teams[r.ReadByte()];
                var switches = new PBESwitchInInfo[r.ReadByte()];
                for (int i = 0; i < switches.Length; i++)
                {
                    switches[i] = PBESwitchInInfo.FromBytes(r);
                }
                SwitchIns = Array.AsReadOnly(switches);
                Forced = r.ReadBoolean();
            }
        }

        public void Dispose() { }
    }
}
