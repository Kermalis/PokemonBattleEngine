using Kermalis.EndianBinaryIO;
using Kermalis.PokemonBattleEngine.Battle;
using System.Collections.ObjectModel;
using System.IO;

namespace Kermalis.PokemonBattleEngine.Packets;

public sealed class PBEMoveResultPacket : IPBEPacket
{
	public const ushort ID = 0x15;
	public ReadOnlyCollection<byte> Data { get; }

	public PBETrainer MoveUserTrainer { get; }
	public PBEFieldPosition MoveUser { get; }
	public PBETrainer Pokemon2Trainer { get; }
	public PBEFieldPosition Pokemon2 { get; }
	public PBEResult Result { get; }

	internal PBEMoveResultPacket(PBEBattlePokemon moveUser, PBEBattlePokemon pokemon2, PBEResult result)
	{
		using (var ms = new MemoryStream())
		{
			EndianBinaryWriter w = PBEPacketProcessor.WritePacketID(ms, ID);

			w.WriteByte((MoveUserTrainer = moveUser.Trainer).Id);
			w.WriteEnum(MoveUser = moveUser.FieldPosition);
			w.WriteByte((Pokemon2Trainer = pokemon2.Trainer).Id);
			w.WriteEnum(Pokemon2 = pokemon2.FieldPosition);
			w.WriteEnum(Result = result);

			Data = new ReadOnlyCollection<byte>(ms.ToArray());
		}
	}
	internal PBEMoveResultPacket(byte[] data, EndianBinaryReader r, PBEBattle battle)
	{
		Data = new ReadOnlyCollection<byte>(data);

		MoveUserTrainer = battle.Trainers[r.ReadByte()];
		MoveUser = r.ReadEnum<PBEFieldPosition>();
		Pokemon2Trainer = battle.Trainers[r.ReadByte()];
		Pokemon2 = r.ReadEnum<PBEFieldPosition>();
		Result = r.ReadEnum<PBEResult>();
	}
}
