using Kermalis.PokemonBattleEngine.Battle;
using Kermalis.PokemonBattleEngine.Data;
using Kermalis.PokemonBattleEngine.Packets;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Xunit;
using Xunit.Abstractions;

namespace Kermalis.PokemonBattleEngineTests
{
    [CollectionDefinition("Utils")]
    public class TestUtilsCollection : ICollectionFixture<TestUtils>
    {
        //
    }

    public class TestUtils
    {
        public TestUtils()
        {
            PBEDataProvider.InitEngine(string.Empty);
        }

        #region Output
        public void SetOutputHelper(ITestOutputHelper output)
        {
            Console.SetOut(new TestOutputConverter(output));
        }

        private class TestOutputConverter : TextWriter
        {
            private readonly ITestOutputHelper _output;
            public TestOutputConverter(ITestOutputHelper output)
            {
                _output = output;
            }
            public override Encoding Encoding => Encoding.Unicode;
            public override void WriteLine(string message)
            {
                _output.WriteLine(message);
            }
            public override void WriteLine(string format, params object[] args)
            {
                _output.WriteLine(format, args);
            }
        }
        #endregion
    }

    internal sealed class TestMoveset : IPBEMoveset, IPBEMoveset<TestMoveset.TestMovesetSlot>
    {
        public sealed class TestMovesetSlot : IPBEMovesetSlot
        {
            public PBEMove Move { get; }
            public byte PPUps { get; }

            public TestMovesetSlot(PBEMove move, byte ppUps)
            {
                Move = move;
                PPUps = ppUps;
            }
        }

        private readonly TestMovesetSlot[] _list;
        public int Count => _list.Length;
        public TestMovesetSlot this[int index]
        {
            get
            {
                if (index >= _list.Length)
                {
                    throw new ArgumentOutOfRangeException(nameof(index));
                }
                return _list[index];
            }
        }
        IPBEMovesetSlot IReadOnlyList<IPBEMovesetSlot>.this[int index] => this[index];

        public TestMoveset(PBESettings settings, params PBEMove[] moves)
        {
            int numMoves = settings.NumMoves;
            _list = new TestMovesetSlot[numMoves];
            int count = moves.Length;
            int i = 0;
            for (; i < count; i++)
            {
                _list[i] = new TestMovesetSlot(moves[i], 0);
            }
            for (; i < numMoves; i++)
            {
                _list[i] = new TestMovesetSlot(PBEMove.None, 0);
            }
        }

        public IEnumerator<TestMovesetSlot> GetEnumerator()
        {
            return ((IEnumerable<TestMovesetSlot>)_list).GetEnumerator();
        }
        IEnumerator<IPBEMovesetSlot> IEnumerable<IPBEMovesetSlot>.GetEnumerator()
        {
            return ((IEnumerable<TestMovesetSlot>)_list).GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return _list.GetEnumerator();
        }
    }
    internal sealed class TestPartyMoveset : IPBEMoveset, IPBEPartyMoveset, IPBEMoveset<TestPartyMoveset.TestPartyMovesetSlot>, IPBEPartyMoveset<TestPartyMoveset.TestPartyMovesetSlot>
    {
        public sealed class TestPartyMovesetSlot : IPBEPartyMovesetSlot
        {
            public PBEMove Move { get; }
            public byte PPUps { get; }
            public int PP { get; set; }

            public TestPartyMovesetSlot(PBEMove move, byte ppUps, PBESettings settings)
            {
                Move = move;
                PPUps = ppUps;
                PP = PBEDataUtils.CalcMaxPP(move, ppUps, settings);
            }
        }

        private readonly TestPartyMovesetSlot[] _list;
        public int Count => _list.Length;

        public TestPartyMovesetSlot this[int index]
        {
            get
            {
                if (index >= _list.Length)
                {
                    throw new ArgumentOutOfRangeException(nameof(index));
                }
                return _list[index];
            }
        }
        IPBEPartyMovesetSlot IReadOnlyList<IPBEPartyMovesetSlot>.this[int index] => this[index];
        IPBEMovesetSlot IReadOnlyList<IPBEMovesetSlot>.this[int index] => this[index];

        public TestPartyMoveset(PBESettings settings, params PBEMove[] moves)
        {
            int numMoves = settings.NumMoves;
            _list = new TestPartyMovesetSlot[numMoves];
            int count = moves.Length;
            int i = 0;
            for (; i < count; i++)
            {
                _list[i] = new TestPartyMovesetSlot(moves[i], 0, settings);
            }
            for (; i < numMoves; i++)
            {
                _list[i] = new TestPartyMovesetSlot(PBEMove.None, 0, settings);
            }
        }

        public IEnumerator<TestPartyMovesetSlot> GetEnumerator()
        {
            return ((IEnumerable<TestPartyMovesetSlot>)_list).GetEnumerator();
        }
        IEnumerator<IPBEMovesetSlot> IEnumerable<IPBEMovesetSlot>.GetEnumerator()
        {
            return ((IEnumerable<TestPartyMovesetSlot>)_list).GetEnumerator();
        }
        IEnumerator<IPBEPartyMovesetSlot> IEnumerable<IPBEPartyMovesetSlot>.GetEnumerator()
        {
            return ((IEnumerable<TestPartyMovesetSlot>)_list).GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return _list.GetEnumerator();
        }
    }
    internal sealed class TestPokemon : IPBEPokemon
    {
        public PBESpecies Species { get; set; }
        public PBEForm Form { get; set; }
        public PBEGender Gender { get; set; }
        public string Nickname { get; set; }
        public bool Shiny { get; set; }
        public byte Level { get; set; }
        public uint EXP { get; set; }
        public PBEItem Item { get; set; }
        public byte Friendship { get; set; }
        public PBEAbility Ability { get; set; }
        public PBENature Nature { get; set; }
        public PBEItem CaughtBall { get; set; }
        public IPBEStatCollection EffortValues { get; set; }
        public IPBEReadOnlyStatCollection IndividualValues { get; set; }
        public TestMoveset Moveset { get; set; }
        IPBEMoveset IPBEPokemon.Moveset => Moveset;

        public TestPokemon(PBESettings settings, PBESpecies species, PBEForm form, byte level, params PBEMove[] moves)
        {
            Species = species;
            Form = form;
            Level = level;
            EXP = PBEDataProvider.Instance.GetEXPRequired(PBEDataProvider.Instance.GetPokemonData(species, form).GrowthRate, level);
            Nickname = species.ToString();
            Gender = PBEDataProvider.GlobalRandom.RandomGender(PBEDataProvider.Instance.GetPokemonData(species, form).GenderRatio);
            CaughtBall = PBEItem.PokeBall;
            EffortValues = new PBEStatCollection(0, 0, 0, 0, 0, 0);
            IndividualValues = new PBEStatCollection(0, 0, 0, 0, 0, 0);
            Moveset = new TestMoveset(settings, moves);
        }
    }
    internal sealed class TestPartyPokemon : IPBEPartyPokemon
    {
        public ushort HP { get; set; }
        public PBEStatus1 Status1 { get; set; }
        public byte SleepTurns { get; set; }
        public PBESpecies Species { get; set; }
        public PBEForm Form { get; set; }
        public PBEGender Gender { get; set; }
        public string Nickname { get; set; }
        public bool Shiny { get; set; }
        public byte Level { get; set; }
        public uint EXP { get; set; }
        public PBEItem Item { get; set; }
        public byte Friendship { get; set; }
        public PBEAbility Ability { get; set; }
        public PBENature Nature { get; set; }
        public PBEItem CaughtBall { get; set; }
        public IPBEStatCollection EffortValues { get; set; }
        public IPBEReadOnlyStatCollection IndividualValues { get; set; }
        public TestPartyMoveset Moveset { get; set; }
        IPBEMoveset IPBEPokemon.Moveset => Moveset;
        IPBEPartyMoveset IPBEPartyPokemon.Moveset => Moveset;

        public TestPartyPokemon(PBESettings settings, PBESpecies species, PBEForm form, byte level, params PBEMove[] moves)
        {
            Species = species;
            Form = form;
            Level = level;
            EXP = PBEDataProvider.Instance.GetEXPRequired(PBEDataProvider.Instance.GetPokemonData(species, form).GrowthRate, level);
            Nickname = species.ToString();
            Gender = PBEDataProvider.GlobalRandom.RandomGender(PBEDataProvider.Instance.GetPokemonData(species, form).GenderRatio);
            CaughtBall = PBEItem.PokeBall;
            EffortValues = new PBEStatCollection(0, 0, 0, 0, 0, 0);
            IndividualValues = new PBEStatCollection(0, 0, 0, 0, 0, 0);
            Moveset = new TestPartyMoveset(settings, moves);
            HP = PBEDataUtils.CalculateStat(species, form, PBEStat.HP, Nature, EffortValues.GetStat(PBEStat.HP), IndividualValues.GetStat(PBEStat.HP), level, settings);
        }
    }
    internal sealed class TestPokemonCollection : IPBEPokemonCollection, IPBEPokemonCollection<TestPokemon>
    {
        private readonly TestPokemon[] _list;
        public int Count => _list.Length;
        public TestPokemon this[int index]
        {
            get => _list[index];
            set => _list[index] = value;
        }
        IPBEPokemon IReadOnlyList<IPBEPokemon>.this[int index] => this[index];

        public TestPokemonCollection(int count)
        {
            _list = new TestPokemon[count];
        }

        public IEnumerator<TestPokemon> GetEnumerator()
        {
            return ((IEnumerable<TestPokemon>)_list).GetEnumerator();
        }
        IEnumerator<IPBEPokemon> IEnumerable<IPBEPokemon>.GetEnumerator()
        {
            return ((IEnumerable<TestPokemon>)_list).GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return _list.GetEnumerator();
        }
    }
    internal sealed class TestPartyPokemonCollection : IPBEPokemonCollection, IPBEPartyPokemonCollection, IPBEPokemonCollection<TestPartyPokemon>, IPBEPartyPokemonCollection<TestPartyPokemon>
    {
        private readonly TestPartyPokemon[] _list;
        public int Count => _list.Length;

        public TestPartyPokemon this[int index]
        {
            get => _list[index];
            set => _list[index] = value;
        }
        IPBEPartyPokemon IReadOnlyList<IPBEPartyPokemon>.this[int index] => this[index];
        IPBEPokemon IReadOnlyList<IPBEPokemon>.this[int index] => this[index];

        public TestPartyPokemonCollection(int count)
        {
            _list = new TestPartyPokemon[count];
        }

        public IEnumerator<TestPartyPokemon> GetEnumerator()
        {
            return ((IEnumerable<TestPartyPokemon>)_list).GetEnumerator();
        }
        IEnumerator<IPBEPartyPokemon> IEnumerable<IPBEPartyPokemon>.GetEnumerator()
        {
            return ((IEnumerable<TestPartyPokemon>)_list).GetEnumerator();
        }
        IEnumerator<IPBEPokemon> IEnumerable<IPBEPokemon>.GetEnumerator()
        {
            return ((IEnumerable<TestPartyPokemon>)_list).GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return _list.GetEnumerator();
        }
    }

    internal static class TestExtensions
    {
        public static bool VerifyAbilityHappened(this PBEBattle battle, PBEBattlePokemon abilityOwner, PBEBattlePokemon pokemon2, PBEAbility ability, PBEAbilityAction abilityAction)
        {
            foreach (IPBEPacket packet in battle.Events)
            {
                if (packet is PBEAbilityPacket ap
                    && ap.Ability == ability
                    && ap.AbilityAction == abilityAction
                    && ap.AbilityOwnerTrainer.TryGetPokemon(ap.AbilityOwner) == abilityOwner
                    && ap.Pokemon2Trainer.TryGetPokemon(ap.Pokemon2) == pokemon2)
                {
                    return true;
                }
            }
            return false;
        }
        public static bool VerifyItemHappened(this PBEBattle battle, PBEBattlePokemon itemHolder, PBEBattlePokemon pokemon2, PBEItem item, PBEItemAction itemAction)
        {
            foreach (IPBEPacket packet in battle.Events)
            {
                if (packet is PBEItemPacket ip
                    && ip.Item == item
                    && ip.ItemAction == itemAction
                    && ip.ItemHolderTrainer.TryGetPokemon(ip.ItemHolder) == itemHolder
                    && ip.Pokemon2Trainer.TryGetPokemon(ip.Pokemon2) == pokemon2)
                {
                    return true;
                }
            }
            return false;
        }
        public static bool VerifyMoveResultHappened(this PBEBattle battle, PBEBattlePokemon moveUser, PBEBattlePokemon pokemon2, PBEResult result)
        {
            foreach (IPBEPacket packet in battle.Events)
            {
                if (packet is PBEMoveResultPacket mrp
                    && mrp.Result == result
                    && mrp.MoveUserTrainer.TryGetPokemon(mrp.MoveUser) == moveUser
                    && mrp.Pokemon2Trainer.TryGetPokemon(mrp.Pokemon2) == pokemon2)
                {
                    return true;
                }
            }
            return false;
        }
        public static bool VerifySpecialMessageHappened(this PBEBattle battle, PBESpecialMessage message, params object[] p)
        {
            foreach (IPBEPacket packet in battle.Events)
            {
                if (packet is PBESpecialMessagePacket smp
                    && smp.Message == message
                    && p.Length == smp.Params.Count)
                {
                    for (int i = 0; i < p.Length; i++)
                    {
                        if (!p[i].Equals(smp.Params[i]))
                        {
                            goto nope;
                        }
                    }
                    return true;
                }
            nope:
                ;
            }
            return false;
        }
        public static bool VerifyStatus1Happened(this PBEBattle battle, PBEBattlePokemon status1Receiver, PBEBattlePokemon pokemon2, PBEStatus1 status1, PBEStatusAction statusAction)
        {
            foreach (IPBEPacket packet in battle.Events)
            {
                if (packet is PBEStatus1Packet s1p
                    && s1p.Status1 == status1
                    && s1p.StatusAction == statusAction
                    && s1p.Status1ReceiverTrainer.TryGetPokemon(s1p.Status1Receiver) == status1Receiver
                    && s1p.Pokemon2Trainer.TryGetPokemon(s1p.Pokemon2) == pokemon2)
                {
                    return true;
                }
            }
            return false;
        }
        public static bool VerifyStatus2Happened(this PBEBattle battle, PBEBattlePokemon status2Receiver, PBEBattlePokemon pokemon2, PBEStatus2 status2, PBEStatusAction statusAction)
        {
            foreach (IPBEPacket packet in battle.Events)
            {
                if (packet is PBEStatus2Packet s2p
                    && s2p.Status2 == status2
                    && s2p.StatusAction == statusAction
                    && s2p.Status2ReceiverTrainer.TryGetPokemon(s2p.Status2Receiver) == status2Receiver
                    && s2p.Pokemon2Trainer.TryGetPokemon(s2p.Pokemon2) == pokemon2)
                {
                    return true;
                }
            }
            return false;
        }
        public static bool VerifyTeamStatusHappened(this PBEBattle battle, PBETeam team, PBETeamStatus teamStatus, PBETeamStatusAction teamStatusAction, PBEBattlePokemon damageVictim = null)
        {
            foreach (IPBEPacket packet in battle.Events)
            {
                if (packet is PBETeamStatusPacket tsp
                    && tsp.Team == team
                    && tsp.TeamStatus == teamStatus
                    && tsp.TeamStatusAction == teamStatusAction
                    && tsp.DamageVictimTrainer?.TryGetPokemon(tsp.DamageVictim) == damageVictim)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
