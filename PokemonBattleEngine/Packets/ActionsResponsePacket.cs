using Kermalis.EndianBinaryIO;
using Kermalis.PokemonBattleEngine.Battle;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;

namespace Kermalis.PokemonBattleEngine.Packets;

public sealed class PBEActionsResponsePacket : IPBEPacket
{
	public const ushort ID = 0x08;
	public ReadOnlyCollection<byte> Data { get; }

	public ReadOnlyCollection<PBETurnAction> Actions { get; }

	public PBEActionsResponsePacket(IList<PBETurnAction> actions)
	{
		if (actions.Count == 0)
		{
			throw new ArgumentOutOfRangeException(nameof(actions));
		}
		using (var ms = new MemoryStream())
		{
			EndianBinaryWriter w = PBEPacketProcessor.WritePacketID(ms, ID);

			byte count = (byte)(Actions = new ReadOnlyCollection<PBETurnAction>(actions)).Count;
			w.WriteByte(count);
			for (int i = 0; i < count; i++)
			{
				Actions[i].ToBytes(w);
			}

			Data = new ReadOnlyCollection<byte>(ms.ToArray());
		}
	}
	internal PBEActionsResponsePacket(byte[] data, EndianBinaryReader r)
	{
		Data = new ReadOnlyCollection<byte>(data);

		var actions = new PBETurnAction[r.ReadByte()];
		for (int i = 0; i < actions.Length; i++)
		{
			actions[i] = new PBETurnAction(r);
		}
		Actions = new ReadOnlyCollection<PBETurnAction>(actions);
	}
}
