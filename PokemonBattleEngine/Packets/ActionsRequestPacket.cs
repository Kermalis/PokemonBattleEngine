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
    public sealed class PBEActionsRequestPacket : INetPacket
    {
        public const short Code = 0x07;
        public ReadOnlyCollection<byte> Buffer { get; }

        public PBETeam Team { get; }
        public ReadOnlyCollection<PBEFieldPosition> Pokemon { get; }

        internal PBEActionsRequestPacket(PBETeam team)
        {
            var bytes = new List<byte>();
            bytes.AddRange(BitConverter.GetBytes(Code));
            bytes.Add((Team = team).Id);
            bytes.Add((byte)(Pokemon = new ReadOnlyCollection<PBEFieldPosition>(Team.ActionsRequired.Select(p => p.FieldPosition).ToArray())).Count);
            for (int i = 0; i < (byte)Pokemon.Count; i++)
            {
                bytes.Add((byte)Pokemon[i]);
            }
            bytes.InsertRange(0, BitConverter.GetBytes((short)bytes.Count));
            Buffer = new ReadOnlyCollection<byte>(bytes);
        }
        internal PBEActionsRequestPacket(ReadOnlyCollection<byte> buffer, BinaryReader r, PBEBattle battle)
        {
            Buffer = buffer;
            Team = battle.Teams[r.ReadByte()];
            var pkmn = new PBEFieldPosition[r.ReadByte()];
            for (int i = 0; i < pkmn.Length; i++)
            {
                pkmn[i] = (PBEFieldPosition)r.ReadByte();
            }
            Pokemon = new ReadOnlyCollection<PBEFieldPosition>(pkmn);
        }

        public void Dispose() { }
    }
}
