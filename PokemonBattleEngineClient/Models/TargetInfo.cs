using Kermalis.PokemonBattleEngine.Battle;
using System.ComponentModel;

namespace Kermalis.PokemonBattleEngineClient.Models
{
    public sealed class TargetInfo : INotifyPropertyChanged
    {
        private void OnPropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }
        public event PropertyChangedEventHandler? PropertyChanged;

        private bool _enabled;
        public bool Enabled
        {
            get => _enabled;
            internal set
            {
                if (_enabled != value)
                {
                    _enabled = value;
                    OnPropertyChanged(nameof(Enabled));
                }
            }
        }
        private PokemonInfo _pokemon; // TODO: Can be null?
        public PokemonInfo Pokemon
        {
            get => _pokemon;
            internal set
            {
                if (_pokemon != value)
                {
                    _pokemon = value;
                    OnPropertyChanged(nameof(Pokemon));
                }
            }
        }
        private bool _lineRightVisible;
        public bool LineRightVisible
        {
            get => _lineRightVisible;
            internal set
            {
                if (_lineRightVisible != value)
                {
                    _lineRightVisible = value;
                    OnPropertyChanged(nameof(LineRightVisible));
                }
            }
        }
        private bool _lineDownVisible;
        public bool LineDownVisible
        {
            get => _lineDownVisible;
            internal set
            {
                if (_lineDownVisible != value)
                {
                    _lineDownVisible = value;
                    OnPropertyChanged(nameof(LineDownVisible));
                }
            }
        }

        internal PBETurnTarget Targets { get; set; }
    }
}
