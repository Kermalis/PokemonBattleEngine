using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace Kermalis.PokemonBattleEngine.Data
{
    // TODO: Listen for changes to settings
    // TODO: Cannot set slot 3 if slot 2 is none, and cannot clear slot 2 if slot 3 is not none
    public sealed class PBEMovesetBuilder
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

            public PBEReadOnlyObservableCollection<PBEMove> Allowed { get; private set; }
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
            }

            // TODO: Only fire if changed?
            internal void Update(List<PBEMove> allowed, PBEMove? move, byte? ppUps)
            {
                if (allowed != null)
                {
                    Allowed = new PBEReadOnlyObservableCollection<PBEMove>(allowed);
                    OnPropertyChanged(nameof(Allowed));
                }
                if (move.HasValue)
                {
                    this.move = move.Value;
                    OnPropertyChanged(nameof(Move));
                    OnPropertyChanged(nameof(IsPPUpsEditable));
                }
                if (ppUps.HasValue)
                {
                    this.ppUps = ppUps.Value;
                    OnPropertyChanged(nameof(PPUps));
                }
            }
        }

        private readonly PBESpecies species;
        private readonly PBESettings settings;
        private readonly IEnumerable<PBEMove> legalMoves;
        public ReadOnlyCollection<PBEMoveSlot> MoveSlots { get; }

        public PBEMovesetBuilder(PBESpecies species, byte level, PBESettings settings)
        {
            this.species = species;
            this.settings = settings;
            legalMoves = PBELegalityChecker.GetLegalMoves(species, level);
            var moves = new PBEMoveSlot[settings.NumMoves];
            for (int i = 0; i < settings.NumMoves; i++)
            {
                moves[i] = new PBEMoveSlot(this, i);
            }
            MoveSlots = new ReadOnlyCollection<PBEMoveSlot>(moves);
            Randomize();
        }

        public void Randomize()
        {
            byte MakePP()
            {
                return (byte)PBEUtils.RNG.Next(settings.MaxPPUps + 1);
            }

            int i;
            var used = new List<PBEMove>();
            if (species == PBESpecies.Keldeo_Resolute)
            {
                used.Add(PBEMove.SecretSword);
                MoveSlots[0].Update(new List<PBEMove>() { PBEMove.SecretSword }, PBEMove.SecretSword, MakePP());
                i = 1;
            }
            else
            {
                i = 0;
            }
            for (; i < settings.NumMoves; i++)
            {
                var allowed = new List<PBEMove>(legalMoves.Except(used));
                bool none = allowed.Count == 0;
                PBEMove move;
                byte ppUps;
                if (none)
                {
                    move = PBEMove.None;
                    ppUps = 0;
                }
                else
                {
                    move = allowed.Sample();
                    ppUps = MakePP();
                    used.Add(move);
                    for (int j = 0; j < i; j++)
                    {
                        MoveSlots[j].Allowed.Remove(move);
                    }
                }
                if (i != 0)
                {
                    allowed.Insert(0, PBEMove.None);
                }
                MoveSlots[i].Update(allowed, move, ppUps);
            }
        }

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
                        slot.Update(null, mVal, mVal == PBEMove.None ? 0 : (byte?)null);
                        for (int i = 0; i < settings.NumMoves; i++)
                        {
                            if (i != slotIndex)
                            {
                                PBEReadOnlyObservableCollection<PBEMove> a = MoveSlots[i].Allowed;
                                if (mVal != PBEMove.None)
                                {
                                    a.Remove(mVal);
                                }
                                if (old != PBEMove.None && !(species == PBESpecies.Keldeo_Resolute && i == 0))
                                {
                                    a.Add(old);
                                }
                            }
                        }
                    }
                }
                if (ppUps.HasValue)
                {
                    byte pVal = ppUps.Value;
                    if (slot.PPUps != pVal)
                    {
                        if (pVal > settings.MaxPPUps)
                        {
                            throw new ArgumentOutOfRangeException(nameof(ppUps), $"\"{nameof(ppUps)}\" cannot exceed {settings.MaxPPUps}.");
                        }
                        if (!slot.IsPPUpsEditable)
                        {
                            throw new InvalidOperationException($"Slot {slotIndex}'s PP-Ups cannot be changed because it has no move.");
                        }
                        slot.Update(null, null, pVal);
                    }
                }
            }
            else
            {
                throw new ArgumentException($"\"{nameof(move)}\" or \"{nameof(ppUps)}\" has to have a value to set.");
            }
        }
        // This is just temporary. If I don't use it, then trying to set a slot to a move that is in another slot would crash, and there is a high chance since this starts with random moves
        public void Clear()
        {
            for (int i = settings.NumMoves - 1; i >= 1; i--)
            {
                Set(i, PBEMove.None, null);
            }
        }
    }
}
