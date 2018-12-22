using Avalonia.Data.Converters;
using System;
using System.Globalization;

namespace Kermalis.PokemonBattleEngineClient.Infrastructure
{
    class ObjectToTextBitmapConverter : IValueConverter
    {
        public static ObjectToTextBitmapConverter Instance { get; } = new ObjectToTextBitmapConverter();
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Enum.TryParse(parameter?.ToString(), out Utils.StringRenderStyle style);
            return Utils.RenderString(value?.ToString(), style);
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
