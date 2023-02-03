using Kermalis.EndianBinaryIO;
using Kermalis.PokemonBattleEngine.Battle;
using System.Collections.ObjectModel;
using System.IO;

namespace Kermalis.PokemonBattleEngine.Packets;

public sealed class PBETeamStatusDamagePacket : IPBEPacket
{
	public const ushort ID = 0x41;
	public ReadOnlyCollection<byte> Data { get; }

	public PBETeam Team { get; }
	public PBETeamStatus TeamStatus { get; }
	public PBETrainer DamageVictimTrainer { get; }
	public PBEFieldPosition DamageVictim { get; }

	internal PBETeamStatusDamagePacket(PBETeam team, PBETeamStatus teamStatus, PBEBattlePokemon damageVictim)
	{
		using (var ms = new MemoryStream())
		{
			EndianBinaryWriter w = PBEPacketProcessor.WritePacketID(ms, ID);

			w.WriteByte((Team = team).Id);
			w.WriteEnum(TeamStatus = teamStatus);
			w.WriteByte((DamageVictimTrainer = damageVictim.Trainer).Id);
			w.WriteEnum(DamageVictim = damageVictim.FieldPosition);

			Data = new ReadOnlyCollection<byte>(ms.ToArray());
		}
	}
	internal PBETeamStatusDamagePacket(byte[] data, EndianBinaryReader r, PBEBattle battle)
	{
		Data = new ReadOnlyCollection<byte>(data);

		Team = battle.Teams[r.ReadByte()];
		TeamStatus = r.ReadEnum<PBETeamStatus>();
		DamageVictimTrainer = battle.Trainers[r.ReadByte()];
		DamageVictim = r.ReadEnum<PBEFieldPosition>();
	}
}
