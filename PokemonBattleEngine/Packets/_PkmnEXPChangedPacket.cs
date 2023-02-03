using Kermalis.EndianBinaryIO;
using Kermalis.PokemonBattleEngine.Battle;
using System.Collections.ObjectModel;
using System.IO;

namespace Kermalis.PokemonBattleEngine.Packets;

public sealed class PBEPkmnEXPChangedPacket : IPBEPacket
{
	public const ushort ID = 0x3D;
	public ReadOnlyCollection<byte> Data { get; }

	public PBETrainer PokemonTrainer { get; }
	public byte Pokemon { get; }
	public uint OldEXP { get; }
	public uint NewEXP { get; }

	internal PBEPkmnEXPChangedPacket(PBEBattlePokemon pokemon, uint oldEXP)
	{
		using (var ms = new MemoryStream())
		{
			EndianBinaryWriter w = PBEPacketProcessor.WritePacketID(ms, ID);

			w.WriteByte((PokemonTrainer = pokemon.Trainer).Id);
			w.WriteByte(Pokemon = pokemon.Id);
			w.WriteUInt32(OldEXP = oldEXP);
			w.WriteUInt32(NewEXP = pokemon.EXP);

			Data = new ReadOnlyCollection<byte>(ms.ToArray());
		}
	}
	internal PBEPkmnEXPChangedPacket(byte[] data, EndianBinaryReader r, PBEBattle battle)
	{
		Data = new ReadOnlyCollection<byte>(data);

		PokemonTrainer = battle.Trainers[r.ReadByte()];
		Pokemon = r.ReadByte();
		OldEXP = r.ReadUInt32();
		NewEXP = r.ReadUInt32();
	}
}
