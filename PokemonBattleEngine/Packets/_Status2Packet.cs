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

        public PBEBattlePokemon Status2Receiver { get; }
        public PBEBattlePokemon Pokemon2 { get; }
        public PBEStatus2 Status2 { get; }
        public PBEStatusAction StatusAction { get; }

        internal PBEStatus2Packet(PBEBattlePokemon status1Receiver, PBEBattlePokemon pokemon2, PBEStatus2 status2, PBEStatusAction statusAction)
        {
            using (var ms = new MemoryStream())
            using (var w = new EndianBinaryWriter(ms, encoding: EncodingType.UTF16))
            {
                w.Write(Code);
                (Status2Receiver = status1Receiver).ToBytes_Position(w);
                (Pokemon2 = pokemon2).ToBytes_Position(w);
                w.Write(Status2 = status2);
                w.Write(StatusAction = statusAction);
                Data = new ReadOnlyCollection<byte>(ms.ToArray());
            }
        }
        internal PBEStatus2Packet(byte[] data, EndianBinaryReader r, PBEBattle battle)
        {
            Data = new ReadOnlyCollection<byte>(data);
            Status2Receiver = battle.GetPokemon_Position(r);
            Pokemon2 = battle.GetPokemon_Position(r);
            Status2 = r.ReadEnum<PBEStatus2>();
            StatusAction = r.ReadEnum<PBEStatusAction>();
        }
    }
}
