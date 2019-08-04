using Kermalis.PokemonBattleEngine.Data;
using System.ComponentModel;

namespace Kermalis.PokemonBattleEngineClient.Models
{
    public class TeamInfo : INotifyPropertyChanged
    {
        private void OnPropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }
        public event PropertyChangedEventHandler PropertyChanged;

        private string name;
        public string Name
        {
            get => name;
            set
            {
                name = value;
                OnPropertyChanged(nameof(Name));
            }
        }
        private PBETeamShell shell;
        public PBETeamShell Shell
        {
            get => shell;
            set
            {
                shell = value;
                OnPropertyChanged(nameof(Shell));
            }
        }
    }
}
