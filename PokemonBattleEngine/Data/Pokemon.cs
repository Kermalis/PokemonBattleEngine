using Kermalis.PokemonBattleEngine.Util;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Kermalis.PokemonBattleEngine.Data
{
    public struct PokemonShell
    {
        public Species Species;
        public byte Level;
        public Ability Ability;
        public Nature Nature;
        public Item Item;
        public byte[] EVs, IVs;
        public Move[] Moves;

        public static void ValidateMany(IEnumerable<PokemonShell> shells)
        {
            var arr = shells.ToArray();
            for (int i = 0; i < arr.Length; i++)
            {
                arr[i].Validate();
            }
        }
        public void Validate()
        {
            // Validate Species
            PokemonData pData;
            try
            {
                pData = PokemonData.Data[Species];
            }
            catch
            {
                throw new ArgumentOutOfRangeException("Species");
            }

            // Validate Level
            // Level being 0 will force the level to be chosen in the Pokemon class
            if (Level > Constants.MaxLevel)
                throw new ArgumentOutOfRangeException("Level");

            // Validate Ability
            // Ability.None will force the ability to be chosen in the Pokemon class
            if (Ability != Ability.None && Ability != pData.Ability1 && Ability != pData.Ability2 && Ability != pData.AbilityHidden)
                throw new ArgumentOutOfRangeException("Ability");

            // Nature being >= Nature.MAX will force the nature to be chosen in the Pokemon class

            // Validate Item
            if (Item != Item.None)
            {
                try
                {
                    var iData = ItemData.Data[Item];
                }
                catch
                {
                    throw new ArgumentOutOfRangeException("Item");
                }
            }

            // Validate EVs & IVs
            if (EVs != null)
            {
                if (EVs.Length != 6)
                    throw new ArgumentOutOfRangeException("EVs");
                int total = 0;
                for (int i = 0; i < 6; i++)
                {
                    total += EVs[i];
                    if (EVs[i] > 252 || total > 510)
                        throw new ArgumentOutOfRangeException("EVs");
                }
            }
            if (IVs != null)
            {
                if (IVs.Length != 6)
                    throw new ArgumentOutOfRangeException("IVs");
                if (IVs.Any(e => e > 31))
                    throw new ArgumentOutOfRangeException("IVs");
            }

            // Validate Moves
            // Moves being null will force the moves to be chosen in the Pokemon class
            if (Moves != null)
            {
                if (Moves.Length >= Constants.NumMoves)
                    throw new ArgumentOutOfRangeException("Moves");

                // TODO: All moves other than level-up
                IEnumerable<Move> legalMoves = pData.LevelUpMoves.Select(t => t.Item2);
                if (Moves.Any(m => !legalMoves.Contains(m)))
                    throw new ArgumentOutOfRangeException("Moves");
            }
        }
    }

    internal class Pokemon
    {
        public readonly Species Species;
        public readonly Gender Gender;
        public readonly Nature Nature;
        public ushort HP, MaxHP;
        public ushort Attack, Defense, SpAttack, SpDefense, Speed;
        public byte Level;
        public Status Status;
        public Ability Ability;
        public Item Item;

        public byte[] EVs = new byte[6], IVs = new byte[6];
        public Move[] Moves = new Move[Constants.NumMoves];
        public byte[] PP = new byte[Constants.NumMoves];

        public Pokemon(PokemonShell shell)
        {
            Species = shell.Species;
            PokemonData pData = PokemonData.Data[Species];

            Gender = GetRandomGenderForSpecies(Species);

            Ability = shell.Ability;
            if (Ability == Ability.None)
            {
                // TODO: Ability generation
                Ability = pData.Ability1; // Temporarily use first ability if expected to generate one
            }

            if (shell.Level == 0)
                Level = Constants.MaxLevel;
            else
                Level = shell.Level;

            Item = shell.Item;

            if (shell.EVs != null)
                for (int i = 0; i < 6; i++)
                    EVs[i] = shell.EVs[i]; // Copy EVs
            if (shell.IVs != null)
                for (int i = 0; i < 6; i++)
                    IVs[i] = shell.IVs[i]; // Copy IVs
            else
                for (int i = 0; i < 6; i++)
                    IVs[i] = (byte)Utils.RNG.Next(0, 32); // Randomly assign IVs

            if (shell.Moves == null)
                SetDefaultMovesForCurrentLevel();
            else
                SetMovesAndPP(shell.Moves);

            if (shell.Nature >= Nature.MAX)
                Nature = (Nature)Utils.RNG.Next(0, (int)Nature.MAX);

            CalculateStats();
            HP = MaxHP;
        }

        static readonly Dictionary<Nature, sbyte[]> NatureStatTable = new Dictionary<Nature, sbyte[]>
        {
            //                               Atk   Def SpAtk SpDef   Spd
            { Nature.Adamant, new sbyte[] {   +1,    0,   -1,    0,    0} },
            { Nature.Bashful, new sbyte[] {    0,    0,    0,    0,    0} },
            { Nature.Bold,    new sbyte[] {   -1,   +1,    0,    0,    0} },
            { Nature.Brave,   new sbyte[] {   +1,    0,    0,    0,   -1} },
            { Nature.Calm,    new sbyte[] {   -1,    0,    0,   +1,    0} },
            { Nature.Careful, new sbyte[] {    0,    0,   -1,   +1,    0} },
            { Nature.Docile,  new sbyte[] {    0,    0,    0,    0,    0} },
            { Nature.Gentle,  new sbyte[] {    0,   -1,    0,   +1,    0} },
            { Nature.Hardy,   new sbyte[] {    0,    0,    0,    0,    0} },
            { Nature.Hasty,   new sbyte[] {    0,   -1,    0,    0,   +1} },
            { Nature.Impish,  new sbyte[] {    0,   +1,   -1,    0,    0} },
            { Nature.Jolly,   new sbyte[] {    0,    0,   -1,    0,   +1} },
            { Nature.Lax,     new sbyte[] {    0,   +1,    0,   -1,    0} },
            { Nature.Loney,   new sbyte[] {   +1,   -1,    0,    0,    0} },
            { Nature.Mild,    new sbyte[] {    0,   -1,   +1,    0,    0} },
            { Nature.Modest,  new sbyte[] {   -1,    0,   +1,    0,    0} },
            { Nature.Naive,   new sbyte[] {    0,    0,    0,   -1,   +1} },
            { Nature.Naughty, new sbyte[] {   +1,    0,    0,   -1,    0} },
            { Nature.Quiet,   new sbyte[] {    0,    0,   +1,    0,   -1} },
            { Nature.Quirky,  new sbyte[] {    0,    0,    0,    0,    0} },
            { Nature.Rash,    new sbyte[] {    0,    0,   +1,   -1,    0} },
            { Nature.Relaxed, new sbyte[] {    0,   +1,    0,    0,   -1} },
            { Nature.Sassy,   new sbyte[] {    0,    0,    0,   +1,   -1} },
            { Nature.Serious, new sbyte[] {    0,    0,    0,    0,    0} },
            { Nature.Timid,   new sbyte[] {   -1,    0,    0,    0,   +1} },
        };
        void CalculateStats()
        {
            PokemonData pData = PokemonData.Data[Species];

            MaxHP = (ushort)(((2 * pData.HP + IVs[0] + (EVs[0] / 4)) * Level / Constants.MaxLevel) + Level + 10);

            int i = 0;
            ushort OtherStat(byte baseVal)
            {
                double natureMultiplier = 1 + (NatureStatTable[Nature][i] * 0.1);
                ushort val = (ushort)((((2 * baseVal + IVs[i + 1] + (EVs[i + 1] / 4)) * Level / Constants.MaxLevel) + 5) * natureMultiplier);
                i++;
                return val;
            }
            Attack = OtherStat(pData.Attack);
            Defense = OtherStat(pData.Defense);
            SpAttack = OtherStat(pData.SpAttack);
            SpDefense = OtherStat(pData.SpDefense);
            Speed = OtherStat(pData.Speed);
        }

        void SetDefaultMovesForCurrentLevel()
        {
            PokemonData pData = PokemonData.Data[Species];

            Move[] learnedMoves = pData.LevelUpMoves.Where(t => t.Item1 <= Level).Select(t => t.Item2).ToArray();
            SetMovesAndPP(learnedMoves);
        }
        void SetMovesAndPP(Move[] moves)
        {
            int min = Math.Min(Constants.NumMoves, moves.Length);
            // Copy last "min" amount of moves from the array
            for (int i = min - 1; i >= 0; i--)
            {
                Move newMove = moves[i];
                Moves[i] = newMove;
                PP[i] = MoveData.Data[newMove].PP;
            }
        }

        static Gender GetRandomGenderForSpecies(Species species)
        {
            PokemonData pData = PokemonData.Data[species];

            switch (pData.GenderRatio)
            {
                case Gender.Male:
                case Gender.Female:
                case Gender.Genderless:
                    return pData.GenderRatio;
            }

            byte b = (byte)Utils.RNG.Next(0, byte.MaxValue + 1);
            if ((byte)pData.GenderRatio > b)
                return Gender.Female;
            else
                return Gender.Male;
        }

        public override string ToString() => $"{Species} Lv.{Level} {Nature} {Gender} {Ability} {HP}/{MaxHP} HP";
    }
}
