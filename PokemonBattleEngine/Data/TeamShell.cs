using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;

namespace Kermalis.PokemonBattleEngine.Data
{
    public sealed class PBETeamShell : IEnumerable<PBEPokemonShell>, INotifyCollectionChanged, INotifyPropertyChanged
    {
        private void FireEvents(NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged(nameof(Count));
            OnPropertyChanged("Item[]");
            OnCollectionChanged(e);
        }
        private void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            CollectionChanged?.Invoke(this, e);
        }
        private void OnPropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }
        public event NotifyCollectionChangedEventHandler CollectionChanged;
        public event PropertyChangedEventHandler PropertyChanged;

        private readonly List<PBEPokemonShell> list;
        public int Count => list.Count;
        public PBEPokemonShell this[int index] => list[index];

        public PBESettings Settings { get; }

        internal PBETeamShell(BinaryReader r)
        {
            Settings = new PBESettings(r);
            list = new List<PBEPokemonShell>(Settings.MaxPartySize);
            sbyte count = r.ReadSByte();
            if (count < 1 || count > Settings.MaxPartySize)
            {
                throw new InvalidDataException();
            }
            for (int i = 0; i < count; i++)
            {
                list.Add(new PBEPokemonShell(r, this));
            }
        }
        public PBETeamShell(string path)
        {
            var json = JObject.Parse(File.ReadAllText(path));
            Settings = new PBESettings(json[nameof(Settings)].Value<string>());
            var partyObj = (JArray)json["Party"];
            if (partyObj.Count < 1 || partyObj.Count > Settings.MaxPartySize)
            {
                throw new InvalidDataException("File has an invalid party size for the settings provided.");
            }
            list = new List<PBEPokemonShell>(Settings.MaxPartySize);
            for (int i = 0; i < partyObj.Count; i++)
            {
                list.Add(new PBEPokemonShell(partyObj[i], this));
            }
        }
        public PBETeamShell(PBESettings settings, int numPkmnToGenerate, bool setToMaxLevel)
        {
            if (settings == null)
            {
                throw new ArgumentNullException(nameof(settings));
            }
            if (numPkmnToGenerate < 1 || numPkmnToGenerate > settings.MaxPartySize)
            {
                throw new ArgumentOutOfRangeException(nameof(numPkmnToGenerate));
            }
            Settings = settings;
            list = new List<PBEPokemonShell>(settings.MaxPartySize);
            for (int i = 0; i < numPkmnToGenerate; i++)
            {
                AddOne(false, PBEUtils.RandomSpecies(), setToMaxLevel ? settings.MaxLevel : PBEUtils.RandomLevel(settings));
            }
        }

        private void AddOne(bool fireEvent)
        {
            AddOne(fireEvent, PBEUtils.RandomSpecies(), PBEUtils.RandomLevel(Settings));
        }
        private void AddOne(bool fireEvent, PBESpecies species, byte level)
        {
            var item = new PBEPokemonShell(species, level, this);
            int index = list.Count;
            list.Insert(index, item);
            if (fireEvent)
            {
                FireEvents(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, index));
            }
        }

        public void Add()
        {
            Add(PBEUtils.RandomSpecies(), PBEUtils.RandomLevel(Settings));
        }
        public void Add(PBESpecies species, byte level)
        {
            if (list.Count < Settings.MaxPartySize)
            {
                AddOne(true, species, level);
            }
            else
            {
                throw new InvalidOperationException($"Party size cannot exceed \"{nameof(Settings.MaxPartySize)}\" ({Settings.MaxPartySize}).");
            }
        }
        public void Clear()
        {
            list.Clear();
            AddOne(false);
            FireEvents(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }
        public bool Remove(PBEPokemonShell item)
        {
            int index = list.IndexOf(item);
            bool b = index != -1;
            if (b)
            {
                list.RemoveAt(index);
                NotifyCollectionChangedEventArgs e;
                if (list.Count == 0)
                {
                    AddOne(false);
                    e = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset);
                }
                else
                {
                    e = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item, index);
                }
                FireEvents(e);
            }
            return b;
        }
        public void RemoveAt(int index)
        {
            if (index >= list.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }
            else
            {
                PBEPokemonShell item = list[index];
                list.RemoveAt(index);
                NotifyCollectionChangedEventArgs e;
                if (list.Count == 0)
                {
                    AddOne(false);
                    e = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset);
                }
                else
                {
                    e = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item, index);
                }
                FireEvents(e);
            }
        }

        public bool Contains(PBEPokemonShell item)
        {
            return list.IndexOf(item) != -1;
        }
        public int IndexOf(PBEPokemonShell item)
        {
            return list.IndexOf(item);
        }

        public IEnumerator<PBEPokemonShell> GetEnumerator()
        {
            return list.GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return list.GetEnumerator();
        }

        internal List<byte> ToBytes()
        {
            var bytes = new List<byte>();
            bytes.AddRange(Settings.ToBytes());
            bytes.Add((byte)list.Count);
            for (int i = 0; i < list.Count; i++)
            {
                bytes.AddRange(list[i].ToBytes());
            }
            return bytes;
        }
        public void ToJsonFile(string path)
        {
            using (var w = new JsonTextWriter(File.CreateText(path)) { Formatting = Formatting.Indented })
            {
                w.WriteStartObject();
                w.WritePropertyName(nameof(Settings));
                w.WriteValue(Settings.ToString());
                w.WritePropertyName("Party");
                w.WriteStartArray();
                for (int i = 0; i < list.Count; i++)
                {
                    list[i].ToJson(w);
                }
                w.WriteEndArray();
                w.WriteEndObject();
            }
        }
    }
}
