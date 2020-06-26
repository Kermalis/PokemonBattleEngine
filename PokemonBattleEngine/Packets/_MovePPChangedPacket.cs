using Kermalis.EndianBinaryIO;
using Kermalis.PokemonBattleEngine.Battle;
using Kermalis.PokemonBattleEngine.Data;
using System.Collections.ObjectModel;
using System.IO;

namespace Kermalis.PokemonBattleEngine.Packets
{
    public sealed class PBEMovePPChangedPacket : IPBEPacket
    {
        public const ushort Code = 0x17;
        public ReadOnlyCollection<byte> Data { get; }

        public PBEBattlePokemon MoveUser { get; }
        public PBEMove Move { get; }
        public int AmountReduced { get; }

        internal PBEMovePPChangedPacket(PBEBattlePokemon moveUser, PBEMove move, int amountReduced)
        {
            using (var ms = new MemoryStream())
            using (var w = new EndianBinaryWriter(ms, encoding: EncodingType.UTF16))
            {
                w.Write(Code);
                (MoveUser = moveUser).ToBytes_Position(w);
                w.Write(Move = move);
                w.Write(AmountReduced = amountReduced);
                Data = new ReadOnlyCollection<byte>(ms.ToArray());
            }
        }
        internal PBEMovePPChangedPacket(byte[] data, EndianBinaryReader r, PBEBattle battle)
        {
            Data = new ReadOnlyCollection<byte>(data);
            MoveUser = battle.GetPokemon_Position(r);
            Move = r.ReadEnum<PBEMove>();
            AmountReduced = r.ReadInt32();
        }
    }
}
