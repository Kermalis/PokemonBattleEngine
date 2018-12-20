using Ether.Network.Packets;
using Kermalis.PokemonBattleEngine.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Kermalis.PokemonBattleEngine.Packets
{
    public sealed class PBEPkmnSwitchInPacket : INetPacket
    {
        public const short Code = 0x06;
        public IEnumerable<byte> Buffer => BuildBuffer();

        public class PBESwitchInInfo
        {
            public byte PokemonId { get; }
            public PBESpecies Species { get; }
            public string Nickname { get; }
            public byte Level { get; }
            public bool Shiny { get; }
            public ushort HP { get; }
            public ushort MaxHP { get; }
            public PBEGender Gender { get; }
            public PBEFieldPosition FieldPosition { get; }

            public PBESwitchInInfo(PBEPokemon pkmn)
            {
                PokemonId = pkmn.Id;
                Species = pkmn.Shell.Species;
                Nickname = pkmn.Shell.Nickname;
                Level = pkmn.Shell.Level;
                Shiny = pkmn.Shell.Shiny;
                HP = pkmn.HP;
                MaxHP = pkmn.MaxHP;
                Gender = pkmn.Shell.Gender;
                FieldPosition = pkmn.FieldPosition;
            }
            public PBESwitchInInfo(byte pkmnId, PBESpecies species, string nickname, byte level, bool shiny, ushort hp, ushort maxHP, PBEGender gender, PBEFieldPosition fieldPosition)
            {
                PokemonId = pkmnId;
                Species = species;
                Nickname = nickname;
                Level = level;
                Shiny = shiny;
                HP = hp;
                MaxHP = maxHP;
                Gender = gender;
                FieldPosition = fieldPosition;
            }

            internal byte[] ToBytes()
            {
                var bytes = new List<byte>();
                bytes.Add(PokemonId);
                bytes.AddRange(BitConverter.GetBytes((uint)Species));
                bytes.AddRange(PBEUtils.StringToBytes(Nickname));
                bytes.Add(Level);
                bytes.Add((byte)(Shiny ? 1 : 0));
                bytes.AddRange(BitConverter.GetBytes(HP));
                bytes.AddRange(BitConverter.GetBytes(MaxHP));
                bytes.Add((byte)Gender);
                bytes.Add((byte)FieldPosition);
                return bytes.ToArray();
            }
            internal static PBESwitchInInfo FromBytes(BinaryReader r)
            {
                return new PBESwitchInInfo(r.ReadByte(), (PBESpecies)r.ReadUInt32(), PBEUtils.StringFromBytes(r), r.ReadByte(), r.ReadBoolean(), r.ReadUInt16(), r.ReadUInt16(), (PBEGender)r.ReadByte(), (PBEFieldPosition)r.ReadByte());
            }
        }

        public bool LocalTeam { get; set; }
        public PBESwitchInInfo[] SwitchIns { get; }

        public PBEPkmnSwitchInPacket(bool localTeam, IEnumerable<PBEPokemon> pokemon)
        {
            LocalTeam = localTeam;
            SwitchIns = pokemon.Select(p => new PBESwitchInInfo(p)).ToArray();
        }
        public PBEPkmnSwitchInPacket(byte[] buffer)
        {
            using (var r = new BinaryReader(new MemoryStream(buffer)))
            {
                r.ReadInt16(); // Skip Code
                LocalTeam = r.ReadBoolean();
                var num = Math.Min((byte)3, r.ReadByte());
                SwitchIns = new PBESwitchInInfo[num];
                for (int i = 0; i < num; i++)
                {
                    SwitchIns[i] = PBESwitchInInfo.FromBytes(r);
                }
            }
        }
        IEnumerable<byte> BuildBuffer()
        {
            var bytes = new List<byte>();
            bytes.AddRange(BitConverter.GetBytes(Code));
            bytes.Add((byte)(LocalTeam ? 1 : 0));
            var num = Math.Min(3, SwitchIns.Length);
            bytes.Add((byte)num);
            for (int i = 0; i < num; i++)
            {
                bytes.AddRange(SwitchIns[i].ToBytes());
            }
            return BitConverter.GetBytes((short)bytes.Count).Concat(bytes);
        }

        public void Dispose() { }
    }
}
