using Kermalis.EndianBinaryIO;
using Kermalis.PokemonBattleEngine.Battle;
using Kermalis.PokemonBattleEngine.Data;
using System.Collections.ObjectModel;
using System.IO;

namespace Kermalis.PokemonBattleEngine.Packets;

public sealed class PBETransformPacket : IPBEPacket
{
	public const ushort ID = 0x18;
	public ReadOnlyCollection<byte> Data { get; }

	public PBETrainer UserTrainer { get; }
	public PBEFieldPosition User { get; }
	public PBETrainer TargetTrainer { get; }
	public PBEFieldPosition Target { get; }
	public ushort TargetAttack { get; }
	public ushort TargetDefense { get; }
	public ushort TargetSpAttack { get; }
	public ushort TargetSpDefense { get; }
	public ushort TargetSpeed { get; }
	public sbyte TargetAttackChange { get; }
	public sbyte TargetDefenseChange { get; }
	public sbyte TargetSpAttackChange { get; }
	public sbyte TargetSpDefenseChange { get; }
	public sbyte TargetSpeedChange { get; }
	public sbyte TargetAccuracyChange { get; }
	public sbyte TargetEvasionChange { get; }
	public PBEAbility TargetAbility { get; }
	public PBESpecies TargetSpecies { get; }
	public PBEForm TargetForm { get; }
	public PBEType TargetType1 { get; }
	public PBEType TargetType2 { get; }
	public float TargetWeight { get; }
	public ReadOnlyCollection<PBEMove> TargetMoves { get; }

	internal PBETransformPacket(PBEBattlePokemon user, PBEBattlePokemon target)
	{
		using (var ms = new MemoryStream())
		{
			EndianBinaryWriter w = PBEPacketProcessor.WritePacketID(ms, ID);

			w.WriteByte((UserTrainer = user.Trainer).Id);
			w.WriteEnum(User = user.FieldPosition);
			w.WriteByte((TargetTrainer = target.Trainer).Id);
			w.WriteEnum(Target = target.FieldPosition);
			w.WriteUInt16(TargetAttack = target.Attack);
			w.WriteUInt16(TargetDefense = target.Defense);
			w.WriteUInt16(TargetSpAttack = target.SpAttack);
			w.WriteUInt16(TargetSpDefense = target.SpDefense);
			w.WriteUInt16(TargetSpeed = target.Speed);
			w.WriteSByte(TargetAttackChange = target.AttackChange);
			w.WriteSByte(TargetDefenseChange = target.DefenseChange);
			w.WriteSByte(TargetSpAttackChange = target.SpAttackChange);
			w.WriteSByte(TargetSpDefenseChange = target.SpDefenseChange);
			w.WriteSByte(TargetSpeedChange = target.SpeedChange);
			w.WriteSByte(TargetAccuracyChange = target.AccuracyChange);
			w.WriteSByte(TargetEvasionChange = target.EvasionChange);
			w.WriteEnum(TargetAbility = target.Ability);
			w.WriteEnum(TargetSpecies = target.Species);
			w.WriteEnum(TargetForm = target.Form);
			w.WriteEnum(TargetType1 = target.Type1);
			w.WriteEnum(TargetType2 = target.Type2);
			w.WriteSingle(TargetWeight = target.Weight);
			TargetMoves = target.Moves.ForTransformPacket();
			for (int i = 0; i < TargetMoves.Count; i++)
			{
				w.WriteEnum(TargetMoves[i]);
			}

			Data = new ReadOnlyCollection<byte>(ms.ToArray());
		}
	}
	internal PBETransformPacket(byte[] data, EndianBinaryReader r, PBEBattle battle)
	{
		Data = new ReadOnlyCollection<byte>(data);

		UserTrainer = battle.Trainers[r.ReadByte()];
		User = r.ReadEnum<PBEFieldPosition>();
		TargetTrainer = battle.Trainers[r.ReadByte()];
		Target = r.ReadEnum<PBEFieldPosition>();
		TargetAttack = r.ReadUInt16();
		TargetDefense = r.ReadUInt16();
		TargetSpAttack = r.ReadUInt16();
		TargetSpDefense = r.ReadUInt16();
		TargetSpeed = r.ReadUInt16();
		TargetAttackChange = r.ReadSByte();
		TargetDefenseChange = r.ReadSByte();
		TargetSpAttackChange = r.ReadSByte();
		TargetSpDefenseChange = r.ReadSByte();
		TargetSpeedChange = r.ReadSByte();
		TargetAccuracyChange = r.ReadSByte();
		TargetEvasionChange = r.ReadSByte();
		TargetAbility = r.ReadEnum<PBEAbility>();
		TargetSpecies = r.ReadEnum<PBESpecies>();
		TargetForm = r.ReadEnum<PBEForm>();
		TargetType1 = r.ReadEnum<PBEType>();
		TargetType2 = r.ReadEnum<PBEType>();
		TargetWeight = r.ReadSingle();
		var moves = new PBEMove[battle.Settings.NumMoves];
		for (int i = 0; i < moves.Length; i++)
		{
			moves[i] = r.ReadEnum<PBEMove>();
		}
		TargetMoves = new ReadOnlyCollection<PBEMove>(moves);
	}
}
