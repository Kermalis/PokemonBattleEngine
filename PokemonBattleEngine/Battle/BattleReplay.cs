using Kermalis.EndianBinaryIO;
using Kermalis.PokemonBattleEngine.Data;
using Kermalis.PokemonBattleEngine.Packets;
using Kermalis.PokemonBattleEngine.Utils;
using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;

namespace Kermalis.PokemonBattleEngine.Battle;

public sealed partial class PBEBattle
{
	private const ushort CUR_REPLAY_VERSION = 0;

	public string GetDefaultReplayFileName()
	{
		// "2020-12-30 23-59-59 - Team 1 vs Team 2.pbereplay"
		return PBEUtils.ToSafeFileName(new string(string.Format("{0:yyyy-MM-dd HH-mm-ss} - {1} vs {2}", DateTime.Now, Teams[0].CombinedName, Teams[1].CombinedName).Take(200).ToArray())) + ".pbereplay";
	}
	private void CheckCanSaveReplay()
	{
		if (!IsLocallyHosted)
		{
			throw new InvalidOperationException("Can only save replays of locally hosted battles");
		}
		if (_battleState != PBEBattleState.Ended)
		{
			throw new InvalidOperationException($"{nameof(BattleState)} must be {PBEBattleState.Ended} to save a replay.");
		}
	}

	public void SaveReplay()
	{
		CheckCanSaveReplay();
		SaveReplay(GetDefaultReplayFileName());
	}
	public void SaveReplayToFolder(string path)
	{
		CheckCanSaveReplay();
		SaveReplay(Path.Combine(path, GetDefaultReplayFileName()));
	}
	public void SaveReplay(string path)
	{
		CheckCanSaveReplay();

		using (var ms = new MemoryStream())
		{
			var w = new EndianBinaryWriter(ms);
			w.WriteUInt16(CUR_REPLAY_VERSION);
			w.WriteInt32(_rand.Seed);

			int numEvents = Events.Count;
			w.WriteInt32(numEvents);
			for (int i = 0; i < numEvents; i++)
			{
				byte[] data = Events[i].Data.ToArray();
				w.WriteUInt16((ushort)data.Length);
				w.WriteBytes(data);
			}

			ms.Position = 0;
			w.WriteBytes(MD5.HashData(ms));

			File.WriteAllBytes(path, ms.ToArray());
		}
	}

	public static PBEBattle LoadReplay(string path, PBEPacketProcessor packetProcessor)
	{
		byte[] fileBytes = File.ReadAllBytes(path);
		using (var s = new MemoryStream(fileBytes))
		{
			var r = new EndianBinaryReader(s);

			byte[] hash;
			hash = MD5.HashData(fileBytes.AsSpan(0, fileBytes.Length - 16));
			for (int i = 0; i < 16; i++)
			{
				if (hash[i] != fileBytes[fileBytes.Length - 16 + i])
				{
					throw new InvalidDataException();
				}
			}
			ushort version = r.ReadUInt16(); // Unused for now
			int seed = r.ReadInt32(); // Unused for now
			PBEBattle b = null!; // The first packet should be a PBEBattlePacket
			int numEvents = r.ReadInt32();
			if (numEvents < 1)
			{
				throw new InvalidDataException();
			}
			for (int i = 0; i < numEvents; i++)
			{
				byte[] data = new byte[r.ReadUInt16()];
				r.ReadBytes(data);
				IPBEPacket packet = packetProcessor.CreatePacket(data, b);
				if (packet is PBEBattlePacket bp)
				{
					if (i != 0)
					{
						throw new InvalidDataException();
					}
					b = new PBEBattle(bp);
				}
				else
				{
					if (i == 0)
					{
						throw new InvalidDataException();
					}
					if (packet is PBEWildPkmnAppearedPacket wpap)
					{
						PBETrainer wildTrainer = b.Teams[1].Trainers[0];
						foreach (PBEPkmnAppearedInfo info in wpap.Pokemon)
						{
							PBEBattlePokemon pkmn = wildTrainer.GetPokemon(info.Pokemon);
							// Process disguise and position now
							pkmn.FieldPosition = info.FieldPosition;
							if (info.IsDisguised)
							{
								pkmn.Status2 |= PBEStatus2.Disguised;
								pkmn.KnownCaughtBall = info.CaughtBall;
								pkmn.KnownGender = info.Gender;
								pkmn.KnownNickname = info.Nickname;
								pkmn.KnownShiny = info.Shiny;
								pkmn.KnownSpecies = info.Species;
								pkmn.KnownForm = info.Form;
								IPBEPokemonData pData = PBEDataProvider.Instance.GetPokemonData(info);
								pkmn.KnownType1 = pData.Type1;
								pkmn.KnownType2 = pData.Type2;
							}
							b.ActiveBattlers.Add(pkmn);
						}
					}
					b.Events.Add(packet);
				}
			}
			b.BattleState = PBEBattleState.Ended;
			return b;
		}
	}
}
