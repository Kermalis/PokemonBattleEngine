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
        public bool LocallyOwned;
        public readonly PSpecies Species;
        public readonly byte Level;
        public readonly ushort HP, MaxHP;
        public readonly PGender Gender;

        public PPkmnSwitchInPacket(PPokemon pkmn)
        {
            PokemonId = pkmn.Id;
            LocallyOwned = pkmn.LocallyOwned;
            Species = pkmn.Shell.Species;
            Level = pkmn.Shell.Level;
            HP = pkmn.HP;
            MaxHP = pkmn.MaxHP;
            Gender = pkmn.Shell.Gender;
        }
        public PPkmnSwitchInPacket(byte[] buffer)
        {
            using (var r = new BinaryReader(new MemoryStream(buffer)))
            {
                r.ReadInt16(); // Skip Code
                PokemonId = new Guid(r.ReadBytes(0x10));
                LocallyOwned = r.ReadByte() != 0;
                Species = (PSpecies)r.ReadUInt16();
                Level = r.ReadByte();
                HP = r.ReadUInt16();
                MaxHP = r.ReadUInt16();
                Gender = (PGender)r.ReadByte();
            }
        }
        IEnumerable<byte> BuildBuffer()
        {
            var bytes = new List<byte>();
            bytes.AddRange(BitConverter.GetBytes(Code));
            bytes.AddRange(PokemonId.ToByteArray());
            bytes.Add((byte)(LocallyOwned ? 1 : 0));
            bytes.AddRange(BitConverter.GetBytes((ushort)Species));
            bytes.Add(Level);
            bytes.AddRange(BitConverter.GetBytes(HP));
            bytes.AddRange(BitConverter.GetBytes(MaxHP));
            bytes.Add((byte)Gender);
            return BitConverter.GetBytes((short)bytes.Count).Concat(bytes);
        }
        
        public void Dispose() { }
    }
}
