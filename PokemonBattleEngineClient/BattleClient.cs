using Kermalis.PokemonBattleEngine.AI;
using Kermalis.PokemonBattleEngine.Battle;
using Kermalis.PokemonBattleEngine.Data;
using Kermalis.PokemonBattleEngine.Packets;
using Kermalis.PokemonBattleEngine.Utils;
using Kermalis.PokemonBattleEngineClient.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Kermalis.PokemonBattleEngineClient
{
    internal abstract class BattleClient : IDisposable
    {
        protected const int WaitMilliseconds = 2000;
        protected enum ClientMode : byte
        {
            Online,
            Replay,
            SinglePlayer
        }

        public PBEBattle Battle { get; }
        public BattleView BattleView { get; }
        private readonly ClientMode _mode;
        public int BattleId { get; protected set; } = int.MaxValue;
        public PBETeam Team { get; protected set; }
        public bool ShowEverything0 { get; protected set; }
        public bool ShowEverything1 { get; protected set; }

        protected BattleClient(PBEBattle battle, ClientMode mode)
        {
            Battle = battle;
            BattleView = new BattleView(this);
            _mode = mode;
        }

        private readonly List<PBEBattlePokemon> _actions = new List<PBEBattlePokemon>(3);
        public List<PBEBattlePokemon> StandBy { get; } = new List<PBEBattlePokemon>(3);
        public void ActionsLoop(bool begin)
        {
            if (begin)
            {
                foreach (PBEBattlePokemon pkmn in Team.Party)
                {
                    pkmn.TurnAction = null;
                }
                _actions.Clear();
                _actions.AddRange(Team.ActiveBattlers);
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
        private byte switchesRequired;
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
                switchesRequired--;
            }
            if (switchesRequired == 0)
            {
                OnSwitchesReady();
            }
            else
            {
                BattleView.AddMessage($"You must send in {switchesRequired} Pokémon.", messageLog: false);
                BattleView.Actions.DisplaySwitches();
            }
        }
        protected abstract void OnSwitchesReady();

        public virtual void Dispose()
        {
            _stopPacketThread = true;
        }

        #region Automatic packet processing

        private int _currentPacket = -1;
        private Thread _packetThread;
        private readonly object _packetThreadLockObj = new object();
        private bool _plsStartPacketThreadForMe = false;
        private bool _stopPacketThread = false;

        private void CreateThread__Unsafe()
        {
            _packetThread = new Thread(PacketThread) { Name = "Packet Thread" };
            _packetThread.Start();
        }
        protected void StartPacketThread()
        {
            lock (_packetThreadLockObj)
            {
                _stopPacketThread = false;
                if (_packetThread == null)
                {
                    CreateThread__Unsafe();
                }
                else
                {
                    _plsStartPacketThreadForMe = true;
                }
            }
        }
        private void PacketThread()
        {
            while (!_stopPacketThread && _currentPacket < Battle.Events.Count - 1)
            {
                _plsStartPacketThreadForMe = false;
                _currentPacket++;
                if (!ProcessPacket(Battle.Events[_currentPacket]))
                {
                    Thread.Sleep(WaitMilliseconds);
                }
            }
            lock (_packetThreadLockObj)
            {
                if (_plsStartPacketThreadForMe)
                {
                    CreateThread__Unsafe();
                }
                else
                {
                    _packetThread = null;
                }
            }
        }
        // Returns true if the next packet should be run immediately
        protected bool ProcessPacket(IPBEPacket packet)
        {
            string NameForTrainer(PBEBattlePokemon pkmn, bool firstLetterCapitalized)
            {
                if (pkmn == null)
                {
                    return string.Empty;
                }
                if (BattleId >= 2)
                {
                    return $"{pkmn.Team.TrainerName}'s {pkmn.KnownNickname}";
                }
                else
                {
                    string prefix;
                    if (firstLetterCapitalized)
                    {
                        if (pkmn.Team.Id == BattleId)
                        {
                            prefix = string.Empty;
                        }
                        else
                        {
                            prefix = "The foe's ";
                        }
                    }
                    else
                    {
                        if (pkmn.Team.Id == BattleId)
                        {
                            prefix = string.Empty;
                        }
                        else
                        {
                            prefix = "the foe's ";
                        }
                    }
                    return prefix + pkmn.KnownNickname;
                }
            }

            switch (packet)
            {
                case PBEAbilityPacket ap:
                {
                    PBEBattlePokemon abilityOwner = ap.AbilityOwnerTeam.TryGetPokemon(ap.AbilityOwner),
                            pokemon2 = ap.Pokemon2Team.TryGetPokemon(ap.Pokemon2);
                    if (_mode != ClientMode.SinglePlayer)
                    {
                        abilityOwner.Ability = abilityOwner.KnownAbility = ap.Ability;
                    }
                    bool abilityOwnerCaps = true,
                            pokemon2Caps = true;
                    string message;
                    switch (ap.Ability)
                    {
                        case PBEAbility.AirLock:
                        case PBEAbility.CloudNine:
                        {
                            switch (ap.AbilityAction)
                            {
                                case PBEAbilityAction.Weather: message = "{0}'s {2} causes the effects of weather to disappear!"; break;
                                default: throw new ArgumentOutOfRangeException(nameof(ap.AbilityAction));
                            }
                            break;
                        }
                        case PBEAbility.Anticipation:
                        {
                            switch (ap.AbilityAction)
                            {
                                case PBEAbilityAction.Announced: message = "{0}'s {2} made it shudder!"; break;
                                default: throw new ArgumentOutOfRangeException(nameof(ap.AbilityAction));
                            }
                            break;
                        }
                        case PBEAbility.BadDreams:
                        {
                            switch (ap.AbilityAction)
                            {
                                case PBEAbilityAction.Damage: message = "{1} is tormented by {0}'s {2}!"; abilityOwnerCaps = false; break;
                                default: throw new ArgumentOutOfRangeException(nameof(ap.AbilityAction));
                            }
                            break;
                        }
                        case PBEAbility.ClearBody:
                        case PBEAbility.WhiteSmoke:
                        {
                            switch (ap.AbilityAction)
                            {
                                case PBEAbilityAction.Stats: message = "{0}'s {2} prevents stat reduction!"; break;
                                default: throw new ArgumentOutOfRangeException(nameof(ap.AbilityAction));
                            }
                            break;
                        }
                        case PBEAbility.ColorChange:
                        case PBEAbility.FlowerGift:
                        case PBEAbility.Forecast:
                        case PBEAbility.Imposter:
                        {
                            switch (ap.AbilityAction)
                            {
                                case PBEAbilityAction.ChangedAppearance: message = "{0}'s {2} activated!"; break;
                                default: throw new ArgumentOutOfRangeException(nameof(ap.AbilityAction));
                            }
                            break;
                        }
                        case PBEAbility.CuteCharm:
                        case PBEAbility.EffectSpore:
                        case PBEAbility.FlameBody:
                        case PBEAbility.Healer:
                        case PBEAbility.PoisonPoint:
                        case PBEAbility.ShedSkin:
                        case PBEAbility.Static:
                        {
                            switch (ap.AbilityAction)
                            {
                                case PBEAbilityAction.ChangedStatus: message = "{0}'s {2} activated!"; break;
                                default: throw new ArgumentOutOfRangeException(nameof(ap.AbilityAction));
                            }
                            break;
                        }
                        case PBEAbility.Download:
                        {
                            switch (ap.AbilityAction)
                            {
                                case PBEAbilityAction.Stats: message = "{0}'s {2} activated!"; break;
                                default: throw new ArgumentOutOfRangeException(nameof(ap.AbilityAction));
                            }
                            break;
                        }
                        case PBEAbility.Drizzle:
                        case PBEAbility.Drought:
                        case PBEAbility.SandStream:
                        case PBEAbility.SnowWarning:
                        {
                            switch (ap.AbilityAction)
                            {
                                case PBEAbilityAction.Weather: message = "{0}'s {2} activated!"; break;
                                default: throw new ArgumentOutOfRangeException(nameof(ap.AbilityAction));
                            }
                            break;
                        }
                        case PBEAbility.IceBody:
                        case PBEAbility.RainDish:
                        {
                            switch (ap.AbilityAction)
                            {
                                case PBEAbilityAction.RestoredHP: message = "{0}'s {2} activated!"; break;
                                default: throw new ArgumentOutOfRangeException(nameof(ap.AbilityAction));
                            }
                            break;
                        }
                        case PBEAbility.Illusion:
                        {
                            switch (ap.AbilityAction)
                            {
                                case PBEAbilityAction.ChangedAppearance: return true;
                                default: throw new ArgumentOutOfRangeException(nameof(ap.AbilityAction));
                            }
                        }
                        case PBEAbility.Immunity:
                        case PBEAbility.Insomnia:
                        case PBEAbility.Limber:
                        case PBEAbility.MagmaArmor:
                        case PBEAbility.Oblivious:
                        case PBEAbility.OwnTempo:
                        case PBEAbility.VitalSpirit:
                        case PBEAbility.WaterVeil:
                        {
                            switch (ap.AbilityAction)
                            {
                                case PBEAbilityAction.ChangedStatus:
                                case PBEAbilityAction.PreventedStatus: message = "{0}'s {2} activated!"; break;
                                default: throw new ArgumentOutOfRangeException(nameof(ap.AbilityAction));
                            }
                            break;
                        }
                        case PBEAbility.IronBarbs:
                        case PBEAbility.Justified:
                        case PBEAbility.Levitate:
                        case PBEAbility.Mummy:
                        case PBEAbility.Rattled:
                        case PBEAbility.RoughSkin:
                        case PBEAbility.SolarPower:
                        case PBEAbility.Sturdy:
                        case PBEAbility.WeakArmor:
                        case PBEAbility.WonderGuard:
                        {
                            switch (ap.AbilityAction)
                            {
                                case PBEAbilityAction.Damage: message = "{0}'s {2} activated!"; break;
                                default: throw new ArgumentOutOfRangeException(nameof(ap.AbilityAction));
                            }
                            break;
                        }
                        case PBEAbility.LeafGuard:
                        {
                            switch (ap.AbilityAction)
                            {
                                case PBEAbilityAction.PreventedStatus: message = "{0}'s {2} activated!"; break;
                                default: throw new ArgumentOutOfRangeException(nameof(ap.AbilityAction));
                            }
                            break;
                        }
                        case PBEAbility.LiquidOoze:
                        {
                            switch (ap.AbilityAction)
                            {
                                case PBEAbilityAction.Damage: message = "{1} sucked up the liquid ooze!"; break;
                                default: throw new ArgumentOutOfRangeException(nameof(ap.AbilityAction));
                            }
                            break;
                        }
                        case PBEAbility.MoldBreaker:
                        {
                            switch (ap.AbilityAction)
                            {
                                case PBEAbilityAction.Announced: message = "{0} breaks the mold!"; break;
                                default: throw new ArgumentOutOfRangeException(nameof(ap.AbilityAction));
                            }
                            break;
                        }
                        case PBEAbility.Moody:
                        case PBEAbility.SpeedBoost:
                        case PBEAbility.Steadfast:
                        {
                            switch (ap.AbilityAction)
                            {
                                case PBEAbilityAction.Stats: message = "{0}'s {2} activated!"; break;
                                default: throw new ArgumentOutOfRangeException(nameof(ap.AbilityAction));
                            }
                            break;
                        }
                        case PBEAbility.SlowStart:
                        {
                            switch (ap.AbilityAction)
                            {
                                case PBEAbilityAction.Announced: message = "{0} can't get it going!"; break;
                                case PBEAbilityAction.SlowStart_Ended: message = "{0} finally got its act together!"; break;
                                default: throw new ArgumentOutOfRangeException(nameof(ap.AbilityAction));
                            }
                            break;
                        }
                        case PBEAbility.Teravolt:
                        {
                            switch (ap.AbilityAction)
                            {
                                case PBEAbilityAction.Announced: message = "{0} is radiating a bursting aura!"; break;
                                default: throw new ArgumentOutOfRangeException(nameof(ap.AbilityAction));
                            }
                            break;
                        }
                        case PBEAbility.Turboblaze:
                        {
                            switch (ap.AbilityAction)
                            {
                                case PBEAbilityAction.Announced: message = "{0} is radiating a blazing aura!"; break;
                                default: throw new ArgumentOutOfRangeException(nameof(ap.AbilityAction));
                            }
                            break;
                        }
                        default: throw new ArgumentOutOfRangeException(nameof(ap.Ability));
                    }
                    BattleView.AddMessage(string.Format(message, NameForTrainer(abilityOwner, abilityOwnerCaps), NameForTrainer(pokemon2, pokemon2Caps), PBELocalizedString.GetAbilityName(ap.Ability)));
                    return false;
                }
                case PBEAbilityReplacedPacket arp:
                {
                    PBEBattlePokemon abilityOwner = arp.AbilityOwnerTeam.TryGetPokemon(arp.AbilityOwner);
                    if (_mode != ClientMode.SinglePlayer)
                    {
                        abilityOwner.Ability = abilityOwner.KnownAbility = arp.NewAbility;
                    }
                    string message;
                    switch (arp.NewAbility)
                    {
                        case PBEAbility.None: message = "{0}'s {1} was suppressed!"; break;
                        default: message = "{0}'s {1} was changed to {2}!"; break;
                    }
                    BattleView.AddMessage(string.Format(message, NameForTrainer(abilityOwner, true), arp.OldAbility.HasValue ? PBELocalizedString.GetAbilityName(arp.OldAbility.Value).ToString() : "Ability", PBELocalizedString.GetAbilityName(arp.NewAbility)));
                    return false;
                }
                case PBEBattleStatusPacket bsp:
                {
                    if (_mode != ClientMode.SinglePlayer)
                    {
                        switch (bsp.BattleStatusAction)
                        {
                            case PBEBattleStatusAction.Added: Battle.BattleStatus |= bsp.BattleStatus; break;
                            case PBEBattleStatusAction.Cleared:
                            case PBEBattleStatusAction.Ended: Battle.BattleStatus &= ~bsp.BattleStatus; break;
                            default: throw new ArgumentOutOfRangeException(nameof(bsp.BattleStatusAction));
                        }
                    }
                    string message;
                    switch (bsp.BattleStatus)
                    {
                        case PBEBattleStatus.TrickRoom:
                        {
                            switch (bsp.BattleStatusAction)
                            {
                                case PBEBattleStatusAction.Added: message = "The dimensions were twisted!"; break;
                                case PBEBattleStatusAction.Cleared:
                                case PBEBattleStatusAction.Ended: message = "The twisted dimensions returned to normal!"; break;
                                default: throw new ArgumentOutOfRangeException(nameof(bsp.BattleStatusAction));
                            }
                            break;
                        }
                        default: throw new ArgumentOutOfRangeException(nameof(bsp.BattleStatus));
                    }
                    BattleView.AddMessage(message);
                    return false;
                }
                case PBEHazePacket _:
                {
                    if (_mode != ClientMode.SinglePlayer)
                    {
                        foreach (PBEBattlePokemon pkmn in Battle.ActiveBattlers)
                        {
                            pkmn.ClearStatChanges();
                        }
                    }
                    BattleView.AddMessage("All stat changes were eliminated!");
                    return false;
                }
                case PBEIllusionPacket ilp:
                {
                    PBEBattlePokemon pokemon = ilp.PokemonTeam.TryGetPokemon(ilp.Pokemon);
                    if (_mode != ClientMode.SinglePlayer)
                    {
                        pokemon.DisguisedAsPokemon = null;
                        pokemon.Ability = pokemon.KnownAbility = PBEAbility.Illusion;
                        pokemon.Gender = pokemon.KnownGender = ilp.ActualGender;
                        pokemon.Nickname = pokemon.KnownNickname = ilp.ActualNickname;
                        pokemon.Shiny = pokemon.KnownShiny = ilp.ActualShiny;
                        pokemon.OriginalSpecies = pokemon.Species = pokemon.KnownSpecies = ilp.ActualSpecies;
                        pokemon.Form = pokemon.KnownForm = ilp.ActualForm;
                        pokemon.Type1 = pokemon.KnownType1 = ilp.ActualType1;
                        pokemon.Type2 = pokemon.KnownType2 = ilp.ActualType2;
                        pokemon.Weight = pokemon.KnownWeight = ilp.ActualWeight;
                    }
                    return true;
                }
                case PBEItemPacket ip:
                {
                    PBEBattlePokemon itemHolder = ip.ItemHolderTeam.TryGetPokemon(ip.ItemHolder),
                            pokemon2 = ip.Pokemon2Team.TryGetPokemon(ip.Pokemon2);
                    if (_mode != ClientMode.SinglePlayer)
                    {
                        switch (ip.ItemAction)
                        {
                            case PBEItemAction.ChangedStatus:
                            case PBEItemAction.Damage:
                            case PBEItemAction.RestoredHP: itemHolder.Item = itemHolder.KnownItem = ip.Item; break;
                            case PBEItemAction.Consumed: itemHolder.Item = itemHolder.KnownItem = PBEItem.None; break;
                            default: throw new ArgumentOutOfRangeException(nameof(ip.ItemAction));
                        }
                    }
                    bool itemHolderCaps = true,
                            pokemon2Caps = false;
                    string message;
                    switch (ip.Item)
                    {
                        case PBEItem.AguavBerry:
                        case PBEItem.BerryJuice:
                        case PBEItem.FigyBerry:
                        case PBEItem.IapapaBerry:
                        case PBEItem.MagoBerry:
                        case PBEItem.OranBerry:
                        case PBEItem.SitrusBerry:
                        case PBEItem.WikiBerry:
                        {
                            switch (ip.ItemAction)
                            {
                                case PBEItemAction.Consumed: message = "{0} restored its health using its {2}!"; break;
                                default: throw new ArgumentOutOfRangeException(nameof(ip.ItemAction));
                            }
                            break;
                        }
                        case PBEItem.ApicotBerry:
                        case PBEItem.GanlonBerry:
                        case PBEItem.LiechiBerry:
                        case PBEItem.PetayaBerry:
                        case PBEItem.SalacBerry:
                        case PBEItem.StarfBerry:
                        {
                            switch (ip.ItemAction)
                            {
                                case PBEItemAction.Consumed: message = "{0} used its {2}!"; break;
                                default: throw new ArgumentOutOfRangeException(nameof(ip.ItemAction));
                            }
                            break;
                        }
                        case PBEItem.BugGem:
                        case PBEItem.DarkGem:
                        case PBEItem.DragonGem:
                        case PBEItem.ElectricGem:
                        case PBEItem.FightingGem:
                        case PBEItem.FireGem:
                        case PBEItem.FlyingGem:
                        case PBEItem.GhostGem:
                        case PBEItem.GrassGem:
                        case PBEItem.GroundGem:
                        case PBEItem.IceGem:
                        case PBEItem.NormalGem:
                        case PBEItem.PoisonGem:
                        case PBEItem.PsychicGem:
                        case PBEItem.RockGem:
                        case PBEItem.SteelGem:
                        case PBEItem.WaterGem:
                        {
                            switch (ip.ItemAction)
                            {
                                case PBEItemAction.Consumed: message = "The {2} strengthened {0}'s power!"; itemHolderCaps = false; break;
                                default: throw new ArgumentOutOfRangeException(nameof(ip.ItemAction));
                            }
                            break;
                        }
                        case PBEItem.BlackSludge:
                        {
                            switch (ip.ItemAction)
                            {
                                case PBEItemAction.Damage: message = "{0} is hurt by its {2}!"; break;
                                case PBEItemAction.RestoredHP: message = "{0} restored a little HP using its {2}!"; break;
                                default: throw new ArgumentOutOfRangeException(nameof(ip.ItemAction));
                            }
                            break;
                        }
                        case PBEItem.DestinyKnot:
                        {
                            switch (ip.ItemAction)
                            {
                                case PBEItemAction.ChangedStatus: message = "{0}'s {2} activated!"; break;
                                default: throw new ArgumentOutOfRangeException(nameof(ip.ItemAction));
                            }
                            break;
                        }
                        case PBEItem.FlameOrb:
                        {
                            switch (ip.ItemAction)
                            {
                                case PBEItemAction.ChangedStatus: message = "{0} was burned by its {2}!"; break;
                                default: throw new ArgumentOutOfRangeException(nameof(ip.ItemAction));
                            }
                            break;
                        }
                        case PBEItem.FocusBand:
                        {
                            switch (ip.ItemAction)
                            {
                                case PBEItemAction.Damage: message = "{0} hung on using its {2}!"; break;
                                default: throw new ArgumentOutOfRangeException(nameof(ip.ItemAction));
                            }
                            break;
                        }
                        case PBEItem.FocusSash:
                        {
                            switch (ip.ItemAction)
                            {
                                case PBEItemAction.Consumed: message = "{0} hung on using its {2}!"; break;
                                default: throw new ArgumentOutOfRangeException(nameof(ip.ItemAction));
                            }
                            break;
                        }
                        case PBEItem.Leftovers:
                        {
                            switch (ip.ItemAction)
                            {
                                case PBEItemAction.RestoredHP: message = "{0} restored a little HP using its {2}!"; break;
                                default: throw new ArgumentOutOfRangeException(nameof(ip.ItemAction));
                            }
                            break;
                        }
                        case PBEItem.LifeOrb:
                        {
                            switch (ip.ItemAction)
                            {
                                case PBEItemAction.Damage: message = "{0} is hurt by its {2}!"; break;
                                default: throw new ArgumentOutOfRangeException(nameof(ip.ItemAction));
                            }
                            break;
                        }
                        case PBEItem.PowerHerb:
                        {
                            switch (ip.ItemAction)
                            {
                                case PBEItemAction.Consumed: message = "{0} became fully charged due to its {2}!"; break;
                                default: throw new ArgumentOutOfRangeException(nameof(ip.ItemAction));
                            }
                            break;
                        }
                        case PBEItem.RockyHelmet:
                        {
                            switch (ip.ItemAction)
                            {
                                case PBEItemAction.Damage: message = "{1} was hurt by the {2}!"; pokemon2Caps = true; break;
                                default: throw new ArgumentOutOfRangeException(nameof(ip.ItemAction));
                            }
                            break;
                        }
                        case PBEItem.ToxicOrb:
                        {
                            switch (ip.ItemAction)
                            {
                                case PBEItemAction.ChangedStatus: message = "{0} was badly poisoned by its {2}!"; break;
                                default: throw new ArgumentOutOfRangeException(nameof(ip.ItemAction));
                            }
                            break;
                        }
                        default: throw new ArgumentOutOfRangeException(nameof(ip.Item));
                    }
                    BattleView.AddMessage(string.Format(message, NameForTrainer(itemHolder, itemHolderCaps), NameForTrainer(pokemon2, pokemon2Caps), PBELocalizedString.GetItemName(ip.Item)));
                    return false;
                }
                case PBEMoveCritPacket mcp:
                {
                    PBEBattlePokemon victim = mcp.VictimTeam.TryGetPokemon(mcp.Victim);
                    BattleView.AddMessage(string.Format("A critical hit on {0}!", NameForTrainer(victim, false)));
                    return false;
                }
                case PBEMoveLockPacket mlp:
                {
                    if (_mode != ClientMode.SinglePlayer)
                    {
                        PBEBattlePokemon moveUser = mlp.MoveUserTeam.TryGetPokemon(mlp.MoveUser);
                        switch (mlp.MoveLockType)
                        {
                            case PBEMoveLockType.ChoiceItem: moveUser.ChoiceLockedMove = mlp.LockedMove; break;
                            case PBEMoveLockType.Temporary: moveUser.TempLockedMove = mlp.LockedMove; break;
                            default: throw new ArgumentOutOfRangeException(nameof(mlp.MoveLockType));
                        }
                        if (mlp.LockedTargets.HasValue)
                        {
                            moveUser.TempLockedTargets = mlp.LockedTargets.Value;
                        }
                    }
                    return true;
                }
                case PBEMoveMissedPacket mmp:
                {
                    PBEBattlePokemon moveUser = mmp.MoveUserTeam.TryGetPokemon(mmp.MoveUser),
                            pokemon2 = mmp.Pokemon2Team.TryGetPokemon(mmp.Pokemon2);
                    BattleView.AddMessage(string.Format("{0}'s attack missed {1}!", NameForTrainer(moveUser, true), NameForTrainer(pokemon2, false)));
                    return false;
                }
                case PBEMovePPChangedPacket mpcp:
                {
                    if (_mode != ClientMode.SinglePlayer)
                    {
                        PBEBattlePokemon moveUser = mpcp.MoveUserTeam.TryGetPokemon(mpcp.MoveUser);
                        if (_mode != ClientMode.Online || mpcp.MoveUserTeam.Id == BattleId)
                        {
                            moveUser.Moves[mpcp.Move].PP -= mpcp.AmountReduced;
                        }
                        moveUser.UpdateKnownPP(mpcp.Move, mpcp.AmountReduced);
                    }
                    return true;
                }
                case PBEMoveResultPacket mrp:
                {
                    PBEBattlePokemon moveUser = mrp.MoveUserTeam.TryGetPokemon(mrp.MoveUser),
                            pokemon2 = mrp.Pokemon2Team.TryGetPokemon(mrp.Pokemon2);
                    string message;
                    switch (mrp.Result)
                    {
                        case PBEResult.Ineffective_Ability: message = "{1} is protected by its Ability!"; break;
                        case PBEResult.Ineffective_Gender: message = "It doesn't affect {2}..."; break;
                        case PBEResult.Ineffective_Level: message = "{1} is protected by its level!"; break;
                        case PBEResult.Ineffective_MagnetRise: message = $"{{1}} is protected by {PBELocalizedString.GetMoveName(PBEMove.MagnetRise)}!"; break;
                        case PBEResult.Ineffective_Safeguard: message = $"{{1}} is protected by {PBELocalizedString.GetMoveName(PBEMove.Safeguard)}!"; break;
                        case PBEResult.Ineffective_Stat:
                        case PBEResult.Ineffective_Status:
                        case PBEResult.InvalidConditions: message = "But it failed!"; break;
                        case PBEResult.Ineffective_Substitute: message = $"{{1}} is protected by {PBELocalizedString.GetMoveName(PBEMove.Substitute)}!"; break;
                        case PBEResult.Ineffective_Type: message = "{1} is protected by its Type!"; break;
                        case PBEResult.NoTarget: message = "But there was no target..."; break;
                        case PBEResult.NotVeryEffective_Type: message = "It's not very effective on {2}..."; break;
                        case PBEResult.SuperEffective_Type: message = "It's super effective on {2}!"; break;
                        default: throw new ArgumentOutOfRangeException(nameof(mrp.Result));
                    }
                    BattleView.AddMessage(string.Format(message, NameForTrainer(moveUser, true), NameForTrainer(pokemon2, true), NameForTrainer(pokemon2, false)));
                    return false;
                }
                case PBEMoveUsedPacket mup:
                {
                    PBEBattlePokemon moveUser = mup.MoveUserTeam.TryGetPokemon(mup.MoveUser);
                    if (_mode != ClientMode.SinglePlayer && mup.Reveal)
                    {
                        moveUser.KnownMoves[PBEMove.MAX].Move = mup.Move;
                    }
                    BattleView.AddMessage(string.Format("{0} used {1}!", NameForTrainer(moveUser, true), PBELocalizedString.GetMoveName(mup.Move)));
                    return false;
                }
                case PBEPkmnFaintedPacket pfap:
                {
                    PBEBattlePokemon pokemon = _mode == ClientMode.SinglePlayer ? pfap.PokemonTeam.TryGetPokemon(pfap.PokemonId) : pfap.PokemonTeam.TryGetPokemon(pfap.PokemonPosition);
                    if (_mode != ClientMode.SinglePlayer)
                    {
                        Battle.ActiveBattlers.Remove(pokemon);
                        pokemon.FieldPosition = PBEFieldPosition.None;
                        if (_mode == ClientMode.Online && pfap.PokemonTeam.Id != BattleId)
                        {
                            PBETeam.Remove(pokemon);
                        }
                    }
                    BattleView.Field.HidePokemon(pokemon, pfap.PokemonPosition);
                    BattleView.AddMessage(string.Format("{0} fainted!", NameForTrainer(pokemon, true)));
                    return false;
                }
                case PBEPkmnFormChangedPacket pfcp:
                {
                    PBEBattlePokemon pokemon = pfcp.PokemonTeam.TryGetPokemon(pfcp.Pokemon);
                    if (_mode != ClientMode.SinglePlayer)
                    {
                        pokemon.Attack = pfcp.NewAttack;
                        pokemon.Defense = pfcp.NewDefense;
                        pokemon.SpAttack = pfcp.NewSpAttack;
                        pokemon.SpDefense = pfcp.NewSpDefense;
                        pokemon.Speed = pfcp.NewSpeed;
                        pokemon.Ability = pfcp.NewAbility;
                        pokemon.KnownAbility = pfcp.NewKnownAbility;
                        pokemon.Form = pokemon.KnownForm = pfcp.NewForm;
                        pokemon.Type1 = pokemon.KnownType1 = pfcp.NewType1;
                        pokemon.Type2 = pokemon.KnownType2 = pfcp.NewType2;
                        pokemon.Weight = pokemon.KnownWeight = pfcp.NewWeight;
                        if (pfcp.IsRevertForm)
                        {
                            pokemon.RevertForm = pfcp.NewForm;
                            pokemon.RevertAbility = pfcp.NewAbility; // If it's an opponent, this will be PBEAbility.MAX which is fine
                        }
                    }
                    BattleView.Field.UpdatePokemon(pokemon, false, true);
                    BattleView.AddMessage(string.Format("{0} transformed!", NameForTrainer(pokemon, true)));
                    return false;
                }
                case PBEPkmnHPChangedPacket phcp:
                {
                    PBEBattlePokemon pokemon = phcp.PokemonTeam.TryGetPokemon(phcp.Pokemon);
                    if (_mode != ClientMode.SinglePlayer)
                    {
                        pokemon.HP = phcp.NewHP;
                        pokemon.HPPercentage = phcp.NewHPPercentage;
                    }
                    BattleView.Field.UpdatePokemon(pokemon, true, false);
                    int change = phcp.NewHP - phcp.OldHP;
                    int absChange = Math.Abs(change);
                    double percentageChange = phcp.NewHPPercentage - phcp.OldHPPercentage;
                    double absPercentageChange = Math.Abs(percentageChange);
                    if (pokemon.Team.Id == BattleId || _mode == ClientMode.Replay) // Show raw values
                    {
                        BattleView.AddMessage(string.Format("{0} {1} {2} ({3:P2}) HP!", NameForTrainer(pokemon, true), change <= 0 ? "lost" : "restored", absChange, absPercentageChange));
                    }
                    else
                    {
                        BattleView.AddMessage(string.Format("{0} {1} {2:P2} of its HP!", NameForTrainer(pokemon, true), percentageChange <= 0 ? "lost" : "restored", absPercentageChange));
                    }
                    return false;
                }
                case PBEPkmnStatChangedPacket pscp:
                {
                    PBEBattlePokemon pokemon = pscp.PokemonTeam.TryGetPokemon(pscp.Pokemon);
                    if (_mode != ClientMode.SinglePlayer)
                    {
                        pokemon.SetStatChange(pscp.Stat, pscp.NewValue);
                    }
                    string statName, message;
                    switch (pscp.Stat)
                    {
                        case PBEStat.Accuracy: statName = "Accuracy"; break;
                        case PBEStat.Attack: statName = "Attack"; break;
                        case PBEStat.Defense: statName = "Defense"; break;
                        case PBEStat.Evasion: statName = "Evasion"; break;
                        case PBEStat.SpAttack: statName = "Special Attack"; break;
                        case PBEStat.SpDefense: statName = "Special Defense"; break;
                        case PBEStat.Speed: statName = "Speed"; break;
                        default: throw new ArgumentOutOfRangeException(nameof(pscp.Stat));
                    }
                    int change = pscp.NewValue - pscp.OldValue;
                    switch (change)
                    {
                        case -2: message = "harshly fell"; break;
                        case -1: message = "fell"; break;
                        case +1: message = "rose"; break;
                        case +2: message = "rose sharply"; break;
                        default:
                        {
                            if (change == 0 && pscp.NewValue == -Battle.Settings.MaxStatChange)
                            {
                                message = "won't go lower";
                            }
                            else if (change == 0 && pscp.NewValue == Battle.Settings.MaxStatChange)
                            {
                                message = "won't go higher";
                            }
                            else if (change <= -3)
                            {
                                message = "severely fell";
                            }
                            else if (change >= +3)
                            {
                                message = "rose drastically";
                            }
                            else
                            {
                                throw new ArgumentOutOfRangeException();
                            }
                            break;
                        }
                    }
                    BattleView.AddMessage(string.Format("{0}'s {1} {2}!", NameForTrainer(pokemon, true), statName, message));
                    return false;
                }
                case PBEPkmnSwitchInPacket psip:
                {
                    foreach (PBEPkmnSwitchInPacket.PBESwitchInInfo info in psip.SwitchIns)
                    {
                        PBEBattlePokemon pokemon;
                        if (_mode == ClientMode.Online && psip.Team.Id != BattleId)
                        {
                            pokemon = new PBEBattlePokemon(psip.Team, info);
                        }
                        else
                        {
                            pokemon = psip.Team.TryGetPokemon(info.PokemonId);
                            if (_mode != ClientMode.SinglePlayer)
                            {
                                pokemon.FieldPosition = info.FieldPosition;
                                PBETeam.SwitchTwoPokemon(pokemon, info.FieldPosition);
                                if (info.DisguisedAsId != info.PokemonId)
                                {
                                    pokemon.Status2 |= PBEStatus2.Disguised;
                                    pokemon.DisguisedAsPokemon = psip.Team.TryGetPokemon(info.DisguisedAsId);
                                    pokemon.KnownGender = pokemon.DisguisedAsPokemon.Gender;
                                    pokemon.KnownNickname = pokemon.DisguisedAsPokemon.Nickname;
                                    pokemon.KnownShiny = pokemon.DisguisedAsPokemon.Shiny;
                                    pokemon.KnownSpecies = pokemon.DisguisedAsPokemon.OriginalSpecies;
                                    pokemon.KnownForm = pokemon.DisguisedAsPokemon.Form;
                                    var pData = PBEPokemonData.GetData(pokemon.KnownSpecies, pokemon.KnownForm);
                                    pokemon.KnownType1 = pData.Type1;
                                    pokemon.KnownType2 = pData.Type2;
                                }
                            }
                        }
                        if (_mode != ClientMode.SinglePlayer)
                        {
                            Battle.ActiveBattlers.Add(pokemon);
                        }
                        BattleView.Field.ShowPokemon(pokemon);
                    }
                    if (!psip.Forced)
                    {
                        BattleView.AddMessage(string.Format("{1} sent out {0}!", PBEUtils.Andify(psip.SwitchIns.Select(s => s.Nickname).ToArray()), psip.Team.TrainerName));
                    }
                    return false;
                }
                case PBEPkmnSwitchOutPacket psop:
                {
                    PBEBattlePokemon pokemon = psop.PokemonId != byte.MaxValue ? psop.PokemonTeam.TryGetPokemon(psop.PokemonId) : psop.PokemonTeam.TryGetPokemon(psop.PokemonPosition);
                    PBEBattlePokemon disguisedAsPokemon = psop.DisguisedAsPokemonId != byte.MaxValue ? psop.PokemonTeam.TryGetPokemon(psop.DisguisedAsPokemonId) : pokemon;
                    if (_mode != ClientMode.SinglePlayer)
                    {
                        Battle.ActiveBattlers.Remove(pokemon);
                    }
                    if (_mode == ClientMode.Online && psop.PokemonTeam.Id != BattleId)
                    {
                        PBETeam.Remove(pokemon);
                    }
                    else if (_mode != ClientMode.SinglePlayer)
                    {
                        pokemon.ClearForSwitch();
                    }
                    BattleView.Field.HidePokemon(pokemon, psop.PokemonPosition);
                    if (!psop.Forced)
                    {
                        BattleView.AddMessage(string.Format("{1} withdrew {0}!", disguisedAsPokemon.Nickname, psop.PokemonTeam.TrainerName));
                    }
                    return false;
                }
                case PBEPsychUpPacket pup:
                {
                    PBEBattlePokemon user = pup.UserTeam.TryGetPokemon(pup.User),
                            target = pup.TargetTeam.TryGetPokemon(pup.Target);
                    if (_mode != ClientMode.SinglePlayer)
                    {
                        user.AttackChange = target.AttackChange = pup.AttackChange;
                        user.DefenseChange = target.DefenseChange = pup.DefenseChange;
                        user.SpAttackChange = target.SpAttackChange = pup.SpAttackChange;
                        user.SpDefenseChange = target.SpDefenseChange = pup.SpDefenseChange;
                        user.SpeedChange = target.SpeedChange = pup.SpeedChange;
                        user.AccuracyChange = target.AccuracyChange = pup.AccuracyChange;
                        user.EvasionChange = target.EvasionChange = pup.EvasionChange;
                    }
                    BattleView.AddMessage(string.Format("{0} copied {1}'s stat changes!", NameForTrainer(user, true), NameForTrainer(target, false)));
                    return false;
                }
                case PBEReflectTypePacket rtp:
                {
                    PBEBattlePokemon user = rtp.UserTeam.TryGetPokemon(rtp.User);
                    PBEBattlePokemon target = rtp.TargetTeam.TryGetPokemon(rtp.Target);
                    PBEType type1 = rtp.Type1;
                    PBEType type2 = rtp.Type2;
                    if (type1 == PBEType.None && type2 == PBEType.None) // Hidden info
                    {
                        if (_mode != ClientMode.SinglePlayer)
                        {
                            user.KnownType1 = target.KnownType1;
                            user.KnownType2 = target.KnownType2;
                        }
                        BattleView.AddMessage(string.Format("{0} copied {1}'s types!", NameForTrainer(user, true), NameForTrainer(target, false)));
                    }
                    else
                    {
                        if (_mode != ClientMode.SinglePlayer)
                        {
                            user.Type1 = user.KnownType1 = target.KnownType1 = target.Type1 = type1;
                            user.Type2 = user.KnownType2 = target.KnownType2 = target.Type2 = type2;
                        }
                        string type1Str = PBELocalizedString.GetTypeName(type1).ToString();
                        BattleView.AddMessage(string.Format("{0} copied {1}'s {2}",
                            NameForTrainer(user, true),
                            NameForTrainer(target, false),
                            type2 == PBEType.None ? $"{type1Str} type!" : $"{type1Str} and {PBELocalizedString.GetTypeName(type2)} types!"));
                    }
                    return false;
                }
                case PBESpecialMessagePacket smp:
                {
                    string message;
                    switch (smp.Message)
                    {
                        case PBESpecialMessage.DraggedOut:
                        {
                            message = string.Format("{0} was dragged out!", NameForTrainer(((PBETeam)smp.Params[1]).TryGetPokemon((PBEFieldPosition)smp.Params[0]), true));
                            break;
                        }
                        case PBESpecialMessage.Endure:
                        {
                            message = string.Format("{0} endured the hit!", NameForTrainer(((PBETeam)smp.Params[1]).TryGetPokemon((PBEFieldPosition)smp.Params[0]), true));
                            break;
                        }
                        case PBESpecialMessage.HPDrained:
                        {
                            message = string.Format("{0} had its energy drained!", NameForTrainer(((PBETeam)smp.Params[1]).TryGetPokemon((PBEFieldPosition)smp.Params[0]), true));
                            break;
                        }
                        case PBESpecialMessage.Magnitude:
                        {
                            message = string.Format("Magnitude {0}!", (byte)smp.Params[0]);
                            break;
                        }
                        case PBESpecialMessage.MultiHit:
                        {
                            message = string.Format("Hit {0} time(s)!", (byte)smp.Params[0]);
                            break;
                        }
                        case PBESpecialMessage.NothingHappened:
                        {
                            message = "But nothing happened!";
                            break;
                        }
                        case PBESpecialMessage.OneHitKnockout:
                        {
                            message = "It's a one-hit KO!";
                            break;
                        }
                        case PBESpecialMessage.PainSplit:
                        {
                            message = "The battlers shared their pain!";
                            break;
                        }
                        case PBESpecialMessage.Recoil:
                        {
                            message = string.Format("{0} is damaged by recoil!", NameForTrainer(((PBETeam)smp.Params[1]).TryGetPokemon((PBEFieldPosition)smp.Params[0]), true));
                            break;
                        }
                        case PBESpecialMessage.Struggle:
                        {
                            message = string.Format("{0} has no moves left!", NameForTrainer(((PBETeam)smp.Params[1]).TryGetPokemon((PBEFieldPosition)smp.Params[0]), true));
                            break;
                        }
                        default: throw new ArgumentOutOfRangeException(nameof(smp.Message));
                    }
                    BattleView.AddMessage(message);
                    return false;
                }
                case PBEStatus1Packet s1p:
                {
                    PBEBattlePokemon status1Receiver = s1p.Status1ReceiverTeam.TryGetPokemon(s1p.Status1Receiver);
                    if (_mode != ClientMode.SinglePlayer)
                    {
                        switch (s1p.StatusAction)
                        {
                            case PBEStatusAction.Added:
                            case PBEStatusAction.Announced:
                            case PBEStatusAction.CausedImmobility:
                            case PBEStatusAction.Damage: status1Receiver.Status1 = s1p.Status1; break;
                            case PBEStatusAction.Cleared:
                            case PBEStatusAction.Ended: status1Receiver.Status1 = PBEStatus1.None; break;
                            default: throw new ArgumentOutOfRangeException(nameof(s1p.StatusAction));
                        }
                    }
                    BattleView.Field.UpdatePokemon(status1Receiver, true, false);
                    string message;
                    switch (s1p.Status1)
                    {
                        case PBEStatus1.Asleep:
                        {
                            switch (s1p.StatusAction)
                            {
                                case PBEStatusAction.Added: message = "{0} fell asleep!"; break;
                                case PBEStatusAction.CausedImmobility: message = "{0} is fast asleep."; break;
                                case PBEStatusAction.Cleared:
                                case PBEStatusAction.Ended: message = "{0} woke up!"; break;
                                default: throw new ArgumentOutOfRangeException(nameof(s1p.StatusAction));
                            }
                            break;
                        }
                        case PBEStatus1.BadlyPoisoned:
                        {
                            switch (s1p.StatusAction)
                            {
                                case PBEStatusAction.Added: message = "{0} was badly poisoned!"; break;
                                case PBEStatusAction.Cleared: message = "{0} was cured of its poisoning."; break;
                                case PBEStatusAction.Damage: message = "{0} was hurt by poison!"; break;
                                default: throw new ArgumentOutOfRangeException(nameof(s1p.StatusAction));
                            }
                            break;
                        }
                        case PBEStatus1.Burned:
                        {
                            switch (s1p.StatusAction)
                            {
                                case PBEStatusAction.Added: message = "{0} was burned!"; break;
                                case PBEStatusAction.Cleared: message = "{0}'s burn was healed."; break;
                                case PBEStatusAction.Damage: message = "{0} was hurt by its burn!"; break;
                                default: throw new ArgumentOutOfRangeException(nameof(s1p.StatusAction));
                            }
                            break;
                        }
                        case PBEStatus1.Frozen:
                        {
                            switch (s1p.StatusAction)
                            {
                                case PBEStatusAction.Added: message = "{0} was frozen solid!"; break;
                                case PBEStatusAction.CausedImmobility: message = "{0} is frozen solid!"; break;
                                case PBEStatusAction.Cleared:
                                case PBEStatusAction.Ended: message = "{0} thawed out!"; break;
                                default: throw new ArgumentOutOfRangeException(nameof(s1p.StatusAction));
                            }
                            break;
                        }
                        case PBEStatus1.Paralyzed:
                        {
                            switch (s1p.StatusAction)
                            {
                                case PBEStatusAction.Added: message = "{0} is paralyzed! It may be unable to move!"; break;
                                case PBEStatusAction.CausedImmobility: message = "{0} is paralyzed! It can't move!"; break;
                                case PBEStatusAction.Cleared: message = "{0} was cured of paralysis."; break;
                                default: throw new ArgumentOutOfRangeException(nameof(s1p.StatusAction));
                            }
                            break;
                        }
                        case PBEStatus1.Poisoned:
                        {
                            switch (s1p.StatusAction)
                            {
                                case PBEStatusAction.Added: message = "{0} was poisoned!"; break;
                                case PBEStatusAction.Cleared: message = "{0} was cured of its poisoning."; break;
                                case PBEStatusAction.Damage: message = "{0} was hurt by poison!"; break;
                                default: throw new ArgumentOutOfRangeException(nameof(s1p.StatusAction));
                            }
                            break;
                        }
                        default: throw new ArgumentOutOfRangeException(nameof(s1p.Status1));
                    }
                    BattleView.AddMessage(string.Format(message, NameForTrainer(status1Receiver, true)));
                    return false;
                }
                case PBEStatus2Packet s2p:
                {
                    PBEBattlePokemon status2Receiver = s2p.Status2ReceiverTeam.TryGetPokemon(s2p.Status2Receiver),
                            pokemon2 = s2p.Pokemon2Team.TryGetPokemon(s2p.Pokemon2);
                    if (_mode != ClientMode.SinglePlayer)
                    {
                        switch (s2p.StatusAction)
                        {
                            case PBEStatusAction.Added:
                            case PBEStatusAction.Announced:
                            case PBEStatusAction.CausedImmobility:
                            case PBEStatusAction.Damage: status2Receiver.Status2 |= s2p.Status2; status2Receiver.KnownStatus2 |= s2p.Status2; break;
                            case PBEStatusAction.Cleared:
                            case PBEStatusAction.Ended: status2Receiver.Status2 &= ~s2p.Status2; status2Receiver.KnownStatus2 &= ~s2p.Status2; break;
                            default: throw new ArgumentOutOfRangeException(nameof(s2p.StatusAction));
                        }
                    }
                    string message;
                    bool status2ReceiverCaps = true,
                            pokemon2Caps = false;
                    switch (s2p.Status2)
                    {
                        case PBEStatus2.Airborne:
                        {
                            BattleView.Field.UpdatePokemon(status2Receiver, false, true);
                            switch (s2p.StatusAction)
                            {
                                case PBEStatusAction.Added: message = "{0} flew up high!"; break;
                                case PBEStatusAction.Ended: return true;
                                default: throw new ArgumentOutOfRangeException(nameof(s2p.StatusAction));
                            }
                            break;
                        }
                        case PBEStatus2.Confused:
                        {
                            switch (s2p.StatusAction)
                            {
                                case PBEStatusAction.Added: message = "{0} became confused!"; break;
                                case PBEStatusAction.Announced: message = "{0} is confused!"; break;
                                case PBEStatusAction.Cleared:
                                case PBEStatusAction.Ended: message = "{0} snapped out of its confusion."; break;
                                case PBEStatusAction.Damage: message = "It hurt itself in its confusion!"; break;
                                default: throw new ArgumentOutOfRangeException(nameof(s2p.StatusAction));
                            }
                            break;
                        }
                        case PBEStatus2.Cursed:
                        {
                            switch (s2p.StatusAction)
                            {
                                case PBEStatusAction.Added: message = "{1} cut its own HP and laid a curse on {0}!"; status2ReceiverCaps = false; pokemon2Caps = true; break;
                                case PBEStatusAction.Damage: message = "{0} is afflicted by the curse!"; break;
                                default: throw new ArgumentOutOfRangeException(nameof(s2p.StatusAction));
                            }
                            break;
                        }
                        case PBEStatus2.Disguised:
                        {
                            switch (s2p.StatusAction)
                            {
                                case PBEStatusAction.Ended:
                                {
                                    BattleView.Field.UpdatePokemon(status2Receiver, true, true);
                                    message = "{0}'s illusion wore off!";
                                    break;
                                }
                                default: throw new ArgumentOutOfRangeException(nameof(s2p.StatusAction));
                            }
                            break;
                        }
                        case PBEStatus2.Flinching:
                        {
                            switch (s2p.StatusAction)
                            {
                                case PBEStatusAction.CausedImmobility: message = "{0} flinched and couldn't move!"; break;
                                case PBEStatusAction.Ended: return true;
                                default: throw new ArgumentOutOfRangeException(nameof(s2p.StatusAction));
                            }
                            break;
                        }
                        case PBEStatus2.HelpingHand:
                        {
                            switch (s2p.StatusAction)
                            {
                                case PBEStatusAction.Added: message = "{1} is ready to help {0}!"; status2ReceiverCaps = false; pokemon2Caps = true; break;
                                case PBEStatusAction.Ended: return true;
                                default: throw new ArgumentOutOfRangeException(nameof(s2p.StatusAction));
                            }
                            break;
                        }
                        case PBEStatus2.Identified:
                        case PBEStatus2.MiracleEye:
                        {
                            switch (s2p.StatusAction)
                            {
                                case PBEStatusAction.Added: message = "{0} was identified!"; break;
                                default: throw new ArgumentOutOfRangeException(nameof(s2p.StatusAction));
                            }
                            break;
                        }
                        case PBEStatus2.Infatuated:
                        {
                            switch (s2p.StatusAction)
                            {
                                case PBEStatusAction.Added:
                                {
                                    if (_mode != ClientMode.SinglePlayer)
                                    {
                                        status2Receiver.InfatuatedWithPokemon = pokemon2;
                                    }
                                    message = "{0} fell in love with {1}!"; break;
                                }
                                case PBEStatusAction.Announced: message = "{0} is in love with {1}!"; break;
                                case PBEStatusAction.CausedImmobility: message = "{0} is immobilized by love!"; break;
                                case PBEStatusAction.Cleared:
                                case PBEStatusAction.Ended:
                                {
                                    if (_mode != ClientMode.SinglePlayer)
                                    {
                                        status2Receiver.InfatuatedWithPokemon = null;
                                    }
                                    message = "{0} got over its infatuation.";
                                    break;
                                }
                                default: throw new ArgumentOutOfRangeException(nameof(s2p.StatusAction));
                            }
                            break;
                        }
                        case PBEStatus2.LeechSeed:
                        {
                            switch (s2p.StatusAction)
                            {
                                case PBEStatusAction.Added:
                                {
                                    if (_mode != ClientMode.SinglePlayer)
                                    {
                                        status2Receiver.SeededPosition = pokemon2.FieldPosition;
                                        status2Receiver.SeededTeam = pokemon2.Team;
                                    }
                                    message = "{0} was seeded!";
                                    break;
                                }
                                case PBEStatusAction.Damage: message = "{0}'s health is sapped by Leech Seed!"; break;
                                default: throw new ArgumentOutOfRangeException(nameof(s2p.StatusAction));
                            }
                            break;
                        }
                        case PBEStatus2.LockOn:
                        {
                            switch (s2p.StatusAction)
                            {
                                case PBEStatusAction.Added:
                                {
                                    if (_mode != ClientMode.SinglePlayer)
                                    {
                                        status2Receiver.LockOnPokemon = pokemon2;
                                    }
                                    message = "{0} took aim at {1}!";
                                    break;
                                }
                                case PBEStatusAction.Ended:
                                {
                                    if (_mode != ClientMode.SinglePlayer)
                                    {
                                        status2Receiver.LockOnPokemon = null;
                                    }
                                    return true;
                                }
                                default: throw new ArgumentOutOfRangeException(nameof(s2p.StatusAction));
                            }
                            break;
                        }
                        case PBEStatus2.MagnetRise:
                        {
                            switch (s2p.StatusAction)
                            {
                                case PBEStatusAction.Added: message = "{0} levitated with electromagnetism!"; break;
                                case PBEStatusAction.Ended: message = "{0}'s electromagnetism wore off!"; break;
                                default: throw new ArgumentOutOfRangeException(nameof(s2p.StatusAction));
                            }
                            break;
                        }
                        case PBEStatus2.Nightmare:
                        {
                            switch (s2p.StatusAction)
                            {
                                case PBEStatusAction.Added: message = "{0} began having a nightmare!"; break;
                                case PBEStatusAction.Damage: message = "{0} is locked in a nightmare!"; break;
                                case PBEStatusAction.Ended: return true;
                                default: throw new ArgumentOutOfRangeException(nameof(s2p.StatusAction));
                            }
                            break;
                        }
                        case PBEStatus2.PowerTrick:
                        {
                            switch (s2p.StatusAction)
                            {
                                case PBEStatusAction.Added:
                                {
                                    if (_mode != ClientMode.SinglePlayer)
                                    {
                                        status2Receiver.ApplyPowerTrickChange();
                                    }
                                    message = "{0} switched its Attack and Defense!";
                                    break;
                                }
                                case PBEStatusAction.Ended: return true;
                                default: throw new ArgumentOutOfRangeException(nameof(s2p.StatusAction));
                            }
                            break;
                        }
                        case PBEStatus2.Protected:
                        {
                            switch (s2p.StatusAction)
                            {
                                case PBEStatusAction.Added:
                                case PBEStatusAction.Damage: message = "{0} protected itself!"; break;
                                case PBEStatusAction.Cleared: message = "{1} broke through {0}'s protection!"; status2ReceiverCaps = false; pokemon2Caps = true; break;
                                case PBEStatusAction.Ended: return true;
                                default: throw new ArgumentOutOfRangeException(nameof(s2p.StatusAction));
                            }
                            break;
                        }
                        case PBEStatus2.Pumped:
                        {
                            switch (s2p.StatusAction)
                            {
                                case PBEStatusAction.Added: message = "{0} is getting pumped!"; break;
                                default: throw new ArgumentOutOfRangeException(nameof(s2p.StatusAction));
                            }
                            break;
                        }
                        case PBEStatus2.ShadowForce:
                        {
                            BattleView.Field.UpdatePokemon(status2Receiver, false, true);
                            switch (s2p.StatusAction)
                            {
                                case PBEStatusAction.Added: message = "{0} vanished instantly!"; break;
                                case PBEStatusAction.Ended: return true;
                                default: throw new ArgumentOutOfRangeException(nameof(s2p.StatusAction));
                            }
                            break;
                        }
                        case PBEStatus2.Substitute:
                        {
                            BattleView.Field.UpdatePokemon(status2Receiver, false, true);
                            switch (s2p.StatusAction)
                            {
                                case PBEStatusAction.Added: message = "{0} put in a substitute!"; break;
                                case PBEStatusAction.Damage: message = "The substitute took damage for {0}!"; status2ReceiverCaps = false; break;
                                case PBEStatusAction.Ended: message = "{0}'s substitute faded!"; break;
                                default: throw new ArgumentOutOfRangeException(nameof(s2p.StatusAction));
                            }
                            break;
                        }
                        case PBEStatus2.Transformed:
                        {
                            switch (s2p.StatusAction)
                            {
                                case PBEStatusAction.Added:
                                {
                                    if (_mode != ClientMode.SinglePlayer)
                                    {
                                        status2Receiver.Transform(pokemon2);
                                    }
                                    BattleView.Field.UpdatePokemon(status2Receiver, false, true);
                                    message = "{0} transformed into {1}!";
                                    break;
                                }
                                default: throw new ArgumentOutOfRangeException(nameof(s2p.StatusAction));
                            }
                            break;
                        }
                        case PBEStatus2.Underground:
                        {
                            BattleView.Field.UpdatePokemon(status2Receiver, false, true);
                            switch (s2p.StatusAction)
                            {
                                case PBEStatusAction.Added: message = "{0} burrowed its way under the ground!"; break;
                                case PBEStatusAction.Ended: return true;
                                default: throw new ArgumentOutOfRangeException(nameof(s2p.StatusAction));
                            }
                            break;
                        }
                        case PBEStatus2.Underwater:
                        {
                            BattleView.Field.UpdatePokemon(status2Receiver, false, true);
                            switch (s2p.StatusAction)
                            {
                                case PBEStatusAction.Added: message = "{0} hid underwater!"; break;
                                case PBEStatusAction.Ended: return true;
                                default: throw new ArgumentOutOfRangeException(nameof(s2p.StatusAction));
                            }
                            break;
                        }
                        default: throw new ArgumentOutOfRangeException(nameof(s2p.Status2));
                    }
                    BattleView.AddMessage(string.Format(message, NameForTrainer(status2Receiver, status2ReceiverCaps), NameForTrainer(pokemon2, pokemon2Caps)));
                    return false;
                }
                case PBETeamStatusPacket tsp:
                {
                    if (_mode != ClientMode.SinglePlayer)
                    {
                        switch (tsp.TeamStatusAction)
                        {
                            case PBETeamStatusAction.Added:
                            case PBETeamStatusAction.Damage: tsp.Team.TeamStatus |= tsp.TeamStatus; break;
                            case PBETeamStatusAction.Cleared:
                            case PBETeamStatusAction.Ended: tsp.Team.TeamStatus &= ~tsp.TeamStatus; break;
                            default: throw new ArgumentOutOfRangeException(nameof(tsp.TeamStatusAction));
                        }
                    }
                    PBEBattlePokemon damageVictim = tsp.DamageVictim.HasValue ? tsp.Team.TryGetPokemon(tsp.DamageVictim.Value) : null;
                    string message;
                    bool damageVictimCaps = false;
                    switch (tsp.TeamStatus)
                    {
                        case PBETeamStatus.LightScreen:
                        {
                            switch (tsp.TeamStatusAction)
                            {
                                case PBETeamStatusAction.Added: message = "Light Screen raised {0} team's Special Defense!"; break;
                                case PBETeamStatusAction.Cleared:
                                case PBETeamStatusAction.Ended: message = "{1} team's Light Screen wore off!"; break;
                                default: throw new ArgumentOutOfRangeException(nameof(tsp.TeamStatusAction));
                            }
                            break;
                        }
                        case PBETeamStatus.LuckyChant:
                        {
                            switch (tsp.TeamStatusAction)
                            {
                                case PBETeamStatusAction.Added: message = "The Lucky Chant shielded {0} team from critical hits!"; break;
                                case PBETeamStatusAction.Ended: message = "{1} team's Lucky Chant wore off!"; break;
                                default: throw new ArgumentOutOfRangeException(nameof(tsp.TeamStatusAction));
                            }
                            break;
                        }
                        case PBETeamStatus.Reflect:
                        {
                            switch (tsp.TeamStatusAction)
                            {
                                case PBETeamStatusAction.Added: message = "Reflect raised {0} team's Defense!"; break;
                                case PBETeamStatusAction.Cleared:
                                case PBETeamStatusAction.Ended: message = "{1} team's Reflect wore off!"; break;
                                default: throw new ArgumentOutOfRangeException(nameof(tsp.TeamStatusAction));
                            }
                            break;
                        }
                        case PBETeamStatus.Safeguard:
                        {
                            switch (tsp.TeamStatusAction)
                            {
                                case PBETeamStatusAction.Added: message = "{5} team became cloaked in a mystical veil!"; break;
                                case PBETeamStatusAction.Ended: message = "{5} team is no longer protected by Safeguard!"; break;
                                default: throw new ArgumentOutOfRangeException(nameof(tsp.TeamStatusAction));
                            }
                            break;
                        }
                        case PBETeamStatus.Spikes:
                        {
                            switch (tsp.TeamStatusAction)
                            {
                                case PBETeamStatusAction.Added:
                                {
                                    if (_mode != ClientMode.SinglePlayer)
                                    {
                                        tsp.Team.SpikeCount++;
                                    }
                                    message = "Spikes were scattered all around the feet of {2} team!";
                                    break;
                                }
                                /*case PBETeamStatusAction.Cleared:
                                {
                                    if (Mode != ClientMode.SinglePlayer)
                                    {
                                        tsp.Team.SpikeCount = 0;
                                    }
                                    message = "The spikes disappeared from around {2} team's feet!";
                                    break;
                                }*/
                                case PBETeamStatusAction.Damage: message = "{4} is hurt by the spikes!"; damageVictimCaps = true; break;
                                default: throw new ArgumentOutOfRangeException(nameof(tsp.TeamStatusAction));
                            }
                            break;
                        }
                        case PBETeamStatus.StealthRock:
                        {
                            switch (tsp.TeamStatusAction)
                            {
                                case PBETeamStatusAction.Added: message = "Pointed stones float in the air around {3} team!"; break;
                                //case PBETeamStatusAction.Cleared: message = "The pointed stones disappeared from around {2} team!"; break;
                                case PBETeamStatusAction.Damage: message = "Pointed stones dug into {4}!"; break;
                                default: throw new ArgumentOutOfRangeException(nameof(tsp.TeamStatusAction));
                            }
                            break;
                        }
                        case PBETeamStatus.Tailwind:
                        {
                            switch (tsp.TeamStatusAction)
                            {
                                case PBETeamStatusAction.Added: message = "The tailwind blew from behind {2} team!"; break;
                                case PBETeamStatusAction.Ended: message = "{5} team's tailwind petered out!"; break;
                                default: throw new ArgumentOutOfRangeException(nameof(tsp.TeamStatusAction));
                            }
                            break;
                        }
                        case PBETeamStatus.ToxicSpikes:
                        {
                            switch (tsp.TeamStatusAction)
                            {
                                case PBETeamStatusAction.Added:
                                {
                                    if (_mode != ClientMode.SinglePlayer)
                                    {
                                        tsp.Team.ToxicSpikeCount++;
                                    }
                                    message = "Poison spikes were scattered all around {2} team's feet!";
                                    break;
                                }
                                case PBETeamStatusAction.Cleared:
                                {
                                    if (_mode != ClientMode.SinglePlayer)
                                    {
                                        tsp.Team.ToxicSpikeCount = 0;
                                    }
                                    message = "The poison spikes disappeared from around {2} team's feet!";
                                    break;
                                }
                                default: throw new ArgumentOutOfRangeException(nameof(tsp.TeamStatusAction));
                            }
                            break;
                        }
                        case PBETeamStatus.WideGuard:
                        {
                            switch (tsp.TeamStatusAction)
                            {
                                case PBETeamStatusAction.Added: message = "Wide Guard protected {2} team!"; break;
                                case PBETeamStatusAction.Cleared: message = "{1} team's Wide Guard was destroyed!"; break;
                                case PBETeamStatusAction.Damage: message = "Wide Guard protected {4}!"; break;
                                case PBETeamStatusAction.Ended: return true;
                                default: throw new ArgumentOutOfRangeException(nameof(tsp.TeamStatusAction));
                            }
                            break;
                        }
                        default: throw new ArgumentOutOfRangeException(nameof(tsp.TeamStatus));
                    }
                    BattleView.AddMessage(string.Format(message,
                        BattleId >= 2 ? $"{tsp.Team.TrainerName}'s" : tsp.Team.Id == BattleId ? "your" : "the opposing",
                        BattleId >= 2 ? $"{tsp.Team.TrainerName}'s" : tsp.Team.Id == BattleId ? "Your" : "The opposing",
                        BattleId >= 2 ? $"{tsp.Team.TrainerName}'s" : tsp.Team.Id == BattleId ? "your" : "the foe's",
                        BattleId >= 2 ? $"{tsp.Team.TrainerName}'s" : tsp.Team.Id == BattleId ? "your" : "your foe's",
                        NameForTrainer(damageVictim, damageVictimCaps),
                        BattleId >= 2 ? $"{tsp.Team.TrainerName}'s" : tsp.Team.Id == BattleId ? "Your" : "The foe's"
                        ));
                    return false;
                }
                case PBETransformPacket tp:
                {
                    if (_mode != ClientMode.SinglePlayer)
                    {
                        PBEBattlePokemon user = tp.UserTeam.TryGetPokemon(tp.User),
                                target = tp.TargetTeam.TryGetPokemon(tp.Target);
                        target.Attack = tp.TargetAttack;
                        target.Defense = tp.TargetDefense;
                        target.SpAttack = tp.TargetSpAttack;
                        target.SpDefense = tp.TargetSpDefense;
                        target.Speed = tp.TargetSpeed;
                        target.AttackChange = tp.TargetAttackChange;
                        target.DefenseChange = tp.TargetDefenseChange;
                        target.SpAttackChange = tp.TargetSpAttackChange;
                        target.SpDefenseChange = tp.TargetSpDefenseChange;
                        target.SpeedChange = tp.TargetSpeedChange;
                        target.AccuracyChange = tp.TargetAccuracyChange;
                        target.EvasionChange = tp.TargetEvasionChange;
                        target.Ability = target.KnownAbility = tp.TargetAbility;
                        for (int i = 0; i < Battle.Settings.NumMoves; i++)
                        {
                            target.Moves[i].Move = tp.TargetMoves[i];
                        }
                        target.Species = target.KnownSpecies = tp.TargetSpecies;
                        target.Type1 = target.KnownType1 = tp.TargetType1;
                        target.Type2 = target.KnownType2 = tp.TargetType2;
                        target.Weight = target.KnownWeight = tp.TargetWeight;
                    }
                    return true;
                }
                case PBETypeChangedPacket tcp:
                {
                    PBEBattlePokemon pokemon = tcp.PokemonTeam.TryGetPokemon(tcp.Pokemon);
                    PBEType type1 = tcp.Type1;
                    PBEType type2 = tcp.Type2;
                    if (_mode != ClientMode.SinglePlayer)
                    {
                        pokemon.Type1 = type1;
                        pokemon.KnownType1 = type1;
                        pokemon.Type2 = type2;
                        pokemon.KnownType2 = type2;
                    }
                    string type1Str = PBELocalizedString.GetTypeName(type1).ToString();
                    BattleView.AddMessage(string.Format("{0} transformed into the {1}", NameForTrainer(pokemon, true), type2 == PBEType.None ? $"{type1Str} type!" : $"{type1Str} and {PBELocalizedString.GetTypeName(type2)} types!"));
                    return false;
                }
                case PBEWeatherPacket wp:
                {
                    if (_mode != ClientMode.SinglePlayer)
                    {
                        switch (wp.WeatherAction)
                        {
                            case PBEWeatherAction.Added:
                            case PBEWeatherAction.CausedDamage: Battle.Weather = wp.Weather; break;
                            case PBEWeatherAction.Ended: Battle.Weather = PBEWeather.None; break;
                            default: throw new ArgumentOutOfRangeException(nameof(wp.WeatherAction));
                        }
                    }
                    switch (wp.WeatherAction)
                    {
                        case PBEWeatherAction.Added:
                        case PBEWeatherAction.Ended: BattleView.Field.UpdateWeather(); break;
                        case PBEWeatherAction.CausedDamage: break;
                        default: throw new ArgumentOutOfRangeException(nameof(wp.WeatherAction));
                    }
                    PBEBattlePokemon damageVictim = wp.DamageVictim.HasValue ? wp.DamageVictimTeam.TryGetPokemon(wp.DamageVictim.Value) : null;
                    string message;
                    switch (wp.Weather)
                    {
                        case PBEWeather.Hailstorm:
                        {
                            switch (wp.WeatherAction)
                            {
                                case PBEWeatherAction.Added: message = "It started to hail!"; break;
                                case PBEWeatherAction.CausedDamage: message = "{0} is buffeted by the hail!"; break;
                                case PBEWeatherAction.Ended: message = "The hail stopped."; break;
                                default: throw new ArgumentOutOfRangeException(nameof(wp.WeatherAction));
                            }
                            break;
                        }
                        case PBEWeather.HarshSunlight:
                        {
                            switch (wp.WeatherAction)
                            {
                                case PBEWeatherAction.Added: message = "The sunlight turned harsh!"; break;
                                case PBEWeatherAction.Ended: message = "The sunlight faded."; break;
                                default: throw new ArgumentOutOfRangeException(nameof(wp.WeatherAction));
                            }
                            break;
                        }
                        case PBEWeather.Rain:
                        {
                            switch (wp.WeatherAction)
                            {
                                case PBEWeatherAction.Added: message = "It started to rain!"; break;
                                case PBEWeatherAction.Ended: message = "The rain stopped."; break;
                                default: throw new ArgumentOutOfRangeException(nameof(wp.WeatherAction));
                            }
                            break;
                        }
                        case PBEWeather.Sandstorm:
                        {
                            switch (wp.WeatherAction)
                            {
                                case PBEWeatherAction.Added: message = "A sandstorm kicked up!"; break;
                                case PBEWeatherAction.CausedDamage: message = "{0} is buffeted by the sandstorm!"; break;
                                case PBEWeatherAction.Ended: message = "The sandstorm subsided."; break;
                                default: throw new ArgumentOutOfRangeException(nameof(wp.WeatherAction));
                            }
                            break;
                        }
                        default: throw new ArgumentOutOfRangeException(nameof(wp.Weather));
                    }
                    BattleView.AddMessage(string.Format(message, NameForTrainer(damageVictim, true)));
                    return false;
                }
                case PBEActionsRequestPacket arp:
                {
                    switch (_mode)
                    {
                        case ClientMode.Online:
                        {
                            if (arp.Team.Id == BattleId)
                            {
                                ActionsLoop(true);
                            }
                            else if (BattleId >= 2) // Spectators
                            {
                                BattleView.AddMessage("Waiting for players...", messageLog: false);
                            }
                            break;
                        }
                        case ClientMode.SinglePlayer:
                        {
                            if (arp.Team.Id == BattleId)
                            {
                                ActionsLoop(true);
                            }
                            else
                            {
                                new Thread(() => PBEBattle.SelectActionsIfValid(arp.Team, PBEAI.CreateActions(arp.Team))) { Name = "Battle Thread" }.Start();
                            }
                            break;
                        }
                    }
                    return true;
                }
                case PBEAutoCenterPacket acp:
                {
                    PBEBattlePokemon pokemon1,
                            pokemon2;
                    if (_mode == ClientMode.SinglePlayer)
                    {
                        pokemon1 = acp.Pokemon1Team.TryGetPokemon(acp.Pokemon1Id);
                        pokemon2 = acp.Pokemon2Team.TryGetPokemon(acp.Pokemon2Id);
                    }
                    else
                    {
                        pokemon1 = acp.Pokemon1Team.TryGetPokemon(acp.Pokemon1Position);
                        pokemon2 = acp.Pokemon2Team.TryGetPokemon(acp.Pokemon2Position);
                        pokemon1.FieldPosition = PBEFieldPosition.Center;
                        pokemon2.FieldPosition = PBEFieldPosition.Center;
                    }
                    BattleView.Field.MovePokemon(pokemon1, acp.Pokemon1Position);
                    BattleView.Field.MovePokemon(pokemon2, acp.Pokemon2Position);
                    BattleView.AddMessage("The battlers shifted to the center!");
                    return false;
                }
                case PBESwitchInRequestPacket sirp:
                {
                    switch (_mode)
                    {
                        case ClientMode.Online:
                        {
                            sirp.Team.SwitchInsRequired = sirp.Amount;
                            if (sirp.Team.Id == BattleId)
                            {
                                switchesRequired = sirp.Amount;
                                SwitchesLoop(true);
                            }
                            else if (BattleId >= 2) // Spectators
                            {
                                BattleView.AddMessage("Waiting for players...", messageLog: false);
                            }
                            else if (switchesRequired == 0) // Don't display this message if we're in switchesloop because it'd overwrite the messages we need to see.
                            {
                                BattleView.AddMessage($"Waiting for {Team.OpposingTeam.TrainerName}...", messageLog: false);
                            }
                            break;
                        }
                        case ClientMode.SinglePlayer:
                        {
                            if (sirp.Team.Id == BattleId)
                            {
                                switchesRequired = sirp.Amount;
                                SwitchesLoop(true);
                            }
                            else
                            {
                                new Thread(() => PBEBattle.SelectSwitchesIfValid(sirp.Team, PBEAI.CreateSwitches(sirp.Team))) { Name = "Battle Thread" }.Start();
                            }
                        }
                        break;
                    }
                    return true;
                }
                case PBETeamPacket _:
                {
                    return true;
                }
                case PBETurnBeganPacket tbp:
                {
                    BattleView.AddMessage($"Turn {Battle.TurnNumber = tbp.TurnNumber}", messageBox: false);
                    return true;
                }
                case PBEWinnerPacket win:
                {
                    if (_mode != ClientMode.SinglePlayer)
                    {
                        Battle.Winner = win.WinningTeam;
                    }
                    BattleView.AddMessage(string.Format("{0} defeated {1}!", win.WinningTeam.TrainerName, win.WinningTeam.OpposingTeam.TrainerName));
                    return true;
                }
                default: throw new ArgumentOutOfRangeException(nameof(packet));
            }
        }

        #endregion
    }
}
