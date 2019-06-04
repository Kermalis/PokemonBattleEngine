using Kermalis.PokemonBattleEngine.Battle;
using Kermalis.PokemonBattleEngine.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using Xamarin.Forms;

namespace Kermalis.PokemonBattleEngineMobile.Infrastructure
{
    internal static class Utils
    {
        private static readonly Assembly assembly = Assembly.GetExecutingAssembly();
        public static bool DoesResourceExist(string resource)
        {
            string[] resources = assembly.GetManifestResourceNames();
            return resources.Contains(resource);
        }
        // https://forums.xamarin.com/discussion/80050/how-to-maintain-aspect-ratio-of-custom-rendered-view
        public static VisualElement GetFirstParentVisualElement(Element element)
        {
            Element parent = element.Parent;
            while (parent != null)
            {
                if (parent is VisualElement parentView)
                {
                    return parentView;
                }
                else
                {
                    parent = parent.Parent;
                }
            }
            return null;
        }
        public static Stream GetResourceStream(string resource)
        {
            return assembly.GetManifestResourceStream(resource);
        }

        public static string GetPokemonSpriteResource(PBEPokemon pokemon, bool backSprite)
        {
            return GetPokemonSpriteResource(pokemon.KnownSpecies, pokemon.KnownShiny, pokemon.KnownGender, pokemon.Status2.HasFlag(PBEStatus2.Substitute), backSprite);
        }
        public static string GetPokemonSpriteResource(PBESpecies species, bool shiny, PBEGender gender, bool behindSubstitute, bool backSprite)
        {
            string resource;
            string orientation = backSprite ? "_B" : "_F";
            if (behindSubstitute)
            {
                resource = $"Kermalis.PokemonBattleEngineMobile.PKMN.STATUS2_Substitute{orientation}.gif";
            }
            else
            {
                uint speciesID = (uint)species & 0xFFFF;
                uint formeID = (uint)species >> 0x10;
                string sss = $"{speciesID}{(formeID > 0 ? $"_{formeID}" : string.Empty)}{orientation}{(shiny ? "_S" : string.Empty)}";
                string genderStr = gender == PBEGender.Female && DoesResourceExist($"Kermalis.PokemonBattleEngineMobile.PKMN.PKMN_{sss}_F.gif") ? "_F" : string.Empty;
                resource = $"Kermalis.PokemonBattleEngineMobile.PKMN.PKMN_{sss}{genderStr}.gif";
            }
            return resource;
        }
    }
}
