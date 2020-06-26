using Kermalis.EndianBinaryIO;
using Kermalis.PokemonBattleEngine.Battle;
using Kermalis.PokemonBattleEngine.Data;
using System.Collections.ObjectModel;
using System.IO;

namespace Kermalis.PokemonBattleEngine.Packets
{
    public sealed class PBEMoveUsedPacket : IPBEPacket
    {
        public const ushort Code = 0x09;
        public ReadOnlyCollection<byte> Data { get; }

        public PBEBattlePokemon MoveUser { get; }
        public PBEMove Move { get; }
        public bool Reveal { get; }

        internal PBEMoveUsedPacket(PBEBattlePokemon moveUser, PBEMove move, bool reveal)
        {
            using (var ms = new MemoryStream())
            using (var w = new EndianBinaryWriter(ms, encoding: EncodingType.UTF16))
            {
                w.Write(Code);
                (MoveUser = moveUser).ToBytes_Position(w);
                w.Write(Move = move);
                w.Write(Reveal = reveal);
                Data = new ReadOnlyCollection<byte>(ms.ToArray());
            }
        }
        internal PBEMoveUsedPacket(byte[] data, EndianBinaryReader r, PBEBattle battle)
        {
            Data = new ReadOnlyCollection<byte>(data);
            MoveUser = battle.GetPokemon_Position(r);
            Move = r.ReadEnum<PBEMove>();
            Reveal = r.ReadBoolean();
        }
    }
}
