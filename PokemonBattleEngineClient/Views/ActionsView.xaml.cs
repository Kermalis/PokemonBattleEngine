using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Kermalis.PokemonBattleEngine.Battle;
using Kermalis.PokemonBattleEngine.Data;
using Kermalis.PokemonBattleEngineClient.Clients;
using Kermalis.PokemonBattleEngineClient.Models;
using System;
using System.ComponentModel;
using System.IO;

namespace Kermalis.PokemonBattleEngineClient.Views
{
    public sealed class ActionsView : UserControl, INotifyPropertyChanged
    {
        private void OnPropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }
        public new event PropertyChangedEventHandler? PropertyChanged;

        public TargetInfo TargetAllyLeft { get; } = new TargetInfo();
        public TargetInfo TargetAllyCenter { get; } = new TargetInfo();
        public TargetInfo TargetAllyRight { get; } = new TargetInfo();
        public TargetInfo TargetFoeLeft { get; } = new TargetInfo();
        public TargetInfo TargetFoeCenter { get; } = new TargetInfo();
        public TargetInfo TargetFoeRight { get; } = new TargetInfo();

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
        private SwitchInfo[] _party;
        public SwitchInfo[] Party
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
        internal ActionsBuilder? ActionsBuilder { get; set; }
        internal SwitchesBuilder? SwitchesBuilder { get; set; }
        public PBEBattlePokemon Pokemon { get; private set; }

        public ActionsView()
        {
            // These are set in the appropriate states
            _moves = null!;
            _party = null!;
            BattleView = null!;
            Pokemon = null!;
            // Avalonia setup
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
            var pInfo = new SwitchInfo[pkmn.Trainer.Party.Count];
            for (int i = 0; i < pInfo.Length; i++)
            {
                PBEBattlePokemon p = pkmn.Trainer.Party[i];
                pInfo[i] = new SwitchInfo(p, !pkmn.CanSwitchOut() || ActionsBuilder!.IsStandBy(p), SelectPokemonForTurn);
            }
            Party = pInfo;
            MovesVisible = true;
            SwitchesVisible = true;
        }
        internal void DisplaySwitches()
        {
            PBEList<PBEBattlePokemon> pa = BattleView.Client.Trainer!.Party;
            var pInfo = new SwitchInfo[pa.Count];
            for (int i = 0; i < pa.Count; i++)
            {
                PBEBattlePokemon p = pa[i];
                pInfo[i] = new SwitchInfo(p, SwitchesBuilder!.IsStandBy(p), SelectSwitch);
            }
            Party = pInfo;
            SwitchesVisible = true;
        }

        private void SelectPokemonForTurn(PBEBattlePokemon pkmn)
        {
            MovesVisible = false;
            SwitchesVisible = false;
            ActionsBuilder!.EnqueueSwitch(pkmn);
        }
        private void SelectMoveForTurn(PBEMove move)
        {
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

            BattleClient client = BattleView.Client;
            if (client.Battle.BattleFormat == PBEBattleFormat.Single || client.Battle.BattleFormat == PBEBattleFormat.Rotation)
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
                    default: throw new InvalidDataException(nameof(possibleTargets));
                }
                ActionsBuilder!.EnqueueMove(move, targets);
                return;
            }
            // Double / Triple
            PBETeam team = client.Trainer!.Team;
            PBETeam oTeam = team.OpposingTeam;
            _fightMove = move;
            TargetAllyLeft.Pokemon = PokemonInfo.From(client, team, PBEFieldPosition.Left);
            TargetAllyCenter.Pokemon = PokemonInfo.From(client, team, PBEFieldPosition.Center);
            TargetAllyRight.Pokemon = PokemonInfo.From(client, team, PBEFieldPosition.Right);
            TargetFoeLeft.Pokemon = PokemonInfo.From(client, oTeam, PBEFieldPosition.Left);
            TargetFoeCenter.Pokemon = PokemonInfo.From(client, oTeam, PBEFieldPosition.Center);
            TargetFoeRight.Pokemon = PokemonInfo.From(client, oTeam, PBEFieldPosition.Right);

            if (client.Battle.BattleFormat == PBEBattleFormat.Double)
            {
                CenterTargetsVisible = false;
                TargetFoeCenter.LineDownVisible = TargetFoeCenter.LineRightVisible = TargetAllyCenter.LineRightVisible = false;
                switch (possibleTargets)
                {
                    case PBEMoveTarget.All:
                    {
                        TargetAllyLeft.Enabled = TargetAllyRight.Enabled = TargetFoeLeft.Enabled = TargetFoeRight.Enabled = true;
                        TargetAllyLeft.LineRightVisible = TargetFoeRight.LineRightVisible = TargetFoeLeft.LineDownVisible = TargetFoeRight.LineDownVisible = true;
                        TargetAllyLeft.Targets = TargetAllyRight.Targets = TargetFoeLeft.Targets = TargetFoeRight.Targets = PBETurnTarget.AllyLeft | PBETurnTarget.AllyRight | PBETurnTarget.FoeLeft | PBETurnTarget.FoeRight;
                        break;
                    }
                    case PBEMoveTarget.AllFoes:
                    case PBEMoveTarget.AllFoesSurrounding:
                    {
                        TargetAllyLeft.Enabled = TargetAllyRight.Enabled = false;
                        TargetFoeLeft.Enabled = TargetFoeRight.Enabled = true;
                        TargetAllyLeft.LineRightVisible = TargetFoeLeft.LineDownVisible = TargetFoeRight.LineDownVisible = false;
                        TargetFoeRight.LineRightVisible = true;
                        TargetFoeLeft.Targets = TargetFoeRight.Targets = PBETurnTarget.FoeLeft | PBETurnTarget.FoeRight;
                        break;
                    }
                    case PBEMoveTarget.AllSurrounding:
                    {
                        if (Pokemon.FieldPosition == PBEFieldPosition.Left)
                        {
                            TargetAllyLeft.Enabled = false;
                            TargetAllyRight.Enabled = true;
                            TargetFoeRight.LineDownVisible = false;
                            TargetFoeLeft.LineDownVisible = true;
                            TargetAllyRight.Targets = TargetFoeLeft.Targets = TargetFoeRight.Targets = PBETurnTarget.AllyRight | PBETurnTarget.FoeLeft | PBETurnTarget.FoeRight;
                        }
                        else
                        {
                            TargetAllyLeft.Enabled = true;
                            TargetAllyRight.Enabled = false;
                            TargetFoeRight.LineDownVisible = true;
                            TargetFoeLeft.LineDownVisible = false;
                            TargetAllyLeft.Targets = TargetFoeLeft.Targets = TargetFoeRight.Targets = PBETurnTarget.AllyLeft | PBETurnTarget.FoeLeft | PBETurnTarget.FoeRight;
                        }
                        TargetFoeLeft.Enabled = TargetFoeRight.Enabled = true;
                        TargetAllyLeft.LineRightVisible = false;
                        TargetFoeRight.LineRightVisible = true;
                        break;
                    }
                    case PBEMoveTarget.AllTeam:
                    {
                        TargetAllyLeft.Enabled = TargetAllyRight.Enabled = true;
                        TargetFoeLeft.Enabled = TargetFoeRight.Enabled = false;
                        TargetFoeRight.LineRightVisible = TargetFoeLeft.LineDownVisible = TargetFoeRight.LineDownVisible = false;
                        TargetAllyLeft.LineRightVisible = true;
                        TargetAllyLeft.Targets = TargetAllyRight.Targets = PBETurnTarget.AllyLeft | PBETurnTarget.AllyRight;
                        break;
                    }
                    case PBEMoveTarget.RandomFoeSurrounding:
                    case PBEMoveTarget.Self:
                    {
                        if (Pokemon.FieldPosition == PBEFieldPosition.Left)
                        {
                            TargetAllyLeft.Enabled = true;
                            TargetAllyRight.Enabled = false;
                            TargetAllyLeft.Targets = PBETurnTarget.AllyLeft;
                        }
                        else
                        {
                            TargetAllyLeft.Enabled = false;
                            TargetAllyRight.Enabled = true;
                            TargetAllyRight.Targets = PBETurnTarget.AllyRight;
                        }
                        TargetFoeLeft.Enabled = TargetFoeRight.Enabled = false;
                        TargetAllyLeft.LineRightVisible = TargetFoeRight.LineRightVisible = TargetFoeLeft.LineDownVisible = TargetFoeRight.LineDownVisible = false;
                        break;
                    }
                    case PBEMoveTarget.SelfOrAllySurrounding:
                    {
                        TargetAllyLeft.Enabled = TargetAllyRight.Enabled = true;
                        TargetFoeLeft.Enabled = TargetFoeRight.Enabled = false;
                        TargetAllyLeft.LineRightVisible = TargetFoeRight.LineRightVisible = TargetFoeLeft.LineDownVisible = TargetFoeRight.LineDownVisible = false;
                        TargetAllyLeft.Targets = PBETurnTarget.AllyLeft;
                        TargetAllyRight.Targets = PBETurnTarget.AllyRight;
                        break;
                    }
                    case PBEMoveTarget.SingleAllySurrounding:
                    {
                        if (Pokemon.FieldPosition == PBEFieldPosition.Left)
                        {
                            TargetAllyLeft.Enabled = false;
                            TargetAllyRight.Enabled = true;
                            TargetAllyRight.Targets = PBETurnTarget.AllyRight;
                        }
                        else
                        {
                            TargetAllyLeft.Enabled = true;
                            TargetAllyRight.Enabled = false;
                            TargetAllyLeft.Targets = PBETurnTarget.AllyLeft;
                        }
                        TargetFoeLeft.Enabled = TargetFoeRight.Enabled = false;
                        TargetAllyLeft.LineRightVisible = TargetFoeRight.LineRightVisible = TargetFoeLeft.LineDownVisible = TargetFoeRight.LineDownVisible = false;
                        break;
                    }
                    case PBEMoveTarget.SingleFoeSurrounding:
                    {
                        TargetAllyLeft.Enabled = TargetAllyRight.Enabled = false;
                        TargetFoeLeft.Enabled = TargetFoeRight.Enabled = true;
                        TargetAllyLeft.LineRightVisible = TargetFoeRight.LineRightVisible = TargetFoeLeft.LineDownVisible = TargetFoeRight.LineDownVisible = false;
                        TargetFoeLeft.Targets = PBETurnTarget.FoeLeft;
                        TargetFoeRight.Targets = PBETurnTarget.FoeRight;
                        break;
                    }
                    case PBEMoveTarget.SingleNotSelf:
                    case PBEMoveTarget.SingleSurrounding:
                    {
                        if (Pokemon.FieldPosition == PBEFieldPosition.Left)
                        {
                            TargetAllyLeft.Enabled = false;
                            TargetAllyRight.Enabled = true;
                            TargetAllyRight.Targets = PBETurnTarget.AllyRight;
                        }
                        else
                        {
                            TargetAllyLeft.Enabled = true;
                            TargetAllyRight.Enabled = false;
                            TargetAllyLeft.Targets = PBETurnTarget.AllyLeft;
                        }
                        TargetFoeLeft.Enabled = TargetFoeRight.Enabled = true;
                        TargetAllyLeft.LineRightVisible = TargetFoeRight.LineRightVisible = TargetFoeLeft.LineDownVisible = TargetFoeRight.LineDownVisible = false;
                        TargetFoeLeft.Targets = PBETurnTarget.FoeLeft;
                        TargetFoeRight.Targets = PBETurnTarget.FoeRight;
                        break;
                    }
                    default: throw new InvalidDataException(nameof(possibleTargets));
                }
            }
            else // Triple
            {
                CenterTargetsVisible = true;
                switch (possibleTargets)
                {
                    case PBEMoveTarget.All:
                    {
                        TargetAllyLeft.Enabled = TargetAllyCenter.Enabled = TargetAllyRight.Enabled = TargetFoeLeft.Enabled = TargetFoeCenter.Enabled = TargetFoeRight.Enabled = true;
                        TargetAllyLeft.LineRightVisible = TargetAllyCenter.LineRightVisible = TargetFoeRight.LineRightVisible = TargetFoeCenter.LineRightVisible = TargetFoeCenter.LineDownVisible = TargetFoeLeft.LineDownVisible = TargetFoeRight.LineDownVisible = true;
                        TargetAllyLeft.Targets = TargetAllyCenter.Targets = TargetAllyRight.Targets = TargetFoeLeft.Targets = TargetFoeCenter.Targets = TargetFoeRight.Targets = PBETurnTarget.AllyLeft | PBETurnTarget.AllyCenter | PBETurnTarget.AllyRight | PBETurnTarget.FoeLeft | PBETurnTarget.FoeCenter | PBETurnTarget.FoeRight;
                        break;
                    }
                    case PBEMoveTarget.AllFoes:
                    {
                        TargetAllyLeft.Enabled = TargetAllyCenter.Enabled = TargetAllyRight.Enabled = false;
                        TargetFoeLeft.Enabled = TargetFoeCenter.Enabled = TargetFoeRight.Enabled = true;
                        TargetAllyLeft.LineRightVisible = TargetAllyCenter.LineRightVisible = TargetFoeCenter.LineDownVisible = TargetFoeLeft.LineDownVisible = TargetFoeRight.LineDownVisible = false;
                        TargetFoeRight.LineRightVisible = TargetFoeCenter.LineRightVisible = true;
                        TargetFoeLeft.Targets = TargetFoeCenter.Targets = TargetFoeRight.Targets = PBETurnTarget.FoeLeft | PBETurnTarget.FoeCenter | PBETurnTarget.FoeRight;
                        break;
                    }
                    case PBEMoveTarget.AllFoesSurrounding:
                    {
                        if (Pokemon.FieldPosition == PBEFieldPosition.Left)
                        {
                            TargetFoeCenter.Enabled = TargetFoeRight.Enabled = true;
                            TargetFoeLeft.Enabled = false;
                            TargetFoeRight.LineRightVisible = true;
                            TargetFoeCenter.LineRightVisible = false;
                            TargetFoeCenter.Targets = TargetFoeRight.Targets = PBETurnTarget.FoeCenter | PBETurnTarget.FoeRight;
                        }
                        else if (Pokemon.FieldPosition == PBEFieldPosition.Center)
                        {
                            TargetFoeLeft.Enabled = TargetFoeCenter.Enabled = TargetFoeRight.Enabled = true;
                            TargetFoeRight.LineRightVisible = TargetFoeCenter.LineRightVisible = true;
                            TargetFoeLeft.Targets = TargetFoeCenter.Targets = TargetFoeRight.Targets = PBETurnTarget.FoeLeft | PBETurnTarget.FoeCenter | PBETurnTarget.FoeRight;
                        }
                        else
                        {
                            TargetFoeLeft.Enabled = TargetFoeCenter.Enabled = true;
                            TargetFoeRight.Enabled = false;
                            TargetFoeCenter.LineRightVisible = true;
                            TargetFoeRight.LineRightVisible = false;
                            TargetFoeLeft.Targets = TargetFoeCenter.Targets = PBETurnTarget.FoeLeft | PBETurnTarget.FoeCenter;
                        }
                        TargetAllyLeft.Enabled = TargetAllyCenter.Enabled = TargetAllyRight.Enabled = false;
                        TargetFoeRight.LineDownVisible = TargetAllyLeft.LineRightVisible = TargetAllyCenter.LineRightVisible = TargetFoeCenter.LineDownVisible = TargetFoeLeft.LineDownVisible = false;
                        break;
                    }
                    case PBEMoveTarget.AllSurrounding:
                    {
                        if (Pokemon.FieldPosition == PBEFieldPosition.Left)
                        {
                            TargetFoeRight.Enabled = TargetFoeCenter.Enabled = TargetAllyCenter.Enabled = true;
                            TargetAllyLeft.Enabled = TargetAllyRight.Enabled = TargetFoeLeft.Enabled = false;
                            TargetAllyLeft.LineRightVisible = TargetAllyCenter.LineRightVisible = TargetFoeCenter.LineRightVisible = TargetFoeLeft.LineDownVisible = TargetFoeRight.LineDownVisible = false;
                            TargetFoeRight.LineRightVisible = TargetFoeCenter.LineDownVisible = true;
                            TargetAllyCenter.Targets = TargetFoeCenter.Targets = TargetFoeRight.Targets = PBETurnTarget.AllyCenter | PBETurnTarget.FoeCenter | PBETurnTarget.FoeRight;
                        }
                        else if (Pokemon.FieldPosition == PBEFieldPosition.Center)
                        {
                            TargetAllyLeft.Enabled = TargetAllyRight.Enabled = TargetFoeLeft.Enabled = TargetFoeCenter.Enabled = TargetFoeRight.Enabled = true;
                            TargetAllyCenter.Enabled = false;
                            TargetAllyLeft.LineRightVisible = TargetAllyCenter.LineRightVisible = TargetFoeCenter.LineDownVisible = false;
                            TargetFoeRight.LineRightVisible = TargetFoeCenter.LineRightVisible = TargetFoeLeft.LineDownVisible = TargetFoeRight.LineDownVisible = true;
                            TargetAllyLeft.Targets = TargetAllyRight.Targets = TargetFoeLeft.Targets = TargetFoeCenter.Targets = TargetFoeRight.Targets = PBETurnTarget.AllyLeft | PBETurnTarget.AllyRight | PBETurnTarget.FoeLeft | PBETurnTarget.FoeCenter | PBETurnTarget.FoeRight;
                        }
                        else
                        {
                            TargetAllyLeft.Enabled = TargetAllyRight.Enabled = TargetFoeRight.Enabled = false;
                            TargetAllyCenter.Enabled = TargetFoeLeft.Enabled = TargetFoeCenter.Enabled = true;
                            TargetAllyLeft.LineRightVisible = TargetAllyCenter.LineRightVisible = TargetFoeRight.LineRightVisible = TargetFoeLeft.LineDownVisible = TargetFoeRight.LineDownVisible = false;
                            TargetFoeCenter.LineRightVisible = TargetFoeCenter.LineDownVisible = true;
                            TargetAllyCenter.Targets = TargetFoeLeft.Targets = TargetFoeCenter.Targets = PBETurnTarget.AllyCenter | PBETurnTarget.FoeLeft | PBETurnTarget.FoeCenter;
                        }
                        break;
                    }
                    case PBEMoveTarget.AllTeam:
                    {
                        TargetAllyLeft.Enabled = TargetAllyCenter.Enabled = TargetAllyRight.Enabled = true;
                        TargetFoeLeft.Enabled = TargetFoeCenter.Enabled = TargetFoeRight.Enabled = false;
                        TargetAllyLeft.LineRightVisible = TargetAllyCenter.LineRightVisible = true;
                        TargetFoeRight.LineRightVisible = TargetFoeCenter.LineRightVisible = TargetFoeCenter.LineDownVisible = TargetFoeLeft.LineDownVisible = TargetFoeRight.LineDownVisible = false;
                        TargetAllyLeft.Targets = TargetAllyCenter.Targets = TargetAllyRight.Targets = PBETurnTarget.AllyLeft | PBETurnTarget.AllyCenter | PBETurnTarget.AllyRight;
                        break;
                    }
                    case PBEMoveTarget.RandomFoeSurrounding:
                    case PBEMoveTarget.Self:
                    {
                        if (Pokemon.FieldPosition == PBEFieldPosition.Left)
                        {
                            TargetAllyLeft.Enabled = true;
                            TargetAllyCenter.Enabled = TargetAllyRight.Enabled = false;
                            TargetAllyLeft.Targets = PBETurnTarget.AllyLeft;
                        }
                        else if (Pokemon.FieldPosition == PBEFieldPosition.Center)
                        {
                            TargetAllyCenter.Enabled = true;
                            TargetAllyLeft.Enabled = TargetAllyRight.Enabled = false;
                            TargetAllyCenter.Targets = PBETurnTarget.AllyCenter;
                        }
                        else
                        {
                            TargetAllyRight.Enabled = true;
                            TargetAllyLeft.Enabled = TargetAllyCenter.Enabled = false;
                            TargetAllyRight.Targets = PBETurnTarget.AllyRight;
                        }
                        TargetFoeLeft.Enabled = TargetFoeCenter.Enabled = TargetFoeRight.Enabled = false;
                        TargetAllyLeft.LineRightVisible = TargetAllyCenter.LineRightVisible = TargetFoeRight.LineRightVisible = TargetFoeCenter.LineRightVisible = TargetFoeCenter.LineDownVisible = TargetFoeLeft.LineDownVisible = TargetFoeRight.LineDownVisible = false;
                        break;
                    }
                    case PBEMoveTarget.SelfOrAllySurrounding:
                    {
                        if (Pokemon.FieldPosition == PBEFieldPosition.Left)
                        {
                            TargetAllyLeft.Enabled = TargetAllyCenter.Enabled = true;
                            TargetAllyRight.Enabled = false;
                            TargetAllyLeft.Targets = PBETurnTarget.AllyLeft;
                            TargetAllyCenter.Targets = PBETurnTarget.AllyCenter;
                        }
                        else if (Pokemon.FieldPosition == PBEFieldPosition.Center)
                        {
                            TargetAllyCenter.Enabled = TargetAllyLeft.Enabled = TargetAllyRight.Enabled = true;
                            TargetAllyLeft.Targets = PBETurnTarget.AllyLeft;
                            TargetAllyCenter.Targets = PBETurnTarget.AllyCenter;
                            TargetAllyRight.Targets = PBETurnTarget.AllyRight;
                        }
                        else
                        {
                            TargetAllyCenter.Enabled = TargetAllyRight.Enabled = true;
                            TargetAllyLeft.Enabled = false;
                            TargetAllyCenter.Targets = PBETurnTarget.AllyCenter;
                            TargetAllyRight.Targets = PBETurnTarget.AllyRight;
                        }
                        TargetFoeLeft.Enabled = TargetFoeCenter.Enabled = TargetFoeRight.Enabled = false;
                        TargetAllyLeft.LineRightVisible = TargetAllyCenter.LineRightVisible = TargetFoeRight.LineRightVisible = TargetFoeCenter.LineRightVisible = TargetFoeCenter.LineDownVisible = TargetFoeLeft.LineDownVisible = TargetFoeRight.LineDownVisible = false;
                        break;
                    }
                    case PBEMoveTarget.SingleAllySurrounding:
                    {
                        if (Pokemon.FieldPosition == PBEFieldPosition.Left || Pokemon.FieldPosition == PBEFieldPosition.Right)
                        {
                            TargetAllyCenter.Enabled = true;
                            TargetAllyLeft.Enabled = TargetAllyRight.Enabled = false;
                            TargetAllyCenter.Targets = PBETurnTarget.AllyCenter;
                        }
                        else
                        {
                            TargetAllyCenter.Enabled = false;
                            TargetAllyLeft.Enabled = TargetAllyRight.Enabled = true;
                            TargetAllyLeft.Targets = PBETurnTarget.AllyLeft;
                            TargetAllyRight.Targets = PBETurnTarget.AllyRight;
                        }
                        TargetFoeLeft.Enabled = TargetFoeCenter.Enabled = TargetFoeRight.Enabled = false;
                        TargetAllyLeft.LineRightVisible = TargetAllyCenter.LineRightVisible = TargetFoeRight.LineRightVisible = TargetFoeCenter.LineRightVisible = TargetFoeCenter.LineDownVisible = TargetFoeLeft.LineDownVisible = TargetFoeRight.LineDownVisible = false;
                        break;
                    }
                    case PBEMoveTarget.SingleFoeSurrounding:
                    {
                        if (Pokemon.FieldPosition == PBEFieldPosition.Left)
                        {
                            TargetFoeCenter.Enabled = TargetFoeRight.Enabled = true;
                            TargetFoeLeft.Enabled = false;
                            TargetFoeCenter.Targets = PBETurnTarget.FoeCenter;
                            TargetFoeRight.Targets = PBETurnTarget.FoeRight;
                        }
                        else if (Pokemon.FieldPosition == PBEFieldPosition.Center)
                        {
                            TargetFoeLeft.Enabled = TargetFoeCenter.Enabled = TargetFoeRight.Enabled = true;
                            TargetFoeLeft.Targets = PBETurnTarget.FoeLeft;
                            TargetFoeCenter.Targets = PBETurnTarget.FoeCenter;
                            TargetFoeRight.Targets = PBETurnTarget.FoeRight;
                        }
                        else
                        {
                            TargetFoeLeft.Enabled = TargetFoeCenter.Enabled = true;
                            TargetFoeRight.Enabled = false;
                            TargetFoeLeft.Targets = PBETurnTarget.FoeLeft;
                            TargetFoeCenter.Targets = PBETurnTarget.FoeCenter;
                        }
                        TargetAllyLeft.Enabled = TargetAllyCenter.Enabled = TargetAllyRight.Enabled = false;
                        TargetAllyLeft.LineRightVisible = TargetAllyCenter.LineRightVisible = TargetFoeRight.LineRightVisible = TargetFoeCenter.LineRightVisible = TargetFoeCenter.LineDownVisible = TargetFoeLeft.LineDownVisible = TargetFoeRight.LineDownVisible = false;
                        break;
                    }
                    case PBEMoveTarget.SingleNotSelf:
                    {
                        if (Pokemon.FieldPosition == PBEFieldPosition.Left)
                        {
                            TargetAllyLeft.Enabled = false;
                            TargetAllyCenter.Enabled = TargetAllyRight.Enabled = true;
                            TargetAllyCenter.Targets = PBETurnTarget.AllyCenter;
                            TargetAllyRight.Targets = PBETurnTarget.AllyRight;
                        }
                        else if (Pokemon.FieldPosition == PBEFieldPosition.Center)
                        {
                            TargetAllyCenter.Enabled = false;
                            TargetAllyLeft.Enabled = TargetAllyRight.Enabled = true;
                            TargetAllyLeft.Targets = PBETurnTarget.AllyLeft;
                            TargetAllyRight.Targets = PBETurnTarget.AllyRight;
                        }
                        else
                        {
                            TargetAllyRight.Enabled = false;
                            TargetAllyLeft.Enabled = TargetAllyCenter.Enabled = true;
                            TargetAllyLeft.Targets = PBETurnTarget.AllyLeft;
                            TargetAllyCenter.Targets = PBETurnTarget.AllyCenter;
                        }
                        TargetFoeLeft.Enabled = TargetFoeCenter.Enabled = TargetFoeRight.Enabled = true;
                        TargetAllyLeft.LineRightVisible = TargetAllyCenter.LineRightVisible = TargetFoeRight.LineRightVisible = TargetFoeCenter.LineRightVisible = TargetFoeCenter.LineDownVisible = TargetFoeLeft.LineDownVisible = TargetFoeRight.LineDownVisible = false;
                        TargetFoeLeft.Targets = PBETurnTarget.FoeLeft;
                        TargetFoeCenter.Targets = PBETurnTarget.FoeCenter;
                        TargetFoeRight.Targets = PBETurnTarget.FoeRight;
                        break;
                    }
                    case PBEMoveTarget.SingleSurrounding:
                    {
                        if (Pokemon.FieldPosition == PBEFieldPosition.Left)
                        {
                            TargetAllyCenter.Enabled = TargetFoeCenter.Enabled = TargetFoeRight.Enabled = true;
                            TargetAllyLeft.Enabled = TargetAllyRight.Enabled = TargetFoeLeft.Enabled = false;
                            TargetAllyCenter.Targets = PBETurnTarget.AllyCenter;
                            TargetFoeCenter.Targets = PBETurnTarget.FoeCenter;
                            TargetFoeRight.Targets = PBETurnTarget.FoeRight;
                        }
                        else if (Pokemon.FieldPosition == PBEFieldPosition.Center)
                        {
                            TargetAllyLeft.Enabled = TargetAllyRight.Enabled = TargetFoeLeft.Enabled = TargetFoeCenter.Enabled = TargetFoeRight.Enabled = true;
                            TargetAllyCenter.Enabled = false;
                            TargetAllyLeft.Targets = PBETurnTarget.AllyLeft;
                            TargetAllyRight.Targets = PBETurnTarget.AllyRight;
                            TargetFoeLeft.Targets = PBETurnTarget.FoeLeft;
                            TargetFoeCenter.Targets = PBETurnTarget.FoeCenter;
                            TargetFoeRight.Targets = PBETurnTarget.FoeRight;
                        }
                        else
                        {
                            TargetAllyCenter.Enabled = TargetFoeLeft.Enabled = TargetFoeCenter.Enabled = true;
                            TargetAllyLeft.Enabled = TargetAllyRight.Enabled = TargetFoeRight.Enabled = false;
                            TargetAllyCenter.Targets = PBETurnTarget.AllyCenter;
                            TargetFoeLeft.Targets = PBETurnTarget.FoeLeft;
                            TargetFoeCenter.Targets = PBETurnTarget.FoeCenter;
                        }
                        TargetAllyLeft.LineRightVisible = TargetAllyCenter.LineRightVisible = TargetFoeRight.LineRightVisible = TargetFoeCenter.LineRightVisible = TargetFoeCenter.LineDownVisible = TargetFoeLeft.LineDownVisible = TargetFoeRight.LineDownVisible = false;
                        break;
                    }
                    default: throw new InvalidDataException(nameof(possibleTargets));
                }
            }

            // This would still show the lines if a move had lines
            if (Pokemon.TempLockedTargets != PBETurnTarget.None)
            {
                if (!Pokemon.TempLockedTargets.HasFlag(PBETurnTarget.AllyLeft))
                {
                    TargetAllyLeft.Enabled = false;
                }
                if (!Pokemon.TempLockedTargets.HasFlag(PBETurnTarget.AllyCenter))
                {
                    TargetAllyCenter.Enabled = false;
                }
                if (!Pokemon.TempLockedTargets.HasFlag(PBETurnTarget.AllyRight))
                {
                    TargetAllyRight.Enabled = false;
                }
                if (!Pokemon.TempLockedTargets.HasFlag(PBETurnTarget.FoeLeft))
                {
                    TargetFoeLeft.Enabled = false;
                }
                if (!Pokemon.TempLockedTargets.HasFlag(PBETurnTarget.FoeCenter))
                {
                    TargetFoeCenter.Enabled = false;
                }
                if (!Pokemon.TempLockedTargets.HasFlag(PBETurnTarget.FoeRight))
                {
                    TargetFoeRight.Enabled = false;
                }
            }

            TargetsVisible = true;
        }
        private bool CanUsePosition(PBEFieldPosition pos)
        {
            PBETrainer t = BattleView.Client.Trainer!;
            return !SwitchesBuilder!.IsStandBy(pos) && t.OwnsSpot(pos) && !t.Team.IsSpotOccupied(pos);
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
                    LeftPositionEnabled = CanUsePosition(PBEFieldPosition.Left);
                    RightPositionEnabled = CanUsePosition(PBEFieldPosition.Right);
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
                    LeftPositionEnabled = CanUsePosition(PBEFieldPosition.Left);
                    CenterPositionEnabled = CanUsePosition(PBEFieldPosition.Center);
                    RightPositionEnabled = CanUsePosition(PBEFieldPosition.Right);
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
                case "AllyLeft": targets = TargetAllyLeft.Targets; break;
                case "AllyCenter": targets = TargetAllyCenter.Targets; break;
                case "AllyRight": targets = TargetAllyRight.Targets; break;
                case "FoeLeft": targets = TargetFoeLeft.Targets; break;
                case "FoeCenter": targets = TargetFoeCenter.Targets; break;
                case "FoeRight": targets = TargetFoeRight.Targets; break;
                default: throw new ArgumentOutOfRangeException(nameof(arg));
            }
            TargetsVisible = false;
            ActionsBuilder!.EnqueueMove(_fightMove, targets);
        }
        public void SelectPosition(string arg)
        {
            PBEFieldPosition pos = Enum.Parse<PBEFieldPosition>(arg);
            PositionsVisible = false;
            SwitchesBuilder!.Enqueue(Pokemon, pos);
        }
    }
}
