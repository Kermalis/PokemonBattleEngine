using Kermalis.EndianBinaryIO;
using Kermalis.PokemonBattleEngine.Battle;
using System.Collections.ObjectModel;
using System.IO;

namespace Kermalis.PokemonBattleEngine.Packets
{
    public sealed class PBEMoveMissedPacket : IPBEPacket
    {
        public const ushort Code = 0x0D;
        public ReadOnlyCollection<byte> Data { get; }

        public PBEBattlePokemon MoveUser { get; }
        public PBEBattlePokemon Pokemon2 { get; }

        internal PBEMoveMissedPacket(PBEBattlePokemon moveUser, PBEBattlePokemon pokemon2)
        {
            using (var ms = new MemoryStream())
            using (var w = new EndianBinaryWriter(ms, encoding: EncodingType.UTF16))
            {
                w.Write(Code);
                (MoveUser = moveUser).ToBytes_Position(w);
                (Pokemon2 = pokemon2).ToBytes_Position(w);
                Data = new ReadOnlyCollection<byte>(ms.ToArray());
            }
        }
        internal PBEMoveMissedPacket(byte[] data, EndianBinaryReader r, PBEBattle battle)
        {
            Data = new ReadOnlyCollection<byte>(data);
            MoveUser = battle.GetPokemon_Position(r);
            Pokemon2 = battle.GetPokemon_Position(r);
        }
    }
}
