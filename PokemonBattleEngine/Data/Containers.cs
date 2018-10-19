using Kermalis.PokemonBattleEngine.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Kermalis.PokemonBattleEngine.Data
{
    public sealed class PPokemonShell
    {
        public PSpecies Species;
        public byte Level = PConstants.MaxLevel, Friendship = byte.MaxValue;
        public PAbility Ability;
        public PNature Nature = (PNature)PUtils.RNG.Next(0, (int)PNature.MAX);
        public PGender Gender = PGender.Genderless;
        public PItem Item;
        public byte[] EVs = new byte[6], IVs = new byte[6];
        public PMove[] Moves = new PMove[PConstants.NumMoves];

        public static void ValidateMany(IEnumerable<PPokemonShell> shells)
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
            PPokemonData pData;
            try
            {
                pData = PPokemonData.Data[Species];
            }
            catch
            {
                throw new ArgumentOutOfRangeException(nameof(Species));
            }

            // Validate Level
            if (Level == 0 || Level > PConstants.MaxLevel)
                throw new ArgumentOutOfRangeException(nameof(Level));

            // Validate Ability
            if (!pData.HasAbility(Ability))
                throw new ArgumentOutOfRangeException(nameof(Ability));

            // Validate Nature
            if (Nature >= PNature.MAX)
                throw new ArgumentOutOfRangeException(nameof(Nature));

            // Validate Gender
            if ((Gender != PGender.Male && Gender != PGender.Female && Gender != PGender.Genderless)
                || (Gender == PGender.Male && (pData.GenderRatio == PGender.Female || pData.GenderRatio == PGender.Genderless))
                || (Gender == PGender.Female && (pData.GenderRatio == PGender.Male || pData.GenderRatio == PGender.Genderless))
                || (Gender == PGender.Genderless && pData.GenderRatio != PGender.Genderless)
                )
                throw new ArgumentOutOfRangeException(nameof(Gender));

            // Validate Item
            if (Item != PItem.NoItem)
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
            if (EVs == null || EVs.Length != 6 || EVs.Select(e => (int)e).Sum() > 510)
                throw new ArgumentOutOfRangeException(nameof(EVs));
            // Validate IVs
            if (IVs == null || IVs.Length != 6 || IVs.Any(e => e > 31))
                throw new ArgumentOutOfRangeException(nameof(IVs));

            // Validate Moves
            IEnumerable<PMove> legalMoves = pData.LevelUpMoves.Where(t => t.Item1 <= Level).Select(t => t.Item2).Union(pData.OtherMoves);
            if (Moves == null || Moves.Length != PConstants.NumMoves // Illegal array
                || Moves.Any(m => m != PMove.None && !legalMoves.Contains(m)) // Has a move not in the legal list
                || Moves.Any(m => Moves.Count(m2 => m != PMove.None && m == m2) > 1) // Has a duplicate move
                || Moves.All(m => m == PMove.None) // Has no moves
                )
                throw new ArgumentOutOfRangeException(nameof(Moves));
        }

        internal byte[] ToBytes()
        {
            var bytes = new List<byte>();
            bytes.AddRange(BitConverter.GetBytes((ushort)Species));
            bytes.Add(Level);
            bytes.Add(Friendship);
            bytes.Add((byte)Ability);
            bytes.Add((byte)Nature);
            bytes.Add((byte)Gender);
            bytes.AddRange(BitConverter.GetBytes((ushort)Item));
            bytes.AddRange(EVs);
            bytes.AddRange(IVs);
            for (int i = 0; i < PConstants.NumMoves; i++)
                bytes.AddRange(BitConverter.GetBytes((ushort)Moves[i]));
            return bytes.ToArray();
        }
        internal static PPokemonShell FromBytes(BinaryReader r)
        {
            var pkmn = new PPokemonShell
            {
                Species = (PSpecies)r.ReadUInt16(),
                Level = r.ReadByte(),
                Friendship = r.ReadByte(),
                Ability = (PAbility)r.ReadByte(),
                Nature = (PNature)r.ReadByte(),
                Gender = (PGender)r.ReadByte(),
                Item = (PItem)r.ReadUInt16(),
                EVs = r.ReadBytes(6),
                IVs = r.ReadBytes(6)
            };
            for (int j = 0; j < PConstants.NumMoves; j++)
                pkmn.Moves[j] = (PMove)r.ReadUInt16();
            return pkmn;
        }
    }
    public sealed class PTeamShell
    {
        public string DisplayName;
        public readonly List<PPokemonShell> Party = new List<PPokemonShell>(PConstants.MaxPokemon);

        internal byte[] ToBytes()
        {
            var bytes = new List<byte>();

            byte[] nameBytes = Encoding.ASCII.GetBytes(DisplayName);
            bytes.Add((byte)nameBytes.Length);
            bytes.AddRange(nameBytes);

            var numPkmn = Math.Min(PConstants.MaxPokemon, Party.Count);
            bytes.Add((byte)numPkmn);
            for (int i = 0; i < numPkmn; i++)
                bytes.AddRange(Party[i].ToBytes());
            return bytes.ToArray();
        }
        internal static PTeamShell FromBytes(BinaryReader r)
        {
            var team = new PTeamShell
            {
                DisplayName = Encoding.ASCII.GetString(r.ReadBytes(r.ReadByte()))
            };
            var numPkmn = Math.Min(PConstants.MaxPokemon, r.ReadByte());
            for (int i = 0; i < numPkmn; i++)
                team.Party.Add(PPokemonShell.FromBytes(r));
            return team;
        }
    }

    // Proof of concept container for online play
    // "Local" represents team 0 while "Opponent" represents team 1
    // In online play, information slowly gets revealed (opponent ability, opponent item, etc)
    public sealed class PKnownInfo
    {
        static PKnownInfo instance;
        public static PKnownInfo Instance
        {
            get
            {
                if (instance == null)
                    instance = new PKnownInfo();
                return instance;
            }
        }

        public string LocalDisplayName, RemoteDisplayName;
        List<PPokemon> localParty = new List<PPokemon>(PConstants.MaxPokemon),
            remoteParty = new List<PPokemon>(PConstants.MaxPokemon);

        public string DisplayName(bool local) => local ? LocalDisplayName : RemoteDisplayName;
        // Returns null if not found
        public PPokemon Pokemon(Guid id) => localParty.Concat(remoteParty).SingleOrDefault(p => p.Id == id);
        public PPokemon[] LocalParty => localParty.ToArray();
        public PPokemon[] RemoteParty => remoteParty.ToArray();

        public void Clear()
        {
            LocalDisplayName = RemoteDisplayName = string.Empty;
            localParty.Clear();
            remoteParty.Clear();
        }
        public void SetPartyPokemon(IEnumerable<PPokemon> pkmn, bool local)
        {
            var list = pkmn.Take(PConstants.MaxPokemon).ToList();
            list.Capacity = PConstants.MaxPokemon;

            foreach (var p in list)
                p.LocallyOwned = local;

            if (local)
                localParty = list;
            else
                remoteParty = list;
        }
        public void AddRemotePokemon(Guid id, PSpecies species, byte level, ushort hp, ushort maxHP, PGender gender)
        {
            PPokemon pkmn;

            if (Pokemon(id) == null)
            {
                if (remoteParty.Count == PConstants.MaxPokemon)
                    throw new InvalidOperationException("Too many Pokémon!");

                // Use remote pokemon constructor, which sets LocallyOwned to false
                pkmn = new PPokemon(id, species, level, gender);
                remoteParty.Add(pkmn);
            }
            else
                pkmn = Pokemon(id);

            // If this pokemon was already added, it also already knows the info other than hp (opponent could have regenerator or could have been healed by an ally)
            pkmn.HP = hp;
            pkmn.MaxHP = maxHP;
        }
    }
}
