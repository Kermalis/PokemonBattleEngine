using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Kermalis.PokemonBattleEngine.Data
{
    public static partial class PBEDataUtils
    {
        #region Static Collections
        public static PBEAlphabeticalList<PBEItem> AllItems { get; } = new PBEAlphabeticalList<PBEItem>(Enum.GetValues(typeof(PBEItem)).Cast<PBEItem>());
        public static PBEAlphabeticalList<PBESpecies> AllSpecies { get; } = new PBEAlphabeticalList<PBESpecies>(Enum.GetValues(typeof(PBESpecies)).Cast<PBESpecies>().Except(new[] { PBESpecies.MAX }));
        public static PBEAlphabeticalList<PBESpecies> FullyEvolvedSpecies { get; } = new PBEAlphabeticalList<PBESpecies>(AllSpecies.Where(s => PBEPokemonData.GetData(s, 0).Evolutions.Count == 0));
        public static ReadOnlyDictionary<PBEType, PBEItem> TypeToGem { get; } = new ReadOnlyDictionary<PBEType, PBEItem>(new Dictionary<PBEType, PBEItem>()
        {
            { PBEType.Bug, PBEItem.BugGem },
            { PBEType.Dark, PBEItem.DarkGem },
            { PBEType.Dragon, PBEItem.DragonGem },
            { PBEType.Electric, PBEItem.ElectricGem },
            { PBEType.Fighting, PBEItem.FightingGem },
            { PBEType.Fire, PBEItem.FireGem },
            { PBEType.Flying, PBEItem.FlyingGem },
            { PBEType.Ghost, PBEItem.GhostGem },
            { PBEType.Grass, PBEItem.GrassGem },
            { PBEType.Ground, PBEItem.GroundGem },
            { PBEType.Ice, PBEItem.IceGem },
            { PBEType.Normal, PBEItem.NormalGem },
            { PBEType.Poison, PBEItem.PoisonGem },
            { PBEType.Psychic, PBEItem.PsychicGem },
            { PBEType.Rock, PBEItem.RockGem },
            { PBEType.Steel, PBEItem.SteelGem },
            { PBEType.Water, PBEItem.WaterGem }
        });
        #region Forms
        private static readonly PBEAlphabeticalList<PBEForm> _arceus = new PBEAlphabeticalList<PBEForm>(new[] { PBEForm.Arceus, PBEForm.Arceus_Bug, PBEForm.Arceus_Dark,
            PBEForm.Arceus_Dragon, PBEForm.Arceus_Electric, PBEForm.Arceus_Fighting, PBEForm.Arceus_Fire, PBEForm.Arceus_Flying, PBEForm.Arceus_Ghost, PBEForm.Arceus_Grass,
            PBEForm.Arceus_Ground, PBEForm.Arceus_Ice, PBEForm.Arceus_Poison, PBEForm.Arceus_Psychic, PBEForm.Arceus_Rock, PBEForm.Arceus_Steel, PBEForm.Arceus_Water }, PBESpecies.Arceus);
        private static readonly PBEAlphabeticalList<PBEForm> _basculin = new PBEAlphabeticalList<PBEForm>(new[] { PBEForm.Basculin_Blue, PBEForm.Basculin_Red }, PBESpecies.Basculin);
        private static readonly PBEAlphabeticalList<PBEForm> _burmy = new PBEAlphabeticalList<PBEForm>(new[] { PBEForm.Burmy_Plant, PBEForm.Burmy_Sandy, PBEForm.Burmy_Trash }, PBESpecies.Burmy);
        private static readonly PBEAlphabeticalList<PBEForm> _castform = new PBEAlphabeticalList<PBEForm>(new[] { PBEForm.Castform, PBEForm.Castform_Rainy, PBEForm.Castform_Snowy,
            PBEForm.Castform_Sunny }, PBESpecies.Castform);
        private static readonly PBEAlphabeticalList<PBEForm> _cherrim = new PBEAlphabeticalList<PBEForm>(new[] { PBEForm.Cherrim, PBEForm.Cherrim_Sunshine }, PBESpecies.Cherrim);
        private static readonly PBEAlphabeticalList<PBEForm> _darmanitan = new PBEAlphabeticalList<PBEForm>(new[] { PBEForm.Darmanitan, PBEForm.Darmanitan_Zen }, PBESpecies.Darmanitan);
        private static readonly PBEAlphabeticalList<PBEForm> _deerling = new PBEAlphabeticalList<PBEForm>(new[] { PBEForm.Deerling_Autumn, PBEForm.Deerling_Spring, PBEForm.Deerling_Summer,
            PBEForm.Deerling_Winter }, PBESpecies.Deerling);
        private static readonly PBEAlphabeticalList<PBEForm> _deoxys = new PBEAlphabeticalList<PBEForm>(new[] { PBEForm.Deoxys, PBEForm.Deoxys_Attack, PBEForm.Deoxys_Defense,
            PBEForm.Deoxys_Speed }, PBESpecies.Deoxys);
        private static readonly PBEAlphabeticalList<PBEForm> _gastrodon = new PBEAlphabeticalList<PBEForm>(new[] { PBEForm.Gastrodon_East, PBEForm.Gastrodon_West }, PBESpecies.Gastrodon);
        private static readonly PBEAlphabeticalList<PBEForm> _genesect = new PBEAlphabeticalList<PBEForm>(new[] { PBEForm.Genesect, PBEForm.Genesect_Burn, PBEForm.Genesect_Chill,
            PBEForm.Genesect_Douse, PBEForm.Genesect_Shock }, PBESpecies.Genesect);
        private static readonly PBEAlphabeticalList<PBEForm> _giratina = new PBEAlphabeticalList<PBEForm>(new[] { PBEForm.Giratina, PBEForm.Giratina_Origin }, PBESpecies.Giratina);
        private static readonly PBEAlphabeticalList<PBEForm> _keldeo = new PBEAlphabeticalList<PBEForm>(new[] { PBEForm.Keldeo, PBEForm.Keldeo_Resolute }, PBESpecies.Keldeo);
        private static readonly PBEAlphabeticalList<PBEForm> _kyurem = new PBEAlphabeticalList<PBEForm>(new[] { PBEForm.Kyurem, PBEForm.Kyurem_Black, PBEForm.Kyurem_White }, PBESpecies.Kyurem);
        private static readonly PBEAlphabeticalList<PBEForm> _landorus = new PBEAlphabeticalList<PBEForm>(new[] { PBEForm.Landorus, PBEForm.Landorus_Therian }, PBESpecies.Landorus);
        private static readonly PBEAlphabeticalList<PBEForm> _meloetta = new PBEAlphabeticalList<PBEForm>(new[] { PBEForm.Meloetta, PBEForm.Meloetta_Pirouette }, PBESpecies.Meloetta);
        private static readonly PBEAlphabeticalList<PBEForm> _rotom = new PBEAlphabeticalList<PBEForm>(new[] { PBEForm.Rotom, PBEForm.Rotom_Fan, PBEForm.Rotom_Frost, PBEForm.Rotom_Heat,
            PBEForm.Rotom_Mow, PBEForm.Rotom_Wash }, PBESpecies.Rotom);
        private static readonly PBEAlphabeticalList<PBEForm> _sawsbuck = new PBEAlphabeticalList<PBEForm>(new[] { PBEForm.Sawsbuck_Autumn, PBEForm.Sawsbuck_Spring, PBEForm.Sawsbuck_Summer,
            PBEForm.Sawsbuck_Winter }, PBESpecies.Sawsbuck);
        private static readonly PBEAlphabeticalList<PBEForm> _shaymin = new PBEAlphabeticalList<PBEForm>(new[] { PBEForm.Shaymin, PBEForm.Shaymin_Sky }, PBESpecies.Shaymin);
        private static readonly PBEAlphabeticalList<PBEForm> _shellos = new PBEAlphabeticalList<PBEForm>(new[] { PBEForm.Shellos_East, PBEForm.Shellos_West }, PBESpecies.Shellos);
        private static readonly PBEAlphabeticalList<PBEForm> _thundurus = new PBEAlphabeticalList<PBEForm>(new[] { PBEForm.Thundurus, PBEForm.Thundurus_Therian }, PBESpecies.Thundurus);
        private static readonly PBEAlphabeticalList<PBEForm> _tornadus = new PBEAlphabeticalList<PBEForm>(new[] { PBEForm.Tornadus, PBEForm.Tornadus_Therian }, PBESpecies.Tornadus);
        private static readonly PBEAlphabeticalList<PBEForm> _unown = new PBEAlphabeticalList<PBEForm>(new[] { PBEForm.Unown_A, PBEForm.Unown_B, PBEForm.Unown_C, PBEForm.Unown_D,
            PBEForm.Unown_E, PBEForm.Unown_Exclamation, PBEForm.Unown_F, PBEForm.Unown_G, PBEForm.Unown_H, PBEForm.Unown_I, PBEForm.Unown_J, PBEForm.Unown_K, PBEForm.Unown_L, PBEForm.Unown_M,
            PBEForm.Unown_N, PBEForm.Unown_O, PBEForm.Unown_P, PBEForm.Unown_Q, PBEForm.Unown_Question, PBEForm.Unown_R, PBEForm.Unown_S, PBEForm.Unown_T, PBEForm.Unown_U, PBEForm.Unown_V,
            PBEForm.Unown_W, PBEForm.Unown_X, PBEForm.Unown_Y, PBEForm.Unown_Z }, PBESpecies.Unown);
        private static readonly PBEAlphabeticalList<PBEForm> _wormadam = new PBEAlphabeticalList<PBEForm>(new[] { PBEForm.Wormadam_Plant, PBEForm.Wormadam_Sandy, PBEForm.Wormadam_Trash },
            PBESpecies.Wormadam);
        #region Items
        private static readonly PBEAlphabeticalList<PBEItem> _arceusItems = new PBEAlphabeticalList<PBEItem>(AllItems.Except(new[] { PBEItem.DracoPlate, PBEItem.DreadPlate, PBEItem.EarthPlate,
            PBEItem.FistPlate, PBEItem.FlamePlate, PBEItem.IciclePlate, PBEItem.InsectPlate, PBEItem.IronPlate, PBEItem.MeadowPlate, PBEItem.MindPlate, PBEItem.SkyPlate, PBEItem.SplashPlate,
            PBEItem.SpookyPlate, PBEItem.StonePlate, PBEItem.ToxicPlate, PBEItem.ZapPlate }));
        private static readonly PBEAlphabeticalList<PBEItem> _arceusBugItems = new PBEAlphabeticalList<PBEItem>(new[] { PBEItem.InsectPlate });
        private static readonly PBEAlphabeticalList<PBEItem> _arceusDarkItems = new PBEAlphabeticalList<PBEItem>(new[] { PBEItem.DreadPlate });
        private static readonly PBEAlphabeticalList<PBEItem> _arceusDragonItems = new PBEAlphabeticalList<PBEItem>(new[] { PBEItem.DracoPlate });
        private static readonly PBEAlphabeticalList<PBEItem> _arceusElectricItems = new PBEAlphabeticalList<PBEItem>(new[] { PBEItem.ZapPlate });
        private static readonly PBEAlphabeticalList<PBEItem> _arceusFightingItems = new PBEAlphabeticalList<PBEItem>(new[] { PBEItem.FistPlate });
        private static readonly PBEAlphabeticalList<PBEItem> _arceusFireItems = new PBEAlphabeticalList<PBEItem>(new[] { PBEItem.FlamePlate });
        private static readonly PBEAlphabeticalList<PBEItem> _arceusFlyingItems = new PBEAlphabeticalList<PBEItem>(new[] { PBEItem.SkyPlate });
        private static readonly PBEAlphabeticalList<PBEItem> _arceusGhostItems = new PBEAlphabeticalList<PBEItem>(new[] { PBEItem.SpookyPlate });
        private static readonly PBEAlphabeticalList<PBEItem> _arceusGrassItems = new PBEAlphabeticalList<PBEItem>(new[] { PBEItem.MeadowPlate });
        private static readonly PBEAlphabeticalList<PBEItem> _arceusGroundItems = new PBEAlphabeticalList<PBEItem>(new[] { PBEItem.EarthPlate });
        private static readonly PBEAlphabeticalList<PBEItem> _arceusIceItems = new PBEAlphabeticalList<PBEItem>(new[] { PBEItem.IciclePlate });
        private static readonly PBEAlphabeticalList<PBEItem> _arceusPoisonItems = new PBEAlphabeticalList<PBEItem>(new[] { PBEItem.ToxicPlate });
        private static readonly PBEAlphabeticalList<PBEItem> _arceusPsychicItems = new PBEAlphabeticalList<PBEItem>(new[] { PBEItem.MindPlate });
        private static readonly PBEAlphabeticalList<PBEItem> _arceusRockItems = new PBEAlphabeticalList<PBEItem>(new[] { PBEItem.StonePlate });
        private static readonly PBEAlphabeticalList<PBEItem> _arceusSteelItems = new PBEAlphabeticalList<PBEItem>(new[] { PBEItem.IronPlate });
        private static readonly PBEAlphabeticalList<PBEItem> _arceusWaterItems = new PBEAlphabeticalList<PBEItem>(new[] { PBEItem.SplashPlate });
        private static readonly PBEAlphabeticalList<PBEItem> _genesectItems = new PBEAlphabeticalList<PBEItem>(AllItems.Except(new[] { PBEItem.BurnDrive, PBEItem.ChillDrive, PBEItem.DouseDrive,
            PBEItem.ShockDrive }));
        private static readonly PBEAlphabeticalList<PBEItem> _genesectBurnItems = new PBEAlphabeticalList<PBEItem>(new[] { PBEItem.BurnDrive });
        private static readonly PBEAlphabeticalList<PBEItem> _genesectChillItems = new PBEAlphabeticalList<PBEItem>(new[] { PBEItem.ChillDrive });
        private static readonly PBEAlphabeticalList<PBEItem> _genesectDouseItems = new PBEAlphabeticalList<PBEItem>(new[] { PBEItem.DouseDrive });
        private static readonly PBEAlphabeticalList<PBEItem> _genesectShockItems = new PBEAlphabeticalList<PBEItem>(new[] { PBEItem.ShockDrive });
        private static readonly PBEAlphabeticalList<PBEItem> _giratinaItems = new PBEAlphabeticalList<PBEItem>(AllItems.Except(new[] { PBEItem.GriseousOrb }));
        private static readonly PBEAlphabeticalList<PBEItem> _giratinaOriginItems = new PBEAlphabeticalList<PBEItem>(new[] { PBEItem.GriseousOrb });
        #endregion
        #endregion
        #endregion

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

        public static string GetNameOfForm(PBESpecies species, PBEForm form)
        {
            PBEPokemonShell.ValidateSpecies(species, form, false);
            switch (species)
            {
                case PBESpecies.Arceus:
                {
                    switch (form)
                    {
                        case PBEForm.Arceus: return nameof(PBEForm.Arceus);
                        case PBEForm.Arceus_Bug: return nameof(PBEForm.Arceus_Bug);
                        case PBEForm.Arceus_Dark: return nameof(PBEForm.Arceus_Dark);
                        case PBEForm.Arceus_Dragon: return nameof(PBEForm.Arceus_Dragon);
                        case PBEForm.Arceus_Electric: return nameof(PBEForm.Arceus_Electric);
                        case PBEForm.Arceus_Fighting: return nameof(PBEForm.Arceus_Fighting);
                        case PBEForm.Arceus_Fire: return nameof(PBEForm.Arceus_Fire);
                        case PBEForm.Arceus_Flying: return nameof(PBEForm.Arceus_Flying);
                        case PBEForm.Arceus_Ghost: return nameof(PBEForm.Arceus_Ghost);
                        case PBEForm.Arceus_Grass: return nameof(PBEForm.Arceus_Grass);
                        case PBEForm.Arceus_Ground: return nameof(PBEForm.Arceus_Ground);
                        case PBEForm.Arceus_Ice: return nameof(PBEForm.Arceus_Ice);
                        case PBEForm.Arceus_Poison: return nameof(PBEForm.Arceus_Poison);
                        case PBEForm.Arceus_Psychic: return nameof(PBEForm.Arceus_Psychic);
                        case PBEForm.Arceus_Rock: return nameof(PBEForm.Arceus_Rock);
                        case PBEForm.Arceus_Steel: return nameof(PBEForm.Arceus_Steel);
                        case PBEForm.Arceus_Water: return nameof(PBEForm.Arceus_Water);
                        default: throw new ArgumentOutOfRangeException(nameof(form));
                    }
                }
                case PBESpecies.Basculin:
                {
                    switch (form)
                    {
                        case PBEForm.Basculin_Blue: return nameof(PBEForm.Basculin_Blue);
                        case PBEForm.Basculin_Red: return nameof(PBEForm.Basculin_Red);
                        default: throw new ArgumentOutOfRangeException(nameof(form));
                    }
                }
                case PBESpecies.Burmy:
                {
                    switch (form)
                    {
                        case PBEForm.Burmy_Plant: return nameof(PBEForm.Burmy_Plant);
                        case PBEForm.Burmy_Sandy: return nameof(PBEForm.Burmy_Sandy);
                        case PBEForm.Burmy_Trash: return nameof(PBEForm.Burmy_Trash);
                        default: throw new ArgumentOutOfRangeException(nameof(form));
                    }
                }
                case PBESpecies.Castform:
                {
                    switch (form)
                    {
                        case PBEForm.Castform: return nameof(PBEForm.Castform);
                        case PBEForm.Castform_Rainy: return nameof(PBEForm.Castform_Rainy);
                        case PBEForm.Castform_Snowy: return nameof(PBEForm.Castform_Snowy);
                        case PBEForm.Castform_Sunny: return nameof(PBEForm.Castform_Sunny);
                        default: throw new ArgumentOutOfRangeException(nameof(form));
                    }
                }
                case PBESpecies.Cherrim:
                {
                    switch (form)
                    {
                        case PBEForm.Cherrim: return nameof(PBEForm.Cherrim);
                        case PBEForm.Cherrim_Sunshine: return nameof(PBEForm.Cherrim_Sunshine);
                        default: throw new ArgumentOutOfRangeException(nameof(form));
                    }
                }
                case PBESpecies.Darmanitan:
                {
                    switch (form)
                    {
                        case PBEForm.Darmanitan: return nameof(PBEForm.Darmanitan);
                        case PBEForm.Darmanitan_Zen: return nameof(PBEForm.Darmanitan_Zen);
                        default: throw new ArgumentOutOfRangeException(nameof(form));
                    }
                }
                case PBESpecies.Deerling:
                {
                    switch (form)
                    {
                        case PBEForm.Deerling_Autumn: return nameof(PBEForm.Deerling_Autumn);
                        case PBEForm.Deerling_Spring: return nameof(PBEForm.Deerling_Spring);
                        case PBEForm.Deerling_Summer: return nameof(PBEForm.Deerling_Summer);
                        case PBEForm.Deerling_Winter: return nameof(PBEForm.Deerling_Winter);
                        default: throw new ArgumentOutOfRangeException(nameof(form));
                    }
                }
                case PBESpecies.Deoxys:
                {
                    switch (form)
                    {
                        case PBEForm.Deoxys: return nameof(PBEForm.Deoxys);
                        case PBEForm.Deoxys_Attack: return nameof(PBEForm.Deoxys_Attack);
                        case PBEForm.Deoxys_Defense: return nameof(PBEForm.Deoxys_Defense);
                        case PBEForm.Deoxys_Speed: return nameof(PBEForm.Deoxys_Speed);
                        default: throw new ArgumentOutOfRangeException(nameof(form));
                    }
                }
                case PBESpecies.Gastrodon:
                {
                    switch (form)
                    {
                        case PBEForm.Gastrodon_East: return nameof(PBEForm.Gastrodon_East);
                        case PBEForm.Gastrodon_West: return nameof(PBEForm.Gastrodon_West);
                        default: throw new ArgumentOutOfRangeException(nameof(form));
                    }
                }
                case PBESpecies.Genesect:
                {
                    switch (form)
                    {
                        case PBEForm.Genesect: return nameof(PBEForm.Genesect);
                        case PBEForm.Genesect_Burn: return nameof(PBEForm.Genesect_Burn);
                        case PBEForm.Genesect_Chill: return nameof(PBEForm.Genesect_Chill);
                        case PBEForm.Genesect_Douse: return nameof(PBEForm.Genesect_Douse);
                        case PBEForm.Genesect_Shock: return nameof(PBEForm.Genesect_Shock);
                        default: throw new ArgumentOutOfRangeException(nameof(form));
                    }
                }
                case PBESpecies.Giratina:
                {
                    switch (form)
                    {
                        case PBEForm.Giratina: return nameof(PBEForm.Giratina);
                        case PBEForm.Giratina_Origin: return nameof(PBEForm.Giratina_Origin);
                        default: throw new ArgumentOutOfRangeException(nameof(form));
                    }
                }
                case PBESpecies.Keldeo:
                {
                    switch (form)
                    {
                        case PBEForm.Keldeo: return nameof(PBEForm.Keldeo);
                        case PBEForm.Keldeo_Resolute: return nameof(PBEForm.Keldeo_Resolute);
                        default: throw new ArgumentOutOfRangeException(nameof(form));
                    }
                }
                case PBESpecies.Kyurem:
                {
                    switch (form)
                    {
                        case PBEForm.Kyurem: return nameof(PBEForm.Kyurem);
                        case PBEForm.Kyurem_Black: return nameof(PBEForm.Kyurem_Black);
                        case PBEForm.Kyurem_White: return nameof(PBEForm.Kyurem_White);
                        default: throw new ArgumentOutOfRangeException(nameof(form));
                    }
                }
                case PBESpecies.Landorus:
                {
                    switch (form)
                    {
                        case PBEForm.Landorus: return nameof(PBEForm.Landorus);
                        case PBEForm.Landorus_Therian: return nameof(PBEForm.Landorus_Therian);
                        default: throw new ArgumentOutOfRangeException(nameof(form));
                    }
                }
                case PBESpecies.Meloetta:
                {
                    switch (form)
                    {
                        case PBEForm.Meloetta: return nameof(PBEForm.Meloetta);
                        case PBEForm.Meloetta_Pirouette: return nameof(PBEForm.Meloetta_Pirouette);
                        default: throw new ArgumentOutOfRangeException(nameof(form));
                    }
                }
                case PBESpecies.Rotom:
                {
                    switch (form)
                    {
                        case PBEForm.Rotom: return nameof(PBEForm.Rotom);
                        case PBEForm.Rotom_Fan: return nameof(PBEForm.Rotom_Fan);
                        case PBEForm.Rotom_Frost: return nameof(PBEForm.Rotom_Frost);
                        case PBEForm.Rotom_Heat: return nameof(PBEForm.Rotom_Heat);
                        case PBEForm.Rotom_Mow: return nameof(PBEForm.Rotom_Mow);
                        case PBEForm.Rotom_Wash: return nameof(PBEForm.Rotom_Wash);
                        default: throw new ArgumentOutOfRangeException(nameof(form));
                    }
                }
                case PBESpecies.Sawsbuck:
                {
                    switch (form)
                    {
                        case PBEForm.Sawsbuck_Autumn: return nameof(PBEForm.Sawsbuck_Autumn);
                        case PBEForm.Sawsbuck_Spring: return nameof(PBEForm.Sawsbuck_Spring);
                        case PBEForm.Sawsbuck_Summer: return nameof(PBEForm.Sawsbuck_Summer);
                        case PBEForm.Sawsbuck_Winter: return nameof(PBEForm.Sawsbuck_Winter);
                        default: throw new ArgumentOutOfRangeException(nameof(form));
                    }
                }
                case PBESpecies.Shaymin:
                {
                    switch (form)
                    {
                        case PBEForm.Shaymin: return nameof(PBEForm.Shaymin);
                        case PBEForm.Shaymin_Sky: return nameof(PBEForm.Shaymin_Sky);
                        default: throw new ArgumentOutOfRangeException(nameof(form));
                    }
                }
                case PBESpecies.Shellos:
                {
                    switch (form)
                    {
                        case PBEForm.Shellos_East: return nameof(PBEForm.Shellos_East);
                        case PBEForm.Shellos_West: return nameof(PBEForm.Shellos_West);
                        default: throw new ArgumentOutOfRangeException(nameof(form));
                    }
                }
                case PBESpecies.Thundurus:
                {
                    switch (form)
                    {
                        case PBEForm.Thundurus: return nameof(PBEForm.Thundurus);
                        case PBEForm.Thundurus_Therian: return nameof(PBEForm.Thundurus_Therian);
                        default: throw new ArgumentOutOfRangeException(nameof(form));
                    }
                }
                case PBESpecies.Tornadus:
                {
                    switch (form)
                    {
                        case PBEForm.Tornadus: return nameof(PBEForm.Tornadus);
                        case PBEForm.Tornadus_Therian: return nameof(PBEForm.Tornadus_Therian);
                        default: throw new ArgumentOutOfRangeException(nameof(form));
                    }
                }
                case PBESpecies.Unown:
                {
                    switch (form)
                    {
                        case PBEForm.Unown_A: return nameof(PBEForm.Unown_A);
                        case PBEForm.Unown_B: return nameof(PBEForm.Unown_B);
                        case PBEForm.Unown_C: return nameof(PBEForm.Unown_C);
                        case PBEForm.Unown_D: return nameof(PBEForm.Unown_D);
                        case PBEForm.Unown_E: return nameof(PBEForm.Unown_E);
                        case PBEForm.Unown_Exclamation: return nameof(PBEForm.Unown_Exclamation);
                        case PBEForm.Unown_F: return nameof(PBEForm.Unown_F);
                        case PBEForm.Unown_G: return nameof(PBEForm.Unown_G);
                        case PBEForm.Unown_H: return nameof(PBEForm.Unown_H);
                        case PBEForm.Unown_I: return nameof(PBEForm.Unown_I);
                        case PBEForm.Unown_J: return nameof(PBEForm.Unown_J);
                        case PBEForm.Unown_K: return nameof(PBEForm.Unown_K);
                        case PBEForm.Unown_L: return nameof(PBEForm.Unown_L);
                        case PBEForm.Unown_M: return nameof(PBEForm.Unown_M);
                        case PBEForm.Unown_N: return nameof(PBEForm.Unown_N);
                        case PBEForm.Unown_O: return nameof(PBEForm.Unown_O);
                        case PBEForm.Unown_P: return nameof(PBEForm.Unown_P);
                        case PBEForm.Unown_Q: return nameof(PBEForm.Unown_Q);
                        case PBEForm.Unown_Question: return nameof(PBEForm.Unown_Question);
                        case PBEForm.Unown_R: return nameof(PBEForm.Unown_R);
                        case PBEForm.Unown_S: return nameof(PBEForm.Unown_S);
                        case PBEForm.Unown_T: return nameof(PBEForm.Unown_T);
                        case PBEForm.Unown_U: return nameof(PBEForm.Unown_U);
                        case PBEForm.Unown_V: return nameof(PBEForm.Unown_V);
                        case PBEForm.Unown_W: return nameof(PBEForm.Unown_W);
                        case PBEForm.Unown_X: return nameof(PBEForm.Unown_X);
                        case PBEForm.Unown_Y: return nameof(PBEForm.Unown_Y);
                        case PBEForm.Unown_Z: return nameof(PBEForm.Unown_Z);
                        default: throw new ArgumentOutOfRangeException(nameof(form));
                    }
                }
                case PBESpecies.Wormadam:
                {
                    switch (form)
                    {
                        case PBEForm.Wormadam_Plant: return nameof(PBEForm.Wormadam_Plant);
                        case PBEForm.Wormadam_Sandy: return nameof(PBEForm.Wormadam_Sandy);
                        case PBEForm.Wormadam_Trash: return nameof(PBEForm.Wormadam_Trash);
                        default: throw new ArgumentOutOfRangeException(nameof(form));
                    }
                }
                default: return null;
            }
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
                case PBESpecies.Castform: return requireUsableOutsideOfBattle ? (IReadOnlyList<PBEForm>)Array.Empty<PBEForm>() : _castform;
                case PBESpecies.Cherrim: return requireUsableOutsideOfBattle ? (IReadOnlyList<PBEForm>)Array.Empty<PBEForm>() : _cherrim;
                case PBESpecies.Darmanitan: return requireUsableOutsideOfBattle ? (IReadOnlyList<PBEForm>)Array.Empty<PBEForm>() : _darmanitan;
                case PBESpecies.Deerling: return _deerling;
                case PBESpecies.Deoxys: return _deoxys;
                case PBESpecies.Gastrodon: return _gastrodon;
                case PBESpecies.Genesect: return _genesect;
                case PBESpecies.Giratina: return _giratina;
                case PBESpecies.Keldeo: return _keldeo;
                case PBESpecies.Kyurem: return _kyurem;
                case PBESpecies.Landorus: return _landorus;
                case PBESpecies.Meloetta: return requireUsableOutsideOfBattle ? (IReadOnlyList<PBEForm>)Array.Empty<PBEForm>() : _meloetta;
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
            PBEPokemonShell.ValidateSpecies(species, form, false);
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
