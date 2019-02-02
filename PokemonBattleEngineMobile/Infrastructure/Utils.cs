using Kermalis.PokemonBattleEngine.Battle;
using Kermalis.PokemonBattleEngine.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using Xamarin.Forms;

namespace Kermalis.PokemonBattleEngineMobile.Infrastructure
{
    static class Utils
    {
        public static bool DoesResourceExist(string resource)
        {
            string[] resources = Assembly.GetExecutingAssembly().GetManifestResourceNames();
            return resources.Contains(resource);
        }
        public static void GetGifResourceWidthAndHeight(string resource, out short width, out short height)
        {
            using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resource))
            using (BinaryReader reader = new BinaryReader(stream))
            {
                reader.ReadBytes(6); // Skip header
                width = reader.ReadInt16();
                height = reader.ReadInt16();
            }
        }
        // https://forums.xamarin.com/discussion/80050/how-to-maintain-aspect-ratio-of-custom-rendered-view
        public static VisualElement GetFirstParentVisualElement(Element element)
        {
            Element parent = element.Parent;
            while (parent != null)
            {
                var parentView = parent as VisualElement;
                if (parentView != null)
                {
                    return parentView;
                }
                parent = parent.Parent;
            }
            return null;
        }

        public static ImageSource GetPokemonSprite(PBEPokemon pokemon, bool backSprite, out short width, out short height)
        {
            return GetPokemonSprite(pokemon.VisualSpecies, pokemon.VisualShiny, pokemon.VisualGender, pokemon.Status2.HasFlag(PBEStatus2.Substitute), backSprite, out width, out height);
        }
        public static ImageSource GetPokemonSprite(PBESpecies species, bool shiny, PBEGender gender, bool behindSubstitute, bool backSprite, out short width, out short height)
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
                bool spriteIsGenderNeutral = DoesResourceExist($"Kermalis.PokemonBattleEngineMobile.PKMN.PKMN_{sss}.gif");
                string genderStr = spriteIsGenderNeutral ? string.Empty : gender == PBEGender.Female ? "_F" : "_M";
                resource = $"Kermalis.PokemonBattleEngineMobile.PKMN.PKMN_{sss}{genderStr}.gif";
            }
            GetGifResourceWidthAndHeight(resource, out width, out height);
            return ImageSource.FromResource(resource);
        }
    }
}
