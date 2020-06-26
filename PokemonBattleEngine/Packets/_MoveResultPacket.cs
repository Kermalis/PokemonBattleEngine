using Kermalis.EndianBinaryIO;
using Kermalis.PokemonBattleEngine.Battle;
using Kermalis.PokemonBattleEngine.Data;
using System.Collections.ObjectModel;
using System.IO;

namespace Kermalis.PokemonBattleEngine.Packets
{
    public sealed class PBEMoveResultPacket : IPBEPacket
    {
        public const ushort Code = 0x15;
        public ReadOnlyCollection<byte> Data { get; }

        public PBEBattlePokemon MoveUser { get; }
        public PBEBattlePokemon Pokemon2 { get; }
        public PBEResult Result { get; }

        internal PBEMoveResultPacket(PBEBattlePokemon moveUser, PBEBattlePokemon pokemon2, PBEResult result)
        {
            using (var ms = new MemoryStream())
            using (var w = new EndianBinaryWriter(ms, encoding: EncodingType.UTF16))
            {
                w.Write(Code);
                (MoveUser = moveUser).ToBytes_Position(w);
                (Pokemon2 = pokemon2).ToBytes_Position(w);
                w.Write(Result = result);
                Data = new ReadOnlyCollection<byte>(ms.ToArray());
            }
        }
        internal PBEMoveResultPacket(byte[] data, EndianBinaryReader r, PBEBattle battle)
        {
            Data = new ReadOnlyCollection<byte>(data);
            MoveUser = battle.GetPokemon_Position(r);
            Pokemon2 = battle.GetPokemon_Position(r);
            Result = r.ReadEnum<PBEResult>();
        }
    }
}
