using Ether.Network.Packets;
using Kermalis.PokemonBattleEngine.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Kermalis.PokemonBattleEngine.Network
{
    public sealed class PPkmnSwitchInPacket : INetPacketStream
    {
        public const int Code = 0x6;
        public byte[] Buffer => BuildBuffer();

        public Guid PokemonId;
        public bool LocallyOwned;
        public PSpecies Species;
        public byte Level;
        public ushort HP, MaxHP;
        public PGender Gender;

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
                r.ReadInt32(); // Skip Code
                PokemonId = new Guid(r.ReadBytes(0x10));
                LocallyOwned = r.ReadByte() != 0;
                Species = (PSpecies)r.ReadUInt16();
                Level = r.ReadByte();
                HP = r.ReadUInt16();
                MaxHP = r.ReadUInt16();
                Gender = (PGender)r.ReadByte();
            }
        }
        byte[] BuildBuffer()
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
            return BitConverter.GetBytes(bytes.Count).Concat(bytes).ToArray();
        }

        public int Size => throw new NotImplementedException();
        public long Position => throw new NotImplementedException();
        public T Read<T>() => throw new NotImplementedException();
        public T[] ReadArray<T>(int amount) => throw new NotImplementedException();
        public void Write<T>(T value) => throw new NotImplementedException();
        public void Dispose() { }
    }
}
