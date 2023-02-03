using Kermalis.EndianBinaryIO;
using Kermalis.PokemonBattleEngine.Battle;
using System.Collections.ObjectModel;
using System.IO;

namespace Kermalis.PokemonBattleEngine.Packets;

public sealed class PBEPsychUpPacket : IPBEPacket
{
	public const ushort ID = 0x22;
	public ReadOnlyCollection<byte> Data { get; }

	public PBETrainer UserTrainer { get; }
	public PBEFieldPosition User { get; }
	public PBETrainer TargetTrainer { get; }
	public PBEFieldPosition Target { get; }
	public sbyte AttackChange { get; }
	public sbyte DefenseChange { get; }
	public sbyte SpAttackChange { get; }
	public sbyte SpDefenseChange { get; }
	public sbyte SpeedChange { get; }
	public sbyte AccuracyChange { get; }
	public sbyte EvasionChange { get; }

	internal PBEPsychUpPacket(PBEBattlePokemon user, PBEBattlePokemon target)
	{
		using (var ms = new MemoryStream())
		{
			EndianBinaryWriter w = PBEPacketProcessor.WritePacketID(ms, ID);

			w.WriteByte((UserTrainer = user.Trainer).Id);
			w.WriteEnum(User = user.FieldPosition);
			w.WriteByte((TargetTrainer = target.Trainer).Id);
			w.WriteEnum(Target = target.FieldPosition);
			w.WriteSByte(AttackChange = target.AttackChange);
			w.WriteSByte(DefenseChange = target.DefenseChange);
			w.WriteSByte(SpAttackChange = target.SpAttackChange);
			w.WriteSByte(SpDefenseChange = target.SpDefenseChange);
			w.WriteSByte(SpeedChange = target.SpeedChange);
			w.WriteSByte(AccuracyChange = target.AccuracyChange);
			w.WriteSByte(EvasionChange = target.EvasionChange);

			Data = new ReadOnlyCollection<byte>(ms.ToArray());
		}
	}
	internal PBEPsychUpPacket(byte[] data, EndianBinaryReader r, PBEBattle battle)
	{
		Data = new ReadOnlyCollection<byte>(data);

		UserTrainer = battle.Trainers[r.ReadByte()];
		User = r.ReadEnum<PBEFieldPosition>();
		TargetTrainer = battle.Trainers[r.ReadByte()];
		Target = r.ReadEnum<PBEFieldPosition>();
		AttackChange = r.ReadSByte();
		DefenseChange = r.ReadSByte();
		SpAttackChange = r.ReadSByte();
		SpDefenseChange = r.ReadSByte();
		SpeedChange = r.ReadSByte();
		AccuracyChange = r.ReadSByte();
		EvasionChange = r.ReadSByte();
	}
}
