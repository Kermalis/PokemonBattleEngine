using Kermalis.PokemonBattleEngine;
using Kermalis.PokemonBattleEngine.AI;
using Kermalis.PokemonBattleEngine.Battle;
using Kermalis.PokemonBattleEngine.Data;
using Kermalis.PokemonBattleEngine.Localization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Kermalis.PokemonBattleEngineTesting
{
    class AIBattle
    {
        const string LogFile = "Test Log.txt";
        const string ReplayFile = "Test Replay.pbereplay";
        static StreamWriter writer;
        static TextWriter oldWriter;

        public static void Test()
        {
            Console.WriteLine("----- Pokémon Battle Engine Test -----");

            PBESettings settings = PBESettings.DefaultSettings;
            PBEPokemonShell[] team0Party, team1Party;

            // Completely Randomized Pokémon
            team0Party = new PBEPokemonShell[settings.MaxPartySize];
            team1Party = new PBEPokemonShell[settings.MaxPartySize];
            IEnumerable<PBESpecies> allSpecies = Enum.GetValues(typeof(PBESpecies)).Cast<PBESpecies>().Except(new[] { PBESpecies.Arceus_Bug, PBESpecies.Arceus_Dark, PBESpecies.Arceus_Dragon, PBESpecies.Arceus_Electric, PBESpecies.Arceus_Fighting, PBESpecies.Arceus_Fire, PBESpecies.Arceus_Flying, PBESpecies.Arceus_Ghost, PBESpecies.Arceus_Grass, PBESpecies.Arceus_Ground, PBESpecies.Arceus_Ice, PBESpecies.Arceus_Poison, PBESpecies.Arceus_Psychic, PBESpecies.Arceus_Rock, PBESpecies.Arceus_Steel, PBESpecies.Arceus_Water, PBESpecies.Castform_Rainy, PBESpecies.Castform_Snowy, PBESpecies.Castform_Sunny, PBESpecies.Cherrim_Sunshine, PBESpecies.Darmanitan_Zen, PBESpecies.Genesect_Burn, PBESpecies.Genesect_Chill, PBESpecies.Genesect_Douse, PBESpecies.Genesect_Shock, PBESpecies.Giratina_Origin, PBESpecies.Keldeo_Resolute, PBESpecies.Meloetta_Pirouette });
            for (int i = 0; i < settings.MaxPartySize; i++)
            {
                PBEPokemonShell Create()
                {
                    PBESpecies species = allSpecies.Sample();
                    PBEPokemonData pData = PBEPokemonData.Data[species];
                    var shell = new PBEPokemonShell
                    {
                        Species = species,
                        Ability = pData.Abilities.Sample(),
                        Gender = PBEUtils.RNG.NextGender(species),
                        Level = settings.MaxLevel,
                        Friendship = byte.MaxValue,
                        Nature = (PBENature)PBEUtils.RNG.Next((int)PBENature.MAX),
                        Nickname = PBEPokemonLocalization.Names[(PBESpecies)((uint)species & 0xFFFF)].English,
                        Shiny = PBEUtils.RNG.NextShiny(),
                        EVs = new byte[6],
                        IVs = new byte[6],
                        PPUps = new byte[settings.NumMoves],
                        Moves = new PBEMove[settings.NumMoves],
                        Item = PBEItem.None
                    };
                    for (int j = 0; j < 6; j++)
                    {
                        shell.IVs[j] = (byte)PBEUtils.RNG.Next(settings.MaxIVs + 1);
                    }
                    var legalMoves = PBELegalityChecker.GetLegalMoves(species, shell.Level).ToList();
                    for (int j = 0; j < settings.NumMoves; j++)
                    {
                        if (legalMoves.Count == 0)
                        {
                            break;
                        }
                        PBEMove move = legalMoves.Sample();
                        shell.Moves[j] = move;
                        legalMoves.Remove(move);
                    }
                    return shell;
                }
                team0Party[i] = Create();
                team1Party[i] = Create();
            }

            // Randomized Competitive Pokémon
            /*team0Party = PBECompetitivePokemonShells.CreateRandomTeam(settings.MaxPartySize).ToArray();
            team1Party = PBECompetitivePokemonShells.CreateRandomTeam(settings.MaxPartySize).ToArray();*/

            // Predefined Pokémon
            /*team0Party = new PBEPokemonShell[]
            {
                PBECompetitivePokemonShells.Zoroark_VGC,
                PBECompetitivePokemonShells.Volcarona_VGC,
                PBECompetitivePokemonShells.Vaporeon_VGC,
                PBECompetitivePokemonShells.Thundurus_VGC,
                PBECompetitivePokemonShells.Vanilluxe_VGC,
                PBECompetitivePokemonShells.Chandelure_VGC
            };
            team1Party = new PBEPokemonShell[]
            {
                PBECompetitivePokemonShells.Arceus_Uber,
                PBECompetitivePokemonShells.Darkrai_Uber,
                PBECompetitivePokemonShells.Kyurem_White_Uber,
                PBECompetitivePokemonShells.Latias_VGC,
                PBECompetitivePokemonShells.Metagross_VGC,
                PBECompetitivePokemonShells.Victini_Uber
            };*/

            var battle = new PBEBattle(PBEBattleFormat.Double, settings, team0Party, team1Party);
            battle.Teams[0].TrainerName = "Team 1";
            battle.Teams[1].TrainerName = "Team 2";
            battle.OnNewEvent += PBEBattle.ConsoleBattleEventHandler;
            battle.OnStateChanged += Battle_OnStateChanged;
            try
            {
                writer = new StreamWriter(new FileStream(LogFile, FileMode.Create, FileAccess.Write));
            }
            catch (Exception e)
            {
                Console.WriteLine($"Cannot open \"{LogFile}\" for writing.");
                Console.WriteLine(e.Message);
                return;
            }
            oldWriter = Console.Out;
            Console.SetOut(writer);
            battle.Begin();
        }
        static void Battle_OnStateChanged(PBEBattle battle)
        {
            try
            {
                switch (battle.BattleState)
                {
                    case PBEBattleState.Ended:
                        {
                            Console.SetOut(oldWriter);
                            writer.Close();
                            try
                            {
                                battle.SaveReplay(ReplayFile);
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine("Error saving replay:");
                                Console.WriteLine(e.Message);
                                Console.WriteLine(e.StackTrace);
                            }
                            Console.WriteLine("Test battle ended. The battle was saved to \"{0}\" and \"{1}\".", LogFile, ReplayFile);
                            Console.ReadKey();
                            break;
                        }
                    case PBEBattleState.ReadyToRunTurn:
                        {
                            foreach (PBETeam team in battle.Teams)
                            {
                                Console.WriteLine();
                                Console.WriteLine("{0}'s team:", team.TrainerName);
                                foreach (PBEPokemon pkmn in team.ActiveBattlers)
                                {
                                    Console.WriteLine(pkmn);
                                    Console.WriteLine();
                                }
                            }
                            battle.RunTurn();
                            break;
                        }
                    case PBEBattleState.WaitingForActions:
                        {
                            foreach (PBETeam team in battle.Teams)
                            {
                                IEnumerable<PBEAction> actions = PBEAIManager.CreateActions(team);
                                if (!PBEBattle.AreActionsValid(team, actions))
                                {
                                    throw new Exception($"{team.TrainerName}'s AI created invalid actions!");
                                }
                                PBEBattle.SelectActionsIfValid(team, actions);
                            }
                            break;
                        }
                    case PBEBattleState.WaitingForSwitchIns:
                        {
                            foreach (PBETeam team in battle.Teams.Where(t => t.SwitchInsRequired > 0))
                            {
                                IEnumerable<Tuple<byte, PBEFieldPosition>> switches = PBEAIManager.CreateSwitches(team);
                                if (!PBEBattle.AreSwitchesValid(team, switches))
                                {
                                    throw new Exception($"{team.TrainerName}'s AI created invalid switches!");
                                }
                                PBEBattle.SelectSwitchesIfValid(team, switches);
                            }
                            break;
                        }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
                Console.SetOut(oldWriter);
                writer.Close();
                Console.WriteLine("Test battle threw an exception, check \"{0}\" for details.", LogFile);
                Console.ReadKey();
                Environment.Exit(-1);
            }
        }
    }
}
