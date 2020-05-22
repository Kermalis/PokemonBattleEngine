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
    }
}
