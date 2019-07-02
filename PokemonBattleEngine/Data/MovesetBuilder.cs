using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace Kermalis.PokemonBattleEngine.Data
{
    // TODO: Set settings and listen for changes
    // TODO: IsMoveEditable (if I set slot 3 to none, slot 4 should not be editable)
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

            public PBEReadOnlyObservableCollection<PBEMove> Allowed { get; }
            private PBEMove move;
            public PBEMove Move
            {
                get => move;
                set => parent.Set(slotIndex, value, null);
            }
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
                Allowed = new PBEReadOnlyObservableCollection<PBEMove>();
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
                    PBEPokemonShell.ValidateLevel(value, settings);
                    level = value;
                    OnPropertyChanged(nameof(Level));
                    SetAlloweds();
                }
            }
        }
        private readonly PBESettings settings;
        public ReadOnlyCollection<PBEMoveSlot> MoveSlots { get; }

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
            PBEPokemonShell.ValidateLevel(level, settings);
            PBEPokemonShell.ValidateSpecies(species);
            this.species = species;
            this.level = level;
            this.settings = settings;
            var moves = new PBEMoveSlot[settings.NumMoves];
            for (int i = 0; i < settings.NumMoves; i++)
            {
                moves[i] = new PBEMoveSlot(this, i);
            }
            MoveSlots = new ReadOnlyCollection<PBEMoveSlot>(moves);
            SetAlloweds();
            if (randomize)
            {
                Randomize();
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
            IEnumerable<PBEMove> legalMoves = PBELegalityChecker.GetLegalMoves(species, level);
            for (; i < settings.NumMoves; i++)
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

        /// <summary>Sets every move slot excluding the first to <see cref="PBEMove.None"/> with 0 PP-Ups.</summary>
        public void Clear()
        {
            for (int i = settings.NumMoves - 1; i >= 1; i--)
            {
                MoveSlots[i].Update(PBEMove.None, 0);
            }
        }
        /// <summary>Randomizes the move and PP-Ups in each slot without creating duplicate moves.</summary>
        public void Randomize()
        {
            var blacklist = new List<PBEMove>(settings.NumMoves) { PBEMove.None };
            for (int i = 0; i < settings.NumMoves; i++)
            {
                PBEMoveSlot slot = MoveSlots[i];
                PBEMove[] allowed = slot.Allowed.Except(blacklist).ToArray();
                if (allowed.Length == 0)
                {
                    for (int j = i; j < settings.NumMoves; j++)
                    {
                        MoveSlots[j].Update(PBEMove.None, 0);
                    }
                    break;
                }
                else
                {
                    PBEMove move = allowed.Sample();
                    if (i < settings.NumMoves - 1)
                    {
                        blacklist.Add(move);
                    }
                    slot.Update(move, (byte)PBEUtils.RNG.Next(settings.MaxPPUps + 1));
                }
            }
        }
        /// <summary>Sets a specific move slot's move and/or PP-Ups. If using a for loop to set all values and the moveset is not already cleared, you should call <see cref="Clear"/> first so that move indexes do not move behind your iterator as you change moves.</summary>
        /// <param name="slotIndex">The index of the slot to change.</param>
        /// <param name="move">The move if it needs changing, null if the current move will remain unchanged.</param>
        /// <param name="ppUps">The PP-Ups if it needs changing, null if the current PP-Ups will remain unchanged.</param>
        public void Set(int slotIndex, PBEMove? move, byte? ppUps)
        {
            if (slotIndex < 0 || slotIndex >= settings.NumMoves)
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
                            for (int i = 0; i < settings.NumMoves; i++)
                            {
                                if (i != slotIndex)
                                {
                                    PBEMoveSlot iSlot = MoveSlots[i];
                                    if (iSlot.Move == mVal)
                                    {
                                        // If slot 0 is Snore and slot 3 is None but is trying to become Snore, do nothing because the first Snore is in an earlier slot and swapping None to an earlier slot makes no sense
                                        if (!(old == PBEMove.None && i < slotIndex))
                                        {
                                            UpdateSlot();
                                            iSlot.Update(old, old == PBEMove.None ? 0 : (byte?)null);
                                        }
                                        goto bottom;
                                    }
                                }
                            }
                        }
                        else
                        {
                            // If "move" is None and a slot after "slotIndex" is not None, then place None at the other slot instead and place the other slot's move at "slotIndex"
                            for (int i = settings.NumMoves - 1; i > slotIndex; i--)
                            {
                                PBEMoveSlot iSlot = MoveSlots[i];
                                if (iSlot.Move != PBEMove.None)
                                {
                                    slot.Update(iSlot.Move, null);
                                    iSlot.Update(PBEMove.None, 0);
                                    goto bottom;
                                }
                            }
                        }
                        // This gets reached if:
                        // "move" is not None and there is no other slot with "move"
                        // "move" is None and there is no slot after "slotIndex" with a move
                        UpdateSlot();
                    bottom:
                        ;
                    }
                }
                if (ppUps.HasValue)
                {
                    byte pVal = ppUps.Value;
                    if (slot.PPUps != pVal)
                    {
                        if (pVal > settings.MaxPPUps)
                        {
                            throw new ArgumentOutOfRangeException(nameof(ppUps), $"\"{nameof(ppUps)}\" cannot exceed \"{nameof(settings.MaxPPUps)}\" ({settings.MaxPPUps}).");
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
