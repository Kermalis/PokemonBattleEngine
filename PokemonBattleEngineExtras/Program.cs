using Kermalis.PokemonBattleEngine.DefaultData;
using Microsoft.Data.Sqlite;
using System;

namespace Kermalis.PokemonBattleEngineExtras;

internal sealed class Program
{
	private enum Extra
	{
		AIBattle,
		LocalizationDumper,
		NARCTextDumper,
		PokemonDataDumper
	}

	public static void Main()
	{
		static SqliteConnection GetConnection()
		{
			SQLitePCL.Batteries_V2.Init();
			const string databasePath = @"PokemonBattleEngine.db";
			var con = new SqliteConnection($"Filename={databasePath};");
			con.Open();
			return con;
		}

		Extra e = Extra.AIBattle;
		switch (e)
		{
			case Extra.AIBattle:
			{
				PBEDefaultDataProvider.InitEngine(string.Empty);
				_ = new AIBattleDemo();
				break;
			}
			case Extra.LocalizationDumper:
			{
				using (SqliteConnection con = GetConnection())
				{
					LocalizationDumper.Run(con);
					con.Close();
				}
				break;
			}
			case Extra.NARCTextDumper: NARCTextDumper.Dump(); break;
			case Extra.PokemonDataDumper:
			{
				using (SqliteConnection con = GetConnection())
				{
					PokemonDataDumper.Run(con);
					con.Close();
				}
				break;
			}
			default: throw new Exception(nameof(e));
		}
	}
}
