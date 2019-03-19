using Kermalis.PokemonBattleEngine.Battle;
using Kermalis.PokemonBattleEngine.Data;
using Kermalis.PokemonBattleEngineMobile.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;

namespace Kermalis.PokemonBattleEngineMobile.Views
{
    // If you dislike spaghetti code please close this file unless I told you otherwise
    public partial class ActionsView : ContentView
    {
        PBEPokemon targetAllyLeft;
        public PBEPokemon TargetAllyLeft
        {
            get => targetAllyLeft;
            set
            {
                targetAllyLeft = value;
                OnPropertyChanged(nameof(TargetAllyLeft));
            }
        }
        bool targetAllyLeftEnabled;
        public bool TargetAllyLeftEnabled
        {
            get => targetAllyLeftEnabled;
            set
            {
                targetAllyLeftEnabled = value;
                OnPropertyChanged(nameof(TargetAllyLeftEnabled));
            }
        }
        PBEPokemon targetAllyCenter;
        public PBEPokemon TargetAllyCenter
        {
            get => targetAllyCenter;
            set
            {
                targetAllyCenter = value;
                OnPropertyChanged(nameof(TargetAllyCenter));
            }
        }
        bool targetAllyCenterEnabled;
        public bool TargetAllyCenterEnabled
        {
            get => targetAllyCenterEnabled;
            set
            {
                targetAllyCenterEnabled = value;
                OnPropertyChanged(nameof(TargetAllyCenterEnabled));
            }
        }
        PBEPokemon targetAllyRight;
        public PBEPokemon TargetAllyRight
        {
            get => targetAllyRight;
            set
            {
                targetAllyRight = value;
                OnPropertyChanged(nameof(TargetAllyRight));
            }
        }
        bool targetAllyRightEnabled;
        public bool TargetAllyRightEnabled
        {
            get => targetAllyRightEnabled;
            set
            {
                targetAllyRightEnabled = value;
                OnPropertyChanged(nameof(TargetAllyRightEnabled));
            }
        }
        PBEPokemon targetFoeLeft;
        public PBEPokemon TargetFoeLeft
        {
            get => targetFoeLeft;
            set
            {
                targetFoeLeft = value;
                OnPropertyChanged(nameof(TargetFoeLeft));
            }
        }
        bool targetFoeLeftEnabled;
        public bool TargetFoeLeftEnabled
        {
            get => targetFoeLeftEnabled;
            set
            {
                targetFoeLeftEnabled = value;
                OnPropertyChanged(nameof(TargetFoeLeftEnabled));
            }
        }
        PBEPokemon targetFoeCenter;
        public PBEPokemon TargetFoeCenter
        {
            get => targetFoeCenter;
            set
            {
                targetFoeCenter = value;
                OnPropertyChanged(nameof(TargetFoeCenter));
            }
        }
        bool targetFoeCenterEnabled;
        public bool TargetFoeCenterEnabled
        {
            get => targetFoeCenterEnabled;
            set
            {
                targetFoeCenterEnabled = value;
                OnPropertyChanged(nameof(TargetFoeCenterEnabled));
            }
        }
        PBEPokemon targetFoeRight;
        public PBEPokemon TargetFoeRight
        {
            get => targetFoeRight;
            set
            {
                targetFoeRight = value;
                OnPropertyChanged(nameof(TargetFoeRight));
            }
        }
        bool targetFoeRightEnabled;
        public bool TargetFoeRightEnabled
        {
            get => targetFoeRightEnabled;
            set
            {
                targetFoeRightEnabled = value;
                OnPropertyChanged(nameof(TargetFoeRightEnabled));
            }
        }

        bool centerTargetsVisible;
        public bool CenterTargetsVisible
        {
            get => centerTargetsVisible;
            set
            {
                centerTargetsVisible = value;
                OnPropertyChanged(nameof(CenterTargetsVisible));
            }
        }
        Point targetLineFoeRightAllyLeftLocation;
        public Point TargetLineFoeRightAllyLeftLocation
        {
            get => targetLineFoeRightAllyLeftLocation;
            set
            {
                targetLineFoeRightAllyLeftLocation = value;
                OnPropertyChanged(nameof(TargetLineFoeRightAllyLeftLocation));
            }
        }
        Point targetLineFoeRightFoeCenterLocation;
        public Point TargetLineFoeRightFoeCenterLocation
        {
            get => targetLineFoeRightFoeCenterLocation;
            set
            {
                targetLineFoeRightFoeCenterLocation = value;
                OnPropertyChanged(nameof(TargetLineFoeRightFoeCenterLocation));
            }
        }
        Point targetLineAllyLeftAllyCenterLocation;
        public Point TargetLineAllyLeftAllyCenterLocation
        {
            get => targetLineAllyLeftAllyCenterLocation;
            set
            {
                targetLineAllyLeftAllyCenterLocation = value;
                OnPropertyChanged(nameof(TargetLineAllyLeftAllyCenterLocation));
            }
        }
        Point targetLineFoeLeftAllyRightLocation;
        public Point TargetLineFoeLeftAllyRightLocation
        {
            get => targetLineFoeLeftAllyRightLocation;
            set
            {
                targetLineFoeLeftAllyRightLocation = value;
                OnPropertyChanged(nameof(TargetLineFoeLeftAllyRightLocation));
            }
        }
        Point targetFoeRightLocation;
        public Point TargetFoeRightLocation
        {
            get => targetFoeRightLocation;
            set
            {
                targetFoeRightLocation = value;
                OnPropertyChanged(nameof(TargetFoeRightLocation));
            }
        }
        Point targetFoeLeftLocation;
        public Point TargetFoeLeftLocation
        {
            get => targetFoeLeftLocation;
            set
            {
                targetFoeLeftLocation = value;
                OnPropertyChanged(nameof(TargetFoeLeftLocation));
            }
        }
        Point targetAllyLeftLocation;
        public Point TargetAllyLeftLocation
        {
            get => targetAllyLeftLocation;
            set
            {
                targetAllyLeftLocation = value;
                OnPropertyChanged(nameof(TargetAllyLeftLocation));
            }
        }
        Point targetAllyRightLocation;
        public Point TargetAllyRightLocation
        {
            get => targetAllyRightLocation;
            set
            {
                targetAllyRightLocation = value;
                OnPropertyChanged(nameof(TargetAllyRightLocation));
            }
        }

        bool targetLineFoeLeftFoeCenterEnabled;
        public bool TargetLineFoeLeftFoeCenterEnabled
        {
            get => targetLineFoeLeftFoeCenterEnabled;
            set
            {
                targetLineFoeLeftFoeCenterEnabled = value;
                OnPropertyChanged(nameof(TargetLineFoeLeftFoeCenterEnabled));
            }
        }
        bool targetLineFoeLeftAllyRightEnabled;
        public bool TargetLineFoeLeftAllyRightEnabled
        {
            get => targetLineFoeLeftAllyRightEnabled;
            set
            {
                targetLineFoeLeftAllyRightEnabled = value;
                OnPropertyChanged(nameof(TargetLineFoeLeftAllyRightEnabled));
            }
        }
        bool targetLineFoeCenterAllyCenterEnabled;
        public bool TargetLineFoeCenterAllyCenterEnabled
        {
            get => targetLineFoeCenterAllyCenterEnabled;
            set
            {
                targetLineFoeCenterAllyCenterEnabled = value;
                OnPropertyChanged(nameof(TargetLineFoeCenterAllyCenterEnabled));
            }
        }
        bool targetLineFoeRightFoeCenterEnabled;
        public bool TargetLineFoeRightFoeCenterEnabled
        {
            get => targetLineFoeRightFoeCenterEnabled;
            set
            {
                targetLineFoeRightFoeCenterEnabled = value;
                OnPropertyChanged(nameof(TargetLineFoeRightFoeCenterEnabled));
            }
        }
        bool targetLineFoeRightAllyLeftEnabled;
        public bool TargetLineFoeRightAllyLeftEnabled
        {
            get => targetLineFoeRightAllyLeftEnabled;
            set
            {
                targetLineFoeRightAllyLeftEnabled = value;
                OnPropertyChanged(nameof(TargetLineFoeRightAllyLeftEnabled));
            }
        }
        bool targetLineAllyLeftAllyCenterEnabled;
        public bool TargetLineAllyLeftAllyCenterEnabled
        {
            get => targetLineAllyLeftAllyCenterEnabled;
            set
            {
                targetLineAllyLeftAllyCenterEnabled = value;
                OnPropertyChanged(nameof(TargetLineAllyLeftAllyCenterEnabled));
            }
        }
        bool targetLineAllyRightAllyCenterEnabled;
        public bool TargetLineAllyRightAllyCenterEnabled
        {
            get => targetLineAllyRightAllyCenterEnabled;
            set
            {
                targetLineAllyRightAllyCenterEnabled = value;
                OnPropertyChanged(nameof(TargetLineAllyRightAllyCenterEnabled));
            }
        }

        bool leftPositionEnabled;
        public bool LeftPositionEnabled
        {
            get => leftPositionEnabled;
            set
            {
                leftPositionEnabled = value;
                OnPropertyChanged(nameof(LeftPositionEnabled));
            }
        }
        bool centerPositionEnabled;
        public bool CenterPositionEnabled
        {
            get => centerPositionEnabled;
            set
            {
                centerPositionEnabled = value;
                OnPropertyChanged(nameof(CenterPositionEnabled));
            }
        }
        bool rightPositionEnabled;
        public bool RightPositionEnabled
        {
            get => rightPositionEnabled;
            set
            {
                rightPositionEnabled = value;
                OnPropertyChanged(nameof(RightPositionEnabled));
            }
        }
        Point leftPositionLocation;
        public Point LeftPositionLocation
        {
            get => leftPositionLocation;
            set
            {
                leftPositionLocation = value;
                OnPropertyChanged(nameof(LeftPositionLocation));
            }
        }
        Point rightPositionLocation;
        public Point RightPositionLocation
        {
            get => rightPositionLocation;
            set
            {
                rightPositionLocation = value;
                OnPropertyChanged(nameof(RightPositionLocation));
            }
        }

        PBETarget targetAllyLeftResult, targetAllyCenterResult, targetAllyRightResult,
            targetFoeLeftResult, targetFoeCenterResult, targetFoeRightResult;

        public Command SelectTargetCommand { get; }
        public Command SelectPositionCommand { get; }

        IList<MoveInfo> moves;
        public IList<MoveInfo> Moves
        {
            get => moves;
            set
            {
                moves = value;
                OnPropertyChanged(nameof(Moves));
            }
        }
        IList<PokemonInfo> party;
        public IList<PokemonInfo> Party
        {
            get => party;
            set
            {
                party = value;
                OnPropertyChanged(nameof(Party));
            }
        }

        bool targetsVisible;
        public bool TargetsVisible
        {
            get => targetsVisible;
            set
            {
                targetsVisible = value;
                OnPropertyChanged(nameof(TargetsVisible));
            }
        }
        bool movesVisible;
        public bool MovesVisible
        {
            get => movesVisible;
            set
            {
                movesVisible = value;
                OnPropertyChanged(nameof(MovesVisible));
            }
        }
        bool switchesVisible;
        public bool SwitchesVisible
        {
            get => switchesVisible;
            set
            {
                switchesVisible = value;
                OnPropertyChanged(nameof(SwitchesVisible));
            }
        }
        bool positionsVisible;
        public bool PositionsVisible
        {
            get => positionsVisible;
            set
            {
                positionsVisible = value;
                OnPropertyChanged(nameof(PositionsVisible));
            }
        }

        public BattleView BattleView { get; set; }
        public PBEPokemon Pokemon { get; private set; }

        public ActionsView()
        {
            InitializeComponent();
            BindingContext = this;
            SelectTargetCommand = new Command<string>(SelectTarget);
            SelectPositionCommand = new Command<string>(SelectPosition);

            MoveInfo.CreateBrushes();
        }

        public void DisplayActions(PBEPokemon pkmn)
        {
            Pokemon = pkmn;
            Party = pkmn.Team.Party.Select(p => new PokemonInfo(p, !pkmn.CanSwitchOut() || BattleView.Client.StandBy.Contains(p), SelectPokemonForTurn)).ToArray();
            Moves = pkmn.GetUsableMoves().Select(m => new MoveInfo(pkmn, m, SelectMoveForTurn)).ToArray();
            MovesVisible = true;
        }
        public void DisplaySwitches()
        {
            Party = BattleView.Client.Battle.Teams[BattleView.Client.BattleId].Party.Select(p => new PokemonInfo(p, BattleView.Client.StandBy.Contains(p), SelectSwitch)).ToArray();
            SwitchesVisible = true;
        }

        void SelectPokemonForTurn(PBEPokemon pkmn)
        {
            Pokemon.SelectedAction.Decision = PBEDecision.SwitchOut;
            Pokemon.SelectedAction.SwitchPokemonId = pkmn.Id;
            BattleView.Client.StandBy.Add(pkmn);
            MovesVisible = false;
            BattleView.Client.ActionsLoop(false);
        }
        void SelectMoveForTurn(PBEMove move)
        {
            Pokemon.SelectedAction.Decision = PBEDecision.Fight;
            Pokemon.SelectedAction.FightMove = move;
            MovesVisible = false;
            DisplayTargets(move);
        }
        void SelectSwitch(PBEPokemon pkmn)
        {
            Pokemon = pkmn;
            SwitchesVisible = false;
            DisplayPositions();
        }
        void DisplayTargets(PBEMove move)
        {
            PBEMoveTarget possibleTargets = Pokemon.GetMoveTargets(move);

            if (BattleView.Client.Battle.BattleFormat == PBEBattleFormat.Single || BattleView.Client.Battle.BattleFormat == PBEBattleFormat.Rotation)
            {
                switch (possibleTargets)
                {
                    case PBEMoveTarget.All:
                        Pokemon.SelectedAction.FightTargets = PBETarget.AllyCenter | PBETarget.FoeCenter;
                        break;
                    case PBEMoveTarget.AllFoes:
                    case PBEMoveTarget.AllFoesSurrounding:
                    case PBEMoveTarget.AllSurrounding:
                    case PBEMoveTarget.RandomFoeSurrounding:
                    case PBEMoveTarget.SingleFoeSurrounding:
                    case PBEMoveTarget.SingleNotSelf:
                    case PBEMoveTarget.SingleSurrounding:
                        Pokemon.SelectedAction.FightTargets = PBETarget.FoeCenter;
                        break;
                    case PBEMoveTarget.AllTeam:
                    case PBEMoveTarget.Self:
                    case PBEMoveTarget.SelfOrAllySurrounding:
                    case PBEMoveTarget.SingleAllySurrounding:
                        Pokemon.SelectedAction.FightTargets = PBETarget.AllyCenter;
                        break;
                }
                BattleView.Client.ActionsLoop(false);
            }
            else // Double / Triple
            {
                TargetAllyLeft = BattleView.Client.Battle.Teams[BattleView.Client.BattleId].TryGetPokemon(PBEFieldPosition.Left);
                TargetAllyCenter = BattleView.Client.Battle.Teams[BattleView.Client.BattleId].TryGetPokemon(PBEFieldPosition.Center);
                TargetAllyRight = BattleView.Client.Battle.Teams[BattleView.Client.BattleId].TryGetPokemon(PBEFieldPosition.Right);
                TargetFoeLeft = BattleView.Client.Battle.Teams[BattleView.Client.BattleId == 0 ? 1 : 0].TryGetPokemon(PBEFieldPosition.Left);
                TargetFoeCenter = BattleView.Client.Battle.Teams[BattleView.Client.BattleId == 0 ? 1 : 0].TryGetPokemon(PBEFieldPosition.Center);
                TargetFoeRight = BattleView.Client.Battle.Teams[BattleView.Client.BattleId == 0 ? 1 : 0].TryGetPokemon(PBEFieldPosition.Right);

                void SetLocations(double leftX, double rightX, double leftLineX, double centerLineX, double rightLineX)
                {
                    TargetLineFoeRightAllyLeftLocation = new Point(leftLineX, 51);
                    TargetLineFoeRightFoeCenterLocation = new Point(centerLineX, 32);
                    TargetLineAllyLeftAllyCenterLocation = new Point(centerLineX, 84);
                    TargetLineFoeLeftAllyRightLocation = new Point(rightLineX, 51);
                    TargetFoeRightLocation = new Point(leftX, 23);
                    TargetFoeLeftLocation = new Point(rightX, 23);
                    TargetAllyLeftLocation = new Point(leftX, 75);
                    TargetAllyRightLocation = new Point(rightX, 75);
                }

                if (BattleView.Client.Battle.BattleFormat == PBEBattleFormat.Double)
                {
                    const double baseX = 142;
                    SetLocations(baseX + 0, baseX + 128, baseX + 44, baseX + 98, baseX + 172);
                    CenterTargetsVisible = false;
                    TargetLineFoeCenterAllyCenterEnabled = TargetLineFoeLeftFoeCenterEnabled = TargetLineAllyRightAllyCenterEnabled = false;
                    switch (possibleTargets)
                    {
                        case PBEMoveTarget.All:
                            {
                                TargetAllyLeftEnabled = TargetAllyRightEnabled = TargetFoeLeftEnabled = TargetFoeRightEnabled = true;
                                TargetLineAllyLeftAllyCenterEnabled = TargetLineFoeRightFoeCenterEnabled = TargetLineFoeLeftAllyRightEnabled = TargetLineFoeRightAllyLeftEnabled = true;
                                targetAllyLeftResult = targetAllyRightResult = targetFoeLeftResult = targetFoeRightResult = PBETarget.AllyLeft | PBETarget.AllyRight | PBETarget.FoeLeft | PBETarget.FoeRight;
                                break;
                            }
                        case PBEMoveTarget.AllFoes:
                        case PBEMoveTarget.AllFoesSurrounding:
                            {
                                TargetAllyLeftEnabled = TargetAllyRightEnabled = false;
                                TargetFoeLeftEnabled = TargetFoeRightEnabled = true;
                                TargetLineAllyLeftAllyCenterEnabled = TargetLineFoeLeftAllyRightEnabled = TargetLineFoeRightAllyLeftEnabled = false;
                                TargetLineFoeRightFoeCenterEnabled = true;
                                targetFoeLeftResult = targetFoeRightResult = PBETarget.FoeLeft | PBETarget.FoeRight;
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
                                    targetAllyRightResult = targetFoeLeftResult = targetFoeRightResult = PBETarget.AllyRight | PBETarget.FoeLeft | PBETarget.FoeRight;
                                }
                                else
                                {
                                    TargetAllyLeftEnabled = true;
                                    TargetAllyRightEnabled = false;
                                    TargetLineFoeRightAllyLeftEnabled = true;
                                    TargetLineFoeLeftAllyRightEnabled = false;
                                    targetAllyLeftResult = targetFoeLeftResult = targetFoeRightResult = PBETarget.AllyLeft | PBETarget.FoeLeft | PBETarget.FoeRight;
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
                                targetAllyLeftResult = targetAllyRightResult = PBETarget.AllyLeft | PBETarget.AllyRight;
                                break;
                            }
                        case PBEMoveTarget.RandomFoeSurrounding:
                        case PBEMoveTarget.Self:
                            {
                                if (Pokemon.FieldPosition == PBEFieldPosition.Left)
                                {
                                    TargetAllyLeftEnabled = true;
                                    TargetAllyRightEnabled = false;
                                    targetAllyLeftResult = PBETarget.AllyLeft;
                                }
                                else
                                {
                                    TargetAllyLeftEnabled = false;
                                    TargetAllyRightEnabled = true;
                                    targetAllyRightResult = PBETarget.AllyRight;
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
                                targetAllyLeftResult = PBETarget.AllyLeft;
                                targetAllyRightResult = PBETarget.AllyRight;
                                break;
                            }
                        case PBEMoveTarget.SingleAllySurrounding:
                            {
                                if (Pokemon.FieldPosition == PBEFieldPosition.Left)
                                {
                                    TargetAllyLeftEnabled = false;
                                    TargetAllyRightEnabled = true;
                                    targetAllyRightResult = PBETarget.AllyRight;
                                }
                                else
                                {
                                    TargetAllyLeftEnabled = true;
                                    TargetAllyRightEnabled = false;
                                    targetAllyLeftResult = PBETarget.AllyLeft;
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
                                targetFoeLeftResult = PBETarget.FoeLeft;
                                targetFoeRightResult = PBETarget.FoeRight;
                                break;
                            }
                        case PBEMoveTarget.SingleNotSelf:
                        case PBEMoveTarget.SingleSurrounding:
                            {
                                if (Pokemon.FieldPosition == PBEFieldPosition.Left)
                                {
                                    TargetAllyLeftEnabled = false;
                                    TargetAllyRightEnabled = true;
                                    targetAllyRightResult = PBETarget.AllyRight;
                                }
                                else
                                {
                                    TargetAllyLeftEnabled = true;
                                    TargetAllyRightEnabled = false;
                                    targetAllyLeftResult = PBETarget.AllyLeft;
                                }
                                TargetFoeLeftEnabled = TargetFoeRightEnabled = true;
                                TargetLineAllyLeftAllyCenterEnabled = TargetLineFoeRightFoeCenterEnabled = TargetLineFoeLeftAllyRightEnabled = TargetLineFoeRightAllyLeftEnabled = false;
                                targetFoeLeftResult = PBETarget.FoeLeft;
                                targetFoeRightResult = PBETarget.FoeRight;
                                break;
                            }
                    }
                }
                else // Triple
                {
                    const double baseX = 78;
                    SetLocations(baseX + 0, baseX + 256, baseX + 44, baseX + 98, baseX + 300);
                    CenterTargetsVisible = true;
                    switch (possibleTargets)
                    {
                        case PBEMoveTarget.All:
                            {
                                TargetAllyLeftEnabled = TargetAllyCenterEnabled = TargetAllyRightEnabled = TargetFoeLeftEnabled = TargetFoeCenterEnabled = TargetFoeRightEnabled = true;
                                TargetLineAllyLeftAllyCenterEnabled = TargetLineAllyRightAllyCenterEnabled = TargetLineFoeRightFoeCenterEnabled = TargetLineFoeLeftFoeCenterEnabled = TargetLineFoeCenterAllyCenterEnabled = TargetLineFoeLeftAllyRightEnabled = TargetLineFoeRightAllyLeftEnabled = true;
                                targetAllyLeftResult = targetAllyCenterResult = targetAllyRightResult = targetFoeLeftResult = targetFoeCenterResult = targetFoeRightResult = PBETarget.AllyLeft | PBETarget.AllyCenter | PBETarget.AllyRight | PBETarget.FoeLeft | PBETarget.FoeCenter | PBETarget.FoeRight;
                                break;
                            }
                        case PBEMoveTarget.AllFoes:
                            {
                                TargetAllyLeftEnabled = TargetAllyCenterEnabled = TargetAllyRightEnabled = false;
                                TargetFoeLeftEnabled = TargetFoeCenterEnabled = TargetFoeRightEnabled = true;
                                TargetLineAllyLeftAllyCenterEnabled = TargetLineAllyRightAllyCenterEnabled = TargetLineFoeCenterAllyCenterEnabled = TargetLineFoeLeftAllyRightEnabled = TargetLineFoeRightAllyLeftEnabled = false;
                                TargetLineFoeRightFoeCenterEnabled = TargetLineFoeLeftFoeCenterEnabled = true;
                                targetFoeLeftResult = targetFoeCenterResult = targetFoeRightResult = PBETarget.FoeLeft | PBETarget.FoeCenter | PBETarget.FoeRight;
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
                                    targetFoeCenterResult = targetFoeRightResult = PBETarget.FoeCenter | PBETarget.FoeRight;
                                }
                                else if (Pokemon.FieldPosition == PBEFieldPosition.Center)
                                {
                                    TargetFoeLeftEnabled = TargetFoeCenterEnabled = TargetFoeRightEnabled = true;
                                    TargetLineFoeRightFoeCenterEnabled = TargetLineFoeLeftFoeCenterEnabled = true;
                                    targetFoeLeftResult = targetFoeCenterResult = targetFoeRightResult = PBETarget.FoeLeft | PBETarget.FoeCenter | PBETarget.FoeRight;
                                }
                                else
                                {
                                    TargetFoeLeftEnabled = TargetFoeCenterEnabled = true;
                                    TargetFoeRightEnabled = false;
                                    TargetLineFoeLeftFoeCenterEnabled = true;
                                    TargetLineFoeRightFoeCenterEnabled = false;
                                    targetFoeLeftResult = targetFoeCenterResult = PBETarget.FoeLeft | PBETarget.FoeCenter;
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
                                    targetAllyCenterResult = targetFoeCenterResult = targetFoeRightResult = PBETarget.AllyCenter | PBETarget.FoeCenter | PBETarget.FoeRight;
                                }
                                else if (Pokemon.FieldPosition == PBEFieldPosition.Center)
                                {
                                    TargetAllyLeftEnabled = TargetAllyRightEnabled = TargetFoeLeftEnabled = TargetFoeCenterEnabled = TargetFoeRightEnabled = true;
                                    TargetAllyCenterEnabled = false;
                                    TargetLineAllyLeftAllyCenterEnabled = TargetLineAllyRightAllyCenterEnabled = TargetLineFoeCenterAllyCenterEnabled = false;
                                    TargetLineFoeRightFoeCenterEnabled = TargetLineFoeLeftFoeCenterEnabled = TargetLineFoeLeftAllyRightEnabled = TargetLineFoeRightAllyLeftEnabled = true;
                                    targetAllyLeftResult = targetAllyRightResult = targetFoeLeftResult = targetFoeCenterResult = targetFoeRightResult = PBETarget.AllyLeft | PBETarget.AllyRight | PBETarget.FoeLeft | PBETarget.FoeCenter | PBETarget.FoeRight;
                                }
                                else
                                {
                                    TargetAllyLeftEnabled = TargetAllyRightEnabled = TargetFoeRightEnabled = false;
                                    TargetAllyCenterEnabled = TargetFoeLeftEnabled = TargetFoeCenterEnabled = true;
                                    TargetLineAllyLeftAllyCenterEnabled = TargetLineAllyRightAllyCenterEnabled = TargetLineFoeRightFoeCenterEnabled = TargetLineFoeLeftAllyRightEnabled = TargetLineFoeRightAllyLeftEnabled = false;
                                    TargetLineFoeLeftFoeCenterEnabled = TargetLineFoeCenterAllyCenterEnabled = true;
                                    targetAllyCenterResult = targetFoeLeftResult = targetFoeCenterResult = PBETarget.AllyCenter | PBETarget.FoeLeft | PBETarget.FoeCenter;
                                }
                                break;
                            }
                        case PBEMoveTarget.AllTeam:
                            {
                                TargetAllyLeftEnabled = TargetAllyCenterEnabled = TargetAllyRightEnabled = true;
                                TargetFoeLeftEnabled = TargetFoeCenterEnabled = TargetFoeRightEnabled = false;
                                TargetLineAllyLeftAllyCenterEnabled = TargetLineAllyRightAllyCenterEnabled = true;
                                TargetLineFoeRightFoeCenterEnabled = TargetLineFoeLeftFoeCenterEnabled = TargetLineFoeCenterAllyCenterEnabled = TargetLineFoeLeftAllyRightEnabled = TargetLineFoeRightAllyLeftEnabled = false;
                                targetAllyLeftResult = targetAllyCenterResult = targetAllyRightResult = PBETarget.AllyLeft | PBETarget.AllyCenter | PBETarget.AllyRight;
                                break;
                            }
                        case PBEMoveTarget.RandomFoeSurrounding:
                        case PBEMoveTarget.Self:
                            {
                                if (Pokemon.FieldPosition == PBEFieldPosition.Left)
                                {
                                    TargetAllyLeftEnabled = true;
                                    TargetAllyCenterEnabled = TargetAllyRightEnabled = false;
                                    targetAllyLeftResult = PBETarget.AllyLeft;
                                }
                                else if (Pokemon.FieldPosition == PBEFieldPosition.Center)
                                {
                                    TargetAllyCenterEnabled = true;
                                    TargetAllyLeftEnabled = TargetAllyRightEnabled = false;
                                    targetAllyCenterResult = PBETarget.AllyCenter;
                                }
                                else
                                {
                                    TargetAllyRightEnabled = true;
                                    TargetAllyLeftEnabled = TargetAllyCenterEnabled = false;
                                    targetAllyRightResult = PBETarget.AllyRight;
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
                                    targetAllyLeftResult = PBETarget.AllyLeft;
                                    targetAllyCenterResult = PBETarget.AllyCenter;
                                }
                                else if (Pokemon.FieldPosition == PBEFieldPosition.Center)
                                {
                                    TargetAllyCenterEnabled = TargetAllyLeftEnabled = TargetAllyRightEnabled = true;
                                    targetAllyLeftResult = PBETarget.AllyLeft;
                                    targetAllyCenterResult = PBETarget.AllyCenter;
                                    targetAllyRightResult = PBETarget.AllyRight;
                                }
                                else
                                {
                                    TargetAllyCenterEnabled = TargetAllyRightEnabled = true;
                                    TargetAllyLeftEnabled = false;
                                    targetAllyCenterResult = PBETarget.AllyCenter;
                                    targetAllyRightResult = PBETarget.AllyRight;
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
                                    targetAllyCenterResult = PBETarget.AllyCenter;
                                }
                                else
                                {
                                    TargetAllyCenterEnabled = false;
                                    TargetAllyLeftEnabled = TargetAllyRightEnabled = true;
                                    targetAllyLeftResult = PBETarget.AllyLeft;
                                    targetAllyRightResult = PBETarget.AllyRight;
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
                                    targetFoeCenterResult = PBETarget.FoeCenter;
                                    targetFoeRightResult = PBETarget.FoeRight;
                                }
                                else if (Pokemon.FieldPosition == PBEFieldPosition.Center)
                                {
                                    TargetFoeLeftEnabled = TargetFoeCenterEnabled = TargetFoeRightEnabled = true;
                                    targetFoeLeftResult = PBETarget.FoeLeft;
                                    targetFoeCenterResult = PBETarget.FoeCenter;
                                    targetFoeRightResult = PBETarget.FoeRight;
                                }
                                else
                                {
                                    TargetFoeLeftEnabled = TargetFoeCenterEnabled = true;
                                    TargetFoeRightEnabled = false;
                                    targetFoeLeftResult = PBETarget.FoeLeft;
                                    targetFoeCenterResult = PBETarget.FoeCenter;
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
                                    targetAllyCenterResult = PBETarget.AllyCenter;
                                    targetAllyRightResult = PBETarget.AllyRight;
                                }
                                else if (Pokemon.FieldPosition == PBEFieldPosition.Center)
                                {
                                    TargetAllyCenterEnabled = false;
                                    TargetAllyLeftEnabled = TargetAllyRightEnabled = true;
                                    targetAllyLeftResult = PBETarget.AllyLeft;
                                    targetAllyRightResult = PBETarget.AllyRight;
                                }
                                else
                                {
                                    TargetAllyRightEnabled = false;
                                    TargetAllyLeftEnabled = TargetAllyCenterEnabled = true;
                                    targetAllyLeftResult = PBETarget.AllyLeft;
                                    targetAllyCenterResult = PBETarget.AllyCenter;
                                }
                                TargetFoeLeftEnabled = TargetFoeCenterEnabled = TargetFoeRightEnabled = true;
                                TargetLineAllyLeftAllyCenterEnabled = TargetLineAllyRightAllyCenterEnabled = TargetLineFoeRightFoeCenterEnabled = TargetLineFoeLeftFoeCenterEnabled = TargetLineFoeCenterAllyCenterEnabled = TargetLineFoeLeftAllyRightEnabled = TargetLineFoeRightAllyLeftEnabled = false;
                                targetFoeLeftResult = PBETarget.FoeLeft;
                                targetFoeCenterResult = PBETarget.FoeCenter;
                                targetFoeRightResult = PBETarget.FoeRight;
                                break;
                            }
                        case PBEMoveTarget.SingleSurrounding:
                            {
                                if (Pokemon.FieldPosition == PBEFieldPosition.Left)
                                {
                                    TargetAllyCenterEnabled = TargetFoeCenterEnabled = TargetFoeRightEnabled = true;
                                    TargetAllyLeftEnabled = TargetAllyRightEnabled = TargetFoeLeftEnabled = false;
                                    targetAllyCenterResult = PBETarget.AllyCenter;
                                    targetFoeCenterResult = PBETarget.FoeCenter;
                                    targetFoeRightResult = PBETarget.FoeRight;
                                }
                                else if (Pokemon.FieldPosition == PBEFieldPosition.Center)
                                {
                                    TargetAllyLeftEnabled = TargetAllyRightEnabled = TargetFoeLeftEnabled = TargetFoeCenterEnabled = TargetFoeRightEnabled = true;
                                    TargetAllyCenterEnabled = false;
                                    targetAllyLeftResult = PBETarget.AllyLeft;
                                    targetAllyRightResult = PBETarget.AllyRight;
                                    targetFoeLeftResult = PBETarget.FoeLeft;
                                    targetFoeCenterResult = PBETarget.FoeCenter;
                                    targetFoeRightResult = PBETarget.FoeRight;
                                }
                                else
                                {
                                    TargetAllyCenterEnabled = TargetFoeLeftEnabled = TargetFoeCenterEnabled = true;
                                    TargetAllyLeftEnabled = TargetAllyRightEnabled = TargetFoeRightEnabled = false;
                                    targetAllyCenterResult = PBETarget.AllyCenter;
                                    targetFoeLeftResult = PBETarget.FoeLeft;
                                    targetFoeCenterResult = PBETarget.FoeCenter;
                                }
                                TargetLineAllyLeftAllyCenterEnabled = TargetLineAllyRightAllyCenterEnabled = TargetLineFoeRightFoeCenterEnabled = TargetLineFoeLeftFoeCenterEnabled = TargetLineFoeCenterAllyCenterEnabled = TargetLineFoeLeftAllyRightEnabled = TargetLineFoeRightAllyLeftEnabled = false;
                                break;
                            }
                    }
                }

                if (Pokemon.TempLockedTargets != PBETarget.None)
                {
                    if (!Pokemon.TempLockedTargets.HasFlag(PBETarget.AllyLeft))
                    {
                        TargetAllyLeftEnabled = false;
                    }
                    if (!Pokemon.TempLockedTargets.HasFlag(PBETarget.AllyCenter))
                    {
                        TargetAllyCenterEnabled = false;
                    }
                    if (!Pokemon.TempLockedTargets.HasFlag(PBETarget.AllyRight))
                    {
                        TargetAllyRightEnabled = false;
                    }
                    if (!Pokemon.TempLockedTargets.HasFlag(PBETarget.FoeLeft))
                    {
                        TargetFoeLeftEnabled = false;
                    }
                    if (!Pokemon.TempLockedTargets.HasFlag(PBETarget.FoeCenter))
                    {
                        TargetFoeCenterEnabled = false;
                    }
                    if (!Pokemon.TempLockedTargets.HasFlag(PBETarget.FoeRight))
                    {
                        TargetFoeRightEnabled = false;
                    }
                }

                TargetsVisible = true;
            }
        }
        void DisplayPositions()
        {
            LeftPositionEnabled = CenterPositionEnabled = RightPositionEnabled = false;
            void SetLocations(double leftX, double rightX)
            {
                LeftPositionLocation = new Point(leftX, 49);
                RightPositionLocation = new Point(rightX, 49);
            }
            switch (BattleView.Client.Battle.BattleFormat)
            {
                case PBEBattleFormat.Single:
                    {
                        SelectPosition("Center");
                        break;
                    }
                case PBEBattleFormat.Double:
                    {
                        LeftPositionEnabled = !BattleView.Client.PositionStandBy.Contains(PBEFieldPosition.Left) && BattleView.Client.Battle.Teams[BattleView.Client.BattleId].TryGetPokemon(PBEFieldPosition.Left) == null;
                        RightPositionEnabled = !BattleView.Client.PositionStandBy.Contains(PBEFieldPosition.Right) && BattleView.Client.Battle.Teams[BattleView.Client.BattleId].TryGetPokemon(PBEFieldPosition.Right) == null;
                        if (leftPositionEnabled && !rightPositionEnabled)
                        {
                            SelectPosition("Left");
                        }
                        else if (!leftPositionEnabled && rightPositionEnabled)
                        {
                            SelectPosition("Right");
                        }
                        else
                        {
                            const double baseX = 142;
                            SetLocations(baseX + 0, baseX + 128);
                            BattleView.AddMessage($"Send {Pokemon.Nickname} where?", true, false);
                            PositionsVisible = true;
                        }
                        break;
                    }
                case PBEBattleFormat.Triple:
                case PBEBattleFormat.Rotation:
                    {
                        LeftPositionEnabled = !BattleView.Client.PositionStandBy.Contains(PBEFieldPosition.Left) && BattleView.Client.Battle.Teams[BattleView.Client.BattleId].TryGetPokemon(PBEFieldPosition.Left) == null;
                        CenterPositionEnabled = !BattleView.Client.PositionStandBy.Contains(PBEFieldPosition.Center) && BattleView.Client.Battle.Teams[BattleView.Client.BattleId].TryGetPokemon(PBEFieldPosition.Center) == null;
                        RightPositionEnabled = !BattleView.Client.PositionStandBy.Contains(PBEFieldPosition.Right) && BattleView.Client.Battle.Teams[BattleView.Client.BattleId].TryGetPokemon(PBEFieldPosition.Right) == null;
                        if (leftPositionEnabled && !centerPositionEnabled && !rightPositionEnabled)
                        {
                            SelectPosition("Left");
                        }
                        else if (!leftPositionEnabled && centerPositionEnabled && !rightPositionEnabled)
                        {
                            SelectPosition("Center");
                        }
                        else if (!leftPositionEnabled && !centerPositionEnabled && rightPositionEnabled)
                        {
                            SelectPosition("Right");
                        }
                        else
                        {
                            const double baseX = 78;
                            SetLocations(baseX + 0, baseX + 256);
                            BattleView.AddMessage($"Send {Pokemon.Nickname} where?", true, false);
                            PositionsVisible = true;
                        }
                        break;
                    }
            }
        }
        void SelectTarget(string arg)
        {
            switch (arg)
            {
                case "AllyLeft": Pokemon.SelectedAction.FightTargets = targetAllyLeftResult; break;
                case "AllyCenter": Pokemon.SelectedAction.FightTargets = targetAllyCenterResult; break;
                case "AllyRight": Pokemon.SelectedAction.FightTargets = targetAllyRightResult; break;
                case "FoeLeft": Pokemon.SelectedAction.FightTargets = targetFoeLeftResult; break;
                case "FoeCenter": Pokemon.SelectedAction.FightTargets = targetFoeCenterResult; break;
                case "FoeRight": Pokemon.SelectedAction.FightTargets = targetFoeRightResult; break;
            }
            TargetsVisible = false;
            BattleView.Client.ActionsLoop(false);
        }
        void SelectPosition(string arg)
        {
            PBEFieldPosition pos = (PBEFieldPosition)Enum.Parse(typeof(PBEFieldPosition), arg);
            BattleView.Client.Switches.Add(Tuple.Create(Pokemon.Id, pos));
            BattleView.Client.StandBy.Add(Pokemon);
            BattleView.Client.PositionStandBy.Add(pos);
            PositionsVisible = false;
            BattleView.Client.SwitchesLoop(false);
        }
    }
}
