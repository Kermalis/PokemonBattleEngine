using Ether.Network.Packets;

namespace Kermalis.PokemonBattleEngine.Packets
{
    public sealed class ReadyUpPacket : NetPacketStream
    {
        public const int Code = 0x0;

        public ReadyUpPacket() : base()
        {
            Write(4); // Size
            Write(Code);
        }
        public ReadyUpPacket(byte[] buffer) : base(buffer) { }
    }
}
