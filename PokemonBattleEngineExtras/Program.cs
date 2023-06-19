using Kermalis.PokemonBattleEngine.DefaultData;
using Microsoft.Data.Sqlite;
using SQLitePCL;
using System;
using System.Threading.Tasks;

namespace Kermalis.PokemonBattleEngineExtras;

internal static class Program
{
	private enum Extra : byte
	{
		AIBattle,
		LocalizationDumper,
		NARCTextDumper,
		PokemonDataDumper
	}

	private const string DB_PATH = @"PokemonBattleEngine.db";

	private static SqliteConnection CreateDBConnection()
	{
		Batteries_V2.Init();
		var con = new SqliteConnection($"Filename={DB_PATH};");
		con.Open();
		return con;
	}

	public static async Task Main()
	{
		Extra e = Extra.AIBattle;
		try
		{
			switch (e)
			{
				case Extra.AIBattle:
				{
					PBEDefaultDataProvider.InitEngine(string.Empty);
					await new AIBattleDemo().Run();
					break;
				}
				case Extra.LocalizationDumper:
				{
					using (SqliteConnection con = CreateDBConnection())
					{
						LocalizationDumper.Run(con);
						con.Close();
					}
					break;
				}
				case Extra.NARCTextDumper:
				{
					NARCTextDumper.Dump();
					break;
				}
				case Extra.PokemonDataDumper:
				{
					using (SqliteConnection con = CreateDBConnection())
					{
						PokemonDataDumper.Run(con);
						con.Close();
					}
					break;
				}
			}
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex);
			Console.ReadKey();
		}
	}
}
