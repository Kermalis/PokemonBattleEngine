using Kermalis.EndianBinaryIO;
using Kermalis.PokemonBattleEngine.Data.Utils;
using Kermalis.PokemonBattleEngine.Utils;
using Newtonsoft.Json.Linq;
using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace Kermalis.PokemonBattleEngine.Data.Legality
{
    public sealed class PBELegalPokemon : IPBEPokemon, INotifyPropertyChanged
    {
        private void OnPropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }
        public event PropertyChangedEventHandler? PropertyChanged;

        public PBESettings Settings { get; }

        private IPBEPokemonData _pData;
        public PBEAlphabeticalList<PBEAbility> SelectableAbilities { get; } = new();
        public PBEAlphabeticalList<PBEForm> SelectableForms { get; } = new();
        public PBEAlphabeticalList<PBEGender> SelectableGenders { get; } = new();
        public PBEAlphabeticalList<PBEItem> SelectableItems { get; } = new();

        public bool PBEIgnore => false;

        private PBESpecies _species;
        public PBESpecies Species
        {
            get => _species;
            set
            {
                if (_species != value)
                {
                    PBEDataUtils.ValidateSpecies(value, 0, true);
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
                    PBEDataUtils.ValidateSpecies(_species, value, true);
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
                    PBEDataUtils.ValidateNickname(value, Settings);
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
                    PBEDataUtils.ValidateLevel(value, Settings);
                    _level = value;
                    OnPropertyChanged(nameof(Level));
                    Moveset.Level = value;
                    EXP = PBEDataProvider.Instance.GetEXPRequired(_pData.GrowthRate, value);
                }
            }
        }
        private uint _exp;
        public uint EXP
        {
            get => _exp;
            set
            {
                if (_exp != value)
                {
                    _level = PBEDataProvider.Instance.GetEXPLevel(_pData.GrowthRate, value);
                    _exp = value;
                    OnPropertyChanged(nameof(EXP));
                    OnPropertyChanged(nameof(Level));
                    Moveset.Level = _level;
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
        private bool _pokerus;
        public bool Pokerus
        {
            get => _pokerus;
            set
            {
                if (value != _pokerus)
                {
                    _pokerus = value;
                    OnPropertyChanged(nameof(Pokerus));
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
                    PBEDataUtils.ValidateAbility(SelectableAbilities, value);
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
                    PBEDataUtils.ValidateNature(value);
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
                    PBEDataUtils.ValidateCaughtBall(value);
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
                    PBEDataUtils.ValidateGender(SelectableGenders, value);
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
                    PBEDataUtils.ValidateItem(SelectableItems, value);
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
            PBEDataUtils.ValidateSpecies(species, form, true);
            _species = species;
            _form = form;
            SetSelectable();
            string nickname = r.ReadStringNullTerminated();
            PBEDataUtils.ValidateNickname(nickname, Settings);
            _nickname = nickname;
            byte level = r.ReadByte();
            PBEDataUtils.ValidateLevel(level, Settings);
            _level = level;
            uint exp = r.ReadUInt32();
            PBEDataUtils.ValidateEXP(_pData!.GrowthRate, exp, level);
            _exp = exp;
            _friendship = r.ReadByte();
            _shiny = r.ReadBoolean();
            _pokerus = r.ReadBoolean();
            PBEAbility ability = r.ReadEnum<PBEAbility>();
            PBEDataUtils.ValidateAbility(SelectableAbilities, ability);
            _ability = ability;
            PBENature nature = r.ReadEnum<PBENature>();
            PBEDataUtils.ValidateNature(nature);
            _nature = nature;
            PBEItem caughtBall = r.ReadEnum<PBEItem>();
            PBEDataUtils.ValidateCaughtBall(caughtBall);
            _caughtBall = caughtBall;
            PBEGender gender = r.ReadEnum<PBEGender>();
            PBEDataUtils.ValidateGender(SelectableGenders, gender);
            _gender = gender;
            PBEItem item = r.ReadEnum<PBEItem>();
            PBEDataUtils.ValidateItem(SelectableItems, item);
            _item = item;
            EffortValues = new PBELegalEffortValues(Settings, r);
            IndividualValues = new PBELegalIndividualValues(Settings, r);
            Moveset = new PBELegalMoveset(species, form, level, Settings, new PBEReadOnlyMoveset(r));
        }
        internal PBELegalPokemon(PBESettings settings, JToken jToken)
        {
            Settings = settings;
            _friendship = jToken.GetSafe(nameof(Friendship)).Value<byte>();
            _shiny = jToken.GetSafe(nameof(Shiny)).Value<bool>();
            _pokerus = jToken.GetSafe(nameof(Pokerus)).Value<bool>();
            byte level = jToken.GetSafe(nameof(Level)).Value<byte>();
            PBEDataUtils.ValidateLevel(level, Settings);
            _level = level;
            string nickname = jToken.GetSafeString(nameof(Nickname));
            PBEDataUtils.ValidateNickname(nickname, Settings);
            _nickname = nickname;
            if (!PBEDataProvider.Instance.GetNatureByName(jToken.GetSafeString(nameof(Nature)), out PBENature? nature))
            {
                throw new InvalidDataException("Invalid nature");
            }
            PBEDataUtils.ValidateNature(nature.Value);
            _nature = nature.Value;
            if (!PBEDataProvider.Instance.GetItemByName(jToken.GetSafeString(nameof(CaughtBall)), out PBEItem? caughtBall))
            {
                throw new InvalidDataException("Invalid caught ball");
            }
            PBEDataUtils.ValidateCaughtBall(caughtBall.Value);
            _caughtBall = caughtBall.Value;
            if (!PBEDataProvider.Instance.GetSpeciesByName(jToken.GetSafeString(nameof(Species)), out PBESpecies? species))
            {
                throw new InvalidDataException("Invalid species");
            }
            PBEForm form;
            if (PBEDataUtils.HasForms(species.Value, true))
            {
                form = Enum.Parse<PBEForm>(jToken.GetSafeString(nameof(Form)));
            }
            else
            {
                form = 0;
            }
            PBEDataUtils.ValidateSpecies(species.Value, form, true);
            _species = species.Value;
            _form = form;
            SetSelectable();
            uint exp = jToken.GetSafe(nameof(EXP)).Value<uint>();
            PBEDataUtils.ValidateEXP(_pData.GrowthRate, exp, level);
            _exp = exp;
            if (!PBEDataProvider.Instance.GetAbilityByName(jToken.GetSafeString(nameof(Ability)), out PBEAbility? ability))
            {
                throw new InvalidDataException("Invalid ability");
            }
            PBEDataUtils.ValidateAbility(SelectableAbilities, ability.Value);
            _ability = ability.Value;
            if (!PBEDataProvider.Instance.GetGenderByName(jToken.GetSafeString(nameof(Gender)), out PBEGender? gender))
            {
                throw new InvalidDataException("Invalid gender");
            }
            PBEDataUtils.ValidateGender(SelectableGenders, gender.Value);
            _gender = gender.Value;
            if (!PBEDataProvider.Instance.GetItemByName(jToken.GetSafeString(nameof(Item)), out PBEItem? item))
            {
                throw new InvalidDataException("Invalid item");
            }
            PBEDataUtils.ValidateItem(SelectableItems, item.Value);
            _item = item.Value;
            EffortValues = new PBELegalEffortValues(Settings, jToken.GetSafe(nameof(EffortValues)));
            IndividualValues = new PBELegalIndividualValues(Settings, jToken.GetSafe(nameof(IndividualValues)));
            Moveset = new PBELegalMoveset(_species, form, level, Settings, new PBEReadOnlyMoveset((JArray)jToken.GetSafe(nameof(Moveset))));
        }
        public PBELegalPokemon(PBESpecies species, PBEForm form, byte level, uint exp, PBESettings settings)
        {
            settings.ShouldBeReadOnly(nameof(settings));
            PBEDataUtils.ValidateSpecies(species, form, true);
            PBEDataUtils.ValidateLevel(level, settings);
            PBEDataUtils.ValidateEXP(PBEDataProvider.Instance.GetPokemonData(species, form).GrowthRate, exp, level);
            Settings = settings;
            _species = species;
            _form = form;
            _level = level;
            _exp = exp;
            _friendship = (byte)PBEDataProvider.GlobalRandom.RandomInt(0, byte.MaxValue);
            _shiny = PBEDataProvider.GlobalRandom.RandomShiny();
            _nature = PBEDataProvider.GlobalRandom.RandomElement(PBEDataUtils.AllNatures);
            _caughtBall = PBEDataProvider.GlobalRandom.RandomElement(PBEDataUtils.AllBalls);
            EffortValues = new PBELegalEffortValues(Settings, true);
            IndividualValues = new PBELegalIndividualValues(Settings, true);
            Moveset = new PBELegalMoveset(_species, _form, _level, Settings, true);
            SetSelectable();
            _nickname = GetTruncatedNickname();
            UpdateAbility();
            UpdateGender();
            UpdateItem();
        }
        [MemberNotNull(nameof(_pData))]
        private void SetSelectable()
        {
            _pData = PBEDataProvider.Instance.GetPokemonData(_species, _form);
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
                Ability = PBEDataProvider.GlobalRandom.RandomElement(SelectableAbilities);
            }
            if (!SelectableItems.Contains(_item))
            {
                Item = PBEDataProvider.GlobalRandom.RandomElement(SelectableItems);
            }
            Moveset.Form = _form;
        }
        private string GetTruncatedNickname()
        {
            string newNickname = PBEDataProvider.Instance.GetSpeciesName(_species).FromGlobalLanguage();
            if (newNickname.Length > Settings.MaxPokemonNameLength)
            {
                newNickname = newNickname.Substring(0, Settings.MaxPokemonNameLength);
            }
            return newNickname;
        }
        private void UpdateAbility()
        {
            Ability = PBEDataProvider.GlobalRandom.RandomElement(SelectableAbilities);
        }
        private void UpdateGender()
        {
            Gender = PBEDataProvider.GlobalRandom.RandomGender(_pData.GenderRatio);
        }
        private void UpdateItem()
        {
            Item = PBEDataProvider.GlobalRandom.RandomElement(SelectableItems);
        }
        private void OnSpeciesChanged(PBESpecies oldSpecies)
        {
            SetSelectable();
            if (_nickname == PBEDataProvider.Instance.GetSpeciesName(oldSpecies).FromGlobalLanguage())
            {
                Nickname = GetTruncatedNickname();
            }
            if (!SelectableAbilities.Contains(_ability))
            {
                UpdateAbility();
            }
            if (!SelectableGenders.Contains(_gender))
            {
                UpdateGender();
            }
            if (!SelectableItems.Contains(_item))
            {
                UpdateItem();
            }
            Moveset.Species = _species;
            Moveset.Form = _form;
        }
    }
}
