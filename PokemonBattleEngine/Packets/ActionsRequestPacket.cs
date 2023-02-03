using Kermalis.EndianBinaryIO;
using Kermalis.PokemonBattleEngine.Battle;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

namespace Kermalis.PokemonBattleEngine.Packets;

public sealed class PBEActionsRequestPacket : IPBEPacket
{
	public const ushort ID = 0x07;
	public ReadOnlyCollection<byte> Data { get; }

	public PBETrainer Trainer { get; }
	public ReadOnlyCollection<PBEFieldPosition> Pokemon { get; }

	internal PBEActionsRequestPacket(PBETrainer trainer)
	{
		using (var ms = new MemoryStream())
		{
			EndianBinaryWriter w = PBEPacketProcessor.WritePacketID(ms, ID);

			w.WriteByte((Trainer = trainer).Id);
			byte count = (byte)(Pokemon = new ReadOnlyCollection<PBEFieldPosition>(trainer.ActionsRequired.Select(p => p.FieldPosition).ToArray())).Count;
			w.WriteByte(count);
			for (int i = 0; i < count; i++)
			{
				w.WriteEnum(Pokemon[i]);
			}

			Data = new ReadOnlyCollection<byte>(ms.ToArray());
		}
	}
	internal PBEActionsRequestPacket(byte[] data, EndianBinaryReader r, PBEBattle battle)
	{
		Data = new ReadOnlyCollection<byte>(data);

		Trainer = battle.Trainers[r.ReadByte()];
		var pkmn = new PBEFieldPosition[r.ReadByte()];
		for (int i = 0; i < pkmn.Length; i++)
		{
			pkmn[i] = r.ReadEnum<PBEFieldPosition>();
		}
		Pokemon = new ReadOnlyCollection<PBEFieldPosition>(pkmn);
	}
}
