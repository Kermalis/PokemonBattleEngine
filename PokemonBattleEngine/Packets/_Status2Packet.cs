using Kermalis.EndianBinaryIO;
using Kermalis.PokemonBattleEngine.Battle;
using System.Collections.ObjectModel;
using System.IO;

namespace Kermalis.PokemonBattleEngine.Packets;

public sealed class PBEStatus2Packet : IPBEPacket
{
	public const ushort ID = 0x12;
	public ReadOnlyCollection<byte> Data { get; }

	public PBETrainer Status2ReceiverTrainer { get; }
	public PBEFieldPosition Status2Receiver { get; }
	public PBETrainer Pokemon2Trainer { get; }
	public PBEFieldPosition Pokemon2 { get; }
	public PBEStatus2 Status2 { get; }
	public PBEStatusAction StatusAction { get; }

	internal PBEStatus2Packet(PBEBattlePokemon status2Receiver, PBEBattlePokemon pokemon2, PBEStatus2 status2, PBEStatusAction statusAction)
	{
		using (var ms = new MemoryStream())
		{
			EndianBinaryWriter w = PBEPacketProcessor.WritePacketID(ms, ID);

			w.WriteByte((Status2ReceiverTrainer = status2Receiver.Trainer).Id);
			w.WriteEnum(Status2Receiver = status2Receiver.FieldPosition);
			w.WriteByte((Pokemon2Trainer = pokemon2.Trainer).Id);
			w.WriteEnum(Pokemon2 = pokemon2.FieldPosition);
			w.WriteEnum(Status2 = status2);
			w.WriteEnum(StatusAction = statusAction);

			Data = new ReadOnlyCollection<byte>(ms.ToArray());
		}
	}
	internal PBEStatus2Packet(byte[] data, EndianBinaryReader r, PBEBattle battle)
	{
		Data = new ReadOnlyCollection<byte>(data);

		Status2ReceiverTrainer = battle.Trainers[r.ReadByte()];
		Status2Receiver = r.ReadEnum<PBEFieldPosition>();
		Pokemon2Trainer = battle.Trainers[r.ReadByte()];
		Pokemon2 = r.ReadEnum<PBEFieldPosition>();
		Status2 = r.ReadEnum<PBEStatus2>();
		StatusAction = r.ReadEnum<PBEStatusAction>();
	}
}
