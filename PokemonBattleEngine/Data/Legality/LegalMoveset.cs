using Kermalis.PokemonBattleEngine.Data.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;

namespace Kermalis.PokemonBattleEngine.Data.Legality
{
    public sealed class PBELegalMoveset : IPBEMoveset, IPBEMoveset<PBELegalMoveset.PBELegalMovesetSlot>, INotifyPropertyChanged
    {
        public sealed class PBELegalMovesetSlot : IPBEMovesetSlot, INotifyPropertyChanged
        {
            private void OnPropertyChanged(string property)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
            }
            public event PropertyChangedEventHandler? PropertyChanged;

            private readonly PBELegalMoveset _parent;
            private readonly int _index;

            public PBEAlphabeticalList<PBEMove> Allowed { get; }
            private PBEMove _move;
            public PBEMove Move
            {
                get => _move;
                set
                {
                    PBEMove old = _move;
                    if (old != value)
                    {
                        if (value >= PBEMove.MAX || (value != PBEMove.None && !PBEDataUtils.IsMoveUsable(value)))
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
                                    PBELegalMovesetSlot iSlot = _parent[i];
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
                                PBELegalMovesetSlot iSlot = _parent[i];
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

            private static readonly PBEMove[] _none = new PBEMove[1] { PBEMove.None };
            internal PBELegalMovesetSlot(PBELegalMoveset parent, int index)
            {
                _parent = parent;
                _index = index;
                _isMoveEditable = index < 2;
                Allowed = new PBEAlphabeticalList<PBEMove>(_none);
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

        private void OnPropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }
        public event PropertyChangedEventHandler? PropertyChanged;

        private PBESpecies _species;
        public PBESpecies Species
        {
            get => _species;
            set
            {
                if (_species != value)
                {
                    PBEDataUtils.ValidateSpecies(value, 0, true);
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
                if (_form != value)
                {
                    PBEDataUtils.ValidateSpecies(_species, value, true);
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
                if (_level != value)
                {
                    PBEDataUtils.ValidateLevel(value, Settings);
                    _level = value;
                    OnPropertyChanged(nameof(Level));
                    SetAlloweds();
                }
            }
        }
        public PBESettings Settings { get; }
        private readonly PBELegalMovesetSlot[] _list;
        public int Count => Settings.NumMoves;
        public PBELegalMovesetSlot this[int index]
        {
            get
            {
                if (index >= Settings.NumMoves)
                {
                    throw new ArgumentOutOfRangeException(nameof(index));
                }
                return _list[index];
            }
        }
        IPBEMovesetSlot IReadOnlyList<IPBEMovesetSlot>.this[int index] => this[index];
        public PBELegalMovesetSlot? this[PBEMove move]
        {
            get
            {
                for (int i = 0; i < Settings.NumMoves; i++)
                {
                    PBELegalMovesetSlot slot = _list[i];
                    if (slot.Move == move)
                    {
                        return slot;
                    }
                }
                return null;
            }
        }

        internal PBELegalMoveset(PBELegalMoveset other)
        {
            _species = other._species;
            _form = other._form;
            _level = other._level;
            Settings = other.Settings;
            int count = Settings.NumMoves;
            _list = new PBELegalMovesetSlot[count];
            for (int i = 0; i < count; i++)
            {
                _list[i] = new PBELegalMovesetSlot(this, i);
            }
            SetAlloweds();
            for (int i = 0; i < count; i++)
            {
                PBELegalMovesetSlot slot = _list[i];
                PBELegalMovesetSlot oSlot = other[i];
                slot.Move = oSlot.Move;
                slot.PPUps = oSlot.PPUps;
            }
        }
        internal PBELegalMoveset(PBESpecies species, PBEForm form, byte level, PBESettings settings, IPBEMoveset other)
        {
            int count = other.Count;
            if (count != settings.NumMoves)
            {
                throw new InvalidDataException($"Moveset count must be equal to \"{nameof(settings.NumMoves)}\" ({settings.NumMoves}).");
            }
            _species = species;
            _form = form;
            _level = level;
            Settings = settings;
            _list = new PBELegalMovesetSlot[count];
            for (int i = 0; i < count; i++)
            {
                _list[i] = new PBELegalMovesetSlot(this, i);
            }
            SetAlloweds();
            for (int i = 0; i < count; i++)
            {
                IPBEMovesetSlot oSlot = other[i];
                PBELegalMovesetSlot slot = _list[i];
                PBEMove move = oSlot.Move;
                slot.Move = move;
                if (slot.Move != move)
                {
                    throw new ArgumentOutOfRangeException(nameof(IPBEPokemon.Moveset), "Invalid moves.");
                }
                byte ppUps = oSlot.PPUps;
                slot.PPUps = ppUps;
                if (slot.PPUps != ppUps)
                {
                    throw new ArgumentOutOfRangeException(nameof(IPBEPokemon.Moveset), "Invalid PP-Ups.");
                }
            }
        }
        /// <summary>Creates a new <see cref="PBELegalMoveset"/> object with the specified traits.</summary>
        /// <param name="species">The species of the Pokémon that this moveset will be built for.</param>
        /// <param name="form">The form of the Pokémon that this moveset will be built for.</param>
        /// <param name="level">The level of the Pokémon that this moveset will be built for.</param>
        /// <param name="settings">The settings that will be used to evaluate the <see cref="PBELegalMoveset"/>.</param>
        /// <param name="randomize">True if <see cref="Randomize"/> should be called, False if the move slots use their default values.</param>
        public PBELegalMoveset(PBESpecies species, PBEForm form, byte level, PBESettings settings, bool randomize)
        {
            settings.ShouldBeReadOnly(nameof(settings));
            PBEDataUtils.ValidateSpecies(species, form, true);
            PBEDataUtils.ValidateLevel(level, settings);
            _level = level;
            _species = species;
            _form = form;
            Settings = settings;
            int count = settings.NumMoves;
            _list = new PBELegalMovesetSlot[count];
            for (int i = 0; i < count; i++)
            {
                _list[i] = new PBELegalMovesetSlot(this, i);
            }
            SetAlloweds();
            if (randomize)
            {
                Randomize();
            }
        }

        private static readonly PBEAlphabeticalList<PBEMove> secretSwordArray = new(new PBEMove[1] { PBEMove.SecretSword });
        private void SetAlloweds()
        {
            // Set alloweds
            int i;
            IReadOnlyCollection<PBEMove> legalMoves = PBEDataProvider.Instance.GetLegalMoves(_species, _form, _level);
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
                int bad = Array.FindIndex(_list, s => !s.Allowed.Contains(s.Move));
                if (bad == -1)
                {
                    break;
                }
                else
                {
                    PBELegalMovesetSlot slot = _list[bad];
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

        /// <summary>Sets every move slot excluding the first to <see cref="PBEMove.None"/> with 0 PP-Ups.</summary>
        public void Clear()
        {
            for (int i = 1; i < Settings.NumMoves; i++)
            {
                _list[i].Move = PBEMove.None;
            }
            SetEditables();
        }
        public bool Contains(PBEMove move)
        {
            return this[move] is not null;
        }
        /// <summary>Randomizes the move and PP-Ups in each slot without creating duplicate moves.</summary>
        public void Randomize()
        {
            var blacklist = new List<PBEMove>(Settings.NumMoves) { PBEMove.None };
            for (int i = 0; i < Settings.NumMoves; i++)
            {
                PBELegalMovesetSlot slot = _list[i];
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
                    PBEMove move = PBEDataProvider.GlobalRandom.RandomElement(allowed);
                    if (i < Settings.NumMoves - 1)
                    {
                        blacklist.Add(move);
                    }
                    slot.Move = move;
                    slot.PPUps = (byte)PBEDataProvider.GlobalRandom.RandomInt(0, Settings.MaxPPUps);
                }
            }
            SetEditables();
        }

        public IEnumerator<PBELegalMovesetSlot> GetEnumerator()
        {
            for (int i = 0; i < Settings.NumMoves; i++)
            {
                yield return _list[i];
            }
        }
        IEnumerator<IPBEMovesetSlot> IEnumerable<IPBEMovesetSlot>.GetEnumerator()
        {
            return GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
