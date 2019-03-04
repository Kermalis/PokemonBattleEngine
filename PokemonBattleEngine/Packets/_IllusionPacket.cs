using Ether.Network.Packets;
using Kermalis.PokemonBattleEngine.Battle;
using Kermalis.PokemonBattleEngine.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Kermalis.PokemonBattleEngine.Packets
{
    public sealed class PBEIllusionPacket : INetPacket
    {
        public const short Code = 0x25;
        public IEnumerable<byte> Buffer { get; }

        public PBEFieldPosition Pokemon { get; }
        public PBETeam PokemonTeam { get; }
        public PBEGender ActualGender { get; }
        public bool ActualShiny { get; }
        public string ActualNickname { get; }
        public PBESpecies ActualSpecies { get; }
        public PBEType ActualType1 { get; }
        public PBEType ActualType2 { get; }
        public double ActualWeight { get; }

        public PBEIllusionPacket(PBEPokemon pokemon)
        {
            var bytes = new List<byte>();
            bytes.AddRange(BitConverter.GetBytes(Code));
            bytes.Add((byte)(Pokemon = pokemon.FieldPosition));
            bytes.Add((PokemonTeam = pokemon.Team).Id);
            bytes.Add((byte)(ActualGender = pokemon.Gender));
            bytes.AddRange(PBEUtils.StringToBytes(ActualNickname = pokemon.Nickname));
            bytes.Add((byte)((ActualShiny = pokemon.Shiny) ? 1 : 0));
            bytes.AddRange(BitConverter.GetBytes((ushort)(ActualSpecies = pokemon.Species)));
            bytes.Add((byte)(ActualType1 = pokemon.Type1));
            bytes.Add((byte)(ActualType2 = pokemon.Type2));
            bytes.AddRange(BitConverter.GetBytes(ActualWeight = pokemon.Weight));
            Buffer = BitConverter.GetBytes((short)bytes.Count).Concat(bytes);
        }
        public PBEIllusionPacket(byte[] buffer, PBEBattle battle)
        {
            Buffer = buffer;
            using (var r = new BinaryReader(new MemoryStream(buffer)))
            {
                r.ReadInt16(); // Skip Code
                Pokemon = (PBEFieldPosition)r.ReadByte();
                PokemonTeam = battle.Teams[r.ReadByte()];
                ActualGender = (PBEGender)r.ReadByte();
                ActualNickname = PBEUtils.StringFromBytes(r);
                ActualShiny = r.ReadBoolean();
                ActualSpecies = (PBESpecies)r.ReadUInt16();
                ActualType1 = (PBEType)r.ReadByte();
                ActualType2 = (PBEType)r.ReadByte();
                ActualWeight = r.ReadDouble();
            }
        }

        public void Dispose() { }
    }
}
