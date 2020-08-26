using Kermalis.PokemonBattleEngine.Battle;
using Kermalis.PokemonBattleEngine.Packets;
using Kermalis.PokemonBattleEngineClient.Views;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Kermalis.PokemonBattleEngineClient.Clients
{
    internal abstract class BattleClient : IDisposable
    {
        protected const int WaitMilliseconds = 1750;

        public string Name { get; }
        public abstract PBEBattle Battle { get; }
        public abstract PBETrainer Trainer { get; }
        public abstract BattleView BattleView { get; }
        public abstract bool HideNonOwned { get; }

        protected BattleClient(string name)
        {
            Name = name;
        }

        public bool ShouldUseKnownInfo(PBETrainer pkmnTrainer)
        {
            return pkmnTrainer != Trainer && HideNonOwned;
        }

        public abstract void Dispose();

        protected void ShowAllPokemon()
        {
            foreach (PBEBattlePokemon pkmn in Battle.ActiveBattlers)
            {
                BattleView.Field.ShowPokemon(pkmn);
            }
        }

        #region Actions
        private readonly List<PBEBattlePokemon> _actions = new List<PBEBattlePokemon>(3);
        public List<PBEBattlePokemon> StandBy { get; } = new List<PBEBattlePokemon>(3);
        public void ActionsLoop(bool begin)
        {
            if (begin)
            {
                foreach (PBEBattlePokemon pkmn in Trainer.Party)
                {
                    pkmn.TurnAction = null;
                }
                _actions.Clear();
                _actions.AddRange(Trainer.ActiveBattlersOrdered);
                StandBy.Clear();
            }
            int i = _actions.FindIndex(p => p.TurnAction == null);
            if (i == -1)
            {
                OnActionsReady(_actions.Select(p => p.TurnAction).ToArray());
            }
            else
            {
                BattleView.AddMessage($"What will {_actions[i].Nickname} do?", messageLog: false);
                BattleView.Actions.DisplayActions(_actions[i]);
            }
        }
        protected abstract void OnActionsReady(PBETurnAction[] acts);

        public List<PBESwitchIn> Switches { get; } = new List<PBESwitchIn>(3);
        protected byte _switchesRequired;
        public List<PBEFieldPosition> PositionStandBy { get; } = new List<PBEFieldPosition>(3);
        public void SwitchesLoop(bool begin)
        {
            if (begin)
            {
                Switches.Clear();
                StandBy.Clear();
                PositionStandBy.Clear();
            }
            else
            {
                _switchesRequired--;
            }
            if (_switchesRequired == 0)
            {
                OnSwitchesReady();
            }
            else
            {
                BattleView.AddMessage($"You must send in {_switchesRequired} Pokémon.", messageLog: false);
                BattleView.Actions.DisplaySwitches();
            }
        }
        protected abstract void OnSwitchesReady();
        #endregion

        #region Automatic packet processing
        // Returns true if the next packet should be run immediately
        protected virtual bool ProcessPacket(IPBEPacket packet)
        {
            switch (packet)
            {
                case PBEMoveLockPacket _:
                case PBEMovePPChangedPacket _:
                case PBEIllusionPacket _:
                case PBETransformPacket _:
                case PBEBattlePacket _:
                case PBEActionsRequestPacket _:
                case PBESwitchInRequestPacket _: return true;
                case PBEPkmnFaintedPacket pfp:
                {
                    PBEBattlePokemon pokemon = pfp.PokemonTrainer.TryGetPokemon(pfp.Pokemon);
                    BattleView.Field.HidePokemon(pokemon, pfp.OldPosition);
                    break;
                }
                case PBEPkmnFaintedPacket_Hidden pfph:
                {
                    PBEBattlePokemon pokemon = pfph.PokemonTrainer.TryGetPokemon(pfph.OldPosition);
                    BattleView.Field.HidePokemon(pokemon, pfph.OldPosition);
                    break;
                }
                case IPBEPkmnFormChangedPacket pfcp:
                {
                    PBEBattlePokemon pokemon = pfcp.PokemonTrainer.TryGetPokemon(pfcp.Pokemon);
                    BattleView.Field.UpdatePokemon(pokemon, false, true);
                    break;
                }
                case IPBEPkmnHPChangedPacket phcp:
                {
                    PBEBattlePokemon pokemon = phcp.PokemonTrainer.TryGetPokemon(phcp.Pokemon);
                    BattleView.Field.UpdatePokemon(pokemon, true, false);
                    break;
                }
                case IPBEPkmnSwitchInPacket psip:
                {
                    if (!psip.Forced)
                    {
                        foreach (IPBEPkmnSwitchInInfo_Hidden info in psip.SwitchIns)
                        {
                            BattleView.Field.ShowPokemon(psip.Trainer.TryGetPokemon(info.FieldPosition));
                        }
                    }
                    break;
                }
                case PBEPkmnSwitchOutPacket psop:
                {
                    PBEBattlePokemon pokemon = psop.PokemonTrainer.TryGetPokemon(psop.Pokemon);
                    BattleView.Field.HidePokemon(pokemon, psop.OldPosition);
                    break;
                }
                case PBEPkmnSwitchOutPacket_Hidden psoph:
                {
                    PBEBattlePokemon pokemon = psoph.PokemonTrainer.TryGetPokemon(psoph.OldPosition);
                    BattleView.Field.HidePokemon(pokemon, psoph.OldPosition);
                    break;
                }
                case PBEStatus1Packet s1p:
                {
                    PBEBattlePokemon status1Receiver = s1p.Status1ReceiverTrainer.TryGetPokemon(s1p.Status1Receiver);
                    BattleView.Field.UpdatePokemon(status1Receiver, true, false);
                    break;
                }
                case PBEStatus2Packet s2p:
                {
                    PBEBattlePokemon status2Receiver = s2p.Status2ReceiverTrainer.TryGetPokemon(s2p.Status2Receiver);
                    switch (s2p.Status2)
                    {
                        case PBEStatus2.Airborne: BattleView.Field.UpdatePokemon(status2Receiver, false, true); break;
                        case PBEStatus2.Disguised:
                        {
                            switch (s2p.StatusAction)
                            {
                                case PBEStatusAction.Ended: BattleView.Field.UpdatePokemon(status2Receiver, true, true); break;
                            }
                            break;
                        }
                        case PBEStatus2.ShadowForce: BattleView.Field.UpdatePokemon(status2Receiver, false, true); break;
                        case PBEStatus2.Substitute:
                        {
                            switch (s2p.StatusAction)
                            {
                                case PBEStatusAction.Added:
                                case PBEStatusAction.Ended: BattleView.Field.UpdatePokemon(status2Receiver, false, true); break;
                            }
                            break;
                        }
                        case PBEStatus2.Transformed:
                        {
                            switch (s2p.StatusAction)
                            {
                                case PBEStatusAction.Added: BattleView.Field.UpdatePokemon(status2Receiver, false, true); break;
                            }
                            break;
                        }
                        case PBEStatus2.Underground: BattleView.Field.UpdatePokemon(status2Receiver, false, true); break;
                        case PBEStatus2.Underwater: BattleView.Field.UpdatePokemon(status2Receiver, false, true); break;
                    }
                    break;
                }
                case PBEWeatherPacket wp:
                {
                    switch (wp.WeatherAction)
                    {
                        case PBEWeatherAction.Added:
                        case PBEWeatherAction.Ended: BattleView.Field.UpdateWeather(); break;
                        case PBEWeatherAction.CausedDamage: break;
                    }
                    break;
                }
                case IPBEWildPkmnAppearedPacket wpap:
                {
                    foreach (IPBEPkmnAppearedInfo_Hidden info in wpap.Pokemon)
                    {
                        BattleView.Field.ShowPokemon(Battle.Teams[1].Trainers[0].TryGetPokemon(info.FieldPosition));
                    }
                    break;
                }
                case IPBEAutoCenterPacket acp:
                {
                    PBEBattlePokemon pokemon0 = acp is IPBEAutoCenterPacket_0 acp0
                        ? acp.Pokemon0Trainer.TryGetPokemon(acp0.Pokemon0)
                        : acp.Pokemon0Trainer.TryGetPokemon(acp.Pokemon0OldPosition);
                    PBEBattlePokemon pokemon1 = acp is IPBEAutoCenterPacket_1 acp1
                        ? acp.Pokemon1Trainer.TryGetPokemon(acp1.Pokemon1)
                        : acp.Pokemon1Trainer.TryGetPokemon(acp.Pokemon1OldPosition);
                    BattleView.Field.MovePokemon(pokemon0, acp.Pokemon0OldPosition);
                    BattleView.Field.MovePokemon(pokemon1, acp.Pokemon1OldPosition);
                    break;
                }
                case PBETurnBeganPacket tbp:
                {
                    BattleView.AddMessage($"Turn {tbp.TurnNumber}", messageBox: false);
                    return true;
                }
            }
            string message = PBEBattle.GetDefaultMessage(Battle, packet, showRawHP: !HideNonOwned, userTrainer: Trainer);
            if (string.IsNullOrEmpty(message))
            {
                return true;
            }
            BattleView.AddMessage(message);
            return false;
        }
        #endregion
    }
}
