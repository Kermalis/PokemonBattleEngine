using Kermalis.EndianBinaryIO;
using System.Collections.Generic;
using System.Text.Json;

namespace Kermalis.PokemonBattleEngine.Data;

public interface IPBEMovesetSlot
{
	PBEMove Move { get; }
	byte PPUps { get; }
}
public interface IPBEPartyMovesetSlot : IPBEMovesetSlot
{
	int PP { get; }
}
public interface IPBEMoveset<T> : IReadOnlyList<T> where T : IPBEMovesetSlot
{
	//
}
public interface IPBEMoveset : IPBEMoveset<IPBEMovesetSlot>
{
	//
}
public interface IPBEPartyMoveset<T> : IReadOnlyList<T> where T : IPBEPartyMovesetSlot
{
	//
}
public interface IPBEPartyMoveset : IPBEPartyMoveset<IPBEPartyMovesetSlot>
{
	//
}

public static class PBEMovesetInterfaceExtensions
{
	public static int CountMoves(this IPBEMoveset moves)
	{
		int num = 0;
		for (int i = 0; i < moves.Count; i++)
		{
			if (moves[i].Move != PBEMove.None)
			{
				num++;
			}
		}
		return num;
	}

	internal static void ToBytes(this IPBEMoveset moveset, EndianBinaryWriter w)
	{
		byte count = (byte)moveset.Count;
		w.WriteByte(count);
		for (int i = 0; i < count; i++)
		{
			IPBEMovesetSlot slot = moveset[i];
			w.WriteEnum(slot.Move);
			w.WriteByte(slot.PPUps);
		}
	}
	internal static void ToBytes(this IPBEPartyMoveset moveset, EndianBinaryWriter w)
	{
		byte count = (byte)moveset.Count;
		w.WriteByte(count);
		for (int i = 0; i < count; i++)
		{
			IPBEPartyMovesetSlot slot = moveset[i];
			w.WriteEnum(slot.Move);
			w.WriteInt32(slot.PP);
			w.WriteByte(slot.PPUps);
		}
	}
	internal static void ToJson(this IPBEMoveset moveset, Utf8JsonWriter w)
	{
		w.WriteStartArray();
		for (int i = 0; i < moveset.Count; i++)
		{
			IPBEMovesetSlot slot = moveset[i];
			w.WriteStartObject();
			w.WriteString(nameof(IPBEMovesetSlot.Move), slot.Move.ToString());
			w.WriteNumber(nameof(IPBEMovesetSlot.PPUps), slot.PPUps);
			w.WriteEndObject();
		}
		w.WriteEndArray();
	}
	internal static void ToJson(this IPBEPartyMoveset moveset, Utf8JsonWriter w)
	{
		w.WriteStartArray();
		for (int i = 0; i < moveset.Count; i++)
		{
			IPBEPartyMovesetSlot slot = moveset[i];
			w.WriteStartObject();
			w.WriteString(nameof(IPBEPartyMovesetSlot.Move), slot.Move.ToString());
			w.WriteNumber(nameof(IPBEPartyMovesetSlot.PP), slot.PP);
			w.WriteNumber(nameof(IPBEPartyMovesetSlot.PPUps), slot.PPUps);
			w.WriteEndObject();
		}
		w.WriteEndArray();
	}
}
