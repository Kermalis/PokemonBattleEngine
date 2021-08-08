using Avalonia;
using Avalonia.Data.Converters;
using Kermalis.PokemonBattleEngine.Battle;
using Kermalis.PokemonBattleEngine.Data;
using Kermalis.PokemonBattleEngine.Data.Utils;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Kermalis.PokemonBattleEngineClient.Infrastructure
{
    public sealed class FormToTextBitmapConverter : IMultiValueConverter
    {
        public static FormToTextBitmapConverter Instance { get; } = new();
        public object? Convert(IList<object> values, Type? targetType, object? parameter, CultureInfo? culture)
        {
            var species = (PBESpecies)values[0];
            if (!PBEDataUtils.HasForms(species, true))
            {
                return AvaloniaProperty.UnsetValue;
            }
            PBEForm form = true ? 0 : (PBEForm)values[1]; // TODO
            IPBEReadOnlyLocalizedString localized = PBEDataProvider.Instance.GetFormName(species, form);
            return StringRenderer.Render(localized.FromGlobalLanguage(), parameter?.ToString());
        }
    }
    public sealed class ObjectToTextBitmapConverter : IValueConverter
    {
        public static ObjectToTextBitmapConverter Instance { get; } = new();
        public object? Convert(object? value, Type? targetType, object? parameter, CultureInfo? culture)
        {
            if (value is null)
            {
                return AvaloniaProperty.UnsetValue;
            }
            IPBEReadOnlyLocalizedString? localized = null;
            switch (value)
            {
                case PBEAbility ability: localized = PBEDataProvider.Instance.GetAbilityName(ability); break;
                case PBEGender gender: localized = PBEDataProvider.Instance.GetGenderName(gender); break;
                case PBEItem item: localized = PBEDataProvider.Instance.GetItemName(item); break;
                case IPBEReadOnlyLocalizedString l: localized = l; break;
                case PBEMove move: localized = PBEDataProvider.Instance.GetMoveName(move); break;
                case PBENature nature: localized = PBEDataProvider.Instance.GetNatureName(nature); break;
                case PBESpecies species: localized = PBEDataProvider.Instance.GetSpeciesName(species); break;
                case PBEStat stat: localized = PBEDataProvider.Instance.GetStatName(stat); break;
                case PBEType type: localized = PBEDataProvider.Instance.GetTypeName(type); break;
            }
            return StringRenderer.Render(localized is null ? value.ToString() : localized.FromGlobalLanguage(), parameter?.ToString()) ?? AvaloniaProperty.UnsetValue;
        }
        public object? ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    public sealed class SpeciesToMinispriteConverter : IValueConverter
    {
        public static SpeciesToMinispriteConverter Instance { get; } = new();
        public object? Convert(object? value, Type? targetType, object? parameter, CultureInfo? culture)
        {
            PBESpecies species;
            PBEForm form;
            PBEGender gender;
            bool shiny;
            if (value is PBESpecies s)
            {
                species = s;
                form = 0;
                gender = PBEGender.Male;
                shiny = false;
            }
            else if (value is IPBEPokemon pkmn)
            {
                species = pkmn.Species;
                form = pkmn.Form;
                gender = pkmn.Gender;
                shiny = pkmn.Shiny;
            }
            else if (value is PBEBattlePokemon bpkmn)
            {
                bool useKnownInfo = parameter is bool b && b;
                species = useKnownInfo ? bpkmn.KnownSpecies : bpkmn.OriginalSpecies;
                form = useKnownInfo ? bpkmn.KnownForm : bpkmn.RevertForm;
                gender = useKnownInfo ? bpkmn.KnownGender : bpkmn.Gender;
                shiny = useKnownInfo ? bpkmn.KnownShiny : bpkmn.Shiny;
            }
            else
            {
                throw new ArgumentOutOfRangeException(nameof(value));
            }
            return Utils.GetMinispriteBitmap(species, form, gender, shiny);
        }
        public object? ConvertBack(object? value, Type? targetType, object? parameter, CultureInfo? culture)
        {
            throw new NotImplementedException();
        }
    }
}
