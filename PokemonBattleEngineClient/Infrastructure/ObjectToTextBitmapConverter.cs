using Avalonia.Data.Converters;
using Kermalis.PokemonBattleEngine.Data;
using System;
using System.Globalization;

namespace Kermalis.PokemonBattleEngineClient.Infrastructure
{
    public class ObjectToTextBitmapConverter : IValueConverter
    {
        public static ObjectToTextBitmapConverter Instance { get; } = new ObjectToTextBitmapConverter();
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            PBELocalizedString localized = null;
            switch (value)
            {
                case PBEAbility ability: localized = PBELocalizedString.GetAbilityName(ability); break;
                case PBEItem item: localized = PBELocalizedString.GetItemName(item); break;
                case PBEMove move: localized = PBELocalizedString.GetMoveName(move); break;
                case PBESpecies species: localized = PBELocalizedString.GetSpeciesName(species); break;
            }
            return StringRendering.RenderString(localized == null ? value?.ToString() : localized.FromUICultureInfo(), parameter?.ToString());
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
