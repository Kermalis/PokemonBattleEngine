namespace Kermalis.PokemonBattleEngine.Data
{
    public sealed class PBEReadOnlyLocalizedString : IPBEReadOnlyLocalizedString
    {
        public string English { get; }
        public string French { get; }
        public string German { get; }
        public string Italian { get; }
        public string Japanese_Kana { get; }
        public string Japanese_Kanji { get; }
        public string Korean { get; }
        public string Spanish { get; }

        public PBEReadOnlyLocalizedString(IPBEReadOnlyLocalizedString other)
        {
            English = other.English;
            French = other.French;
            German = other.German;
            Italian = other.Italian;
            Japanese_Kana = other.Japanese_Kana;
            Japanese_Kanji = other.Japanese_Kanji;
            Korean = other.Korean;
            Spanish = other.Spanish;
        }

        public override string ToString()
        {
            return this.FromGlobalLanguage();
        }
    }
}
