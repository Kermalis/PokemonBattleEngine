using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Kermalis.PokemonBattleEngine.Data
{
    public sealed class PPokemonShell
    {
        public PSpecies Species;
        public string Nickname;
        public byte Level = PSettings.MaxLevel,
            Friendship = 255;
        public bool Shiny;
        public PAbility Ability;
        public PNature Nature = (PNature)PUtils.RNG.Next(0, (int)PNature.MAX);
        public PGender Gender;
        public PItem Item;
        public byte[] EVs = new byte[6],
            IVs = new byte[6];
        public PMove[] Moves = new PMove[PSettings.NumMoves];
        public byte[] PPUps = new byte[PSettings.NumMoves];

        // Throws ArgumentOutOfRangeException for the invalid information
        public static void ValidateMany(IEnumerable<PPokemonShell> shells)
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
            PPokemonData pData;
            try
            {
                pData = PPokemonData.Data[Species];
            }
            catch
            {
                throw new ArgumentOutOfRangeException(nameof(Species));
            }

            // Validate Shininess
            if (Shiny && pData.ShinyLocked)
                throw new ArgumentOutOfRangeException(nameof(Shiny));

            // Validate Nickname
            if (string.IsNullOrWhiteSpace(Nickname) || Nickname.Length > PSettings.MaxPokemonNameLength)
                throw new ArgumentOutOfRangeException(nameof(Nickname));

            // Validate Level
            if (Level < pData.MinLevel || Level > PSettings.MaxLevel)
                throw new ArgumentOutOfRangeException(nameof(Level));

            // Validate Ability
            if (!pData.HasAbility(Ability))
                throw new ArgumentOutOfRangeException(nameof(Ability));

            // Validate Nature
            if (Nature >= PNature.MAX)
                throw new ArgumentOutOfRangeException(nameof(Nature));

            // Validate Gender
            if (Gender >= PGender.MAX
                || (Gender == PGender.Male && (pData.GenderRatio == PGenderRatio.M0_F1 || pData.GenderRatio == PGenderRatio.M0_F0))
                || (Gender == PGender.Female && (pData.GenderRatio == PGenderRatio.M1_F0 || pData.GenderRatio == PGenderRatio.M0_F0))
                || (Gender == PGender.Genderless && pData.GenderRatio != PGenderRatio.M0_F0)
                )
                throw new ArgumentOutOfRangeException(nameof(Gender));

            // Validate Item
            if (Item != PItem.None)
            {
                try
                {
                    var iData = PItemData.Data[Item];
                }
                catch
                {
                    throw new ArgumentOutOfRangeException(nameof(Item));
                }
            }

            // Validate EVs
            if (EVs == null || EVs.Length != 6 || EVs.Select(e => (int)e).Sum() > PSettings.MaxTotalEVs)
                throw new ArgumentOutOfRangeException(nameof(EVs));
            // Validate IVs
            if (IVs == null || IVs.Length != 6 || IVs.Any(i => i > PSettings.MaxIVs))
                throw new ArgumentOutOfRangeException(nameof(IVs));

            // Validate Moves
            // TODO: legal moves
            IEnumerable<PMove> legalMoves = pData.LevelUpMoves.Where(t => t.Item2 <= Level).Select(t => t.Item1).Union(pData.OtherMoves.Select(t => t.Item1));
            if (Moves == null || Moves.Length != PSettings.NumMoves // Illegal array
                || Moves.Any(m => m != PMove.None && !legalMoves.Contains(m)) // Has a move not in the legal list
                || Moves.Any(m => Moves.Count(m2 => m != PMove.None && m == m2) > 1) // Has a duplicate move
                || Moves.All(m => m == PMove.None) // Has no moves
                )
                throw new ArgumentOutOfRangeException(nameof(Moves));

            // Validate PPUps
            if (PPUps == null || PPUps.Length != PSettings.NumMoves || PPUps.Any(p => p > PSettings.MaxPPUps))
                throw new ArgumentOutOfRangeException(nameof(PPUps));

            // Validate Forme-Specific Requirements
            switch (Species)
            {
                case PSpecies.Genesect:
                    if (Item == PItem.BurnDrive
                        || Item == PItem.ChillDrive
                        || Item == PItem.DouseDrive
                        || Item == PItem.ShockDrive
                        )
                        throw new ArgumentOutOfRangeException(nameof(Item));
                    break;
                case PSpecies.Genesect_Burn:
                    if (Item != PItem.BurnDrive)
                        throw new ArgumentOutOfRangeException(nameof(Item));
                    break;
                case PSpecies.Genesect_Chill:
                    if (Item != PItem.ChillDrive)
                        throw new ArgumentOutOfRangeException(nameof(Item));
                    break;
                case PSpecies.Genesect_Douse:
                    if (Item != PItem.DouseDrive)
                        throw new ArgumentOutOfRangeException(nameof(Item));
                    break;
                case PSpecies.Genesect_Shock:
                    if (Item != PItem.ShockDrive)
                        throw new ArgumentOutOfRangeException(nameof(Item));
                    break;
            }
        }

        internal byte[] ToBytes()
        {
            var bytes = new List<byte>();
            bytes.AddRange(BitConverter.GetBytes((uint)Species));
            bytes.AddRange(PUtils.StringToBytes(Nickname));
            bytes.Add(Level);
            bytes.Add(Friendship);
            bytes.Add((byte)(Shiny ? 1 : 0));
            bytes.Add((byte)Ability);
            bytes.Add((byte)Nature);
            bytes.Add((byte)Gender);
            bytes.AddRange(BitConverter.GetBytes((ushort)Item));
            bytes.AddRange(EVs);
            bytes.AddRange(IVs);
            for (int i = 0; i < PSettings.NumMoves; i++)
                bytes.AddRange(BitConverter.GetBytes((ushort)Moves[i]));
            bytes.AddRange(PPUps);
            return bytes.ToArray();
        }
        internal static PPokemonShell FromBytes(BinaryReader r)
        {
            var pkmn = new PPokemonShell
            {
                Species = (PSpecies)r.ReadUInt32(),
                Nickname = PUtils.StringFromBytes(r),
                Level = r.ReadByte(),
                Friendship = r.ReadByte(),
                Shiny = r.ReadBoolean(),
                Ability = (PAbility)r.ReadByte(),
                Nature = (PNature)r.ReadByte(),
                Gender = (PGender)r.ReadByte(),
                Item = (PItem)r.ReadUInt16(),
                EVs = r.ReadBytes(6),
                IVs = r.ReadBytes(6)
            };
            for (int i = 0; i < PSettings.NumMoves; i++)
                pkmn.Moves[i] = (PMove)r.ReadUInt16();
            pkmn.PPUps = r.ReadBytes(PSettings.NumMoves);
            return pkmn;
        }
    }
    public sealed class PTeamShell
    {
        public string PlayerName;
        public readonly List<PPokemonShell> Party = new List<PPokemonShell>(PSettings.MaxPartySize);

        // Throws ArgumentOutOfRangeException for the invalid information
        public static void ValidateMany(IEnumerable<PTeamShell> shells)
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
            if (string.IsNullOrWhiteSpace(PlayerName) || PlayerName.Length > PSettings.MaxPlayerNameLength)
                throw new ArgumentOutOfRangeException(nameof(PlayerName));

            // Validate Party
            if (Party == null || Party.Count == 0 || Party.Count > PSettings.MaxPartySize)
                throw new ArgumentOutOfRangeException(nameof(Party));
            // Validate Party Pokemon
            PPokemonShell.ValidateMany(Party);
        }

        internal byte[] ToBytes()
        {
            var bytes = new List<byte>();

            bytes.AddRange(PUtils.StringToBytes(PlayerName));

            var numPkmn = (byte)Party.Count;
            bytes.Add(numPkmn);
            for (int i = 0; i < numPkmn; i++)
                bytes.AddRange(Party[i].ToBytes());

            return bytes.ToArray();
        }
        internal static PTeamShell FromBytes(BinaryReader r)
        {
            var team = new PTeamShell
            {
                PlayerName = PUtils.StringFromBytes(r)
            };
            var numPkmn = Math.Min(PSettings.MaxPartySize, r.ReadByte());
            for (int i = 0; i < numPkmn; i++)
                team.Party.Add(PPokemonShell.FromBytes(r));
            return team;
        }
    }
}
