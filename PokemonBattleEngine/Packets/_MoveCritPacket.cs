﻿using Ether.Network.Packets;
using Kermalis.PokemonBattleEngine.Battle;
using Kermalis.PokemonBattleEngine.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;

namespace Kermalis.PokemonBattleEngine.Packets
{
    public sealed class PBEMoveCritPacket : INetPacket
    {
        public const short Code = 0x0F;
        public ReadOnlyCollection<byte> Buffer { get; }

        public PBEFieldPosition Victim { get; }
        public PBETeam VictimTeam { get; }

        internal PBEMoveCritPacket(PBEPokemon victim)
        {
            var bytes = new List<byte>();
            bytes.AddRange(BitConverter.GetBytes(Code));
            bytes.Add((byte)(Victim = victim.FieldPosition));
            bytes.Add((VictimTeam = victim.Team).Id);
            bytes.InsertRange(0, BitConverter.GetBytes((short)bytes.Count));
            Buffer = new ReadOnlyCollection<byte>(bytes);
        }
        internal PBEMoveCritPacket(ReadOnlyCollection<byte> buffer, BinaryReader r, PBEBattle battle)
        {
            Buffer = buffer;
            Victim = (PBEFieldPosition)r.ReadByte();
            VictimTeam = battle.Teams[r.ReadByte()];
        }

        public void Dispose() { }
    }
}
