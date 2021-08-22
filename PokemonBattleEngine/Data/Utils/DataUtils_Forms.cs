using Kermalis.PokemonBattleEngine.Utils;
using System;
using System.Collections.Generic;

namespace Kermalis.PokemonBattleEngine.Data.Utils
{
    public static partial class PBEDataUtils
    {
        #region Static Collections

        public static PBEAlphabeticalList<PBESpecies> AllSpecies { get; } = new(Enum.GetValues<PBESpecies>().ExceptOne(PBESpecies.MAX));
        public static PBEAlphabeticalList<PBESpecies> FullyEvolvedSpecies { get; } = new(AllSpecies.FindAll(s => !PBEDataProvider.Instance.HasEvolutions(s, 0)));

        #region Forms

        private static readonly PBEAlphabeticalList<PBEForm> _arceus = new(new[] { PBEForm.Arceus, PBEForm.Arceus_Bug, PBEForm.Arceus_Dark,
            PBEForm.Arceus_Dragon, PBEForm.Arceus_Electric, PBEForm.Arceus_Fighting, PBEForm.Arceus_Fire, PBEForm.Arceus_Flying, PBEForm.Arceus_Ghost, PBEForm.Arceus_Grass,
            PBEForm.Arceus_Ground, PBEForm.Arceus_Ice, PBEForm.Arceus_Poison, PBEForm.Arceus_Psychic, PBEForm.Arceus_Rock, PBEForm.Arceus_Steel, PBEForm.Arceus_Water }, PBESpecies.Arceus);
        private static readonly PBEAlphabeticalList<PBEForm> _basculin = new(new[] { PBEForm.Basculin_Blue, PBEForm.Basculin_Red }, PBESpecies.Basculin);
        private static readonly PBEAlphabeticalList<PBEForm> _burmy = new(new[] { PBEForm.Burmy_Plant, PBEForm.Burmy_Sandy, PBEForm.Burmy_Trash }, PBESpecies.Burmy);
        private static readonly PBEAlphabeticalList<PBEForm> _castform = new(new[] { PBEForm.Castform, PBEForm.Castform_Rainy, PBEForm.Castform_Snowy,
            PBEForm.Castform_Sunny }, PBESpecies.Castform);
        private static readonly PBEAlphabeticalList<PBEForm> _cherrim = new(new[] { PBEForm.Cherrim, PBEForm.Cherrim_Sunshine }, PBESpecies.Cherrim);
        private static readonly PBEAlphabeticalList<PBEForm> _darmanitan = new(new[] { PBEForm.Darmanitan, PBEForm.Darmanitan_Zen }, PBESpecies.Darmanitan);
        private static readonly PBEAlphabeticalList<PBEForm> _deerling = new(new[] { PBEForm.Deerling_Autumn, PBEForm.Deerling_Spring, PBEForm.Deerling_Summer,
            PBEForm.Deerling_Winter }, PBESpecies.Deerling);
        private static readonly PBEAlphabeticalList<PBEForm> _deoxys = new(new[] { PBEForm.Deoxys, PBEForm.Deoxys_Attack, PBEForm.Deoxys_Defense,
            PBEForm.Deoxys_Speed }, PBESpecies.Deoxys);
        private static readonly PBEAlphabeticalList<PBEForm> _gastrodon = new(new[] { PBEForm.Gastrodon_East, PBEForm.Gastrodon_West }, PBESpecies.Gastrodon);
        private static readonly PBEAlphabeticalList<PBEForm> _genesect = new(new[] { PBEForm.Genesect, PBEForm.Genesect_Burn, PBEForm.Genesect_Chill,
            PBEForm.Genesect_Douse, PBEForm.Genesect_Shock }, PBESpecies.Genesect);
        private static readonly PBEAlphabeticalList<PBEForm> _giratina = new(new[] { PBEForm.Giratina, PBEForm.Giratina_Origin }, PBESpecies.Giratina);
        private static readonly PBEAlphabeticalList<PBEForm> _keldeo = new(new[] { PBEForm.Keldeo, PBEForm.Keldeo_Resolute }, PBESpecies.Keldeo);
        private static readonly PBEAlphabeticalList<PBEForm> _kyurem = new(new[] { PBEForm.Kyurem, PBEForm.Kyurem_Black, PBEForm.Kyurem_White }, PBESpecies.Kyurem);
        private static readonly PBEAlphabeticalList<PBEForm> _landorus = new(new[] { PBEForm.Landorus, PBEForm.Landorus_Therian }, PBESpecies.Landorus);
        private static readonly PBEAlphabeticalList<PBEForm> _meloetta = new(new[] { PBEForm.Meloetta, PBEForm.Meloetta_Pirouette }, PBESpecies.Meloetta);
        private static readonly PBEAlphabeticalList<PBEForm> _rotom = new(new[] { PBEForm.Rotom, PBEForm.Rotom_Fan, PBEForm.Rotom_Frost, PBEForm.Rotom_Heat,
            PBEForm.Rotom_Mow, PBEForm.Rotom_Wash }, PBESpecies.Rotom);
        private static readonly PBEAlphabeticalList<PBEForm> _sawsbuck = new(new[] { PBEForm.Sawsbuck_Autumn, PBEForm.Sawsbuck_Spring, PBEForm.Sawsbuck_Summer,
            PBEForm.Sawsbuck_Winter }, PBESpecies.Sawsbuck);
        private static readonly PBEAlphabeticalList<PBEForm> _shaymin = new(new[] { PBEForm.Shaymin, PBEForm.Shaymin_Sky }, PBESpecies.Shaymin);
        private static readonly PBEAlphabeticalList<PBEForm> _shellos = new(new[] { PBEForm.Shellos_East, PBEForm.Shellos_West }, PBESpecies.Shellos);
        private static readonly PBEAlphabeticalList<PBEForm> _thundurus = new(new[] { PBEForm.Thundurus, PBEForm.Thundurus_Therian }, PBESpecies.Thundurus);
        private static readonly PBEAlphabeticalList<PBEForm> _tornadus = new(new[] { PBEForm.Tornadus, PBEForm.Tornadus_Therian }, PBESpecies.Tornadus);
        private static readonly PBEAlphabeticalList<PBEForm> _unown = new(new[] { PBEForm.Unown_A, PBEForm.Unown_B, PBEForm.Unown_C, PBEForm.Unown_D,
            PBEForm.Unown_E, PBEForm.Unown_Exclamation, PBEForm.Unown_F, PBEForm.Unown_G, PBEForm.Unown_H, PBEForm.Unown_I, PBEForm.Unown_J, PBEForm.Unown_K, PBEForm.Unown_L, PBEForm.Unown_M,
            PBEForm.Unown_N, PBEForm.Unown_O, PBEForm.Unown_P, PBEForm.Unown_Q, PBEForm.Unown_Question, PBEForm.Unown_R, PBEForm.Unown_S, PBEForm.Unown_T, PBEForm.Unown_U, PBEForm.Unown_V,
            PBEForm.Unown_W, PBEForm.Unown_X, PBEForm.Unown_Y, PBEForm.Unown_Z }, PBESpecies.Unown);
        private static readonly PBEAlphabeticalList<PBEForm> _wormadam = new(new[] { PBEForm.Wormadam_Plant, PBEForm.Wormadam_Sandy, PBEForm.Wormadam_Trash },
            PBESpecies.Wormadam);

        #endregion

        #endregion

        public static bool CanChangeForm(PBESpecies species, bool requireUsableOutsideOfBattle)
        {
            if (species <= 0 || species >= PBESpecies.MAX)
            {
                throw new ArgumentOutOfRangeException(nameof(species));
            }
            switch (species)
            {
                case PBESpecies.Arceus:
                case PBESpecies.Burmy:
                case PBESpecies.Deerling:
                case PBESpecies.Deoxys:
                case PBESpecies.Genesect:
                case PBESpecies.Giratina:
                case PBESpecies.Keldeo:
                case PBESpecies.Kyurem:
                case PBESpecies.Landorus:
                case PBESpecies.Rotom:
                case PBESpecies.Sawsbuck:
                case PBESpecies.Shaymin:
                case PBESpecies.Thundurus:
                case PBESpecies.Tornadus: return true;
                case PBESpecies.Castform:
                case PBESpecies.Cherrim:
                case PBESpecies.Darmanitan:
                case PBESpecies.Meloetta: return !requireUsableOutsideOfBattle;
                default: return false;
            }
        }
        public static bool HasForms(PBESpecies species, bool requireUsableOutsideOfBattle)
        {
            if (species <= 0 || species >= PBESpecies.MAX)
            {
                throw new ArgumentOutOfRangeException(nameof(species));
            }
            switch (species)
            {
                case PBESpecies.Arceus:
                case PBESpecies.Basculin:
                case PBESpecies.Burmy:
                case PBESpecies.Deerling:
                case PBESpecies.Deoxys:
                case PBESpecies.Gastrodon:
                case PBESpecies.Genesect:
                case PBESpecies.Giratina:
                case PBESpecies.Keldeo:
                case PBESpecies.Kyurem:
                case PBESpecies.Landorus:
                case PBESpecies.Rotom:
                case PBESpecies.Sawsbuck:
                case PBESpecies.Shaymin:
                case PBESpecies.Shellos:
                case PBESpecies.Thundurus:
                case PBESpecies.Tornadus:
                case PBESpecies.Unown:
                case PBESpecies.Wormadam: return true;
                case PBESpecies.Castform:
                case PBESpecies.Cherrim:
                case PBESpecies.Darmanitan:
                case PBESpecies.Meloetta: return !requireUsableOutsideOfBattle;
                default: return false;
            }
        }

        public static string? GetNameOfForm(PBESpecies species, PBEForm form)
        {
            ValidateSpecies(species, form, false);
            string[] names = Enum.GetNames<PBEForm>();
            PBEForm[] forms = Enum.GetValues<PBEForm>();
            Dictionary<PBEForm, string> combo = new();
            for (int i = 0; i < names.Length; i++)
            {
                PBEForm f = forms[i];
                string name = names[i];
                if (name.StartsWith(species.ToString()))
                {
                    combo.Add(f, name);
                }
            }
            if (combo.Count == 0)
            {
                return null;
            }
            return combo[form];
        }

        public static IReadOnlyList<PBEForm> GetForms(PBESpecies species, bool requireUsableOutsideOfBattle)
        {
            if (species <= 0 || species >= PBESpecies.MAX)
            {
                throw new ArgumentOutOfRangeException(nameof(species));
            }
            switch (species)
            {
                case PBESpecies.Arceus: return _arceus;
                case PBESpecies.Basculin: return _basculin;
                case PBESpecies.Burmy: return _burmy;
                case PBESpecies.Castform: return requireUsableOutsideOfBattle ? Array.Empty<PBEForm>() : _castform;
                case PBESpecies.Cherrim: return requireUsableOutsideOfBattle ? Array.Empty<PBEForm>() : _cherrim;
                case PBESpecies.Darmanitan: return requireUsableOutsideOfBattle ? Array.Empty<PBEForm>() : _darmanitan;
                case PBESpecies.Deerling: return _deerling;
                case PBESpecies.Deoxys: return _deoxys;
                case PBESpecies.Gastrodon: return _gastrodon;
                case PBESpecies.Genesect: return _genesect;
                case PBESpecies.Giratina: return _giratina;
                case PBESpecies.Keldeo: return _keldeo;
                case PBESpecies.Kyurem: return _kyurem;
                case PBESpecies.Landorus: return _landorus;
                case PBESpecies.Meloetta: return requireUsableOutsideOfBattle ? Array.Empty<PBEForm>() : _meloetta;
                case PBESpecies.Rotom: return _rotom;
                case PBESpecies.Sawsbuck: return _sawsbuck;
                case PBESpecies.Shaymin: return _shaymin;
                case PBESpecies.Shellos: return _shellos;
                case PBESpecies.Thundurus: return _thundurus;
                case PBESpecies.Tornadus: return _tornadus;
                case PBESpecies.Unown: return _unown;
                case PBESpecies.Wormadam: return _wormadam;
                default: return Array.Empty<PBEForm>();
            }
        }

        public static IReadOnlyList<PBEItem> GetValidItems(PBESpecies species, PBEForm form)
        {
            ValidateSpecies(species, form, false);
            switch (species)
            {
                case PBESpecies.Arceus:
                {
                    switch (form)
                    {
                        case PBEForm.Arceus: return _arceusItems;
                        case PBEForm.Arceus_Bug: return _arceusBugItems;
                        case PBEForm.Arceus_Dark: return _arceusDarkItems;
                        case PBEForm.Arceus_Dragon: return _arceusDragonItems;
                        case PBEForm.Arceus_Electric: return _arceusElectricItems;
                        case PBEForm.Arceus_Fighting: return _arceusFightingItems;
                        case PBEForm.Arceus_Fire: return _arceusFireItems;
                        case PBEForm.Arceus_Flying: return _arceusFlyingItems;
                        case PBEForm.Arceus_Ghost: return _arceusGhostItems;
                        case PBEForm.Arceus_Grass: return _arceusGrassItems;
                        case PBEForm.Arceus_Ground: return _arceusGroundItems;
                        case PBEForm.Arceus_Ice: return _arceusIceItems;
                        case PBEForm.Arceus_Poison: return _arceusPoisonItems;
                        case PBEForm.Arceus_Psychic: return _arceusPsychicItems;
                        case PBEForm.Arceus_Rock: return _arceusRockItems;
                        case PBEForm.Arceus_Steel: return _arceusSteelItems;
                        case PBEForm.Arceus_Water: return _arceusWaterItems;
                        default: throw new ArgumentOutOfRangeException(nameof(form));
                    }
                }
                case PBESpecies.Genesect:
                {
                    switch (form)
                    {
                        case PBEForm.Genesect: return _genesectItems;
                        case PBEForm.Genesect_Burn: return _genesectBurnItems;
                        case PBEForm.Genesect_Chill: return _genesectChillItems;
                        case PBEForm.Genesect_Douse: return _genesectDouseItems;
                        case PBEForm.Genesect_Shock: return _genesectShockItems;
                        default: throw new ArgumentOutOfRangeException(nameof(form));
                    }
                }
                case PBESpecies.Giratina:
                {
                    switch (form)
                    {
                        case PBEForm.Giratina: return _giratinaItems;
                        case PBEForm.Giratina_Origin: return _giratinaOriginItems;
                        default: throw new ArgumentOutOfRangeException(nameof(form));
                    }
                }
                default: return AllItems;
            }
        }

        public static bool IsValidForm(PBESpecies species, PBEForm form, bool requireUsableOutsideOfBattle)
        {
            if (species <= 0 || species >= PBESpecies.MAX)
            {
                throw new ArgumentOutOfRangeException(nameof(species));
            }
            switch (species)
            {
                case PBESpecies.Arceus: return form <= PBEForm.Arceus_Dark;
                case PBESpecies.Basculin: return form <= PBEForm.Basculin_Blue;
                case PBESpecies.Burmy: return form <= PBEForm.Burmy_Trash;
                case PBESpecies.Castform: return form <= (requireUsableOutsideOfBattle ? 0 : PBEForm.Castform_Snowy);
                case PBESpecies.Cherrim: return form <= (requireUsableOutsideOfBattle ? 0 : PBEForm.Cherrim_Sunshine);
                case PBESpecies.Darmanitan: return form <= (requireUsableOutsideOfBattle ? 0 : PBEForm.Darmanitan_Zen);
                case PBESpecies.Deerling: return form <= PBEForm.Deerling_Winter;
                case PBESpecies.Deoxys: return form <= PBEForm.Deoxys_Speed;
                case PBESpecies.Gastrodon: return form <= PBEForm.Gastrodon_East;
                case PBESpecies.Genesect: return form <= PBEForm.Genesect_Chill;
                case PBESpecies.Giratina: return form <= PBEForm.Giratina_Origin;
                case PBESpecies.Keldeo: return form <= PBEForm.Keldeo_Resolute;
                case PBESpecies.Kyurem: return form <= PBEForm.Kyurem_Black;
                case PBESpecies.Landorus: return form <= PBEForm.Landorus_Therian;
                case PBESpecies.Meloetta: return form <= (requireUsableOutsideOfBattle ? 0 : PBEForm.Meloetta_Pirouette);
                case PBESpecies.Rotom: return form <= PBEForm.Rotom_Mow;
                case PBESpecies.Sawsbuck: return form <= PBEForm.Sawsbuck_Winter;
                case PBESpecies.Shaymin: return form <= PBEForm.Shaymin_Sky;
                case PBESpecies.Shellos: return form <= PBEForm.Shellos_East;
                case PBESpecies.Thundurus: return form <= PBEForm.Thundurus_Therian;
                case PBESpecies.Tornadus: return form <= PBEForm.Tornadus_Therian;
                case PBESpecies.Unown: return form <= PBEForm.Unown_Question;
                case PBESpecies.Wormadam: return form <= PBEForm.Wormadam_Trash;
                default: return form <= 0;
            }
        }
    }
}
