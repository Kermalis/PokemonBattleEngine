using Kermalis.EndianBinaryIO;
using Kermalis.PokemonBattleEngine.Battle;
using Kermalis.PokemonBattleEngine.Data;
using System.Collections.ObjectModel;
using System.IO;

namespace Kermalis.PokemonBattleEngine.Packets
{
    public sealed class PBEStatus2Packet : IPBEPacket
    {
        public const ushort Code = 0x12;
        public ReadOnlyCollection<byte> Data { get; }

        public PBEFieldPosition Status2Receiver { get; }
        public PBETeam Status2ReceiverTeam { get; }
        public PBEFieldPosition Pokemon2 { get; }
        public PBETeam Pokemon2Team { get; }
        public PBEStatus2 Status2 { get; }
        public PBEStatusAction StatusAction { get; }

        internal PBEStatus2Packet(PBEPokemon status2Receiver, PBEPokemon pokemon2, PBEStatus2 status2, PBEStatusAction statusAction)
        {
            using (var ms = new MemoryStream())
            using (var w = new EndianBinaryWriter(ms, encoding: EncodingType.UTF16))
            {
                w.Write(Code);
                w.Write(Status2Receiver = status2Receiver.FieldPosition);
                w.Write((Status2ReceiverTeam = status2Receiver.Team).Id);
                w.Write(Pokemon2 = pokemon2.FieldPosition);
                w.Write((Pokemon2Team = pokemon2.Team).Id);
                w.Write(Status2 = status2);
                w.Write(StatusAction = statusAction);
                Data = new ReadOnlyCollection<byte>(ms.ToArray());
            }
        }
        internal PBEStatus2Packet(byte[] data, EndianBinaryReader r, PBEBattle battle)
        {
            Data = new ReadOnlyCollection<byte>(data);
            Status2Receiver = r.ReadEnum<PBEFieldPosition>();
            Status2ReceiverTeam = battle.Teams[r.ReadByte()];
            Pokemon2 = r.ReadEnum<PBEFieldPosition>();
            Pokemon2Team = battle.Teams[r.ReadByte()];
            Status2 = r.ReadEnum<PBEStatus2>();
            StatusAction = r.ReadEnum<PBEStatusAction>();
        }
    }
}
