using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Kermalis.PokemonBattleEngine.Battle;
using Kermalis.PokemonBattleEngine.Data;
using Kermalis.PokemonBattleEngineClient.Models;
using System;
using System.ComponentModel;

namespace Kermalis.PokemonBattleEngineClient.Views
{
    class ActionsView : UserControl, INotifyPropertyChanged
    {
        void OnPropertyChanged(string property) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        public new event PropertyChangedEventHandler PropertyChanged;

        PPokemon pokemon;
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

        public BattleClient Client;

        public ActionsView()
        {
            AvaloniaXamlLoader.Load(this);
            DataContext = this;

            MoveInfo.CreateBrushes();
        }

        public void SetInfo(PPokemon pkmn)
        {
            pokemon = pkmn;
            var info = new MoveInfo[PConstants.NumMoves];
            for (int i = 0; i < PConstants.NumMoves; i++)
                info[i] = new MoveInfo(i, pkmn, this);
            Moves = info;
        }

        public void SelectMove(MoveInfo moveInfo)
        {
            int moveIndex = Array.IndexOf(moves, moveInfo);
            PMove move = pokemon.Shell.Moves[moveIndex];

            // TODO: Targets view, non-single battles
            PTarget targets = 0;
            switch (PMoveData.Data[move].Targets)
            {
                case PMoveTarget.All:
                    targets = PTarget.AllyCenter | PTarget.FoeCenter;
                    break;
                case PMoveTarget.AllFoes:
                case PMoveTarget.AllFoesSurrounding:
                case PMoveTarget.AllSurrounding:
                case PMoveTarget.RandomFoeSurrounding:
                case PMoveTarget.SingleFoeSurrounding:
                case PMoveTarget.SingleNotSelf:
                case PMoveTarget.SingleSurrounding:
                    targets = PTarget.FoeCenter;
                    break;
                case PMoveTarget.AllTeam:
                case PMoveTarget.Self:
                case PMoveTarget.SelfOrAllySurrounding:
                case PMoveTarget.SingleAllySurrounding:
                    targets = PTarget.AllyCenter;
                    break;
            }

            var action = new PAction
            {
                PokemonId = pokemon.Id,
                Move = move,
                Targets = targets
            };
            Client.MoveSelected(action);
        }
    }
}
