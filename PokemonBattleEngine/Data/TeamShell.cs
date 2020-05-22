using Kermalis.EndianBinaryIO;
using Kermalis.PokemonBattleEngine.Utils;
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
    public sealed class PBETeamShell : IDisposable, IEnumerable<PBEPokemonShell>, INotifyCollectionChanged, INotifyPropertyChanged
    {
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

        private readonly List<PBEPokemonShell> _list;
        public int Count => _list.Count;
        public PBEPokemonShell this[int index]
        {
            get
            {
                if (index >= _list.Count)
                {
                    throw new ArgumentOutOfRangeException(nameof(index));
                }
                return _list[index];
            }
        }

        public PBESettings Settings { get; }

        internal PBETeamShell(PBESettings settings, EndianBinaryReader r)
        {
            sbyte count = r.ReadSByte();
            if (count < 1 || count > settings.MaxPartySize)
            {
                throw new InvalidDataException();
            }
            Settings = settings;
            Settings.PropertyChanged += OnSettingsChanged;
            _list = new List<PBEPokemonShell>(Settings.MaxPartySize);
            for (int i = 0; i < count; i++)
            {
                InsertWithEvents(false, new PBEPokemonShell(Settings, r), i);
            }
        }
        public PBETeamShell(string path)
        {
            if (path == null)
            {
                throw new ArgumentNullException(nameof(path));
            }
            var json = JObject.Parse(File.ReadAllText(path));
            Settings = new PBESettings(json[nameof(Settings)].Value<string>());
            var jArray = (JArray)json["Party"];
            if (jArray.Count < 1 || jArray.Count > Settings.MaxPartySize)
            {
                throw new InvalidDataException("Invalid party size.");
            }
            Settings.PropertyChanged += OnSettingsChanged;
            _list = new List<PBEPokemonShell>(Settings.MaxPartySize);
            for (int i = 0; i < jArray.Count; i++)
            {
                InsertWithEvents(false, new PBEPokemonShell(Settings, jArray[i]), i);
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
            _list = new List<PBEPokemonShell>(Settings.MaxPartySize);
            for (int i = 0; i < numPkmnToGenerate; i++)
            {
                Insert(PBERandom.RandomSpecies(), setToMaxLevel ? Settings.MaxLevel : PBERandom.RandomLevel(Settings), false, i);
            }
        }

        private void InsertRandom(bool fireEvent, int index)
        {
            Insert(PBERandom.RandomSpecies(), PBERandom.RandomLevel(Settings), fireEvent, index);
        }
        private void Insert(PBESpecies species, byte level, bool fireEvent, int index)
        {
            InsertWithEvents(fireEvent, new PBEPokemonShell(species, level, Settings) { CanDispose = false }, index);
        }
        private void InsertWithEvents(bool fireEvent, PBEPokemonShell item, int index)
        {
            _list.Insert(index, item);
            if (fireEvent)
            {
                OnPropertyChanged(nameof(Count));
                OnPropertyChanged("Item[]");
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, index));
            }
        }
        private void RemoveWithEvents(PBEPokemonShell item, int index)
        {
            item.CanDispose = true;
            _list.RemoveAt(index);
            NotifyCollectionChangedEventArgs e;
            if (_list.Count == 0)
            {
                InsertRandom(false, 0);
                e = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, _list[0], item, 0);
            }
            else
            {
                OnPropertyChanged(nameof(Count));
                e = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item, index);
            }
            OnPropertyChanged("Item[]");
            OnCollectionChanged(e);
        }

        private void ExceedException()
        {
            throw new InvalidOperationException($"Party size cannot exceed \"{nameof(Settings.MaxPartySize)}\" ({Settings.MaxPartySize}).");
        }
        public void AddRandom()
        {
            if (IsDisposed)
            {
                throw new ObjectDisposedException(null);
            }
            if (_list.Count < Settings.MaxPartySize)
            {
                InsertRandom(true, _list.Count);
            }
            else
            {
                ExceedException();
            }
        }
        public void Add(PBESpecies species, byte level)
        {
            if (IsDisposed)
            {
                throw new ObjectDisposedException(null);
            }
            PBEPokemonShell.ValidateSpecies(species);
            PBEPokemonShell.ValidateLevel(level, Settings);
            if (_list.Count < Settings.MaxPartySize)
            {
                Insert(species, level, true, _list.Count);
            }
            else
            {
                ExceedException();
            }
        }
        public void InsertRandom(int index)
        {
            if (IsDisposed)
            {
                throw new ObjectDisposedException(null);
            }
            if (_list.Count < Settings.MaxPartySize)
            {
                InsertRandom(true, index);
            }
            else
            {
                ExceedException();
            }
        }
        public void Insert(PBESpecies species, byte level, int index)
        {
            if (IsDisposed)
            {
                throw new ObjectDisposedException(null);
            }
            PBEPokemonShell.ValidateSpecies(species);
            PBEPokemonShell.ValidateLevel(level, Settings);
            if (_list.Count < Settings.MaxPartySize)
            {
                Insert(species, level, true, index);
            }
            else
            {
                ExceedException();
            }
        }
        public void Clear()
        {
            if (IsDisposed)
            {
                throw new ObjectDisposedException(null);
            }
            int oldCount = _list.Count;
            for (int i = 0; i < oldCount; i++)
            {
                _list[i].CanDispose = true;
            }
            _list.Clear();
            InsertRandom(false, 0);
            if (oldCount != 1)
            {
                OnPropertyChanged(nameof(Count));
            }
            OnPropertyChanged("Item[]");
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }
        public bool Remove(PBEPokemonShell item)
        {
            if (IsDisposed)
            {
                throw new ObjectDisposedException(null);
            }
            if (item.IsDisposed)
            {
                throw new ObjectDisposedException(nameof(item));
            }
            int index = _list.IndexOf(item);
            bool b = index != -1;
            if (b)
            {
                RemoveWithEvents(item, index);
            }
            return b;
        }
        public void RemoveAt(int index)
        {
            if (IsDisposed)
            {
                throw new ObjectDisposedException(null);
            }
            if (index < 0 || index >= _list.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }
            else
            {
                RemoveWithEvents(_list[index], index);
            }
        }

        public bool Contains(PBEPokemonShell item)
        {
            if (item.IsDisposed)
            {
                throw new ObjectDisposedException(nameof(item));
            }
            return _list.IndexOf(item) != -1;
        }
        public int IndexOf(PBEPokemonShell item)
        {
            if (item.IsDisposed)
            {
                throw new ObjectDisposedException(nameof(item));
            }
            return _list.IndexOf(item);
        }

        public IEnumerator<PBEPokemonShell> GetEnumerator()
        {
            return _list.GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        private void OnSettingsChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(Settings.MaxPartySize):
                {
                    if (_list.Count > Settings.MaxPartySize)
                    {
                        int numToRemove = _list.Count - Settings.MaxPartySize;
                        var changedItems = new PBEPokemonShell[numToRemove];
                        for (int i = 0; i < numToRemove; i++)
                        {
                            int index = _list.Count - 1;
                            PBEPokemonShell item = _list[index];
                            item.CanDispose = true;
                            _list.RemoveAt(index);
                            changedItems[i] = item;
                        }
                        OnPropertyChanged(nameof(Count));
                        OnPropertyChanged("Item[]");
                        OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, changedItems, _list.Count));
                    }
                    break;
                }
            }
        }

        internal void ToBytes(EndianBinaryWriter w)
        {
            sbyte count = (sbyte)_list.Count;
            w.Write(count);
            for (int i = 0; i < count; i++)
            {
                _list[i].ToBytes(w);
            }
        }
        public void ToJsonFile(string path)
        {
            if (IsDisposed)
            {
                throw new ObjectDisposedException(null);
            }
            using (var w = new JsonTextWriter(File.CreateText(path)) { Formatting = Formatting.Indented })
            {
                w.WriteStartObject();
                w.WritePropertyName(nameof(Settings));
                w.WriteValue(Settings.ToString());
                w.WritePropertyName("Party");
                w.WriteStartArray();
                for (int i = 0; i < _list.Count; i++)
                {
                    _list[i].ToJson(w);
                }
                w.WriteEndArray();
                w.WriteEndObject();
            }
        }

        public bool IsDisposed { get; private set; }
        public void Dispose()
        {
            if (!IsDisposed)
            {
                IsDisposed = true;
                OnPropertyChanged(nameof(IsDisposed));
                Settings.PropertyChanged -= OnSettingsChanged;
                for (int i = 0; i < _list.Count; i++)
                {
                    PBEPokemonShell item = _list[i];
                    item.CanDispose = true;
                    item.Dispose();
                }
            }
        }
    }
}
