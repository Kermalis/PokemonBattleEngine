using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;

namespace Kermalis.PokemonBattleEngine.Data
{
    // TODO: What should happen if you continue using this after it was removed from a PBETeamShell?
    // This should be able to be used aside from team shells (which would help event pokemon be represented again)
    public sealed class PBEPokemonShell : INotifyPropertyChanged
    {
        public static ReadOnlyCollection<PBESpecies> AllSpecies { get; } = new ReadOnlyCollection<PBESpecies>(Enum.GetValues(typeof(PBESpecies)).Cast<PBESpecies>().Except(new[] { PBESpecies.Castform_Rainy, PBESpecies.Castform_Snowy, PBESpecies.Castform_Sunny, PBESpecies.Cherrim_Sunshine, PBESpecies.Darmanitan_Zen, PBESpecies.Meloetta_Pirouette }).ToArray());
        public static ReadOnlyCollection<PBENature> AllNatures { get; } = new ReadOnlyCollection<PBENature>(Enum.GetValues(typeof(PBENature)).Cast<PBENature>().Except(new[] { PBENature.MAX }).ToArray());
        public static ReadOnlyCollection<PBEItem> AllItems { get; } = new ReadOnlyCollection<PBEItem>(Enum.GetValues(typeof(PBEItem)).Cast<PBEItem>().ToArray());

        private void OnPropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }
        public event PropertyChangedEventHandler PropertyChanged;

        private readonly PBETeamShell parent;

        private PBEPokemonData pData;
        public ReadOnlyCollection<PBEAbility> SelectableAbilities { get; private set; }
        private PBEGender[] selectableGenders;
        public ReadOnlyCollection<PBEGender> SelectableGenders { get; private set; }
        private IEnumerable<PBEItem> selectableItems;
        public ReadOnlyCollection<PBEItem> SelectableItems { get; private set; }

        private PBESpecies species;
        public PBESpecies Species
        {
            get => species;
            set
            {
                if (species != value)
                {
                    ValidateSpecies(value);
                    PBESpecies old = species;
                    species = value;
                    OnPropertyChanged(nameof(Species));
                    OnSpeciesChanged(old);
                }
            }
        }
        private string nickname;
        public string Nickname
        {
            get => nickname;
            set
            {
                if (nickname != value)
                {
                    ValidateNickname(value, parent.Settings);
                    nickname = value;
                    OnPropertyChanged(nameof(Nickname));
                }
            }
        }
        private byte level;
        public byte Level
        {
            get => level;
            set
            {
                if (level != value)
                {
                    ValidateLevel(value, parent.Settings);
                    level = value;
                    OnPropertyChanged(nameof(Level));
                    Moveset.Level = value;
                }
            }
        }
        private byte friendship;
        public byte Friendship
        {
            get => friendship;
            set
            {
                if (value != friendship)
                {
                    friendship = value;
                    OnPropertyChanged(nameof(Friendship));
                }
            }
        }
        private bool shiny;
        public bool Shiny
        {
            get => shiny;
            set
            {
                if (value != shiny)
                {
                    shiny = value;
                    OnPropertyChanged(nameof(Shiny));
                }
            }
        }
        private PBEAbility ability;
        public PBEAbility Ability
        {
            get => ability;
            set
            {
                if (value != ability)
                {
                    ValidateAbility(value);
                    ability = value;
                    OnPropertyChanged(nameof(Ability));
                }
            }
        }
        private PBENature nature;
        public PBENature Nature
        {
            get => nature;
            set
            {
                if (value != nature)
                {
                    ValidateNature(value);
                    nature = value;
                    OnPropertyChanged(nameof(Nature));
                }
            }
        }
        private PBEGender gender;
        public PBEGender Gender
        {
            get => gender;
            set
            {
                if (value != gender)
                {
                    ValidateGender(value);
                    gender = value;
                    OnPropertyChanged(nameof(Gender));
                }
            }
        }
        private PBEItem item;
        public PBEItem Item
        {
            get => item;
            set
            {
                if (value != item)
                {
                    ValidateItem(value);
                    item = value;
                    OnPropertyChanged(nameof(Item));
                }
            }
        }
        public PBEEffortValueCollection EffortValues { get; private set; }
        public PBEIndividualValueCollection IndividualValues { get; private set; }
        public PBEMovesetBuilder Moveset { get; private set; }

        internal PBEPokemonShell(BinaryReader r, PBETeamShell parent)
        {
            this.parent = parent;
            var species = (PBESpecies)r.ReadUInt32();
            ValidateSpecies(species);
            this.species = species;
            SetSelectable();
            string nickname = PBEUtils.StringFromBytes(r);
            ValidateNickname(nickname, parent.Settings);
            this.nickname = nickname;
            byte level = r.ReadByte();
            ValidateLevel(level, parent.Settings);
            this.level = level;
            friendship = r.ReadByte();
            shiny = r.ReadBoolean();
            var ability = (PBEAbility)r.ReadByte();
            ValidateAbility(ability);
            this.ability = ability;
            var nature = (PBENature)r.ReadByte();
            ValidateNature(nature);
            this.nature = nature;
            var gender = (PBEGender)r.ReadByte();
            ValidateGender(gender);
            this.gender = gender;
            var item = (PBEItem)r.ReadUInt16();
            ValidateItem(item);
            this.item = item;
            byte hpEV = r.ReadByte();
            byte attackEV = r.ReadByte();
            byte defenseEV = r.ReadByte();
            byte spAttackEV = r.ReadByte();
            byte spDefenseEV = r.ReadByte();
            byte speedEV = r.ReadByte();
            if (hpEV + attackEV + defenseEV + spAttackEV + spDefenseEV + speedEV > parent.Settings.MaxTotalEVs)
            {
                throw new ArgumentOutOfRangeException(nameof(EffortValues), $"Total must not exceed \"{nameof(parent.Settings.MaxTotalEVs)}\" ({parent.Settings.MaxTotalEVs})");
            }
            EffortValues = new PBEEffortValueCollection(parent.Settings, hpEV, attackEV, defenseEV, spAttackEV, spDefenseEV, speedEV);
            void ValidateIV(byte val, string name)
            {
                if (val > parent.Settings.MaxIVs)
                {
                    throw new ArgumentOutOfRangeException(nameof(IndividualValues), $"\"{name}\" value must not exceed \"{nameof(parent.Settings.MaxIVs)}\" ({parent.Settings.MaxIVs})");
                }
            }
            byte hpIV = r.ReadByte();
            ValidateIV(hpIV, nameof(PBEStat.HP));
            byte attackIV = r.ReadByte();
            ValidateIV(attackIV, nameof(PBEStat.Attack));
            byte defenseIV = r.ReadByte();
            ValidateIV(defenseIV, nameof(PBEStat.Defense));
            byte spAttackIV = r.ReadByte();
            ValidateIV(spAttackIV, nameof(PBEStat.SpAttack));
            byte spDefenseIV = r.ReadByte();
            ValidateIV(spDefenseIV, nameof(PBEStat.SpDefense));
            byte speedIV = r.ReadByte();
            ValidateIV(speedIV, nameof(PBEStat.Speed));
            IndividualValues = new PBEIndividualValueCollection(parent.Settings, hpIV, attackIV, defenseIV, spAttackIV, spDefenseIV, speedIV);
            Moveset = new PBEMovesetBuilder(species, level, parent.Settings, false);
            for (int i = 0; i < parent.Settings.NumMoves; i++)
            {
                var move = (PBEMove)r.ReadUInt16();
                byte ppUps = r.ReadByte();
                Moveset.Set(i, move, ppUps); // "Set()" will throw its own exceptions for invalid moves and pp-ups
                                             // The following check is for the case where identical moves were stored in the same moveset, therefore forcing "Set()" to overwrite one with PBEMove.None
                PBEMovesetBuilder.PBEMoveSlot slot = Moveset.MoveSlots[i];
                if (slot.Move != move || slot.PPUps != ppUps)
                {
                    throw new InvalidDataException("Invalid moveset.");
                }
            }
        }
        internal PBEPokemonShell(JToken jObj, PBETeamShell parent)
        {
            this.parent = parent;
            friendship = jObj[nameof(Friendship)].Value<byte>();
            shiny = jObj[nameof(Shiny)].Value<bool>();
            byte level = jObj[nameof(Level)].Value<byte>();
            ValidateLevel(level, parent.Settings);
            this.level = level;
            string nickname = jObj[nameof(Nickname)].Value<string>();
            ValidateNickname(nickname, parent.Settings);
            this.nickname = nickname;
            PBENature nature = PBELocalizedString.GetNatureByName(jObj[nameof(Nature)].Value<string>()).Value;
            ValidateNature(nature);
            this.nature = nature;
            PBESpecies species = PBELocalizedString.GetSpeciesByName(jObj[nameof(Species)].Value<string>()).Value;
            ValidateSpecies(species);
            this.species = species;
            SetSelectable();
            PBEAbility ability = PBELocalizedString.GetAbilityByName(jObj[nameof(Ability)].Value<string>()).Value;
            ValidateAbility(ability);
            this.ability = ability;
            PBEGender gender = PBELocalizedString.GetGenderByName(jObj[nameof(Gender)].Value<string>()).Value;
            ValidateGender(gender);
            this.gender = gender;
            PBEItem item = PBELocalizedString.GetItemByName(jObj[nameof(Item)].Value<string>()).Value;
            ValidateItem(item);
            this.item = item;
            JToken eiObj = jObj[nameof(EffortValues)];
            EffortValues = new PBEEffortValueCollection(parent.Settings,
                eiObj[nameof(PBEStat.HP)].Value<byte>(),
                eiObj[nameof(PBEStat.Attack)].Value<byte>(),
                eiObj[nameof(PBEStat.Defense)].Value<byte>(),
                eiObj[nameof(PBEStat.SpAttack)].Value<byte>(),
                eiObj[nameof(PBEStat.SpDefense)].Value<byte>(),
                eiObj[nameof(PBEStat.Speed)].Value<byte>()
            );
            eiObj = jObj[nameof(IndividualValues)];
            IndividualValues = new PBEIndividualValueCollection(parent.Settings,
                eiObj[nameof(PBEStat.HP)].Value<byte>(),
                eiObj[nameof(PBEStat.Attack)].Value<byte>(),
                eiObj[nameof(PBEStat.Defense)].Value<byte>(),
                eiObj[nameof(PBEStat.SpAttack)].Value<byte>(),
                eiObj[nameof(PBEStat.SpDefense)].Value<byte>(),
                eiObj[nameof(PBEStat.Speed)].Value<byte>()
            );
            var mObj = (JArray)jObj[nameof(Moveset)];
            Moveset = new PBEMovesetBuilder(species, level, parent.Settings, false);
            for (int j = 0; j < parent.Settings.NumMoves; j++)
            {
                eiObj = mObj[j];
                Moveset.Set(j,
                    PBELocalizedString.GetMoveByName(eiObj[nameof(PBEMovesetBuilder.PBEMoveSlot.Move)].Value<string>()).Value,
                    eiObj[nameof(PBEMovesetBuilder.PBEMoveSlot.PPUps)].Value<byte>()
                    );
            }
        }
        internal PBEPokemonShell(PBESpecies species, byte level, PBETeamShell parent)
        {
            ValidateLevel(level, parent.Settings);
            ValidateSpecies(species);
            this.parent = parent;
            this.species = species;
            this.level = level;
            friendship = (byte)PBEUtils.RandomInt(0, byte.MaxValue);
            shiny = PBEUtils.RandomShiny();
            nature = AllNatures.RandomElement();
            EffortValues = new PBEEffortValueCollection(parent.Settings, true);
            IndividualValues = new PBEIndividualValueCollection(parent.Settings, true);
            OnSpeciesChanged(0);
        }
        private void SetSelectable()
        {
            pData = PBEPokemonData.GetData(species);
            SelectableAbilities = pData.Abilities;
            OnPropertyChanged(nameof(SelectableAbilities));
            switch (pData.GenderRatio)
            {
                case PBEGenderRatio.M0_F0: selectableGenders = new[] { PBEGender.Genderless }; break;
                case PBEGenderRatio.M1_F0: selectableGenders = new[] { PBEGender.Male }; break;
                case PBEGenderRatio.M0_F1: selectableGenders = new[] { PBEGender.Female }; break;
                default: selectableGenders = new[] { PBEGender.Female, PBEGender.Male }; break;
            }
            SelectableGenders = new ReadOnlyCollection<PBEGender>(selectableGenders);
            OnPropertyChanged(nameof(SelectableGenders));
            switch (species)
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
            SelectableItems = new ReadOnlyCollection<PBEItem>(selectableItems.ToArray());
            OnPropertyChanged(nameof(SelectableItems));
        }
        private void OnSpeciesChanged(PBESpecies oldSpecies)
        {
            SetSelectable();
            if (oldSpecies == 0 || nickname == PBELocalizedString.GetSpeciesName(oldSpecies).FromUICultureInfo())
            {
                string newNickname = PBELocalizedString.GetSpeciesName(species).FromUICultureInfo();
                if (newNickname.Length > parent.Settings.MaxPokemonNameLength)
                {
                    newNickname = newNickname.Substring(0, parent.Settings.MaxPokemonNameLength);
                }
                Nickname = newNickname;
            }
            if (oldSpecies == 0 || !SelectableAbilities.Contains(ability))
            {
                Ability = SelectableAbilities.RandomElement();
            }
            if (oldSpecies == 0 || !SelectableGenders.Contains(gender))
            {
                Gender = PBEUtils.RandomGender(pData.GenderRatio);
            }
            if (oldSpecies == 0 || !SelectableItems.Contains(item))
            {
                Item = SelectableItems.RandomElement();
            }
            if (oldSpecies == 0)
            {
                Moveset = new PBEMovesetBuilder(species, level, parent.Settings, true);
            }
            else
            {
                Moveset.Species = species;
            }
        }

        internal void OnSettingsChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(parent.Settings.MaxLevel):
                {
                    if (level > parent.Settings.MaxLevel)
                    {
                        Level = parent.Settings.MaxLevel;
                    }
                    break;
                }
                case nameof(parent.Settings.MaxPokemonNameLength):
                {
                    if (nickname.Length > parent.Settings.MaxPokemonNameLength)
                    {
                        Nickname = nickname.Substring(0, parent.Settings.MaxPokemonNameLength);
                    }
                    break;
                }
                case nameof(parent.Settings.MinLevel):
                {
                    if (level < parent.Settings.MinLevel)
                    {
                        Level = parent.Settings.MinLevel;
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
            if (!selectableItems.Contains(value))
            {
                throw new ArgumentOutOfRangeException(nameof(value));
            }
        }

        internal List<byte> ToBytes()
        {
            var bytes = new List<byte>();
            bytes.AddRange(BitConverter.GetBytes((uint)species));
            bytes.AddRange(PBEUtils.StringToBytes(nickname));
            bytes.Add(level);
            bytes.Add(friendship);
            bytes.Add((byte)(shiny ? 1 : 0));
            bytes.Add((byte)ability);
            bytes.Add((byte)nature);
            bytes.Add((byte)gender);
            bytes.AddRange(BitConverter.GetBytes((ushort)item));
            bytes.AddRange(EffortValues.Select(ev => ev.Value));
            bytes.AddRange(IndividualValues.Select(iv => iv.Value));
            foreach (PBEMovesetBuilder.PBEMoveSlot slot in Moveset.MoveSlots)
            {
                bytes.AddRange(BitConverter.GetBytes((ushort)slot.Move));
                bytes.Add(slot.PPUps);
            }
            return bytes;
        }
        internal void ToJson(JsonTextWriter w)
        {
            w.WriteStartObject();
            w.WritePropertyName(nameof(Species));
            w.WriteValue(species.ToString());
            w.WritePropertyName(nameof(Nickname));
            w.WriteValue(nickname);
            w.WritePropertyName(nameof(Level));
            w.WriteValue(level);
            w.WritePropertyName(nameof(Friendship));
            w.WriteValue(friendship);
            w.WritePropertyName(nameof(Shiny));
            w.WriteValue(shiny);
            w.WritePropertyName(nameof(Ability));
            w.WriteValue(ability.ToString());
            w.WritePropertyName(nameof(Nature));
            w.WriteValue(nature.ToString());
            w.WritePropertyName(nameof(Gender));
            w.WriteValue(gender.ToString());
            w.WritePropertyName(nameof(Item));
            w.WriteValue(item.ToString());
            w.WritePropertyName(nameof(EffortValues));
            w.WriteStartObject();
            foreach (PBEEffortValueCollection.PBEEffortValue ev in EffortValues)
            {
                w.WritePropertyName(ev.Stat.ToString());
                w.WriteValue(ev.Value);
            }
            w.WriteEndObject();
            w.WritePropertyName(nameof(IndividualValues));
            w.WriteStartObject();
            foreach (PBEIndividualValueCollection.PBEIndividualValue iv in IndividualValues)
            {
                w.WritePropertyName(iv.Stat.ToString());
                w.WriteValue(iv.Value);
            }
            w.WriteEndObject();
            w.WritePropertyName(nameof(Moveset));
            w.WriteStartArray();
            foreach (PBEMovesetBuilder.PBEMoveSlot slot in Moveset.MoveSlots)
            {
                w.WriteStartObject();
                w.WritePropertyName(nameof(PBEMovesetBuilder.PBEMoveSlot.Move));
                w.WriteValue(slot.Move.ToString());
                w.WritePropertyName(nameof(PBEMovesetBuilder.PBEMoveSlot.PPUps));
                w.WriteValue(slot.PPUps);
                w.WriteEndObject();
            }
            w.WriteEndArray();
            w.WriteEndObject();
        }
    }
}
