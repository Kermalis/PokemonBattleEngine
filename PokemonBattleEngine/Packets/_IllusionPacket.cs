using Kermalis.EndianBinaryIO;
using Kermalis.PokemonBattleEngine.Battle;
using Kermalis.PokemonBattleEngine.Data;
using System.Collections.ObjectModel;
using System.IO;

namespace Kermalis.PokemonBattleEngine.Packets;

public sealed class PBEIllusionPacket : IPBEPacket
{
	public const ushort ID = 0x25;
	public ReadOnlyCollection<byte> Data { get; }

	public PBETrainer PokemonTrainer { get; }
	public PBEFieldPosition Pokemon { get; }
	public PBEGender ActualGender { get; }
	public PBEItem ActualCaughtBall { get; }
	public bool ActualShiny { get; }
	public string ActualNickname { get; }
	public PBESpecies ActualSpecies { get; }
	public PBEForm ActualForm { get; }
	public PBEType ActualType1 { get; }
	public PBEType ActualType2 { get; }
	public float ActualWeight { get; }

	internal PBEIllusionPacket(PBEBattlePokemon pokemon)
	{
		using (var ms = new MemoryStream())
		{
			EndianBinaryWriter w = PBEPacketProcessor.WritePacketID(ms, ID);

			w.WriteByte((PokemonTrainer = pokemon.Trainer).Id);
			w.WriteEnum(Pokemon = pokemon.FieldPosition);
			w.WriteEnum(ActualGender = pokemon.Gender);
			w.WriteEnum(ActualCaughtBall = pokemon.CaughtBall);
			w.WriteChars_NullTerminated(ActualNickname = pokemon.Nickname);
			w.WriteBoolean(ActualShiny = pokemon.Shiny);
			w.WriteEnum(ActualSpecies = pokemon.Species);
			w.WriteEnum(ActualForm = pokemon.Form);
			w.WriteEnum(ActualType1 = pokemon.Type1);
			w.WriteEnum(ActualType2 = pokemon.Type2);
			w.WriteSingle(ActualWeight = pokemon.Weight);

			Data = new ReadOnlyCollection<byte>(ms.ToArray());
		}
	}
	internal PBEIllusionPacket(byte[] data, EndianBinaryReader r, PBEBattle battle)
	{
		Data = new ReadOnlyCollection<byte>(data);

		PokemonTrainer = battle.Trainers[r.ReadByte()];
		Pokemon = r.ReadEnum<PBEFieldPosition>();
		ActualGender = r.ReadEnum<PBEGender>();
		ActualCaughtBall = r.ReadEnum<PBEItem>();
		ActualNickname = r.ReadString_NullTerminated();
		ActualShiny = r.ReadBoolean();
		ActualSpecies = r.ReadEnum<PBESpecies>();
		ActualForm = r.ReadEnum<PBEForm>();
		ActualType1 = r.ReadEnum<PBEType>();
		ActualType2 = r.ReadEnum<PBEType>();
		ActualWeight = r.ReadSingle();
	}
}
