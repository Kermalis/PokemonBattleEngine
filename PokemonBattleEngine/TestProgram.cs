using Kermalis.PokemonBattleEngine.Battle;
using Kermalis.PokemonBattleEngine.Data;
using System;
using System.Linq;

namespace Kermalis.PokemonBattleEngine
{
    class PBETestProgram
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Pokémon Battle Engine Test");
            Console.WriteLine();

            PBESettings settings = PBESettings.DefaultSettings;
            PBEBattle battle = new PBEBattle(PBEBattleFormat.Single, settings, PBECompetitivePokemonShells.CreateRandomTeam(settings.MaxPartySize), PBECompetitivePokemonShells.CreateRandomTeam(settings.MaxPartySize));
            battle.OnNewEvent += PBEBattle.ConsoleBattleEventHandler;
            battle.OnStateChanged += Battle_OnStateChanged;
            battle.Begin();
        }
        static void Battle_OnStateChanged(PBEBattle battle)
        {
            switch (battle.BattleState)
            {
                case PBEBattleState.Ended:
                    Console.ReadKey();
                    break;
                case PBEBattleState.ReadyToRunTurn:
                    Console.WriteLine();
                    foreach (PBEPokemon pkmn in battle.ActiveBattlers)
                    {
                        Console.WriteLine(pkmn);
                        Console.WriteLine();
                    }
                    battle.RunTurn();
                    break;
                case PBEBattleState.WaitingForActions:
                    {
                        var actions = new PBEAction[1];
                        bool valid;
                        PBEPokemon pkmn;
                        int move;
                        do
                        {
                            pkmn = battle.Teams[0].ActiveBattlers[0];
                            move = PBEUtils.RNG.Next(0, pkmn.Moves.Length);
                            actions[0].PokemonId = pkmn.Id;
                            actions[0].Decision = PBEDecision.Fight;
                            actions[0].FightMove = pkmn.Moves[move];
                            actions[0].FightTargets = PBETarget.FoeCenter;
                            valid = battle.SelectActionsIfValid(true, actions);
                        } while (!valid);
                        do
                        {
                            pkmn = battle.Teams[1].ActiveBattlers[0];
                            move = PBEUtils.RNG.Next(0, pkmn.Moves.Length);
                            actions[0].PokemonId = pkmn.Id;
                            actions[0].Decision = PBEDecision.Fight;
                            actions[0].FightMove = pkmn.Moves[move];
                            actions[0].FightTargets = PBETarget.FoeCenter;
                            valid = battle.SelectActionsIfValid(false, actions);
                        } while (!valid);
                    }
                    break;
                case PBEBattleState.WaitingForSwitchIns:
                    {
                        var switches = new Tuple<byte, PBEFieldPosition>[1];
                        if (battle.Teams[0].SwitchInsRequired > 0)
                        {
                            PBEPokemon pkmn = battle.Teams[0].Party.First(p => p.FieldPosition == PBEFieldPosition.None && p.HP > 0);
                            switches[0] = Tuple.Create(pkmn.Id, PBEFieldPosition.Center);
                            battle.SelectSwitchesIfValid(true, switches);
                        }
                        if (battle.Teams[1].SwitchInsRequired > 0)
                        {
                            PBEPokemon pkmn = battle.Teams[1].Party.First(p => p.FieldPosition == PBEFieldPosition.None && p.HP > 0);
                            switches[0] = Tuple.Create(pkmn.Id, PBEFieldPosition.Center);
                            battle.SelectSwitchesIfValid(false, switches);
                        }
                    }
                    break;
            }
        }
    }
}
