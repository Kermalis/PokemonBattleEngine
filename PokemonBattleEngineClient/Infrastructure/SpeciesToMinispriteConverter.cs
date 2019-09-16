using Avalonia.Data.Converters;
using Kermalis.PokemonBattleEngine.Data;
using System;
using System.Globalization;

namespace Kermalis.PokemonBattleEngineClient.Infrastructure
{
    public sealed class SpeciesToMinispriteConverter : IValueConverter
    {
        public static SpeciesToMinispriteConverter Instance { get; } = new SpeciesToMinispriteConverter();
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is PBESpecies species)
            {
                return Utils.GetMinisprite(species, PBEGender.Male, false);
            }
            else if (value is PBEPokemonShell shell)
            {
                return Utils.GetMinisprite(shell.Species, shell.Gender, shell.Shiny);
            }
            else
            {
                throw new ArgumentException();
            }
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
