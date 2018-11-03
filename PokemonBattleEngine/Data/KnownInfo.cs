using System;
using System.Collections.Generic;
using System.Linq;

namespace Kermalis.PokemonBattleEngine.Data
{
    // Proof of concept container for online play
    // "Local" represents team 0 while "Opponent" represents team 1
    // In online play, information slowly gets revealed (opponent ability, opponent item, etc)
    public sealed class PKnownInfo
    {
        public static PKnownInfo Instance { get; } = new PKnownInfo();

        public string LocalDisplayName = string.Empty,
            RemoteDisplayName = string.Empty;
        List<PPokemon> localParty = new List<PPokemon>(PConstants.MaxPartySize),
            remoteParty = new List<PPokemon>(PConstants.MaxPartySize);

        public string DisplayName(bool local) => local ? LocalDisplayName : RemoteDisplayName;
        // Returns null if it doesn't exist
        public PPokemon Pokemon(Guid pkmnId) => localParty.Concat(remoteParty).SingleOrDefault(p => p.Id == pkmnId);
        // Returns null if the field position is empty
        public PPokemon PokemonAtPosition(bool local, PFieldPosition pos) => (local ? localParty : remoteParty).SingleOrDefault(p => p.FieldPosition == pos);
        public PPokemon[] LocalParty => localParty.ToArray();
        public PPokemon[] RemoteParty => remoteParty.ToArray();

        public void Clear()
        {
            LocalDisplayName = RemoteDisplayName = string.Empty;
            localParty.Clear();
            remoteParty.Clear();
        }
        public void SetPartyPokemon(PPokemon[] party, bool local)
        {
            var list = new List<PPokemon>(party);

            foreach (var p in list)
                p.Local = local;

            if (local)
                localParty = list;
            else
                remoteParty = list;
        }
        public void AddRemotePokemon(Guid id, PSpecies species, string nickname, byte level, ushort hp, ushort maxHP, PGender gender)
        {
            PPokemon pkmn;

            if (Pokemon(id) == null)
            {
                if (remoteParty.Count == PConstants.MaxPartySize)
                    throw new InvalidOperationException("Too many Pokémon!");

                // Use remote pokemon constructor, which sets Local to false and moves to PMove.MAX
                pkmn = new PPokemon(id, species, nickname, level, gender);
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
