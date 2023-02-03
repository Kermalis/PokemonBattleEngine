using Kermalis.EndianBinaryIO;
using Kermalis.PokemonBattleEngine.Battle;
using System.Collections.ObjectModel;
using System.IO;

namespace Kermalis.PokemonBattleEngine.Packets;

public sealed class PBEWeatherDamagePacket : IPBEPacket
{
	public const ushort ID = 0x40;
	public ReadOnlyCollection<byte> Data { get; }

	public PBEWeather Weather { get; }
	public PBETrainer DamageVictimTrainer { get; }
	public PBEFieldPosition DamageVictim { get; }

	internal PBEWeatherDamagePacket(PBEWeather weather, PBEBattlePokemon damageVictim)
	{
		using (var ms = new MemoryStream())
		{
			EndianBinaryWriter w = PBEPacketProcessor.WritePacketID(ms, ID);

			w.WriteEnum(Weather = weather);
			w.WriteByte((DamageVictimTrainer = damageVictim.Trainer).Id);
			w.WriteEnum(DamageVictim = damageVictim.FieldPosition);

			Data = new ReadOnlyCollection<byte>(ms.ToArray());
		}
	}
	internal PBEWeatherDamagePacket(byte[] data, EndianBinaryReader r, PBEBattle battle)
	{
		Data = new ReadOnlyCollection<byte>(data);

		Weather = r.ReadEnum<PBEWeather>();
		DamageVictimTrainer = battle.Trainers[r.ReadByte()];
		DamageVictim = r.ReadEnum<PBEFieldPosition>();
	}
}
