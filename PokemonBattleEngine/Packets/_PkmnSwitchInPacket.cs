using Ether.Network.Packets;
using Kermalis.PokemonBattleEngine.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Kermalis.PokemonBattleEngine.Packets
{
    public sealed class PPkmnSwitchInPacket : INetPacket
    {
        public const short Code = 0x06;
        public IEnumerable<byte> Buffer => BuildBuffer();

        public readonly Guid PokemonId;
        public bool Local;
        public readonly PSpecies Species;
        public readonly string Nickname;
        public readonly byte Level;
        public readonly bool Shiny;
        public readonly ushort HP, MaxHP;
        public readonly PGender Gender;
        public readonly PFieldPosition FieldPosition;

        public PPkmnSwitchInPacket(PPokemon pkmn)
        {
            PokemonId = pkmn.Id;
            Local = pkmn.Local;
            Species = pkmn.Shell.Species;
            Nickname = pkmn.Shell.Nickname;
            Level = pkmn.Shell.Level;
            Shiny = pkmn.Shell.Shiny;
            HP = pkmn.HP;
            MaxHP = pkmn.MaxHP;
            Gender = pkmn.Shell.Gender;
            FieldPosition = pkmn.FieldPosition;
        }
        public PPkmnSwitchInPacket(byte[] buffer)
        {
            using (var r = new BinaryReader(new MemoryStream(buffer)))
            {
                r.ReadInt16(); // Skip Code
                PokemonId = new Guid(r.ReadBytes(0x10));
                Local = r.ReadBoolean();
                Species = (PSpecies)r.ReadUInt32();
                Nickname = PUtils.StringFromBytes(r);
                Level = r.ReadByte();
                Shiny = r.ReadBoolean();
                HP = r.ReadUInt16();
                MaxHP = r.ReadUInt16();
                Gender = (PGender)r.ReadByte();
                FieldPosition = (PFieldPosition)r.ReadByte();
            }
        }
        IEnumerable<byte> BuildBuffer()
        {
            var bytes = new List<byte>();
            bytes.AddRange(BitConverter.GetBytes(Code));
            bytes.AddRange(PokemonId.ToByteArray());
            bytes.Add((byte)(Local ? 1 : 0));
            bytes.AddRange(BitConverter.GetBytes((uint)Species));
            bytes.AddRange(PUtils.StringToBytes(Nickname));
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
