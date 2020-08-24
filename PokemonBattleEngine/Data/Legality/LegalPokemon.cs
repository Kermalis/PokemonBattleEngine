using Kermalis.EndianBinaryIO;
using Kermalis.PokemonBattleEngine.Utils;
using Newtonsoft.Json.Linq;
using System;
using System.ComponentModel;

namespace Kermalis.PokemonBattleEngine.Data.Legality
{
    public sealed class PBELegalPokemon : IPBEPokemon, INotifyPropertyChanged
    {
        private void OnPropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }
        public event PropertyChangedEventHandler PropertyChanged;

        public PBESettings Settings { get; }

        private PBEPokemonData _pData;
        public PBEAlphabeticalList<PBEAbility> SelectableAbilities { get; } = new PBEAlphabeticalList<PBEAbility>();
        public PBEAlphabeticalList<PBEForm> SelectableForms { get; } = new PBEAlphabeticalList<PBEForm>();
        public PBEAlphabeticalList<PBEGender> SelectableGenders { get; } = new PBEAlphabeticalList<PBEGender>();
        public PBEAlphabeticalList<PBEItem> SelectableItems { get; } = new PBEAlphabeticalList<PBEItem>();

        private PBESpecies _species;
        public PBESpecies Species
        {
            get => _species;
            set
            {
                if (_species != value)
                {
                    PBELegalityChecker.ValidateSpecies(value, 0, true);
                    PBESpecies oldSpecies = _species;
                    _species = value;
                    _form = 0;
                    OnPropertyChanged(nameof(Species));
                    OnSpeciesChanged(oldSpecies);
                    OnPropertyChanged(nameof(Form));
                }
            }
        }
        private PBEForm _form;
        public PBEForm Form
        {
            get => _form;
            set
            {
                if (_form != value)
                {
                    PBELegalityChecker.ValidateSpecies(_species, value, true);
                    _form = value;
                    OnPropertyChanged(nameof(Form));
                    OnFormChanged();
                }
            }
        }
        private string _nickname;
        public string Nickname
        {
            get => _nickname;
            set
            {
                if (_nickname != value)
                {
                    PBELegalityChecker.ValidateNickname(value, Settings);
                    _nickname = value;
                    OnPropertyChanged(nameof(Nickname));
                }
            }
        }
        private byte _level;
        public byte Level
        {
            get => _level;
            set
            {
                if (_level != value)
                {
                    PBELegalityChecker.ValidateLevel(value, Settings);
                    _level = value;
                    OnPropertyChanged(nameof(Level));
                    Moveset.Level = value;
                }
            }
        }
        private byte _friendship;
        public byte Friendship
        {
            get => _friendship;
            set
            {
                if (value != _friendship)
                {
                    _friendship = value;
                    OnPropertyChanged(nameof(Friendship));
                }
            }
        }
        private bool _shiny;
        public bool Shiny
        {
            get => _shiny;
            set
            {
                if (value != _shiny)
                {
                    _shiny = value;
                    OnPropertyChanged(nameof(Shiny));
                }
            }
        }
        private PBEAbility _ability;
        public PBEAbility Ability
        {
            get => _ability;
            set
            {
                if (value != _ability)
                {
                    PBELegalityChecker.ValidateAbility(SelectableAbilities, value);
                    _ability = value;
                    OnPropertyChanged(nameof(Ability));
                }
            }
        }
        private PBENature _nature;
        public PBENature Nature
        {
            get => _nature;
            set
            {
                if (value != _nature)
                {
                    PBELegalityChecker.ValidateNature(value);
                    _nature = value;
                    OnPropertyChanged(nameof(Nature));
                }
            }
        }
        private PBEItem _caughtBall;
        public PBEItem CaughtBall
        {
            get => _caughtBall;
            set
            {
                if (value != _caughtBall)
                {
                    PBELegalityChecker.ValidateCaughtBall(value);
                    _caughtBall = value;
                    OnPropertyChanged(nameof(CaughtBall));
                }
            }
        }
        private PBEGender _gender;
        public PBEGender Gender
        {
            get => _gender;
            set
            {
                if (value != _gender)
                {
                    PBELegalityChecker.ValidateGender(SelectableGenders, value);
                    _gender = value;
                    OnPropertyChanged(nameof(Gender));
                }
            }
        }
        private PBEItem _item;
        public PBEItem Item
        {
            get => _item;
            set
            {
                if (value != _item)
                {
                    PBELegalityChecker.ValidateItem(SelectableItems, value);
                    _item = value;
                    OnPropertyChanged(nameof(Item));
                }
            }
        }
        public PBELegalEffortValues EffortValues { get; }
        IPBEStatCollection IPBEPokemon.EffortValues => EffortValues;
        public PBELegalIndividualValues IndividualValues { get; }
        IPBEReadOnlyStatCollection IPBEPokemon.IndividualValues => IndividualValues;
        public PBELegalMoveset Moveset { get; }
        IPBEMoveset IPBEPokemon.Moveset => Moveset;

        internal PBELegalPokemon(PBESettings settings, EndianBinaryReader r)
        {
            Settings = settings;
            PBESpecies species = r.ReadEnum<PBESpecies>();
            PBEForm form = r.ReadEnum<PBEForm>();
            PBELegalityChecker.ValidateSpecies(species, form, true);
            _species = species;
            _form = form;
            SetSelectable();
            string nickname = r.ReadStringNullTerminated();
            PBELegalityChecker.ValidateNickname(nickname, Settings);
            _nickname = nickname;
            byte level = r.ReadByte();
            PBELegalityChecker.ValidateLevel(level, Settings);
            _level = level;
            _friendship = r.ReadByte();
            _shiny = r.ReadBoolean();
            PBEAbility ability = r.ReadEnum<PBEAbility>();
            PBELegalityChecker.ValidateAbility(SelectableAbilities, ability);
            _ability = ability;
            PBENature nature = r.ReadEnum<PBENature>();
            PBELegalityChecker.ValidateNature(nature);
            _nature = nature;
            PBEItem caughtBall = r.ReadEnum<PBEItem>();
            PBELegalityChecker.ValidateCaughtBall(caughtBall);
            _caughtBall = caughtBall;
            PBEGender gender = r.ReadEnum<PBEGender>();
            PBELegalityChecker.ValidateGender(SelectableGenders, gender);
            _gender = gender;
            PBEItem item = r.ReadEnum<PBEItem>();
            PBELegalityChecker.ValidateItem(SelectableItems, item);
            _item = item;
            EffortValues = new PBELegalEffortValues(Settings, r);
            IndividualValues = new PBELegalIndividualValues(Settings, r);
            Moveset = new PBELegalMoveset(species, form, level, Settings, new PBEReadOnlyMoveset(r));
        }
        internal PBELegalPokemon(PBESettings settings, JToken jToken)
        {
            Settings = settings;
            _friendship = jToken[nameof(Friendship)].Value<byte>();
            _shiny = jToken[nameof(Shiny)].Value<bool>();
            byte level = jToken[nameof(Level)].Value<byte>();
            PBELegalityChecker.ValidateLevel(level, Settings);
            _level = level;
            string nickname = jToken[nameof(Nickname)].Value<string>();
            PBELegalityChecker.ValidateNickname(nickname, Settings);
            _nickname = nickname;
            PBENature nature = PBELocalizedString.GetNatureByName(jToken[nameof(Nature)].Value<string>()).Value;
            PBELegalityChecker.ValidateNature(nature);
            _nature = nature;
            PBEItem caughtBall = PBELocalizedString.GetItemByName(jToken[nameof(CaughtBall)].Value<string>()).Value;
            PBELegalityChecker.ValidateCaughtBall(caughtBall);
            _caughtBall = caughtBall;
            PBESpecies species = PBELocalizedString.GetSpeciesByName(jToken[nameof(Species)].Value<string>()).Value;
            PBEForm form;
            if (PBEDataUtils.HasForms(species, true))
            {
                form = (PBEForm)Enum.Parse(typeof(PBEForm), jToken[nameof(Form)].Value<string>());
            }
            else
            {
                form = 0;
            }
            PBELegalityChecker.ValidateSpecies(species, form, true);
            _species = species;
            _form = form;
            SetSelectable();
            PBEAbility ability = PBELocalizedString.GetAbilityByName(jToken[nameof(Ability)].Value<string>()).Value;
            PBELegalityChecker.ValidateAbility(SelectableAbilities, ability);
            _ability = ability;
            PBEGender gender = PBELocalizedString.GetGenderByName(jToken[nameof(Gender)].Value<string>()).Value;
            PBELegalityChecker.ValidateGender(SelectableGenders, gender);
            _gender = gender;
            PBEItem item = PBELocalizedString.GetItemByName(jToken[nameof(Item)].Value<string>()).Value;
            PBELegalityChecker.ValidateItem(SelectableItems, item);
            _item = item;
            EffortValues = new PBELegalEffortValues(Settings, jToken[nameof(EffortValues)]);
            IndividualValues = new PBELegalIndividualValues(Settings, jToken[nameof(IndividualValues)]);
            Moveset = new PBELegalMoveset(species, form, level, Settings, new PBEReadOnlyMoveset((JArray)jToken[nameof(Moveset)]));
        }
        public PBELegalPokemon(PBESpecies species, PBEForm form, byte level, PBESettings settings)
        {
            if (settings == null)
            {
                throw new ArgumentNullException(nameof(settings));
            }
            if (!settings.IsReadOnly)
            {
                throw new ArgumentException("Settings must be read-only.", nameof(settings));
            }
            PBELegalityChecker.ValidateSpecies(species, form, true);
            PBELegalityChecker.ValidateLevel(level, settings);
            Settings = settings;
            _species = species;
            _form = form;
            _level = level;
            _friendship = (byte)PBEUtils.GlobalRandom.RandomInt(0, byte.MaxValue);
            _shiny = PBEUtils.GlobalRandom.RandomShiny();
            _nature = PBEUtils.GlobalRandom.RandomElement(PBEDataUtils.AllNatures);
            _caughtBall = PBEUtils.GlobalRandom.RandomElement(PBEDataUtils.AllBalls);
            EffortValues = new PBELegalEffortValues(Settings, true);
            IndividualValues = new PBELegalIndividualValues(Settings, true);
            Moveset = new PBELegalMoveset(_species, _form, _level, Settings, true);
            OnSpeciesChanged(0);
        }
        private void SetSelectable()
        {
            _pData = PBEPokemonData.GetData(_species, _form);
            SelectableAbilities.Reset(_pData.Abilities);
            SelectableForms.Reset(PBEDataUtils.GetForms(_species, true));
            SelectableGenders.Reset(PBEDataUtils.GetValidGenders(_pData.GenderRatio));
            SelectableItems.Reset(PBEDataUtils.GetValidItems(_species, _form));
        }
        private void OnFormChanged()
        {
            SetSelectable();
            if (!SelectableAbilities.Contains(_ability))
            {
                Ability = PBEUtils.GlobalRandom.RandomElement(SelectableAbilities);
            }
            if (!SelectableItems.Contains(_item))
            {
                Item = PBEUtils.GlobalRandom.RandomElement(SelectableItems);
            }
            Moveset.Form = _form;
        }
        private void OnSpeciesChanged(PBESpecies oldSpecies)
        {
            SetSelectable();
            if (oldSpecies == 0 || _nickname == PBELocalizedString.GetSpeciesName(oldSpecies).ToString())
            {
                string newNickname = PBELocalizedString.GetSpeciesName(_species).ToString();
                if (newNickname.Length > Settings.MaxPokemonNameLength)
                {
                    newNickname = newNickname.Substring(0, Settings.MaxPokemonNameLength);
                }
                Nickname = newNickname;
            }
            if (oldSpecies == 0 || !SelectableAbilities.Contains(_ability))
            {
                Ability = PBEUtils.GlobalRandom.RandomElement(SelectableAbilities);
            }
            if (oldSpecies == 0 || !SelectableGenders.Contains(_gender))
            {
                Gender = PBEUtils.GlobalRandom.RandomGender(_pData.GenderRatio);
            }
            if (oldSpecies == 0 || !SelectableItems.Contains(_item))
            {
                Item = PBEUtils.GlobalRandom.RandomElement(SelectableItems);
            }
            if (oldSpecies != 0)
            {
                Moveset.Species = _species;
                Moveset.Form = _form;
            }
        }
    }
}
