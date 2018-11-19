using Avalonia.Controls;
using Avalonia.Markup.Xaml;
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

        string targetAllyLeft;
        string TargetAllyLeft
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
        string targetAllyCenter;
        string TargetAllyCenter
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
        string targetAllyRight;
        string TargetAllyRight
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
        string targetFoeLeft;
        string TargetFoeLeft
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
        string targetFoeCenter;
        string TargetFoeCenter
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
        string targetFoeRight;
        string TargetFoeRight
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

        PTarget targetAllyLeftResult, targetAllyCenterResult, targetAllyRightResult,
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
        public PPokemon Pokemon { get; private set; }

        public ActionsView()
        {
            AvaloniaXamlLoader.Load(this);
            DataContext = this;
            SelectTargetCommand = ReactiveCommand.Create<string>(SelectTarget);

            MoveInfo.CreateBrushes();
        }

        public void DisplayActions(IEnumerable<PPokemon> party, PPokemon pkmn, List<PPokemon> standBy)
        {
            Pokemon = pkmn;
            
            Party = party.Select(p => new PokemonInfo(p, this, standBy)).ToArray();

            var mInfo = new MoveInfo[PSettings.NumMoves];
            for (int i = 0; i < PSettings.NumMoves; i++)
                mInfo[i] = new MoveInfo(i, Pokemon, this);
            Moves = mInfo;

            MovesVisible = true;
        }

        public void SelectPokemon(PokemonInfo pkmnInfo)
        {
            Pokemon.SelectedAction.Decision = PDecision.Switch;
            Pokemon.SelectedAction.SwitchPokemonId = pkmnInfo.Pokemon.Id;
            MovesVisible = false;
            Client.ActionSet();
        }
        public void SelectMove(MoveInfo moveInfo)
        {
            Pokemon.SelectedAction.Decision = PDecision.Fight;
            Pokemon.SelectedAction.FightMove = moveInfo.Move;
            MovesVisible = false;
            DisplayTargets(moveInfo);
        }
        void DisplayTargets(MoveInfo moveInfo)
        {
            PMoveData mData = PMoveData.Data[moveInfo.Move];

            if (Client.Battle.BattleStyle == PBattleStyle.Single || Client.Battle.BattleStyle == PBattleStyle.Rotation)
            {
                switch (mData.Targets)
                {
                    case PMoveTarget.All:
                        Pokemon.SelectedAction.FightTargets = PTarget.AllyCenter | PTarget.FoeCenter;
                        break;
                    case PMoveTarget.AllFoes:
                    case PMoveTarget.AllFoesSurrounding:
                    case PMoveTarget.AllSurrounding:
                    case PMoveTarget.RandomFoeSurrounding:
                    case PMoveTarget.SingleFoeSurrounding:
                    case PMoveTarget.SingleNotSelf:
                    case PMoveTarget.SingleSurrounding:
                        Pokemon.SelectedAction.FightTargets = PTarget.FoeCenter;
                        break;
                    case PMoveTarget.AllTeam:
                    case PMoveTarget.Self:
                    case PMoveTarget.SelfOrAllySurrounding:
                    case PMoveTarget.SingleAllySurrounding:
                        Pokemon.SelectedAction.FightTargets = PTarget.AllyCenter;
                        break;
                }
                Client.ActionSet();
            }
            else // Double / Triple
            {
                PPokemon pkmn;

                pkmn = Client.Battle.Teams[0].PokemonAtPosition(PFieldPosition.Left);
                TargetAllyLeft = pkmn?.NameWithGender;
                pkmn = Client.Battle.Teams[0].PokemonAtPosition(PFieldPosition.Center);
                TargetAllyCenter = pkmn?.NameWithGender;
                pkmn = Client.Battle.Teams[0].PokemonAtPosition(PFieldPosition.Right);
                TargetAllyRight = pkmn?.NameWithGender;
                pkmn = Client.Battle.Teams[1].PokemonAtPosition(PFieldPosition.Left);
                TargetFoeLeft = pkmn?.NameWithGender;
                pkmn = Client.Battle.Teams[1].PokemonAtPosition(PFieldPosition.Center);
                TargetFoeCenter = pkmn?.NameWithGender;
                pkmn = Client.Battle.Teams[1].PokemonAtPosition(PFieldPosition.Right);
                TargetFoeRight = pkmn?.NameWithGender;

                if (Client.Battle.BattleStyle == PBattleStyle.Double)
                {
                    const double baseX = 142;
                    LeftX = baseX + 0; RightX = baseX + 128; LeftLineX = baseX + 44; CenterLineX = baseX + 98; RightLineX = baseX + 300;
                    CenterTargetsVisible = false;
                    TargetLineFoeCenterAllyCenterEnabled = TargetLineFoeLeftFoeCenterEnabled = TargetLineAllyRightAllyCenterEnabled = false;
                    switch (mData.Targets)
                    {
                        case PMoveTarget.All:
                            TargetAllyLeftEnabled = TargetAllyRightEnabled = TargetFoeLeftEnabled = TargetFoeRightEnabled = true;
                            TargetLineAllyLeftAllyCenterEnabled = TargetLineFoeRightFoeCenterEnabled = TargetLineFoeLeftAllyRightEnabled = TargetLineFoeRightAllyLeftEnabled = true;
                            targetAllyLeftResult = targetAllyRightResult = targetFoeLeftResult = targetFoeRightResult = PTarget.AllyLeft | PTarget.AllyRight | PTarget.FoeLeft | PTarget.FoeRight;
                            break;
                        case PMoveTarget.AllFoes:
                        case PMoveTarget.AllFoesSurrounding:
                            TargetAllyLeftEnabled = TargetAllyRightEnabled = false;
                            TargetFoeLeftEnabled = TargetFoeRightEnabled = true;
                            TargetLineAllyLeftAllyCenterEnabled = TargetLineFoeLeftAllyRightEnabled = TargetLineFoeRightAllyLeftEnabled = false;
                            TargetLineFoeRightFoeCenterEnabled = true;
                            targetFoeLeftResult = targetFoeRightResult = PTarget.FoeLeft | PTarget.FoeRight;
                            break;
                        case PMoveTarget.AllSurrounding:
                            if (Pokemon.FieldPosition == PFieldPosition.Left)
                            {
                                TargetAllyLeftEnabled = false;
                                TargetAllyRightEnabled = true;
                                TargetLineFoeRightAllyLeftEnabled = false;
                                TargetLineFoeLeftAllyRightEnabled = true;
                                targetAllyRightResult = targetFoeLeftResult = targetFoeRightResult = PTarget.AllyRight | PTarget.FoeLeft | PTarget.FoeRight;
                            }
                            else
                            {
                                TargetAllyLeftEnabled = true;
                                TargetAllyRightEnabled = false;
                                TargetLineFoeRightAllyLeftEnabled = true;
                                TargetLineFoeLeftAllyRightEnabled = false;
                                targetAllyLeftResult = targetFoeLeftResult = targetFoeRightResult = PTarget.AllyLeft | PTarget.FoeLeft | PTarget.FoeRight;
                            }
                            TargetFoeLeftEnabled = TargetFoeRightEnabled = true;
                            TargetLineAllyLeftAllyCenterEnabled = false;
                            TargetLineFoeRightFoeCenterEnabled = true;
                            break;
                        case PMoveTarget.AllTeam:
                            TargetAllyLeftEnabled = TargetAllyRightEnabled = true;
                            TargetFoeLeftEnabled = TargetFoeRightEnabled = false;
                            TargetLineFoeRightFoeCenterEnabled = TargetLineFoeLeftAllyRightEnabled = TargetLineFoeRightAllyLeftEnabled = false;
                            TargetLineAllyLeftAllyCenterEnabled = true;
                            targetAllyLeftResult = targetAllyRightResult = PTarget.AllyLeft | PTarget.AllyRight;
                            break;
                        case PMoveTarget.RandomFoeSurrounding:
                        case PMoveTarget.Self:
                            if (Pokemon.FieldPosition == PFieldPosition.Left)
                            {
                                TargetAllyLeftEnabled = true;
                                TargetAllyRightEnabled = false;
                                targetAllyLeftResult = PTarget.AllyLeft;
                            }
                            else
                            {
                                TargetAllyLeftEnabled = false;
                                TargetAllyRightEnabled = true;
                                targetAllyRightResult = PTarget.AllyRight;
                            }
                            TargetFoeLeftEnabled = TargetFoeRightEnabled = false;
                            TargetLineAllyLeftAllyCenterEnabled = TargetLineFoeRightFoeCenterEnabled = TargetLineFoeLeftAllyRightEnabled = TargetLineFoeRightAllyLeftEnabled = false;
                            break;
                        case PMoveTarget.SelfOrAllySurrounding:
                            TargetAllyLeftEnabled = TargetAllyRightEnabled = true;
                            TargetFoeLeftEnabled = TargetFoeRightEnabled = false;
                            TargetLineAllyLeftAllyCenterEnabled = TargetLineFoeRightFoeCenterEnabled = TargetLineFoeLeftAllyRightEnabled = TargetLineFoeRightAllyLeftEnabled = false;
                            targetAllyLeftResult = PTarget.AllyLeft;
                            targetAllyRightResult = PTarget.AllyRight;
                            break;
                        case PMoveTarget.SingleAllySurrounding:
                            if (Pokemon.FieldPosition == PFieldPosition.Left)
                            {
                                TargetAllyLeftEnabled = false;
                                TargetAllyRightEnabled = true;
                                targetAllyRightResult = PTarget.AllyRight;
                            }
                            else
                            {
                                TargetAllyLeftEnabled = true;
                                TargetAllyRightEnabled = false;
                                targetAllyLeftResult = PTarget.AllyLeft;
                            }
                            TargetFoeLeftEnabled = TargetFoeRightEnabled = false;
                            TargetLineAllyLeftAllyCenterEnabled = TargetLineFoeRightFoeCenterEnabled = TargetLineFoeLeftAllyRightEnabled = TargetLineFoeRightAllyLeftEnabled = false;
                            break;
                        case PMoveTarget.SingleFoeSurrounding:
                            TargetAllyLeftEnabled = TargetAllyRightEnabled = false;
                            TargetFoeLeftEnabled = TargetFoeRightEnabled = true;
                            TargetLineAllyLeftAllyCenterEnabled = TargetLineFoeRightFoeCenterEnabled = TargetLineFoeLeftAllyRightEnabled = TargetLineFoeRightAllyLeftEnabled = false;
                            targetFoeLeftResult = PTarget.FoeLeft;
                            targetFoeRightResult = PTarget.FoeRight;
                            break;
                        case PMoveTarget.SingleNotSelf:
                        case PMoveTarget.SingleSurrounding:
                            if (Pokemon.FieldPosition == PFieldPosition.Left)
                            {
                                TargetAllyLeftEnabled = false;
                                TargetAllyRightEnabled = true;
                                targetAllyRightResult = PTarget.AllyRight;
                            }
                            else
                            {
                                TargetAllyLeftEnabled = true;
                                TargetAllyRightEnabled = false;
                                targetAllyLeftResult = PTarget.AllyLeft;
                            }
                            TargetFoeLeftEnabled = TargetFoeRightEnabled = true;
                            TargetLineAllyLeftAllyCenterEnabled = TargetLineFoeRightFoeCenterEnabled = TargetLineFoeLeftAllyRightEnabled = TargetLineFoeRightAllyLeftEnabled = false;
                            targetFoeLeftResult = PTarget.FoeLeft;
                            targetFoeRightResult = PTarget.FoeRight;
                            break;
                    }
                }
                else // Triple
                {
                    const double baseX = 78;
                    LeftX = baseX + 0; RightX = baseX + 256; LeftLineX = baseX + 44; CenterLineX = baseX + 98; RightLineX = baseX + 300;
                    CenterTargetsVisible = true;
                    switch (mData.Targets)
                    {
                        case PMoveTarget.All:
                            TargetAllyLeftEnabled = TargetAllyCenterEnabled = TargetAllyRightEnabled = TargetFoeLeftEnabled = TargetFoeCenterEnabled = TargetFoeRightEnabled = true;
                            TargetLineAllyLeftAllyCenterEnabled = TargetLineAllyRightAllyCenterEnabled = TargetLineFoeRightFoeCenterEnabled = TargetLineFoeLeftFoeCenterEnabled = TargetLineFoeCenterAllyCenterEnabled = TargetLineFoeLeftAllyRightEnabled = TargetLineFoeRightAllyLeftEnabled = true;
                            targetAllyLeftResult = targetAllyCenterResult = targetAllyRightResult = targetFoeLeftResult = targetFoeCenterResult = targetFoeRightResult = PTarget.AllyLeft | PTarget.AllyCenter | PTarget.AllyRight | PTarget.FoeLeft | PTarget.FoeCenter | PTarget.FoeRight;
                            break;
                        case PMoveTarget.AllFoes:
                            TargetAllyLeftEnabled = TargetAllyCenterEnabled = TargetAllyRightEnabled = false;
                            TargetFoeLeftEnabled = TargetFoeCenterEnabled = TargetFoeRightEnabled = true;
                            TargetLineAllyLeftAllyCenterEnabled = TargetLineAllyRightAllyCenterEnabled = TargetLineFoeCenterAllyCenterEnabled = TargetLineFoeLeftAllyRightEnabled = TargetLineFoeRightAllyLeftEnabled = false;
                            TargetLineFoeRightFoeCenterEnabled = TargetLineFoeLeftFoeCenterEnabled = true;
                            targetFoeLeftResult = targetFoeCenterResult = targetFoeRightResult = PTarget.FoeLeft | PTarget.FoeCenter | PTarget.FoeRight;
                            break;
                        case PMoveTarget.AllFoesSurrounding:
                            if (Pokemon.FieldPosition == PFieldPosition.Left)
                            {
                                TargetFoeCenterEnabled = TargetFoeRightEnabled = true;
                                TargetFoeLeftEnabled = false;
                                TargetLineFoeRightFoeCenterEnabled = true;
                                TargetLineFoeLeftFoeCenterEnabled = false;
                                targetFoeCenterResult = targetFoeRightResult = PTarget.FoeCenter | PTarget.FoeRight;
                            }
                            else if (Pokemon.FieldPosition == PFieldPosition.Center)
                            {
                                TargetFoeLeftEnabled = TargetFoeCenterEnabled = TargetFoeRightEnabled = true;
                                TargetLineFoeRightFoeCenterEnabled = TargetLineFoeLeftFoeCenterEnabled = true;
                                targetFoeLeftResult = targetFoeCenterResult = targetFoeRightResult = PTarget.FoeLeft | PTarget.FoeCenter | PTarget.FoeRight;
                            }
                            else
                            {
                                TargetFoeLeftEnabled = TargetFoeCenterEnabled = true;
                                TargetFoeRightEnabled = false;
                                TargetLineFoeLeftFoeCenterEnabled = true;
                                TargetLineFoeRightFoeCenterEnabled = false;
                                targetFoeLeftResult = targetFoeCenterResult = PTarget.FoeLeft | PTarget.FoeCenter;
                            }
                            TargetAllyLeftEnabled = TargetAllyCenterEnabled = TargetAllyRightEnabled = false;
                            TargetLineFoeRightAllyLeftEnabled = TargetLineAllyLeftAllyCenterEnabled = TargetLineAllyRightAllyCenterEnabled = TargetLineFoeCenterAllyCenterEnabled = TargetLineFoeLeftAllyRightEnabled = false;
                            break;
                        case PMoveTarget.AllSurrounding:
                            if (Pokemon.FieldPosition == PFieldPosition.Left)
                            {
                                TargetFoeRightEnabled = TargetFoeCenterEnabled = TargetAllyCenterEnabled = true;
                                TargetAllyLeftEnabled = TargetAllyRightEnabled = TargetFoeLeftEnabled = false;
                                TargetLineAllyLeftAllyCenterEnabled = TargetLineAllyRightAllyCenterEnabled = TargetLineFoeLeftFoeCenterEnabled = TargetLineFoeLeftAllyRightEnabled = TargetLineFoeRightAllyLeftEnabled = false;
                                TargetLineFoeRightFoeCenterEnabled = TargetLineFoeCenterAllyCenterEnabled = true;
                                targetAllyCenterResult = targetFoeCenterResult = targetFoeRightResult = PTarget.AllyCenter | PTarget.FoeCenter | PTarget.FoeRight;
                            }
                            else if (Pokemon.FieldPosition == PFieldPosition.Center)
                            {
                                TargetAllyLeftEnabled = TargetAllyRightEnabled = TargetFoeLeftEnabled = TargetFoeCenterEnabled = TargetFoeRightEnabled = true;
                                TargetAllyCenterEnabled = false;
                                TargetLineAllyLeftAllyCenterEnabled = TargetLineAllyRightAllyCenterEnabled = TargetLineFoeCenterAllyCenterEnabled = false;
                                TargetLineFoeRightFoeCenterEnabled = TargetLineFoeLeftFoeCenterEnabled = TargetLineFoeLeftAllyRightEnabled = TargetLineFoeRightAllyLeftEnabled = true;
                                targetAllyLeftResult = targetAllyRightResult = targetFoeLeftResult = targetFoeCenterResult = targetFoeRightResult = PTarget.AllyLeft | PTarget.AllyRight | PTarget.FoeLeft | PTarget.FoeCenter | PTarget.FoeRight;
                            }
                            else
                            {
                                TargetAllyLeftEnabled = TargetAllyRightEnabled = TargetFoeRightEnabled = false;
                                TargetAllyCenterEnabled = TargetFoeLeftEnabled = TargetFoeCenterEnabled = true;
                                TargetLineAllyLeftAllyCenterEnabled = TargetLineAllyRightAllyCenterEnabled = TargetLineFoeRightFoeCenterEnabled = TargetLineFoeLeftAllyRightEnabled = TargetLineFoeRightAllyLeftEnabled = false;
                                TargetLineFoeLeftFoeCenterEnabled = TargetLineFoeCenterAllyCenterEnabled = true;
                                targetAllyCenterResult = targetFoeLeftResult = targetFoeCenterResult = PTarget.AllyCenter | PTarget.FoeLeft | PTarget.FoeCenter;
                            }
                            break;
                        case PMoveTarget.AllTeam:
                            TargetAllyLeftEnabled = TargetAllyCenterEnabled = TargetAllyRightEnabled = true;
                            TargetFoeLeftEnabled = TargetFoeCenterEnabled = TargetFoeRightEnabled = false;
                            TargetLineAllyLeftAllyCenterEnabled = TargetLineAllyRightAllyCenterEnabled = true;
                            TargetLineFoeRightFoeCenterEnabled = TargetLineFoeLeftFoeCenterEnabled = TargetLineFoeCenterAllyCenterEnabled = TargetLineFoeLeftAllyRightEnabled = TargetLineFoeRightAllyLeftEnabled = false;
                            targetAllyLeftResult = targetAllyCenterResult = targetAllyRightResult = PTarget.AllyLeft | PTarget.AllyCenter | PTarget.AllyRight;
                            break;
                        case PMoveTarget.RandomFoeSurrounding:
                        case PMoveTarget.Self:
                            if (Pokemon.FieldPosition == PFieldPosition.Left)
                            {
                                TargetAllyLeftEnabled = true;
                                TargetAllyCenterEnabled = TargetAllyRightEnabled = false;
                                targetAllyLeftResult = PTarget.AllyLeft;
                            }
                            else if (Pokemon.FieldPosition == PFieldPosition.Center)
                            {
                                TargetAllyCenterEnabled = true;
                                TargetAllyLeftEnabled = TargetAllyRightEnabled = false;
                                targetAllyCenterResult = PTarget.AllyCenter;
                            }
                            else
                            {
                                TargetAllyRightEnabled = true;
                                TargetAllyLeftEnabled = TargetAllyCenterEnabled = false;
                                targetAllyRightResult = PTarget.AllyRight;
                            }
                            TargetFoeLeftEnabled = TargetFoeCenterEnabled = TargetFoeRightEnabled = false;
                            TargetLineAllyLeftAllyCenterEnabled = TargetLineAllyRightAllyCenterEnabled = TargetLineFoeRightFoeCenterEnabled = TargetLineFoeLeftFoeCenterEnabled = TargetLineFoeCenterAllyCenterEnabled = TargetLineFoeLeftAllyRightEnabled = TargetLineFoeRightAllyLeftEnabled = false;
                            break;
                        case PMoveTarget.SelfOrAllySurrounding:
                            if (Pokemon.FieldPosition == PFieldPosition.Left)
                            {
                                TargetAllyLeftEnabled = TargetAllyCenterEnabled = true;
                                TargetAllyRightEnabled = false;
                                targetAllyLeftResult = PTarget.AllyLeft;
                                targetAllyCenterResult = PTarget.AllyCenter;
                            }
                            else if (Pokemon.FieldPosition == PFieldPosition.Center)
                            {
                                TargetAllyCenterEnabled = TargetAllyLeftEnabled = TargetAllyRightEnabled = true;
                                targetAllyLeftResult = PTarget.AllyLeft;
                                targetAllyCenterResult = PTarget.AllyCenter;
                                targetAllyRightResult = PTarget.AllyRight;
                            }
                            else
                            {
                                TargetAllyCenterEnabled = TargetAllyRightEnabled = true;
                                TargetAllyLeftEnabled = false;
                                targetAllyCenterResult = PTarget.AllyCenter;
                                targetAllyRightResult = PTarget.AllyRight;
                            }
                            TargetFoeLeftEnabled = TargetFoeCenterEnabled = TargetFoeRightEnabled = false;
                            TargetLineAllyLeftAllyCenterEnabled = TargetLineAllyRightAllyCenterEnabled = TargetLineFoeRightFoeCenterEnabled = TargetLineFoeLeftFoeCenterEnabled = TargetLineFoeCenterAllyCenterEnabled = TargetLineFoeLeftAllyRightEnabled = TargetLineFoeRightAllyLeftEnabled = false;
                            break;
                        case PMoveTarget.SingleAllySurrounding:
                            if (Pokemon.FieldPosition == PFieldPosition.Left || Pokemon.FieldPosition == PFieldPosition.Right)
                            {
                                TargetAllyCenterEnabled = true;
                                TargetAllyLeftEnabled = TargetAllyRightEnabled = false;
                                targetAllyCenterResult = PTarget.AllyCenter;
                            }
                            else
                            {
                                TargetAllyCenterEnabled = false;
                                TargetAllyLeftEnabled = TargetAllyRightEnabled = true;
                                targetAllyLeftResult = PTarget.AllyLeft;
                                targetAllyRightResult = PTarget.AllyRight;
                            }
                            TargetFoeLeftEnabled = TargetFoeCenterEnabled = TargetFoeRightEnabled = false;
                            TargetLineAllyLeftAllyCenterEnabled = TargetLineAllyRightAllyCenterEnabled = TargetLineFoeRightFoeCenterEnabled = TargetLineFoeLeftFoeCenterEnabled = TargetLineFoeCenterAllyCenterEnabled = TargetLineFoeLeftAllyRightEnabled = TargetLineFoeRightAllyLeftEnabled = false;
                            break;
                        case PMoveTarget.SingleFoeSurrounding:
                            if (Pokemon.FieldPosition == PFieldPosition.Left)
                            {
                                TargetFoeCenterEnabled = TargetFoeRightEnabled = true;
                                TargetFoeLeftEnabled = false;
                                targetFoeCenterResult = PTarget.FoeCenter;
                                targetFoeRightResult = PTarget.FoeRight;
                            }
                            else if (Pokemon.FieldPosition == PFieldPosition.Center)
                            {
                                TargetFoeLeftEnabled = TargetFoeCenterEnabled = TargetFoeRightEnabled = true;
                                targetFoeLeftResult = PTarget.FoeLeft;
                                targetFoeCenterResult = PTarget.FoeCenter;
                                targetFoeRightResult = PTarget.FoeRight;
                            }
                            else
                            {
                                TargetFoeLeftEnabled = TargetFoeCenterEnabled = true;
                                TargetFoeRightEnabled = false;
                                targetFoeLeftResult = PTarget.FoeLeft;
                                targetFoeCenterResult = PTarget.FoeCenter;
                            }
                            TargetAllyLeftEnabled = TargetAllyCenterEnabled = TargetAllyRightEnabled = false;
                            TargetLineAllyLeftAllyCenterEnabled = TargetLineAllyRightAllyCenterEnabled = TargetLineFoeRightFoeCenterEnabled = TargetLineFoeLeftFoeCenterEnabled = TargetLineFoeCenterAllyCenterEnabled = TargetLineFoeLeftAllyRightEnabled = TargetLineFoeRightAllyLeftEnabled = false;
                            break;
                        case PMoveTarget.SingleNotSelf:
                            if (Pokemon.FieldPosition == PFieldPosition.Left)
                            {
                                TargetAllyLeftEnabled = false;
                                TargetAllyCenterEnabled = TargetAllyRightEnabled = true;
                                targetAllyCenterResult = PTarget.AllyCenter;
                                targetAllyRightResult = PTarget.AllyRight;
                            }
                            else if (Pokemon.FieldPosition == PFieldPosition.Center)
                            {
                                TargetAllyCenterEnabled = false;
                                TargetAllyLeftEnabled = TargetAllyRightEnabled = true;
                                targetAllyLeftResult = PTarget.AllyLeft;
                                targetAllyRightResult = PTarget.AllyRight;
                            }
                            else
                            {
                                TargetAllyRightEnabled = false;
                                TargetAllyLeftEnabled = TargetAllyCenterEnabled = true;
                                targetAllyLeftResult = PTarget.AllyLeft;
                                targetAllyCenterResult = PTarget.AllyCenter;
                            }
                            TargetFoeLeftEnabled = TargetFoeCenterEnabled = TargetFoeRightEnabled = true;
                            TargetLineAllyLeftAllyCenterEnabled = TargetLineAllyRightAllyCenterEnabled = TargetLineFoeRightFoeCenterEnabled = TargetLineFoeLeftFoeCenterEnabled = TargetLineFoeCenterAllyCenterEnabled = TargetLineFoeLeftAllyRightEnabled = TargetLineFoeRightAllyLeftEnabled = false;
                            targetFoeLeftResult = PTarget.FoeLeft;
                            targetFoeCenterResult = PTarget.FoeCenter;
                            targetFoeRightResult = PTarget.FoeRight;
                            break;
                        case PMoveTarget.SingleSurrounding:
                            if (Pokemon.FieldPosition == PFieldPosition.Left)
                            {
                                TargetAllyCenterEnabled = TargetFoeCenterEnabled = TargetFoeRightEnabled = true;
                                TargetAllyLeftEnabled = TargetAllyRightEnabled = TargetFoeLeftEnabled = false;
                                targetAllyCenterResult = PTarget.AllyCenter;
                                targetFoeCenterResult = PTarget.FoeCenter;
                                targetFoeRightResult = PTarget.FoeRight;
                            }
                            else if (Pokemon.FieldPosition == PFieldPosition.Center)
                            {
                                TargetAllyLeftEnabled = TargetAllyRightEnabled = TargetFoeLeftEnabled = TargetFoeCenterEnabled = TargetFoeRightEnabled = true;
                                TargetAllyCenterEnabled = false;
                                targetAllyLeftResult = PTarget.AllyLeft;
                                targetAllyRightResult = PTarget.AllyRight;
                                targetFoeLeftResult = PTarget.FoeLeft;
                                targetFoeCenterResult = PTarget.FoeCenter;
                                targetFoeRightResult = PTarget.FoeRight;
                            }
                            else
                            {
                                TargetAllyCenterEnabled = TargetFoeLeftEnabled = TargetFoeCenterEnabled = true;
                                TargetAllyLeftEnabled = TargetAllyRightEnabled = TargetFoeRightEnabled = false;
                                targetAllyCenterResult = PTarget.AllyCenter;
                                targetFoeLeftResult = PTarget.FoeLeft;
                                targetFoeCenterResult = PTarget.FoeCenter;
                            }
                            TargetLineAllyLeftAllyCenterEnabled = TargetLineAllyRightAllyCenterEnabled = TargetLineFoeRightFoeCenterEnabled = TargetLineFoeLeftFoeCenterEnabled = TargetLineFoeCenterAllyCenterEnabled = TargetLineFoeLeftAllyRightEnabled = TargetLineFoeRightAllyLeftEnabled = false;
                            break;
                    }
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
            Client.ActionSet();
        }
    }
}
