using Kermalis.EndianBinaryIO;
using Kermalis.PokemonBattleEngine.Battle;
using System.Collections.ObjectModel;
using System.IO;

namespace Kermalis.PokemonBattleEngine.Packets;

public sealed class PBEWeatherPacket : IPBEPacket
{
	public const ushort ID = 0x14;
	public ReadOnlyCollection<byte> Data { get; }

	public PBEWeather Weather { get; }
	public PBEWeatherAction WeatherAction { get; }

	internal PBEWeatherPacket(PBEWeather weather, PBEWeatherAction weatherAction)
	{
		using (var ms = new MemoryStream())
		{
			EndianBinaryWriter w = PBEPacketProcessor.WritePacketID(ms, ID);

			w.WriteEnum(Weather = weather);
			w.WriteEnum(WeatherAction = weatherAction);

			Data = new ReadOnlyCollection<byte>(ms.ToArray());
		}
	}
	internal PBEWeatherPacket(byte[] data, EndianBinaryReader r)
	{
		Data = new ReadOnlyCollection<byte>(data);

		Weather = r.ReadEnum<PBEWeather>();
		WeatherAction = r.ReadEnum<PBEWeatherAction>();
	}
}
