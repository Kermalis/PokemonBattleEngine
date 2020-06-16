using Avalonia.Data.Converters;
using Kermalis.PokemonBattleEngine.Battle;
using Kermalis.PokemonBattleEngine.Data;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Kermalis.PokemonBattleEngineClient.Infrastructure
{
    public sealed class FormToTextBitmapConverter : IMultiValueConverter
    {
        public static FormToTextBitmapConverter Instance { get; } = new FormToTextBitmapConverter();
        public object Convert(IList<object> values, Type targetType, object parameter, CultureInfo culture)
        {
            var species = (PBESpecies)values[0];
            if (!PBEDataUtils.HasForms(species, true))
            {
                return null;
            }
            else
            {
                PBEForm form = true ? 0 : (PBEForm)values[1]; // TODO
                var localized = PBELocalizedString.GetFormName(species, form);
                return StringRenderer.Render(localized.ToString(), parameter?.ToString());
            }
        }
    }
    public sealed class ObjectToTextBitmapConverter : IValueConverter
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
            return StringRenderer.Render(localized == null ? value?.ToString() : localized.ToString(), parameter?.ToString());
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    public sealed class SpeciesToMinispriteConverter : IValueConverter
    {
        public static SpeciesToMinispriteConverter Instance { get; } = new SpeciesToMinispriteConverter();
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
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
                throw new ArgumentException();
            }
            return Utils.GetMinispriteBitmap(species, form, gender, shiny);
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
