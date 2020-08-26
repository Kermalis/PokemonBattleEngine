namespace Kermalis.PokemonBattleEngine.Data
{
    public static partial class PBEDataUtils
    {
        #region Static Collections
        public static PBEAlphabeticalList<PBEStat> MoodyStats { get; } = new PBEAlphabeticalList<PBEStat>(new[] { PBEStat.Attack, PBEStat.Defense, PBEStat.SpAttack, PBEStat.SpDefense, PBEStat.Speed, PBEStat.Accuracy, PBEStat.Evasion });
        public static PBEAlphabeticalList<PBEStat> StarfBerryStats { get; } = new PBEAlphabeticalList<PBEStat>(new[] { PBEStat.Attack, PBEStat.Defense, PBEStat.SpAttack, PBEStat.SpDefense, PBEStat.Speed });
        #endregion
    }
}
