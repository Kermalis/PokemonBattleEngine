using Kermalis.PokemonBattleEngine.Battle;
using Kermalis.PokemonBattleEngine.Data;
using Kermalis.PokemonBattleEngine.Data.Legality;
using Kermalis.PokemonBattleEngine.DefaultData;
using Kermalis.PokemonBattleEngine.DefaultData.AI;
using Kermalis.PokemonBattleEngine.Packets;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Kermalis.PokemonBattleEngineExtras;

internal sealed class AIBattleDemo
{
	private const string LOG_FILE = "AI Demo Log.txt";
	private const string REPLAY_FILE = "AI Demo.pbereplay";

	private readonly PBEBattle _battle;
	private readonly PBEDDAI _ai0, _ai1;
	private readonly StreamWriter _writer;
	private readonly TextWriter _oldWriter;

	public AIBattleDemo()
	{
		Console.WriteLine("----- Pokémon Battle Engine - AI Battle Demo -----");

		_writer = new StreamWriter(new FileStream(LOG_FILE, FileMode.Create, FileAccess.Write));
		_oldWriter = Console.Out;

		PBESettings settings = PBESettings.DefaultSettings;
		//var settings = new PBESettings { NumMoves = 8, MaxPartySize = 10 };
		//settings.MakeReadOnly();
		PBELegalPokemonCollection p0, p1;

		// Competitively Randomized Pokémon
		p0 = PBEDDRandomTeamGenerator.CreateRandomTeam(settings.MaxPartySize);
		p1 = PBEDDRandomTeamGenerator.CreateRandomTeam(settings.MaxPartySize);

		_battle = PBEBattle.CreateTrainerBattle(PBEBattleFormat.Double, settings, new PBETrainerInfo(p0, "Trainer 0", false), new PBETrainerInfo(p1, "Trainer 1", false),
			battleTerrain: PBEDataProvider.GlobalRandom.RandomBattleTerrain());
		_battle.OnNewEvent += PBEBattle.ConsoleBattleEventHandler;
		_battle.OnNewEvent += Battle_OnNewEvent;
		_ai0 = new PBEDDAI(_battle.Trainers[0]);
		_ai1 = new PBEDDAI(_battle.Trainers[1]);
		Console.SetOut(_writer);
	}
	public async Task Run()
	{
		try
		{
			await _battle.Begin();
			await RunTurns();
		}
		catch
		{
			Console.SetOut(_oldWriter);
			_writer.Dispose();
			_battle.OnNewEvent -= PBEBattle.ConsoleBattleEventHandler;
			_battle.OnNewEvent -= Battle_OnNewEvent;

			Console.WriteLine("Demo battle threw an exception; check \"{0}\" for details.", LOG_FILE);
			throw;
		}
	}
	private async Task RunTurns()
	{
		while (true)
		{
			switch (_battle.BattleState)
			{
				case PBEBattleState.Ended:
				{
					Console.SetOut(_oldWriter);
					_writer.Dispose();

					await _battle.SaveReplay(REPLAY_FILE);
					Console.WriteLine("Test battle ended. The battle was saved to \"{0}\" and \"{1}\".", LOG_FILE, REPLAY_FILE);
					Console.ReadKey();
					return;
				}
				case PBEBattleState.ReadyToRunSwitches:
				{
					await _battle.RunSwitches();
					break;
				}
				case PBEBattleState.ReadyToRunTurn:
				{
					foreach (PBETrainer t in _battle.Trainers)
					{
						Console.WriteLine();
						Console.WriteLine("{0}'s team:", t.Name);
						foreach (PBEBattlePokemon p in t.ActiveBattlersOrdered)
						{
							Console.WriteLine(p);
							Console.WriteLine();
						}
					}

					await _battle.RunTurn();
					break;
				}
				default: throw new InvalidOperationException();
			}
		}
	}

	private PBEDDAI GetAI(PBETrainer t)
	{
		return t.Id == 0 ? _ai0 : _ai1;
	}

	private Task Battle_OnNewEvent(PBEBattle battle, IPBEPacket packet)
	{
		switch (packet)
		{
			case PBEActionsRequestPacket arp:
			{
				GetAI(arp.Trainer).CreateActions();
				break;
			}
			case PBESwitchInRequestPacket sirp:
			{
				GetAI(sirp.Trainer).CreateSwitches();
				break;
			}
			case PBETurnBeganPacket tbp:
			{
				Console.SetOut(_oldWriter);
				DateTime time = DateTime.Now;
				Console.WriteLine($"Turn {tbp.TurnNumber}... ({time.Hour:D2}:{time.Minute:D2}:{time.Second:D2}:{time.Millisecond:D3})");
				Console.SetOut(_writer);
				break;
			}
		}

		return Task.CompletedTask;
	}
}
