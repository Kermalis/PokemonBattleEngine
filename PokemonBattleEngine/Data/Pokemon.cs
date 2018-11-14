using Kermalis.PokemonBattleEngine.Battle;
using Kermalis.PokemonBattleEngine.Packets;
using System;
using System.Collections.Generic;
using System.IO;

namespace Kermalis.PokemonBattleEngine.Data
{
    public sealed class PPokemon
    {
        public readonly Guid Id;
        // Not included in ToBytes() or FromBytes(). Set manually by the host and by PKnownInfo
        // True indicates this pokemon is owned by the client or team 0 in the eyes of the host/spectators
        public bool Local;
        public readonly PPokemonShell Shell;

        public ushort HP, MaxHP, Attack, Defense, SpAttack, SpDefense, Speed;
        public PMove[] Moves = new PMove[PSettings.NumMoves];
        public byte[] PP = new byte[PSettings.NumMoves], MaxPP = new byte[PSettings.NumMoves];

        public PSpecies Species;
        public bool Shiny;
        public PAbility Ability;
        public PItem Item;
        public PFieldPosition FieldPosition = PFieldPosition.None;
        public PStatus1 Status1;
        public PStatus2 Status2;
        // These are in a set order; see BattleEffects->ApplyStatChange()
        public sbyte AttackChange, DefenseChange, SpAttackChange, SpDefenseChange, SpeedChange, AccuracyChange, EvasionChange;

        public byte Status1Counter; // Toxic/Sleep
        public byte SleepTurns; // Amount of turns to sleep

        public byte ConfusionCounter; // Confused
        public byte ConfusionTurns; // Amount of turns to be confused

        public byte ProtectCounter; // Protect
        public ushort SubstituteHP; // Substitute

        public PAction PreviousAction, LockedAction, SelectedAction;
        public int TurnOrder;

        public string OwnerDisplayName => Local ? PKnownInfo.Instance.LocalDisplayName : PKnownInfo.Instance.RemoteDisplayName;

        // Stats & PP are set from the shell info, but Local will need to be manually set by the host
        public PPokemon(Guid id, PPokemonShell shell)
        {
            Shell = shell;
            Species = Shell.Species;
            Shiny = Shell.Shiny;
            Ability = Shell.Ability;
            Item = Shell.Item;
            Id = id;
            SelectedAction.PokemonId = id;
            CalculateStats();
            HP = MaxHP;
            Moves = Shell.Moves;
            for (int i = 0; i < PSettings.NumMoves; i++)
            {
                PMove move = Shell.Moves[i];
                if (move != PMove.None)
                {
                    byte tier = PMoveData.Data[move].PPTier;
                    int movePP = (tier * PSettings.PPMultiplier) + (tier * Shell.PPUps[i]);
                    PP[i] = MaxPP[i] = (byte)movePP;
                }
            }
        }
        // This constructor is to define an unknown remote pokemon
        // Local is set to false here
        // Moves are set to PMove.MAX which will be displayed as "???"
        public PPokemon(PPkmnSwitchInPacket psip)
        {
            Id = psip.PokemonId;
            Local = false;
            Shell = new PPokemonShell
            {
                Nickname = psip.Nickname,
                Level = psip.Level,
                Gender = psip.Gender,
                Nature = PNature.MAX
            };
            Species = psip.Species;
            Shiny = psip.Shiny;
            Ability = PAbility.MAX;
            Item = PItem.MAX;
            for (int i = 0; i < PSettings.NumMoves; i++)
                Moves[i] = PMove.MAX;
        }

        void CalculateStats()
        {
            PPokemonData pData = PPokemonData.Data[Species];

            MaxHP = (ushort)(((2 * pData.HP + Shell.IVs[0] + (Shell.EVs[0] / 4)) * Shell.Level / PSettings.MaxLevel) + Shell.Level + 10);

            int i = 0;
            ushort OtherStat(byte baseVal)
            {
                double natureMultiplier = 1 + (PPokemonData.NatureBoosts[Shell.Nature][i] * PSettings.NatureStatBoost);
                ushort val = (ushort)((((2 * baseVal + Shell.IVs[i + 1] + (Shell.EVs[i + 1] / 4)) * Shell.Level / PSettings.MaxLevel) + 5) * natureMultiplier);
                i++;
                return val;
            }
            Attack = OtherStat(pData.Attack);
            Defense = OtherStat(pData.Defense);
            SpAttack = OtherStat(pData.SpAttack);
            SpDefense = OtherStat(pData.SpDefense);
            Speed = OtherStat(pData.Speed);
        }

        // Transforms into "target" and sets both Pokémons' information to the parameters
        // Also sets the status2 transformed bit
        public void Transform(PPokemon target, ushort targetAttack, ushort targetDefense, ushort targetSpAttack, ushort targetSpDefense, ushort targetSpeed, PAbility targetAbility, PMove[] targetMoves)
        {
            Species = target.Species;
            Shiny = target.Shiny;
            Ability = target.Ability = targetAbility;
            Attack = target.Attack = targetAttack;
            Defense = target.Defense = targetDefense;
            SpAttack = target.SpAttack = targetSpAttack;
            SpDefense = target.SpDefense = targetSpDefense;
            Speed = target.Speed = targetSpeed;
            AttackChange = target.AttackChange;
            DefenseChange = target.DefenseChange;
            SpAttackChange = target.SpAttackChange;
            SpDefenseChange = target.SpDefenseChange;
            SpeedChange = target.SpeedChange;
            AccuracyChange = target.AccuracyChange;
            EvasionChange = target.EvasionChange;
            Moves = target.Moves = targetMoves;
            for (int i = 0; i < PSettings.NumMoves; i++)
                PP[i] = MaxPP[i] = PSettings.PPMultiplier;
            Status2 |= PStatus2.Transformed;
        }

        public PType GetHiddenPowerType()
        {
            int a = Shell.IVs[0] & 1,
                b = Shell.IVs[1] & 1,
                c = Shell.IVs[2] & 1,
                d = Shell.IVs[5] & 1,
                e = Shell.IVs[3] & 1,
                f = Shell.IVs[4] & 1;
            return PPokemonData.HiddenPowerTypes[((1 << 0) * a + (1 << 1) * b + (1 << 2) * c + (1 << 3) * d + (1 << 4) * e + (1 << 5) * f) * (PPokemonData.HiddenPowerTypes.Length - 1) / ((1 << 6) - 1)];
        }
        public int GetHiddenPowerBasePower()
        {
            int a = (Shell.IVs[0] & 2) == 2 ? 1 : 0,
                b = (Shell.IVs[1] & 2) == 2 ? 1 : 0,
                c = (Shell.IVs[2] & 2) == 2 ? 1 : 0,
                d = (Shell.IVs[5] & 2) == 2 ? 1 : 0,
                e = (Shell.IVs[3] & 2) == 2 ? 1 : 0,
                f = (Shell.IVs[4] & 2) == 2 ? 1 : 0;
            // 30 is minimum, 30+40 is maximum
            return (((1 << 0) * a + (1 << 1) * b + (1 << 2) * c + (1 << 3) * d + (1 << 4) * e + (1 << 5) * f) * 40 / ((1 << 6) - 1)) + 30;
        }

        // ToBytes() and FromBytes() will only be used when the server sends you your team Ids, so they do not need to contain all info
        internal byte[] ToBytes()
        {
            var bytes = new List<byte>();
            bytes.AddRange(Id.ToByteArray());
            bytes.AddRange(Shell.ToBytes());
            return bytes.ToArray();
        }
        internal static PPokemon FromBytes(BinaryReader r)
        {
            return new PPokemon(new Guid(r.ReadBytes(0x10)), PPokemonShell.FromBytes(r));
        }

        public override bool Equals(object obj)
        {
            if (obj is PPokemon other)
                return other.Id.Equals(Id);
            return base.Equals(obj);
        }
        public override int GetHashCode() => Id.GetHashCode();
        public override string ToString()
        {
            bool remotePokemon = Shell.Nature == PNature.MAX; // If the nature is unset, the program is not the host and does not own the Pokémon

            string item = Item.ToString().Replace("MAX", "???");
            string nature = Shell.Nature.ToString().Replace("MAX", "???");
            string ability = Ability.ToString().Replace("MAX", "???");
            string[] moveStrs = new string[PSettings.NumMoves];
            for (int i = 0; i < PSettings.NumMoves; i++)
            {
                string mStr = Moves[i].ToString().Replace("MAX", "???");
                if (!remotePokemon)
                    mStr += $" {PP[i]}/{MaxPP[i]}";
                moveStrs[i] = mStr;
            }
            string moves = moveStrs.Print(false);

            string str = string.Empty;
            str += $"{Shell.Nickname}/{Species} {GenderSymbol} Lv.{Shell.Level}";
            str += Environment.NewLine;
            str += $"HP: {HP}/{MaxHP} ({(double)HP / MaxHP:P2})";
            str += Environment.NewLine;
            str += $"Status1: {Status1}";
            str += Environment.NewLine;
            str += $"Status2: {Status2}";
            if (!remotePokemon && Status2.HasFlag(PStatus2.Substitute))
            {
                str += Environment.NewLine;
                str += $"Substitute HP: {SubstituteHP}";
            }
            str += Environment.NewLine;
            str += $"Item: {item}";
            str += Environment.NewLine;
            str += $"Ability: {ability}";
            if (!remotePokemon)
            {
                str += Environment.NewLine;
                str += $"Nature: {nature}";
            }
            if (!remotePokemon)
            {
                str += Environment.NewLine;
                str += $"Hidden Power: {GetHiddenPowerType()}/{GetHiddenPowerBasePower()}";
            }
            str += Environment.NewLine;
            str += $"Moves: {moves}";

            return str;
        }
        public char GenderSymbol => Shell.Gender == PGender.Female ? '♀' : Shell.Gender == PGender.Male ? '♂' : ' ';
    }
}
