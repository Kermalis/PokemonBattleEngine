using Kermalis.EndianBinaryIO;
using Kermalis.PokemonBattleEngine.Battle;
using Kermalis.PokemonBattleEngine.Data;
using System.Collections.ObjectModel;
using System.IO;

namespace Kermalis.PokemonBattleEngine.Packets;

public sealed class PBEMovePPChangedPacket : IPBEPacket
{
	public const ushort ID = 0x17;
	public ReadOnlyCollection<byte> Data { get; }

	public PBETrainer MoveUserTrainer { get; }
	public PBEFieldPosition MoveUser { get; }
	public PBEMove Move { get; }
	public int AmountReduced { get; }

	internal PBEMovePPChangedPacket(PBEBattlePokemon moveUser, PBEMove move, int amountReduced)
	{
		using (var ms = new MemoryStream())
		{
			EndianBinaryWriter w = PBEPacketProcessor.WritePacketID(ms, ID);

			w.WriteByte((MoveUserTrainer = moveUser.Trainer).Id);
			w.WriteEnum(MoveUser = moveUser.FieldPosition);
			w.WriteEnum(Move = move);
			w.WriteInt32(AmountReduced = amountReduced);

			Data = new ReadOnlyCollection<byte>(ms.ToArray());
		}
	}
	internal PBEMovePPChangedPacket(byte[] data, EndianBinaryReader r, PBEBattle battle)
	{
		Data = new ReadOnlyCollection<byte>(data);

		MoveUserTrainer = battle.Trainers[r.ReadByte()];
		MoveUser = r.ReadEnum<PBEFieldPosition>();
		Move = r.ReadEnum<PBEMove>();
		AmountReduced = r.ReadInt32();
	}
}
