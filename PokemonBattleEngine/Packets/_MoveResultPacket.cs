using Ether.Network.Packets;
using Kermalis.PokemonBattleEngine.Battle;
using Kermalis.PokemonBattleEngine.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;

namespace Kermalis.PokemonBattleEngine.Packets
{
    public sealed class PBEMoveResultPacket : INetPacket
    {
        public const short Code = 0x15;
        public ReadOnlyCollection<byte> Buffer { get; }

        public PBEFieldPosition MoveUser { get; }
        public PBETeam MoveUserTeam { get; }
        public PBEFieldPosition Pokemon2 { get; }
        public PBETeam Pokemon2Team { get; }
        public PBEResult Result { get; }

        internal PBEMoveResultPacket(PBEPokemon moveUser, PBEPokemon pokemon2, PBEResult result)
        {
            var bytes = new List<byte>();
            bytes.AddRange(BitConverter.GetBytes(Code));
            bytes.Add((byte)(MoveUser = moveUser.FieldPosition));
            bytes.Add((MoveUserTeam = moveUser.Team).Id);
            bytes.Add((byte)(Pokemon2 = pokemon2.FieldPosition));
            bytes.Add((Pokemon2Team = pokemon2.Team).Id);
            bytes.Add((byte)(Result = result));
            bytes.InsertRange(0, BitConverter.GetBytes((short)bytes.Count));
            Buffer = new ReadOnlyCollection<byte>(bytes);
        }
        internal PBEMoveResultPacket(ReadOnlyCollection<byte> buffer, BinaryReader r, PBEBattle battle)
        {
            Buffer = buffer;
            MoveUser = (PBEFieldPosition)r.ReadByte();
            MoveUserTeam = battle.Teams[r.ReadByte()];
            Pokemon2 = (PBEFieldPosition)r.ReadByte();
            Pokemon2Team = battle.Teams[r.ReadByte()];
            Result = (PBEResult)r.ReadByte();
        }

        public void Dispose() { }
    }
}
