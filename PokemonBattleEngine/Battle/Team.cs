using Kermalis.EndianBinaryIO;
using Kermalis.PokemonBattleEngine.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kermalis.PokemonBattleEngine.Battle
{
    public sealed class PBETeams : IReadOnlyList<PBETeam>
    {
        private readonly PBETeam _team1;
        private readonly PBETeam _team2;

        public int Count => 2;
        public PBETeam this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0: return _team1;
                    case 1: return _team2;
                    default: throw new ArgumentOutOfRangeException(nameof(index));
                }
            }
        }

        internal PBETeams(PBEBattle battle, PBETeamShell team1Shell, string team1TrainerName, PBETeamShell team2Shell, string team2TrainerName)
        {
            _team1 = new PBETeam(battle, 0, team1Shell, team1TrainerName);
            _team2 = new PBETeam(battle, 1, team2Shell, team2TrainerName);
            _team1.OpposingTeam = _team2;
            _team2.OpposingTeam = _team1;
        }
        internal PBETeams(PBEBattle battle)
        {
            _team1 = new PBETeam(battle, 0);
            _team2 = new PBETeam(battle, 1);
            _team1.OpposingTeam = _team2;
            _team2.OpposingTeam = _team1;
        }

        public IEnumerator<PBETeam> GetEnumerator()
        {
            yield return _team1;
            yield return _team2;
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
    // TODO: INPC
    /// <summary>Represents a team in a specific <see cref="PBEBattle"/>.</summary>
    public sealed class PBETeam
    {
        /// <summary>The battle this team and its party belongs to.</summary>
        public PBEBattle Battle { get; }
        public byte Id { get; }
        public string TrainerName { get; set; } // Setter is public because a client cannot submit the opponent's team
        public PBEList<PBEPokemon> Party { get; }
        public PBETeam OpposingTeam { get; internal set; }

        public PBEPokemon[] ActiveBattlers => Battle.ActiveBattlers.Where(p => p.Team == this).OrderBy(p => p.FieldPosition).ToArray();
        public int NumConsciousPkmn => Party.Count(p => p.HP > 0);
        public int NumPkmnOnField => Party.Count(p => p.FieldPosition != PBEFieldPosition.None);

        public List<PBEPokemon> ActionsRequired { get; } = new List<PBEPokemon>(3); // PBEBattleState.WaitingForActions
        public byte SwitchInsRequired { get; set; } // PBEBattleState.WaitingForSwitchIns
        public List<PBEPokemon> SwitchInQueue { get; } = new List<PBEPokemon>(3); // PBEBattleState.WaitingForSwitchIns

        public PBETeamStatus TeamStatus { get; set; }
        public byte LightScreenCount { get; set; }
        public byte LuckyChantCount { get; set; }
        public byte ReflectCount { get; set; }
        public byte SafeguardCount { get; set; }
        public byte SpikeCount { get; set; }
        public byte TailwindCount { get; set; }
        public byte ToxicSpikeCount { get; set; }
        public bool MonFaintedLastTurn { get; set; }
        public bool MonFaintedThisTurn { get; set; }

        internal PBETeam(PBEBattle battle, byte id, PBETeamShell shell, string trainerName)
        {
            Battle = battle;
            Id = id;
            Party = new PBEList<PBEPokemon>(Battle.Settings.MaxPartySize);
            CreateParty(shell, trainerName);
        }
        internal PBETeam(PBEBattle battle, byte id)
        {
            Battle = battle;
            Id = id;
            Party = new PBEList<PBEPokemon>(Battle.Settings.MaxPartySize);
        }
        internal void CreateParty(PBETeamShell shell, string trainerName)
        {
            TrainerName = trainerName;
            for (int i = 0; i < shell.Count; i++)
            {
                new PBEPokemon(this, (byte)((Id * Battle.Settings.MaxPartySize) + i), shell[i]);
            }
        }

        /// <summary>Gets a specific active <see cref="PBEPokemon"/> by its <see cref="PBEPokemon.FieldPosition"/>.</summary>
        /// <param name="pos">The <see cref="PBEFieldPosition"/> of the <see cref="PBEPokemon"/>.</param>
        public PBEPokemon TryGetPokemon(PBEFieldPosition pos)
        {
            if (IsDisposed)
            {
                throw new ObjectDisposedException(null);
            }
            return ActiveBattlers.SingleOrDefault(p => p.FieldPosition == pos);
        }
        /// <summary>Gets a specific <see cref="PBEPokemon"/> by its ID.</summary>
        /// <param name="pkmnId">The ID of the <see cref="PBEPokemon"/>.</param>
        public PBEPokemon TryGetPokemon(byte pkmnId)
        {
            if (IsDisposed)
            {
                throw new ObjectDisposedException(null);
            }
            return Party.SingleOrDefault(p => p.Id == pkmnId);
        }

        public bool Remove(PBEPokemon pokemon)
        {
            if (IsDisposed)
            {
                throw new ObjectDisposedException(null);
            }
            if (pokemon.IsDisposed)
            {
                throw new ObjectDisposedException(nameof(pokemon));
            }
            bool b = Party.Remove(pokemon);
            if (b)
            {
                pokemon.Dispose();
            }
            return b;
        }

        internal void ToBytes(EndianBinaryWriter w)
        {
            w.Write(TrainerName, true);
            sbyte count = (sbyte)Party.Count;
            w.Write(count);
            for (int i = 0; i < count; i++)
            {
                Party[i].ToBytes(w);
            }
        }
        internal void FromBytes(EndianBinaryReader r)
        {
            TrainerName = r.ReadStringNullTerminated();
            sbyte count = r.ReadSByte();
            for (int i = 0; i < count; i++)
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
            sb.AppendLine($"NumConsciousPkmn: {NumConsciousPkmn}");
            sb.AppendLine($"NumPkmnOnField: {NumPkmnOnField}");
            return sb.ToString();
        }

        internal bool IsDisposed { get; private set; }
        internal void Dispose()
        {
            if (!IsDisposed)
            {
                IsDisposed = true;
                for (int i = 0; i < Party.Count; i++)
                {
                    Party[i].Dispose();
                }
            }
        }
    }
}
