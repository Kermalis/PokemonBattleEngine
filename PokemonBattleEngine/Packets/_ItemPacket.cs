using Kermalis.EndianBinaryIO;
using Kermalis.PokemonBattleEngine.Battle;
using Kermalis.PokemonBattleEngine.Data;
using System.Collections.ObjectModel;
using System.IO;

namespace Kermalis.PokemonBattleEngine.Packets;

public sealed class PBEItemPacket : IPBEPacket
{
	public const ushort ID = 0x16;
	public ReadOnlyCollection<byte> Data { get; }

	public PBETrainer ItemHolderTrainer { get; }
	public PBEFieldPosition ItemHolder { get; }
	public PBETrainer Pokemon2Trainer { get; }
	public PBEFieldPosition Pokemon2 { get; }
	public PBEItem Item { get; }
	public PBEItemAction ItemAction { get; }

	internal PBEItemPacket(PBEBattlePokemon itemHolder, PBEBattlePokemon pokemon2, PBEItem item, PBEItemAction itemAction)
	{
		using (var ms = new MemoryStream())
		{
			EndianBinaryWriter w = PBEPacketProcessor.WritePacketID(ms, ID);

			w.WriteByte((ItemHolderTrainer = itemHolder.Trainer).Id);
			w.WriteEnum(ItemHolder = itemHolder.FieldPosition);
			w.WriteByte((Pokemon2Trainer = pokemon2.Trainer).Id);
			w.WriteEnum(Pokemon2 = pokemon2.FieldPosition);
			w.WriteEnum(Item = item);
			w.WriteEnum(ItemAction = itemAction);

			Data = new ReadOnlyCollection<byte>(ms.ToArray());
		}
	}
	internal PBEItemPacket(byte[] data, EndianBinaryReader r, PBEBattle battle)
	{
		Data = new ReadOnlyCollection<byte>(data);

		ItemHolderTrainer = battle.Trainers[r.ReadByte()];
		ItemHolder = r.ReadEnum<PBEFieldPosition>();
		Pokemon2Trainer = battle.Trainers[r.ReadByte()];
		Pokemon2 = r.ReadEnum<PBEFieldPosition>();
		Item = r.ReadEnum<PBEItem>();
		ItemAction = r.ReadEnum<PBEItemAction>();
	}
}
