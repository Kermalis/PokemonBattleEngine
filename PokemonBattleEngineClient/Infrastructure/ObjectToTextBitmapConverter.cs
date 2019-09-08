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
                case PBEGender gender: localized = PBELocalizedString.GetGenderName(gender); break;
                case PBEItem item: localized = PBELocalizedString.GetItemName(item); break;
                case PBELocalizedString l: localized = l; break;
                case PBEMove move: localized = PBELocalizedString.GetMoveName(move); break;
                case PBENature nature: localized = PBELocalizedString.GetNatureName(nature); break;
                case PBESpecies species: localized = PBELocalizedString.GetSpeciesName(species); break;
                case PBEStat stat: localized = PBELocalizedString.GetStatName(stat); break;
                case PBEType type: localized = PBELocalizedString.GetTypeName(type); break;
            }
            return StringRenderer.Render(localized == null ? value?.ToString() : localized.FromUICultureInfo(), parameter?.ToString());
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
