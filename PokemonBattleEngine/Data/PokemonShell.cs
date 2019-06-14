using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;

namespace Kermalis.PokemonBattleEngine.Data
{
    public sealed class PBEPokemonShell : INotifyPropertyChanged
    {
        private void OnPropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }
        public event PropertyChangedEventHandler PropertyChanged;

        private PBESpecies species;
        public PBESpecies Species
        {
            get => species;
            set
            {
                species = value;
                OnPropertyChanged(nameof(Species));
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
        private byte[] evs;
        public byte[] EVs
        {
            get => evs;
            set
            {
                evs = value;
                OnPropertyChanged(nameof(EVs));
            }
        }
        private byte[] ivs;
        public byte[] IVs
        {
            get => ivs;
            set
            {
                ivs = value;
                OnPropertyChanged(nameof(IVs));
            }
        }
        private PBEMove[] moves;
        public PBEMove[] Moves
        {
            get => moves;
            set
            {
                moves = value;
                OnPropertyChanged(nameof(Moves));
            }
        }
        private byte[] ppups;
        public byte[] PPUps
        {
            get => ppups;
            set
            {
                ppups = value;
                OnPropertyChanged(nameof(PPUps));
            }
        }

        // TODO: Include settings
        // TODO: Reject team sizes above settings max
        public static IEnumerable<PBEPokemonShell> TeamFromJsonFile(string path)
        {
            var json = JObject.Parse(File.ReadAllText(path));
            var partyObject = (JArray)json["Party"];
            var party = new PBEPokemonShell[partyObject.Count];
            for (int i = 0; i < party.Length; i++)
            {
                JToken pkmnObject = partyObject[i];
                var pkmn = new PBEPokemonShell
                {
                    species = PBELocalizedString.GetSpeciesByName(pkmnObject[nameof(Species)].Value<string>()).Value, // Must have species
                    nickname = pkmnObject[nameof(Nickname)].Value<string>(),
                    level = pkmnObject[nameof(Level)].Value<byte>(),
                    friendship = pkmnObject[nameof(Friendship)].Value<byte>(),
                    shiny = pkmnObject[nameof(Shiny)].Value<bool>(),
                    ability = PBELocalizedString.GetAbilityByName(pkmnObject[nameof(Ability)].Value<string>()).Value, // Must have ability
                    nature = (PBENature)Enum.Parse(typeof(PBENature), pkmnObject[nameof(Nature)].Value<string>(), true),
                    gender = (PBEGender)Enum.Parse(typeof(PBEGender), pkmnObject[nameof(Gender)].Value<string>(), true),
                    item = PBELocalizedString.GetItemByName(pkmnObject[nameof(Item)].Value<string>()).GetValueOrDefault() // Item can be None
                };
                JToken wObject = pkmnObject[nameof(EVs)];
                pkmn.evs = new byte[6]
                {
                    wObject[nameof(PBEStat.HP)].Value<byte>(),
                    wObject[nameof(PBEStat.Attack)].Value<byte>(),
                    wObject[nameof(PBEStat.Defense)].Value<byte>(),
                    wObject[nameof(PBEStat.SpAttack)].Value<byte>(),
                    wObject[nameof(PBEStat.SpDefense)].Value<byte>(),
                    wObject[nameof(PBEStat.Speed)].Value<byte>()
                };
                wObject = pkmnObject[nameof(IVs)];
                pkmn.ivs = new byte[6]
                {
                    wObject[nameof(PBEStat.HP)].Value<byte>(),
                    wObject[nameof(PBEStat.Attack)].Value<byte>(),
                    wObject[nameof(PBEStat.Defense)].Value<byte>(),
                    wObject[nameof(PBEStat.SpAttack)].Value<byte>(),
                    wObject[nameof(PBEStat.SpDefense)].Value<byte>(),
                    wObject[nameof(PBEStat.Speed)].Value<byte>()
                };
                wObject = pkmnObject[nameof(Moves)];
                pkmn.moves = new PBEMove[4];
                for (int j = 0; j < 4; j++)
                {
                    pkmn.moves[j] = PBELocalizedString.GetMoveByName(wObject[$"Move {j}"].Value<string>()).GetValueOrDefault(); // Move can be None
                }
                wObject = pkmnObject[nameof(PPUps)];
                pkmn.ppups = new byte[4];
                for (int j = 0; j < 4; j++)
                {
                    pkmn.ppups[j] = wObject[$"Move {j}"].Value<byte>();
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
                    writer.WritePropertyName(nameof(EVs));
                    writer.WriteStartObject();
                    for (int j = 0; j < 6; j++)
                    {
                        writer.WritePropertyName(((PBEStat)j).ToString());
                        writer.WriteValue(pkmn.evs[j]);
                    }
                    writer.WriteEndObject();
                    writer.WritePropertyName(nameof(IVs));
                    writer.WriteStartObject();
                    for (int j = 0; j < 6; j++)
                    {
                        writer.WritePropertyName(((PBEStat)j).ToString());
                        writer.WriteValue(pkmn.ivs[j]);
                    }
                    writer.WriteEndObject();
                    writer.WritePropertyName(nameof(Moves));
                    writer.WriteStartObject();
                    for (int j = 0; j < 4; j++)
                    {
                        writer.WritePropertyName($"Move {j}");
                        writer.WriteValue(pkmn.moves[j].ToString());
                    }
                    writer.WriteEndObject();
                    writer.WritePropertyName(nameof(PPUps));
                    writer.WriteStartObject();
                    for (int j = 0; j < 4; j++)
                    {
                        writer.WritePropertyName($"Move {j}");
                        writer.WriteValue(pkmn.ppups[j]);
                    }
                    writer.WriteEndObject();
                    writer.WriteEndObject();
                }
                writer.WriteEndArray();
                writer.WriteEndObject();
            }
        }

        internal byte[] ToBytes()
        {
            var bytes = new List<byte>();
            bytes.AddRange(BitConverter.GetBytes((uint)Species));
            bytes.AddRange(PBEUtils.StringToBytes(Nickname));
            bytes.Add(Level);
            bytes.Add(Friendship);
            bytes.Add((byte)(Shiny ? 1 : 0));
            bytes.Add((byte)Ability);
            bytes.Add((byte)Nature);
            bytes.Add((byte)Gender);
            bytes.AddRange(BitConverter.GetBytes((ushort)Item));
            bytes.AddRange(EVs);
            bytes.AddRange(IVs);
            bytes.Add((byte)Moves.Length);
            for (int i = 0; i < Moves.Length; i++)
            {
                bytes.AddRange(BitConverter.GetBytes((ushort)Moves[i]));
            }
            bytes.AddRange(PPUps);
            return bytes.ToArray();
        }
        internal static PBEPokemonShell FromBytes(BinaryReader r)
        {
            var pkmn = new PBEPokemonShell
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
                evs = r.ReadBytes(6),
                ivs = r.ReadBytes(6)
            };
            byte numMoves = r.ReadByte();
            pkmn.moves = new PBEMove[numMoves];
            for (int i = 0; i < numMoves; i++)
            {
                pkmn.moves[i] = (PBEMove)r.ReadUInt16();
            }
            pkmn.ppups = r.ReadBytes(numMoves);
            return pkmn;
        }
    }
}
