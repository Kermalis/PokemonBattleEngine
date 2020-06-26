using Kermalis.PokemonBattleEngine.Battle;
using Kermalis.PokemonBattleEngine.Data;
using Kermalis.PokemonBattleEngine.Packets;
using Kermalis.PokemonBattleEngine.Utils;
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
            PBEUtils.InitEngine(string.Empty);
        }

        public static bool VerifyMoveResult(PBEBattle battle, PBEBattlePokemon moveUser, PBEBattlePokemon pokemon2, PBEResult result)
        {
            foreach (IPBEPacket packet in battle.Events)
            {
                if (packet is PBEMoveResultPacket mrp)
                {
                    if (mrp.Result == result && mrp.MoveUserTeam.TryGetPokemon(mrp.MoveUser) == moveUser && mrp.Pokemon2Team.TryGetPokemon(mrp.Pokemon2) == pokemon2)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        public static bool VerifyStatus2Happened(PBEBattle battle, PBEBattlePokemon status2Receiver, PBEBattlePokemon pokemon2, PBEStatus2 status2, PBEStatusAction statusAction)
        {
            foreach (IPBEPacket packet in battle.Events)
            {
                if (packet is PBEStatus2Packet s2p)
                {
                    if (s2p.Status2 == status2 && s2p.StatusAction == statusAction && s2p.Status2ReceiverTeam.TryGetPokemon(s2p.Status2Receiver) == status2Receiver && s2p.Pokemon2Team.TryGetPokemon(s2p.Pokemon2) == pokemon2)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        public static bool VerifyTeamStatusHappened(PBEBattle battle, PBETeam team, PBETeamStatus teamStatus, PBETeamStatusAction teamStatusAction, PBEBattlePokemon damageVictim = null)
        {
            foreach (IPBEPacket packet in battle.Events)
            {
                if (packet is PBETeamStatusPacket tsp)
                {
                    if (tsp.Team == team && tsp.TeamStatus == teamStatus && tsp.TeamStatusAction == teamStatusAction && team.TryGetPokemon(tsp.DamageVictim) == damageVictim)
                    {
                        return true;
                    }
                }
            }
            return false;
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

    public class TestMoveset : IPBEMoveset, IPBEMoveset<TestMoveset.TestMovesetSlot>
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

        public TestMoveset(PBESettings settings, PBEMove[] moves)
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
            for (int i = 0; i < _list.Length; i++)
            {
                yield return _list[i];
            }
        }
        IEnumerator<IPBEMovesetSlot> IEnumerable<IPBEMovesetSlot>.GetEnumerator()
        {
            return GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
    public class TestPokemon : IPBEPokemon
    {
        public PBESpecies Species { get; set; }
        public PBEForm Form { get; set; }
        public PBEGender Gender { get; set; }
        public string Nickname { get; set; }
        public bool Shiny { get; set; }
        public byte Level { get; set; }
        public PBEItem Item { get; set; }
        public byte Friendship { get; set; }
        public PBEAbility Ability { get; set; }
        public PBENature Nature { get; set; }
        public IPBEStatCollection EffortValues { get; set; }
        public IPBEReadOnlyStatCollection IndividualValues { get; set; }
        public TestMoveset Moveset { get; set; }
        IPBEMoveset IPBEPokemon.Moveset => Moveset;

        public TestPokemon(PBESpecies species, PBEForm form, byte level)
        {
            Species = species;
            Form = form;
            Level = level;
            Nickname = species.ToString();
            Gender = PBERandom.RandomGender(PBEPokemonData.GetData(species, form).GenderRatio);
            EffortValues = new PBEStatCollection(0, 0, 0, 0, 0, 0);
            IndividualValues = new PBEStatCollection(0, 0, 0, 0, 0, 0);
        }
    }
    public class TestPokemonCollection : IPBEPokemonCollection, IPBEPokemonCollection<TestPokemon>
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

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _list.GetEnumerator();
        }
        IEnumerator<IPBEPokemon> IEnumerable<IPBEPokemon>.GetEnumerator()
        {
            return ((IEnumerable<TestPokemon>)_list).GetEnumerator();
        }
        public IEnumerator<TestPokemon> GetEnumerator()
        {
            return ((IEnumerable<TestPokemon>)_list).GetEnumerator();
        }
    }
}
