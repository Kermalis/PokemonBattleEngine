using Ether.Network.Packets;
using Kermalis.PokemonBattleEngine.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Kermalis.PokemonBattleEngine.Packets
{
    public sealed class PBEActionsRequestPacket : INetPacket
    {
        public const short Code = 0x07;
        public IEnumerable<byte> Buffer => BuildBuffer();

        public bool LocalTeam { get; set; }
        public byte[] PokemonIDs { get; }

        public PBEActionsRequestPacket(bool localTeam, IEnumerable<PBEPokemon> pkmn)
        {
            LocalTeam = localTeam;
            PokemonIDs = pkmn.Select(p => p.Id).ToArray();
        }
        public PBEActionsRequestPacket(byte[] buffer)
        {
            using (var r = new BinaryReader(new MemoryStream(buffer)))
            {
                r.ReadInt16(); // Skip Code
                LocalTeam = r.ReadBoolean();
                var numPkmn = Math.Min((byte)3, r.ReadByte());
                PokemonIDs = new byte[numPkmn];
                for (int i = 0; i < numPkmn; i++)
                {
                    PokemonIDs[i] = r.ReadByte();
                }
            }
        }
        IEnumerable<byte> BuildBuffer()
        {
            var bytes = new List<byte>();
            bytes.AddRange(BitConverter.GetBytes(Code));
            bytes.Add((byte)(LocalTeam ? 1 : 0));
            var numPkmn = Math.Min(3, PokemonIDs.Length);
            bytes.Add((byte)numPkmn);
            for (int i = 0; i < numPkmn; i++)
            {
                bytes.Add(PokemonIDs[i]);
            }
            return BitConverter.GetBytes((short)bytes.Count).Concat(bytes);
        }

        public void Dispose() { }
    }
}
