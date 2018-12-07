using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Kermalis.PokemonBattleEngine.Data
{
    public sealed class PBEPokemonShell
    {
        public PBESpecies Species;
        public string Nickname;
        public byte Level = PBESettings.MaxLevel,
            Friendship = 255;
        public bool Shiny;
        public PBEAbility Ability;
        public PBENature Nature = (PBENature)PBEUtils.RNG.Next(0, (int)PBENature.MAX);
        public PBEGender Gender;
        public PBEItem Item;
        public byte[] EVs = new byte[6],
            IVs = new byte[6];
        public PBEMove[] Moves = new PBEMove[PBESettings.NumMoves];
        public byte[] PPUps = new byte[PBESettings.NumMoves];

        // Throws ArgumentOutOfRangeException for the invalid information
        public static void ValidateMany(IEnumerable<PBEPokemonShell> shells)
        {
            var arr = shells.ToArray();
            for (int i = 0; i < arr.Length; i++)
            {
                arr[i].Validate();
            }
        }
        // Throws ArgumentOutOfRangeException for the invalid information
        public void Validate()
        {
            // Validate Species
            PBEPokemonData pData;
            try
            {
                pData = PBEPokemonData.Data[Species];
            }
            catch
            {
                throw new ArgumentOutOfRangeException(nameof(Species));
            }

            // Validate Shininess
            if (Shiny && pData.ShinyLocked)
            {
                throw new ArgumentOutOfRangeException(nameof(Shiny));
            }

            // Validate Nickname
            if (string.IsNullOrWhiteSpace(Nickname) || Nickname.Length > PBESettings.MaxPokemonNameLength)
            {
                throw new ArgumentOutOfRangeException(nameof(Nickname));
            }

            // Validate Level
            if (Level < pData.MinLevel || Level > PBESettings.MaxLevel)
            {
                throw new ArgumentOutOfRangeException(nameof(Level));
            }

            // Validate Ability
            if (!pData.HasAbility(Ability))
            {
                throw new ArgumentOutOfRangeException(nameof(Ability));
            }

            // Validate Nature
            if (Nature >= PBENature.MAX)
            {
                throw new ArgumentOutOfRangeException(nameof(Nature));
            }

            // Validate Gender
            if (Gender >= PBEGender.MAX
                || (Gender == PBEGender.Male && (pData.GenderRatio == PBEGenderRatio.M0_F1 || pData.GenderRatio == PBEGenderRatio.M0_F0))
                || (Gender == PBEGender.Female && (pData.GenderRatio == PBEGenderRatio.M1_F0 || pData.GenderRatio == PBEGenderRatio.M0_F0))
                || (Gender == PBEGender.Genderless && pData.GenderRatio != PBEGenderRatio.M0_F0)
                )
            {
                throw new ArgumentOutOfRangeException(nameof(Gender));
            }

            // Validate Item
            if (Item != PBEItem.None)
            {
                try
                {
                    var iData = PBEItemData.Data[Item];
                }
                catch
                {
                    throw new ArgumentOutOfRangeException(nameof(Item));
                }
            }

            // Validate EVs
            if (EVs == null || EVs.Length != 6 || EVs.Select(e => (int)e).Sum() > PBESettings.MaxTotalEVs)
            {
                throw new ArgumentOutOfRangeException(nameof(EVs));
            }
            // Validate IVs
            if (IVs == null || IVs.Length != 6 || IVs.Any(i => i > PBESettings.MaxIVs))
            {
                throw new ArgumentOutOfRangeException(nameof(IVs));
            }

            // Validate Moves
            // TODO: legal moves
            //IEnumerable<PBEMove> legalMoves = pData.LevelUpMoves.Where(t => t.Item2 <= Level).Select(t => t.Item1).Union(pData.OtherMoves.Select(t => t.Item1));
            if (Moves == null || Moves.Length != PBESettings.NumMoves // Illegal array
                                                                      //|| Moves.Any(m => m != PBEMove.None && !legalMoves.Contains(m)) // Has a move not in the legal list
                || Moves.Any(m => Moves.Count(m2 => m != PBEMove.None && m == m2) > 1) // Has a duplicate move
                || Moves.All(m => m == PBEMove.None) // Has no moves
                )
            {
                throw new ArgumentOutOfRangeException(nameof(Moves));
            }

            // Validate PPUps
            if (PPUps == null || PPUps.Length != PBESettings.NumMoves || PPUps.Any(p => p > PBESettings.MaxPPUps))
            {
                throw new ArgumentOutOfRangeException(nameof(PPUps));
            }

            // Validate Forme-Specific Requirements
            switch (Species)
            {
                case PBESpecies.Giratina_Altered:
                    if (Item == PBEItem.GriseousOrb)
                    {
                        throw new ArgumentOutOfRangeException(nameof(Item));
                    }

                    break;
                case PBESpecies.Giratina_Origin:
                    if (Item != PBEItem.GriseousOrb)
                    {
                        throw new ArgumentOutOfRangeException(nameof(Item));
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
                        throw new ArgumentOutOfRangeException(nameof(Item));
                    }

                    break;
                case PBESpecies.Arceus_Bug:
                    if (Item != PBEItem.InsectPlate)
                    {
                        throw new ArgumentOutOfRangeException(nameof(Item));
                    }

                    break;
                case PBESpecies.Arceus_Dark:
                    if (Item != PBEItem.DreadPlate)
                    {
                        throw new ArgumentOutOfRangeException(nameof(Item));
                    }

                    break;
                case PBESpecies.Arceus_Dragon:
                    if (Item != PBEItem.DracoPlate)
                    {
                        throw new ArgumentOutOfRangeException(nameof(Item));
                    }

                    break;
                case PBESpecies.Arceus_Electric:
                    if (Item != PBEItem.ZapPlate)
                    {
                        throw new ArgumentOutOfRangeException(nameof(Item));
                    }

                    break;
                case PBESpecies.Arceus_Fighting:
                    if (Item != PBEItem.FistPlate)
                    {
                        throw new ArgumentOutOfRangeException(nameof(Item));
                    }

                    break;
                case PBESpecies.Arceus_Fire:
                    if (Item != PBEItem.FlamePlate)
                    {
                        throw new ArgumentOutOfRangeException(nameof(Item));
                    }

                    break;
                case PBESpecies.Arceus_Flying:
                    if (Item != PBEItem.SkyPlate)
                    {
                        throw new ArgumentOutOfRangeException(nameof(Item));
                    }

                    break;
                case PBESpecies.Arceus_Ghost:
                    if (Item != PBEItem.SpookyPlate)
                    {
                        throw new ArgumentOutOfRangeException(nameof(Item));
                    }

                    break;
                case PBESpecies.Arceus_Grass:
                    if (Item != PBEItem.MeadowPlate)
                    {
                        throw new ArgumentOutOfRangeException(nameof(Item));
                    }

                    break;
                case PBESpecies.Arceus_Ground:
                    if (Item != PBEItem.EarthPlate)
                    {
                        throw new ArgumentOutOfRangeException(nameof(Item));
                    }

                    break;
                case PBESpecies.Arceus_Ice:
                    if (Item != PBEItem.IciclePlate)
                    {
                        throw new ArgumentOutOfRangeException(nameof(Item));
                    }

                    break;
                case PBESpecies.Arceus_Poison:
                    if (Item != PBEItem.ToxicPlate)
                    {
                        throw new ArgumentOutOfRangeException(nameof(Item));
                    }

                    break;
                case PBESpecies.Arceus_Psychic:
                    if (Item != PBEItem.MindPlate)
                    {
                        throw new ArgumentOutOfRangeException(nameof(Item));
                    }

                    break;
                case PBESpecies.Arceus_Rock:
                    if (Item != PBEItem.StonePlate)
                    {
                        throw new ArgumentOutOfRangeException(nameof(Item));
                    }

                    break;
                case PBESpecies.Arceus_Steel:
                    if (Item != PBEItem.IronPlate)
                    {
                        throw new ArgumentOutOfRangeException(nameof(Item));
                    }

                    break;
                case PBESpecies.Arceus_Water:
                    if (Item != PBEItem.SplashPlate)
                    {
                        throw new ArgumentOutOfRangeException(nameof(Item));
                    }

                    break;
                case PBESpecies.Genesect:
                    if (Item == PBEItem.BurnDrive
                        || Item == PBEItem.ChillDrive
                        || Item == PBEItem.DouseDrive
                        || Item == PBEItem.ShockDrive)
                    {
                        throw new ArgumentOutOfRangeException(nameof(Item));
                    }

                    break;
                case PBESpecies.Genesect_Burn:
                    if (Item != PBEItem.BurnDrive)
                    {
                        throw new ArgumentOutOfRangeException(nameof(Item));
                    }

                    break;
                case PBESpecies.Genesect_Chill:
                    if (Item != PBEItem.ChillDrive)
                    {
                        throw new ArgumentOutOfRangeException(nameof(Item));
                    }

                    break;
                case PBESpecies.Genesect_Douse:
                    if (Item != PBEItem.DouseDrive)
                    {
                        throw new ArgumentOutOfRangeException(nameof(Item));
                    }

                    break;
                case PBESpecies.Genesect_Shock:
                    if (Item != PBEItem.ShockDrive)
                    {
                        throw new ArgumentOutOfRangeException(nameof(Item));
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
            for (int i = 0; i < PBESettings.NumMoves; i++)
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
                IVs = r.ReadBytes(6)
            };
            for (int i = 0; i < PBESettings.NumMoves; i++)
            {
                pkmn.Moves[i] = (PBEMove)r.ReadUInt16();
            }

            pkmn.PPUps = r.ReadBytes(PBESettings.NumMoves);
            return pkmn;
        }
    }
    public sealed class PBETeamShell
    {
        public string PlayerName;
        public readonly List<PBEPokemonShell> Party = new List<PBEPokemonShell>(PBESettings.MaxPartySize);

        // Throws ArgumentOutOfRangeException for the invalid information
        public static void ValidateMany(IEnumerable<PBETeamShell> shells)
        {
            var arr = shells.ToArray();
            for (int i = 0; i < arr.Length; i++)
            {
                arr[i].Validate();
            }
        }
        // Throws ArgumentOutOfRangeException for the invalid information
        public void Validate()
        {
            // Validate display name
            if (string.IsNullOrWhiteSpace(PlayerName) || PlayerName.Length > PBESettings.MaxPlayerNameLength)
            {
                throw new ArgumentOutOfRangeException(nameof(PlayerName));
            }

            // Validate Party
            if (Party == null || Party.Count == 0 || Party.Count > PBESettings.MaxPartySize)
            {
                throw new ArgumentOutOfRangeException(nameof(Party));
            }
            // Validate Party Pokemon
            PBEPokemonShell.ValidateMany(Party);
        }

        internal byte[] ToBytes()
        {
            var bytes = new List<byte>();

            bytes.AddRange(PBEUtils.StringToBytes(PlayerName));

            var numPkmn = (byte)Party.Count;
            bytes.Add(numPkmn);
            for (int i = 0; i < numPkmn; i++)
            {
                bytes.AddRange(Party[i].ToBytes());
            }

            return bytes.ToArray();
        }
        internal static PBETeamShell FromBytes(BinaryReader r)
        {
            var team = new PBETeamShell
            {
                PlayerName = PBEUtils.StringFromBytes(r)
            };
            var numPkmn = Math.Min(PBESettings.MaxPartySize, r.ReadByte());
            for (int i = 0; i < numPkmn; i++)
            {
                team.Party.Add(PBEPokemonShell.FromBytes(r));
            }

            return team;
        }
    }
}
