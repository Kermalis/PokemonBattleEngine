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
            switch (value)
            {
                case PBEAbility ability: str = PBEAbilityLocalization.Names[ability].English; break;
                case PBEItem item: str = PBEItemLocalization.Names[item].English; break;
                case PBEMove move: str = PBEMoveLocalization.Names[move].English; break;
                case PBESpecies species: str = PBEPokemonLocalization.Names[species].English; break;
                default: str = value?.ToString(); break;
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
