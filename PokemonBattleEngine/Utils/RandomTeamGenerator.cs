// This file is adapted from Pokémon Showdown (MIT License): https://github.com/smogon/pokemon-showdown/blob/master/data/mods/gen5/random-teams.ts
// Those guys know what they're doing!
using Kermalis.PokemonBattleEngine.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Kermalis.PokemonBattleEngine.Utils
{
    public static class PBERandomTeamGenerator
    {
        internal class PBETeamDetails
        {
            public bool Hail;
            public bool HarshSunlight;
            public bool Rain;
            public bool Sandstorm;
            public bool StealthRock;
            public bool ToxicSpikes;
            public bool RapidSpin;
        }

        internal class PBECounter
        {
            public int Damage;
            public int Recovery;
            public int STAB;
            public int Inaccurate;
            public int Priority;
            public int Recoil;
            public int Drain;
            public int Sound;
            public int PhysicalSetup;
            public int SpecialSetup;
            public int MixedSetup;
            public int SpeedSetup;
            public int PhysicalPool;
            public int SpecialPool;
            public int Hazards;
            public int DamagingMoves;
            public char SetupCategory; // 'N', 'M', 'P', 'S' - None, Mixed, Physical, Special
            private readonly Dictionary<PBEMoveCategory, float> _categories = new Dictionary<PBEMoveCategory, float>();
            private readonly Dictionary<PBEType, int> _types = new Dictionary<PBEType, int>();
            private readonly Dictionary<PBEAbility, int> _abilities = new Dictionary<PBEAbility, int>();

            public PBECounter()
            {
                for (PBEMoveCategory c = 0; c < PBEMoveCategory.MAX; c++)
                {
                    _categories.Add(c, 0);
                }
                for (var t = (PBEType)1; t < PBEType.MAX; t++)
                {
                    _types.Add(t, 0);
                }
                foreach (PBEAbility a in _counterAbilities)
                {
                    _abilities.Add(a, 0);
                }
            }

            public float this[PBEMoveCategory category]
            {
                get => _categories[category];
                set => _categories[category] = value;
            }
            public int this[PBEType type]
            {
                get => _types[type];
                set => _types[type] = value;
            }
            public int this[PBEAbility ability]
            {
                get => _abilities[ability];
                set => _abilities[ability] = value;
            }
        }

        private static readonly PBEMove[] _setupExceptionMoves = new[] { PBEMove.CloseCombat, PBEMove.DracoMeteor, PBEMove.ExtremeSpeed, PBEMove.SuckerPunch, PBEMove.Superpower };
        private static readonly PBEAbility[] _counterAbilities = new[] { PBEAbility.Adaptability, PBEAbility.Contrary, PBEAbility.Hustle, PBEAbility.IronFist, PBEAbility.SereneGrace, PBEAbility.SheerForce, PBEAbility.SkillLink, PBEAbility.Technician };
        private static readonly PBEMove[] _recoveryMoves = new[] { PBEMove.HealOrder, PBEMove.MilkDrink, PBEMove.Moonlight, PBEMove.MorningSun, PBEMove.Recover, PBEMove.Roost, PBEMove.SlackOff, PBEMove.Softboiled, PBEMove.Synthesis };
        private static readonly PBEMove[] _contraryMoves = new[] { PBEMove.CloseCombat, PBEMove.LeafStorm, PBEMove.Overheat, PBEMove.Superpower, PBEMove.VCreate };
        private static readonly PBEMove[] _physicalSetupMoves = new[] { PBEMove.BellyDrum, PBEMove.BulkUp, PBEMove.Coil, PBEMove.Curse, PBEMove.DragonDance, PBEMove.HoneClaws, PBEMove.Howl, PBEMove.SwordsDance };
        private static readonly PBEMove[] _specialSetupMoves = new[] { PBEMove.CalmMind, PBEMove.ChargeBeam, PBEMove.NastyPlot, PBEMove.QuiverDance, PBEMove.TailGlow };
        private static readonly PBEMove[] _mixedSetupMoves = new[] { PBEMove.Growth, PBEMove.ShellSmash, PBEMove.WorkUp };
        private static readonly PBEMove[] _speedSetupMoves = new[] { PBEMove.Agility, PBEMove.Autotomize, PBEMove.FlameCharge, PBEMove.RockPolish, PBEMove.ShiftGear };
        private static readonly PBEMove[] _hazardMoves = new[] { PBEMove.Spikes, PBEMove.StealthRock, PBEMove.ToxicSpikes };
        // Moves that shouldn't be the only STAB move
        private static readonly PBEMove[] _noSTABMoves = new[] { PBEMove.AquaJet, PBEMove.Bounce, PBEMove.Explosion, PBEMove.FakeOut, PBEMove.FlameCharge, PBEMove.IceShard, PBEMove.MachPunch, PBEMove.Pluck,
        PBEMove.Pursuit, PBEMove.QuickAttack, PBEMove.Selfdestruct, PBEMove.SuckerPunch, PBEMove.ClearSmog, PBEMove.Eruption, PBEMove.IcyWind, PBEMove.Incinerate, PBEMove.Snarl, PBEMove.VacuumWave, PBEMove.WaterSpout };

        private static PBEAbility GetAbility(PBESpecies species, List<PBEMove> moves, PBEPokemonData pData, PBECounter counter, PBETeamDetails teamDs)
        {
            var abilityPool = new List<PBEAbility>(pData.Abilities);
            PBEAbility ability;
            do
            {
                bool reject = false;
                ability = abilityPool.RandomElement();
                // Reasons to reject
                switch (ability)
                {
                    case PBEAbility.AngerPoint:
                    case PBEAbility.Gluttony:
                    case PBEAbility.Moody: reject = true; break;
                    case PBEAbility.Blaze: reject = counter[PBEType.Fire] == 0; break;
                    case PBEAbility.Chlorophyll: reject = !moves.Contains(PBEMove.SunnyDay) && !teamDs.HarshSunlight; break;
                    case PBEAbility.Compoundeyes:
                    case PBEAbility.NoGuard: reject = counter.Inaccurate == 0; break;
                    case PBEAbility.Defiant:
                    case PBEAbility.Moxie: reject = counter[PBEMoveCategory.Physical] == 0 && !moves.Contains(PBEMove.BatonPass); break;
                    case PBEAbility.Hydration:
                    case PBEAbility.RainDish:
                    case PBEAbility.SwiftSwim: reject = !moves.Contains(PBEMove.RainDance) && !teamDs.Rain; break;
                    case PBEAbility.IceBody:
                    case PBEAbility.SnowCloak: reject = !teamDs.Hail; break;
                    // Zangoose
                    case PBEAbility.Immunity: reject = pData.Abilities.Contains(PBEAbility.ToxicBoost); break;
                    case PBEAbility.Lightningrod: reject = pData.HasType(PBEType.Ground); break;
                    // Basculin
                    case PBEAbility.MoldBreaker: reject = pData.Abilities.Contains(PBEAbility.Adaptability); break;
                    case PBEAbility.Overgrow: reject = counter[PBEType.Grass] == 0; break;
                    // Breloom
                    case PBEAbility.PoisonHeal: reject = pData.Abilities.Contains(PBEAbility.Technician) && counter[PBEAbility.Technician] > 0; break;
                    case PBEAbility.Prankster: reject = counter[PBEMoveCategory.Status] == 0; break;
                    case PBEAbility.Reckless:
                    case PBEAbility.RockHead: reject = counter.Recoil == 0; break;
                    // Solosis, Duosion, Reuniclus
                    case PBEAbility.Regenerator: reject = pData.Abilities.Contains(PBEAbility.MagicGuard); break;
                    case PBEAbility.SandForce:
                    case PBEAbility.SandRush:
                    case PBEAbility.SandVeil: reject = !teamDs.Sandstorm; break;
                    case PBEAbility.SereneGrace: reject = species == PBESpecies.Blissey || species == PBESpecies.Togetic; break;
                    // Timburr, Gurdurr, Conkeldurr
                    case PBEAbility.SheerForce: reject = moves.Contains(PBEMove.FakeOut) || (pData.Abilities.Contains(PBEAbility.IronFist) && counter[PBEAbility.IronFist] > counter[PBEAbility.SheerForce]); break;
                    case PBEAbility.Simple:
                    case PBEAbility.WeakArmor: reject = counter.SetupCategory == 'N'; break;
                    case PBEAbility.Sturdy: reject = counter.Recoil > 0 && counter.Recovery == 0; break;
                    case PBEAbility.Swarm: reject = counter[PBEType.Bug] == 0; break;
                    // Ambipom, Minccino, Cinccino
                    case PBEAbility.Technician: reject = pData.Abilities.Contains(PBEAbility.SkillLink) && counter[PBEAbility.SkillLink] >= counter[PBEAbility.Technician]; break;
                    case PBEAbility.TintedLens: reject = counter.Damage >= counter.DamagingMoves || (counter[PBEMoveCategory.Status] > 2 && counter.SetupCategory == 'N'); break;
                    case PBEAbility.Torrent: reject = counter[PBEType.Water] == 0; break;
                    // Clefable
                    case PBEAbility.Unaware: reject = pData.Abilities.Contains(PBEAbility.MagicGuard) && counter[PBEMoveCategory.Status] < 2; break;
                    case PBEAbility.Unburden: reject = pData.BaseStats.Speed > 100; break;
                    // Chinchou, Lanturn
                    case PBEAbility.WaterAbsorb: reject = pData.Abilities.Contains(PBEAbility.VoltAbsorb); break;
                }
                // Reasons to always accept
                if (!reject)
                {
                    if (_counterAbilities.Contains(ability))
                    {
                        reject = counter[ability] == 0;
                        if (!reject) // If we have an ability that the counter says has good moves, use the ability
                        {
                            break;
                        }
                    }
                    else if (ability == PBEAbility.Prankster && counter[PBEMoveCategory.Status] > 1)
                    {
                        break;
                    }
                    else if (ability == PBEAbility.SwiftSwim && moves.Contains(PBEMove.RainDance))
                    {
                        break;
                    }
                    else if ((ability == PBEAbility.Guts || ability == PBEAbility.QuickFeet) && moves.Contains(PBEMove.Facade))
                    {
                        break;
                    }
                }
                // Ability was rejected
                if (reject)
                {
                    ability = PBEAbility.None;
                    abilityPool.Remove(ability);
                }
            } while (ability == PBEAbility.None && abilityPool.Count > 0);
            return ability;
        }
        private static PBEItem GetItem(PBESpecies species, PBEForm form, PBEAbility ability, List<PBEMove> moves, PBEPokemonData pData, PBECounter counter, bool isLead)
        {
            PBEItem item;
            void GetRandomGem()
            {
                var list = new List<PBEType>();
                foreach (PBEMove move in moves)
                {
                    PBEMoveData mData = PBEMoveData.Data[move];
                    //if (!move.basePower && !move.basePowerCallback) continue;
                    // KERMALIS: Instead gonna check status for now
                    if (mData.Category != PBEMoveCategory.Status)
                    {
                        list.Add(mData.Type);
                    }
                }
                item = PBEDataUtils.TypeToGem[list.RandomElement()];
            }
            // First, the extra high-priority items
            if (species == PBESpecies.Marowak)
            {
                item = PBEItem.ThickClub;
            }
            else if (species == PBESpecies.Deoxys && form == PBEForm.Deoxys_Attack)
            {
                item = isLead && moves.Contains(PBEMove.StealthRock) ? PBEItem.FocusSash : PBEItem.LifeOrb;
            }
            else if (species == PBESpecies.Farfetchd)
            {
                item = PBEItem.Stick;
            }
            else if (species == PBESpecies.Pikachu)
            {
                item = PBEItem.LightBall;
            }
            else if (species == PBESpecies.Shedinja || species == PBESpecies.Smeargle)
            {
                item = PBEItem.FocusSash;
            }
            else if (species == PBESpecies.Unown)
            {
                item = PBEItem.ChoiceSpecs;
            }
            else if (species == PBESpecies.Wobbuffet && moves.Contains(PBEMove.DestinyBond) && PBERandom.RandomBool())
            {
                item = PBEItem.CustapBerry;
            }
            else if (ability == PBEAbility.Imposter)
            {
                item = PBEItem.ChoiceScarf;
            }
            else if ((moves.Contains(PBEMove.Switcheroo) || moves.Contains(PBEMove.Trick)) && moves.Contains(PBEMove.GyroBall))
            {
                item = ability == PBEAbility.Levitate || pData.HasType(PBEType.Flying) ? PBEItem.MachoBrace : PBEItem.IronBall;
            }
            else if (moves.Contains(PBEMove.Switcheroo) || moves.Contains(PBEMove.Trick))
            {
                if (pData.BaseStats.Speed >= 60 && pData.BaseStats.Speed <= 108)
                {
                    item = PBEItem.ChoiceScarf;
                }
                else
                {
                    item = counter[PBEMoveCategory.Physical] > counter[PBEMoveCategory.Special] ? PBEItem.ChoiceBand : PBEItem.ChoiceSpecs;
                }
            }
            else if (pData.Evolutions.Count > 0)
            {
                item = PBEItem.Eviolite;
            }
            else if (moves.Contains(PBEMove.ShellSmash))
            {
                item = PBEItem.WhiteHerb;
            }
            else if (ability == PBEAbility.Harvest || moves.Contains(PBEMove.BellyDrum))
            {
                item = PBEItem.SitrusBerry;
            }
            else if ((ability == PBEAbility.MagicGuard || ability == PBEAbility.SheerForce) && counter.DamagingMoves > 1)
            {
                item = PBEItem.LifeOrb;
            }
            else if (moves.Contains(PBEMove.Facade) || ability == PBEAbility.PoisonHeal || ability == PBEAbility.ToxicBoost)
            {
                item = PBEItem.ToxicOrb;
            }
            else if (moves.Contains(PBEMove.Rest) && !moves.Contains(PBEMove.SleepTalk) && ability != PBEAbility.NaturalCure && ability != PBEAbility.ShedSkin)
            {
                item = PBEItem.ChestoBerry;
            }
            else if (moves.Contains(PBEMove.RainDance))
            {
                item = PBEItem.DampRock;
            }
            else if (moves.Contains(PBEMove.SunnyDay))
            {
                item = PBEItem.HeatRock;
            }
            else if (moves.Contains(PBEMove.LightScreen) || moves.Contains(PBEMove.Reflect))
            {
                item = PBEItem.LightClay;
            }
            else if (moves.Contains(PBEMove.Acrobatics))
            {
                item = PBEItem.FlyingGem;
            }
            else if (moves.Contains(PBEMove.PsychoShift) || (ability == PBEAbility.Guts && !moves.Contains(PBEMove.SleepTalk)))
            {
                item = moves.Contains(PBEMove.DrainPunch) ? PBEItem.FlameOrb : PBEItem.ToxicOrb;
            }
            else if (ability == PBEAbility.Unburden && (counter[PBEMoveCategory.Physical] > 0 || counter[PBEMoveCategory.Special] > 0))
            {
                // Give Unburden mons a random Gem of the type of one of their damaging moves
                GetRandomGem();
            }
            // Medium priority
            else if ((moves.Contains(PBEMove.Eruption) || moves.Contains(PBEMove.WaterSpout)) && counter[PBEMoveCategory.Status] == 0)
            {
                item = PBEItem.ChoiceScarf;
            }
            else if (ability == PBEAbility.SpeedBoost && !moves.Contains(PBEMove.Substitute) && counter[PBEMoveCategory.Physical] + counter[PBEMoveCategory.Special] > 2)
            {
                item = PBEItem.LifeOrb;
            }
            else if (counter[PBEMoveCategory.Physical] >= PBESettings.DefaultNumMoves && !moves.Contains(PBEMove.DragonTail) && !moves.Contains(PBEMove.FakeOut) && !moves.Contains(PBEMove.FlameCharge)
                && !moves.Contains(PBEMove.SuckerPunch) && (!moves.Contains(PBEMove.RapidSpin) || PBETypeEffectiveness.GetEffectiveness(PBEType.Rock, pData) < 1))
            {
                item = (pData.BaseStats.Attack >= 100 || pData.Abilities.Contains(PBEAbility.HugePower)) && pData.BaseStats.Speed >= 60 && pData.BaseStats.Speed <= 108 && counter.Priority == 0 && PBERandom.RandomBool(2, 3) ? PBEItem.ChoiceScarf : PBEItem.ChoiceBand;
            }
            else if (counter[PBEMoveCategory.Special] >= PBESettings.DefaultNumMoves && !moves.Contains(PBEMove.ClearSmog) && !moves.Contains(PBEMove.FieryDance))
            {
                item = pData.BaseStats.SpAttack >= 100 && pData.BaseStats.Speed >= 60 && pData.BaseStats.Speed <= 108 && counter.Priority == 0 && PBERandom.RandomBool(2, 3) ? PBEItem.ChoiceScarf : PBEItem.ChoiceSpecs;
            }
            else if (counter[PBEMoveCategory.Special] >= 3 && moves.Contains(PBEMove.Uturn))
            {
                item = PBEItem.ChoiceSpecs;
            }
            else if (PBETypeEffectiveness.GetEffectiveness(PBEType.Ground, pData) > 1 && ability != PBEAbility.Levitate && !moves.Contains(PBEMove.MagnetRise))
            {
                item = PBEItem.AirBalloon;
            }
            else if (moves.Contains(PBEMove.Substitute) && moves.Contains(PBEMove.Reversal))
            {
                GetRandomGem();
            }
            else if ((moves.Contains(PBEMove.Flail) || moves.Contains(PBEMove.Reversal)) && ability != PBEAbility.Sturdy)
            {
                item = PBEItem.FocusSash;
            }
            else if (ability == PBEAbility.SlowStart || moves.Contains(PBEMove.Detect) || moves.Contains(PBEMove.Protect) || moves.Contains(PBEMove.SleepTalk) || moves.Contains(PBEMove.Substitute))
            {
                item = PBEItem.Leftovers;
            }
            else if (ability == PBEAbility.IronBarbs)
            {
                item = PBEItem.RockyHelmet;
            }
            else if (species == PBESpecies.Palkia && (moves.Contains(PBEMove.DracoMeteor) || moves.Contains(PBEMove.SpacialRend)) && moves.Contains(PBEMove.HydroPump))
            {
                item = PBEItem.LustrousOrb;
            }
            else if (pData.BaseStats.HP + pData.BaseStats.Defense + pData.BaseStats.SpDefense > 275)
            {
                item = PBEItem.Leftovers;
            }
            else if (counter[PBEMoveCategory.Physical] + counter[PBEMoveCategory.Special] >= 3 && counter.SetupCategory != 'N' && ability != PBEAbility.Sturdy && !moves.Contains(PBEMove.RapidSpin))
            {
                item = moves.Contains(PBEMove.Outrage) ? PBEItem.LumBerry : PBEItem.LifeOrb;
            }
            else if (counter[PBEMoveCategory.Physical] + counter[PBEMoveCategory.Special] >= PBESettings.DefaultNumMoves)
            {
                item = counter[PBEType.Normal] > 0 ? PBEItem.LifeOrb : PBEItem.ExpertBelt;
            }
            else if (isLead && ability != PBEAbility.Regenerator && ability != PBEAbility.Sturdy && counter.Recoil == 0 && counter.Recovery == 0 && pData.BaseStats.HP + pData.BaseStats.Defense + pData.BaseStats.SpDefense <= 275)
            {
                item = PBEItem.FocusSash;
            }
            // This is the "REALLY can't think of a good item" cutoff
            else if (PBETypeEffectiveness.GetEffectiveness(PBEType.Rock, pData) >= 1 || moves.Contains(PBEMove.DragonTail))
            {
                item = PBEItem.Leftovers;
            }
            else if (counter[PBEMoveCategory.Status] <= 1 && ability != PBEAbility.Sturdy && !moves.Contains(PBEMove.RapidSpin) && !moves.Contains(PBEMove.Uturn))
            {
                item = PBEItem.LifeOrb;
            }
            else
            {
                item = PBEItem.Leftovers;
            }

            // For Trick / Switcheroo
            if (item == PBEItem.Leftovers && pData.HasType(PBEType.Poison))
            {
                item = PBEItem.BlackSludge;
            }
            return item;
        }
        private static List<PBEMove> GetMoves(PBESpecies species, PBEForm form, PBEPokemonData pData, bool isLead, PBETeamDetails teamDs, out PBECounter counter)
        {
            var movePool = new List<PBEMove>(PBELegalityChecker.GetLegalMoves(species, form, PBESettings.DefaultMaxLevel, PBESettings.DefaultSettings));
            var moves = new List<PBEMove>(PBESettings.DefaultNumMoves);
            int startI;
            if (species == PBESpecies.Keldeo && form == PBEForm.Keldeo_Resolute)
            {
                moves[0] = PBEMove.SecretSword;
                movePool.Remove(PBEMove.SecretSword);
                startI = 1;
            }
            else
            {
                startI = 0;
            }
            var rejectedPool = new List<PBEMove>();

            do
            {
                // Choose next moves from learnset/viable moves and add them to moves list:
                while (moves.Count < PBESettings.DefaultNumMoves && movePool.Count > 0)
                {
                    PBEMove move = movePool.RandomElement();
                    movePool.Remove(move);
                    moves.Add(move);
                }
                while (moves.Count < PBESettings.DefaultNumMoves && rejectedPool.Count > 0)
                {
                    PBEMove move = rejectedPool.RandomElement();
                    rejectedPool.Remove(move);
                    moves.Add(move);
                }

                counter = QueryMoves(pData, moves, movePool);

                // If there are no more moves to choose, there's nothing to cull
                if (movePool.Count == 0 && rejectedPool.Count == 0)
                {
                    break;
                }

                // Iterate through the moves again, this time to cull them:
                for (int i = startI; i < moves.Count; i++)
                {
                    PBEMove move = moves[i];
                    PBEMoveData mData = PBEMoveData.Data[move];

                    bool reject = false;
                    bool isSetup = false;

                    switch (move)
                    {
                        // Not very useful without their supporting moves
                        case PBEMove.BatonPass:
                        {
                            if (counter.SetupCategory == 'N' && counter.SpeedSetup == 0 && !moves.Contains(PBEMove.Substitute) && !moves.Contains(PBEMove.Wish) && !pData.HasAbility(PBEAbility.SpeedBoost))
                            {
                                reject = true;
                            }
                            break;
                        }
                        case PBEMove.FocusPunch:
                        {
                            if (!moves.Contains(PBEMove.Substitute) || counter.DamagingMoves < 2 || moves.Contains(PBEMove.SwordsDance))
                            {
                                reject = true;
                            }
                            break;
                        }
                        case PBEMove.PerishSong:
                        {
                            if (!moves.Contains(PBEMove.Protect))
                            {
                                reject = true;
                            }
                            break;
                        }
                        case PBEMove.Rest:
                        {
                            if (!movePool.Contains(PBEMove.SleepTalk))
                            {
                                reject = true;
                            }
                            break;
                        }
                        case PBEMove.SleepTalk:
                        {
                            if (!moves.Contains(PBEMove.Rest))
                            {
                                reject = true;
                            }
                            movePool.Remove(PBEMove.Rest);
                            break;
                        }
                        case PBEMove.StoredPower:
                        {
                            if (counter.SetupCategory == 'N' && !moves.Contains(PBEMove.CosmicPower))
                            {
                                reject = true;
                            }
                            break;
                        }

                        // Set up once and only if we have the moves for it
                        case PBEMove.BellyDrum:
                        case PBEMove.BulkUp:
                        case PBEMove.Coil:
                        case PBEMove.Curse:
                        case PBEMove.DragonDance:
                        case PBEMove.HoneClaws:
                        case PBEMove.SwordsDance:
                        {
                            if (counter.SetupCategory != 'P' || counter.PhysicalSetup > 1)
                            {
                                reject = true;
                            }
                            else if (counter[PBEMoveCategory.Physical] + counter.PhysicalPool < 2 && !moves.Contains(PBEMove.BatonPass) && (!moves.Contains(PBEMove.Rest) || !moves.Contains(PBEMove.SleepTalk)))
                            {
                                reject = true;
                            }
                            isSetup = true;
                            break;
                        }
                        case PBEMove.CalmMind:
                        case PBEMove.NastyPlot:
                        case PBEMove.QuiverDance:
                        case PBEMove.TailGlow:
                        {
                            if (counter.SetupCategory != 'S' || counter.SpecialSetup > 1)
                            {
                                reject = true;
                            }
                            else if (counter[PBEMoveCategory.Special] + counter.SpecialPool < 2 && !moves.Contains(PBEMove.BatonPass) && (!moves.Contains(PBEMove.Rest) || !moves.Contains(PBEMove.SleepTalk)))
                            {
                                reject = true;
                            }
                            isSetup = true;
                            break;
                        }
                        case PBEMove.Growth:
                        case PBEMove.ShellSmash:
                        case PBEMove.WorkUp:
                        {
                            if (counter.SetupCategory != 'M' || counter.MixedSetup > 1)
                            {
                                reject = true;
                            }
                            else if (counter.DamagingMoves + counter.PhysicalPool + counter.SpecialPool < 2 && !moves.Contains(PBEMove.BatonPass))
                            {
                                reject = true;
                            }
                            else if (move == PBEMove.Growth && !moves.Contains(PBEMove.SunnyDay))
                            {
                                reject = true;
                            }
                            isSetup = true;
                            break;
                        }
                        case PBEMove.Agility:
                        case PBEMove.Autotomize:
                        case PBEMove.RockPolish:
                        {
                            if (counter.DamagingMoves < 2 && counter.SetupCategory == 'N' && !moves.Contains(PBEMove.BatonPass))
                            {
                                reject = true;
                            }
                            else if (moves.Contains(PBEMove.Rest) && moves.Contains(PBEMove.SleepTalk))
                            {
                                reject = true;
                            }
                            else if (counter.SetupCategory == 'N')
                            {
                                isSetup = true;
                            }
                            break;
                        }

                        // Bad after setup
                        case PBEMove.BulletPunch:
                        {
                            if (counter.SpeedSetup > 0)
                            {
                                reject = true;
                            }
                            break;
                        }
                        case PBEMove.CircleThrow:
                        case PBEMove.DragonTail:
                        {
                            if (counter.SetupCategory != 'N' && ((!moves.Contains(PBEMove.Rest) && !moves.Contains(PBEMove.SleepTalk)) || moves.Contains(PBEMove.StormThrow)))
                            {
                                reject = true;
                            }
                            else if (counter.SpeedSetup > 0 || moves.Contains(PBEMove.Encore) || moves.Contains(PBEMove.Roar) || moves.Contains(PBEMove.Whirlwind))
                            {
                                reject = true;
                            }
                            break;
                        }
                        case PBEMove.FakeOut:
                        {
                            if (counter.SetupCategory != 'N' || moves.Contains(PBEMove.Substitute) || moves.Contains(PBEMove.Switcheroo) || moves.Contains(PBEMove.Trick))
                            {
                                reject = true;
                            }
                            break;
                        }
                        case PBEMove.Haze:
                        case PBEMove.MagicCoat:
                        case PBEMove.Pursuit:
                        case PBEMove.Selfdestruct:
                        case PBEMove.Spikes:
                        case PBEMove.WaterSpout:
                        {
                            if (counter.SetupCategory != 'N' || counter.SpeedSetup > 0 || (moves.Contains(PBEMove.Rest) && moves.Contains(PBEMove.SleepTalk)))
                            {
                                reject = true;
                            }
                            break;
                        }
                        case PBEMove.HealingWish:
                        {
                            if (counter.SetupCategory != 'N' || counter.Recovery > 0 || moves.Contains(PBEMove.Substitute))
                            {
                                reject = true;
                            }
                            break;
                        }
                        case PBEMove.LeechSeed:
                        case PBEMove.Roar:
                        case PBEMove.Whirlwind:
                        {
                            if (counter.SetupCategory != 'N' || counter.SpeedSetup > 0 || moves.Contains(PBEMove.DragonTail))
                            {
                                reject = true;
                            }
                            break;
                        }
                        case PBEMove.NightShade:
                        case PBEMove.SeismicToss:
                        case PBEMove.SuperFang:
                        {
                            if (counter.DamagingMoves > 1 || counter.SetupCategory != 'N')
                            {
                                reject = true;
                            }
                            break;
                        }
                        case PBEMove.Protect:
                        {
                            if (counter.SetupCategory != 'N' && (pData.HasAbility(PBEAbility.Guts) || pData.HasAbility(PBEAbility.SpeedBoost)) && !moves.Contains(PBEMove.BatonPass))
                            {
                                reject = true;
                            }
                            else if (moves.Contains(PBEMove.Rest) || (moves.Contains(PBEMove.LightScreen) && moves.Contains(PBEMove.Reflect))) // A typo? light screen and reflect together? not ||?
                            {
                                reject = true;
                            }
                            break;
                        }
                        case PBEMove.RapidSpin:
                        {
                            if (moves.Contains(PBEMove.ShellSmash) || (counter.SetupCategory != 'N' && counter[PBEMoveCategory.Status] >= 2))
                            {
                                reject = true;
                            }
                            break;
                        }
                        case PBEMove.StealthRock:
                        {
                            if (counter.SetupCategory != 'N' || counter.SpeedSetup > 0 || moves.Contains(PBEMove.Rest) || teamDs.StealthRock)
                            {
                                reject = true;
                            }
                            break;
                        }
                        case PBEMove.Switcheroo:
                        case PBEMove.Trick:
                        {
                            if (counter[PBEMoveCategory.Physical] + counter[PBEMoveCategory.Special] < 3 || counter.Priority > 0 || moves.Contains(PBEMove.RapidSpin))
                            {
                                reject = true;
                            }
                            break;
                        }
                        case PBEMove.ToxicSpikes:
                        {
                            if (counter.SetupCategory != 'N' || teamDs.ToxicSpikes)
                            {
                                reject = true;
                            }
                            break;
                        }
                        case PBEMove.TrickRoom:
                        {
                            if (counter.SetupCategory != 'N' || counter.SpeedSetup > 0 || counter.DamagingMoves < 2)
                            {
                                reject = true;
                            }
                            if (moves.Contains(PBEMove.LightScreen) || moves.Contains(PBEMove.Reflect))
                            {
                                reject = true;
                            }
                            break;
                        }
                        case PBEMove.Uturn:
                        {
                            if (counter.SetupCategory != 'N' || counter.SpeedSetup > 0 || moves.Contains(PBEMove.BatonPass)) // KERMALIS: Should reject if they have volt switch?
                            {
                                reject = true;
                            }
                            break;
                        }
                        case PBEMove.VoltSwitch:
                        {
                            if (counter.SetupCategory != 'N' || counter.SpeedSetup > 0 || moves.Contains(PBEMove.BatonPass) || moves.Contains(PBEMove.MagnetRise) || moves.Contains(PBEMove.Uturn))
                            {
                                reject = true;
                            }
                            break;
                        }

                        // Bit redundant to have both
                        // Attacks:
                        case PBEMove.BugBite:
                        {
                            if (moves.Contains(PBEMove.Uturn))
                            {
                                reject = true;
                            }
                            break;
                        }
                        case PBEMove.Crunch:
                        {
                            if (!pData.HasType(PBEType.Dark) && moves.Contains(PBEMove.SuckerPunch))
                            {
                                reject = true;
                            }
                            break;
                        }
                        case PBEMove.CloseCombat:
                        {
                            if (counter.SetupCategory != 'N' && moves.Contains(PBEMove.AuraSphere))
                            {
                                reject = true;
                            }
                            break;
                        }
                        case PBEMove.DragonPulse:
                        case PBEMove.SpacialRend:
                        {
                            if (moves.Contains(PBEMove.DracoMeteor) || moves.Contains(PBEMove.Outrage))
                            {
                                reject = true;
                            }
                            break;
                        }
                        case PBEMove.Thunderbolt:
                        {
                            if (moves.Contains(PBEMove.WildCharge))
                            {
                                reject = true;
                            }
                            break;
                        }
                        case PBEMove.AuraSphere:
                        case PBEMove.HiJumpKick:
                        {
                            if (counter.SetupCategory != 'N' && moves.Contains(PBEMove.CloseCombat))
                            {
                                reject = true;
                            }
                            break;
                        }
                        case PBEMove.DrainPunch:
                        case PBEMove.FocusBlast:
                        {
                            if (moves.Contains(PBEMove.CloseCombat) || moves.Contains(PBEMove.CrossChop) || moves.Contains(PBEMove.HiJumpKick) || moves.Contains(PBEMove.LowKick))
                            {
                                reject = true;
                            }
                            break;
                        }
                        case PBEMove.BlueFlare:
                        case PBEMove.FlareBlitz:
                        case PBEMove.FieryDance:
                        case PBEMove.Flamethrower:
                        case PBEMove.LavaPlume:
                        {
                            if (moves.Contains(PBEMove.FireBlast) || moves.Contains(PBEMove.Overheat) || moves.Contains(PBEMove.VCreate))
                            {
                                reject = true;
                            }
                            break;
                        }
                        case PBEMove.AirSlash:
                        case PBEMove.BraveBird:
                        case PBEMove.Pluck:
                        {
                            if (moves.Contains(PBEMove.Acrobatics) || moves.Contains(PBEMove.Hurricane))
                            {
                                reject = true;
                            }
                            break;
                        }
                        case PBEMove.GigaDrain:
                        {
                            if ((counter.SetupCategory != 'N' && moves.Contains(PBEMove.LeafStorm)) || moves.Contains(PBEMove.PetalDance) || moves.Contains(PBEMove.PowerWhip))
                            {
                                reject = true;
                            }
                            break;
                        }
                        case PBEMove.SolarBeam:
                        {
                            if ((!pData.HasAbility(PBEAbility.Drought) && !moves.Contains(PBEMove.SunnyDay)) || moves.Contains(PBEMove.GigaDrain) || moves.Contains(PBEMove.LeafStorm))
                            {
                                reject = true;
                            }
                            break;
                        }
                        case PBEMove.LeafStorm:
                        {
                            if (counter.SetupCategory != 'N' && moves.Contains(PBEMove.GigaDrain))
                            {
                                reject = true;
                            }
                            break;
                        }
                        case PBEMove.Bonemerang:
                        case PBEMove.EarthPower:
                        {
                            if (moves.Contains(PBEMove.Earthquake))
                            {
                                reject = true;
                            }
                            break;
                        }
                        case PBEMove.Endeavor:
                        {
                            if (!isLead)
                            {
                                reject = true;
                            }
                            break;
                        }
                        case PBEMove.Facade:
                        {
                            if (moves.Contains(PBEMove.SuckerPunch) && !pData.HasType(PBEType.Normal))
                            {
                                reject = true;
                            }
                            break;
                        }
                        case PBEMove.Judgment:
                        {
                            if (counter.SetupCategory != 'S' && counter.STAB > 1)
                            {
                                reject = true;
                            }
                            break;
                        }
                        case PBEMove.Return:
                        {
                            if (moves.Contains(PBEMove.BodySlam) || moves.Contains(PBEMove.DoubleEdge))
                            {
                                reject = true;
                            }
                            break;
                        }
                        case PBEMove.WeatherBall:
                        {
                            if (!moves.Contains(PBEMove.SunnyDay))
                            {
                                reject = true;
                            }
                            break;
                        }
                        case PBEMove.PoisonJab:
                        {
                            if (moves.Contains(PBEMove.GunkShot))
                            {
                                reject = true;
                            }
                            break;
                        }
                        case PBEMove.Psychic:
                        {
                            if (moves.Contains(PBEMove.Psyshock))
                            {
                                reject = true;
                            }
                            break;
                        }
                        case PBEMove.RockBlast:
                        case PBEMove.RockSlide:
                        {
                            if (moves.Contains(PBEMove.HeadSmash) || moves.Contains(PBEMove.StoneEdge))
                            {
                                reject = true;
                            }
                            break;
                        }
                        case PBEMove.StoneEdge:
                        {
                            if (moves.Contains(PBEMove.HeadSmash))
                            {
                                reject = true;
                            }
                            break;
                        }
                        case PBEMove.Scald:
                        case PBEMove.Surf:
                        {
                            if (moves.Contains(PBEMove.HydroPump))
                            {
                                reject = true;
                            }
                            break;
                        }
                        case PBEMove.Waterfall:
                        {
                            if (moves.Contains(PBEMove.Scald) || (moves.Contains(PBEMove.Rest) && moves.Contains(PBEMove.SleepTalk)))
                            {
                                reject = true;
                            }
                            break;
                        }

                        // Status:
                        case PBEMove.Encore:
                        case PBEMove.IceShard:
                        case PBEMove.SuckerPunch:
                        {
                            if (moves.Contains(PBEMove.Rest) && moves.Contains(PBEMove.SleepTalk))
                            {
                                reject = true;
                            }
                            break;
                        }
                        case PBEMove.HealBell:
                        {
                            if (moves.Contains(PBEMove.MagicCoat))
                            {
                                reject = true;
                            }
                            break;
                        }
                        case PBEMove.Moonlight:
                        case PBEMove.PainSplit:
                        case PBEMove.Recover:
                        case PBEMove.Roost:
                        case PBEMove.Softboiled:
                        case PBEMove.Synthesis:
                        {
                            if (moves.Contains(PBEMove.LeechSeed) || moves.Contains(PBEMove.Rest) || moves.Contains(PBEMove.Wish))
                            {
                                reject = true;
                            }
                            break;
                        }
                        case PBEMove.Substitute:
                        {
                            if ((moves.Contains(PBEMove.DoubleEdge) && !pData.HasAbility(PBEAbility.RockHead)) || moves.Contains(PBEMove.Pursuit) || moves.Contains(PBEMove.Rest) || moves.Contains(PBEMove.Superpower) || moves.Contains(PBEMove.Uturn) || moves.Contains(PBEMove.VoltSwitch))
                            {
                                reject = true;
                            }
                            break;
                        }
                        case PBEMove.ThunderWave:
                        {
                            if (counter.SetupCategory != 'N' || counter.SpeedSetup > 0 || (moves.Contains(PBEMove.Rest) && moves.Contains(PBEMove.SleepTalk)))
                            {
                                reject = true;
                            }
                            if (moves.Contains(PBEMove.Discharge) || moves.Contains(PBEMove.TrickRoom))
                            {
                                reject = true;
                            }
                            break;
                        }
                        case PBEMove.WillOWisp:
                        {
                            if (moves.Contains(PBEMove.LavaPlume) || (moves.Contains(PBEMove.Scald) && !pData.HasType(PBEType.Ghost))) // KERMALIS: why ghost check
                            {
                                reject = true;
                            }
                            break;
                        }
                    }

                    // This move doesn't satisfy our setup requirements:
                    if ((mData.Category == PBEMoveCategory.Physical && counter.SetupCategory == 'S') || (mData.Category == PBEMoveCategory.Special && counter.SetupCategory == 'P'))
                    {
                        // Reject STABs last in case the setup type changes later on
                        int stabs = counter[pData.Type1];
                        if (pData.Type2 != PBEType.None)
                        {
                            stabs += counter[pData.Type2];
                        }
                        if (!_setupExceptionMoves.Contains(move) && (!pData.HasType(mData.Type) || stabs > 1 || counter[mData.Category] < 2))
                        {
                            reject = true;
                        }
                    }
                    PBEMoveCategory c = counter.SetupCategory == 'P' ? PBEMoveCategory.Physical : PBEMoveCategory.Special;
                    if (counter.SetupCategory != 'N' && !isSetup && counter.SetupCategory != 'M')
                    {
                        if (mData.Category != c && counter[c] < 2 && !moves.Contains(PBEMove.BatonPass) && (mData.Category != PBEMoveCategory.Status || !mData.IsHPRestoreMove()) && move != PBEMove.SleepTalk)
                        {
                            // Mono-attacking with setup and Rest/SleepTalk is allowed
                            // Reject status moves only if there is nothing else to reject
                            if (mData.Category != PBEMoveCategory.Status || (counter[c] + counter[PBEMoveCategory.Status] > 3 && counter.PhysicalSetup + counter.SpecialSetup < 2))
                            {
                                reject = true;
                            }
                        }
                    }
                    if (counter.SetupCategory == 'S' && move == PBEMove.HiddenPower && pData.Type2 != PBEType.None && counter[PBEMoveCategory.Special] <= 2 && !pData.HasType(mData.Type) && counter[PBEMoveCategory.Physical] == 0 && counter.SpecialPool > 0)
                    {
                        // Hidden Power isn't good enough
                        reject = true;
                    }

                    // Pokemon should have moves that benefit their Type/Ability/Weather, as well as moves required by its forme
                    if (!reject)
                    {
                        if ((
                            counter.PhysicalSetup + counter.SpecialSetup < 2 &&
                            (counter.SetupCategory == 'N' || counter.SetupCategory == 'M' || (mData.Category != PBEMoveCategory.Status && mData.Category != c) || counter[c] + counter[PBEMoveCategory.Status] > 3)
                            ) && (
                            (counter.STAB == 0 && counter.Damage == 0 && (pData.Type2 != PBEType.None || (pData.Type1 != PBEType.Normal && pData.Type1 != PBEType.Psychic) || !moves.Contains(PBEMove.IceBeam) || pData.BaseStats.SpAttack >= pData.BaseStats.SpDefense))
                            || (pData.HasType(PBEType.Dark) && counter[PBEType.Dark] == 0)
                            || (pData.HasType(PBEType.Dragon) && counter[PBEType.Dragon] == 0)
                            || (pData.HasType(PBEType.Electric) && counter[PBEType.Electric] == 0)
                            || (pData.HasType(PBEType.Fighting) && counter[PBEType.Fighting] == 0 && (pData.BaseStats.Attack >= 110 || pData.HasAbility(PBEAbility.Justified) || pData.HasAbility(PBEAbility.PurePower) || counter.SetupCategory != 'N' || counter[PBEMoveCategory.Status] == 0))
                            || (pData.HasType(PBEType.Fire) && counter[PBEType.Fire] == 0)
                            || (pData.HasType(PBEType.Flying) && pData.HasType(PBEType.Normal) && counter[PBEType.Flying] == 0)
                            || (pData.HasType(PBEType.Ground) && counter[PBEType.Ground] == 0 && !moves.Contains(PBEMove.Rest) && !moves.Contains(PBEMove.SleepTalk))
                            || (pData.HasType(PBEType.Ice) && counter[PBEType.Ice] == 0)
                            || (pData.HasType(PBEType.Rock) && counter[PBEType.Rock] == 0 && pData.BaseStats.Attack >= 80)
                            || (pData.HasType(PBEType.Steel) && pData.HasAbility(PBEAbility.Technician) && counter[PBEType.Steel] == 0)
                            || (pData.HasType(PBEType.Water) && counter[PBEType.Water] == 0)
                            || (
                            (pData.HasAbility(PBEAbility.Adaptability) && counter.SetupCategory == 'N' && pData.Type2 != PBEType.None && (counter[pData.Type1] == 0 || counter[pData.Type2] == 0))
                            || (pData.HasAbility(PBEAbility.BadDreams) && movePool.Contains(PBEMove.DarkVoid))
                            || (pData.HasAbility(PBEAbility.Contrary) && counter[PBEAbility.Contrary] == 0 && species != PBESpecies.Shuckle)
                            || (pData.HasAbility(PBEAbility.Guts) && pData.HasType(PBEType.Normal) && movePool.Contains(PBEMove.Facade))
                            || (pData.HasAbility(PBEAbility.SlowStart) && movePool.Contains(PBEMove.Substitute))
                            || (counter.Recovery == 0 && counter.SetupCategory == 'N' && !moves.Contains(PBEMove.HealingWish) && (
                            movePool.Contains(PBEMove.Recover) || movePool.Contains(PBEMove.Roost) || movePool.Contains(PBEMove.Softboiled)
                            ) && (counter[PBEMoveCategory.Status] > 1)))
                            ))
                        {
                            // Reject Status or non-STAB
                            if (!isSetup && !mData.IsWeatherMove() /*&& !move.damage*/ && (mData.Category != PBEMoveCategory.Status || !mData.IsHPRestoreMove()) && move != PBEMove.Judgment && move != PBEMove.SleepTalk)
                            {
                                if (mData.Category == PBEMoveCategory.Status || !pData.HasType(mData.Type) /*|| move.selfSwitch*/ || (mData.Power > 0 && mData.Power < 40 && !mData.IsMultiHitMove()))
                                {
                                    reject = true;
                                }
                            }
                        }
                    }

                    // Sleep Talk shouldn't be selected without Rest
                    if (move == PBEMove.Rest && reject)
                    {
                        int sleeptalk = moves.IndexOf(PBEMove.SleepTalk);
                        if (sleeptalk >= 0)
                        {
                            if (movePool.Count < 2)
                            {
                                reject = false;
                            }
                            else
                            {
                                movePool.RemoveAt(sleeptalk);
                            }
                        }
                    }

                    // Remove rejected moves from the move list
                    if (reject)
                    {
                        // Let's say our movePool originally has 4 moves and we reject three, we have one move and three are looping in rejectedPool forever.
                        // This if prevents them from looping through rejectedPool
                        if (movePool.Count > 0)
                        {
                            rejectedPool.Add(move);
                        }
                        moves.RemoveAt(i);
                        break;
                    }

                    // TODO: Hidden power IVs and types
                }
            } while (moves.Count < PBESettings.DefaultNumMoves);

            return moves;
        }

        private static PBECounter QueryMoves(PBEPokemonData pData, List<PBEMove> moves, List<PBEMove> movePool)
        {
            var counter = new PBECounter();

            if (moves.Count == 0)
            {
                return counter;
            }

            // Iterate through all moves we've chosen so far and keep track of what they do:
            foreach (PBEMove move in moves)
            {
                PBEMoveData mData = PBEMoveData.Data[move];
                PBEType moveType = move == PBEMove.Judgment ? pData.Type1 : mData.Type;

                // Moves that do a set amount of damage:
                if (mData.IsSetDamageMove())
                {
                    counter.Damage++;
                    counter.DamagingMoves++;
                }
                else // Are Physical/Special/Status moves:
                {
                    counter[mData.Category]++;
                }

                // Moves that have a low base power:
                if (move == PBEMove.LowKick || (mData.Power > 0 && mData.Power <= 60 && move != PBEMove.RapidSpin))
                {
                    counter[PBEAbility.Technician]++;
                }
                // Moves that hit up to 5 times: (KERMALIS: Also TripleKick)
                if (mData.IsMultiHitMove())
                {
                    counter[PBEAbility.SkillLink]++;
                }
                if (mData.IsRecoilMove())
                {
                    counter.Recoil++;
                }
                if (mData.IsHPDrainMove())
                {
                    counter.Drain++;
                }
                // Moves which have a base power, but aren't super-weak like RapidSpin
                if (move == PBEMove.NaturePower || mData.Power > 30 || mData.IsMultiHitMove()/* || mData.basePowerCallback*/)
                {
                    counter[moveType]++;
                    if (pData.HasType(moveType) || pData.Abilities.Contains(PBEAbility.Normalize))
                    {
                        counter[PBEAbility.Adaptability]++;
                        // STAB:
                        // Certain moves aren't acceptable as a Pokémon's only STAB attack
                        if (!_noSTABMoves.Contains(move) && (move != PBEMove.HiddenPower || pData.Type2 == PBEType.None))
                        {
                            counter.STAB++;
                            // Ties between Physical and Special setup should broken in favor of STABs
                            counter[mData.Category] += 0.1f;
                        }
                    }
                    if (mData.Flags.HasFlag(PBEMoveFlag.AffectedByIronFist))
                    {
                        counter[PBEAbility.IronFist]++;
                    }
                    if (mData.Flags.HasFlag(PBEMoveFlag.AffectedBySoundproof))
                    {
                        counter.Sound++;
                    }
                    counter.DamagingMoves++;
                }
                // Moves with secondary effects:
                if (mData.HasSecondaryEffects())
                {
                    counter[PBEAbility.SheerForce]++;
                    if (mData.EffectParam >= 20 && mData.EffectParam < 100)
                    {
                        counter[PBEAbility.SereneGrace]++;
                    }
                }
                // Moves with low accuracy:
                if (mData.Accuracy != 0 && mData.Accuracy < 90)
                {
                    counter.Inaccurate++;
                }
                // Moves with non-zero priority:
                if (mData.Category != PBEMoveCategory.Status && mData.Priority != 0)
                {
                    counter.Priority++;
                }
                // Moves that change stats:
                if (_recoveryMoves.Contains(move))
                {
                    counter.Recovery++;
                }
                if (_contraryMoves.Contains(move))
                {
                    counter[PBEAbility.Contrary]++;
                }
                if (_physicalSetupMoves.Contains(move))
                {
                    counter.PhysicalSetup++;
                    counter.SetupCategory = 'P';
                }
                else if (_specialSetupMoves.Contains(move))
                {
                    counter.SpecialSetup++;
                    counter.SetupCategory = 'S';
                }
                if (_mixedSetupMoves.Contains(move))
                {
                    counter.MixedSetup++;
                }
                if (_speedSetupMoves.Contains(move))
                {
                    counter.SpeedSetup++;
                }
                if (_hazardMoves.Contains(move))
                {
                    counter.Hazards++;
                }
            }

            // Keep track of available moves
            foreach (PBEMove move in movePool)
            {
                PBEMoveData mData = PBEMoveData.Data[move];
                if (!mData.IsSetDamageMove())
                {
                    if (mData.Category == PBEMoveCategory.Physical)
                    {
                        counter.PhysicalPool++;
                    }
                    else if (mData.Category == PBEMoveCategory.Special)
                    {
                        counter.SpecialPool++;
                    }
                }
            }

            // Choose a setup type:
            if (counter.MixedSetup > 0)
            {
                counter.SetupCategory = 'M';
            }
            else if (counter.PhysicalSetup > 0 && counter.SpecialSetup > 0)
            {
                float physical = counter[PBEMoveCategory.Physical] + counter.PhysicalPool;
                float special = counter[PBEMoveCategory.Special] + counter.SpecialPool;
                if (physical == special)
                {
                    if (counter[PBEMoveCategory.Physical] > counter[PBEMoveCategory.Special])
                    {
                        counter.SetupCategory = 'P';
                    }
                    if (counter[PBEMoveCategory.Special] > counter[PBEMoveCategory.Physical])
                    {
                        counter.SetupCategory = 'S';
                    }
                }
                else
                {
                    counter.SetupCategory = physical > special ? 'P' : 'S';
                }
            }
            else if (counter.SetupCategory == 'P')
            {
                if (counter[PBEMoveCategory.Physical] < 2 && (counter.STAB == 0 || counter.PhysicalPool == 0) && (!moves.Contains(PBEMove.Rest) || !moves.Contains(PBEMove.SleepTalk)))
                {
                    counter.SetupCategory = 'N';
                }
            }
            else if (counter.SetupCategory == 'S')
            {
                if (counter[PBEMoveCategory.Special] < 2 && (counter.STAB == 0 || counter.SpecialPool == 0) && (!moves.Contains(PBEMove.Rest) || !moves.Contains(PBEMove.SleepTalk)))
                {
                    counter.SetupCategory = 'N';
                }
            }

            counter[PBEMoveCategory.Physical] = (float)Math.Floor(counter[PBEMoveCategory.Physical]);
            counter[PBEMoveCategory.Special] = (float)Math.Floor(counter[PBEMoveCategory.Special]);

            return counter;
        }

        // TODO: Hidden power types
        private static void GetRandomSet(PBESpecies species, PBEForm form, PBEPokemonData pData, bool isLead, PBETeamDetails teamDs, PBEPokemonShell shell)
        {
            shell.EffortValues.Equalize();
            List<PBEMove> moves = GetMoves(species, form, pData, isLead, teamDs, out PBECounter counter);
            // If Hidden Power has been removed, reset the IVs
            if (!moves.Contains(PBEMove.HiddenPower))
            {
                shell.IndividualValues.Maximize();
            }

            if (shell.SelectableAbilities.Count > 1)
            {
                PBEAbility a = GetAbility(species, moves, pData, counter, teamDs);
                if (a != PBEAbility.None)
                {
                    shell.Ability = a;
                }
            }
            if (shell.SelectableItems.Count > 1)
            {
                shell.Item = GetItem(species, form, shell.Ability, moves, pData, counter, isLead);
            }

            // KERMALIS: This is where showdown does level scaling

            // Minimize confusion damage
            if (counter[PBEMoveCategory.Physical] == 0 && !moves.Contains(PBEMove.Transform))
            {
                shell.EffortValues.Attack = 0;
                shell.IndividualValues.Attack = 0;
            }
            if (moves.Contains(PBEMove.GyroBall) || moves.Contains(PBEMove.MetalBurst) || moves.Contains(PBEMove.TrickRoom))
            {
                shell.EffortValues.Speed = 0;
                shell.IndividualValues.Speed = 0;
            }
            shell.Friendship = moves.Contains(PBEMove.Frustration) ? byte.MinValue : byte.MaxValue;
            shell.Moveset.Clear();
            for (int i = 0; i < moves.Count; i++)
            {
                PBEMoveset.PBEMovesetSlot slot = shell.Moveset[i];
                slot.Move = moves[i];
                if (slot.IsPPUpsEditable)
                {
                    slot.PPUps = PBESettings.DefaultMaxPPUps;
                }
            }
        }

        /// <summary>Creates a random team meant for <see cref="PBESettings.DefaultSettings"/>.</summary>
        /// <param name="numPkmn">The amount of Pokémon to create in the team./></param>
        public static PBETeamShell CreateRandomTeam(int numPkmn)
        {
            return CreateRandomTeam(numPkmn, PBEDataUtils.FullyEvolvedSpecies);
        }
        // TODO: Move Illusion out of the way instead of just yeeting it if it's last
        // TODO: Tiers?, level scaling, limit one type combination
        // TODO: Hidden Power types
        // TODO: Custom settings
        // TODO: Non-competitive moves (such as MegaDrain/GigaDrain together, HelpingHand), ban certain species (such as Wobbuffet & Unown, but add certain pre-evolutions such as Pikachu)
        // TODO: Prioritize correct attack stat for species, currently will give special moves to a physical attacker, etc
        /// <summary>Creates a random team meant for <see cref="PBESettings.DefaultSettings"/>.</summary>
        /// <param name="numPkmn">The amount of Pokémon to create in the team./></param>
        /// <param name="allowedSpecies">The allowed species to consider.</param>
        public static PBETeamShell CreateRandomTeam(int numPkmn, IEnumerable<PBESpecies> allowedSpecies)
        {
            if (numPkmn < 1 || numPkmn > PBESettings.DefaultMaxPartySize)
            {
                throw new ArgumentOutOfRangeException(nameof(numPkmn));
            }
            if (allowedSpecies == null)
            {
                throw new ArgumentNullException(nameof(allowedSpecies));
            }

            var speciesPool = new List<PBESpecies>(allowedSpecies);
            var teamDs = new PBETeamDetails();
            var usedTypes = new Dictionary<PBEType, int>(numPkmn - 1);
            var team = new PBETeamShell(PBESettings.DefaultSettings, 1, true); // TODO: Don't generate any here
            int currentIndex = 0;
            while (true)
            {
                (PBESpecies species, PBEForm form) = speciesPool.RandomSpecies(true);
                bool RemoveSpeciesFromPool()
                {
                    speciesPool.Remove(species);
                    return speciesPool.Count == 0;
                }
                // KERMALIS: Showdown limits 2 per tier
                var pData = PBEPokemonData.GetData(species, form);
                if (ShouldDenyType(usedTypes, pData.Type1) || ShouldDenyType(usedTypes, pData.Type2))
                {
                    if (RemoveSpeciesFromPool())
                    {
                        break;
                    }
                    continue;
                }

                PBEPokemonShell shell;
                if (currentIndex == team.Count)
                {
                    team.Add(species, form, PBESettings.DefaultMaxLevel);
                    shell = team[currentIndex];
                }
                else
                {
                    shell = team[currentIndex];
                    shell.Species = species;
                    shell.Form = form;
                }
                GetRandomSet(species, form, pData, currentIndex == 0, teamDs, shell);

                // Illusion shouldn't be the last Pokémon of the team
                if (shell.Ability == PBEAbility.Illusion && currentIndex == numPkmn - 1)
                {
                    if (RemoveSpeciesFromPool())
                    {
                        break;
                    }
                    continue;
                }

                // KERMALIS: Showdown limits one type combination, excluding weather ability users

                // KERMALIS: Showdown sets Illusion user's level to the level of the last in the party

                // Now that our Pokémon has passed all checks, we can increment our counters
                if (team.Count >= numPkmn)
                {
                    break;
                }
                currentIndex++;
                if (RemoveSpeciesFromPool()) // No duplicate species
                {
                    break;
                }
                // Increment our tier counter

                // Increment type counters
                AddTypeToDict(usedTypes, pData.Type1);
                AddTypeToDict(usedTypes, pData.Type2);

                // Team details
                if (shell.Ability == PBEAbility.SnowWarning || shell.Moveset.Contains(PBEMove.Hail))
                {
                    teamDs.Hail = true;
                }
                if (shell.Ability == PBEAbility.Drizzle || shell.Moveset.Contains(PBEMove.RainDance))
                {
                    teamDs.Rain = true;
                }
                if (shell.Ability == PBEAbility.SandStream || shell.Moveset.Contains(PBEMove.Sandstorm))
                {
                    teamDs.Sandstorm = true;
                }
                if (shell.Ability == PBEAbility.Drought || shell.Moveset.Contains(PBEMove.SunnyDay))
                {
                    teamDs.HarshSunlight = true;
                }
                if (shell.Moveset.Contains(PBEMove.StealthRock))
                {
                    teamDs.StealthRock = true;
                }
                if (shell.Moveset.Contains(PBEMove.ToxicSpikes))
                {
                    teamDs.ToxicSpikes = true;
                }
                if (shell.Moveset.Contains(PBEMove.RapidSpin))
                {
                    teamDs.RapidSpin = true;
                }
            }

            if (team.Count < numPkmn)
            {
                throw new Exception("Failed to create a random team");
            }
            return team;
        }

        private static void AddTypeToDict(Dictionary<PBEType, int> dict, PBEType type)
        {
            if (type != PBEType.None)
            {
                if (dict.ContainsKey(type))
                {
                    dict[type]++;
                }
                else
                {
                    dict.Add(type, 1);
                }
            }
        }
        private static bool ShouldDenyType(Dictionary<PBEType, int> dict, PBEType type)
        {
            return type != PBEType.None && dict.TryGetValue(type, out int value) && value >= 2 && PBERandom.RandomBool(4, 5); // No type should appear more than twice
        }
    }
}
