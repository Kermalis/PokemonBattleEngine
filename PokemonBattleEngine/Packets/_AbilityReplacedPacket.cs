using Kermalis.EndianBinaryIO;
using Kermalis.PokemonBattleEngine.Battle;
using Kermalis.PokemonBattleEngine.Data;
using System.Collections.ObjectModel;
using System.IO;

namespace Kermalis.PokemonBattleEngine.Packets;

public sealed class PBEAbilityReplacedPacket : IPBEPacket
{
	public const ushort ID = 0x2C;
	public ReadOnlyCollection<byte> Data { get; }

	public PBETrainer AbilityOwnerTrainer { get; }
	public PBEFieldPosition AbilityOwner { get; }
	public PBEAbility? OldAbility { get; }
	public PBEAbility NewAbility { get; }

	internal PBEAbilityReplacedPacket(PBEBattlePokemon abilityOwner, PBEAbility? oldAbility, PBEAbility newAbility)
	{
		using (var ms = new MemoryStream())
		{
			EndianBinaryWriter w = PBEPacketProcessor.WritePacketID(ms, ID);

			w.WriteByte((AbilityOwnerTrainer = abilityOwner.Trainer).Id);
			w.WriteEnum(AbilityOwner = abilityOwner.FieldPosition);
			w.WriteBoolean(oldAbility is not null);
			if (oldAbility is not null)
			{
				w.WriteEnum((OldAbility = oldAbility).Value);
			}
			w.WriteEnum(NewAbility = newAbility);

			Data = new ReadOnlyCollection<byte>(ms.ToArray());
		}
	}
	internal PBEAbilityReplacedPacket(byte[] data, EndianBinaryReader r, PBEBattle battle)
	{
		Data = new ReadOnlyCollection<byte>(data);

		AbilityOwnerTrainer = battle.Trainers[r.ReadByte()];
		AbilityOwner = r.ReadEnum<PBEFieldPosition>();
		if (r.ReadBoolean())
		{
			OldAbility = r.ReadEnum<PBEAbility>();
		}
		NewAbility = r.ReadEnum<PBEAbility>();
	}
}
