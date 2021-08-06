using Avalonia.Media.Imaging;
using Kermalis.PokemonBattleEngine.Battle;
using Kermalis.PokemonBattleEngine.Data;
using Kermalis.PokemonBattleEngineClient.Clients;
using Kermalis.PokemonBattleEngineClient.Infrastructure;

namespace Kermalis.PokemonBattleEngineClient.Models
{
    public sealed class PokemonInfo
    {
        public Bitmap? MiniSprite { get; }
        public string? Name { get; }

        private PokemonInfo() { }
        internal PokemonInfo(PBEBattlePokemon pkmn, bool useKnownInfo)
        {
            MiniSprite = (Bitmap)SpeciesToMinispriteConverter.Instance.Convert(pkmn, typeof(Bitmap), useKnownInfo, null)!;
            Name = useKnownInfo ? pkmn.KnownNickname : pkmn.Nickname + (useKnownInfo && !pkmn.KnownStatus2.HasFlag(PBEStatus2.Transformed) ? pkmn.KnownGender : pkmn.Gender).ToSymbol();
        }

        internal static PokemonInfo From(BattleClient client, PBETeam team, PBEFieldPosition pos)
        {
            if (!team.TryGetPokemon(pos, out PBEBattlePokemon? pkmn))
            {
                return new PokemonInfo(); // Return null?
            }
            return new PokemonInfo(pkmn, client.ShouldUseKnownInfo(pkmn.Trainer));
        }
    }
}
