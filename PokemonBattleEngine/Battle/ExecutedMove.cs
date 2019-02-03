using Kermalis.PokemonBattleEngine.Data;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Kermalis.PokemonBattleEngine.Battle
{
    public sealed class PBEExecutedMove
    {
        public sealed class PBETargetSuccess
        {
            public PBEPokemon Target { get; set; }
            public ushort OldHP { get; set; }
            public ushort NewHP { get; set; }
            public double OldHPPercentage { get; set; }
            public double NewHPPercentage { get; set; }
            public bool CriticalHit { get; set; }
            public PBEFailReason FailReason { get; set; }
            public bool Missed { get; set; }
        }

        public ushort TurnNumber { get; }
        public PBEMove Move { get; }
        public PBEFailReason FailReason { get; }
        public ReadOnlyCollection<PBETargetSuccess> Targets { get; }

        public PBEExecutedMove(ushort turnNumber, PBEMove move, PBEFailReason failReason, IList<PBETargetSuccess> targets)
        {
            TurnNumber = turnNumber;
            Move = move;
            FailReason = failReason;
            Targets = new ReadOnlyCollection<PBETargetSuccess>(targets);
        }
    }
}
