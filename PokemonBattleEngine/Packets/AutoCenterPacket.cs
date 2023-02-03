using Kermalis.EndianBinaryIO;
using Kermalis.PokemonBattleEngine.Battle;
using System.Collections.ObjectModel;
using System.IO;

namespace Kermalis.PokemonBattleEngine.Packets;

public interface IPBEAutoCenterPacket : IPBEPacket
{
	PBETrainer Pokemon0Trainer { get; }
	PBEFieldPosition Pokemon0OldPosition { get; }
	PBETrainer Pokemon1Trainer { get; }
	PBEFieldPosition Pokemon1OldPosition { get; }
}
public interface IPBEAutoCenterPacket_0 : IPBEAutoCenterPacket
{
	byte Pokemon0 { get; }
}
public interface IPBEAutoCenterPacket_1 : IPBEAutoCenterPacket
{
	byte Pokemon1 { get; }
}
public sealed class PBEAutoCenterPacket : IPBEAutoCenterPacket_0, IPBEAutoCenterPacket_1
{
	public const ushort ID = 0x2A;
	public ReadOnlyCollection<byte> Data { get; }

	public PBETrainer Pokemon0Trainer { get; }
	public byte Pokemon0 { get; }
	public PBEFieldPosition Pokemon0OldPosition { get; }
	public PBETrainer Pokemon1Trainer { get; }
	public byte Pokemon1 { get; }
	public PBEFieldPosition Pokemon1OldPosition { get; }

	internal PBEAutoCenterPacket(PBEBattlePokemon pokemon0, PBEFieldPosition pokemon0OldPosition, PBEBattlePokemon pokemon1, PBEFieldPosition pokemon1OldPosition)
	{
		using (var ms = new MemoryStream())
		{
			EndianBinaryWriter w = PBEPacketProcessor.WritePacketID(ms, ID);

			w.WriteByte((Pokemon0Trainer = pokemon0.Trainer).Id);
			w.WriteByte(Pokemon0 = pokemon0.Id);
			w.WriteEnum(Pokemon0OldPosition = pokemon0OldPosition);
			w.WriteByte((Pokemon1Trainer = pokemon1.Trainer).Id);
			w.WriteByte(Pokemon1 = pokemon1.Id);
			w.WriteEnum(Pokemon1OldPosition = pokemon1OldPosition);

			Data = new ReadOnlyCollection<byte>(ms.ToArray());
		}
	}
	internal PBEAutoCenterPacket(byte[] data, EndianBinaryReader r, PBEBattle battle)
	{
		Data = new ReadOnlyCollection<byte>(data);

		Pokemon0Trainer = battle.Trainers[r.ReadByte()];
		Pokemon0 = r.ReadByte();
		Pokemon0OldPosition = r.ReadEnum<PBEFieldPosition>();
		Pokemon1Trainer = battle.Trainers[r.ReadByte()];
		Pokemon1 = r.ReadByte();
		Pokemon1OldPosition = r.ReadEnum<PBEFieldPosition>();
	}
}
public sealed class PBEAutoCenterPacket_Hidden0 : IPBEAutoCenterPacket_1
{
	public const ushort ID = 0x30;
	public ReadOnlyCollection<byte> Data { get; }

	public PBETrainer Pokemon0Trainer { get; }
	public PBEFieldPosition Pokemon0OldPosition { get; }
	public PBETrainer Pokemon1Trainer { get; }
	public byte Pokemon1 { get; }
	public PBEFieldPosition Pokemon1OldPosition { get; }

	public PBEAutoCenterPacket_Hidden0(PBEAutoCenterPacket other)
	{
		using (var ms = new MemoryStream())
		{
			EndianBinaryWriter w = PBEPacketProcessor.WritePacketID(ms, ID);

			w.WriteByte((Pokemon0Trainer = other.Pokemon0Trainer).Id);
			w.WriteEnum(Pokemon0OldPosition = other.Pokemon0OldPosition);
			w.WriteByte((Pokemon1Trainer = other.Pokemon1Trainer).Id);
			w.WriteByte(Pokemon1 = other.Pokemon1);
			w.WriteEnum(Pokemon1OldPosition = other.Pokemon1OldPosition);

			Data = new ReadOnlyCollection<byte>(ms.ToArray());
		}
	}
	internal PBEAutoCenterPacket_Hidden0(byte[] data, EndianBinaryReader r, PBEBattle battle)
	{
		Data = new ReadOnlyCollection<byte>(data);

		Pokemon0Trainer = battle.Trainers[r.ReadByte()];
		Pokemon0OldPosition = r.ReadEnum<PBEFieldPosition>();
		Pokemon1Trainer = battle.Trainers[r.ReadByte()];
		Pokemon1 = r.ReadByte();
		Pokemon1OldPosition = r.ReadEnum<PBEFieldPosition>();
	}
}
public sealed class PBEAutoCenterPacket_Hidden1 : IPBEAutoCenterPacket_0
{
	public const ushort ID = 0x31;
	public ReadOnlyCollection<byte> Data { get; }

	public PBETrainer Pokemon0Trainer { get; }
	public byte Pokemon0 { get; }
	public PBEFieldPosition Pokemon0OldPosition { get; }
	public PBETrainer Pokemon1Trainer { get; }
	public PBEFieldPosition Pokemon1OldPosition { get; }

	public PBEAutoCenterPacket_Hidden1(PBEAutoCenterPacket other)
	{
		using (var ms = new MemoryStream())
		{
			EndianBinaryWriter w = PBEPacketProcessor.WritePacketID(ms, ID);

			w.WriteByte((Pokemon0Trainer = other.Pokemon0Trainer).Id);
			w.WriteByte(Pokemon0 = other.Pokemon0);
			w.WriteEnum(Pokemon0OldPosition = other.Pokemon0OldPosition);
			w.WriteByte((Pokemon1Trainer = other.Pokemon1Trainer).Id);
			w.WriteEnum(Pokemon1OldPosition = other.Pokemon1OldPosition);

			Data = new ReadOnlyCollection<byte>(ms.ToArray());
		}
	}
	internal PBEAutoCenterPacket_Hidden1(byte[] data, EndianBinaryReader r, PBEBattle battle)
	{
		Data = new ReadOnlyCollection<byte>(data);

		Pokemon0Trainer = battle.Trainers[r.ReadByte()];
		Pokemon0 = r.ReadByte();
		Pokemon0OldPosition = r.ReadEnum<PBEFieldPosition>();
		Pokemon1Trainer = battle.Trainers[r.ReadByte()];
		Pokemon1OldPosition = r.ReadEnum<PBEFieldPosition>();
	}
}
public sealed class PBEAutoCenterPacket_Hidden01 : IPBEAutoCenterPacket
{
	public const ushort ID = 0x32;
	public ReadOnlyCollection<byte> Data { get; }

	public PBETrainer Pokemon0Trainer { get; }
	public PBEFieldPosition Pokemon0OldPosition { get; }
	public PBETrainer Pokemon1Trainer { get; }
	public PBEFieldPosition Pokemon1OldPosition { get; }

	public PBEAutoCenterPacket_Hidden01(PBEAutoCenterPacket other)
	{
		using (var ms = new MemoryStream())
		{
			EndianBinaryWriter w = PBEPacketProcessor.WritePacketID(ms, ID);

			w.WriteByte((Pokemon0Trainer = other.Pokemon0Trainer).Id);
			w.WriteEnum(Pokemon0OldPosition = other.Pokemon0OldPosition);
			w.WriteByte((Pokemon1Trainer = other.Pokemon1Trainer).Id);
			w.WriteEnum(Pokemon1OldPosition = other.Pokemon1OldPosition);

			Data = new ReadOnlyCollection<byte>(ms.ToArray());
		}
	}
	internal PBEAutoCenterPacket_Hidden01(byte[] data, EndianBinaryReader r, PBEBattle battle)
	{
		Data = new ReadOnlyCollection<byte>(data);

		Pokemon0Trainer = battle.Trainers[r.ReadByte()];
		Pokemon0OldPosition = r.ReadEnum<PBEFieldPosition>();
		Pokemon1Trainer = battle.Trainers[r.ReadByte()];
		Pokemon1OldPosition = r.ReadEnum<PBEFieldPosition>();
	}
}
