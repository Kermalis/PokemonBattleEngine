using Kermalis.EndianBinaryIO;
using Kermalis.PokemonBattleEngine.Battle;
using Kermalis.PokemonBattleEngine.Data;
using System.Collections.ObjectModel;
using System.IO;

namespace Kermalis.PokemonBattleEngine.Packets;

public sealed class PBEMoveLockPacket : IPBEPacket
{
	public const ushort ID = 0x28;
	public ReadOnlyCollection<byte> Data { get; }

	public PBETrainer MoveUserTrainer { get; }
	public PBEFieldPosition MoveUser { get; }
	public PBEMoveLockType MoveLockType { get; }
	public PBEMove LockedMove { get; }
	public PBETurnTarget? LockedTargets { get; }

	internal PBEMoveLockPacket(PBEBattlePokemon moveUser, PBEMoveLockType moveLockType, PBEMove lockedMove, PBETurnTarget? lockedTargets = null)
	{
		using (var ms = new MemoryStream())
		{
			EndianBinaryWriter w = PBEPacketProcessor.WritePacketID(ms, ID);

			w.WriteByte((MoveUserTrainer = moveUser.Trainer).Id);
			w.WriteEnum(MoveUser = moveUser.FieldPosition);
			w.WriteEnum(MoveLockType = moveLockType);
			w.WriteEnum(LockedMove = lockedMove);
			w.WriteBoolean(lockedTargets is not null);
			if (lockedTargets is not null)
			{
				w.WriteEnum((LockedTargets = lockedTargets).Value);
			}

			Data = new ReadOnlyCollection<byte>(ms.ToArray());
		}
	}
	internal PBEMoveLockPacket(byte[] data, EndianBinaryReader r, PBEBattle battle)
	{
		Data = new ReadOnlyCollection<byte>(data);

		MoveUserTrainer = battle.Trainers[r.ReadByte()];
		MoveUser = r.ReadEnum<PBEFieldPosition>();
		MoveLockType = r.ReadEnum<PBEMoveLockType>();
		LockedMove = r.ReadEnum<PBEMove>();
		if (r.ReadBoolean())
		{
			LockedTargets = r.ReadEnum<PBETurnTarget>();
		}
	}
}
