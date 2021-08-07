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
        public abstract PBETrainer? Trainer { get; }
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

        #region Automatic packet processing
        // Returns true if the next packet should be run immediately
        protected virtual bool ProcessPacket(IPBEPacket packet)
        {
            switch (packet)
            {
                case PBEPkmnEXPChangedPacket _:
                case PBEMoveLockPacket _:
                case PBEMovePPChangedPacket _:
                case PBEIllusionPacket _:
                case PBETransformPacket _:
                case PBEBattlePacket _:
                case PBEActionsRequestPacket _:
                case PBESwitchInRequestPacket _: return true;
                /*case PBEPkmnEXPChangedPacket pecp:
                {
                    PBEBattlePokemon pokemon = pecp.PokemonTrainer.GetPokemon(pecp.Pokemon);
                    if (pokemon.FieldPosition != PBEFieldPosition.None)
                    {
                        BattleView.Field.UpdatePokemon(pokemon, true, false);
                    }
                    break;
                }*/ // Commented out because we don't have EXP bars
                case PBEPkmnFaintedPacket pfp:
                {
                    PBEBattlePokemon pokemon = pfp.PokemonTrainer.GetPokemon(pfp.Pokemon);
                    BattleView.Field.HidePokemon(pokemon, pfp.OldPosition);
                    break;
                }
                case PBEPkmnFaintedPacket_Hidden pfph:
                {
                    PBEBattlePokemon pokemon = pfph.PokemonTrainer.GetPokemon(pfph.OldPosition);
                    BattleView.Field.HidePokemon(pokemon, pfph.OldPosition);
                    break;
                }
                case IPBEPkmnFormChangedPacket pfcp:
                {
                    PBEBattlePokemon pokemon = pfcp.PokemonTrainer.GetPokemon(pfcp.Pokemon);
                    BattleView.Field.UpdatePokemon(pokemon, true, true);
                    break;
                }
                case IPBEPkmnHPChangedPacket phcp:
                {
                    PBEBattlePokemon pokemon = phcp.PokemonTrainer.GetPokemon(phcp.Pokemon);
                    BattleView.Field.UpdatePokemon(pokemon, true, false);
                    break;
                }
                case PBEPkmnLevelChangedPacket plcp:
                {
                    PBEBattlePokemon pokemon = plcp.PokemonTrainer.GetPokemon(plcp.Pokemon);
                    if (pokemon.FieldPosition != PBEFieldPosition.None)
                    {
                        BattleView.Field.UpdatePokemon(pokemon, true, false);
                    }
                    break;
                }
                case IPBEPkmnSwitchInPacket psip:
                {
                    if (!psip.Forced)
                    {
                        foreach (IPBEPkmnSwitchInInfo_Hidden info in psip.SwitchIns)
                        {
                            BattleView.Field.ShowPokemon(psip.Trainer.GetPokemon(info.FieldPosition));
                        }
                    }
                    break;
                }
                case PBEPkmnSwitchOutPacket psop:
                {
                    PBEBattlePokemon pokemon = psop.PokemonTrainer.GetPokemon(psop.Pokemon);
                    BattleView.Field.HidePokemon(pokemon, psop.OldPosition);
                    break;
                }
                case PBEPkmnSwitchOutPacket_Hidden psoph:
                {
                    PBEBattlePokemon pokemon = psoph.PokemonTrainer.GetPokemon(psoph.OldPosition);
                    BattleView.Field.HidePokemon(pokemon, psoph.OldPosition);
                    break;
                }
                case PBEStatus1Packet s1p:
                {
                    PBEBattlePokemon status1Receiver = s1p.Status1ReceiverTrainer.GetPokemon(s1p.Status1Receiver);
                    BattleView.Field.UpdatePokemon(status1Receiver, true, false);
                    break;
                }
                case PBEStatus2Packet s2p:
                {
                    PBEBattlePokemon status2Receiver = s2p.Status2ReceiverTrainer.GetPokemon(s2p.Status2Receiver);
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
                    }
                    break;
                }
                case IPBEWildPkmnAppearedPacket wpap:
                {
                    PBETrainer wildTrainer = Battle.Teams[1].Trainers[0];
                    foreach (IPBEPkmnAppearedInfo_Hidden info in wpap.Pokemon)
                    {
                        BattleView.Field.ShowPokemon(wildTrainer.GetPokemon(info.FieldPosition));
                    }
                    break;
                }
                case IPBEAutoCenterPacket acp:
                {
                    PBEBattlePokemon pokemon0 = acp is IPBEAutoCenterPacket_0 acp0
                        ? acp.Pokemon0Trainer.GetPokemon(acp0.Pokemon0)
                        : acp.Pokemon0Trainer.GetPokemon(acp.Pokemon0OldPosition);
                    PBEBattlePokemon pokemon1 = acp is IPBEAutoCenterPacket_1 acp1
                        ? acp.Pokemon1Trainer.GetPokemon(acp1.Pokemon1)
                        : acp.Pokemon1Trainer.GetPokemon(acp.Pokemon1OldPosition);
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
            string? message = PBEBattle.GetDefaultMessage(Battle, packet, showRawHP: !HideNonOwned, userTrainer: Trainer);
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
