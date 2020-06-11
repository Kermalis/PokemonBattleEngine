using Kermalis.PokemonBattleEngine.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Kermalis.PokemonBattleEngine.Battle
{
    public sealed class PBEBattleMoveset : IReadOnlyList<PBEBattleMoveset.PBEBattleMovesetSlot>
    {
        public sealed class PBEBattleMovesetSlot : INotifyPropertyChanged
        {
            private void OnPropertyChanged(string property)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
            }
            public event PropertyChangedEventHandler PropertyChanged;

            private PBEMove _move;
            public PBEMove Move
            {
                get => _move;
                set
                {
                    if (_move != value)
                    {
                        _move = value;
                        OnPropertyChanged(nameof(Move));
                    }
                }
            }
            private int _pp;
            public int PP
            {
                get => _pp;
                set
                {
                    if (_pp != value)
                    {
                        _pp = value;
                        OnPropertyChanged(nameof(PP));
                    }
                }
            }
            private int _maxPP;
            public int MaxPP
            {
                get => _maxPP;
                set
                {
                    if (_maxPP != value)
                    {
                        _maxPP = value;
                        OnPropertyChanged(nameof(MaxPP));
                    }
                }
            }

            internal PBEBattleMovesetSlot()
            {
                _move = PBEMove.MAX;
            }
            internal PBEBattleMovesetSlot(PBEMove move, int pp, int maxPP)
            {
                _move = move;
                _pp = pp;
                _maxPP = maxPP;
            }
        }

        private readonly PBEBattleMovesetSlot[] _list;
        public int Count => _list.Length;
        public PBEBattleMovesetSlot this[int index]
        {
            get
            {
                if (index >= _list.Length)
                {
                    throw new ArgumentOutOfRangeException(nameof(index));
                }
                return _list[index];
            }
        }
        public PBEBattleMovesetSlot this[PBEMove move]
        {
            get
            {
                for (int i = 0; i < _list.Length; i++)
                {
                    PBEBattleMovesetSlot slot = _list[i];
                    if (slot.Move == move)
                    {
                        return slot;
                    }
                }
                return null;
            }
        }

        internal PBEBattleMoveset(PBESettings settings)
        {
            _list = new PBEBattleMovesetSlot[settings.NumMoves];
            for (int i = 0; i < _list.Length; i++)
            {
                _list[i] = new PBEBattleMovesetSlot();
            }
        }
        internal PBEBattleMoveset(PBEMoveset moveset)
        {
            _list = new PBEBattleMovesetSlot[moveset.Settings.NumMoves];
            for (int i = 0; i < _list.Length; i++)
            {
                PBEMoveset.PBEMovesetSlot slot = moveset[i];
                PBEMove move = slot.Move;
                int pp;
                if (move != PBEMove.None)
                {
                    byte tier = PBEMoveData.Data[move].PPTier;
                    pp = Math.Max(1, (tier * moveset.Settings.PPMultiplier) + (tier * slot.PPUps));
                }
                else
                {
                    pp = 0;
                }
                _list[i] = new PBEBattleMovesetSlot(move, pp, pp);
            }
        }
        internal PBEBattleMoveset(PBEBattleMoveset other)
        {
            _list = new PBEBattleMovesetSlot[other._list.Length];
            for (int i = 0; i < _list.Length; i++)
            {
                PBEBattleMovesetSlot oSlot = other._list[i];
                _list[i] = new PBEBattleMovesetSlot(oSlot.Move, oSlot.PP, oSlot.MaxPP);
            }
        }

        public static int GetTransformPP(PBESettings settings, PBEMove move)
        {
            if (settings == null)
            {
                throw new ArgumentNullException(nameof(settings));
            }
            else if (move == PBEMove.None)
            {
                return 0;
            }
            else if (move >= PBEMove.MAX || !PBEMoveData.IsMoveUsable(move))
            {
                throw new ArgumentOutOfRangeException(nameof(move));
            }
            else
            {
                return PBEMoveData.Data[move].PPTier == 0 ? 1 : settings.PPMultiplier;
            }
        }
        public static int GetNonTransformPP(PBESettings settings, PBEMove move, byte ppUps)
        {
            if (settings == null)
            {
                throw new ArgumentNullException(nameof(settings));
            }
            else if (move == PBEMove.None)
            {
                return 0;
            }
            else if (move >= PBEMove.MAX || !PBEMoveData.IsMoveUsable(move))
            {
                throw new ArgumentOutOfRangeException(nameof(move));
            }
            else
            {
                byte tier = PBEMoveData.Data[move].PPTier;
                return Math.Max(1, (tier * settings.PPMultiplier) + (tier * ppUps));
            }
        }

        internal static void DoTransform(PBEPokemon user, PBEPokemon target)
        {
            PBEBattleMoveset targetKnownBackup = null;
            if (user.Team != target.Team)
            {
                targetKnownBackup = new PBEBattleMoveset(target.KnownMoves);
            }
            PBESettings settings = user.Team.Battle.Settings;
            for (int i = 0; i < settings.NumMoves; i++)
            {
                PBEBattleMovesetSlot userMove = user.Moves._list[i];
                PBEBattleMovesetSlot userKnownMove = user.KnownMoves._list[i];
                PBEBattleMovesetSlot targetMove = target.Moves._list[i];
                PBEBattleMovesetSlot targetKnownMove = target.KnownMoves._list[i];
                PBEMove move;
                int pp;
                if (user.Team == target.Team)
                {
                    move = targetMove.Move;
                    pp = move == PBEMove.MAX ? 0 : GetTransformPP(settings, move);
                    userMove.Move = move;
                    userMove.PP = pp;
                    userMove.MaxPP = pp;

                    move = targetKnownMove.Move;
                    pp = move == PBEMove.MAX ? 0 : GetTransformPP(settings, move);
                    userKnownMove.Move = move;
                    userKnownMove.PP = 0;
                    userKnownMove.MaxPP = pp;
                }
                else
                {
                    move = targetMove.Move;
                    pp = move == PBEMove.MAX ? 0 : GetTransformPP(settings, move);
                    userMove.Move = move;
                    userMove.PP = pp;
                    userMove.MaxPP = pp;
                    targetKnownMove.Move = move;
                    PBEBattleMovesetSlot bSlot = targetKnownBackup[move];
                    if (bSlot == null)
                    {
                        targetKnownMove.PP = 0;
                        targetKnownMove.MaxPP = 0;
                    }
                    else
                    {
                        targetKnownMove.PP = bSlot.PP;
                        targetKnownMove.MaxPP = bSlot.MaxPP;
                    }
                    userKnownMove.Move = move;
                    userKnownMove.PP = 0;
                    userKnownMove.MaxPP = pp;
                }
            }
        }
        internal ReadOnlyCollection<PBEMove> ForTransformPacket()
        {
            var moves = new PBEMove[_list.Length];
            for (int i = 0; i < _list.Length; i++)
            {
                moves[i] = _list[i].Move;
            }
            return new ReadOnlyCollection<PBEMove>(moves);
        }
        internal void Reset(PBEBattleMoveset other)
        {
            for (int i = 0; i < _list.Length; i++)
            {
                PBEBattleMovesetSlot slot = _list[i];
                PBEBattleMovesetSlot oSlot = other._list[i];
                slot.Move = oSlot.Move;
                slot.PP = oSlot.PP;
                slot.MaxPP = oSlot.MaxPP;
            }
        }
        internal void SetUnknown()
        {
            for (int i = 0; i < _list.Length; i++)
            {
                PBEBattleMovesetSlot slot = _list[i];
                slot.Move = PBEMove.MAX;
                slot.PP = 0;
                slot.MaxPP = 0;
            }
        }

        public bool Contains(PBEMove move)
        {
            return this[move] != null;
        }

        public IEnumerator<PBEBattleMovesetSlot> GetEnumerator()
        {
            for (int i = 0; i < _list.Length; i++)
            {
                yield return _list[i];
            }
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
