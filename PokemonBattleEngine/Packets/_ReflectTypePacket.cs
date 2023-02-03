using Kermalis.EndianBinaryIO;
using Kermalis.PokemonBattleEngine.Battle;
using Kermalis.PokemonBattleEngine.Data;
using System.Collections.ObjectModel;
using System.IO;

namespace Kermalis.PokemonBattleEngine.Packets;

public sealed class PBEReflectTypePacket : IPBEPacket
{
	public const ushort ID = 0x2E;
	public ReadOnlyCollection<byte> Data { get; }

	public PBETrainer UserTrainer { get; }
	public PBEFieldPosition User { get; }
	public PBETrainer TargetTrainer { get; }
	public PBEFieldPosition Target { get; }
	public PBEType Type1 { get; }
	public PBEType Type2 { get; }

	internal PBEReflectTypePacket(PBEBattlePokemon user, PBEBattlePokemon target)
	{
		using (var ms = new MemoryStream())
		{
			EndianBinaryWriter w = PBEPacketProcessor.WritePacketID(ms, ID);

			w.WriteByte((UserTrainer = user.Trainer).Id);
			w.WriteEnum(User = user.FieldPosition);
			w.WriteByte((TargetTrainer = target.Trainer).Id);
			w.WriteEnum(Target = target.FieldPosition);
			w.WriteEnum(Type1 = target.Type1);
			w.WriteEnum(Type2 = target.Type2);

			Data = new ReadOnlyCollection<byte>(ms.ToArray());
		}
	}
	internal PBEReflectTypePacket(byte[] data, EndianBinaryReader r, PBEBattle battle)
	{
		Data = new ReadOnlyCollection<byte>(data);

		UserTrainer = battle.Trainers[r.ReadByte()];
		User = r.ReadEnum<PBEFieldPosition>();
		TargetTrainer = battle.Trainers[r.ReadByte()];
		Target = r.ReadEnum<PBEFieldPosition>();
		Type1 = r.ReadEnum<PBEType>();
		Type2 = r.ReadEnum<PBEType>();
	}
}
public sealed class PBEReflectTypePacket_Hidden : IPBEPacket
{
	public const ushort ID = 0x33;
	public ReadOnlyCollection<byte> Data { get; }

	public PBETrainer UserTrainer { get; }
	public PBEFieldPosition User { get; }
	public PBETrainer TargetTrainer { get; }
	public PBEFieldPosition Target { get; }

	public PBEReflectTypePacket_Hidden(PBEReflectTypePacket other)
	{
		using (var ms = new MemoryStream())
		{
			EndianBinaryWriter w = PBEPacketProcessor.WritePacketID(ms, ID);

			w.WriteByte((UserTrainer = other.UserTrainer).Id);
			w.WriteEnum(User = other.User);
			w.WriteByte((TargetTrainer = other.TargetTrainer).Id);
			w.WriteEnum(Target = other.Target);

			Data = new ReadOnlyCollection<byte>(ms.ToArray());
		}
	}
	internal PBEReflectTypePacket_Hidden(byte[] data, EndianBinaryReader r, PBEBattle battle)
	{
		Data = new ReadOnlyCollection<byte>(data);

		UserTrainer = battle.Trainers[r.ReadByte()];
		User = r.ReadEnum<PBEFieldPosition>();
		TargetTrainer = battle.Trainers[r.ReadByte()];
		Target = r.ReadEnum<PBEFieldPosition>();
	}
}
