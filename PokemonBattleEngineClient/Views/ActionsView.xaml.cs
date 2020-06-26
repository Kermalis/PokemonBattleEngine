using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Kermalis.PokemonBattleEngine.Battle;
using Kermalis.PokemonBattleEngine.Data;
using Kermalis.PokemonBattleEngineClient.Models;
using System;
using System.ComponentModel;

namespace Kermalis.PokemonBattleEngineClient.Views
{
    // If you dislike spaghetti code, I suggest you close this file to avoid death
    public sealed class ActionsView : UserControl, INotifyPropertyChanged
    {
        private void OnPropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }
        public new event PropertyChangedEventHandler PropertyChanged;

        private PBEBattlePokemon _targetAllyLeft;
        public PBEBattlePokemon TargetAllyLeft
        {
            get => _targetAllyLeft;
            private set
            {
                if (_targetAllyLeft != value)
                {
                    _targetAllyLeft = value;
                    OnPropertyChanged(nameof(TargetAllyLeft));
                }
            }
        }
        private bool _targetAllyLeftEnabled;
        public bool TargetAllyLeftEnabled
        {
            get => _targetAllyLeftEnabled;
            private set
            {
                if (_targetAllyLeftEnabled != value)
                {
                    _targetAllyLeftEnabled = value;
                    OnPropertyChanged(nameof(TargetAllyLeftEnabled));
                }
            }
        }
        private PBEBattlePokemon _targetAllyCenter;
        public PBEBattlePokemon TargetAllyCenter
        {
            get => _targetAllyCenter;
            private set
            {
                if (_targetAllyCenter != value)
                {
                    _targetAllyCenter = value;
                    OnPropertyChanged(nameof(TargetAllyCenter));
                }
            }
        }
        private bool _targetAllyCenterEnabled;
        public bool TargetAllyCenterEnabled
        {
            get => _targetAllyCenterEnabled;
            private set
            {
                if (_targetAllyCenterEnabled != value)
                {
                    _targetAllyCenterEnabled = value;
                    OnPropertyChanged(nameof(TargetAllyCenterEnabled));
                }
            }
        }
        private PBEBattlePokemon _targetAllyRight;
        public PBEBattlePokemon TargetAllyRight
        {
            get => _targetAllyRight;
            private set
            {
                if (_targetAllyRight != value)
                {
                    _targetAllyRight = value;
                    OnPropertyChanged(nameof(TargetAllyRight));
                }
            }
        }
        private bool _targetAllyRightEnabled;
        public bool TargetAllyRightEnabled
        {
            get => _targetAllyRightEnabled;
            private set
            {
                if (_targetAllyRightEnabled != value)
                {
                    _targetAllyRightEnabled = value;
                    OnPropertyChanged(nameof(TargetAllyRightEnabled));
                }
            }
        }
        private PBEBattlePokemon _targetFoeLeft;
        public PBEBattlePokemon TargetFoeLeft
        {
            get => _targetFoeLeft;
            private set
            {
                if (_targetFoeLeft != value)
                {
                    _targetFoeLeft = value;
                    OnPropertyChanged(nameof(TargetFoeLeft));
                }
            }
        }
        private bool _targetFoeLeftEnabled;
        public bool TargetFoeLeftEnabled
        {
            get => _targetFoeLeftEnabled;
            private set
            {
                if (_targetFoeLeftEnabled != value)
                {
                    _targetFoeLeftEnabled = value;
                    OnPropertyChanged(nameof(TargetFoeLeftEnabled));
                }
            }
        }
        private PBEBattlePokemon _targetFoeCenter;
        public PBEBattlePokemon TargetFoeCenter
        {
            get => _targetFoeCenter;
            private set
            {
                if (_targetFoeCenter != value)
                {
                    _targetFoeCenter = value;
                    OnPropertyChanged(nameof(TargetFoeCenter));
                }
            }
        }
        private bool _targetFoeCenterEnabled;
        public bool TargetFoeCenterEnabled
        {
            get => _targetFoeCenterEnabled;
            private set
            {
                if (_targetFoeCenterEnabled != value)
                {
                    _targetFoeCenterEnabled = value;
                    OnPropertyChanged(nameof(TargetFoeCenterEnabled));
                }
            }
        }
        private PBEBattlePokemon _targetFoeRight;
        public PBEBattlePokemon TargetFoeRight
        {
            get => _targetFoeRight;
            private set
            {
                if (_targetFoeRight != value)
                {
                    _targetFoeRight = value;
                    OnPropertyChanged(nameof(TargetFoeRight));
                }
            }
        }
        private bool _targetFoeRightEnabled;
        public bool TargetFoeRightEnabled
        {
            get => _targetFoeRightEnabled;
            private set
            {
                if (_targetFoeRightEnabled != value)
                {
                    _targetFoeRightEnabled = value;
                    OnPropertyChanged(nameof(TargetFoeRightEnabled));
                }
            }
        }

        private bool _centerTargetsVisible;
        public bool CenterTargetsVisible
        {
            get => _centerTargetsVisible;
            private set
            {
                if (_centerTargetsVisible != value)
                {
                    _centerTargetsVisible = value;
                    OnPropertyChanged(nameof(CenterTargetsVisible));
                }
            }
        }
        private bool _targetLineFoeLeftFoeCenterEnabled;
        public bool TargetLineFoeLeftFoeCenterEnabled
        {
            get => _targetLineFoeLeftFoeCenterEnabled;
            private set
            {
                if (_targetLineFoeLeftFoeCenterEnabled != value)
                {
                    _targetLineFoeLeftFoeCenterEnabled = value;
                    OnPropertyChanged(nameof(TargetLineFoeLeftFoeCenterEnabled));
                }
            }
        }
        private bool _targetLineFoeLeftAllyRightEnabled;
        public bool TargetLineFoeLeftAllyRightEnabled
        {
            get => _targetLineFoeLeftAllyRightEnabled;
            private set
            {
                if (_targetLineFoeLeftAllyRightEnabled != value)
                {
                    _targetLineFoeLeftAllyRightEnabled = value;
                    OnPropertyChanged(nameof(TargetLineFoeLeftAllyRightEnabled));
                }
            }
        }
        private bool _targetLineFoeCenterAllyCenterEnabled;
        public bool TargetLineFoeCenterAllyCenterEnabled
        {
            get => _targetLineFoeCenterAllyCenterEnabled;
            private set
            {
                if (_targetLineFoeCenterAllyCenterEnabled != value)
                {
                    _targetLineFoeCenterAllyCenterEnabled = value;
                    OnPropertyChanged(nameof(TargetLineFoeCenterAllyCenterEnabled));
                }
            }
        }
        private bool _targetLineFoeRightFoeCenterEnabled;
        public bool TargetLineFoeRightFoeCenterEnabled
        {
            get => _targetLineFoeRightFoeCenterEnabled;
            private set
            {
                if (_targetLineFoeRightFoeCenterEnabled != value)
                {
                    _targetLineFoeRightFoeCenterEnabled = value;
                    OnPropertyChanged(nameof(TargetLineFoeRightFoeCenterEnabled));
                }
            }
        }
        private bool _targetLineFoeRightAllyLeftEnabled;
        public bool TargetLineFoeRightAllyLeftEnabled
        {
            get => _targetLineFoeRightAllyLeftEnabled;
            private set
            {
                if (_targetLineFoeRightAllyLeftEnabled != value)
                {
                    _targetLineFoeRightAllyLeftEnabled = value;
                    OnPropertyChanged(nameof(TargetLineFoeRightAllyLeftEnabled));
                }
            }
        }
        private bool _targetLineAllyLeftAllyCenterEnabled;
        public bool TargetLineAllyLeftAllyCenterEnabled
        {
            get => _targetLineAllyLeftAllyCenterEnabled;
            private set
            {
                if (_targetLineAllyLeftAllyCenterEnabled != value)
                {
                    _targetLineAllyLeftAllyCenterEnabled = value;
                    OnPropertyChanged(nameof(TargetLineAllyLeftAllyCenterEnabled));
                }
            }
        }
        private bool _targetLineAllyRightAllyCenterEnabled;
        public bool TargetLineAllyRightAllyCenterEnabled
        {
            get => _targetLineAllyRightAllyCenterEnabled;
            private set
            {
                if (_targetLineAllyRightAllyCenterEnabled != value)
                {
                    _targetLineAllyRightAllyCenterEnabled = value;
                    OnPropertyChanged(nameof(TargetLineAllyRightAllyCenterEnabled));
                }
            }
        }

        private bool _leftPositionEnabled;
        public bool LeftPositionEnabled
        {
            get => _leftPositionEnabled;
            private set
            {
                if (_leftPositionEnabled != value)
                {
                    _leftPositionEnabled = value;
                    OnPropertyChanged(nameof(LeftPositionEnabled));
                }
            }
        }
        private bool _centerPositionEnabled;
        public bool CenterPositionEnabled
        {
            get => _centerPositionEnabled;
            private set
            {
                if (_centerPositionEnabled != value)
                {
                    _centerPositionEnabled = value;
                    OnPropertyChanged(nameof(CenterPositionEnabled));
                }
            }
        }
        private bool _rightPositionEnabled;
        public bool RightPositionEnabled
        {
            get => _rightPositionEnabled;
            private set
            {
                if (_rightPositionEnabled != value)
                {
                    _rightPositionEnabled = value;
                    OnPropertyChanged(nameof(RightPositionEnabled));
                }
            }
        }

        private PBEMove _fightMove;
        private PBETurnTarget _targetAllyLeftResult, _targetAllyCenterResult, _targetAllyRightResult,
            _targetFoeLeftResult, _targetFoeCenterResult, _targetFoeRightResult;

        private MoveInfo[] _moves;
        public MoveInfo[] Moves
        {
            get => _moves;
            private set
            {
                if (_moves != value)
                {
                    _moves = value;
                    OnPropertyChanged(nameof(Moves));
                }
            }
        }
        private PokemonInfo[] _party;
        public PokemonInfo[] Party
        {
            get => _party;
            private set
            {
                if (_party != value)
                {
                    _party = value;
                    OnPropertyChanged(nameof(Party));
                }
            }
        }

        private bool _targetsVisible;
        public bool TargetsVisible
        {
            get => _targetsVisible;
            private set
            {
                if (_targetsVisible != value)
                {
                    _targetsVisible = value;
                    OnPropertyChanged(nameof(TargetsVisible));
                }
            }
        }
        private bool _movesVisible;
        public bool MovesVisible
        {
            get => _movesVisible;
            private set
            {
                if (_movesVisible != value)
                {
                    _movesVisible = value;
                    OnPropertyChanged(nameof(MovesVisible));
                }
            }
        }
        private bool _switchesVisible;
        public bool SwitchesVisible
        {
            get => _switchesVisible;
            private set
            {
                if (_switchesVisible != value)
                {
                    _switchesVisible = value;
                    OnPropertyChanged(nameof(SwitchesVisible));
                }
            }
        }
        private bool _positionsVisible;
        public bool PositionsVisible
        {
            get => _positionsVisible;
            private set
            {
                if (_positionsVisible != value)
                {
                    _positionsVisible = value;
                    OnPropertyChanged(nameof(PositionsVisible));
                }
            }
        }

        public BattleView BattleView { get; internal set; }
        public PBEBattlePokemon Pokemon { get; private set; }

        public ActionsView()
        {
            DataContext = this;
            AvaloniaXamlLoader.Load(this);
        }

        internal void DisplayActions(PBEBattlePokemon pkmn)
        {
            Pokemon = pkmn;
            PBEMove[] usableMoves = pkmn.GetUsableMoves();
            var mInfo = new MoveInfo[usableMoves.Length];
            for (int i = 0; i < mInfo.Length; i++)
            {
                mInfo[i] = new MoveInfo(pkmn, usableMoves[i], SelectMoveForTurn);
            }
            Moves = mInfo;
            var pInfo = new PokemonInfo[pkmn.Trainer.Party.Count];
            for (int i = 0; i < pInfo.Length; i++)
            {
                PBEBattlePokemon p = pkmn.Trainer.Party[i];
                pInfo[i] = new PokemonInfo(p, !pkmn.CanSwitchOut() || BattleView.Client.StandBy.Contains(p), SelectPokemonForTurn);
            }
            Party = pInfo;
            MovesVisible = true;
            SwitchesVisible = true;
        }
        internal void DisplaySwitches()
        {
            PBEList<PBEBattlePokemon> pa = BattleView.Client.Trainer.Party;
            var pInfo = new PokemonInfo[pa.Count];
            for (int i = 0; i < pa.Count; i++)
            {
                PBEBattlePokemon p = pa[i];
                pInfo[i] = new PokemonInfo(p, BattleView.Client.StandBy.Contains(p), SelectSwitch);
            }
            Party = pInfo;
            SwitchesVisible = true;
        }

        private void SelectPokemonForTurn(PBEBattlePokemon pkmn)
        {
            Pokemon.TurnAction = new PBETurnAction(Pokemon.Id, pkmn.Id);
            BattleView.Client.StandBy.Add(pkmn);
            MovesVisible = false;
            SwitchesVisible = false;
            BattleView.Client.ActionsLoop(false);
        }
        private void SelectMoveForTurn(PBEMove move)
        {
            _fightMove = move;
            MovesVisible = false;
            SwitchesVisible = false;
            DisplayTargets(move);
        }
        private void SelectSwitch(PBEBattlePokemon pkmn)
        {
            Pokemon = pkmn;
            SwitchesVisible = false;
            DisplayPositions();
        }
        private void DisplayTargets(PBEMove move)
        {
            PBEMoveTarget possibleTargets = Pokemon.GetMoveTargets(move);

            if (BattleView.Client.Battle.BattleFormat == PBEBattleFormat.Single || BattleView.Client.Battle.BattleFormat == PBEBattleFormat.Rotation)
            {
                PBETurnTarget targets;
                switch (possibleTargets)
                {
                    case PBEMoveTarget.All: targets = PBETurnTarget.AllyCenter | PBETurnTarget.FoeCenter; break;
                    case PBEMoveTarget.AllFoes:
                    case PBEMoveTarget.AllFoesSurrounding:
                    case PBEMoveTarget.AllSurrounding:
                    case PBEMoveTarget.RandomFoeSurrounding:
                    case PBEMoveTarget.SingleFoeSurrounding:
                    case PBEMoveTarget.SingleNotSelf:
                    case PBEMoveTarget.SingleSurrounding: targets = PBETurnTarget.FoeCenter; break;
                    case PBEMoveTarget.AllTeam:
                    case PBEMoveTarget.Self:
                    case PBEMoveTarget.SelfOrAllySurrounding:
                    case PBEMoveTarget.SingleAllySurrounding: targets = PBETurnTarget.AllyCenter; break;
                    default: throw new ArgumentOutOfRangeException(nameof(possibleTargets));
                }
                Pokemon.TurnAction = new PBETurnAction(Pokemon.Id, move, targets);
                BattleView.Client.ActionsLoop(false);
            }
            else // Double / Triple
            {
                TargetAllyLeft = BattleView.Client.Trainer.Team.TryGetPokemon(PBEFieldPosition.Left);
                TargetAllyCenter = BattleView.Client.Trainer.Team.TryGetPokemon(PBEFieldPosition.Center);
                TargetAllyRight = BattleView.Client.Trainer.Team.TryGetPokemon(PBEFieldPosition.Right);
                TargetFoeLeft = BattleView.Client.Trainer.Team.OpposingTeam.TryGetPokemon(PBEFieldPosition.Left);
                TargetFoeCenter = BattleView.Client.Trainer.Team.OpposingTeam.TryGetPokemon(PBEFieldPosition.Center);
                TargetFoeRight = BattleView.Client.Trainer.Team.OpposingTeam.TryGetPokemon(PBEFieldPosition.Right);

                if (BattleView.Client.Battle.BattleFormat == PBEBattleFormat.Double)
                {
                    CenterTargetsVisible = false;
                    TargetLineFoeCenterAllyCenterEnabled = TargetLineFoeLeftFoeCenterEnabled = TargetLineAllyRightAllyCenterEnabled = false;
                    switch (possibleTargets)
                    {
                        case PBEMoveTarget.All:
                        {
                            TargetAllyLeftEnabled = TargetAllyRightEnabled = TargetFoeLeftEnabled = TargetFoeRightEnabled = true;
                            TargetLineAllyLeftAllyCenterEnabled = TargetLineFoeRightFoeCenterEnabled = TargetLineFoeLeftAllyRightEnabled = TargetLineFoeRightAllyLeftEnabled = true;
                            _targetAllyLeftResult = _targetAllyRightResult = _targetFoeLeftResult = _targetFoeRightResult = PBETurnTarget.AllyLeft | PBETurnTarget.AllyRight | PBETurnTarget.FoeLeft | PBETurnTarget.FoeRight;
                            break;
                        }
                        case PBEMoveTarget.AllFoes:
                        case PBEMoveTarget.AllFoesSurrounding:
                        {
                            TargetAllyLeftEnabled = TargetAllyRightEnabled = false;
                            TargetFoeLeftEnabled = TargetFoeRightEnabled = true;
                            TargetLineAllyLeftAllyCenterEnabled = TargetLineFoeLeftAllyRightEnabled = TargetLineFoeRightAllyLeftEnabled = false;
                            TargetLineFoeRightFoeCenterEnabled = true;
                            _targetFoeLeftResult = _targetFoeRightResult = PBETurnTarget.FoeLeft | PBETurnTarget.FoeRight;
                            break;
                        }
                        case PBEMoveTarget.AllSurrounding:
                        {
                            if (Pokemon.FieldPosition == PBEFieldPosition.Left)
                            {
                                TargetAllyLeftEnabled = false;
                                TargetAllyRightEnabled = true;
                                TargetLineFoeRightAllyLeftEnabled = false;
                                TargetLineFoeLeftAllyRightEnabled = true;
                                _targetAllyRightResult = _targetFoeLeftResult = _targetFoeRightResult = PBETurnTarget.AllyRight | PBETurnTarget.FoeLeft | PBETurnTarget.FoeRight;
                            }
                            else
                            {
                                TargetAllyLeftEnabled = true;
                                TargetAllyRightEnabled = false;
                                TargetLineFoeRightAllyLeftEnabled = true;
                                TargetLineFoeLeftAllyRightEnabled = false;
                                _targetAllyLeftResult = _targetFoeLeftResult = _targetFoeRightResult = PBETurnTarget.AllyLeft | PBETurnTarget.FoeLeft | PBETurnTarget.FoeRight;
                            }
                            TargetFoeLeftEnabled = TargetFoeRightEnabled = true;
                            TargetLineAllyLeftAllyCenterEnabled = false;
                            TargetLineFoeRightFoeCenterEnabled = true;
                            break;
                        }
                        case PBEMoveTarget.AllTeam:
                        {
                            TargetAllyLeftEnabled = TargetAllyRightEnabled = true;
                            TargetFoeLeftEnabled = TargetFoeRightEnabled = false;
                            TargetLineFoeRightFoeCenterEnabled = TargetLineFoeLeftAllyRightEnabled = TargetLineFoeRightAllyLeftEnabled = false;
                            TargetLineAllyLeftAllyCenterEnabled = true;
                            _targetAllyLeftResult = _targetAllyRightResult = PBETurnTarget.AllyLeft | PBETurnTarget.AllyRight;
                            break;
                        }
                        case PBEMoveTarget.RandomFoeSurrounding:
                        case PBEMoveTarget.Self:
                        {
                            if (Pokemon.FieldPosition == PBEFieldPosition.Left)
                            {
                                TargetAllyLeftEnabled = true;
                                TargetAllyRightEnabled = false;
                                _targetAllyLeftResult = PBETurnTarget.AllyLeft;
                            }
                            else
                            {
                                TargetAllyLeftEnabled = false;
                                TargetAllyRightEnabled = true;
                                _targetAllyRightResult = PBETurnTarget.AllyRight;
                            }
                            TargetFoeLeftEnabled = TargetFoeRightEnabled = false;
                            TargetLineAllyLeftAllyCenterEnabled = TargetLineFoeRightFoeCenterEnabled = TargetLineFoeLeftAllyRightEnabled = TargetLineFoeRightAllyLeftEnabled = false;
                            break;
                        }
                        case PBEMoveTarget.SelfOrAllySurrounding:
                        {
                            TargetAllyLeftEnabled = TargetAllyRightEnabled = true;
                            TargetFoeLeftEnabled = TargetFoeRightEnabled = false;
                            TargetLineAllyLeftAllyCenterEnabled = TargetLineFoeRightFoeCenterEnabled = TargetLineFoeLeftAllyRightEnabled = TargetLineFoeRightAllyLeftEnabled = false;
                            _targetAllyLeftResult = PBETurnTarget.AllyLeft;
                            _targetAllyRightResult = PBETurnTarget.AllyRight;
                            break;
                        }
                        case PBEMoveTarget.SingleAllySurrounding:
                        {
                            if (Pokemon.FieldPosition == PBEFieldPosition.Left)
                            {
                                TargetAllyLeftEnabled = false;
                                TargetAllyRightEnabled = true;
                                _targetAllyRightResult = PBETurnTarget.AllyRight;
                            }
                            else
                            {
                                TargetAllyLeftEnabled = true;
                                TargetAllyRightEnabled = false;
                                _targetAllyLeftResult = PBETurnTarget.AllyLeft;
                            }
                            TargetFoeLeftEnabled = TargetFoeRightEnabled = false;
                            TargetLineAllyLeftAllyCenterEnabled = TargetLineFoeRightFoeCenterEnabled = TargetLineFoeLeftAllyRightEnabled = TargetLineFoeRightAllyLeftEnabled = false;
                            break;
                        }
                        case PBEMoveTarget.SingleFoeSurrounding:
                        {
                            TargetAllyLeftEnabled = TargetAllyRightEnabled = false;
                            TargetFoeLeftEnabled = TargetFoeRightEnabled = true;
                            TargetLineAllyLeftAllyCenterEnabled = TargetLineFoeRightFoeCenterEnabled = TargetLineFoeLeftAllyRightEnabled = TargetLineFoeRightAllyLeftEnabled = false;
                            _targetFoeLeftResult = PBETurnTarget.FoeLeft;
                            _targetFoeRightResult = PBETurnTarget.FoeRight;
                            break;
                        }
                        case PBEMoveTarget.SingleNotSelf:
                        case PBEMoveTarget.SingleSurrounding:
                        {
                            if (Pokemon.FieldPosition == PBEFieldPosition.Left)
                            {
                                TargetAllyLeftEnabled = false;
                                TargetAllyRightEnabled = true;
                                _targetAllyRightResult = PBETurnTarget.AllyRight;
                            }
                            else
                            {
                                TargetAllyLeftEnabled = true;
                                TargetAllyRightEnabled = false;
                                _targetAllyLeftResult = PBETurnTarget.AllyLeft;
                            }
                            TargetFoeLeftEnabled = TargetFoeRightEnabled = true;
                            TargetLineAllyLeftAllyCenterEnabled = TargetLineFoeRightFoeCenterEnabled = TargetLineFoeLeftAllyRightEnabled = TargetLineFoeRightAllyLeftEnabled = false;
                            _targetFoeLeftResult = PBETurnTarget.FoeLeft;
                            _targetFoeRightResult = PBETurnTarget.FoeRight;
                            break;
                        }
                        default: throw new ArgumentOutOfRangeException(nameof(possibleTargets));
                    }
                }
                else // Triple
                {
                    CenterTargetsVisible = true;
                    switch (possibleTargets)
                    {
                        case PBEMoveTarget.All:
                        {
                            TargetAllyLeftEnabled = TargetAllyCenterEnabled = TargetAllyRightEnabled = TargetFoeLeftEnabled = TargetFoeCenterEnabled = TargetFoeRightEnabled = true;
                            TargetLineAllyLeftAllyCenterEnabled = TargetLineAllyRightAllyCenterEnabled = TargetLineFoeRightFoeCenterEnabled = TargetLineFoeLeftFoeCenterEnabled = TargetLineFoeCenterAllyCenterEnabled = TargetLineFoeLeftAllyRightEnabled = TargetLineFoeRightAllyLeftEnabled = true;
                            _targetAllyLeftResult = _targetAllyCenterResult = _targetAllyRightResult = _targetFoeLeftResult = _targetFoeCenterResult = _targetFoeRightResult = PBETurnTarget.AllyLeft | PBETurnTarget.AllyCenter | PBETurnTarget.AllyRight | PBETurnTarget.FoeLeft | PBETurnTarget.FoeCenter | PBETurnTarget.FoeRight;
                            break;
                        }
                        case PBEMoveTarget.AllFoes:
                        {
                            TargetAllyLeftEnabled = TargetAllyCenterEnabled = TargetAllyRightEnabled = false;
                            TargetFoeLeftEnabled = TargetFoeCenterEnabled = TargetFoeRightEnabled = true;
                            TargetLineAllyLeftAllyCenterEnabled = TargetLineAllyRightAllyCenterEnabled = TargetLineFoeCenterAllyCenterEnabled = TargetLineFoeLeftAllyRightEnabled = TargetLineFoeRightAllyLeftEnabled = false;
                            TargetLineFoeRightFoeCenterEnabled = TargetLineFoeLeftFoeCenterEnabled = true;
                            _targetFoeLeftResult = _targetFoeCenterResult = _targetFoeRightResult = PBETurnTarget.FoeLeft | PBETurnTarget.FoeCenter | PBETurnTarget.FoeRight;
                            break;
                        }
                        case PBEMoveTarget.AllFoesSurrounding:
                        {
                            if (Pokemon.FieldPosition == PBEFieldPosition.Left)
                            {
                                TargetFoeCenterEnabled = TargetFoeRightEnabled = true;
                                TargetFoeLeftEnabled = false;
                                TargetLineFoeRightFoeCenterEnabled = true;
                                TargetLineFoeLeftFoeCenterEnabled = false;
                                _targetFoeCenterResult = _targetFoeRightResult = PBETurnTarget.FoeCenter | PBETurnTarget.FoeRight;
                            }
                            else if (Pokemon.FieldPosition == PBEFieldPosition.Center)
                            {
                                TargetFoeLeftEnabled = TargetFoeCenterEnabled = TargetFoeRightEnabled = true;
                                TargetLineFoeRightFoeCenterEnabled = TargetLineFoeLeftFoeCenterEnabled = true;
                                _targetFoeLeftResult = _targetFoeCenterResult = _targetFoeRightResult = PBETurnTarget.FoeLeft | PBETurnTarget.FoeCenter | PBETurnTarget.FoeRight;
                            }
                            else
                            {
                                TargetFoeLeftEnabled = TargetFoeCenterEnabled = true;
                                TargetFoeRightEnabled = false;
                                TargetLineFoeLeftFoeCenterEnabled = true;
                                TargetLineFoeRightFoeCenterEnabled = false;
                                _targetFoeLeftResult = _targetFoeCenterResult = PBETurnTarget.FoeLeft | PBETurnTarget.FoeCenter;
                            }
                            TargetAllyLeftEnabled = TargetAllyCenterEnabled = TargetAllyRightEnabled = false;
                            TargetLineFoeRightAllyLeftEnabled = TargetLineAllyLeftAllyCenterEnabled = TargetLineAllyRightAllyCenterEnabled = TargetLineFoeCenterAllyCenterEnabled = TargetLineFoeLeftAllyRightEnabled = false;
                            break;
                        }
                        case PBEMoveTarget.AllSurrounding:
                        {
                            if (Pokemon.FieldPosition == PBEFieldPosition.Left)
                            {
                                TargetFoeRightEnabled = TargetFoeCenterEnabled = TargetAllyCenterEnabled = true;
                                TargetAllyLeftEnabled = TargetAllyRightEnabled = TargetFoeLeftEnabled = false;
                                TargetLineAllyLeftAllyCenterEnabled = TargetLineAllyRightAllyCenterEnabled = TargetLineFoeLeftFoeCenterEnabled = TargetLineFoeLeftAllyRightEnabled = TargetLineFoeRightAllyLeftEnabled = false;
                                TargetLineFoeRightFoeCenterEnabled = TargetLineFoeCenterAllyCenterEnabled = true;
                                _targetAllyCenterResult = _targetFoeCenterResult = _targetFoeRightResult = PBETurnTarget.AllyCenter | PBETurnTarget.FoeCenter | PBETurnTarget.FoeRight;
                            }
                            else if (Pokemon.FieldPosition == PBEFieldPosition.Center)
                            {
                                TargetAllyLeftEnabled = TargetAllyRightEnabled = TargetFoeLeftEnabled = TargetFoeCenterEnabled = TargetFoeRightEnabled = true;
                                TargetAllyCenterEnabled = false;
                                TargetLineAllyLeftAllyCenterEnabled = TargetLineAllyRightAllyCenterEnabled = TargetLineFoeCenterAllyCenterEnabled = false;
                                TargetLineFoeRightFoeCenterEnabled = TargetLineFoeLeftFoeCenterEnabled = TargetLineFoeLeftAllyRightEnabled = TargetLineFoeRightAllyLeftEnabled = true;
                                _targetAllyLeftResult = _targetAllyRightResult = _targetFoeLeftResult = _targetFoeCenterResult = _targetFoeRightResult = PBETurnTarget.AllyLeft | PBETurnTarget.AllyRight | PBETurnTarget.FoeLeft | PBETurnTarget.FoeCenter | PBETurnTarget.FoeRight;
                            }
                            else
                            {
                                TargetAllyLeftEnabled = TargetAllyRightEnabled = TargetFoeRightEnabled = false;
                                TargetAllyCenterEnabled = TargetFoeLeftEnabled = TargetFoeCenterEnabled = true;
                                TargetLineAllyLeftAllyCenterEnabled = TargetLineAllyRightAllyCenterEnabled = TargetLineFoeRightFoeCenterEnabled = TargetLineFoeLeftAllyRightEnabled = TargetLineFoeRightAllyLeftEnabled = false;
                                TargetLineFoeLeftFoeCenterEnabled = TargetLineFoeCenterAllyCenterEnabled = true;
                                _targetAllyCenterResult = _targetFoeLeftResult = _targetFoeCenterResult = PBETurnTarget.AllyCenter | PBETurnTarget.FoeLeft | PBETurnTarget.FoeCenter;
                            }
                            break;
                        }
                        case PBEMoveTarget.AllTeam:
                        {
                            TargetAllyLeftEnabled = TargetAllyCenterEnabled = TargetAllyRightEnabled = true;
                            TargetFoeLeftEnabled = TargetFoeCenterEnabled = TargetFoeRightEnabled = false;
                            TargetLineAllyLeftAllyCenterEnabled = TargetLineAllyRightAllyCenterEnabled = true;
                            TargetLineFoeRightFoeCenterEnabled = TargetLineFoeLeftFoeCenterEnabled = TargetLineFoeCenterAllyCenterEnabled = TargetLineFoeLeftAllyRightEnabled = TargetLineFoeRightAllyLeftEnabled = false;
                            _targetAllyLeftResult = _targetAllyCenterResult = _targetAllyRightResult = PBETurnTarget.AllyLeft | PBETurnTarget.AllyCenter | PBETurnTarget.AllyRight;
                            break;
                        }
                        case PBEMoveTarget.RandomFoeSurrounding:
                        case PBEMoveTarget.Self:
                        {
                            if (Pokemon.FieldPosition == PBEFieldPosition.Left)
                            {
                                TargetAllyLeftEnabled = true;
                                TargetAllyCenterEnabled = TargetAllyRightEnabled = false;
                                _targetAllyLeftResult = PBETurnTarget.AllyLeft;
                            }
                            else if (Pokemon.FieldPosition == PBEFieldPosition.Center)
                            {
                                TargetAllyCenterEnabled = true;
                                TargetAllyLeftEnabled = TargetAllyRightEnabled = false;
                                _targetAllyCenterResult = PBETurnTarget.AllyCenter;
                            }
                            else
                            {
                                TargetAllyRightEnabled = true;
                                TargetAllyLeftEnabled = TargetAllyCenterEnabled = false;
                                _targetAllyRightResult = PBETurnTarget.AllyRight;
                            }
                            TargetFoeLeftEnabled = TargetFoeCenterEnabled = TargetFoeRightEnabled = false;
                            TargetLineAllyLeftAllyCenterEnabled = TargetLineAllyRightAllyCenterEnabled = TargetLineFoeRightFoeCenterEnabled = TargetLineFoeLeftFoeCenterEnabled = TargetLineFoeCenterAllyCenterEnabled = TargetLineFoeLeftAllyRightEnabled = TargetLineFoeRightAllyLeftEnabled = false;
                            break;
                        }
                        case PBEMoveTarget.SelfOrAllySurrounding:
                        {
                            if (Pokemon.FieldPosition == PBEFieldPosition.Left)
                            {
                                TargetAllyLeftEnabled = TargetAllyCenterEnabled = true;
                                TargetAllyRightEnabled = false;
                                _targetAllyLeftResult = PBETurnTarget.AllyLeft;
                                _targetAllyCenterResult = PBETurnTarget.AllyCenter;
                            }
                            else if (Pokemon.FieldPosition == PBEFieldPosition.Center)
                            {
                                TargetAllyCenterEnabled = TargetAllyLeftEnabled = TargetAllyRightEnabled = true;
                                _targetAllyLeftResult = PBETurnTarget.AllyLeft;
                                _targetAllyCenterResult = PBETurnTarget.AllyCenter;
                                _targetAllyRightResult = PBETurnTarget.AllyRight;
                            }
                            else
                            {
                                TargetAllyCenterEnabled = TargetAllyRightEnabled = true;
                                TargetAllyLeftEnabled = false;
                                _targetAllyCenterResult = PBETurnTarget.AllyCenter;
                                _targetAllyRightResult = PBETurnTarget.AllyRight;
                            }
                            TargetFoeLeftEnabled = TargetFoeCenterEnabled = TargetFoeRightEnabled = false;
                            TargetLineAllyLeftAllyCenterEnabled = TargetLineAllyRightAllyCenterEnabled = TargetLineFoeRightFoeCenterEnabled = TargetLineFoeLeftFoeCenterEnabled = TargetLineFoeCenterAllyCenterEnabled = TargetLineFoeLeftAllyRightEnabled = TargetLineFoeRightAllyLeftEnabled = false;
                            break;
                        }
                        case PBEMoveTarget.SingleAllySurrounding:
                        {
                            if (Pokemon.FieldPosition == PBEFieldPosition.Left || Pokemon.FieldPosition == PBEFieldPosition.Right)
                            {
                                TargetAllyCenterEnabled = true;
                                TargetAllyLeftEnabled = TargetAllyRightEnabled = false;
                                _targetAllyCenterResult = PBETurnTarget.AllyCenter;
                            }
                            else
                            {
                                TargetAllyCenterEnabled = false;
                                TargetAllyLeftEnabled = TargetAllyRightEnabled = true;
                                _targetAllyLeftResult = PBETurnTarget.AllyLeft;
                                _targetAllyRightResult = PBETurnTarget.AllyRight;
                            }
                            TargetFoeLeftEnabled = TargetFoeCenterEnabled = TargetFoeRightEnabled = false;
                            TargetLineAllyLeftAllyCenterEnabled = TargetLineAllyRightAllyCenterEnabled = TargetLineFoeRightFoeCenterEnabled = TargetLineFoeLeftFoeCenterEnabled = TargetLineFoeCenterAllyCenterEnabled = TargetLineFoeLeftAllyRightEnabled = TargetLineFoeRightAllyLeftEnabled = false;
                            break;
                        }
                        case PBEMoveTarget.SingleFoeSurrounding:
                        {
                            if (Pokemon.FieldPosition == PBEFieldPosition.Left)
                            {
                                TargetFoeCenterEnabled = TargetFoeRightEnabled = true;
                                TargetFoeLeftEnabled = false;
                                _targetFoeCenterResult = PBETurnTarget.FoeCenter;
                                _targetFoeRightResult = PBETurnTarget.FoeRight;
                            }
                            else if (Pokemon.FieldPosition == PBEFieldPosition.Center)
                            {
                                TargetFoeLeftEnabled = TargetFoeCenterEnabled = TargetFoeRightEnabled = true;
                                _targetFoeLeftResult = PBETurnTarget.FoeLeft;
                                _targetFoeCenterResult = PBETurnTarget.FoeCenter;
                                _targetFoeRightResult = PBETurnTarget.FoeRight;
                            }
                            else
                            {
                                TargetFoeLeftEnabled = TargetFoeCenterEnabled = true;
                                TargetFoeRightEnabled = false;
                                _targetFoeLeftResult = PBETurnTarget.FoeLeft;
                                _targetFoeCenterResult = PBETurnTarget.FoeCenter;
                            }
                            TargetAllyLeftEnabled = TargetAllyCenterEnabled = TargetAllyRightEnabled = false;
                            TargetLineAllyLeftAllyCenterEnabled = TargetLineAllyRightAllyCenterEnabled = TargetLineFoeRightFoeCenterEnabled = TargetLineFoeLeftFoeCenterEnabled = TargetLineFoeCenterAllyCenterEnabled = TargetLineFoeLeftAllyRightEnabled = TargetLineFoeRightAllyLeftEnabled = false;
                            break;
                        }
                        case PBEMoveTarget.SingleNotSelf:
                        {
                            if (Pokemon.FieldPosition == PBEFieldPosition.Left)
                            {
                                TargetAllyLeftEnabled = false;
                                TargetAllyCenterEnabled = TargetAllyRightEnabled = true;
                                _targetAllyCenterResult = PBETurnTarget.AllyCenter;
                                _targetAllyRightResult = PBETurnTarget.AllyRight;
                            }
                            else if (Pokemon.FieldPosition == PBEFieldPosition.Center)
                            {
                                TargetAllyCenterEnabled = false;
                                TargetAllyLeftEnabled = TargetAllyRightEnabled = true;
                                _targetAllyLeftResult = PBETurnTarget.AllyLeft;
                                _targetAllyRightResult = PBETurnTarget.AllyRight;
                            }
                            else
                            {
                                TargetAllyRightEnabled = false;
                                TargetAllyLeftEnabled = TargetAllyCenterEnabled = true;
                                _targetAllyLeftResult = PBETurnTarget.AllyLeft;
                                _targetAllyCenterResult = PBETurnTarget.AllyCenter;
                            }
                            TargetFoeLeftEnabled = TargetFoeCenterEnabled = TargetFoeRightEnabled = true;
                            TargetLineAllyLeftAllyCenterEnabled = TargetLineAllyRightAllyCenterEnabled = TargetLineFoeRightFoeCenterEnabled = TargetLineFoeLeftFoeCenterEnabled = TargetLineFoeCenterAllyCenterEnabled = TargetLineFoeLeftAllyRightEnabled = TargetLineFoeRightAllyLeftEnabled = false;
                            _targetFoeLeftResult = PBETurnTarget.FoeLeft;
                            _targetFoeCenterResult = PBETurnTarget.FoeCenter;
                            _targetFoeRightResult = PBETurnTarget.FoeRight;
                            break;
                        }
                        case PBEMoveTarget.SingleSurrounding:
                        {
                            if (Pokemon.FieldPosition == PBEFieldPosition.Left)
                            {
                                TargetAllyCenterEnabled = TargetFoeCenterEnabled = TargetFoeRightEnabled = true;
                                TargetAllyLeftEnabled = TargetAllyRightEnabled = TargetFoeLeftEnabled = false;
                                _targetAllyCenterResult = PBETurnTarget.AllyCenter;
                                _targetFoeCenterResult = PBETurnTarget.FoeCenter;
                                _targetFoeRightResult = PBETurnTarget.FoeRight;
                            }
                            else if (Pokemon.FieldPosition == PBEFieldPosition.Center)
                            {
                                TargetAllyLeftEnabled = TargetAllyRightEnabled = TargetFoeLeftEnabled = TargetFoeCenterEnabled = TargetFoeRightEnabled = true;
                                TargetAllyCenterEnabled = false;
                                _targetAllyLeftResult = PBETurnTarget.AllyLeft;
                                _targetAllyRightResult = PBETurnTarget.AllyRight;
                                _targetFoeLeftResult = PBETurnTarget.FoeLeft;
                                _targetFoeCenterResult = PBETurnTarget.FoeCenter;
                                _targetFoeRightResult = PBETurnTarget.FoeRight;
                            }
                            else
                            {
                                TargetAllyCenterEnabled = TargetFoeLeftEnabled = TargetFoeCenterEnabled = true;
                                TargetAllyLeftEnabled = TargetAllyRightEnabled = TargetFoeRightEnabled = false;
                                _targetAllyCenterResult = PBETurnTarget.AllyCenter;
                                _targetFoeLeftResult = PBETurnTarget.FoeLeft;
                                _targetFoeCenterResult = PBETurnTarget.FoeCenter;
                            }
                            TargetLineAllyLeftAllyCenterEnabled = TargetLineAllyRightAllyCenterEnabled = TargetLineFoeRightFoeCenterEnabled = TargetLineFoeLeftFoeCenterEnabled = TargetLineFoeCenterAllyCenterEnabled = TargetLineFoeLeftAllyRightEnabled = TargetLineFoeRightAllyLeftEnabled = false;
                            break;
                        }
                        default: throw new ArgumentOutOfRangeException(nameof(possibleTargets));
                    }
                }

                // This would still show the lines if a move had lines
                if (Pokemon.TempLockedTargets != PBETurnTarget.None)
                {
                    if (!Pokemon.TempLockedTargets.HasFlag(PBETurnTarget.AllyLeft))
                    {
                        TargetAllyLeftEnabled = false;
                    }
                    if (!Pokemon.TempLockedTargets.HasFlag(PBETurnTarget.AllyCenter))
                    {
                        TargetAllyCenterEnabled = false;
                    }
                    if (!Pokemon.TempLockedTargets.HasFlag(PBETurnTarget.AllyRight))
                    {
                        TargetAllyRightEnabled = false;
                    }
                    if (!Pokemon.TempLockedTargets.HasFlag(PBETurnTarget.FoeLeft))
                    {
                        TargetFoeLeftEnabled = false;
                    }
                    if (!Pokemon.TempLockedTargets.HasFlag(PBETurnTarget.FoeCenter))
                    {
                        TargetFoeCenterEnabled = false;
                    }
                    if (!Pokemon.TempLockedTargets.HasFlag(PBETurnTarget.FoeRight))
                    {
                        TargetFoeRightEnabled = false;
                    }
                }

                TargetsVisible = true;
            }
        }
        private void DisplayPositions()
        {
            LeftPositionEnabled = CenterPositionEnabled = RightPositionEnabled = false;
            switch (BattleView.Client.Battle.BattleFormat)
            {
                case PBEBattleFormat.Single:
                {
                    SelectPosition("Center");
                    break;
                }
                case PBEBattleFormat.Double:
                {
                    LeftPositionEnabled = !BattleView.Client.PositionStandBy.Contains(PBEFieldPosition.Left) && BattleView.Client.Trainer.OwnsSpot(PBEFieldPosition.Left) && BattleView.Client.Trainer.Team.TryGetPokemon(PBEFieldPosition.Left) == null;
                    RightPositionEnabled = !BattleView.Client.PositionStandBy.Contains(PBEFieldPosition.Right) && BattleView.Client.Trainer.OwnsSpot(PBEFieldPosition.Right) && BattleView.Client.Trainer.Team.TryGetPokemon(PBEFieldPosition.Right) == null;
                    if (_leftPositionEnabled && !_rightPositionEnabled)
                    {
                        SelectPosition("Left");
                    }
                    else if (!_leftPositionEnabled && _rightPositionEnabled)
                    {
                        SelectPosition("Right");
                    }
                    else
                    {
                        BattleView.AddMessage($"Send {Pokemon.Nickname} where?", messageLog: false);
                        PositionsVisible = true;
                    }
                    break;
                }
                case PBEBattleFormat.Triple:
                case PBEBattleFormat.Rotation:
                {
                    LeftPositionEnabled = !BattleView.Client.PositionStandBy.Contains(PBEFieldPosition.Left) && BattleView.Client.Trainer.OwnsSpot(PBEFieldPosition.Left) && BattleView.Client.Trainer.Team.TryGetPokemon(PBEFieldPosition.Left) == null;
                    CenterPositionEnabled = !BattleView.Client.PositionStandBy.Contains(PBEFieldPosition.Center) && BattleView.Client.Trainer.OwnsSpot(PBEFieldPosition.Center) && BattleView.Client.Trainer.Team.TryGetPokemon(PBEFieldPosition.Center) == null;
                    RightPositionEnabled = !BattleView.Client.PositionStandBy.Contains(PBEFieldPosition.Right) && BattleView.Client.Trainer.OwnsSpot(PBEFieldPosition.Right) && BattleView.Client.Trainer.Team.TryGetPokemon(PBEFieldPosition.Right) == null;
                    if (_leftPositionEnabled && !_centerPositionEnabled && !_rightPositionEnabled)
                    {
                        SelectPosition("Left");
                    }
                    else if (!_leftPositionEnabled && _centerPositionEnabled && !_rightPositionEnabled)
                    {
                        SelectPosition("Center");
                    }
                    else if (!_leftPositionEnabled && !_centerPositionEnabled && _rightPositionEnabled)
                    {
                        SelectPosition("Right");
                    }
                    else
                    {
                        BattleView.AddMessage($"Send {Pokemon.Nickname} where?", messageLog: false);
                        PositionsVisible = true;
                    }
                    break;
                }
                default: throw new ArgumentOutOfRangeException(nameof(BattleView.Client.Battle.BattleFormat));
            }
        }
        public void SelectTarget(string arg)
        {
            PBETurnTarget targets;
            switch (arg)
            {
                case "AllyLeft": targets = _targetAllyLeftResult; break;
                case "AllyCenter": targets = _targetAllyCenterResult; break;
                case "AllyRight": targets = _targetAllyRightResult; break;
                case "FoeLeft": targets = _targetFoeLeftResult; break;
                case "FoeCenter": targets = _targetFoeCenterResult; break;
                case "FoeRight": targets = _targetFoeRightResult; break;
                default: throw new ArgumentOutOfRangeException(nameof(arg));
            }
            TargetsVisible = false;
            Pokemon.TurnAction = new PBETurnAction(Pokemon.Id, _fightMove, targets);
            BattleView.Client.ActionsLoop(false);
        }
        public void SelectPosition(string arg)
        {
            var pos = (PBEFieldPosition)Enum.Parse(typeof(PBEFieldPosition), arg);
            BattleView.Client.Switches.Add(new PBESwitchIn(Pokemon.Id, pos));
            BattleView.Client.StandBy.Add(Pokemon);
            BattleView.Client.PositionStandBy.Add(pos);
            PositionsVisible = false;
            BattleView.Client.SwitchesLoop(false);
        }
    }
}
