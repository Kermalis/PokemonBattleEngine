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
using System.Linq;

namespace Kermalis.PokemonBattleEngine.Data
{
    public sealed class PBEMoveset : IDisposable, INotifyCollectionChanged, INotifyPropertyChanged, IReadOnlyList<PBEMoveset.PBEMovesetSlot>
    {
        public sealed class PBEMovesetSlot : INotifyPropertyChanged
        {
            private void OnPropertyChanged(string property)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
            }
            public event PropertyChangedEventHandler PropertyChanged;

            private readonly PBEMoveset _parent;
            private readonly int _index;

            public PBEAlphabeticalList<PBEMove> Allowed { get; }
            private PBEMove _move;
            public PBEMove Move
            {
                get => _move;
                set
                {
                    if (_parent.IsDisposed)
                    {
                        throw new ObjectDisposedException(null);
                    }
                    PBEMove old = _move;
                    if (old != value)
                    {
                        if (value >= PBEMove.MAX || !PBEMoveData.IsMoveUsable(value))
                        {
                            throw new ArgumentOutOfRangeException(nameof(value));
                        }
                        if (!_isMoveEditable)
                        {
                            throw new InvalidOperationException($"Slot {_index}'s move cannot be changed because there is no move in slot {_index - 1}.");
                        }
                        if (!Allowed.Contains(value))
                        {
                            throw new ArgumentOutOfRangeException(nameof(value), $"Slot {_index} does not allow {value}.");
                        }
                        if (value != PBEMove.None)
                        {
                            // If "move" is in another slot, place "slotIndex"'s old move at the other slot
                            for (int i = 0; i < _parent.Settings.NumMoves; i++)
                            {
                                if (i != _index)
                                {
                                    PBEMovesetSlot iSlot = _parent[i];
                                    if (iSlot.Move == value)
                                    {
                                        // If slot 0 is Snore and slot 3 is None but is trying to become Snore, do nothing because the first Snore is in an earlier slot and swapping None to an earlier slot makes no sense
                                        if (old == PBEMove.None && i < _index)
                                        {
                                            goto finish;
                                        }
                                        else
                                        {
                                            UpdateMove(value);
                                            iSlot.UpdateMove(old);
                                            goto editables;
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            // If "move" is None and a slot after "slotIndex" is not None, then place None at the other slot instead and place the other slot's move at "slotIndex"
                            for (int i = _parent.Settings.NumMoves - 1; i > _index; i--)
                            {
                                PBEMovesetSlot iSlot = _parent[i];
                                if (iSlot.Move != PBEMove.None)
                                {
                                    UpdateMove(iSlot.Move);
                                    iSlot.UpdateMove(PBEMove.None);
                                    goto editables;
                                }
                            }
                        }
                        // This gets reached if:
                        // "move" is not None and there is no other slot with "move"
                        // "move" is None and there is no slot after "slotIndex" with a move
                        UpdateMove(value);
                    editables:
                        _parent.SetEditables();
                    finish:
                        ;
                    }
                }
            }
            private bool _isMoveEditable;
            public bool IsMoveEditable
            {
                get => _isMoveEditable;
                internal set
                {
                    if (_isMoveEditable != value)
                    {
                        _isMoveEditable = value;
                        OnPropertyChanged(nameof(IsMoveEditable));
                    }
                }
            }
            private byte _ppUps;
            public byte PPUps
            {
                get => _ppUps;
                set
                {
                    if (_parent.IsDisposed)
                    {
                        throw new ObjectDisposedException(null);
                    }
                    if (_ppUps != value)
                    {
                        if (value > _parent.Settings.MaxPPUps)
                        {
                            throw new ArgumentOutOfRangeException(nameof(value), $"\"{nameof(value)}\" cannot exceed \"{nameof(_parent.Settings.MaxPPUps)}\" ({_parent.Settings.MaxPPUps}).");
                        }
                        if (!IsPPUpsEditable)
                        {
                            throw new InvalidOperationException($"Slot {_index}'s PP-Ups cannot be changed because it has no move.");
                        }
                        UpdatePPUps(value);
                    }
                }
            }
            private bool _isPPUpsEditable;
            public bool IsPPUpsEditable
            {
                get => _isPPUpsEditable;
                private set
                {
                    if (_isPPUpsEditable != value)
                    {
                        _isPPUpsEditable = value;
                        OnPropertyChanged(nameof(IsPPUpsEditable));
                    }
                }
            }

            internal PBEMovesetSlot(PBEMoveset parent, int index)
            {
                _parent = parent;
                _index = index;
                _isMoveEditable = index < 2;
                Allowed = new PBEAlphabeticalList<PBEMove>(new[] { PBEMove.None });
            }

            private void UpdateMove(PBEMove move)
            {
                if (_move != move)
                {
                    _move = move;
                    OnPropertyChanged(nameof(Move));
                    if (_move == PBEMove.None)
                    {
                        UpdatePPUps(0);
                    }
                    IsPPUpsEditable = _move != PBEMove.None;
                }
            }
            private void UpdatePPUps(byte ppUps)
            {
                if (_ppUps != ppUps)
                {
                    _ppUps = ppUps;
                    OnPropertyChanged(nameof(PPUps));
                }
            }
        }

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

        private PBESpecies _species;
        public PBESpecies Species
        {
            get => _species;
            set
            {
                if (IsDisposed)
                {
                    throw new ObjectDisposedException(null);
                }
                if (_species != value)
                {
                    PBEPokemonShell.ValidateSpecies(value, 0, true);
                    _species = value;
                    _form = 0;
                    OnPropertyChanged(nameof(Species));
                    SetAlloweds();
                }
            }
        }
        private PBEForm _form;
        public PBEForm Form
        {
            get => _form;
            set
            {
                if (IsDisposed)
                {
                    throw new ObjectDisposedException(null);
                }
                if (_form != value)
                {
                    PBEPokemonShell.ValidateSpecies(_species, value, true);
                    _form = value;
                    OnPropertyChanged(nameof(Form));
                    SetAlloweds();
                }
            }
        }
        private byte _level;
        public byte Level
        {
            get => _level;
            set
            {
                if (IsDisposed)
                {
                    throw new ObjectDisposedException(null);
                }
                if (_level != value)
                {
                    PBEPokemonShell.ValidateLevel(value, Settings);
                    _level = value;
                    OnPropertyChanged(nameof(Level));
                    SetAlloweds();
                }
            }
        }
        public PBESettings Settings { get; }
        private readonly List<PBEMovesetSlot> _list;
        public int Count => _list.Count;
        public PBEMovesetSlot this[int index]
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
        public PBEMovesetSlot this[PBEMove move]
        {
            get
            {
                for (int i = 0; i < Settings.NumMoves; i++)
                {
                    PBEMovesetSlot slot = _list[i];
                    if (slot.Move == move)
                    {
                        return slot;
                    }
                }
                return null;
            }
        }

        internal PBEMoveset(PBESpecies species, PBEForm form, byte level, PBESettings settings, EndianBinaryReader r)
        {
            if (r.ReadByte() != settings.NumMoves)
            {
                throw new InvalidDataException();
            }
            _species = species;
            _form = form;
            _level = level;
            Settings = settings;
            _list = new List<PBEMovesetSlot>(Settings.NumMoves);
            for (int i = 0; i < Settings.NumMoves; i++)
            {
                Add(new PBEMovesetSlot(this, i));
            }
            SetAlloweds();
            for (int i = 0; i < Settings.NumMoves; i++)
            {
                PBEMovesetSlot slot = _list[i];
                PBEMove move = r.ReadEnum<PBEMove>();
                slot.Move = move;
                if (slot.Move != move)
                {
                    throw new InvalidDataException();
                }
                byte ppUps = r.ReadByte();
                slot.PPUps = ppUps;
                if (slot.PPUps != ppUps)
                {
                    throw new InvalidDataException();
                }
            }
            Settings.PropertyChanged += OnSettingsChanged;
        }
        internal PBEMoveset(PBESpecies species, PBEForm form, byte level, PBESettings settings, JArray jArray)
        {
            if (jArray.Count != settings.NumMoves)
            {
                throw new ArgumentOutOfRangeException(nameof(PBEPokemonShell.Moveset), $"Moveset count must be equal to \"{nameof(settings.NumMoves)}\" ({settings.NumMoves}).");
            }
            _species = species;
            _form = form;
            _level = level;
            Settings = settings;
            _list = new List<PBEMovesetSlot>(Settings.NumMoves);
            for (int i = 0; i < Settings.NumMoves; i++)
            {
                Add(new PBEMovesetSlot(this, i));
            }
            SetAlloweds();
            for (int i = 0; i < Settings.NumMoves; i++)
            {
                PBEMovesetSlot slot = _list[i];
                JToken jToken = jArray[i];
                PBEMove move = PBELocalizedString.GetMoveByName(jToken[nameof(PBEMovesetSlot.Move)].Value<string>()).Value;
                slot.Move = move;
                if (slot.Move != move)
                {
                    throw new ArgumentOutOfRangeException(nameof(PBEPokemonShell.Moveset), "Invalid moves.");
                }
                byte ppUps = jToken[nameof(PBEMovesetSlot.PPUps)].Value<byte>();
                slot.PPUps = ppUps;
                if (slot.PPUps != ppUps)
                {
                    throw new ArgumentOutOfRangeException(nameof(PBEPokemonShell.Moveset), "Invalid PP-Ups.");
                }
            }
            Settings.PropertyChanged += OnSettingsChanged;
        }
        internal PBEMoveset(PBEMoveset other)
        {
            _species = other._species;
            _form = other._form;
            _level = other._level;
            Settings = other.Settings;
            Settings.PropertyChanged += OnSettingsChanged;
            _list = new List<PBEMovesetSlot>(Settings.NumMoves);
            for (int i = 0; i < Settings.NumMoves; i++)
            {
                Add(new PBEMovesetSlot(this, i));
            }
            SetAlloweds();
            for (int i = 0; i < Settings.NumMoves; i++)
            {
                PBEMovesetSlot slot = _list[i];
                PBEMovesetSlot oSlot = other[i];
                slot.Move = oSlot.Move;
                slot.PPUps = oSlot.PPUps;
            }
        }
        /// <summary>Creates a new <see cref="PBEMoveset"/> object with the specified traits.</summary>
        /// <param name="species">The species of the Pokémon that this moveset will be built for.</param>
        /// <param name="form">The form of the Pokémon that this moveset will be built for.</param>
        /// <param name="level">The level of the Pokémon that this moveset will be built for.</param>
        /// <param name="settings">The settings that will be used to evaluate the <see cref="PBEMoveset"/>.</param>
        /// <param name="randomize">True if <see cref="Randomize"/> should be called, False if the move slots use their default values.</param>
        public PBEMoveset(PBESpecies species, PBEForm form, byte level, PBESettings settings, bool randomize)
        {
            if (settings == null)
            {
                throw new ArgumentNullException(nameof(settings));
            }
            PBEPokemonShell.ValidateSpecies(species, form, true);
            PBEPokemonShell.ValidateLevel(level, settings);
            _level = level;
            _species = species;
            _form = form;
            Settings = settings;
            Settings.PropertyChanged += OnSettingsChanged;
            _canDispose = true;
            _list = new List<PBEMovesetSlot>(Settings.NumMoves);
            for (int i = 0; i < Settings.NumMoves; i++)
            {
                Add(new PBEMovesetSlot(this, i));
            }
            SetAlloweds();
            if (randomize)
            {
                Randomize();
            }
        }

        private void Add(PBEMovesetSlot item)
        {
            int index = _list.Count;
            _list.Insert(index, item);
            FireEvents(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, index));
        }
        private bool Remove(PBEMovesetSlot item)
        {
            int index = _list.IndexOf(item);
            bool b = index != -1;
            if (b)
            {
                _list.RemoveAt(index);
                FireEvents(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item, index));
                item.Allowed.Dispose();
            }
            return b;
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
                case nameof(Settings.MaxPPUps):
                {
                    for (int i = 0; i < Settings.NumMoves; i++)
                    {
                        PBEMovesetSlot slot = _list[i];
                        if (slot.PPUps > Settings.MaxPPUps)
                        {
                            slot.PPUps = Settings.MaxPPUps;
                        }
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
                case nameof(Settings.NumMoves):
                {
                    int oldCount = _list.Count;
                    if (Settings.NumMoves != oldCount)
                    {
                        if (Settings.NumMoves > oldCount)
                        {
                            if (Settings.NumMoves > _list.Capacity)
                            {
                                _list.Capacity = Settings.NumMoves;
                            }
                            int numToAdd = Settings.NumMoves - oldCount;
                            for (int i = 0; i < numToAdd; i++)
                            {
                                Add(new PBEMovesetSlot(this, oldCount + i));
                            }
                        }
                        else
                        {
                            int numToRemove = oldCount - Settings.NumMoves;
                            for (int i = 0; i < numToRemove; i++)
                            {
                                Remove(_list[oldCount - 1 - i]);
                            }
                        }
                        SetAlloweds();
                    }
                    break;
                }
            }
        }

        private static readonly PBEAlphabeticalList<PBEMove> secretSwordArray = new PBEAlphabeticalList<PBEMove>(new PBEMove[1] { PBEMove.SecretSword });
        private void SetAlloweds()
        {
            // Set alloweds
            int i;
            IReadOnlyCollection<PBEMove> legalMoves = PBELegalityChecker.GetLegalMoves(_species, _form, _level, Settings);
            var allowed = new List<PBEMove>(legalMoves.Count + 1);
            allowed.AddRange(legalMoves);
            if (_species == PBESpecies.Keldeo && _form == PBEForm.Keldeo_Resolute)
            {
                _list[0].Allowed.Reset(secretSwordArray);
                allowed.Remove(PBEMove.SecretSword);
                i = 1;
            }
            else
            {
                i = 0;
            }
            for (; i < Settings.NumMoves; i++)
            {
                if (i == 1)
                {
                    allowed.Insert(0, PBEMove.None);
                }
                _list[i].Allowed.Reset(allowed);
            }
            // Remove unalloweds (slot.Move setter will automatically sort PBEMove.None)
            while (true)
            {
                int bad = _list.FindIndex(s => !s.Allowed.Contains(s.Move));
                if (bad == -1)
                {
                    break;
                }
                else
                {
                    PBEMovesetSlot slot = _list[bad];
                    slot.Move = slot.Allowed[0];
                }
            }
            SetEditables();
        }
        private void SetEditables()
        {
            for (int i = 2; i < Settings.NumMoves; i++)
            {
                _list[i].IsMoveEditable = _list[i - 1].Move != PBEMove.None;
            }
        }

        private bool _canDispose;
        public bool CanDispose
        {
            get => _canDispose;
            internal set
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
                for (int i = 0; i < _list.Count; i++)
                {
                    _list[i].Allowed.Dispose();
                }
            }
        }

        /// <summary>Sets every move slot excluding the first to <see cref="PBEMove.None"/> with 0 PP-Ups.</summary>
        public void Clear()
        {
            if (IsDisposed)
            {
                throw new ObjectDisposedException(null);
            }
            for (int i = 1; i < Settings.NumMoves; i++)
            {
                _list[i].Move = PBEMove.None;
            }
            SetEditables();
        }
        public bool Contains(PBEMove move)
        {
            if (IsDisposed)
            {
                throw new ObjectDisposedException(null);
            }
            return this[move] != null;
        }
        /// <summary>Randomizes the move and PP-Ups in each slot without creating duplicate moves.</summary>
        public void Randomize()
        {
            if (IsDisposed)
            {
                throw new ObjectDisposedException(null);
            }
            var blacklist = new List<PBEMove>(Settings.NumMoves) { PBEMove.None };
            for (int i = 0; i < Settings.NumMoves; i++)
            {
                PBEMovesetSlot slot = _list[i];
                PBEMove[] allowed = slot.Allowed.Except(blacklist).ToArray();
                if (allowed.Length == 0)
                {
                    for (int j = i; j < Settings.NumMoves; j++)
                    {
                        _list[j].Move = PBEMove.None;
                    }
                    break;
                }
                else
                {
                    PBEMove move = allowed.RandomElement();
                    if (i < Settings.NumMoves - 1)
                    {
                        blacklist.Add(move);
                    }
                    slot.Move = move;
                    slot.PPUps = (byte)PBERandom.RandomInt(0, Settings.MaxPPUps);
                }
            }
            SetEditables();
        }

        public IEnumerator<PBEMovesetSlot> GetEnumerator()
        {
            for (int i = 0; i < _list.Count; i++)
            {
                yield return _list[i];
            }
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        internal void ToBytes(EndianBinaryWriter w)
        {
            byte count = (byte)_list.Count;
            w.Write(count);
            for (int i = 0; i < count; i++)
            {
                PBEMovesetSlot slot = _list[i];
                w.Write(slot.Move);
                w.Write(slot.PPUps);
            }
        }
        internal void ToJson(JsonTextWriter w)
        {
            w.WriteStartArray();
            for (int i = 0; i < _list.Count; i++)
            {
                PBEMovesetSlot slot = _list[i];
                w.WriteStartObject();
                w.WritePropertyName(nameof(PBEMovesetSlot.Move));
                w.WriteValue(slot.Move.ToString());
                w.WritePropertyName(nameof(PBEMovesetSlot.PPUps));
                w.WriteValue(slot.PPUps);
                w.WriteEndObject();
            }
            w.WriteEndArray();
        }
    }
}
