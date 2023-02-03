namespace Kermalis.PokemonBattleEngine.Data.Utils;

public static partial class PBEDataUtils
{
	#region Static Collections

	public static PBEAlphabeticalList<PBEStat> MoodyStats { get; } = new(new[] { PBEStat.Attack, PBEStat.Defense, PBEStat.SpAttack, PBEStat.SpDefense, PBEStat.Speed, PBEStat.Accuracy, PBEStat.Evasion });
	public static PBEAlphabeticalList<PBEStat> StarfBerryStats { get; } = new(new[] { PBEStat.Attack, PBEStat.Defense, PBEStat.SpAttack, PBEStat.SpDefense, PBEStat.Speed });

	#endregion
}
