using Avalonia.Media.Imaging;
using Kermalis.PokemonBattleEngine.Battle;
using Kermalis.PokemonBattleEngine.Utils;
using Kermalis.PokemonBattleEngineClient.Clients;
using Kermalis.PokemonBattleEngineClient.Infrastructure;

namespace Kermalis.PokemonBattleEngineClient.Models
{
    public sealed class PokemonInfo
    {
        public Bitmap MiniSprite { get; }
        public string Name { get; }

        internal PokemonInfo(PBEBattlePokemon pkmn, bool useKnownInfo)
        {
            if (pkmn != null)
            {
                MiniSprite = (Bitmap)SpeciesToMinispriteConverter.Instance.Convert(pkmn, typeof(Bitmap), useKnownInfo, PBEUtils.PBECulture);
                Name = useKnownInfo ? pkmn.KnownNickname + pkmn.KnownGender.ToSymbol() : pkmn.Nickname + pkmn.Gender.ToSymbol();
            }
        }
        internal PokemonInfo(BattleClient client, PBEBattlePokemon pkmn)
            : this(pkmn, pkmn != null && client.ShouldUseKnownInfo(pkmn.Trainer)) { }
    }
}
