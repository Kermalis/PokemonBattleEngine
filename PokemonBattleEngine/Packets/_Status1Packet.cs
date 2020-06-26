using Kermalis.EndianBinaryIO;
using Kermalis.PokemonBattleEngine.Battle;
using Kermalis.PokemonBattleEngine.Data;
using System.Collections.ObjectModel;
using System.IO;

namespace Kermalis.PokemonBattleEngine.Packets
{
    public sealed class PBEStatus1Packet : IPBEPacket
    {
        public const ushort Code = 0x11;
        public ReadOnlyCollection<byte> Data { get; }

        public PBEBattlePokemon Status1Receiver { get; }
        public PBEBattlePokemon Pokemon2 { get; }
        public PBEStatus1 Status1 { get; }
        public PBEStatusAction StatusAction { get; }

        internal PBEStatus1Packet(PBEBattlePokemon status1Receiver, PBEBattlePokemon pokemon2, PBEStatus1 status1, PBEStatusAction statusAction)
        {
            using (var ms = new MemoryStream())
            using (var w = new EndianBinaryWriter(ms, encoding: EncodingType.UTF16))
            {
                w.Write(Code);
                (Status1Receiver = status1Receiver).ToBytes_Position(w);
                (Pokemon2 = pokemon2).ToBytes_Position(w);
                w.Write(Status1 = status1);
                w.Write(StatusAction = statusAction);
                Data = new ReadOnlyCollection<byte>(ms.ToArray());
            }
        }
        internal PBEStatus1Packet(byte[] data, EndianBinaryReader r, PBEBattle battle)
        {
            Data = new ReadOnlyCollection<byte>(data);
            Status1Receiver = battle.GetPokemon_Position(r);
            Pokemon2 = battle.GetPokemon_Position(r);
            Status1 = r.ReadEnum<PBEStatus1>();
            StatusAction = r.ReadEnum<PBEStatusAction>();
        }
    }
}
