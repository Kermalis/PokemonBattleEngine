using Kermalis.EndianBinaryIO;
using Kermalis.PokemonBattleEngine.Battle;
using Kermalis.PokemonBattleEngine.Data;
using System.Collections.ObjectModel;
using System.IO;

namespace Kermalis.PokemonBattleEngine.Packets;

public sealed class PBEItemTurnPacket : IPBEPacket
{
	public const ushort ID = 0x3A;
	public ReadOnlyCollection<byte> Data { get; }

	public PBETrainer ItemUserTrainer { get; }
	public PBEFieldPosition ItemUser { get; }
	public PBEItem Item { get; }
	public PBEItemTurnAction ItemAction { get; }

	internal PBEItemTurnPacket(PBEBattlePokemon itemUserHolder, PBEItem item, PBEItemTurnAction itemAction)
	{
		using (var ms = new MemoryStream())
		{
			EndianBinaryWriter w = PBEPacketProcessor.WritePacketID(ms, ID);

			w.WriteByte((ItemUserTrainer = itemUserHolder.Trainer).Id);
			w.WriteEnum(ItemUser = itemUserHolder.FieldPosition);
			w.WriteEnum(Item = item);
			w.WriteEnum(ItemAction = itemAction);

			Data = new ReadOnlyCollection<byte>(ms.ToArray());
		}
	}
	internal PBEItemTurnPacket(byte[] data, EndianBinaryReader r, PBEBattle battle)
	{
		Data = new ReadOnlyCollection<byte>(data);

		ItemUserTrainer = battle.Trainers[r.ReadByte()];
		ItemUser = r.ReadEnum<PBEFieldPosition>();
		Item = r.ReadEnum<PBEItem>();
		ItemAction = r.ReadEnum<PBEItemTurnAction>();
	}
}
