using Kermalis.EndianBinaryIO;
using Kermalis.PokemonBattleEngine.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.ComponentModel;

namespace Kermalis.PokemonBattleEngine.Data
{
    public sealed class PBEPokemonShell : IDisposable, INotifyPropertyChanged
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
                    ValidateSpecies(value, 0);
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
                    ValidateSpecies(_species, value);
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
                    ValidateNickname(value, Settings);
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
                    ValidateLevel(value, Settings);
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
                    ValidateAbility(value);
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
                    ValidateNature(value);
                    _nature = value;
                    OnPropertyChanged(nameof(Nature));
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
                    ValidateGender(value);
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
                    ValidateItem(value);
                    _item = value;
                    OnPropertyChanged(nameof(Item));
                }
            }
        }
        public PBEEffortValues EffortValues { get; }
        public PBEIndividualValues IndividualValues { get; }
        public PBEMoveset Moveset { get; }

        internal PBEPokemonShell(PBESettings settings, EndianBinaryReader r)
        {
            Settings = settings;
            PBESpecies species = r.ReadEnum<PBESpecies>();
            PBEForm form = r.ReadEnum<PBEForm>();
            ValidateSpecies(species, form);
            _species = species;
            _form = form;
            SetSelectable();
            string nickname = r.ReadStringNullTerminated();
            ValidateNickname(nickname, Settings);
            _nickname = nickname;
            byte level = r.ReadByte();
            ValidateLevel(level, Settings);
            _level = level;
            _friendship = r.ReadByte();
            _shiny = r.ReadBoolean();
            PBEAbility ability = r.ReadEnum<PBEAbility>();
            ValidateAbility(ability);
            _ability = ability;
            PBENature nature = r.ReadEnum<PBENature>();
            ValidateNature(nature);
            _nature = nature;
            PBEGender gender = r.ReadEnum<PBEGender>();
            ValidateGender(gender);
            _gender = gender;
            PBEItem item = r.ReadEnum<PBEItem>();
            ValidateItem(item);
            _item = item;
            Settings.PropertyChanged += OnSettingsChanged;
            EffortValues = new PBEEffortValues(Settings, r);
            IndividualValues = new PBEIndividualValues(Settings, r);
            Moveset = new PBEMoveset(species, form, level, Settings, r);
        }
        internal PBEPokemonShell(PBESettings settings, JToken jToken)
        {
            Settings = settings;
            _friendship = jToken[nameof(Friendship)].Value<byte>();
            _shiny = jToken[nameof(Shiny)].Value<bool>();
            byte level = jToken[nameof(Level)].Value<byte>();
            ValidateLevel(level, Settings);
            _level = level;
            string nickname = jToken[nameof(Nickname)].Value<string>();
            ValidateNickname(nickname, Settings);
            _nickname = nickname;
            PBENature nature = PBELocalizedString.GetNatureByName(jToken[nameof(Nature)].Value<string>()).Value;
            ValidateNature(nature);
            _nature = nature;
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
            ValidateSpecies(species, form);
            _species = species;
            _form = form;
            SetSelectable();
            PBEAbility ability = PBELocalizedString.GetAbilityByName(jToken[nameof(Ability)].Value<string>()).Value;
            ValidateAbility(ability);
            _ability = ability;
            PBEGender gender = PBELocalizedString.GetGenderByName(jToken[nameof(Gender)].Value<string>()).Value;
            ValidateGender(gender);
            _gender = gender;
            PBEItem item = PBELocalizedString.GetItemByName(jToken[nameof(Item)].Value<string>()).Value;
            ValidateItem(item);
            _item = item;
            Settings.PropertyChanged += OnSettingsChanged;
            EffortValues = new PBEEffortValues(Settings, jToken[nameof(EffortValues)]);
            IndividualValues = new PBEIndividualValues(Settings, jToken[nameof(IndividualValues)]);
            Moveset = new PBEMoveset(species, form, level, Settings, (JArray)jToken[nameof(Moveset)]);
        }
        public PBEPokemonShell(PBESpecies species, PBEForm form, byte level, PBESettings settings)
        {
            ValidateSpecies(species, form);
            ValidateLevel(level, settings);
            Settings = settings;
            Settings.PropertyChanged += OnSettingsChanged;
            _canDispose = true;
            _species = species;
            _form = form;
            _level = level;
            _friendship = (byte)PBERandom.RandomInt(0, byte.MaxValue);
            _shiny = PBERandom.RandomShiny();
            _nature = PBEDataUtils.AllNatures.RandomElement();
            EffortValues = new PBEEffortValues(Settings, true) { CanDispose = false };
            IndividualValues = new PBEIndividualValues(Settings, true) { CanDispose = false };
            Moveset = new PBEMoveset(_species, _form, _level, Settings, true) { CanDispose = false };
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
                Ability = SelectableAbilities.RandomElement();
            }
            if (!SelectableItems.Contains(_item))
            {
                Item = SelectableItems.RandomElement();
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
                Ability = SelectableAbilities.RandomElement();
            }
            if (oldSpecies == 0 || !SelectableGenders.Contains(_gender))
            {
                Gender = PBERandom.RandomGender(_pData.GenderRatio);
            }
            if (oldSpecies == 0 || !SelectableItems.Contains(_item))
            {
                Item = SelectableItems.RandomElement();
            }
            if (oldSpecies != 0)
            {
                Moveset.Species = _species;
                Moveset.Form = _form;
            }
        }

        private void OnSettingsChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(Settings.MaxLevel):
                {
                    if (_level > Settings.MaxLevel)
                    {
                        Level = Settings.MaxLevel;
                    }
                    break;
                }
                case nameof(Settings.MaxPokemonNameLength):
                {
                    if (_nickname.Length > Settings.MaxPokemonNameLength)
                    {
                        Nickname = _nickname.Substring(0, Settings.MaxPokemonNameLength);
                    }
                    break;
                }
                case nameof(Settings.MinLevel):
                {
                    if (_level < Settings.MinLevel)
                    {
                        Level = Settings.MinLevel;
                    }
                    break;
                }
            }
        }

        internal static void ValidateSpecies(PBESpecies species, PBEForm form)
        {
            if (!PBEDataUtils.IsValidForm(species, form, true))
            {
                throw new ArgumentOutOfRangeException(nameof(form));
            }
        }
        internal static void ValidateNickname(string value, PBESettings settings)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentOutOfRangeException(nameof(value));
            }
            if (value.Length > settings.MaxPokemonNameLength)
            {
                throw new ArgumentOutOfRangeException(nameof(value), $"{nameof(Nickname)} cannot have more than {nameof(settings.MaxPokemonNameLength)} ({settings.MaxPokemonNameLength}) characters.");
            }
        }
        internal static void ValidateLevel(byte value, PBESettings settings)
        {
            if (value < settings.MinLevel || value > settings.MaxLevel)
            {
                throw new ArgumentOutOfRangeException(nameof(value), $"{nameof(Level)} must be at least {nameof(settings.MinLevel)} ({settings.MinLevel}) and cannot exceed {nameof(settings.MaxLevel)} ({settings.MaxLevel}).");
            }
        }
        private void ValidateAbility(PBEAbility value)
        {
            if (!SelectableAbilities.Contains(value))
            {
                throw new ArgumentOutOfRangeException(nameof(value));
            }
        }
        internal static void ValidateNature(PBENature value)
        {
            if (!PBEDataUtils.AllNatures.Contains(value))
            {
                throw new ArgumentOutOfRangeException(nameof(value));
            }
        }
        private void ValidateGender(PBEGender value)
        {
            if (!SelectableGenders.Contains(value))
            {
                throw new ArgumentOutOfRangeException(nameof(value));
            }
        }
        private void ValidateItem(PBEItem value)
        {
            if (!SelectableItems.Contains(value))
            {
                throw new ArgumentOutOfRangeException(nameof(value));
            }
        }

        internal void ToBytes(EndianBinaryWriter w)
        {
            w.Write(_species);
            w.Write(_form);
            w.Write(_nickname, true);
            w.Write(_level);
            w.Write(_friendship);
            w.Write(_shiny);
            w.Write(_ability);
            w.Write(_nature);
            w.Write(_gender);
            w.Write(_item);
            EffortValues.ToBytes(w);
            IndividualValues.ToBytes(w);
            Moveset.ToBytes(w);
        }
        internal void ToJson(JsonTextWriter w)
        {
            w.WriteStartObject();
            w.WritePropertyName(nameof(Species));
            w.WriteValue(_species.ToString());
            if (PBEDataUtils.HasForms(_species, true))
            {
                w.WritePropertyName(nameof(Form));
                w.WriteValue(PBEDataUtils.GetNameOfForm(_species, _form));
            }
            w.WritePropertyName(nameof(Nickname));
            w.WriteValue(_nickname);
            w.WritePropertyName(nameof(Level));
            w.WriteValue(_level);
            w.WritePropertyName(nameof(Friendship));
            w.WriteValue(_friendship);
            w.WritePropertyName(nameof(Shiny));
            w.WriteValue(_shiny);
            w.WritePropertyName(nameof(Ability));
            w.WriteValue(_ability.ToString());
            w.WritePropertyName(nameof(Nature));
            w.WriteValue(_nature.ToString());
            w.WritePropertyName(nameof(Gender));
            w.WriteValue(_gender.ToString());
            w.WritePropertyName(nameof(Item));
            w.WriteValue(_item.ToString());
            w.WritePropertyName(nameof(EffortValues));
            EffortValues.ToJson(w);
            w.WritePropertyName(nameof(IndividualValues));
            IndividualValues.ToJson(w);
            w.WritePropertyName(nameof(Moveset));
            Moveset.ToJson(w);
            w.WriteEndObject();
        }

        private bool _canDispose;
        public bool CanDispose
        {
            get => _canDispose;
            set
            {
                if (_canDispose != value)
                {
                    _canDispose = value;
                    OnPropertyChanged(nameof(CanDispose));
                }
            }
        }
        public bool IsDisposed { get; private set; }
        public void Dispose()
        {
            if (!_canDispose)
            {
                throw new InvalidOperationException();
            }
            if (!IsDisposed)
            {
                IsDisposed = true;
                OnPropertyChanged(nameof(IsDisposed));
                Settings.PropertyChanged -= OnSettingsChanged;
                SelectableAbilities.Dispose();
                SelectableGenders.Dispose();
                SelectableItems.Dispose();
                EffortValues.CanDispose = true;
                EffortValues.Dispose();
                IndividualValues.CanDispose = true;
                IndividualValues.Dispose();
                Moveset.CanDispose = true;
                Moveset.Dispose();
            }
        }
    }
}
