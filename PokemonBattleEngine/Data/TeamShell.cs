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
            Settings.PropertyChanged += OnSettingsChanged;
            list = new List<PBEPokemonShell>(Settings.MaxPartySize);
            sbyte count = r.ReadSByte();
            if (count < 1 || count > Settings.MaxPartySize)
            {
                throw new InvalidDataException();
            }
            for (int i = 0; i < count; i++)
            {
                AddWithEvents(false, new PBEPokemonShell(r, this), i);
            }
        }
        public PBETeamShell(string path)
        {
            var json = JObject.Parse(File.ReadAllText(path));
            Settings = new PBESettings(json[nameof(Settings)].Value<string>());
            Settings.PropertyChanged += OnSettingsChanged;
            var partyObj = (JArray)json["Party"];
            if (partyObj.Count < 1 || partyObj.Count > Settings.MaxPartySize)
            {
                throw new InvalidDataException("File has an invalid party size for the settings provided.");
            }
            list = new List<PBEPokemonShell>(Settings.MaxPartySize);
            for (int i = 0; i < partyObj.Count; i++)
            {
                AddWithEvents(false, new PBEPokemonShell(partyObj[i], this), i);
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
            Settings.PropertyChanged += OnSettingsChanged;
            list = new List<PBEPokemonShell>(Settings.MaxPartySize);
            for (int i = 0; i < numPkmnToGenerate; i++)
            {
                Add(false, PBEUtils.RandomSpecies(), setToMaxLevel ? Settings.MaxLevel : PBEUtils.RandomLevel(Settings));
            }
        }

        private void AddRandom(bool fireEvent)
        {
            Add(fireEvent, PBEUtils.RandomSpecies(), PBEUtils.RandomLevel(Settings));
        }
        private void Add(bool fireEvent, PBESpecies species, byte level)
        {
            var item = new PBEPokemonShell(species, level, this);
            int index = list.Count;
            AddWithEvents(fireEvent, item, index);
        }
        private void AddWithEvents(bool fireEvent, PBEPokemonShell item, int index)
        {
            Settings.PropertyChanged += item.OnSettingsChanged;
            list.Insert(index, item);
            if (fireEvent)
            {
                FireEvents(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, index));
            }
        }
        private void RemoveWithEvents(PBEPokemonShell item, int index)
        {
            Settings.PropertyChanged -= item.OnSettingsChanged;
            list.RemoveAt(index);
            NotifyCollectionChangedEventArgs e;
            if (list.Count == 0)
            {
                AddRandom(false);
                e = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, list[index], item, index);
            }
            else
            {
                e = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item, index);
            }
            FireEvents(e);
        }

        private void ExceedException()
        {
            throw new InvalidOperationException($"Party size cannot exceed \"{nameof(Settings.MaxPartySize)}\" ({Settings.MaxPartySize}).");
        }
        public void AddRandom()
        {
            if (list.Count < Settings.MaxPartySize)
            {
                AddRandom(true);
            }
            else
            {
                ExceedException();
            }
        }
        public void Add(PBESpecies species, byte level)
        {
            if (list.Count < Settings.MaxPartySize)
            {
                Add(true, species, level);
            }
            else
            {
                ExceedException();
            }
        }
        public void Clear()
        {
            for (int i = 0; i < list.Count; i++)
            {
                Settings.PropertyChanged -= list[i].OnSettingsChanged;
            }
            list.Clear();
            AddRandom(false);
            FireEvents(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }
        public bool Remove(PBEPokemonShell item)
        {
            int index = list.IndexOf(item);
            bool b = index != -1;
            if (b)
            {
                RemoveWithEvents(item, index);
            }
            return b;
        }
        public void RemoveAt(int index)
        {
            if (index < 0 || index >= list.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }
            else
            {
                RemoveWithEvents(list[index], index);
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

        private void OnSettingsChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(Settings.MaxPartySize):
                {
                    if (list.Count > Settings.MaxPartySize)
                    {
                        int numToRemove = list.Count - Settings.MaxPartySize;
                        var changedItems = new PBEPokemonShell[numToRemove];
                        for (int i = 0; i < numToRemove; i++)
                        {
                            int index = list.Count - 1;
                            PBEPokemonShell item = list[index];
                            Settings.PropertyChanged -= item.OnSettingsChanged;
                            list.RemoveAt(index);
                            changedItems[i] = item;
                        }
                        FireEvents(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, changedItems, list.Count));
                    }
                    break;
                }
            }
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
