using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media.Imaging;
using Kermalis.PokemonBattleEngine.Data;
using Kermalis.PokemonBattleEngineClient.Models;
using ReactiveUI;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Kermalis.PokemonBattleEngineClient.Views
{
    // If you dislike spaghetti code please close this file
    class ActionsView : UserControl, INotifyPropertyChanged
    {
        void OnPropertyChanged(string property) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        public new event PropertyChangedEventHandler PropertyChanged;

        IBitmap targetAllyLeft;
        IBitmap TargetAllyLeft
        {
            get => targetAllyLeft;
            set
            {
                targetAllyLeft = value;
                OnPropertyChanged(nameof(TargetAllyLeft));
            }
        }
        bool targetAllyLeftEnabled;
        bool TargetAllyLeftEnabled
        {
            get => targetAllyLeftEnabled;
            set
            {
                targetAllyLeftEnabled = value;
                OnPropertyChanged(nameof(TargetAllyLeftEnabled));
            }
        }
        IBitmap targetAllyCenter;
        IBitmap TargetAllyCenter
        {
            get => targetAllyCenter;
            set
            {
                targetAllyCenter = value;
                OnPropertyChanged(nameof(TargetAllyCenter));
            }
        }
        bool targetAllyCenterEnabled;
        bool TargetAllyCenterEnabled
        {
            get => targetAllyCenterEnabled;
            set
            {
                targetAllyCenterEnabled = value;
                OnPropertyChanged(nameof(TargetAllyCenterEnabled));
            }
        }
        IBitmap targetAllyRight;
        IBitmap TargetAllyRight
        {
            get => targetAllyRight;
            set
            {
                targetAllyRight = value;
                OnPropertyChanged(nameof(TargetAllyRight));
            }
        }
        bool targetAllyRightEnabled;
        bool TargetAllyRightEnabled
        {
            get => targetAllyRightEnabled;
            set
            {
                targetAllyRightEnabled = value;
                OnPropertyChanged(nameof(TargetAllyRightEnabled));
            }
        }
        IBitmap targetFoeLeft;
        IBitmap TargetFoeLeft
        {
            get => targetFoeLeft;
            set
            {
                targetFoeLeft = value;
                OnPropertyChanged(nameof(TargetFoeLeft));
            }
        }
        bool targetFoeLeftEnabled;
        bool TargetFoeLeftEnabled
        {
            get => targetFoeLeftEnabled;
            set
            {
                targetFoeLeftEnabled = value;
                OnPropertyChanged(nameof(TargetFoeLeftEnabled));
            }
        }
        IBitmap targetFoeCenter;
        IBitmap TargetFoeCenter
        {
            get => targetFoeCenter;
            set
            {
                targetFoeCenter = value;
                OnPropertyChanged(nameof(TargetFoeCenter));
            }
        }
        bool targetFoeCenterEnabled;
        bool TargetFoeCenterEnabled
        {
            get => targetFoeCenterEnabled;
            set
            {
                targetFoeCenterEnabled = value;
                OnPropertyChanged(nameof(TargetFoeCenterEnabled));
            }
        }
        IBitmap targetFoeRight;
        IBitmap TargetFoeRight
        {
            get => targetFoeRight;
            set
            {
                targetFoeRight = value;
                OnPropertyChanged(nameof(TargetFoeRight));
            }
        }
        bool targetFoeRightEnabled;
        bool TargetFoeRightEnabled
        {
            get => targetFoeRightEnabled;
            set
            {
                targetFoeRightEnabled = value;
                OnPropertyChanged(nameof(TargetFoeRightEnabled));
            }
        }

        bool centerTargetsVisible;
        bool CenterTargetsVisible
        {
            get => centerTargetsVisible;
            set
            {
                centerTargetsVisible = value;
                OnPropertyChanged(nameof(CenterTargetsVisible));
            }
        }
        double leftX;
        double LeftX
        {
            get => leftX;
            set
            {
                leftX = value;
                OnPropertyChanged(nameof(LeftX));
            }
        }
        double rightX;
        double RightX
        {
            get => rightX;
            set
            {
                rightX = value;
                OnPropertyChanged(nameof(RightX));
            }
        }
        double leftLineX;
        double LeftLineX
        {
            get => leftLineX;
            set
            {
                leftLineX = value;
                OnPropertyChanged(nameof(LeftLineX));
            }
        }
        double centerLineX;
        double CenterLineX
        {
            get => centerLineX;
            set
            {
                centerLineX = value;
                OnPropertyChanged(nameof(CenterLineX));
            }
        }
        double rightLineX;
        double RightLineX
        {
            get => rightLineX;
            set
            {
                rightLineX = value;
                OnPropertyChanged(nameof(RightLineX));
            }
        }

        bool targetLineFoeLeftFoeCenterEnabled;
        bool TargetLineFoeLeftFoeCenterEnabled
        {
            get => targetLineFoeLeftFoeCenterEnabled;
            set
            {
                targetLineFoeLeftFoeCenterEnabled = value;
                OnPropertyChanged(nameof(TargetLineFoeLeftFoeCenterEnabled));
            }
        }
        bool targetLineFoeLeftAllyRightEnabled;
        bool TargetLineFoeLeftAllyRightEnabled
        {
            get => targetLineFoeLeftAllyRightEnabled;
            set
            {
                targetLineFoeLeftAllyRightEnabled = value;
                OnPropertyChanged(nameof(TargetLineFoeLeftAllyRightEnabled));
            }
        }
        bool targetLineFoeCenterAllyCenterEnabled;
        bool TargetLineFoeCenterAllyCenterEnabled
        {
            get => targetLineFoeCenterAllyCenterEnabled;
            set
            {
                targetLineFoeCenterAllyCenterEnabled = value;
                OnPropertyChanged(nameof(TargetLineFoeCenterAllyCenterEnabled));
            }
        }
        bool targetLineFoeRightFoeCenterEnabled;
        bool TargetLineFoeRightFoeCenterEnabled
        {
            get => targetLineFoeRightFoeCenterEnabled;
            set
            {
                targetLineFoeRightFoeCenterEnabled = value;
                OnPropertyChanged(nameof(TargetLineFoeRightFoeCenterEnabled));
            }
        }
        bool targetLineFoeRightAllyLeftEnabled;
        bool TargetLineFoeRightAllyLeftEnabled
        {
            get => targetLineFoeRightAllyLeftEnabled;
            set
            {
                targetLineFoeRightAllyLeftEnabled = value;
                OnPropertyChanged(nameof(TargetLineFoeRightAllyLeftEnabled));
            }
        }
        bool targetLineAllyLeftAllyCenterEnabled;
        bool TargetLineAllyLeftAllyCenterEnabled
        {
            get => targetLineAllyLeftAllyCenterEnabled;
            set
            {
                targetLineAllyLeftAllyCenterEnabled = value;
                OnPropertyChanged(nameof(TargetLineAllyLeftAllyCenterEnabled));
            }
        }
        bool targetLineAllyRightAllyCenterEnabled;
        bool TargetLineAllyRightAllyCenterEnabled
        {
            get => targetLineAllyRightAllyCenterEnabled;
            set
            {
                targetLineAllyRightAllyCenterEnabled = value;
                OnPropertyChanged(nameof(TargetLineAllyRightAllyCenterEnabled));
            }
        }

        PBETarget targetAllyLeftResult, targetAllyCenterResult, targetAllyRightResult,
            targetFoeLeftResult, targetFoeCenterResult, targetFoeRightResult;

        ReactiveCommand SelectTargetCommand { get; }

        MoveInfo[] moves;
        MoveInfo[] Moves
        {
            get => moves;
            set
            {
                moves = value;
                OnPropertyChanged(nameof(Moves));
            }
        }

        PokemonInfo[] party;
        PokemonInfo[] Party
        {
            get => party;
            set
            {
                party = value;
                OnPropertyChanged(nameof(Party));
            }
        }

        bool targetsVisible;
        bool TargetsVisible
        {
            get => targetsVisible;
            set
            {
                targetsVisible = value;
                OnPropertyChanged(nameof(TargetsVisible));
            }
        }
        bool movesVisible;
        bool MovesVisible
        {
            get => movesVisible;
            set
            {
                movesVisible = value;
                OnPropertyChanged(nameof(MovesVisible));
            }
        }

        public BattleClient Client;
        public PBEPokemon Pokemon { get; private set; }

        public ActionsView()
        {
            AvaloniaXamlLoader.Load(this);
            DataContext = this;
            SelectTargetCommand = ReactiveCommand.Create<string>(SelectTarget);

            MoveInfo.CreateBrushes();
        }

        public void DisplayActions(IEnumerable<PBEPokemon> party, PBEPokemon pkmn, List<PBEPokemon> standBy)
        {
            Pokemon = pkmn;

            Party = party.Select(p => new PokemonInfo(p, this, standBy)).ToArray();

            var mInfo = new MoveInfo[PBESettings.NumMoves];
            for (int i = 0; i < PBESettings.NumMoves; i++)
                mInfo[i] = new MoveInfo(i, Pokemon, this);
            Moves = mInfo;

            MovesVisible = true;
        }

        public void SelectPokemon(PokemonInfo pkmnInfo)
        {
            Pokemon.SelectedAction.Decision = PBEDecision.SwitchOut;
            Pokemon.SelectedAction.SwitchPokemonId = pkmnInfo.Pokemon.Id;
            MovesVisible = false;
            Client.ActionsLoop(false);
        }
        public void SelectMove(MoveInfo moveInfo)
        {
            Pokemon.SelectedAction.Decision = PBEDecision.Fight;
            Pokemon.SelectedAction.FightMove = moveInfo.Move;
            MovesVisible = false;
            DisplayTargets(moveInfo);
        }
        void DisplayTargets(MoveInfo moveInfo)
        {
            PBEMoveTarget possibleTargets = PBEMoveData.GetMoveTargetsForPokemon(Pokemon, moveInfo.Move);

            if (Client.Battle.BattleFormat == PBEBattleFormat.Single || Client.Battle.BattleFormat == PBEBattleFormat.Rotation)
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
                Client.ActionsLoop(false);
            }
            else // Double / Triple
            {
                PBEPokemon pkmn;

                pkmn = Client.Battle.Teams[0].PokemonAtPosition(PBEFieldPosition.Left);
                TargetAllyLeft = Utils.RenderString(pkmn?.NameWithGender);
                pkmn = Client.Battle.Teams[0].PokemonAtPosition(PBEFieldPosition.Center);
                TargetAllyCenter = Utils.RenderString(pkmn?.NameWithGender);
                pkmn = Client.Battle.Teams[0].PokemonAtPosition(PBEFieldPosition.Right);
                TargetAllyRight = Utils.RenderString(pkmn?.NameWithGender);
                pkmn = Client.Battle.Teams[1].PokemonAtPosition(PBEFieldPosition.Left);
                TargetFoeLeft = Utils.RenderString(pkmn?.NameWithGender);
                pkmn = Client.Battle.Teams[1].PokemonAtPosition(PBEFieldPosition.Center);
                TargetFoeCenter = Utils.RenderString(pkmn?.NameWithGender);
                pkmn = Client.Battle.Teams[1].PokemonAtPosition(PBEFieldPosition.Right);
                TargetFoeRight = Utils.RenderString(pkmn?.NameWithGender);

                if (Client.Battle.BattleFormat == PBEBattleFormat.Double)
                {
                    const double baseX = 142;
                    LeftX = baseX + 0; RightX = baseX + 128; LeftLineX = baseX + 44; CenterLineX = baseX + 98; RightLineX = baseX + 172;
                    CenterTargetsVisible = false;
                    TargetLineFoeCenterAllyCenterEnabled = TargetLineFoeLeftFoeCenterEnabled = TargetLineAllyRightAllyCenterEnabled = false;
                    switch (possibleTargets)
                    {
                        case PBEMoveTarget.All:
                            TargetAllyLeftEnabled = TargetAllyRightEnabled = TargetFoeLeftEnabled = TargetFoeRightEnabled = true;
                            TargetLineAllyLeftAllyCenterEnabled = TargetLineFoeRightFoeCenterEnabled = TargetLineFoeLeftAllyRightEnabled = TargetLineFoeRightAllyLeftEnabled = true;
                            targetAllyLeftResult = targetAllyRightResult = targetFoeLeftResult = targetFoeRightResult = PBETarget.AllyLeft | PBETarget.AllyRight | PBETarget.FoeLeft | PBETarget.FoeRight;
                            break;
                        case PBEMoveTarget.AllFoes:
                        case PBEMoveTarget.AllFoesSurrounding:
                            TargetAllyLeftEnabled = TargetAllyRightEnabled = false;
                            TargetFoeLeftEnabled = TargetFoeRightEnabled = true;
                            TargetLineAllyLeftAllyCenterEnabled = TargetLineFoeLeftAllyRightEnabled = TargetLineFoeRightAllyLeftEnabled = false;
                            TargetLineFoeRightFoeCenterEnabled = true;
                            targetFoeLeftResult = targetFoeRightResult = PBETarget.FoeLeft | PBETarget.FoeRight;
                            break;
                        case PBEMoveTarget.AllSurrounding:
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
                        case PBEMoveTarget.AllTeam:
                            TargetAllyLeftEnabled = TargetAllyRightEnabled = true;
                            TargetFoeLeftEnabled = TargetFoeRightEnabled = false;
                            TargetLineFoeRightFoeCenterEnabled = TargetLineFoeLeftAllyRightEnabled = TargetLineFoeRightAllyLeftEnabled = false;
                            TargetLineAllyLeftAllyCenterEnabled = true;
                            targetAllyLeftResult = targetAllyRightResult = PBETarget.AllyLeft | PBETarget.AllyRight;
                            break;
                        case PBEMoveTarget.RandomFoeSurrounding:
                        case PBEMoveTarget.Self:
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
                        case PBEMoveTarget.SelfOrAllySurrounding:
                            TargetAllyLeftEnabled = TargetAllyRightEnabled = true;
                            TargetFoeLeftEnabled = TargetFoeRightEnabled = false;
                            TargetLineAllyLeftAllyCenterEnabled = TargetLineFoeRightFoeCenterEnabled = TargetLineFoeLeftAllyRightEnabled = TargetLineFoeRightAllyLeftEnabled = false;
                            targetAllyLeftResult = PBETarget.AllyLeft;
                            targetAllyRightResult = PBETarget.AllyRight;
                            break;
                        case PBEMoveTarget.SingleAllySurrounding:
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
                        case PBEMoveTarget.SingleFoeSurrounding:
                            TargetAllyLeftEnabled = TargetAllyRightEnabled = false;
                            TargetFoeLeftEnabled = TargetFoeRightEnabled = true;
                            TargetLineAllyLeftAllyCenterEnabled = TargetLineFoeRightFoeCenterEnabled = TargetLineFoeLeftAllyRightEnabled = TargetLineFoeRightAllyLeftEnabled = false;
                            targetFoeLeftResult = PBETarget.FoeLeft;
                            targetFoeRightResult = PBETarget.FoeRight;
                            break;
                        case PBEMoveTarget.SingleNotSelf:
                        case PBEMoveTarget.SingleSurrounding:
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
                else // Triple
                {
                    const double baseX = 78;
                    LeftX = baseX + 0; RightX = baseX + 256; LeftLineX = baseX + 44; CenterLineX = baseX + 98; RightLineX = baseX + 300;
                    CenterTargetsVisible = true;
                    switch (possibleTargets)
                    {
                        case PBEMoveTarget.All:
                            TargetAllyLeftEnabled = TargetAllyCenterEnabled = TargetAllyRightEnabled = TargetFoeLeftEnabled = TargetFoeCenterEnabled = TargetFoeRightEnabled = true;
                            TargetLineAllyLeftAllyCenterEnabled = TargetLineAllyRightAllyCenterEnabled = TargetLineFoeRightFoeCenterEnabled = TargetLineFoeLeftFoeCenterEnabled = TargetLineFoeCenterAllyCenterEnabled = TargetLineFoeLeftAllyRightEnabled = TargetLineFoeRightAllyLeftEnabled = true;
                            targetAllyLeftResult = targetAllyCenterResult = targetAllyRightResult = targetFoeLeftResult = targetFoeCenterResult = targetFoeRightResult = PBETarget.AllyLeft | PBETarget.AllyCenter | PBETarget.AllyRight | PBETarget.FoeLeft | PBETarget.FoeCenter | PBETarget.FoeRight;
                            break;
                        case PBEMoveTarget.AllFoes:
                            TargetAllyLeftEnabled = TargetAllyCenterEnabled = TargetAllyRightEnabled = false;
                            TargetFoeLeftEnabled = TargetFoeCenterEnabled = TargetFoeRightEnabled = true;
                            TargetLineAllyLeftAllyCenterEnabled = TargetLineAllyRightAllyCenterEnabled = TargetLineFoeCenterAllyCenterEnabled = TargetLineFoeLeftAllyRightEnabled = TargetLineFoeRightAllyLeftEnabled = false;
                            TargetLineFoeRightFoeCenterEnabled = TargetLineFoeLeftFoeCenterEnabled = true;
                            targetFoeLeftResult = targetFoeCenterResult = targetFoeRightResult = PBETarget.FoeLeft | PBETarget.FoeCenter | PBETarget.FoeRight;
                            break;
                        case PBEMoveTarget.AllFoesSurrounding:
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
                        case PBEMoveTarget.AllSurrounding:
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
                        case PBEMoveTarget.AllTeam:
                            TargetAllyLeftEnabled = TargetAllyCenterEnabled = TargetAllyRightEnabled = true;
                            TargetFoeLeftEnabled = TargetFoeCenterEnabled = TargetFoeRightEnabled = false;
                            TargetLineAllyLeftAllyCenterEnabled = TargetLineAllyRightAllyCenterEnabled = true;
                            TargetLineFoeRightFoeCenterEnabled = TargetLineFoeLeftFoeCenterEnabled = TargetLineFoeCenterAllyCenterEnabled = TargetLineFoeLeftAllyRightEnabled = TargetLineFoeRightAllyLeftEnabled = false;
                            targetAllyLeftResult = targetAllyCenterResult = targetAllyRightResult = PBETarget.AllyLeft | PBETarget.AllyCenter | PBETarget.AllyRight;
                            break;
                        case PBEMoveTarget.RandomFoeSurrounding:
                        case PBEMoveTarget.Self:
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
                        case PBEMoveTarget.SelfOrAllySurrounding:
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
                        case PBEMoveTarget.SingleAllySurrounding:
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
                        case PBEMoveTarget.SingleFoeSurrounding:
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
                        case PBEMoveTarget.SingleNotSelf:
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
                        case PBEMoveTarget.SingleSurrounding:
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

                if (Pokemon.LockedAction.Decision == PBEDecision.Fight && Pokemon.LockedAction.FightTargets != PBETarget.None)
                {
                    if (!Pokemon.LockedAction.FightTargets.HasFlag(PBETarget.AllyLeft))
                        TargetAllyLeftEnabled = false;
                    if (!Pokemon.LockedAction.FightTargets.HasFlag(PBETarget.AllyCenter))
                        TargetAllyCenterEnabled = false;
                    if (!Pokemon.LockedAction.FightTargets.HasFlag(PBETarget.AllyRight))
                        TargetAllyRightEnabled = false;
                    if (!Pokemon.LockedAction.FightTargets.HasFlag(PBETarget.FoeLeft))
                        TargetFoeLeftEnabled = false;
                    if (!Pokemon.LockedAction.FightTargets.HasFlag(PBETarget.FoeCenter))
                        TargetFoeCenterEnabled = false;
                    if (!Pokemon.LockedAction.FightTargets.HasFlag(PBETarget.FoeRight))
                        TargetFoeRightEnabled = false;
                }

                TargetsVisible = true;
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
            Client.ActionsLoop(false);
        }
    }
}
