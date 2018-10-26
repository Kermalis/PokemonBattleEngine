using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using System.Collections.Generic;
using System.ComponentModel;

namespace Kermalis.PokemonBattleEngineClient.Views
{
    public class BattleView : UserControl, INotifyPropertyChanged
    {
        void OnPropertyChanged(string property) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        public new event PropertyChangedEventHandler PropertyChanged;

        string message;
        public string Message
        {
            get => message;
            private set
            {
                message = value;
                OnPropertyChanged(nameof(Message));
                IsMessageBoxVisible = !string.IsNullOrWhiteSpace(message);
            }
        }
        bool isMessageBoxVisible;
        public bool IsMessageBoxVisible
        {
            get => isMessageBoxVisible;
            private set
            {
                isMessageBoxVisible = value;
                OnPropertyChanged(nameof(IsMessageBoxVisible));
            }
        }

        readonly List<string> messageQueue = new List<string>();

        public readonly PokemonView[] PokemonViews;

        public BattleView()
        {
            AvaloniaXamlLoader.Load(this);
            DataContext = this;

            Name = "Battle0"; // Temporary
            PokemonViews = new PokemonView[] { this.FindControl<PokemonView>("Battler0_0"), this.FindControl<PokemonView>("Battler0_1"), this.FindControl<PokemonView>("Battler0_2"),
                this.FindControl<PokemonView>("Battler1_0"), this.FindControl<PokemonView>("Battler1_1"), this.FindControl<PokemonView>("Battler1_2") };
        }

        public void AddMessage(string m, bool autoAdvance = false)
        {
            if (!isMessageBoxVisible && messageQueue.Count == 0)
            {
                Message = m;
            }
            else
            {
                messageQueue.Add(m);
            }
            if (autoAdvance)
                AdvanceMessage();
        }
        public void AdvanceMessage()
        {
            if (messageQueue.Count == 0)
            {
                Message = null;
            }
            else
            {
                Message = messageQueue[0];
                messageQueue.Remove(Message);
            }
        }
    }
}
