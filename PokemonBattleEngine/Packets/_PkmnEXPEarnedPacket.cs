using Kermalis.EndianBinaryIO;
using Kermalis.PokemonBattleEngine.Battle;
using System.Collections.ObjectModel;
using System.IO;

namespace Kermalis.PokemonBattleEngine.Packets;

public sealed class PBEPkmnEXPEarnedPacket : IPBEPacket
{
	public const ushort ID = 0x3E;
	public ReadOnlyCollection<byte> Data { get; }

	public PBETrainer PokemonTrainer { get; }
	public byte Pokemon { get; }
	public uint Earned { get; }

	internal PBEPkmnEXPEarnedPacket(PBEBattlePokemon pokemon, uint earned)
	{
		using (var ms = new MemoryStream())
		{
			EndianBinaryWriter w = PBEPacketProcessor.WritePacketID(ms, ID);

			w.WriteByte((PokemonTrainer = pokemon.Trainer).Id);
			w.WriteByte(Pokemon = pokemon.Id);
			w.WriteUInt32(Earned = earned);

			Data = new ReadOnlyCollection<byte>(ms.ToArray());
		}
	}
	internal PBEPkmnEXPEarnedPacket(byte[] data, EndianBinaryReader r, PBEBattle battle)
	{
		Data = new ReadOnlyCollection<byte>(data);

		PokemonTrainer = battle.Trainers[r.ReadByte()];
		Pokemon = r.ReadByte();
		Earned = r.ReadUInt32();
	}
}
