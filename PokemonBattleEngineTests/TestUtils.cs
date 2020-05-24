using Kermalis.PokemonBattleEngine.Battle;
using Kermalis.PokemonBattleEngine.Data;
using Kermalis.PokemonBattleEngine.Packets;
using Kermalis.PokemonBattleEngine.Utils;
using System;
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
            PBEUtils.CreateDatabaseConnection(string.Empty);
        }

        public static bool VerifyMoveResult(PBEBattle battle, PBEPokemon moveUser, PBEPokemon pokemon2, PBEResult result)
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
}
