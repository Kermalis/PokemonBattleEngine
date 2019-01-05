using Avalonia.Data.Converters;
using Kermalis.PokemonBattleEngine.Data;
using Kermalis.PokemonBattleEngine.Localization;
using System;
using System.Globalization;

namespace Kermalis.PokemonBattleEngineClient.Infrastructure
{
    class SpeciesToLocalizedBitmapConverter : IValueConverter
    {
        public static SpeciesToLocalizedBitmapConverter Instance { get; } = new SpeciesToLocalizedBitmapConverter();
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ObjectToTextBitmapConverter.Instance.Convert(PBEPokemonLocalization.Names[(PBESpecies)value].English, null, parameter, null);
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
