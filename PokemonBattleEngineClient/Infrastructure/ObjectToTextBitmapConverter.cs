using Avalonia.Data.Converters;
using Kermalis.PokemonBattleEngine.Data;
using Kermalis.PokemonBattleEngine.Localization;
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
                case PBEAbility ability: localized = PBEAbilityLocalization.Names[ability]; break;
                case PBEItem item: localized = PBEItemLocalization.Names[item]; break;
                case PBEMove move: localized = PBEMoveLocalization.Names[move]; break;
                case PBESpecies species: localized = PBEPokemonLocalization.Names[(PBESpecies)((int)species & 0xFFFF)]; break;
            }
            return StringRendering.RenderString(localized == null ? value?.ToString() : localized.FromUICultureInfo(), parameter?.ToString());
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
