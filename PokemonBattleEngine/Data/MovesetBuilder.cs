using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Kermalis.PokemonBattleEngine.Data
{
    public sealed class PBEMovesetBuilder : INotifyPropertyChanged
    {
        public sealed class PBEMoveSlot : INotifyPropertyChanged
        {
            private void OnPropertyChanged(string property)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
            }
            public event PropertyChangedEventHandler PropertyChanged;

            private readonly PBEMovesetBuilder parent;
            private readonly int slotIndex;

            public PBEList<PBEMove> Allowed { get; }
            private PBEMove move;
            public PBEMove Move
            {
                get => move;
                set => parent.Set(slotIndex, value, null);
            }
            public bool IsMoveEditable { get; private set; }
            private byte ppUps;
            public byte PPUps
            {
                get => ppUps;
                set => parent.Set(slotIndex, null, value);
            }
            public bool IsPPUpsEditable => move != PBEMove.None;

            internal PBEMoveSlot(PBEMovesetBuilder parent, int slotIndex)
            {
                this.parent = parent;
                this.slotIndex = slotIndex;
                IsMoveEditable = slotIndex < 2;
                Allowed = new PBEList<PBEMove>() { PBEMove.None };
            }

            internal void Update(PBEMove? move, byte? ppUps)
            {
                if (move.HasValue && this.move != move.Value)
                {
                    PBEMove old = this.move;
                    this.move = move.Value;
                    OnPropertyChanged(nameof(Move));
                    if ((old == PBEMove.None && this.move != PBEMove.None) || (old != PBEMove.None && this.move == PBEMove.None))
                    {
                        OnPropertyChanged(nameof(IsPPUpsEditable));
                    }
                }
                if (ppUps.HasValue && this.ppUps != ppUps.Value)
                {
                    this.ppUps = ppUps.Value;
                    OnPropertyChanged(nameof(PPUps));
                }
            }
            internal void SetEditable(bool value)
            {
                if (value != IsMoveEditable)
                {
                    IsMoveEditable = value;
                    OnPropertyChanged(nameof(IsMoveEditable));
                }
            }
            internal void RemoveIfNotAllowed()
            {
                if (!Allowed.Contains(move))
                {
                    Move = Allowed[0];
                }
            }
            internal void RemoveIfNotAllowedSilent()
            {
                if (!Allowed.Contains(move))
                {
                    PBEMove m = Allowed[0];
                    Update(m, m == PBEMove.None ? 0 : (byte?)null);
                }
            }
        }

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
                if (species != value)
                {
                    PBEPokemonShell.ValidateSpecies(value);
                    species = value;
                    OnPropertyChanged(nameof(Species));
                    SetAlloweds();
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
                    PBEPokemonShell.ValidateLevel(value, Settings);
                    level = value;
                    OnPropertyChanged(nameof(Level));
                    SetAlloweds();
                }
            }
        }
        public PBESettings Settings { get; }
        public PBEList<PBEMoveSlot> MoveSlots { get; }

        /// <summary>Creates a new <see cref="PBEMovesetBuilder"/> object with the specified traits.</summary>
        /// <param name="species">The species of the Pokémon that this moveset will be built for.</param>
        /// <param name="level">The level of the Pokémon that this moveset will be built for.</param>
        /// <param name="settings">The settings that will be used to evaluate the <see cref="PBEMovesetBuilder"/>.</param>
        /// <param name="randomize">True if <see cref="Randomize"/> should be called, False if the move slots use their default values.</param>
        public PBEMovesetBuilder(PBESpecies species, byte level, PBESettings settings, bool randomize)
        {
            if (settings == null)
            {
                throw new ArgumentNullException(nameof(settings));
            }
            Settings = settings;
            Settings.PropertyChanged += OnSettingsChanged;
            PBEPokemonShell.ValidateLevel(level, Settings);
            this.level = level;
            PBEPokemonShell.ValidateSpecies(species);
            this.species = species;
            MoveSlots = new PBEList<PBEMoveSlot>(Settings.NumMoves);
            for (int i = 0; i < Settings.NumMoves; i++)
            {
                MoveSlots.Add(new PBEMoveSlot(this, i));
            }
            SetAlloweds();
            if (randomize)
            {
                Randomize();
            }
        }

        private void OnSettingsChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(Settings.MaxLevel):
                {
                    if (level > Settings.MaxLevel)
                    {
                        Level = Settings.MaxLevel;
                    }
                    break;
                }
                case nameof(Settings.MaxPPUps):
                {
                    for (int i = 0; i < Settings.NumMoves; i++)
                    {
                        PBEMoveSlot slot = MoveSlots[i];
                        if (slot.PPUps > Settings.MaxPPUps)
                        {
                            slot.Update(null, Settings.MaxPPUps);
                        }
                    }
                    break;
                }
                case nameof(Settings.MinLevel):
                {
                    if (level < Settings.MinLevel)
                    {
                        Level = Settings.MinLevel;
                    }
                    break;
                }
                case nameof(Settings.NumMoves):
                {
                    int oldCount = MoveSlots.Count;
                    if (Settings.NumMoves != oldCount)
                    {
                        if (Settings.NumMoves > oldCount)
                        {
                            int numToAdd = Settings.NumMoves - oldCount;
                            for (int i = 0; i < numToAdd; i++)
                            {
                                MoveSlots.Add(new PBEMoveSlot(this, oldCount + i));
                            }
                        }
                        else
                        {
                            int numToRemove = oldCount - Settings.NumMoves;
                            for (int i = 0; i < numToRemove; i++)
                            {
                                MoveSlots.RemoveAt(oldCount - 1 - i);
                            }
                        }
                        SetAlloweds();
                        SetEditables();
                    }
                    break;
                }
            }
        }

        private void SetAlloweds()
        {
            int i;
            if (species == PBESpecies.Keldeo_Resolute)
            {
                PBEMoveSlot slot = MoveSlots[0];
                slot.Allowed.Reset(new[] { PBEMove.SecretSword });
                slot.RemoveIfNotAllowedSilent();
                i = 1;
            }
            else
            {
                i = 0;
            }
            // Every move slot except the first will have the same allowed pool, so there is no need to check for PBEMove.None between them.
            IEnumerable<PBEMove> legalMoves = PBELegalityChecker.GetLegalMoves(species, level);
            for (; i < Settings.NumMoves; i++)
            {
                PBEMoveSlot slot = MoveSlots[i];
                var allowed = new List<PBEMove>(legalMoves);
                if (i != 0)
                {
                    allowed.Insert(0, PBEMove.None);
                }
                if (species == PBESpecies.Keldeo_Resolute)
                {
                    allowed.Remove(PBEMove.SecretSword);
                }
                slot.Allowed.Reset(allowed);
                slot.RemoveIfNotAllowedSilent();
            }
        }
        private void SetEditables()
        {
            for (int i = 2; i < Settings.NumMoves; i++)
            {
                MoveSlots[i].SetEditable(MoveSlots[i - 1].Move != PBEMove.None);
            }
        }

        /// <summary>Sets every move slot excluding the first to <see cref="PBEMove.None"/> with 0 PP-Ups.</summary>
        public void Clear()
        {
            for (int i = 1; i < Settings.NumMoves; i++)
            {
                MoveSlots[i].Update(PBEMove.None, 0);
            }
            SetEditables();
        }
        /// <summary>Randomizes the move and PP-Ups in each slot without creating duplicate moves.</summary>
        public void Randomize()
        {
            var blacklist = new List<PBEMove>(Settings.NumMoves) { PBEMove.None };
            for (int i = 0; i < Settings.NumMoves; i++)
            {
                PBEMoveSlot slot = MoveSlots[i];
                PBEMove[] allowed = slot.Allowed.Except(blacklist).ToArray();
                if (allowed.Length == 0)
                {
                    for (int j = i; j < Settings.NumMoves; j++)
                    {
                        MoveSlots[j].Update(PBEMove.None, 0);
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
                    slot.Update(move, (byte)PBEUtils.RandomInt(0, Settings.MaxPPUps));
                }
            }
            SetEditables();
        }
        /// <summary>Sets a specific move slot's move and/or PP-Ups. If using a for loop to set all values and the moveset is not already cleared, you should call <see cref="Clear"/> first so that move indexes do not move behind your iterator as you change moves.</summary>
        /// <param name="slotIndex">The index of the slot to change.</param>
        /// <param name="move">The move if it needs changing, null if the current move will remain unchanged.</param>
        /// <param name="ppUps">The PP-Ups if it needs changing, null if the current PP-Ups will remain unchanged.</param>
        public void Set(int slotIndex, PBEMove? move, byte? ppUps)
        {
            if (slotIndex < 0 || slotIndex >= Settings.NumMoves)
            {
                throw new ArgumentOutOfRangeException(nameof(slotIndex));
            }
            if (move.HasValue || ppUps.HasValue)
            {
                PBEMoveSlot slot = MoveSlots[slotIndex];
                if (move.HasValue)
                {
                    PBEMove mVal = move.Value;
                    PBEMove old = slot.Move;
                    if (old != mVal)
                    {
                        if (!Enum.IsDefined(typeof(PBEMove), move))
                        {
                            throw new ArgumentOutOfRangeException(nameof(move));
                        }
                        if (!slot.IsMoveEditable)
                        {
                            throw new InvalidOperationException($"Slot {slotIndex}'s move cannot be changed because there is no move in slot {slotIndex - 1}.");
                        }
                        if (!slot.Allowed.Contains(mVal))
                        {
                            throw new ArgumentOutOfRangeException(nameof(move), $"Slot {slotIndex} does not allow {mVal}.");
                        }
                        void UpdateSlot()
                        {
                            slot.Update(mVal, mVal == PBEMove.None ? 0 : (byte?)null);
                        }
                        if (mVal != PBEMove.None)
                        {
                            // If "move" is in another slot, place "slotIndex"'s old move at the other slot
                            for (int i = 0; i < Settings.NumMoves; i++)
                            {
                                if (i != slotIndex)
                                {
                                    PBEMoveSlot iSlot = MoveSlots[i];
                                    if (iSlot.Move == mVal)
                                    {
                                        // If slot 0 is Snore and slot 3 is None but is trying to become Snore, do nothing because the first Snore is in an earlier slot and swapping None to an earlier slot makes no sense
                                        if (old == PBEMove.None && i < slotIndex)
                                        {
                                            goto finish;
                                        }
                                        else
                                        {
                                            UpdateSlot();
                                            iSlot.Update(old, old == PBEMove.None ? 0 : (byte?)null);
                                            goto editables;
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            // If "move" is None and a slot after "slotIndex" is not None, then place None at the other slot instead and place the other slot's move at "slotIndex"
                            for (int i = Settings.NumMoves - 1; i > slotIndex; i--)
                            {
                                PBEMoveSlot iSlot = MoveSlots[i];
                                if (iSlot.Move != PBEMove.None)
                                {
                                    slot.Update(iSlot.Move, null);
                                    iSlot.Update(PBEMove.None, 0);
                                    goto editables;
                                }
                            }
                        }
                        // This gets reached if:
                        // "move" is not None and there is no other slot with "move"
                        // "move" is None and there is no slot after "slotIndex" with a move
                        UpdateSlot();
                    editables:
                        SetEditables();
                    finish:
                        ;
                    }
                }
                if (ppUps.HasValue)
                {
                    byte pVal = ppUps.Value;
                    if (slot.PPUps != pVal)
                    {
                        if (pVal > Settings.MaxPPUps)
                        {
                            throw new ArgumentOutOfRangeException(nameof(ppUps), $"\"{nameof(ppUps)}\" cannot exceed \"{nameof(Settings.MaxPPUps)}\" ({Settings.MaxPPUps}).");
                        }
                        if (!slot.IsPPUpsEditable)
                        {
                            throw new InvalidOperationException($"Slot {slotIndex}'s PP-Ups cannot be changed because it has no move.");
                        }
                        slot.Update(null, pVal);
                    }
                }
            }
            else
            {
                throw new ArgumentException($"\"{nameof(move)}\" or \"{nameof(ppUps)}\" has to have a value to set.");
            }
        }
    }
}
