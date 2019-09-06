using Kermalis.PokemonBattleEngine.Data;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Kermalis.PokemonBattleEngine.Battle
{
    // TODO: INPC
    /// <summary>Represents a team in a specific <see cref="PBEBattle"/>.</summary>
    public sealed class PBETeam
    {
        /// <summary>The battle this team and its party belongs to.</summary>
        public PBEBattle Battle { get; }
        public byte Id { get; }
        public string TrainerName { get; set; } // Setter is public because a client cannot submit the opponent's team
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

        internal PBETeam(PBEBattle battle, byte id, PBETeamShell shell, string trainerName, ref byte pkmnIdCounter)
        {
            Battle = battle;
            Id = id;
            CreateParty(shell, trainerName, ref pkmnIdCounter);
        }
        internal PBETeam(PBEBattle battle, byte id)
        {
            Battle = battle;
            Id = id;
            Party = new List<PBEPokemon>(Battle.Settings.MaxPartySize);
        }
        internal void CreateParty(PBETeamShell shell, string trainerName, ref byte pkmnIdCounter)
        {
            TrainerName = trainerName;
            Party = new List<PBEPokemon>(Battle.Settings.MaxPartySize);
            for (int i = 0; i < shell.Count; i++)
            {
                new PBEPokemon(this, pkmnIdCounter++, shell[i]);
            }
        }

        /// <summary>Gets a specific active <see cref="PBEPokemon"/> by its <see cref="PBEPokemon.FieldPosition"/>.</summary>
        /// <param name="pos">The <see cref="PBEFieldPosition"/> of the <see cref="PBEPokemon"/>.</param>
        public PBEPokemon TryGetPokemon(PBEFieldPosition pos)
        {
            return ActiveBattlers.SingleOrDefault(p => p.FieldPosition == pos);
        }
        /// <summary>Gets a specific <see cref="PBEPokemon"/> by its ID.</summary>
        /// <param name="pkmnId">The ID of the <see cref="PBEPokemon"/>.</param>
        public PBEPokemon TryGetPokemon(byte pkmnId)
        {
            return Party.SingleOrDefault(p => p.Id == pkmnId);
        }

        internal List<byte> ToBytes()
        {
            var bytes = new List<byte>();
            bytes.AddRange(PBEUtils.StringToBytes(TrainerName));
            bytes.Add((byte)Party.Count);
            bytes.AddRange(Party.SelectMany(p => p.ToBytes()));
            return bytes;
        }
        internal void FromBytes(BinaryReader r)
        {
            TrainerName = PBEUtils.StringFromBytes(r);
            sbyte amt = r.ReadSByte();
            for (int i = 0; i < amt; i++)
            {
                new PBEPokemon(r, this);
            }
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
