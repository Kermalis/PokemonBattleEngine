using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;

namespace Kermalis.PokemonBattleEngine.Data
{
    public sealed class PBEPokemonShell : IDisposable, INotifyPropertyChanged
    {
        public static PBEAlphabeticalList<PBESpecies> AllSpecies { get; } = new PBEAlphabeticalList<PBESpecies>(
            Enum.GetValues(typeof(PBESpecies))
            .Cast<PBESpecies>()
            .Except(new[] { PBESpecies.Castform_Rainy, PBESpecies.Castform_Snowy, PBESpecies.Castform_Sunny, PBESpecies.Cherrim_Sunshine, PBESpecies.Darmanitan_Zen, PBESpecies.Meloetta_Pirouette })
            );
        public static PBEAlphabeticalList<PBESpecies> AllSpeciesBaseForm { get; } = new PBEAlphabeticalList<PBESpecies>(
            Enum.GetValues(typeof(PBESpecies))
            .Cast<PBESpecies>()
            .Where(s => ((uint)s >> 0x10) == 0)
            );
        public static PBEAlphabeticalList<PBENature> AllNatures { get; } = new PBEAlphabeticalList<PBENature>(
            Enum.GetValues(typeof(PBENature))
            .Cast<PBENature>()
            .Except(new[] { PBENature.MAX })
            );
        public static PBEAlphabeticalList<PBEItem> AllItems { get; } = new PBEAlphabeticalList<PBEItem>(
            Enum.GetValues(typeof(PBEItem))
            .Cast<PBEItem>()
            );

        private void OnPropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }
        public event PropertyChangedEventHandler PropertyChanged;

        public PBESettings Settings { get; }

        private PBEPokemonData _pData;
        public PBEAlphabeticalList<PBEAbility> SelectableAbilities { get; } = new PBEAlphabeticalList<PBEAbility>();
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
                    ValidateSpecies(value);
                    PBESpecies old = _species;
                    _species = value;
                    OnPropertyChanged(nameof(Species));
                    OnSpeciesChanged(old);
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

        internal PBEPokemonShell(PBESettings settings, BinaryReader r)
        {
            Settings = settings;
            var species = (PBESpecies)r.ReadUInt32();
            ValidateSpecies(species);
            _species = species;
            SetSelectable();
            string nickname = PBEUtils.StringFromBytes(r);
            ValidateNickname(nickname, Settings);
            _nickname = nickname;
            byte level = r.ReadByte();
            ValidateLevel(level, Settings);
            _level = level;
            _friendship = r.ReadByte();
            _shiny = r.ReadBoolean();
            var ability = (PBEAbility)r.ReadByte();
            ValidateAbility(ability);
            _ability = ability;
            var nature = (PBENature)r.ReadByte();
            ValidateNature(nature);
            _nature = nature;
            var gender = (PBEGender)r.ReadByte();
            ValidateGender(gender);
            _gender = gender;
            var item = (PBEItem)r.ReadUInt16();
            ValidateItem(item);
            _item = item;
            Settings.PropertyChanged += OnSettingsChanged;
            EffortValues = new PBEEffortValues(Settings, r);
            IndividualValues = new PBEIndividualValues(Settings, r);
            Moveset = new PBEMoveset(species, level, Settings, r);
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
            ValidateSpecies(species);
            _species = species;
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
            Moveset = new PBEMoveset(species, level, Settings, (JArray)jToken[nameof(Moveset)]);
        }
        public PBEPokemonShell(PBESpecies species, byte level, PBESettings settings)
        {
            ValidateLevel(level, settings);
            ValidateSpecies(species);
            Settings = settings;
            Settings.PropertyChanged += OnSettingsChanged;
            _canDispose = true;
            _species = species;
            _level = level;
            _friendship = (byte)PBEUtils.RandomInt(0, byte.MaxValue);
            _shiny = PBEUtils.RandomShiny();
            _nature = AllNatures.RandomElement();
            EffortValues = new PBEEffortValues(Settings, true) { CanDispose = false };
            IndividualValues = new PBEIndividualValues(Settings, true) { CanDispose = false };
            Moveset = new PBEMoveset(_species, _level, Settings, true) { CanDispose = false };
            OnSpeciesChanged(0);
        }
        private void SetSelectable()
        {
            _pData = PBEPokemonData.GetData(_species);
            SelectableAbilities.Reset(_pData.Abilities);
            PBEGender[] selectableGenders;
            switch (_pData.GenderRatio)
            {
                case PBEGenderRatio.M0_F0: selectableGenders = new[] { PBEGender.Genderless }; break;
                case PBEGenderRatio.M1_F0: selectableGenders = new[] { PBEGender.Male }; break;
                case PBEGenderRatio.M0_F1: selectableGenders = new[] { PBEGender.Female }; break;
                default: selectableGenders = new[] { PBEGender.Female, PBEGender.Male }; break;
            }
            SelectableGenders.Reset(selectableGenders);
            IEnumerable<PBEItem> selectableItems;
            switch (_species)
            {
                case PBESpecies.Giratina: selectableItems = AllItems.Except(new[] { PBEItem.GriseousOrb }); break;
                case PBESpecies.Giratina_Origin: selectableItems = new[] { PBEItem.GriseousOrb }; break;
                case PBESpecies.Arceus:
                {
                    selectableItems = AllItems.Except(new[] { PBEItem.DracoPlate, PBEItem.DreadPlate, PBEItem.EarthPlate, PBEItem.FistPlate,
                                PBEItem.FlamePlate, PBEItem.IciclePlate, PBEItem.InsectPlate, PBEItem.IronPlate, PBEItem.MeadowPlate, PBEItem.MindPlate, PBEItem.SkyPlate,
                                PBEItem.SplashPlate, PBEItem.SpookyPlate, PBEItem.StonePlate, PBEItem.ToxicPlate, PBEItem.ZapPlate });
                    break;
                }
                case PBESpecies.Arceus_Bug: selectableItems = new[] { PBEItem.InsectPlate }; break;
                case PBESpecies.Arceus_Dark: selectableItems = new[] { PBEItem.DreadPlate }; break;
                case PBESpecies.Arceus_Dragon: selectableItems = new[] { PBEItem.DracoPlate }; break;
                case PBESpecies.Arceus_Electric: selectableItems = new[] { PBEItem.ZapPlate }; break;
                case PBESpecies.Arceus_Fighting: selectableItems = new[] { PBEItem.FistPlate }; break;
                case PBESpecies.Arceus_Fire: selectableItems = new[] { PBEItem.FlamePlate }; break;
                case PBESpecies.Arceus_Flying: selectableItems = new[] { PBEItem.SkyPlate }; break;
                case PBESpecies.Arceus_Ghost: selectableItems = new[] { PBEItem.SpookyPlate }; break;
                case PBESpecies.Arceus_Grass: selectableItems = new[] { PBEItem.MeadowPlate }; break;
                case PBESpecies.Arceus_Ground: selectableItems = new[] { PBEItem.EarthPlate }; break;
                case PBESpecies.Arceus_Ice: selectableItems = new[] { PBEItem.IciclePlate }; break;
                case PBESpecies.Arceus_Poison: selectableItems = new[] { PBEItem.ToxicPlate }; break;
                case PBESpecies.Arceus_Psychic: selectableItems = new[] { PBEItem.MindPlate }; break;
                case PBESpecies.Arceus_Rock: selectableItems = new[] { PBEItem.StonePlate }; break;
                case PBESpecies.Arceus_Steel: selectableItems = new[] { PBEItem.IronPlate }; break;
                case PBESpecies.Arceus_Water: selectableItems = new[] { PBEItem.SplashPlate }; break;
                case PBESpecies.Genesect: selectableItems = AllItems.Except(new[] { PBEItem.BurnDrive, PBEItem.ChillDrive, PBEItem.DouseDrive, PBEItem.ShockDrive }); break;
                case PBESpecies.Genesect_Burn: selectableItems = new[] { PBEItem.BurnDrive }; break;
                case PBESpecies.Genesect_Chill: selectableItems = new[] { PBEItem.ChillDrive }; break;
                case PBESpecies.Genesect_Douse: selectableItems = new[] { PBEItem.DouseDrive }; break;
                case PBESpecies.Genesect_Shock: selectableItems = new[] { PBEItem.ShockDrive }; break;
                default: selectableItems = AllItems; break;
            }
            SelectableItems.Reset(selectableItems);
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
                Gender = PBEUtils.RandomGender(_pData.GenderRatio);
            }
            if (oldSpecies == 0 || !SelectableItems.Contains(_item))
            {
                Item = SelectableItems.RandomElement();
            }
            if (oldSpecies != 0)
            {
                Moveset.Species = _species;
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

        internal static void ValidateSpecies(PBESpecies value)
        {
            if (!AllSpecies.Contains(value))
            {
                throw new ArgumentOutOfRangeException(nameof(value));
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
            if (!AllNatures.Contains(value))
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

        internal void ToBytes(List<byte> bytes)
        {
            bytes.AddRange(BitConverter.GetBytes((uint)_species));
            PBEUtils.StringToBytes(bytes, _nickname);
            bytes.Add(_level);
            bytes.Add(_friendship);
            bytes.Add((byte)(_shiny ? 1 : 0));
            bytes.Add((byte)_ability);
            bytes.Add((byte)_nature);
            bytes.Add((byte)_gender);
            bytes.AddRange(BitConverter.GetBytes((ushort)_item));
            EffortValues.ToBytes(bytes);
            IndividualValues.ToBytes(bytes);
            Moveset.ToBytes(bytes);
        }
        internal void ToJson(JsonTextWriter w)
        {
            w.WriteStartObject();
            w.WritePropertyName(nameof(Species));
            w.WriteValue(_species.ToString());
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
