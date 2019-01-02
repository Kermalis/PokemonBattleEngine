using Kermalis.PokemonBattleEngine.Battle;
using Kermalis.PokemonBattleEngine.Packets;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Kermalis.PokemonBattleEngine.Data
{
    public sealed class PBEPokemonShell
    {
        public PBESpecies Species { get; set; }
        public string Nickname { get; set; }
        public byte Level { get; set; }
        public byte Friendship { get; set; }
        public bool Shiny { get; set; }
        public PBEAbility Ability { get; set; }
        public PBENature Nature { get; set; }
        public PBEGender Gender { get; set; }
        public PBEItem Item { get; set; }
        public byte[] EVs { get; set; }
        public byte[] IVs { get; set; }
        public PBEMove[] Moves { get; set; }
        public byte[] PPUps { get; set; }

        // Throws ArgumentOutOfRangeException for the invalid information
        public static void ValidateMany(IEnumerable<PBEPokemonShell> shells, PBESettings settings)
        {
            foreach (PBEPokemonShell pkmn in shells)
            {
                pkmn.Validate(settings);
            }
        }
        // Throws ArgumentOutOfRangeException for the invalid information
        public void Validate(PBESettings settings)
        {
            // Validate Species
            PBEPokemonData pData;
            try
            {
                pData = PBEPokemonData.Data[Species];
            }
            catch
            {
                throw new ArgumentOutOfRangeException(nameof(Species), "Invalid species.");
            }

            // Validate Shininess
            if (Shiny && pData.ShinyLocked)
            {
                throw new ArgumentOutOfRangeException(nameof(Shiny), $"{Species} cannot be shiny.");
            }

            // Validate Nickname
            if (string.IsNullOrWhiteSpace(Nickname))
            {
                throw new ArgumentOutOfRangeException(nameof(Nickname), $"{nameof(Nickname)} cannot be empty.");
            }
            if (Nickname.Length > settings.MaxPokemonNameLength)
            {
                throw new ArgumentOutOfRangeException(nameof(Nickname), $"{nameof(Nickname)} cannot exceed {settings.MaxPokemonNameLength} characters.");
            }

            // Validate Level
            if (Level < pData.MinLevel || Level > settings.MaxLevel)
            {
                throw new ArgumentOutOfRangeException(nameof(Level), $"A {Species}'s level must be at least {pData.MinLevel} and cannot exceed {settings.MaxLevel}.");
            }

            // Validate Ability
            if (!pData.HasAbility(Ability))
            {
                throw new ArgumentOutOfRangeException(nameof(Ability), $"{Species} cannot have {Ability}.");
            }

            // Validate Nature
            if (Nature >= PBENature.MAX)
            {
                throw new ArgumentOutOfRangeException(nameof(Nature), "Invalid nature.");
            }

            // Validate Gender
            if (Gender >= PBEGender.MAX
                || (Gender == PBEGender.Male && (pData.GenderRatio == PBEGenderRatio.M0_F1 || pData.GenderRatio == PBEGenderRatio.M0_F0))
                || (Gender == PBEGender.Female && (pData.GenderRatio == PBEGenderRatio.M1_F0 || pData.GenderRatio == PBEGenderRatio.M0_F0))
                || (Gender == PBEGender.Genderless && pData.GenderRatio != PBEGenderRatio.M0_F0)
                )
            {
                throw new ArgumentOutOfRangeException(nameof(Gender), $"Invalid gender for {Species}.");
            }

            // Validate Item
            if (Item != PBEItem.None)
            {
                try
                {
                    PBEItemData iData = PBEItemData.Data[Item];
                }
                catch
                {
                    throw new ArgumentOutOfRangeException(nameof(Item), "Invalid item.");
                }
            }

            // Validate EVs
            if (EVs == null || EVs.Length != 6)
            {
                throw new ArgumentOutOfRangeException(nameof(EVs), $"{nameof(EVs)} array can only have a length of 6.");
            }
            if (EVs.Select(e => (int)e).Sum() > settings.MaxTotalEVs)
            {
                throw new ArgumentOutOfRangeException(nameof(EVs), $"Total EVs cannot exceed {settings.MaxTotalEVs}.");
            }
            // Validate IVs
            if (IVs == null || IVs.Length != 6)
            {
                throw new ArgumentOutOfRangeException(nameof(IVs), $"{nameof(IVs)} array can only have a length of 6.");
            }
            if (IVs.Any(i => i > settings.MaxIVs))
            {
                throw new ArgumentOutOfRangeException(nameof(IVs), $"Each IV cannot exceed {settings.MaxIVs}.");
            }

            // Validate Moves
            if (Moves == null || Moves.Length != settings.NumMoves)
            {
                throw new ArgumentOutOfRangeException(nameof(Moves), $"{nameof(Moves)} array can only have a length of {settings.NumMoves}.");
            }
            // TODO: legal moves
            /*IEnumerable<PBEMove> legalMoves = pData.LevelUpMoves.Where(t => t.Item2 <= Level).Select(t => t.Item1).Union(pData.OtherMoves.Select(t => t.Item1));
            if (Moves.Any(m => m != PBEMove.None && !legalMoves.Contains(m)))
            {
                throw new ArgumentOutOfRangeException(nameof(Moves), $"Illegal moves for {Species}.");
            }*/
            if (Moves.Any(m => Moves.Count(m2 => m != PBEMove.None && m == m2) > 1))
            {
                throw new ArgumentOutOfRangeException(nameof(Moves), $"A Pokémon cannot have duplicate moves other than {PBEMove.None}.");
            }
            if (Moves.All(m => m == PBEMove.None))
            {
                throw new ArgumentOutOfRangeException(nameof(Moves), $"A Pokémon must have at least one move other than {PBEMove.None}.");
            }

            // Validate PPUps
            if (PPUps == null || PPUps.Length != settings.NumMoves)
            {
                throw new ArgumentOutOfRangeException(nameof(PPUps), $"{nameof(PPUps)} array can only have a length of {settings.NumMoves}.");
            }
            if (PPUps.Any(p => p > settings.MaxPPUps))
            {
                throw new ArgumentOutOfRangeException(nameof(PPUps), $"Each PP-Up cannot exceed {settings.MaxPPUps}.");
            }

            // Validate Forme-Specific Requirements
            switch (Species)
            {
                case PBESpecies.Shedinja:
                    if (EVs[0] > 0)
                    {
                        throw new ArgumentOutOfRangeException(nameof(EVs), $"{Species} cannot have any HP EVs.");
                    }
                    break;
                case PBESpecies.Giratina_Altered:
                    if (Item == PBEItem.GriseousOrb)
                    {
                        throw new ArgumentOutOfRangeException(nameof(Item), $"{Species} cannot hold a {Item}.");
                    }
                    break;
                case PBESpecies.Giratina_Origin:
                    if (Item != PBEItem.GriseousOrb)
                    {
                        throw new ArgumentOutOfRangeException(nameof(Item), $"{Species} must hold a {PBEItem.GriseousOrb}.");
                    }
                    break;
                case PBESpecies.Arceus_Normal:
                    if (Item == PBEItem.DracoPlate
                        || Item == PBEItem.DreadPlate
                        || Item == PBEItem.EarthPlate
                        || Item == PBEItem.FistPlate
                        || Item == PBEItem.FlamePlate
                        || Item == PBEItem.IciclePlate
                        || Item == PBEItem.InsectPlate
                        || Item == PBEItem.IronPlate
                        || Item == PBEItem.MeadowPlate
                        || Item == PBEItem.MindPlate
                        || Item == PBEItem.SkyPlate
                        || Item == PBEItem.SplashPlate
                        || Item == PBEItem.SpookyPlate
                        || Item == PBEItem.StonePlate
                        || Item == PBEItem.ToxicPlate
                        || Item == PBEItem.ZapPlate)
                    {
                        throw new ArgumentOutOfRangeException(nameof(Item), $"{Species} cannot hold a {Item}.");
                    }
                    break;
                case PBESpecies.Arceus_Bug:
                    if (Item != PBEItem.InsectPlate)
                    {
                        throw new ArgumentOutOfRangeException(nameof(Item), $"{Species} must hold a {PBEItem.InsectPlate}.");
                    }
                    break;
                case PBESpecies.Arceus_Dark:
                    if (Item != PBEItem.DreadPlate)
                    {
                        throw new ArgumentOutOfRangeException(nameof(Item), $"{Species} must hold a {PBEItem.DreadPlate}.");
                    }
                    break;
                case PBESpecies.Arceus_Dragon:
                    if (Item != PBEItem.DracoPlate)
                    {
                        throw new ArgumentOutOfRangeException(nameof(Item), $"{Species} must hold a {PBEItem.DracoPlate}.");
                    }
                    break;
                case PBESpecies.Arceus_Electric:
                    if (Item != PBEItem.ZapPlate)
                    {
                        throw new ArgumentOutOfRangeException(nameof(Item), $"{Species} must hold a {PBEItem.ZapPlate}.");
                    }
                    break;
                case PBESpecies.Arceus_Fighting:
                    if (Item != PBEItem.FistPlate)
                    {
                        throw new ArgumentOutOfRangeException(nameof(Item), $"{Species} must hold a {PBEItem.FistPlate}.");
                    }
                    break;
                case PBESpecies.Arceus_Fire:
                    if (Item != PBEItem.FlamePlate)
                    {
                        throw new ArgumentOutOfRangeException(nameof(Item), $"{Species} must hold a {PBEItem.FlamePlate}.");
                    }
                    break;
                case PBESpecies.Arceus_Flying:
                    if (Item != PBEItem.SkyPlate)
                    {
                        throw new ArgumentOutOfRangeException(nameof(Item), $"{Species} must hold a {PBEItem.SkyPlate}.");
                    }
                    break;
                case PBESpecies.Arceus_Ghost:
                    if (Item != PBEItem.SpookyPlate)
                    {
                        throw new ArgumentOutOfRangeException(nameof(Item), $"{Species} must hold a {PBEItem.SpookyPlate}.");
                    }
                    break;
                case PBESpecies.Arceus_Grass:
                    if (Item != PBEItem.MeadowPlate)
                    {
                        throw new ArgumentOutOfRangeException(nameof(Item), $"{Species} must hold a {PBEItem.MeadowPlate}.");
                    }
                    break;
                case PBESpecies.Arceus_Ground:
                    if (Item != PBEItem.EarthPlate)
                    {
                        throw new ArgumentOutOfRangeException(nameof(Item), $"{Species} must hold a {PBEItem.EarthPlate}.");
                    }
                    break;
                case PBESpecies.Arceus_Ice:
                    if (Item != PBEItem.IciclePlate)
                    {
                        throw new ArgumentOutOfRangeException(nameof(Item), $"{Species} must hold a {PBEItem.IciclePlate}.");
                    }
                    break;
                case PBESpecies.Arceus_Poison:
                    if (Item != PBEItem.ToxicPlate)
                    {
                        throw new ArgumentOutOfRangeException(nameof(Item), $"{Species} must hold a {PBEItem.ToxicPlate}.");
                    }
                    break;
                case PBESpecies.Arceus_Psychic:
                    if (Item != PBEItem.MindPlate)
                    {
                        throw new ArgumentOutOfRangeException(nameof(Item), $"{Species} must hold a {PBEItem.MindPlate}.");
                    }
                    break;
                case PBESpecies.Arceus_Rock:
                    if (Item != PBEItem.StonePlate)
                    {
                        throw new ArgumentOutOfRangeException(nameof(Item), $"{Species} must hold a {PBEItem.StonePlate}.");
                    }
                    break;
                case PBESpecies.Arceus_Steel:
                    if (Item != PBEItem.IronPlate)
                    {
                        throw new ArgumentOutOfRangeException(nameof(Item), $"{Species} must hold a {PBEItem.IronPlate}.");
                    }
                    break;
                case PBESpecies.Arceus_Water:
                    if (Item != PBEItem.SplashPlate)
                    {
                        throw new ArgumentOutOfRangeException(nameof(Item), $"{Species} must hold a {PBEItem.SplashPlate}.");
                    }
                    break;
                case PBESpecies.Genesect:
                    if (Item == PBEItem.BurnDrive
                        || Item == PBEItem.ChillDrive
                        || Item == PBEItem.DouseDrive
                        || Item == PBEItem.ShockDrive)
                    {
                        throw new ArgumentOutOfRangeException(nameof(Item), $"{Species} cannot hold a {Item}.");
                    }
                    break;
                case PBESpecies.Genesect_Burn:
                    if (Item != PBEItem.BurnDrive)
                    {
                        throw new ArgumentOutOfRangeException(nameof(Item), $"{Species} must hold a {PBEItem.BurnDrive}.");
                    }
                    break;
                case PBESpecies.Genesect_Chill:
                    if (Item != PBEItem.ChillDrive)
                    {
                        throw new ArgumentOutOfRangeException(nameof(Item), $"{Species} must hold a {PBEItem.ChillDrive}.");
                    }
                    break;
                case PBESpecies.Genesect_Douse:
                    if (Item != PBEItem.DouseDrive)
                    {
                        throw new ArgumentOutOfRangeException(nameof(Item), $"{Species} must hold a {PBEItem.DouseDrive}.");
                    }
                    break;
                case PBESpecies.Genesect_Shock:
                    if (Item != PBEItem.ShockDrive)
                    {
                        throw new ArgumentOutOfRangeException(nameof(Item), $"{Species} must hold a {PBEItem.ShockDrive}.");
                    }
                    break;
            }
        }

        internal byte[] ToBytes()
        {
            var bytes = new List<byte>();
            bytes.AddRange(BitConverter.GetBytes((uint)Species));
            bytes.AddRange(PBEUtils.StringToBytes(Nickname));
            bytes.Add(Level);
            bytes.Add(Friendship);
            bytes.Add((byte)(Shiny ? 1 : 0));
            bytes.Add((byte)Ability);
            bytes.Add((byte)Nature);
            bytes.Add((byte)Gender);
            bytes.AddRange(BitConverter.GetBytes((ushort)Item));
            bytes.AddRange(EVs);
            bytes.AddRange(IVs);
            bytes.Add((byte)Moves.Length);
            for (int i = 0; i < Moves.Length; i++)
            {
                bytes.AddRange(BitConverter.GetBytes((ushort)Moves[i]));
            }
            bytes.AddRange(PPUps);
            return bytes.ToArray();
        }
        internal static PBEPokemonShell FromBytes(BinaryReader r)
        {
            var pkmn = new PBEPokemonShell
            {
                Species = (PBESpecies)r.ReadUInt32(),
                Nickname = PBEUtils.StringFromBytes(r),
                Level = r.ReadByte(),
                Friendship = r.ReadByte(),
                Shiny = r.ReadBoolean(),
                Ability = (PBEAbility)r.ReadByte(),
                Nature = (PBENature)r.ReadByte(),
                Gender = (PBEGender)r.ReadByte(),
                Item = (PBEItem)r.ReadUInt16(),
                EVs = r.ReadBytes(6),
                IVs = r.ReadBytes(6),
            };
            byte numMoves = r.ReadByte();
            pkmn.Moves = new PBEMove[numMoves];
            for (int i = 0; i < numMoves; i++)
            {
                pkmn.Moves[i] = (PBEMove)r.ReadUInt16();
            }
            pkmn.PPUps = r.ReadBytes(numMoves);
            return pkmn;
        }
    }

    /// <summary>
    /// Represents a specific Pokémon during a battle.
    /// </summary>
    public sealed class PBEPokemon
    {
        /// <summary>
        /// The team this Pokémon belongs to in its battle.
        /// </summary>
        public PBETeam Team { get; }
        /// <summary>
        /// The Pokémon's ID in its battle.
        /// </summary>
        public byte Id { get; }
        /// <summary>
        /// The shell that was used to construct this Pokémon.
        /// </summary>
        public PBEPokemonShell Shell { get; }

        /// <summary>
        /// The Pokémon's nickname with its gender attached.
        /// </summary>
        public string NameWithGender => Shell.Nickname + GenderSymbol;
        /// <summary>
        /// The Pokémon's gender as a string.
        /// </summary>
        public string GenderSymbol => Shell.Gender == PBEGender.Female ? "♀" : Shell.Gender == PBEGender.Male ? "♂" : string.Empty;

        /// <summary>
        /// The Pokémon's current HP.
        /// </summary>
        public ushort HP { get; set; }
        /// <summary>
        /// The Pokémon's maximum HP.
        /// </summary>
        public ushort MaxHP { get; set; }
        /// <summary>
        /// The Pokémon's attack stat.
        /// </summary>
        public ushort Attack { get; set; }
        /// <summary>
        /// The Pokémon's defense stat.
        /// </summary>
        public ushort Defense { get; set; }
        /// <summary>
        /// The Pokémon's special attack stat.
        /// </summary>
        public ushort SpAttack { get; set; }
        /// <summary>
        /// The Pokémon's special defense stat.
        /// </summary>
        public ushort SpDefense { get; set; }
        /// <summary>
        /// The Pokémon's speed stat.
        /// </summary>
        public ushort Speed { get; set; }
        public PBEMove[] Moves { get; set; }
        public byte[] PP { get; set; }
        public byte[] MaxPP { get; set; }

        public PBESpecies Species { get; set; }
        public bool Shiny { get; set; }
        public PBEAbility Ability { get; set; }
        public PBEType Type1 { get; set; }
        public PBEType Type2 { get; set; }
        public double Weight { get; set; }
        public PBEItem Item { get; set; }
        public PBEFieldPosition FieldPosition { get; set; }
        public PBEStatus1 Status1 { get; set; }
        public PBEStatus2 Status2 { get; set; }

        public sbyte AttackChange { get; set; }
        public sbyte DefenseChange { get; set; }
        public sbyte SpAttackChange { get; set; }
        public sbyte SpDefenseChange { get; set; }
        public sbyte SpeedChange { get; set; }
        public sbyte AccuracyChange { get; set; }
        public sbyte EvasionChange { get; set; }

        /// <summary>
        /// The counter used for <see cref="PBEStatus1.BadlyPoisoned"/> and <see cref="PBEStatus1.Asleep"/>.
        /// </summary>
        public byte Status1Counter { get; set; }
        /// <summary>
        /// The amount of turns the Pokémon will sleep for before waking.
        /// </summary>
        public byte SleepTurns { get; internal set; }
        /// <summary>
        /// The counter used for <see cref="PBEStatus2.Confused"/>.
        /// </summary>
        public byte ConfusionCounter { get; set; }
        /// <summary>
        /// The amount of turns the Pokémon will be confused for before snapping out of it.
        /// </summary>
        public byte ConfusionTurns { get; internal set; }
        /// <summary>
        /// The amount of consecutive turns the Pokémon has used protection.
        /// </summary>
        public byte ProtectCounter { get; set; }
        /// <summary>
        /// The amount of HP the Pokémon's <see cref="PBEStatus2.Substitute"/> has left.
        /// </summary>
        public ushort SubstituteHP { get; set; }
        /// <summary>
        /// The position to return <see cref="PBEStatus2.LeechSeed"/> HP to on the opposing team.
        /// </summary>
        public PBEFieldPosition SeededPosition { get; set; }

        public PBEAction PreviousAction, SelectedAction;
        /// <summary>
        /// The move the Pokémon is forced to use by multi-turn moves.
        /// </summary>
        public PBEMove TempLockedMove { get; set; }
        /// <summary>
        /// The targets the Pokémon is forced to target by multi-turn moves.
        /// </summary>
        public PBETarget TempLockedTargets { get; set; }
        /// <summary>
        /// The move the Pokémon is forced to use by its choice item.
        /// </summary>
        public PBEMove ChoiceLockedMove { get; set; }

        // Stats & PP are set from the shell info
        public PBEPokemon(PBETeam team, byte id, PBEPokemonShell shell)
        {
            Team = team;
            Id = id;
            SelectedAction.PokemonId = id;
            Shell = shell;
            Species = Shell.Species;
            Shiny = Shell.Shiny;
            Ability = Shell.Ability;
            Item = Shell.Item;
            CalculateStats();
            HP = MaxHP;
            Moves = Shell.Moves;
            PP = new byte[Moves.Length];
            MaxPP = new byte[Moves.Length];
            for (int i = 0; i < Moves.Length; i++)
            {
                PBEMove move = Shell.Moves[i];
                if (move != PBEMove.None)
                {
                    byte tier = PBEMoveData.Data[move].PPTier;
                    int movePP = Math.Max(1, (tier * Team.Battle.Settings.PPMultiplier) + (tier * Shell.PPUps[i]));
                    PP[i] = MaxPP[i] = (byte)movePP;
                }
            }
            PBEPokemonData pData = PBEPokemonData.Data[Species];
            Type1 = pData.Type1;
            Type2 = pData.Type2;
            Weight = pData.Weight;
            Team.Party.Add(this);
        }
        // This constructor is to define an unknown remote Pokémon
        public PBEPokemon(PBETeam team, PBEPkmnSwitchInPacket.PBESwitchInInfo info)
        {
            Team = team;
            Id = info.PokemonId;
            Shell = new PBEPokemonShell
            {
                Species = info.Species,
                Shiny = info.Shiny,
                Nickname = info.Nickname,
                Level = info.Level,
                Gender = info.Gender,
                Ability = PBEAbility.MAX,
                Item = PBEItem.MAX,
                Nature = PBENature.MAX,
                Moves = new PBEMove[Team.Battle.Settings.NumMoves],
                PPUps = new byte[Team.Battle.Settings.NumMoves],
                EVs = new byte[6],
                IVs = new byte[6]
            };
            Species = info.Species;
            Shiny = info.Shiny;
            Ability = PBEAbility.MAX;
            Item = PBEItem.MAX;
            Moves = new PBEMove[Team.Battle.Settings.NumMoves];
            PP = new byte[Moves.Length];
            MaxPP = new byte[Moves.Length];
            for (int i = 0; i < Moves.Length; i++)
            {
                Moves[i] = PBEMove.MAX;
            }
            PBEPokemonData pData = PBEPokemonData.Data[Species];
            Type1 = pData.Type1;
            Type2 = pData.Type2;
            Weight = pData.Weight;
            Team.Party.Add(this);
        }

        /// <summary>
        /// Returns True if the Pokémon has <paramref name="type"/>, False otherwise.
        /// </summary>
        /// <param name="type">The type to check.</param>
        public bool HasType(PBEType type) => Type1 == type || Type2 == type;

        /// <summary>
        /// Calculates and sets the Pokémon's stats based on its level, IVs, EVs, nature, and species.
        /// </summary>
        void CalculateStats()
        {
            PBEPokemonData pData = PBEPokemonData.Data[Species];

            MaxHP = (ushort)(Species == PBESpecies.Shedinja ? 1 : (((2 * pData.HP + Shell.IVs[0] + (Shell.EVs[0] / 4)) * Shell.Level / Team.Battle.Settings.MaxLevel) + Shell.Level + 10));

            int i = 0;
            ushort OtherStat(byte baseVal)
            {
                double natureMultiplier = 1 + (PBEPokemonData.NatureBoosts[Shell.Nature][i] * Team.Battle.Settings.NatureStatBoost);
                ushort val = (ushort)((((2 * baseVal + Shell.IVs[i + 1] + (Shell.EVs[i + 1] / 4)) * Shell.Level / Team.Battle.Settings.MaxLevel) + 5) * natureMultiplier);
                i++;
                return val;
            }
            Attack = OtherStat(pData.Attack);
            Defense = OtherStat(pData.Defense);
            SpAttack = OtherStat(pData.SpAttack);
            SpDefense = OtherStat(pData.SpDefense);
            Speed = OtherStat(pData.Speed);
        }

        /// <summary>
        /// Sets and clears all information required for switching out.
        /// </summary>
        public void ClearForSwitch()
        {
            FieldPosition = PBEFieldPosition.None;
            Species = Shell.Species;
            Ability = Shell.Ability;
            Shiny = Shell.Shiny;

            AttackChange = DefenseChange = SpAttackChange = SpDefenseChange = SpeedChange = AccuracyChange = EvasionChange = 0;

            if (Status1 == PBEStatus1.Asleep)
            {
                Status1Counter = SleepTurns;
            }
            else if (Status1 == PBEStatus1.BadlyPoisoned)
            {
                Status1Counter = 1;
            }
            Status2 &= ~PBEStatus2.Confused;
            ConfusionCounter = ConfusionTurns = 0;
            Status2 &= ~PBEStatus2.Cursed;
            Status2 &= ~PBEStatus2.LeechSeed;
            SeededPosition = PBEFieldPosition.None;
            Status2 &= ~PBEStatus2.Minimized;
            Status2 &= ~PBEStatus2.Pumped;
            Status2 &= ~PBEStatus2.Substitute;
            SubstituteHP = 0;
            Status2 &= ~PBEStatus2.Transformed;

            TempLockedMove = ChoiceLockedMove = PBEMove.None;
            TempLockedTargets = PBETarget.None;

            if (Shell.Nature != PBENature.MAX) // If the nature is unset, the program is not the host and does not own the Pokémon
            {
                CalculateStats();
            }
        }

        /// <summary>
        /// Transforms into <paramref name="target"/> and sets <see cref="PBEStatus2.Transformed"/>.
        /// </summary>
        /// <param name="target">The Pokémon to transform into.</param>
        /// <remarks>Frees the Pokémon of its <see cref="ChoiceLockedMove"/>.</remarks>
        public void Transform(PBEPokemon target)
        {
            Species = target.Species;
            Shiny = target.Shiny;
            Ability = target.Ability;
            Type1 = target.Type1;
            Type2 = target.Type2;
            Weight = target.Weight;
            Attack = target.Attack;
            Defense = target.Defense;
            SpAttack = target.SpAttack;
            SpDefense = target.SpDefense;
            Speed = target.Speed;
            AttackChange = target.AttackChange;
            DefenseChange = target.DefenseChange;
            SpAttackChange = target.SpAttackChange;
            SpDefenseChange = target.SpDefenseChange;
            SpeedChange = target.SpeedChange;
            AccuracyChange = target.AccuracyChange;
            EvasionChange = target.EvasionChange;
            Moves = (PBEMove[])target.Moves.Clone();
            for (int i = 0; i < Moves.Length; i++)
            {
                int pp = Moves[i] == PBEMove.None ? 0 : PBEMoveData.Data[Moves[i]].PPTier == 0 ? 1 : Team.Battle.Settings.PPMultiplier;
                PP[i] = MaxPP[i] = (byte)pp;
            }
            ChoiceLockedMove = PBEMove.None;
            Status2 |= PBEStatus2.Transformed;
        }

        /// <summary>
        /// Gets the type that <see cref="PBEMove.HiddenPower"/> will become when used by this Pokémon.
        /// </summary>
        public PBEType GetHiddenPowerType()
        {
            int a = Shell.IVs[0] & 1,
                b = Shell.IVs[1] & 1,
                c = Shell.IVs[2] & 1,
                d = Shell.IVs[5] & 1,
                e = Shell.IVs[3] & 1,
                f = Shell.IVs[4] & 1;
            return PBEPokemonData.HiddenPowerTypes[((1 << 0) * a + (1 << 1) * b + (1 << 2) * c + (1 << 3) * d + (1 << 4) * e + (1 << 5) * f) * (PBEPokemonData.HiddenPowerTypes.Length - 1) / ((1 << 6) - 1)];
        }
        /// <summary>
        /// Gets the base power that <see cref="PBEMove.HiddenPower"/> will have when used by this Pokémon.
        /// </summary>
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
        /// <summary>
        /// Gets the type that a move will become when used by this Pokémon.
        /// </summary>
        /// <param name="move">The move to check.</param>
        public PBEType GetMoveType(PBEMove move)
        {
            switch (move)
            {
                case PBEMove.None: return PBEType.Normal;
                case PBEMove.HiddenPower: return GetHiddenPowerType();
                case PBEMove.TechnoBlast:
                    switch (Item)
                    {
                        case PBEItem.BurnDrive: return PBEType.Fire;
                        case PBEItem.ChillDrive: return PBEType.Ice;
                        case PBEItem.DouseDrive: return PBEType.Water;
                        case PBEItem.ShockDrive: return PBEType.Electric;
                        default: return PBEMoveData.Data[PBEMove.TechnoBlast].Type;
                    }
                case PBEMove.WeatherBall:
                    switch (Team.Battle.Weather)
                    {
                        case PBEWeather.Hailstorm: return PBEType.Ice;
                        case PBEWeather.HarshSunlight: return PBEType.Fire;
                        case PBEWeather.Rain: return PBEType.Water;
                        case PBEWeather.Sandstorm: return PBEType.Rock;
                        default: return PBEMoveData.Data[PBEMove.WeatherBall].Type;
                    }
                default: return PBEMoveData.Data[move].Type;
            }
        }
        /// <summary>
        /// Returns True if the Pokémon is only able to use <see cref="PBEMove.Struggle"/>, False otherwise.
        /// </summary>
        /// <returns></returns>
        public bool IsForcedToStruggle()
        {
            return
                (ChoiceLockedMove != PBEMove.None && PP[Array.IndexOf(Moves, ChoiceLockedMove)] == 0)
                || PP.All(p => p == 0);
        }

        // ToBytes() and FromBytes() will only be used when the server sends you your team Ids, so they do not need to contain all info
        internal byte[] ToBytes()
        {
            var bytes = new List<byte>();
            bytes.Add(Id);
            bytes.AddRange(Shell.ToBytes());
            return bytes.ToArray();
        }
        internal static PBEPokemon FromBytes(BinaryReader r, PBETeam team)
        {
            return new PBEPokemon(team, r.ReadByte(), PBEPokemonShell.FromBytes(r));
        }

        public override bool Equals(object obj)
        {
            if (obj is PBEPokemon other)
            {
                return other.Id.Equals(Id);
            }
            return base.Equals(obj);
        }
        public override int GetHashCode() => Id.GetHashCode();
        public override string ToString()
        {
            bool remotePokemon = Shell.Nature == PBENature.MAX; // If the nature is unset, the program is not the host and does not own the Pokémon

            string item = Item.ToString().Replace("MAX", "???");
            string nature = Shell.Nature.ToString().Replace("MAX", "???");
            string ability = Ability.ToString().Replace("MAX", "???");
            string[] moveStrs = new string[Moves.Length];
            for (int i = 0; i < moveStrs.Length; i++)
            {
                string mStr = Moves[i].ToString().Replace("MAX", "???");
                if (!remotePokemon)
                {
                    mStr += $" {PP[i]}/{MaxPP[i]}";
                }
                moveStrs[i] = mStr;
            }
            string moves = moveStrs.Print(false);

            var sb = new StringBuilder();
            sb.AppendLine($"{Shell.Nickname}/{Species} {GenderSymbol} Lv.{Shell.Level}");
            sb.AppendLine($"HP: {HP}/{MaxHP} ({(double)HP / MaxHP:P2})");
            sb.AppendLine($"Position: {FieldPosition}");
            sb.AppendLine($"Type1: {Type1}");
            sb.AppendLine($"Type2: {Type2}");
            sb.AppendLine($"Status1: {Status1}");
            sb.AppendLine($"Status2: {Status2}");
            if (!remotePokemon && Status2.HasFlag(PBEStatus2.Substitute))
            {
                sb.AppendLine($"Substitute HP: {SubstituteHP}");
            }
            if (!remotePokemon)
            {
                sb.AppendLine($"Stats: {Attack} {Defense} {SpAttack} {SpDefense} {Speed}");
            }
            sb.AppendLine($"Stat changes: {AttackChange} {DefenseChange} {SpAttackChange} {SpDefenseChange} {SpeedChange} {AccuracyChange} {EvasionChange}");
            sb.AppendLine($"Item: {item}");
            sb.AppendLine($"Ability: {ability}");
            if (!remotePokemon)
            {
                sb.AppendLine($"Nature: {nature}");
            }
            if (!remotePokemon)
            {
                sb.AppendLine($"Hidden Power: {GetHiddenPowerType()}/{GetHiddenPowerBasePower()}");
            }
            sb.Append($"Moves: {moves}");
            return sb.ToString();
        }
    }
}
