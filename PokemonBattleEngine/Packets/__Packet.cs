using System.Collections.ObjectModel;

namespace Kermalis.PokemonBattleEngine.Packets;

public interface IPBEPacket
{
	ReadOnlyCollection<byte> Data { get; }
}
