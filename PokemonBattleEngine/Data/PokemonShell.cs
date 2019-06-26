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
    // TODO: Don't fire propertychanged if new value is same as old value
    // TODO: Validate setter values (and constructor)
    // TODO: Way more constructors? The way we load from json and bytes is messy
    // TODO: Set settings and listen to changes
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

        private readonly PBESettings settings;

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
                PBESpecies old = species;
                species = value;
                OnPropertyChanged(nameof(Species));
                SpeciesChanged(old);
            }
        }
        private string nickname;
        public string Nickname
        {
            get => nickname;
            set
            {
                nickname = value;
                OnPropertyChanged(nameof(Nickname));
            }
        }
        private byte level;
        public byte Level
        {
            get => level;
            set
            {
                level = value;
                OnPropertyChanged(nameof(Level));
                UpdateMoves();
            }
        }
        private byte friendship;
        public byte Friendship
        {
            get => friendship;
            set
            {
                friendship = value;
                OnPropertyChanged(nameof(Friendship));
            }
        }
        private bool shiny;
        public bool Shiny
        {
            get => shiny;
            set
            {
                shiny = value;
                OnPropertyChanged(nameof(Shiny));
            }
        }
        private PBEAbility ability;
        public PBEAbility Ability
        {
            get => ability;
            set
            {
                ability = value;
                OnPropertyChanged(nameof(Ability));
            }
        }
        private PBENature nature;
        public PBENature Nature
        {
            get => nature;
            set
            {
                nature = value;
                OnPropertyChanged(nameof(Nature));
            }
        }
        private PBEGender gender;
        public PBEGender Gender
        {
            get => gender;
            set
            {
                gender = value;
                OnPropertyChanged(nameof(Gender));
            }
        }
        private PBEItem item;
        public PBEItem Item
        {
            get => item;
            set
            {
                item = value;
                OnPropertyChanged(nameof(Item));
            }
        }
        public PBEEffortValueCollection EffortValues { get; private set; }
        public PBEIndividualValueCollection IndividualValues { get; private set; }
        public PBEMovesetBuilder Moveset { get; private set; }

        private PBEPokemonShell(PBESettings settings)
        {
            this.settings = settings;
        }
        public PBEPokemonShell(PBESpecies species, byte level, PBESettings settings)
            : this(settings)
        {
            this.species = species;
            this.level = level;
            friendship = (byte)PBEUtils.RNG.Next(byte.MaxValue + 1);
            shiny = PBEUtils.RNG.NextShiny();
            nature = AllNatures.Sample();
            EffortValues = new PBEEffortValueCollection(settings);
            IndividualValues = new PBEIndividualValueCollection(settings);
            SpeciesChanged(0);
        }
        private void SetSelectable()
        {
            var pData = PBEPokemonData.GetData(species);
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
        private void SpeciesChanged(PBESpecies oldSpecies)
        {
            SetSelectable();
            // Change nickname if previous nickname was the species name
            if (oldSpecies == 0 || nickname == PBELocalizedString.GetSpeciesName(oldSpecies).FromUICultureInfo())
            {
                Nickname = PBELocalizedString.GetSpeciesName(species).FromUICultureInfo();
            }
            if (!SelectableAbilities.Contains(ability))
            {
                Ability = SelectableAbilities.Sample();
            }
            if (oldSpecies == 0 || !SelectableGenders.Contains(gender))
            {
                Gender = PBEUtils.RNG.NextGender(PBEPokemonData.GetData(species).GenderRatio);
            }
            if (oldSpecies == 0 || !SelectableItems.Contains(item))
            {
                Item = SelectableItems.Sample();
            }
            if (oldSpecies == 0)
            {
                Moveset = new PBEMovesetBuilder(species, level, settings);
            }
            else
            {
                UpdateMoves();
            }
        }
        private void UpdateMoves()
        {
            PBEMove[] moves = Moveset.MoveSlots.Select(m => m.Move).ToArray();
            byte[] ppUps = Moveset.MoveSlots.Select(m => m.PPUps).ToArray();
            Moveset = new PBEMovesetBuilder(species, level, settings);
            Moveset.Clear();
            for (int i = 0; i < settings.NumMoves; i++)
            {
                PBEMovesetBuilder.PBEMoveSlot slot = Moveset.MoveSlots[i];
                PBEMove move = moves[i];
                if (slot.Allowed.Contains(move))
                {
                    Moveset.Set(i, move, slot.IsPPUpsEditable ? ppUps[i] : (byte?)null);
                }
            }
            OnPropertyChanged(nameof(Moveset));
        }

        // TODO: Include settings
        // TODO: Reject team sizes above settings max
        // TODO: Validate values
        // TODO: Do not allow PBEAbility.None (kinda same as above)
        public static IEnumerable<PBEPokemonShell> TeamFromJsonFile(string path)
        {
            var json = JObject.Parse(File.ReadAllText(path));
            PBESettings settings = PBESettings.DefaultSettings;
            var partyObject = (JArray)json["Party"];
            var party = new PBEPokemonShell[partyObject.Count];
            for (int i = 0; i < party.Length; i++)
            {
                JToken pkmnObject = partyObject[i];
                var pkmn = new PBEPokemonShell(settings)
                {
                    species = PBELocalizedString.GetSpeciesByName(pkmnObject[nameof(Species)].Value<string>()).Value,
                    nickname = pkmnObject[nameof(Nickname)].Value<string>(),
                    level = pkmnObject[nameof(Level)].Value<byte>(),
                    friendship = pkmnObject[nameof(Friendship)].Value<byte>(),
                    shiny = pkmnObject[nameof(Shiny)].Value<bool>(),
                    ability = PBELocalizedString.GetAbilityByName(pkmnObject[nameof(Ability)].Value<string>()).Value,
                    nature = (PBENature)Enum.Parse(typeof(PBENature), pkmnObject[nameof(Nature)].Value<string>(), true),
                    gender = (PBEGender)Enum.Parse(typeof(PBEGender), pkmnObject[nameof(Gender)].Value<string>(), true),
                    item = PBELocalizedString.GetItemByName(pkmnObject[nameof(Item)].Value<string>()).Value
                };
                pkmn.SetSelectable();
                JToken wObject = pkmnObject[nameof(EffortValues)];
                pkmn.EffortValues = new PBEEffortValueCollection(settings,
                    wObject[nameof(PBEStat.HP)].Value<byte>(),
                    wObject[nameof(PBEStat.Attack)].Value<byte>(),
                    wObject[nameof(PBEStat.Defense)].Value<byte>(),
                    wObject[nameof(PBEStat.SpAttack)].Value<byte>(),
                    wObject[nameof(PBEStat.SpDefense)].Value<byte>(),
                    wObject[nameof(PBEStat.Speed)].Value<byte>()
                );
                wObject = pkmnObject[nameof(IndividualValues)];
                pkmn.IndividualValues = new PBEIndividualValueCollection(settings,
                    wObject[nameof(PBEStat.HP)].Value<byte>(),
                    wObject[nameof(PBEStat.Attack)].Value<byte>(),
                    wObject[nameof(PBEStat.Defense)].Value<byte>(),
                    wObject[nameof(PBEStat.SpAttack)].Value<byte>(),
                    wObject[nameof(PBEStat.SpDefense)].Value<byte>(),
                    wObject[nameof(PBEStat.Speed)].Value<byte>()
                );
                var mObject = (JArray)pkmnObject[nameof(Moveset)];
                pkmn.Moveset = new PBEMovesetBuilder(pkmn.species, pkmn.level, settings);
                pkmn.Moveset.Clear();
                for (int j = 0; j < settings.NumMoves; j++)
                {
                    wObject = mObject[j];
                    pkmn.Moveset.Set(j,
                        PBELocalizedString.GetMoveByName(wObject[nameof(PBEMovesetBuilder.PBEMoveSlot.Move)].Value<string>()).Value,
                        wObject[nameof(PBEMovesetBuilder.PBEMoveSlot.PPUps)].Value<byte>()
                        );
                }
                party[i] = pkmn;
            }
            return party;
        }
        // TODO: Include settings
        // TODO: Reject team sizes above settings max
        public static void TeamToJsonFile(string path, IEnumerable<PBEPokemonShell> party)
        {
            using (var writer = new JsonTextWriter(File.CreateText(path)) { Formatting = Formatting.Indented })
            {
                writer.WriteStartObject();
                writer.WritePropertyName("Party");
                writer.WriteStartArray();
                PBEPokemonShell[] p = party.ToArray();
                byte amt = (byte)p.Length;
                for (int i = 0; i < amt; i++)
                {
                    PBEPokemonShell pkmn = p[i];
                    writer.WriteStartObject();
                    writer.WritePropertyName(nameof(Species));
                    writer.WriteValue(pkmn.species.ToString());
                    writer.WritePropertyName(nameof(Nickname));
                    writer.WriteValue(pkmn.nickname);
                    writer.WritePropertyName(nameof(Level));
                    writer.WriteValue(pkmn.level);
                    writer.WritePropertyName(nameof(Friendship));
                    writer.WriteValue(pkmn.friendship);
                    writer.WritePropertyName(nameof(Shiny));
                    writer.WriteValue(pkmn.shiny);
                    writer.WritePropertyName(nameof(Ability));
                    writer.WriteValue(pkmn.ability.ToString());
                    writer.WritePropertyName(nameof(Nature));
                    writer.WriteValue(pkmn.nature.ToString());
                    writer.WritePropertyName(nameof(Gender));
                    writer.WriteValue(pkmn.gender.ToString());
                    writer.WritePropertyName(nameof(Item));
                    writer.WriteValue(pkmn.item.ToString());
                    writer.WritePropertyName(nameof(EffortValues));
                    writer.WriteStartObject();
                    foreach (PBEEffortValueCollection.PBEEffortValue ev in pkmn.EffortValues)
                    {
                        writer.WritePropertyName(ev.Stat.ToString());
                        writer.WriteValue(ev.Value);
                    }
                    writer.WriteEndObject();
                    writer.WritePropertyName(nameof(IndividualValues));
                    writer.WriteStartObject();
                    foreach (PBEIndividualValueCollection.PBEIndividualValue iv in pkmn.IndividualValues)
                    {
                        writer.WritePropertyName(iv.Stat.ToString());
                        writer.WriteValue(iv.Value);
                    }
                    writer.WriteEndObject();
                    writer.WritePropertyName(nameof(Moveset));
                    writer.WriteStartArray();
                    foreach (PBEMovesetBuilder.PBEMoveSlot slot in pkmn.Moveset.MoveSlots)
                    {
                        writer.WriteStartObject();
                        writer.WritePropertyName(nameof(PBEMovesetBuilder.PBEMoveSlot.Move));
                        writer.WriteValue(slot.Move.ToString());
                        writer.WritePropertyName(nameof(PBEMovesetBuilder.PBEMoveSlot.PPUps));
                        writer.WriteValue(slot.PPUps);
                        writer.WriteEndObject();
                    }
                    writer.WriteEndArray();
                    writer.WriteEndObject();
                }
                writer.WriteEndArray();
                writer.WriteEndObject();
            }
        }

        internal byte[] ToBytes()
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
            for (int i = 0; i < settings.NumMoves; i++)
            {
                bytes.AddRange(BitConverter.GetBytes((ushort)Moveset.MoveSlots[i].Move));
            }
            bytes.AddRange(Moveset.MoveSlots.Select(m => m.PPUps));
            return bytes.ToArray();
        }
        internal static PBEPokemonShell FromBytes(BinaryReader r, PBESettings settings)
        {
            var pkmn = new PBEPokemonShell(settings)
            {
                species = (PBESpecies)r.ReadUInt32(),
                nickname = PBEUtils.StringFromBytes(r),
                level = r.ReadByte(),
                friendship = r.ReadByte(),
                shiny = r.ReadBoolean(),
                ability = (PBEAbility)r.ReadByte(),
                nature = (PBENature)r.ReadByte(),
                gender = (PBEGender)r.ReadByte(),
                item = (PBEItem)r.ReadUInt16(),
                EffortValues = new PBEEffortValueCollection(settings, r.ReadByte(), r.ReadByte(), r.ReadByte(), r.ReadByte(), r.ReadByte(), r.ReadByte()),
                IndividualValues = new PBEIndividualValueCollection(settings, r.ReadByte(), r.ReadByte(), r.ReadByte(), r.ReadByte(), r.ReadByte(), r.ReadByte())
            };
            var moves = new PBEMove[settings.NumMoves];
            for (int i = 0; i < settings.NumMoves; i++)
            {
                moves[i] = (PBEMove)r.ReadUInt16();
            }
            byte[] ppUps = r.ReadBytes(settings.NumMoves);
            pkmn.Moveset = new PBEMovesetBuilder(pkmn.species, pkmn.level, settings);
            pkmn.Moveset.Clear();
            for (int i = 0; i < settings.NumMoves; i++)
            {
                pkmn.Moveset.Set(i, moves[i], ppUps[i]);
            }
            return pkmn;
        }
    }
}
