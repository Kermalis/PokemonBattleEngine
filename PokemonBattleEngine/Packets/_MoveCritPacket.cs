using Kermalis.EndianBinaryIO;
using Kermalis.PokemonBattleEngine.Battle;
using System.Collections.ObjectModel;
using System.IO;

namespace Kermalis.PokemonBattleEngine.Packets
{
    public sealed class PBEMoveCritPacket : IPBEPacket
    {
        public const ushort Code = 0x0F;
        public ReadOnlyCollection<byte> Data { get; }

        public PBEBattlePokemon Victim { get; }

        internal PBEMoveCritPacket(PBEBattlePokemon victim)
        {
            using (var ms = new MemoryStream())
            using (var w = new EndianBinaryWriter(ms, encoding: EncodingType.UTF16))
            {
                w.Write(Code);
                (Victim = victim).ToBytes_Position(w);
                Data = new ReadOnlyCollection<byte>(ms.ToArray());
            }
        }
        internal PBEMoveCritPacket(byte[] data, EndianBinaryReader r, PBEBattle battle)
        {
            Data = new ReadOnlyCollection<byte>(data);
            Victim = battle.GetPokemon_Position(r);
        }
    }
}
