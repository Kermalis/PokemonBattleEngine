using Kermalis.EndianBinaryIO;
using Kermalis.PokemonBattleEngine.Battle;
using Kermalis.PokemonBattleEngine.Data;
using System.Collections.ObjectModel;
using System.IO;

namespace Kermalis.PokemonBattleEngine.Packets;

public sealed class PBEAbilityPacket : IPBEPacket
{
	public const ushort ID = 0x19;
	public ReadOnlyCollection<byte> Data { get; }

	public PBETrainer AbilityOwnerTrainer { get; }
	public PBEFieldPosition AbilityOwner { get; }
	public PBETrainer Pokemon2Trainer { get; }
	public PBEFieldPosition Pokemon2 { get; }
	public PBEAbility Ability { get; }
	public PBEAbilityAction AbilityAction { get; }

	internal PBEAbilityPacket(PBEBattlePokemon abilityOwner, PBEBattlePokemon pokemon2, PBEAbility ability, PBEAbilityAction abilityAction)
	{
		using (var ms = new MemoryStream())
		{
			EndianBinaryWriter w = PBEPacketProcessor.WritePacketID(ms, ID);

			w.WriteByte((AbilityOwnerTrainer = abilityOwner.Trainer).Id);
			w.WriteEnum(AbilityOwner = abilityOwner.FieldPosition);
			w.WriteByte((Pokemon2Trainer = pokemon2.Trainer).Id);
			w.WriteEnum(Pokemon2 = pokemon2.FieldPosition);
			w.WriteEnum(Ability = ability);
			w.WriteEnum(AbilityAction = abilityAction);

			Data = new ReadOnlyCollection<byte>(ms.ToArray());
		}
	}
	internal PBEAbilityPacket(byte[] data, EndianBinaryReader r, PBEBattle battle)
	{
		Data = new ReadOnlyCollection<byte>(data);

		AbilityOwnerTrainer = battle.Trainers[r.ReadByte()];
		AbilityOwner = r.ReadEnum<PBEFieldPosition>();
		Pokemon2Trainer = battle.Trainers[r.ReadByte()];
		Pokemon2 = r.ReadEnum<PBEFieldPosition>();
		Ability = r.ReadEnum<PBEAbility>();
		AbilityAction = r.ReadEnum<PBEAbilityAction>();
	}
}
