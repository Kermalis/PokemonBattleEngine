using Kermalis.PokemonBattleEngine.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kermalis.PokemonBattleEngine.Battle
{
    // TODO: INPC
    /// <summary>Represents a team in a specific <see cref="PBEBattle"/>.</summary>
    public sealed class PBETeam
    {
        /// <summary>
        /// The battle this team and its party belongs to.
        /// </summary>
        public PBEBattle Battle { get; }
        public byte Id { get; }
        public string TrainerName { get; set; }
        public List<PBEPokemon> Party { get; private set; }

        public IEnumerable<PBEPokemon> ActiveBattlers => Battle.ActiveBattlers.Where(p => p.Team == this).OrderBy(p => p.FieldPosition);
        public int NumPkmnAlive => Party.Count(p => p.HP > 0);
        public int NumPkmnOnField => Party.Count(p => p.FieldPosition != PBEFieldPosition.None);

        public List<PBEPokemon> ActionsRequired { get; } = new List<PBEPokemon>(3); // PBEBattleState.WaitingForActions
        public byte SwitchInsRequired { get; set; } // PBEBattleState.WaitingForSwitchIns
        public List<PBEPokemon> SwitchInQueue { get; } = new List<PBEPokemon>(3); // PBEBattleState.WaitingForSwitchIns

        public PBETeamStatus TeamStatus { get; set; }
        public byte LightScreenCount { get; set; }
        public byte LuckyChantCount { get; set; }
        public byte ReflectCount { get; set; }
        public byte SpikeCount { get; set; }
        public byte ToxicSpikeCount { get; set; }
        public bool MonFaintedLastTurn { get; set; }
        public bool MonFaintedThisTurn { get; set; }

        // Host constructor
        internal PBETeam(PBEBattle battle, byte id, IEnumerable<PBEPokemonShell> party, ref byte pkmnIdCounter)
        {
            Battle = battle;
            Id = id;
            CreateParty(party, ref pkmnIdCounter);
        }
        // Client constructor
        internal PBETeam(PBEBattle battle, byte id)
        {
            Battle = battle;
            Id = id;
            Party = new List<PBEPokemon>(Battle.Settings.MaxPartySize);
        }
        internal void CreateParty(IEnumerable<PBEPokemonShell> party, ref byte pkmnIdCounter)
        {
            Party = new List<PBEPokemon>(Battle.Settings.MaxPartySize);
            foreach (PBEPokemonShell pkmn in party)
            {
                new PBEPokemon(this, pkmnIdCounter++, pkmn);
            }
        }

        /// <summary>
        /// Gets a specific active Pokémon by its position.
        /// </summary>
        /// <param name="pos">The position of the Pokémon you want to get.</param>
        /// <returns>null if no Pokémon was found was found at <paramref name="pos"/>; otherwise the <see cref="PBEPokemon"/>.</returns>
        public PBEPokemon TryGetPokemon(PBEFieldPosition pos)
        {
            return ActiveBattlers.SingleOrDefault(p => p.FieldPosition == pos);
        }
        public PBEPokemon TryGetPokemon(byte id)
        {
            return Party.SingleOrDefault(p => p.Id == id);
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine($"{TrainerName}'s team:");
            sb.AppendLine($"TeamStatus: {TeamStatus}");
            sb.AppendLine($"NumPkmn: {Party.Count}");
            sb.AppendLine($"NumPkmnAlive: {NumPkmnAlive}");
            sb.AppendLine($"NumPkmnOnField: {NumPkmnOnField}");
            return sb.ToString();
        }
    }
}
