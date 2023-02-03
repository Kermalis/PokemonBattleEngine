using Kermalis.EndianBinaryIO;
using Kermalis.PokemonBattleEngine.Battle;
using Kermalis.PokemonBattleEngine.Data;
using Kermalis.PokemonBattleEngine.Utils;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

namespace Kermalis.PokemonBattleEngine.Packets;

public sealed class PBEBattlePacket : IPBEPacket
{
	public const ushort ID = 0x05;
	public ReadOnlyCollection<byte> Data { get; }

	public sealed class PBETeamInfo
	{
		public sealed class PBETrainerInfo
		{
			public sealed class PBEBattlePokemonInfo // SleepTurns would be too much info for a client to have
			{
				public byte Id { get; }
				public PBESpecies Species { get; }
				public PBEForm Form { get; }
				public string Nickname { get; }
				public byte Level { get; }
				public uint EXP { get; }
				public bool Pokerus { get; }
				public byte Friendship { get; }
				public bool Shiny { get; }
				public PBEAbility Ability { get; }
				public PBENature Nature { get; }
				public PBEGender Gender { get; }
				public PBEItem Item { get; }
				public PBEItem CaughtBall { get; }
				public PBEStatus1 Status1 { get; }
				public PBEReadOnlyStatCollection EffortValues { get; }
				public PBEReadOnlyStatCollection IndividualValues { get; }
				public PBEReadOnlyPartyMoveset Moveset { get; }

				internal PBEBattlePokemonInfo(PBEBattlePokemon pkmn)
				{
					Id = pkmn.Id;
					Species = pkmn.OriginalSpecies;
					Form = pkmn.OriginalForm;
					Nickname = pkmn.Nickname;
					Level = pkmn.OriginalLevel;
					EXP = pkmn.OriginalEXP;
					Friendship = pkmn.Friendship;
					Shiny = pkmn.Shiny;
					Pokerus = pkmn.Pokerus;
					Ability = pkmn.OriginalAbility;
					Nature = pkmn.Nature;
					Gender = pkmn.Gender;
					Item = pkmn.OriginalItem;
					CaughtBall = pkmn.OriginalCaughtBall;
					Status1 = pkmn.OriginalStatus1;
					EffortValues = pkmn.OriginalEffortValues!;
					IndividualValues = pkmn.IndividualValues!;
					Moveset = pkmn.OriginalMoveset!;
				}
				internal PBEBattlePokemonInfo(EndianBinaryReader r)
				{
					Id = r.ReadByte();
					Species = r.ReadEnum<PBESpecies>();
					Form = r.ReadEnum<PBEForm>();
					Nickname = r.ReadString_NullTerminated();
					Level = r.ReadByte();
					EXP = r.ReadUInt32();
					Friendship = r.ReadByte();
					Shiny = r.ReadBoolean();
					Pokerus = r.ReadBoolean();
					Ability = r.ReadEnum<PBEAbility>();
					Nature = r.ReadEnum<PBENature>();
					Gender = r.ReadEnum<PBEGender>();
					Item = r.ReadEnum<PBEItem>();
					CaughtBall = r.ReadEnum<PBEItem>();
					Status1 = r.ReadEnum<PBEStatus1>();
					EffortValues = new PBEReadOnlyStatCollection(r);
					IndividualValues = new PBEReadOnlyStatCollection(r);
					Moveset = new PBEReadOnlyPartyMoveset(r);
				}

				internal void ToBytes(EndianBinaryWriter w)
				{
					w.WriteByte(Id);
					w.WriteEnum(Species);
					w.WriteEnum(Form);
					w.WriteChars_NullTerminated(Nickname);
					w.WriteByte(Level);
					w.WriteUInt32(EXP);
					w.WriteByte(Friendship);
					w.WriteBoolean(Shiny);
					w.WriteBoolean(Pokerus);
					w.WriteEnum(Ability);
					w.WriteEnum(Nature);
					w.WriteEnum(Gender);
					w.WriteEnum(Item);
					w.WriteEnum(CaughtBall);
					w.WriteEnum(Status1);
					EffortValues.ToBytes(w);
					IndividualValues.ToBytes(w);
					Moveset.ToBytes(w);
				}
			}
			public sealed class PBEInventorySlotInfo
			{
				public PBEItem Item { get; }
				public uint Quantity { get; }

				internal PBEInventorySlotInfo(PBEBattleInventory.PBEBattleInventorySlot slot)
				{
					Item = slot.Item;
					Quantity = slot.Quantity;
				}
				internal PBEInventorySlotInfo(EndianBinaryReader r)
				{
					Item = r.ReadEnum<PBEItem>();
					Quantity = r.ReadUInt32();
				}

				internal void ToBytes(EndianBinaryWriter w)
				{
					w.WriteEnum(Item);
					w.WriteUInt32(Quantity);
				}
			}

			public byte Id { get; }
			public string Name { get; }
			public ReadOnlyCollection<PBEInventorySlotInfo> Inventory { get; }
			public ReadOnlyCollection<PBEBattlePokemonInfo> Party { get; }

			internal PBETrainerInfo(PBETrainer trainer)
			{
				Id = trainer.Id;
				if (trainer.IsWild)
				{
					Name = string.Empty;
					Inventory = PBEEmptyReadOnlyCollection<PBEInventorySlotInfo>.Value;
				}
				else
				{
					Name = trainer.Name;
					Inventory = trainer.Inventory.Count == 0
						? PBEEmptyReadOnlyCollection<PBEInventorySlotInfo>.Value
						: new ReadOnlyCollection<PBEInventorySlotInfo>(trainer.Inventory.Values.Select(s => new PBEInventorySlotInfo(s)).ToArray());
				}
				Party = new ReadOnlyCollection<PBEBattlePokemonInfo>(trainer.Party.Select(p => new PBEBattlePokemonInfo(p)).ToArray());
			}
			internal PBETrainerInfo(EndianBinaryReader r)
			{
				Id = r.ReadByte();
				Name = r.ReadString_NullTerminated();
				int count = r.ReadUInt16();
				if (count == 0)
				{
					Inventory = PBEEmptyReadOnlyCollection<PBEInventorySlotInfo>.Value;
				}
				else
				{
					var inv = new PBEInventorySlotInfo[count];
					for (int i = 0; i < count; i++)
					{
						inv[i] = new PBEInventorySlotInfo(r);
					}
					Inventory = new ReadOnlyCollection<PBEInventorySlotInfo>(inv);
				}
				count = r.ReadByte();
				if (count == 0)
				{
					Party = PBEEmptyReadOnlyCollection<PBEBattlePokemonInfo>.Value;
				}
				else
				{
					var party = new PBEBattlePokemonInfo[count];
					for (int i = 0; i < count; i++)
					{
						party[i] = new PBEBattlePokemonInfo(r);
					}
					Party = new ReadOnlyCollection<PBEBattlePokemonInfo>(party);
				}
			}
			internal PBETrainerInfo(PBETrainerInfo other, byte? onlyForTrainer)
			{
				Id = other.Id;
				Name = other.Name;
				if (onlyForTrainer is not null && onlyForTrainer.Value == Id)
				{
					Inventory = other.Inventory;
					Party = other.Party;
				}
				else
				{
					Inventory = PBEEmptyReadOnlyCollection<PBEInventorySlotInfo>.Value;
					Party = PBEEmptyReadOnlyCollection<PBEBattlePokemonInfo>.Value;
				}
			}

			internal void ToBytes(EndianBinaryWriter w)
			{
				w.WriteByte(Id);
				w.WriteChars_NullTerminated(Name);
				ushort icount = (ushort)Inventory.Count;
				w.WriteUInt16(icount);
				for (int i = 0; i < icount; i++)
				{
					Inventory[i].ToBytes(w);
				}
				byte pcount = (byte)Party.Count;
				w.WriteByte(pcount);
				for (int i = 0; i < pcount; i++)
				{
					Party[i].ToBytes(w);
				}
			}
		}

		public byte Id { get; }
		public ReadOnlyCollection<PBETrainerInfo> Trainers { get; }

		internal PBETeamInfo(PBETeam team)
		{
			Id = team.Id;
			Trainers = new ReadOnlyCollection<PBETrainerInfo>(team.Trainers.Select(t => new PBETrainerInfo(t)).ToArray());
		}
		internal PBETeamInfo(EndianBinaryReader r)
		{
			Id = r.ReadByte();
			var trainers = new PBETrainerInfo[r.ReadByte()];
			for (int i = 0; i < trainers.Length; i++)
			{
				trainers[i] = new PBETrainerInfo(r);
			}
			Trainers = new ReadOnlyCollection<PBETrainerInfo>(trainers);
		}
		internal PBETeamInfo(PBETeamInfo other, byte? onlyForTrainer)
		{
			Id = other.Id;
			Trainers = new ReadOnlyCollection<PBETrainerInfo>(other.Trainers.Select(t => new PBETrainerInfo(t, onlyForTrainer)).ToArray());
		}

		internal void ToBytes(EndianBinaryWriter w)
		{
			w.WriteByte(Id);
			byte count = (byte)Trainers.Count;
			w.WriteByte(count);
			for (int i = 0; i < count; i++)
			{
				Trainers[i].ToBytes(w);
			}
		}
	}

	public PBEBattleType BattleType { get; }
	public PBEBattleFormat BattleFormat { get; }
	public PBEBattleTerrain BattleTerrain { get; }
	public PBEWeather Weather { get; }
	public PBESettings Settings { get; }
	public ReadOnlyCollection<PBETeamInfo> Teams { get; }

	internal PBEBattlePacket(PBEBattle battle)
	{
		using (var ms = new MemoryStream())
		{
			EndianBinaryWriter w = PBEPacketProcessor.WritePacketID(ms, ID);

			w.WriteEnum(BattleType = battle.BattleType);
			w.WriteEnum(BattleFormat = battle.BattleFormat);
			w.WriteEnum(BattleTerrain = battle.BattleTerrain);
			w.WriteEnum(Weather = battle.Weather);
			w.WriteBytes((Settings = battle.Settings).ToBytes());
			Teams = new ReadOnlyCollection<PBETeamInfo>(battle.Teams.Select(t => new PBETeamInfo(t)).ToArray());
			for (int i = 0; i < 2; i++)
			{
				Teams[i].ToBytes(w);
			}

			Data = new ReadOnlyCollection<byte>(ms.ToArray());
		}
	}
	internal PBEBattlePacket(byte[] data, EndianBinaryReader r)
	{
		Data = new ReadOnlyCollection<byte>(data);

		BattleType = r.ReadEnum<PBEBattleType>();
		BattleFormat = r.ReadEnum<PBEBattleFormat>();
		BattleTerrain = r.ReadEnum<PBEBattleTerrain>();
		Weather = r.ReadEnum<PBEWeather>();
		Settings = new PBESettings(r);
		Settings.MakeReadOnly();
		var teams = new PBETeamInfo[2];
		for (int i = 0; i < 2; i++)
		{
			teams[i] = new PBETeamInfo(r);
		}
		Teams = new ReadOnlyCollection<PBETeamInfo>(teams);
	}
	public PBEBattlePacket(PBEBattlePacket other, byte? onlyForTrainer)
	{
		using (var ms = new MemoryStream())
		{
			EndianBinaryWriter w = PBEPacketProcessor.WritePacketID(ms, ID);

			w.WriteEnum(BattleType = other.BattleType);
			w.WriteEnum(BattleFormat = other.BattleFormat);
			w.WriteEnum(BattleTerrain = other.BattleTerrain);
			w.WriteEnum(Weather = other.Weather);
			w.WriteBytes((Settings = other.Settings).ToBytes());
			Teams = new ReadOnlyCollection<PBETeamInfo>(other.Teams.Select(t => new PBETeamInfo(t, onlyForTrainer)).ToArray());
			for (int i = 0; i < 2; i++)
			{
				Teams[i].ToBytes(w);
			}

			Data = new ReadOnlyCollection<byte>(ms.ToArray());
		}
	}
}
