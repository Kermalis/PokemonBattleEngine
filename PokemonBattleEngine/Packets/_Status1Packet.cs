using Ether.Network.Packets;
using Kermalis.PokemonBattleEngine.Battle;
using Kermalis.PokemonBattleEngine.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;

namespace Kermalis.PokemonBattleEngine.Packets
{
    public sealed class PBEStatus1Packet : INetPacket
    {
        public const short Code = 0x11;
        public ReadOnlyCollection<byte> Buffer { get; }

        public PBEFieldPosition Pokemon2 { get; }
        public PBETeam Pokemon2Team { get; }
        public PBEFieldPosition Status1Receiver { get; }
        public PBETeam Status1ReceiverTeam { get; }
        public PBEStatus1 Status1 { get; }
        public PBEStatusAction StatusAction { get; }

        internal PBEStatus1Packet(PBEPokemon status1Receiver, PBEPokemon pokemon2, PBEStatus1 status1, PBEStatusAction statusAction)
        {
            var bytes = new List<byte>();
            bytes.AddRange(BitConverter.GetBytes(Code));
            bytes.Add((byte)(Status1Receiver = status1Receiver.FieldPosition));
            bytes.Add((Status1ReceiverTeam = status1Receiver.Team).Id);
            bytes.Add((byte)(Pokemon2 = pokemon2.FieldPosition));
            bytes.Add((Pokemon2Team = pokemon2.Team).Id);
            bytes.Add((byte)(Status1 = status1));
            bytes.Add((byte)(StatusAction = statusAction));
            bytes.InsertRange(0, BitConverter.GetBytes((short)bytes.Count));
            Buffer = new ReadOnlyCollection<byte>(bytes);
        }
        internal PBEStatus1Packet(ReadOnlyCollection<byte> buffer, BinaryReader r, PBEBattle battle)
        {
            Buffer = buffer;
            Status1Receiver = (PBEFieldPosition)r.ReadByte();
            Status1ReceiverTeam = battle.Teams[r.ReadByte()];
            Pokemon2 = (PBEFieldPosition)r.ReadByte();
            Pokemon2Team = battle.Teams[r.ReadByte()];
            Status1 = (PBEStatus1)r.ReadByte();
            StatusAction = (PBEStatusAction)r.ReadByte();
        }

        public void Dispose() { }
    }
}
