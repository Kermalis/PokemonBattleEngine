using Kermalis.EndianBinaryIO;
using Kermalis.PokemonBattleEngine.Data;
using Kermalis.PokemonBattleEngine.Data.Utils;
using Kermalis.PokemonBattleEngine.DefaultData;
using Kermalis.SimpleNARC;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Kermalis.PokemonBattleEngineExtras;

internal static partial class PokemonDataDumper
{
	// You must dump everything yourself
	// The GBA ROMs must all be USA v1.0
	// Colo and XD must be USA
	// DPPt dumps use overlay files which may or may not have different offsets depending on the region, so just keep in mind I have USA versions of each game
	// HGSS/Gen5 dumps should work across all regions
	//
	// Colo and XD level-up moves are in common.fsys/common_rel.fdat
	//
	// D, P, and Pt level-up move NARC is /poketool/personal/wotbl.narc (D and P have identical level-up move NARCs)
	// D, P, and Pt egg moves are in overlay/overlay_0005.bin
	// Pt TMHM moves are in the Pokémon data NARC which is /poketool/personal/pl_personal.narc (Pt changed no TMHM compatibility from DP so I use it alone)
	// Pt tutor compatibility is in overlay/overlay_0005.bin
	// HG and SS level-up move NARC is /a/0/3/3 (HG and SS have identical level-up move NARCs)
	// HG and SS TMHM moves are in the Pokémon data NARC which is /a/0/0/2 (HG and SS have identical Pokémon data NARCs)
	// HG and SS tutor compatibility is in /fielddata/wazaoshie/waza_oshie.bin (HG and SS have identical tutor compatibility)
	// HG and SS egg move NARC is /a/2/2/9 (HG and SS have identical egg move NARCs)
	//
	// B2 and W2 evolution NARC is /a/0/1/9 (B2 and W2 have identical evolution NARCs)
	// B, W, B2, and W2 level-up move NARC is /a/0/1/8 (B and W have identical level-up move NARCs) (B2 and W2 have identical level-up move NARCs)
	// B, W, B2, and W2 TMHM moves are in the Pokémon data NARC which is /a/0/1/6 (B and W have identical Pokémon data NARCs) (B2 and W2 have identical Pokémon data NARCs)
	// B2 and W2 tutor compatibility is in the Pokémon data NARC which is /a/0/1/6 (B2 and W2 have identical Pokémon data NARCs)
	// B and W egg move NARC is /a/1/2/3, B2 and W2 egg move NARC is /a/1/2/4 (B, W, B2, and W2 have identical egg move NARCs)

	private static Dictionary<(PBESpecies, PBEForm), Pokemon> _dict = null!;
	private static EndianBinaryReader _r = null!;
	private static EndianBinaryReader _s = null!;
	private static EndianBinaryReader _fr = null!;
	private static EndianBinaryReader _lg = null!;
	private static EndianBinaryReader _e = null!;
	private static EndianBinaryReader _coloCommonRel = null!;
	private static EndianBinaryReader _xdCommonRel = null!;

	public static void Run(SqliteConnection con)
	{
		using (FileStream s_r = File.OpenRead(@"../../../\DumpedData\R.gba"))
		using (FileStream s_s = File.OpenRead(@"../../../\DumpedData\S.gba"))
		using (FileStream s_fr = File.OpenRead(@"../../../\DumpedData\FR.gba"))
		using (FileStream s_lg = File.OpenRead(@"../../../\DumpedData\LG.gba"))
		using (FileStream s_e = File.OpenRead(@"../../../\DumpedData\E.gba"))
		using (FileStream s_colo = File.OpenRead(@"../../../\DumpedData\Colocommon_rel.fdat"))
		using (FileStream s_xd = File.OpenRead(@"../../../\DumpedData\XDcommon_rel.fdat"))
		using (SqliteTransaction transaction = con.BeginTransaction())
		using (SqliteCommand cmd = con.CreateCommand())
		{
			cmd.Transaction = transaction;
			_dict = new();

			_r = new EndianBinaryReader(s_r, endianness: Endianness.LittleEndian);
			_s = new EndianBinaryReader(s_s, endianness: Endianness.LittleEndian);
			_fr = new EndianBinaryReader(s_fr, endianness: Endianness.LittleEndian);
			_lg = new EndianBinaryReader(s_lg, endianness: Endianness.LittleEndian);
			_e = new EndianBinaryReader(s_e, endianness: Endianness.LittleEndian);
			_coloCommonRel = new EndianBinaryReader(s_colo, endianness: Endianness.BigEndian);
			_xdCommonRel = new EndianBinaryReader(s_xd, endianness: Endianness.BigEndian);

			B2W2_PokeData();

			Gen3_LevelUp();
			Gen4_LevelUp();
			Gen5_LevelUp();

			Gen3_TMHM();
			Gen4_TMHM();
			Gen5_TMHM();

			FRLGE_Tutor();
			XD_Tutor();
			Pt_Tutor();
			HGSS_Tutor();
			Gen5_Tutor();

			Gen34_Egg();
			Gen5_Egg();

			Fix_FormPokedata();
			Fix_BasculinBlueAbility();
			Fix_ArceusTypes();
			Fix_FormEggMoves();
			Fix_ShedinjaEvoMoves();
			Fix_FRLGStarterMoves();
			Fix_XDMew();
			Fix_VoltTackle();
			Fix_Gen4FreeMoves();
			Fix_RotomFormMoves();
			Fix_RelicSong_SecretSword();

			DreamWorld();

			WriteToDatabase(cmd);

			transaction.Commit();
		}
	}

	private static Pokemon AddSpecies((PBESpecies, PBEForm) key)
	{
		if (!_dict.TryGetValue(key, out Pokemon? pkmn))
		{
			pkmn = new Pokemon();
			_dict.Add(key, pkmn);
		}
		return pkmn;
	}
	private static void AddLevelUpMove((PBESpecies, PBEForm) key, PBEMove move, byte level, PBEDDMoveObtainMethod flag)
	{
		Pokemon pkmn = AddSpecies(key);
		Dictionary<(PBEMove, byte), PBEDDMoveObtainMethod> levelUp = pkmn.LevelUpMoves;
		(PBEMove, byte) lKey = (move, level);
		if (levelUp.ContainsKey(lKey))
		{
			levelUp[lKey] |= flag;
		}
		else
		{
			levelUp.Add(lKey, flag);
		}
	}
	private static void AddOtherMove((PBESpecies, PBEForm) key, PBEMove move, PBEDDMoveObtainMethod flag)
	{
		Pokemon pkmn = AddSpecies(key);
		Dictionary<PBEMove, PBEDDMoveObtainMethod> other = pkmn.OtherMoves;
		if (other.ContainsKey(move))
		{
			other[move] |= flag;
		}
		else
		{
			other.Add(move, flag);
		}
	}
	private static void AddEvolution((PBESpecies, PBEForm) baybee, (PBESpecies, PBEForm) dadee)
	{
		AddSpecies(baybee).Evolutions.Add(dadee);
		AddSpecies(dadee).PreEvolutions.Add(baybee);
	}
	private static (PBESpecies, PBEForm) GetGen3Key(EndianBinaryReader reader, int sp)
	{
		PBESpecies species = _gen3SpeciesIndexToPBESpecies[sp];
		PBEForm form = 0;
		if (species == PBESpecies.Deoxys)
		{
			if (reader == _e)
			{
				form = PBEForm.Deoxys_Speed;
			}
			else if (reader == _lg)
			{
				form = PBEForm.Deoxys_Defense;
			}
			else if (reader == _fr)
			{
				form = PBEForm.Deoxys_Attack;
			}
		}
		return (species, form);
	}

	private static void B2W2_PokeData()
	{
		var b2w2Pokedata = new NARC(@"../../../\DumpedData\B2W2Pokedata.narc");
		var b2w2Evolution = new NARC(@"../../../\DumpedData\B2W2Evolution.narc");

		for (int sp = 1; sp <= 708; sp++)
		{
			// Skip Egg, Bad Egg, and Pokéstar Studios Pokémon
			if (sp > 649 && sp < 685)
			{
				continue;
			}

			if (!_b2w2SpeciesIndexToPBESpecies.TryGetValue(sp, out (PBESpecies, PBEForm) key))
			{
				key = ((PBESpecies)sp, 0);
			}
			Pokemon pkmn = AddSpecies(key);

			using (var s_p = new MemoryStream(b2w2Pokedata[sp]))
			using (var s_e = new MemoryStream(b2w2Evolution[sp]))
			{
				var pokedata = new EndianBinaryReader(s_p, endianness: Endianness.LittleEndian);
				var evolution = new EndianBinaryReader(s_e, endianness: Endianness.LittleEndian);

				// Pokedata
				pkmn.HP = pokedata.ReadByte(0x0);
				pkmn.Attack = pokedata.ReadByte(0x1);
				pkmn.Defense = pokedata.ReadByte(0x2);
				pkmn.SpAttack = pokedata.ReadByte(0x4);
				pkmn.SpDefense = pokedata.ReadByte(0x5);
				pkmn.Speed = pokedata.ReadByte(0x3);
				pkmn.Type1 = Utils.Gen5Types[pokedata.ReadByte(0x6)];
				pkmn.Type2 = Utils.Gen5Types[pokedata.ReadByte(0x7)];
				if (pkmn.Type1 == pkmn.Type2)
				{
					pkmn.Type2 = PBEType.None;
				}
				pkmn.CatchRate = pokedata.ReadByte(0x8);
				pkmn.GenderRatio = (PBEGenderRatio)pokedata.ReadByte(0x12);
				pkmn.GrowthRate = (PBEGrowthRate)pokedata.ReadByte(0x15);
				for (int i = 0; i < 3; i++)
				{
					var ability = (PBEAbility)pokedata.ReadByte(0x18 + i);
					if (ability != PBEAbility.None && !pkmn.Abilities.Contains(ability))
					{
						pkmn.Abilities.Add(ability);
					}
				}
				pkmn.FleeRate = pokedata.ReadByte(0x1B);
				pkmn.BaseEXPYield = pokedata.ReadUInt16(0x22);
				pkmn.Weight = MathF.Round(pokedata.ReadUInt16(0x26) * 0.1f, 1);
				// Evolution
				for (int i = 0; i < 7; i++)
				{
					ushort method = evolution.ReadUInt16();
					evolution.ReadUInt16(); // Param
					var evo = (PBESpecies)evolution.ReadUInt16();
					if (method != 0)
					{
						AddEvolution(key, (evo, 0));
					}
				}
			}
		}
	}

	private static void Gen3_LevelUp()
	{
#pragma warning disable CS8321 // Local function is declared but never used
		for (int sp = 1; sp <= 411; sp++)
		{
			// Gen 2 Unown slots are ignored in gen 3
			if (sp > 251 && sp < 277)
			{
				continue;
			}

			// It is the same in Ruby, Sapphire, Colo, and XD; the others have some differences
			_r.Stream.Position = 0x207BC8 + (sizeof(uint) * sp);
			_s.Stream.Position = 0x207B58 + (sizeof(uint) * sp);
			_fr.Stream.Position = 0x25D7B4 + (sizeof(uint) * sp);
			_lg.Stream.Position = 0x25D794 + (sizeof(uint) * sp);
			_e.Stream.Position = 0x32937C + (sizeof(uint) * sp);
			_coloCommonRel.Stream.Position = 0x123250 + (0x11C * sp) + 0xBA;
			_xdCommonRel.Stream.Position = 0x29DA8 + (0x124 * sp) + 0xC4;

			void ReadGBALevelUpMoves(EndianBinaryReader reader, PBEDDMoveObtainMethod flag)
			{
				(PBESpecies, PBEForm) key = GetGen3Key(reader, sp);
				reader.Stream.Position = reader.ReadUInt32() - 0x8000000;
				while (true)
				{
					ushort val = reader.ReadUInt16();
					if (val == 0xFFFF)
					{
						break;
					}
					else
					{
						AddLevelUpMove(key, (PBEMove)(val & 0x1FF), (byte)(val >> 9), flag);
					}
				}
			}
			ReadGBALevelUpMoves(_r, PBEDDMoveObtainMethod.LevelUp_RSColoXD);
			//ReadGBALevelUpMoves(_s, PBEDDMoveObtainMethod.LevelUp_RSColoXD);
			ReadGBALevelUpMoves(_fr, PBEDDMoveObtainMethod.LevelUp_FR);
			ReadGBALevelUpMoves(_lg, PBEDDMoveObtainMethod.LevelUp_LG);
			ReadGBALevelUpMoves(_e, PBEDDMoveObtainMethod.LevelUp_E);
			void ReadGCLevelUpMoves(EndianBinaryReader reader, PBEDDMoveObtainMethod flag)
			{
				(PBESpecies, PBEForm) key = GetGen3Key(reader, sp);
				for (int i = 0; i < 17; i++)
				{
					byte level = reader.ReadByte();
					reader.ReadByte(); // Padding
					var move = (PBEMove)reader.ReadUInt16();
					if (move == PBEMove.None)
					{
						break;
					}
					else
					{
						AddLevelUpMove(key, move, level, flag);
					}
				}
			}
			//ReadGCLevelUpMoves(_coloCommonRel, PBEDDMoveObtainMethod.LevelUp_RSColoXD);
			//ReadGCLevelUpMoves(_xdCommonRel, PBEDDMoveObtainMethod.LevelUp_RSColoXD);
		}
#pragma warning restore CS8321 // Local function is declared but never used
	}
	private static void Gen4_LevelUp()
	{
		var dp = new NARC(@"../../../\DumpedData\DPLevelUp.narc");
		var pt = new NARC(@"../../../\DumpedData\PtLevelUp.narc");
		var hgss = new NARC(@"../../../\DumpedData\HGSSLevelUp.narc");

		for (int sp = 1; sp <= 507; sp++)
		{
			// 494 is Egg, 495 is Bad Egg
			if (sp == 494 || sp == 495)
			{
				continue;
			}

			if (!_gen4SpeciesIndexToPBESpecies.TryGetValue(sp, out (PBESpecies, PBEForm) key))
			{
				key = ((PBESpecies)sp, 0);
			}

			void ReadLevelUpMoves(byte[] file, PBEDDMoveObtainMethod flag)
			{
				using (var ms = new MemoryStream(file))
				{
					var reader = new EndianBinaryReader(ms, endianness: Endianness.LittleEndian);
					while (true)
					{
						ushort val = reader.ReadUInt16();
						if (val == 0xFFFF)
						{
							break;
						}

						AddLevelUpMove(key, (PBEMove)(val & 0x1FF), (byte)(val >> 9), flag);
					}
				}
			}
			// DP only has 0-500
			if (sp <= 500)
			{
				ReadLevelUpMoves(dp[sp], PBEDDMoveObtainMethod.LevelUp_DP);
			}
			ReadLevelUpMoves(pt[sp], PBEDDMoveObtainMethod.LevelUp_Pt);
			ReadLevelUpMoves(hgss[sp], PBEDDMoveObtainMethod.LevelUp_HGSS);
		}
	}
	private static void Gen5_LevelUp()
	{
		var bw = new NARC(@"../../../\DumpedData\BWLevelUp.narc");
		var b2w2 = new NARC(@"../../../\DumpedData\B2W2LevelUp.narc");
		for (int sp = 1; sp <= 708; sp++)
		{
			void ReadLevelUpMoves(byte[] file, bool isBW)
			{
				if (!(isBW ? _bwSpeciesIndexToPBESpecies : _b2w2SpeciesIndexToPBESpecies).TryGetValue(sp, out (PBESpecies, PBEForm) key))
				{
					key = ((PBESpecies)sp, 0);
				}

				using (var ms = new MemoryStream(file))
				{
					var reader = new EndianBinaryReader(ms, endianness: Endianness.LittleEndian);
					while (true)
					{
						uint val = reader.ReadUInt32();
						if (val == 0xFFFFFFFF)
						{
							break;
						}

						AddLevelUpMove(key, (PBEMove)val, (byte)(val >> 0x10), isBW ? PBEDDMoveObtainMethod.LevelUp_BW : PBEDDMoveObtainMethod.LevelUp_B2W2);
					}
				}
			}
			// BW only has 0-667 (no Egg or Bad Egg)
			if (sp <= 667)
			{
				ReadLevelUpMoves(bw[sp], true);
			}
			// Skip Egg, Bad Egg, and Pokéstar Studios Pokémon in B2W2
			if (sp <= 649 || sp >= 685)
			{
				ReadLevelUpMoves(b2w2[sp], false);
			}
		}
	}

	private static void Gen3_TMHM()
	{
#pragma warning disable CS8321 // Local function is declared but never used
		for (int sp = 1; sp <= 411; sp++)
		{
			// Gen 2 Unown slots are ignored in gen 3
			if (sp > 251 && sp < 277)
			{
				continue;
			}

			// It is the same across all of gen 3, so I will only read one
			_r.Stream.Position = 0x1FD0F0 + (8 * sp);
			_s.Stream.Position = 0x1FD080 + (8 * sp);
			_fr.Stream.Position = 0x252BC8 + (8 * sp);
			_lg.Stream.Position = 0x252BA4 + (8 * sp);
			_e.Stream.Position = 0x31E898 + (8 * sp);
			_coloCommonRel.Stream.Position = 0x123250 + (0x11C * sp) + 0x34;
			_xdCommonRel.Stream.Position = 0x29DA8 + (0x124 * sp) + 0x34;

			PBEDDMoveObtainMethod GetFlag(int i)
			{
				return i < 50 ? PBEDDMoveObtainMethod.TM_RSFRLGEColoXD : PBEDDMoveObtainMethod.HM_RSFRLGEColoXD;
			}
			void ReadGBATMHM(EndianBinaryReader reader)
			{
				(PBESpecies, PBEForm) key = GetGen3Key(reader, sp);
				Span<byte> bytes = stackalloc byte[8];
				reader.ReadBytes(bytes);
				for (int i = 0; i < _gen3TMHMs.Length; i++)
				{
					if ((bytes[i / 8] & (1 << (i % 8))) != 0)
					{
						AddOtherMove(key, _gen3TMHMs[i], GetFlag(i));
					}
				}
			}
			ReadGBATMHM(_r);
			//ReadGBATMHM(_s);
			//ReadGBATMHM(_fr);
			//ReadGBATMHM(_lg);
			//ReadGBATMHM(_e);
			void ReadGCTMHM(EndianBinaryReader reader)
			{
				(PBESpecies, PBEForm) key = GetGen3Key(reader, sp);
				for (int i = 0; i < _gen3TMHMs.Length; i++)
				{
					if (reader.ReadBoolean())
					{
						AddOtherMove(key, _gen3TMHMs[i], GetFlag(i));
					}
				}
			}
			//ReadGCTMHM(_coloCommonRel);
			//ReadGCTMHM(_xdCommonRel);
		}
#pragma warning restore CS8321 // Local function is declared but never used
	}
	private static void Gen4_TMHM()
	{
		var dppt = new NARC(@"../../../\DumpedData\PtPokedata.narc");
		var hgss = new NARC(@"../../../\DumpedData\HGSSPokedata.narc");

		for (int sp = 1; sp <= 507; sp++)
		{
			// 494 is Egg, 495 is Bad Egg
			if (sp == 494 || sp == 495)
			{
				continue;
			}

			if (!_gen4SpeciesIndexToPBESpecies.TryGetValue(sp, out (PBESpecies, PBEForm) key))
			{
				key = ((PBESpecies)sp, 0);
			}
			void ReadTMHMMoves(byte[] file, bool isDPPt)
			{
				Span<byte> bytes = stackalloc byte[13];
				using (var ms = new MemoryStream(file))
				{
					var reader = new EndianBinaryReader(ms, endianness: Endianness.LittleEndian);
					ms.Position = 0x1C;
					reader.ReadBytes(bytes);
					for (int i = 0; i < _gen4TMHMs.Length; i++)
					{
						if ((bytes[i / 8] & (1 << (i % 8))) != 0)
						{
							PBEMove move = _gen4TMHMs[i];
							if (move == PBEMove.None)
							{
								move = isDPPt ? PBEMove.Defog : PBEMove.Whirlpool;
							}
							AddOtherMove(key, move, i < 92 ? (isDPPt ? PBEDDMoveObtainMethod.TM_DPPt : PBEDDMoveObtainMethod.TM_HGSS) : (isDPPt ? PBEDDMoveObtainMethod.HM_DPPt : PBEDDMoveObtainMethod.HM_HGSS));
						}
					}
				}
			}
			ReadTMHMMoves(dppt[sp], true);
			ReadTMHMMoves(hgss[sp], false);
		}
	}
	private static void Gen5_TMHM()
	{
		var bw = new NARC(@"../../../\DumpedData\BWPokedata.narc");
		var b2w2 = new NARC(@"../../../\DumpedData\B2W2Pokedata.narc");

		for (int sp = 1; sp <= 708; sp++)
		{
			void ReadTMHMMoves(byte[] file, bool isBW)
			{
				if (!(isBW ? _bwSpeciesIndexToPBESpecies : _b2w2SpeciesIndexToPBESpecies).TryGetValue(sp, out (PBESpecies, PBEForm) key))
				{
					key = ((PBESpecies)sp, 0);
				}

				Span<byte> bytes = stackalloc byte[13];
				using (var ms = new MemoryStream(file))
				{
					var reader = new EndianBinaryReader(ms, endianness: Endianness.LittleEndian);
					ms.Position = 0x28;
					reader.ReadBytes(bytes);
					for (int i = 0; i < _gen5TMHMs.Length; i++)
					{
						if ((bytes[i / 8] & (1 << (i % 8))) == 0)
						{
							continue;
						}

						PBEDDMoveObtainMethod flag;
						if (i < 95)
						{
							flag = isBW ? PBEDDMoveObtainMethod.TM_BW : PBEDDMoveObtainMethod.TM_B2W2;
						}
						else
						{
							flag = PBEDDMoveObtainMethod.HM_BWB2W2;
						}
						AddOtherMove(key, _gen5TMHMs[i], flag);
					}
				}
			}
			// BW only has 0-667 (no Egg or Bad Egg)
			if (sp <= 667)
			{
				ReadTMHMMoves(bw[sp], true);
			}
			// Skip Egg, Bad Egg, and Pokéstar Studios Pokémon in B2W2
			if (sp <= 649 || sp >= 685)
			{
				ReadTMHMMoves(b2w2[sp], false);
			}
		}
	}

	private static void FRLGE_Tutor()
	{
		for (int sp = 1; sp <= 411; sp++)
		{
			// Gen 2 Unown slots are ignored in gen 3
			if (sp > 251 && sp < 277)
			{
				continue;
			}

			// It is the same in FR and LG, so I will only read one
			_fr.Stream.Position = 0x459B7E + (sizeof(ushort) * sp);
			_lg.Stream.Position = 0x45959E + (sizeof(ushort) * sp);
			_e.Stream.Position = 0x615048 + (sizeof(uint) * sp);

			void ReadTutorMoves(EndianBinaryReader reader, PBEMove[] tutorMoves, PBEDDMoveObtainMethod flag)
			{
				(PBESpecies, PBEForm) key = GetGen3Key(reader, sp);
				uint val = reader == _e ? reader.ReadUInt32() : reader.ReadUInt16();
				for (int i = 0; i < tutorMoves.Length; i++)
				{
					if ((val & (1u << i)) != 0)
					{
						AddOtherMove(key, tutorMoves[i], flag);
					}
				}
			}
			ReadTutorMoves(_fr, _frlgTutorMoves, PBEDDMoveObtainMethod.MoveTutor_FRLG);
			//ReadTutorMoves(_lg, frlgTutorMoves, PBEDDMoveObtainMethod.MoveTutor_FRLG);
			ReadTutorMoves(_e, _emeraldTutorMoves, PBEDDMoveObtainMethod.MoveTutor_E);
		}
	}
	private static void XD_Tutor()
	{
		for (int sp = 1; sp <= 411; sp++)
		{
			// Gen 2 Unown slots are ignored in gen 3
			if (sp > 251 && sp < 277)
			{
				continue;
			}

			(PBESpecies, PBEForm) key = GetGen3Key(_xdCommonRel, sp);
			_xdCommonRel.Stream.Position = 0x29DA8 + (0x124 * sp) + 0x6E;
			for (int i = 0; i < _xdTutorMoves.Length; i++)
			{
				if (_xdCommonRel.ReadBoolean())
				{
					AddOtherMove(key, _xdTutorMoves[i], PBEDDMoveObtainMethod.MoveTutor_XD);
				}
			}
		}
	}
	private static void Pt_Tutor()
	{
		using (FileStream fs = File.OpenRead(@"../../../\DumpedData\Ptoverlay_0005.bin"))
		{
			var pt = new EndianBinaryReader(fs, endianness: Endianness.LittleEndian);
			Span<byte> bytes = stackalloc byte[5];

			fs.Position = 0x3012C;
			for (int sp = 1; sp <= 493; sp++)
			{
				(PBESpecies, PBEForm) key = ((PBESpecies)sp, 0);
				pt.ReadBytes(bytes);

				for (int i = 0; i < _ptTutorMoves.Length; i++)
				{
					if ((bytes[i / 8] & (1 << (i % 8))) != 0)
					{
						AddOtherMove(key, _ptTutorMoves[i], PBEDDMoveObtainMethod.MoveTutor_Pt);
					}
				}
			}
		}
	}
	private static void HGSS_Tutor()
	{
		using (FileStream fs = File.OpenRead(@"../../../\DumpedData\HGSSTutor.bin"))
		{
			var r = new EndianBinaryReader(fs, endianness: Endianness.LittleEndian);
			Span<byte> bytes = stackalloc byte[8];

			for (int sp = 1; sp <= 505; sp++) // Includes forms but not eggs
			{
				(PBESpecies, PBEForm) key = sp > 493 ? _gen4SpeciesIndexToPBESpecies[sp + 2] : ((PBESpecies)sp, 0);
				r.ReadBytes(bytes);

				for (int i = 0; i < _hgssTutorMoves.Length; i++)
				{
					if ((bytes[i / 8] & (1 << (i % 8))) != 0)
					{
						AddOtherMove(key, _hgssTutorMoves[i], PBEDDMoveObtainMethod.MoveTutor_HGSS);
					}
				}
			}
		}
	}
	private static void Gen5_Tutor()
	{
		var bw = new NARC(@"../../../\DumpedData\BWPokedata.narc");
		var b2w2 = new NARC(@"../../../\DumpedData\B2W2Pokedata.narc");

		for (int sp = 1; sp <= 708; sp++)
		{
			void ReadFreeTutorMoves(EndianBinaryReader reader, bool isBW)
			{
				if (!(isBW ? _bwSpeciesIndexToPBESpecies : _b2w2SpeciesIndexToPBESpecies).TryGetValue(sp, out (PBESpecies, PBEForm) key))
				{
					key = ((PBESpecies)sp, 0);
				}
				byte val = reader.ReadByte(0x38);
				for (int i = 0; i < _gen5FreeTutorMoves.Length; i++)
				{
					if ((val & (1 << i)) != 0)
					{
						AddOtherMove(key, _gen5FreeTutorMoves[i], isBW ? PBEDDMoveObtainMethod.MoveTutor_BW : PBEDDMoveObtainMethod.MoveTutor_B2W2);
					}
				}
			}
			void ReadB2W2TutorMoves(EndianBinaryReader reader)
			{
				if (!_b2w2SpeciesIndexToPBESpecies.TryGetValue(sp, out (PBESpecies, PBEForm) key))
				{
					key = ((PBESpecies)sp, 0);
				}
				reader.Stream.Position = 0x3C;
				for (int i = 0; i < _b2w2TutorMoves.Length; i++)
				{
					uint val = reader.ReadUInt32();
					for (int j = 0; j < _b2w2TutorMoves[i].Length; j++)
					{
						if ((val & (1u << j)) != 0)
						{
							AddOtherMove(key, _b2w2TutorMoves[i][j], PBEDDMoveObtainMethod.MoveTutor_B2W2);
						}
					}
				}
			}
			// BW only has 0-667 (no Egg or Bad Egg)
			if (sp <= 667)
			{
				using (var ms = new MemoryStream(bw[sp]))
				{
					var reader = new EndianBinaryReader(ms, endianness: Endianness.LittleEndian);
					ReadFreeTutorMoves(reader, true);
				}
			}
			// Skip Egg, Bad Egg, and Pokéstar Studios Pokémon
			if (sp <= 649 || sp >= 685)
			{
				using (var ms = new MemoryStream(b2w2[sp]))
				{
					var reader = new EndianBinaryReader(ms, endianness: Endianness.LittleEndian);
					ReadFreeTutorMoves(reader, false);
					ReadB2W2TutorMoves(reader);
				}
			}
		}
	}

	private static void Gen34_Egg()
	{
		using (FileStream s_d = File.OpenRead(@"../../../\DumpedData\Doverlay_0005.bin"))
		using (FileStream s_p = File.OpenRead(@"../../../\DumpedData\Poverlay_0005.bin"))
		using (FileStream s_pt = File.OpenRead(@"../../../\DumpedData\Ptoverlay_0005.bin"))
		using (var s_hgss = new MemoryStream(new NARC(@"../../../\DumpedData\HGSSEgg.narc")[0]))
		{
			var d = new EndianBinaryReader(s_d, endianness: Endianness.LittleEndian);
			var p = new EndianBinaryReader(s_p, endianness: Endianness.LittleEndian);
			var pt = new EndianBinaryReader(s_pt, endianness: Endianness.LittleEndian);
			var hgss = new EndianBinaryReader(s_hgss, endianness: Endianness.LittleEndian);

			// The table is the same in all five GBA games, so I will only read one
			_r.Stream.Position = 0x2091DC;
			_s.Stream.Position = 0x20916C;
			_fr.Stream.Position = 0x25EF0C;
			_lg.Stream.Position = 0x25EEEC;
			_e.Stream.Position = 0x32ADD8;
			// The table is the same across DPPt, so I will only read one
			d.Stream.Position = 0x20668;
			p.Stream.Position = 0x20668;
			pt.Stream.Position = 0x29222;

			static void ReadEggMoves(EndianBinaryReader reader, bool isGen3, PBEDDMoveObtainMethod flag)
			{
				PBESpecies species = 0;
				while (true)
				{
					ushort val = reader.ReadUInt16();
					if (val == 0xFFFF)
					{
						break;
					}
					else if (val > 20000)
					{
						int speciesIndex = val - 20000;
						species = isGen3 ? _gen3SpeciesIndexToPBESpecies[speciesIndex] : (PBESpecies)speciesIndex;
					}
					else
					{
						AddOtherMove((species, 0), (PBEMove)val, flag);
					}
				}
			}
			ReadEggMoves(_r, true, PBEDDMoveObtainMethod.EggMove_RSFRLGE);
			//ReadEggMoves(_s, true, PBEDDMoveObtainMethod.EggMove_RSFRLGE);
			//ReadEggMoves(_fr, true, PBEDDMoveObtainMethod.EggMove_RSFRLGE);
			//ReadEggMoves(_lg, true, PBEDDMoveObtainMethod.EggMove_RSFRLGE);
			//ReadEggMoves(_e, true, PBEDDMoveObtainMethod.EggMove_RSFRLGE);
			ReadEggMoves(d, false, PBEDDMoveObtainMethod.EggMove_DPPt);
			//ReadEggMoves(p, false, PBEDDMoveObtainMethod.EggMove_DPPt);
			//ReadEggMoves(pt, false, PBEDDMoveObtainMethod.EggMove_DPPt);
			ReadEggMoves(hgss, false, PBEDDMoveObtainMethod.EggMove_HGSS);
		}
	}
	private static void Gen5_Egg()
	{
		var bwb2w2 = new NARC(@"../../../\DumpedData\BWB2W2Egg.narc");
		for (int sp = 1; sp <= 649; sp++)
		{
			using (var ms = new MemoryStream(bwb2w2[sp]))
			{
				var reader = new EndianBinaryReader(ms, endianness: Endianness.LittleEndian);
				ushort numEggMoves = reader.ReadUInt16();
				if (numEggMoves > 0)
				{
					(PBESpecies, PBEForm) key = ((PBESpecies)sp, 0);
					for (int i = 0; i < numEggMoves; i++)
					{
						AddOtherMove(key, (PBEMove)reader.ReadUInt16(), PBEDDMoveObtainMethod.EggMove_BWB2W2);
					}
				}
			}
		}
	}

	private static void Fix_FormPokedata()
	{
		static void CopySpecies((PBESpecies, PBEForm) baseKey, (PBESpecies, PBEForm) newKey)
		{
			Pokemon basePkmn = _dict[baseKey];
			Pokemon pkmn = AddSpecies(newKey);
			pkmn.HP = basePkmn.HP;
			pkmn.Attack = basePkmn.Attack;
			pkmn.Defense = basePkmn.Defense;
			pkmn.SpAttack = basePkmn.SpAttack;
			pkmn.SpDefense = basePkmn.SpDefense;
			pkmn.Speed = basePkmn.Speed;
			pkmn.Type1 = basePkmn.Type1;
			pkmn.Type2 = basePkmn.Type2;
			pkmn.GenderRatio = basePkmn.GenderRatio;
			pkmn.GrowthRate = basePkmn.GrowthRate;
			pkmn.BaseEXPYield = basePkmn.BaseEXPYield;
			pkmn.Abilities = basePkmn.Abilities;
			pkmn.CatchRate = basePkmn.CatchRate;
			pkmn.FleeRate = basePkmn.FleeRate;
			pkmn.Weight = basePkmn.Weight;
			pkmn.LevelUpMoves = basePkmn.LevelUpMoves;
			pkmn.OtherMoves = basePkmn.OtherMoves;
		}
		CopySpecies((PBESpecies.Burmy, PBEForm.Burmy_Plant), (PBESpecies.Burmy, PBEForm.Burmy_Sandy));
		CopySpecies((PBESpecies.Burmy, PBEForm.Burmy_Plant), (PBESpecies.Burmy, PBEForm.Burmy_Trash));
		CopySpecies((PBESpecies.Cherrim, PBEForm.Cherrim), (PBESpecies.Cherrim, PBEForm.Cherrim_Sunshine));
		CopySpecies((PBESpecies.Deerling, PBEForm.Deerling_Spring), (PBESpecies.Deerling, PBEForm.Deerling_Summer));
		CopySpecies((PBESpecies.Deerling, PBEForm.Deerling_Spring), (PBESpecies.Deerling, PBEForm.Deerling_Autumn));
		CopySpecies((PBESpecies.Deerling, PBEForm.Deerling_Spring), (PBESpecies.Deerling, PBEForm.Deerling_Winter));
		CopySpecies((PBESpecies.Gastrodon, PBEForm.Gastrodon_West), (PBESpecies.Gastrodon, PBEForm.Gastrodon_East));
		CopySpecies((PBESpecies.Genesect, PBEForm.Genesect), (PBESpecies.Genesect, PBEForm.Genesect_Douse));
		CopySpecies((PBESpecies.Genesect, PBEForm.Genesect), (PBESpecies.Genesect, PBEForm.Genesect_Shock));
		CopySpecies((PBESpecies.Genesect, PBEForm.Genesect), (PBESpecies.Genesect, PBEForm.Genesect_Burn));
		CopySpecies((PBESpecies.Genesect, PBEForm.Genesect), (PBESpecies.Genesect, PBEForm.Genesect_Chill));
		CopySpecies((PBESpecies.Sawsbuck, PBEForm.Sawsbuck_Spring), (PBESpecies.Sawsbuck, PBEForm.Sawsbuck_Summer));
		CopySpecies((PBESpecies.Sawsbuck, PBEForm.Sawsbuck_Spring), (PBESpecies.Sawsbuck, PBEForm.Sawsbuck_Autumn));
		CopySpecies((PBESpecies.Sawsbuck, PBEForm.Sawsbuck_Spring), (PBESpecies.Sawsbuck, PBEForm.Sawsbuck_Winter));
		CopySpecies((PBESpecies.Shellos, PBEForm.Shellos_West), (PBESpecies.Shellos, PBEForm.Shellos_East));
		CopySpecies((PBESpecies.Unown, PBEForm.Unown_A), (PBESpecies.Unown, PBEForm.Unown_B));
		CopySpecies((PBESpecies.Unown, PBEForm.Unown_A), (PBESpecies.Unown, PBEForm.Unown_C));
		CopySpecies((PBESpecies.Unown, PBEForm.Unown_A), (PBESpecies.Unown, PBEForm.Unown_D));
		CopySpecies((PBESpecies.Unown, PBEForm.Unown_A), (PBESpecies.Unown, PBEForm.Unown_E));
		CopySpecies((PBESpecies.Unown, PBEForm.Unown_A), (PBESpecies.Unown, PBEForm.Unown_F));
		CopySpecies((PBESpecies.Unown, PBEForm.Unown_A), (PBESpecies.Unown, PBEForm.Unown_G));
		CopySpecies((PBESpecies.Unown, PBEForm.Unown_A), (PBESpecies.Unown, PBEForm.Unown_H));
		CopySpecies((PBESpecies.Unown, PBEForm.Unown_A), (PBESpecies.Unown, PBEForm.Unown_I));
		CopySpecies((PBESpecies.Unown, PBEForm.Unown_A), (PBESpecies.Unown, PBEForm.Unown_J));
		CopySpecies((PBESpecies.Unown, PBEForm.Unown_A), (PBESpecies.Unown, PBEForm.Unown_K));
		CopySpecies((PBESpecies.Unown, PBEForm.Unown_A), (PBESpecies.Unown, PBEForm.Unown_L));
		CopySpecies((PBESpecies.Unown, PBEForm.Unown_A), (PBESpecies.Unown, PBEForm.Unown_M));
		CopySpecies((PBESpecies.Unown, PBEForm.Unown_A), (PBESpecies.Unown, PBEForm.Unown_N));
		CopySpecies((PBESpecies.Unown, PBEForm.Unown_A), (PBESpecies.Unown, PBEForm.Unown_O));
		CopySpecies((PBESpecies.Unown, PBEForm.Unown_A), (PBESpecies.Unown, PBEForm.Unown_P));
		CopySpecies((PBESpecies.Unown, PBEForm.Unown_A), (PBESpecies.Unown, PBEForm.Unown_Q));
		CopySpecies((PBESpecies.Unown, PBEForm.Unown_A), (PBESpecies.Unown, PBEForm.Unown_R));
		CopySpecies((PBESpecies.Unown, PBEForm.Unown_A), (PBESpecies.Unown, PBEForm.Unown_S));
		CopySpecies((PBESpecies.Unown, PBEForm.Unown_A), (PBESpecies.Unown, PBEForm.Unown_T));
		CopySpecies((PBESpecies.Unown, PBEForm.Unown_A), (PBESpecies.Unown, PBEForm.Unown_U));
		CopySpecies((PBESpecies.Unown, PBEForm.Unown_A), (PBESpecies.Unown, PBEForm.Unown_V));
		CopySpecies((PBESpecies.Unown, PBEForm.Unown_A), (PBESpecies.Unown, PBEForm.Unown_W));
		CopySpecies((PBESpecies.Unown, PBEForm.Unown_A), (PBESpecies.Unown, PBEForm.Unown_X));
		CopySpecies((PBESpecies.Unown, PBEForm.Unown_A), (PBESpecies.Unown, PBEForm.Unown_Y));
		CopySpecies((PBESpecies.Unown, PBEForm.Unown_A), (PBESpecies.Unown, PBEForm.Unown_Z));
		CopySpecies((PBESpecies.Unown, PBEForm.Unown_A), (PBESpecies.Unown, PBEForm.Unown_Exclamation));
		CopySpecies((PBESpecies.Unown, PBEForm.Unown_A), (PBESpecies.Unown, PBEForm.Unown_Question));
		AddEvolution((PBESpecies.Burmy, PBEForm.Burmy_Sandy), (PBESpecies.Wormadam, PBEForm.Wormadam_Sandy));
		AddEvolution((PBESpecies.Burmy, PBEForm.Burmy_Sandy), (PBESpecies.Mothim, 0));
		AddEvolution((PBESpecies.Burmy, PBEForm.Burmy_Trash), (PBESpecies.Wormadam, PBEForm.Wormadam_Trash));
		AddEvolution((PBESpecies.Burmy, PBEForm.Burmy_Trash), (PBESpecies.Mothim, 0));
		AddEvolution((PBESpecies.Deerling, PBEForm.Deerling_Summer), (PBESpecies.Sawsbuck, PBEForm.Sawsbuck_Summer));
		AddEvolution((PBESpecies.Deerling, PBEForm.Deerling_Autumn), (PBESpecies.Sawsbuck, PBEForm.Sawsbuck_Autumn));
		AddEvolution((PBESpecies.Deerling, PBEForm.Deerling_Winter), (PBESpecies.Sawsbuck, PBEForm.Sawsbuck_Winter));
		AddEvolution((PBESpecies.Shellos, PBEForm.Shellos_East), (PBESpecies.Gastrodon, PBEForm.Gastrodon_East));
	}
	private static void Fix_BasculinBlueAbility()
	{
		_dict[(PBESpecies.Basculin, PBEForm.Basculin_Blue)].Abilities.Add(PBEAbility.Reckless);
	}
	private static void Fix_ArceusTypes()
	{
		Pokemon basePkmn = _dict[(PBESpecies.Arceus, PBEForm.Arceus)];

		void FixArceus(PBEForm form, PBEType type)
		{
			Pokemon pkmn = AddSpecies((PBESpecies.Arceus, form));
			pkmn.HP = basePkmn.HP;
			pkmn.Attack = basePkmn.Attack;
			pkmn.Defense = basePkmn.Defense;
			pkmn.SpAttack = basePkmn.SpAttack;
			pkmn.SpDefense = basePkmn.SpDefense;
			pkmn.Speed = basePkmn.Speed;
			pkmn.Type1 = type;
			pkmn.Type2 = basePkmn.Type2;
			pkmn.GenderRatio = basePkmn.GenderRatio;
			pkmn.GrowthRate = basePkmn.GrowthRate;
			pkmn.BaseEXPYield = basePkmn.BaseEXPYield;
			pkmn.Abilities = basePkmn.Abilities;
			pkmn.CatchRate = basePkmn.CatchRate;
			pkmn.FleeRate = basePkmn.FleeRate;
			pkmn.Weight = basePkmn.Weight;
			pkmn.Evolutions = basePkmn.Evolutions;
			pkmn.PreEvolutions = basePkmn.PreEvolutions;
			pkmn.LevelUpMoves = basePkmn.LevelUpMoves;
			pkmn.OtherMoves = basePkmn.OtherMoves;
		}

		FixArceus(PBEForm.Arceus_Fighting, PBEType.Fighting);
		FixArceus(PBEForm.Arceus_Flying, PBEType.Flying);
		FixArceus(PBEForm.Arceus_Poison, PBEType.Poison);
		FixArceus(PBEForm.Arceus_Ground, PBEType.Ground);
		FixArceus(PBEForm.Arceus_Rock, PBEType.Rock);
		FixArceus(PBEForm.Arceus_Bug, PBEType.Bug);
		FixArceus(PBEForm.Arceus_Ghost, PBEType.Ghost);
		FixArceus(PBEForm.Arceus_Steel, PBEType.Steel);
		FixArceus(PBEForm.Arceus_Fire, PBEType.Fire);
		FixArceus(PBEForm.Arceus_Water, PBEType.Water);
		FixArceus(PBEForm.Arceus_Grass, PBEType.Grass);
		FixArceus(PBEForm.Arceus_Electric, PBEType.Electric);
		FixArceus(PBEForm.Arceus_Psychic, PBEType.Psychic);
		FixArceus(PBEForm.Arceus_Ice, PBEType.Ice);
		FixArceus(PBEForm.Arceus_Dragon, PBEType.Dragon);
		FixArceus(PBEForm.Arceus_Dark, PBEType.Dark);
	}
	private static void Fix_FormEggMoves()
	{
		Span<PBEDDMoveObtainMethod> flags = stackalloc PBEDDMoveObtainMethod[]
		{
			PBEDDMoveObtainMethod.EggMove_RSFRLGE,
			PBEDDMoveObtainMethod.EggMove_DPPt,
			PBEDDMoveObtainMethod.EggMove_HGSS,
			PBEDDMoveObtainMethod.EggMove_BWB2W2,
			PBEDDMoveObtainMethod.EggMove_Special,
		};

		foreach ((PBESpecies, PBEForm) key in _b2w2SpeciesIndexToPBESpecies.Values)
		{
			foreach (KeyValuePair<PBEMove, PBEDDMoveObtainMethod> kvp in _dict[(key.Item1, 0)].OtherMoves)
			{
				PBEMove move = kvp.Key;
				PBEDDMoveObtainMethod o = kvp.Value;
				foreach (PBEDDMoveObtainMethod flag in flags)
				{
					if (o.HasFlag(flag))
					{
						AddOtherMove(key, move, flag);
					}
				}
			}
		}
	}
	private static void Fix_ShedinjaEvoMoves()
	{
		(PBESpecies, PBEForm) key = (PBESpecies.Shedinja, 0);
		Span<PBEDDMoveObtainMethod> flags = stackalloc PBEDDMoveObtainMethod[]
		{
			PBEDDMoveObtainMethod.LevelUp_RSColoXD,
			PBEDDMoveObtainMethod.LevelUp_FR,
			PBEDDMoveObtainMethod.LevelUp_E,
			PBEDDMoveObtainMethod.LevelUp_DP,
			PBEDDMoveObtainMethod.LevelUp_Pt,
			PBEDDMoveObtainMethod.LevelUp_HGSS,
		};

		// Nincada evolves starting at level 20
		foreach (KeyValuePair<(PBEMove, byte), PBEDDMoveObtainMethod> kvp in _dict[(PBESpecies.Ninjask, 0)].LevelUpMoves)
		{
			(PBEMove move, byte level) = kvp.Key;
			if (level >= 20)
			{
				PBEDDMoveObtainMethod o = kvp.Value;
				foreach (PBEDDMoveObtainMethod flag in flags)
				{
					if (o.HasFlag(flag))
					{
						AddLevelUpMove(key, move, level, flag);
					}
				}
			}
		}
	}
	private static void Fix_FRLGStarterMoves()
	{
		PBEDDMoveObtainMethod flag = PBEDDMoveObtainMethod.MoveTutor_FRLG;
		AddOtherMove((PBESpecies.Venusaur, 0), PBEMove.FrenzyPlant, flag);
		AddOtherMove((PBESpecies.Charizard, 0), PBEMove.BlastBurn, flag);
		AddOtherMove((PBESpecies.Blastoise, 0), PBEMove.HydroCannon, flag);
	}
	private static void Fix_XDMew()
	{
		(PBESpecies, PBEForm) key = (PBESpecies.Mew, 0);
		Pokemon pkmn = _dict[key];
		var list = new List<PBEMove>
		{
			PBEMove.FaintAttack,
			PBEMove.FakeOut,
			PBEMove.Hypnosis,
			PBEMove.NightShade,
			PBEMove.RolePlay,
			PBEMove.ZapCannon,
		};
		foreach (KeyValuePair<PBEMove, PBEDDMoveObtainMethod> kvp in pkmn.OtherMoves)
		{
			PBEDDMoveObtainMethod o = kvp.Value;
			if (o.HasFlag(PBEDDMoveObtainMethod.TM_RSFRLGEColoXD)
				|| o.HasFlag(PBEDDMoveObtainMethod.HM_RSFRLGEColoXD)
				|| o.HasFlag(PBEDDMoveObtainMethod.MoveTutor_FRLG)
				|| o.HasFlag(PBEDDMoveObtainMethod.MoveTutor_E))
			{
				list.Add(kvp.Key);
			}
		}
		foreach (PBEMove move in list)
		{
			AddOtherMove(key, move, PBEDDMoveObtainMethod.MoveTutor_XD);
		}
	}
	private static void Fix_VoltTackle()
	{
		AddOtherMove((PBESpecies.Pichu, 0), PBEMove.VoltTackle, PBEDDMoveObtainMethod.EggMove_Special);
	}
	private static void Fix_Gen4FreeMoves()
	{
		PBEDDMoveObtainMethod flag = PBEDDMoveObtainMethod.MoveTutor_DP | PBEDDMoveObtainMethod.MoveTutor_Pt | PBEDDMoveObtainMethod.MoveTutor_HGSS;
		AddOtherMove((PBESpecies.Venusaur, 0), PBEMove.FrenzyPlant, flag);
		AddOtherMove((PBESpecies.Charizard, 0), PBEMove.BlastBurn, flag);
		AddOtherMove((PBESpecies.Blastoise, 0), PBEMove.HydroCannon, flag);
		AddOtherMove((PBESpecies.Meganium, 0), PBEMove.FrenzyPlant, flag);
		AddOtherMove((PBESpecies.Typhlosion, 0), PBEMove.BlastBurn, flag);
		AddOtherMove((PBESpecies.Feraligatr, 0), PBEMove.HydroCannon, flag);
		AddOtherMove((PBESpecies.Sceptile, 0), PBEMove.FrenzyPlant, flag);
		AddOtherMove((PBESpecies.Blaziken, 0), PBEMove.BlastBurn, flag);
		AddOtherMove((PBESpecies.Swampert, 0), PBEMove.HydroCannon, flag);
		AddOtherMove((PBESpecies.Torterra, 0), PBEMove.FrenzyPlant, flag);
		AddOtherMove((PBESpecies.Infernape, 0), PBEMove.BlastBurn, flag);
		AddOtherMove((PBESpecies.Empoleon, 0), PBEMove.HydroCannon, flag);
		// Draco Meteor is taught to any Dragon type, including Arceus_Dragon
		foreach (KeyValuePair<(PBESpecies, PBEForm), Pokemon> pkmn in _dict.Where(kvp => kvp.Key.Item1 <= PBESpecies.Arceus && kvp.Value.HasType(PBEType.Dragon)))
		{
			AddOtherMove(pkmn.Key, PBEMove.DracoMeteor, flag);
		}
	}
	private static void Fix_RotomFormMoves()
	{
		AddOtherMove((PBESpecies.Rotom, PBEForm.Rotom), PBEMove.ThunderShock, PBEDDMoveObtainMethod.Form);
		AddOtherMove((PBESpecies.Rotom, PBEForm.Rotom_Fan), PBEMove.AirSlash, PBEDDMoveObtainMethod.Form);
		AddOtherMove((PBESpecies.Rotom, PBEForm.Rotom_Frost), PBEMove.Blizzard, PBEDDMoveObtainMethod.Form);
		AddOtherMove((PBESpecies.Rotom, PBEForm.Rotom_Heat), PBEMove.Overheat, PBEDDMoveObtainMethod.Form);
		AddOtherMove((PBESpecies.Rotom, PBEForm.Rotom_Mow), PBEMove.LeafStorm, PBEDDMoveObtainMethod.Form);
		AddOtherMove((PBESpecies.Rotom, PBEForm.Rotom_Wash), PBEMove.HydroPump, PBEDDMoveObtainMethod.Form);
	}
	private static void Fix_RelicSong_SecretSword()
	{
		PBEDDMoveObtainMethod flag = PBEDDMoveObtainMethod.MoveTutor_BW | PBEDDMoveObtainMethod.MoveTutor_B2W2;
		AddOtherMove((PBESpecies.Meloetta, 0), PBEMove.RelicSong, flag);
		AddOtherMove((PBESpecies.Keldeo, 0), PBEMove.SecretSword, flag);
	}

	private static void DreamWorld()
	{
		foreach ((PBESpecies species, PBEForm form, PBEMove moveA, PBEMove moveB, PBEMove moveC, bool bw, bool b2w2) in _dreamWorld)
		{
			PBEDDMoveObtainMethod o = PBEDDMoveObtainMethod.None;
			if (bw)
			{
				o |= PBEDDMoveObtainMethod.DreamWorld_BW;
			}
			if (b2w2)
			{
				o |= PBEDDMoveObtainMethod.DreamWorld_B2W2;
			}
			if (o == PBEDDMoveObtainMethod.None)
			{
				throw new Exception($"Problem with Dream World - {species}");
			}
			(PBESpecies, PBEForm) key = (species, form);
			AddOtherMove(key, moveA, o);
			AddOtherMove(key, moveB, o);
			AddOtherMove(key, moveC, o);
		}
	}

	private static void WriteToDatabase(SqliteCommand cmd)
	{
		const char Split1Char = '+';
		const char Split2Char = '|'; // Don't use commas since flags enums do

		cmd.CommandText = "DROP TABLE IF EXISTS PokemonData";
		cmd.ExecuteNonQuery();
		cmd.CommandText = "CREATE TABLE PokemonData(Species TEXT, Form TEXT"
			+ ", HP INTEGER, Attack INTEGER, Defense INTEGER, SpAttack INTEGER, SpDefense INTEGER, Speed INTEGER"
			+ ", Type1 INTEGER, Type2 INTEGER, GenderRatio INTEGER, GrowthRate INTEGER, BaseEXPYield INTEGER, CatchRate INTEGER, FleeRate INTEGER, Weight FLOAT"
			+ ", PreEvolutions TEXT, Evolutions TEXT, Abilities TEXT, LevelUpMoves TEXT, OtherMoves TEXT"
			+ ")";
		cmd.ExecuteNonQuery();
		cmd.CommandText = "INSERT INTO PokemonData VALUES(@0, @1, @2, @3, @4, @5, @6, @7, @8, @9, @10, @11, @12, @13, @14, @15, @16, @17, @18, @19, @20)";
		var list = new List<string>();
		foreach (KeyValuePair<(PBESpecies, PBEForm), Pokemon> tup in _dict)
		{
			(PBESpecies species, PBEForm form) = tup.Key;
			cmd.Parameters.AddWithValue("@0", species.ToString());
			cmd.Parameters.AddWithValue("@1", PBEDataUtils.GetNameOfForm(species, form) ?? "0");
			Pokemon pkmn = tup.Value;
			cmd.Parameters.AddWithValue("@2", pkmn.HP);
			cmd.Parameters.AddWithValue("@3", pkmn.Attack);
			cmd.Parameters.AddWithValue("@4", pkmn.Defense);
			cmd.Parameters.AddWithValue("@5", pkmn.SpAttack);
			cmd.Parameters.AddWithValue("@6", pkmn.SpDefense);
			cmd.Parameters.AddWithValue("@7", pkmn.Speed);
			cmd.Parameters.AddWithValue("@8", pkmn.Type1);
			cmd.Parameters.AddWithValue("@9", pkmn.Type2);
			cmd.Parameters.AddWithValue("@10", pkmn.GenderRatio);
			cmd.Parameters.AddWithValue("@11", pkmn.GrowthRate);
			cmd.Parameters.AddWithValue("@12", pkmn.BaseEXPYield);
			cmd.Parameters.AddWithValue("@13", pkmn.CatchRate);
			cmd.Parameters.AddWithValue("@14", pkmn.FleeRate);
			cmd.Parameters.AddWithValue("@15", pkmn.Weight.ToString("0.0")); // Gets weird if you don't truncate (like 9.9 becomes 9.89999999999999)
			list.Clear();
			foreach ((PBESpecies, PBEForm) key in pkmn.PreEvolutions)
			{
				PBESpecies spe = key.Item1;
				list.Add(spe.ToString() + Split2Char + (PBEDataUtils.GetNameOfForm(spe, key.Item2) ?? "0"));
			}
			cmd.Parameters.AddWithValue("@16", string.Join(Split1Char, list));
			list.Clear();
			foreach ((PBESpecies, PBEForm) key in pkmn.Evolutions)
			{
				PBESpecies spe = key.Item1;
				list.Add(spe.ToString() + Split2Char + (PBEDataUtils.GetNameOfForm(spe, key.Item2) ?? "0"));
			}
			cmd.Parameters.AddWithValue("@17", string.Join(Split1Char, list));
			list.Clear();
			foreach (PBEAbility ab in pkmn.Abilities)
			{
				list.Add(ab.ToString());
			}
			cmd.Parameters.AddWithValue("@18", string.Join(Split1Char, list));
			list.Clear();
			foreach (KeyValuePair<(PBEMove Move, byte Level), PBEDDMoveObtainMethod> levelUpMove in pkmn.LevelUpMoves)
			{
				(PBEMove move, byte level) = levelUpMove.Key;
				list.Add(move.ToString() + Split2Char + level.ToString() + Split2Char + levelUpMove.Value.ToString());
			}
			cmd.Parameters.AddWithValue("@19", string.Join(Split1Char, list));
			list.Clear();
			foreach (KeyValuePair<PBEMove, PBEDDMoveObtainMethod> otherMove in pkmn.OtherMoves)
			{
				list.Add(otherMove.Key.ToString() + Split2Char + otherMove.Value.ToString());
			}
			cmd.Parameters.AddWithValue("@20", string.Join(Split1Char, list));
			cmd.ExecuteNonQuery();
			cmd.Parameters.Clear();
		}
	}
}
