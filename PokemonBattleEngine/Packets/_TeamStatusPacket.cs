using Kermalis.EndianBinaryIO;
using Kermalis.PokemonBattleEngine.Battle;
using System.Collections.ObjectModel;
using System.IO;

namespace Kermalis.PokemonBattleEngine.Packets;

public sealed class PBETeamStatusPacket : IPBEPacket
{
	public const ushort ID = 0x13;
	public ReadOnlyCollection<byte> Data { get; }

	public PBETeam Team { get; }
	public PBETeamStatus TeamStatus { get; }
	public PBETeamStatusAction TeamStatusAction { get; }

	internal PBETeamStatusPacket(PBETeam team, PBETeamStatus teamStatus, PBETeamStatusAction teamStatusAction)
	{
		using (var ms = new MemoryStream())
		{
			EndianBinaryWriter w = PBEPacketProcessor.WritePacketID(ms, ID);

			w.WriteByte((Team = team).Id);
			w.WriteEnum(TeamStatus = teamStatus);
			w.WriteEnum(TeamStatusAction = teamStatusAction);

			Data = new ReadOnlyCollection<byte>(ms.ToArray());
		}
	}
	internal PBETeamStatusPacket(byte[] data, EndianBinaryReader r, PBEBattle battle)
	{
		Data = new ReadOnlyCollection<byte>(data);

		Team = battle.Teams[r.ReadByte()];
		TeamStatus = r.ReadEnum<PBETeamStatus>();
		TeamStatusAction = r.ReadEnum<PBETeamStatusAction>();
	}
}
