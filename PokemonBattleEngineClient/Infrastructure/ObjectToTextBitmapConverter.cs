using Avalonia.Data.Converters;
using Kermalis.PokemonBattleEngine.Data;
using Kermalis.PokemonBattleEngine.Localization;
using System;
using System.Globalization;

namespace Kermalis.PokemonBattleEngineClient.Infrastructure
{
    class ObjectToTextBitmapConverter : IValueConverter
    {
        public static ObjectToTextBitmapConverter Instance { get; } = new ObjectToTextBitmapConverter();
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string str;
            if (value is PBEAbility ability)
            {
                str = PBEAbilityLocalization.Names[ability].English;
            }
            else if (value is PBESpecies species)
            {
                str = PBEPokemonLocalization.Names[species].English;
            }
            else
            {
                str = value?.ToString();
            }
            Enum.TryParse(parameter?.ToString(), out Utils.StringRenderStyle style);
            return Utils.RenderString(str, style);
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
