using Kermalis.EndianBinaryIO;
using Kermalis.PokemonBattleEngine.Battle;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;

namespace Kermalis.PokemonBattleEngine.Packets;

public sealed class PBESpecialMessagePacket : IPBEPacket
{
	public const ushort ID = 0x20;
	public ReadOnlyCollection<byte> Data { get; }

	public PBESpecialMessage Message { get; }
	public ReadOnlyCollection<object> Params { get; }

	internal PBESpecialMessagePacket(PBESpecialMessage message, params object[] parameters)
	{
		using (var ms = new MemoryStream())
		{
			EndianBinaryWriter w = PBEPacketProcessor.WritePacketID(ms, ID);

			w.WriteEnum(Message = message);
			var par = new List<object>();
			switch (Message)
			{
				case PBESpecialMessage.DraggedOut:
				case PBESpecialMessage.Endure:
				case PBESpecialMessage.HPDrained:
				case PBESpecialMessage.Recoil:
				case PBESpecialMessage.Struggle:
				{
					var p0 = (PBEBattlePokemon)parameters[0];
					par.Add(p0.Trainer);
					par.Add(p0.FieldPosition);
					w.WriteByte(p0.Trainer.Id);
					w.WriteEnum(p0.FieldPosition);
					break;
				}
				case PBESpecialMessage.Magnitude:
				case PBESpecialMessage.MultiHit:
				{
					byte p0 = (byte)parameters[0];
					par.Add(p0);
					w.WriteByte(p0);
					break;
				}
				case PBESpecialMessage.PainSplit:
				{
					var p0 = (PBEBattlePokemon)parameters[0];
					var p1 = (PBEBattlePokemon)parameters[1];
					par.Add(p0.Trainer);
					par.Add(p0.FieldPosition);
					par.Add(p1.Trainer);
					par.Add(p1.FieldPosition);
					w.WriteByte(p1.Trainer.Id);
					w.WriteEnum(p1.FieldPosition);
					w.WriteByte(p1.Trainer.Id);
					w.WriteEnum(p1.FieldPosition);
					break;
				}
			}
			Params = new ReadOnlyCollection<object>(par);

			Data = new ReadOnlyCollection<byte>(ms.ToArray());
		}
	}
	internal PBESpecialMessagePacket(byte[] data, EndianBinaryReader r, PBEBattle battle)
	{
		Data = new ReadOnlyCollection<byte>(data);

		Message = r.ReadEnum<PBESpecialMessage>();
		switch (Message)
		{
			case PBESpecialMessage.DraggedOut:
			case PBESpecialMessage.Endure:
			case PBESpecialMessage.HPDrained:
			case PBESpecialMessage.Recoil:
			case PBESpecialMessage.Struggle:
			{
				Params = new ReadOnlyCollection<object>(new object[] { battle.Trainers[r.ReadByte()], r.ReadEnum<PBEFieldPosition>() });
				break;
			}
			case PBESpecialMessage.Magnitude:
			case PBESpecialMessage.MultiHit:
			{
				Params = new ReadOnlyCollection<object>(new object[] { r.ReadByte() });
				break;
			}
			case PBESpecialMessage.NothingHappened:
			case PBESpecialMessage.OneHitKnockout:
			case PBESpecialMessage.PayDay:
			{
				Params = new ReadOnlyCollection<object>(Array.Empty<object>());
				break;
			}
			case PBESpecialMessage.PainSplit:
			{
				Params = new ReadOnlyCollection<object>(new object[] { battle.Trainers[r.ReadByte()], r.ReadEnum<PBEFieldPosition>(), battle.Trainers[r.ReadByte()], r.ReadEnum<PBEFieldPosition>() });
				break;
			}
			default: throw new InvalidDataException();
		}
	}
}
