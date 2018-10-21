using Kermalis.PokemonBattleEngine.Util;
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
        public bool LocallyOwned;
        public readonly PPokemonShell Shell;

        public PStatus1 Status1;
        public PStatus2 Status2;
        public ushort HP, MaxHP, Attack, Defense, SpAttack, SpDefense, Speed;
        // These are in a set order; see BattleEffects->ApplyStatChange()
        public sbyte AttackChange, DefenseChange, SpAttackChange, SpDefenseChange, SpeedChange, AccuracyChange, EvasionChange;
        public byte[] PP = new byte[PConstants.NumMoves], MaxPP = new byte[PConstants.NumMoves];

        public string OwnerDisplayName => LocallyOwned ? PKnownInfo.Instance.LocalDisplayName : PKnownInfo.Instance.RemoteDisplayName;

        // Pass in Guid.Empty to generate a fresh id
        // LocallyOwned will need to be set manually by the host
        public PPokemon(Guid id, PPokemonShell shell)
        {
            Shell = shell;
            if (id == Guid.Empty)
                Id = Guid.NewGuid();
            else
                Id = id;
            CalculateStats();
            HP = MaxHP;
            for (int i = 0; i < PConstants.NumMoves; i++)
            {
                PMove move = Shell.Moves[i];
                if (move != PMove.None)
                {
                    byte tier = PMoveData.Data[move].PPTier;
                    int movePP = (tier * PConstants.PPMultiplier) + (tier * Shell.PPUps[i]);
                    PP[i] = MaxPP[i] = (byte)movePP;
                }
            }
        }
        // This constructor is to define an unknown remote pokemon
        // LocallyOwned is set to false here
        // Moves are set to PMove.MAX which will be displayed as "???"
        public PPokemon(Guid id, PSpecies species, string nickname, byte level, PGender gender)
        {
            Id = id;
            LocallyOwned = false;
            Shell = new PPokemonShell
            {
                Species = species,
                Nickname = nickname,
                Level = level,
                Gender = gender,
                Item = PItem.MAX,
                Nature = PNature.MAX,
                Ability = PAbility.MAX
            };
            for (int i = 0; i < PConstants.NumMoves; i++)
                Shell.Moves[i] = PMove.MAX;
        }

        void CalculateStats()
        {
            PPokemonData pData = PPokemonData.Data[Shell.Species];

            MaxHP = (ushort)(((2 * pData.HP + Shell.IVs[0] + (Shell.EVs[0] / 4)) * Shell.Level / PConstants.MaxLevel) + Shell.Level + 10);

            int i = 0;
            ushort OtherStat(byte baseVal)
            {
                double natureMultiplier = 1 + (PPokemonData.NatureBoosts[Shell.Nature][i] * PConstants.NatureStatBoost);
                ushort val = (ushort)((((2 * baseVal + Shell.IVs[i + 1] + (Shell.EVs[i + 1] / 4)) * Shell.Level / PConstants.MaxLevel) + 5) * natureMultiplier);
                i++;
                return val;
            }
            Attack = OtherStat(pData.Attack);
            Defense = OtherStat(pData.Defense);
            SpAttack = OtherStat(pData.SpAttack);
            SpDefense = OtherStat(pData.SpDefense);
            Speed = OtherStat(pData.Speed);
        }

        internal byte[] ToBytes()
        {
            var bytes = new List<byte>();
            bytes.AddRange(Id.ToByteArray());
            bytes.AddRange(Shell.ToBytes());
            bytes.Add((byte)Status1);
            bytes.AddRange(BitConverter.GetBytes((uint)Status2));
            bytes.AddRange(BitConverter.GetBytes(HP));
            bytes.Add((byte)AttackChange);
            bytes.Add((byte)DefenseChange);
            bytes.Add((byte)SpAttackChange);
            bytes.Add((byte)SpDefenseChange);
            bytes.Add((byte)SpeedChange);
            bytes.Add((byte)AccuracyChange);
            bytes.Add((byte)EvasionChange);
            bytes.AddRange(PP);
            bytes.AddRange(MaxPP);
            return bytes.ToArray();
        }
        internal static PPokemon FromBytes(BinaryReader r)
        {
            var pkmn = new PPokemon(new Guid(r.ReadBytes(0x10)), PPokemonShell.FromBytes(r))
            {
                Status1 = (PStatus1)r.ReadByte(),
                Status2 = (PStatus2)r.ReadUInt32(),
                HP = r.ReadUInt16(),
                AttackChange = r.ReadSByte(),
                DefenseChange = r.ReadSByte(),
                SpAttackChange = r.ReadSByte(),
                SpDefenseChange = r.ReadSByte(),
                SpeedChange = r.ReadSByte(),
                AccuracyChange = r.ReadSByte(),
                EvasionChange = r.ReadSByte(),
                PP = r.ReadBytes(PConstants.NumMoves),
                MaxPP = r.ReadBytes(PConstants.NumMoves)
            };
            return pkmn;
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
            string item = Shell.Item.ToString().Replace("MAX", "???");
            string nature = Shell.Nature.ToString().Replace("MAX", "???");
            string ability = Shell.Ability.ToString().Replace("MAX", "???");
            string[] moveStrs = new string[PConstants.NumMoves];
            for (int i = 0; i < PConstants.NumMoves; i++)
            {
                string mStr = Shell.Moves[i].ToString().Replace("MAX", "???");
                if (MaxPP[i] != 0)
                    mStr += $" {PP[i]}/{MaxPP[i]}";
                moveStrs[i] = mStr;
            }
            string moves = moveStrs.Print();

            string str = string.Empty;
            str += $"{Shell.Gender}";
            str += $" {Shell.Nickname}/{Shell.Species}";
            str += $" Lv.{Shell.Level}";
            str += $" {HP}/{MaxHP} HP";
            str += $" {Status1}";
            str += $" {item}";
            if (nature != "???") // You will never know the nature of an opponent
                str += $" {nature}";
            str += $" {ability}";
            str += $" {moves}";

            return str;
        }
    }
}
