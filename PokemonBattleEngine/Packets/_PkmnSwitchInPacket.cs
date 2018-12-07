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

        public byte PokemonId { get; }
        public bool LocalTeam { get; set; }
        public PBESpecies Species { get; }
        public string Nickname { get; }
        public byte Level { get; }
        public bool Shiny { get; }
        public ushort HP { get; }
        public ushort MaxHP { get; }
        public PBEGender Gender { get; }
        public PBEFieldPosition FieldPosition { get; }

        public PBEPkmnSwitchInPacket(PBEPokemon pkmn)
        {
            PokemonId = pkmn.Id;
            LocalTeam = pkmn.LocalTeam;
            Species = pkmn.Shell.Species;
            Nickname = pkmn.Shell.Nickname;
            Level = pkmn.Shell.Level;
            Shiny = pkmn.Shell.Shiny;
            HP = pkmn.HP;
            MaxHP = pkmn.MaxHP;
            Gender = pkmn.Shell.Gender;
            FieldPosition = pkmn.FieldPosition;
        }
        public PBEPkmnSwitchInPacket(byte[] buffer)
        {
            using (var r = new BinaryReader(new MemoryStream(buffer)))
            {
                r.ReadInt16(); // Skip Code
                PokemonId = r.ReadByte();
                LocalTeam = r.ReadBoolean();
                Species = (PBESpecies)r.ReadUInt32();
                Nickname = PBEUtils.StringFromBytes(r);
                Level = r.ReadByte();
                Shiny = r.ReadBoolean();
                HP = r.ReadUInt16();
                MaxHP = r.ReadUInt16();
                Gender = (PBEGender)r.ReadByte();
                FieldPosition = (PBEFieldPosition)r.ReadByte();
            }
        }
        IEnumerable<byte> BuildBuffer()
        {
            var bytes = new List<byte>();
            bytes.AddRange(BitConverter.GetBytes(Code));
            bytes.Add(PokemonId);
            bytes.Add((byte)(LocalTeam ? 1 : 0));
            bytes.AddRange(BitConverter.GetBytes((uint)Species));
            bytes.AddRange(PBEUtils.StringToBytes(Nickname));
            bytes.Add(Level);
            bytes.Add((byte)(Shiny ? 1 : 0));
            bytes.AddRange(BitConverter.GetBytes(HP));
            bytes.AddRange(BitConverter.GetBytes(MaxHP));
            bytes.Add((byte)Gender);
            bytes.Add((byte)FieldPosition);
            return BitConverter.GetBytes((short)bytes.Count).Concat(bytes);
        }

        public void Dispose() { }
    }
}
