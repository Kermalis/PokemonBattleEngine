using Kermalis.PokemonBattleEngine.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Kermalis.PokemonBattleEngine.Battle
{
    // This constant looping order that's present in hitting as well as turn ended effects is very weird and unnecessary, but I mimic it for accuracy
    // That's why this file exists in favor of the order I had before
    public sealed partial class PBEBattle
    {
        private class PBEAttackVictim
        {
            public PBEBattlePokemon Pkmn { get; }
            public PBEResult Result { get; }
            public float TypeEffectiveness { get; }
            public bool Crit { get; set; }
            public ushort Damage { get; set; }

            public PBEAttackVictim(PBEBattlePokemon pkmn, PBEResult result, float typeEffectiveness)
            {
                Pkmn = pkmn; Result = result; TypeEffectiveness = typeEffectiveness;
            }
        }

        // TODO: TripleKick miss logic
        private void Hit_GetVictims(PBEBattlePokemon user, PBEBattlePokemon[] targets, IPBEMoveData mData, PBEType moveType, out List<PBEAttackVictim> victims,
            Func<PBEBattlePokemon, PBEResult>? failFunc = null)
        {
            victims = new List<PBEAttackVictim>(targets.Length);
            foreach (PBEBattlePokemon target in targets)
            {
                if (!AttackTypeCheck(user, target, moveType, out PBEResult result, out float typeEffectiveness))
                {
                    continue;
                }
                // Verified: These fails are after type effectiveness (So SuckerPunch will not affect Ghost types due to Normalize before it fails due to invalid conditions)
                if (failFunc is not null && failFunc.Invoke(target) != PBEResult.Success)
                {
                    continue;
                }
                victims.Add(new PBEAttackVictim(target, result, typeEffectiveness));
            }
            if (victims.Count == 0)
            {
                return;
            }
            victims.RemoveAll(t => MissCheck(user, t.Pkmn, mData));
            return;
        }
        // Outs are for hit targets that were not behind substitute
        private static void Hit_HitTargets(PBETeam user, Action<List<PBEAttackVictim>> doSub, Action<List<PBEAttackVictim>> doNormal, List<PBEAttackVictim> victims,
            out List<PBEAttackVictim> allies, out List<PBEAttackVictim> foes)
        {
            List<PBEAttackVictim> subAllies = victims.FindAll(v =>
            {
                PBEBattlePokemon pkmn = v.Pkmn;
                return pkmn.Team == user && pkmn.Status2.HasFlag(PBEStatus2.Substitute);
            });
            allies = victims.FindAll(v =>
            {
                PBEBattlePokemon pkmn = v.Pkmn;
                return pkmn.Team == user && !pkmn.Status2.HasFlag(PBEStatus2.Substitute);
            });
            List<PBEAttackVictim> subFoes = victims.FindAll(v =>
            {
                PBEBattlePokemon pkmn = v.Pkmn;
                return pkmn.Team != user && pkmn.Status2.HasFlag(PBEStatus2.Substitute);
            });
            foes = victims.FindAll(v =>
            {
                PBEBattlePokemon pkmn = v.Pkmn;
                return pkmn.Team != user && !pkmn.Status2.HasFlag(PBEStatus2.Substitute);
            });
            doSub(subAllies);
            doNormal(allies);
            doSub(subFoes);
            doNormal(foes);
        }
        private void Hit_DoCrit(List<PBEAttackVictim> victims)
        {
            foreach (PBEAttackVictim victim in victims)
            {
                if (victim.Crit)
                {
                    BroadcastMoveCrit(victim.Pkmn);
                }
            }
        }
        private void Hit_DoMoveResult(PBEBattlePokemon user, List<PBEAttackVictim> victims)
        {
            foreach (PBEAttackVictim victim in victims)
            {
                PBEResult result = victim.Result;
                if (result != PBEResult.Success)
                {
                    BroadcastMoveResult(user, victim.Pkmn, result);
                }
            }
        }
        private void Hit_FaintCheck(List<PBEAttackVictim> victims)
        {
            foreach (PBEAttackVictim victim in victims)
            {
                FaintCheck(victim.Pkmn);
            }
        }

        private void BasicHit(PBEBattlePokemon user, PBEBattlePokemon[] targets, IPBEMoveData mData,
            Func<PBEBattlePokemon, PBEResult>? failFunc = null,
            Action<PBEBattlePokemon>? beforeDoingDamage = null,
            Action<PBEBattlePokemon, ushort>? beforePostHit = null,
            Action<PBEBattlePokemon>? afterPostHit = null,
            Func<int, int?>? recoilFunc = null)
        {
            // Targets array is [FoeLeft, FoeCenter, FoeRight, AllyLeft, AllyCenter, AllyRight]
            // User can faint or heal with a berry at LiquidOoze, IronBarbs/RockyHelmet, and also at Recoil/LifeOrb
            // -------------Official order-------------
            // Setup   - [effectiveness/fail checks foes], [effectiveness/fail checks allies], [miss/protection checks foes] [miss/protection checks allies], gem,
            // Allies  - [sub damage allies, sub effectiveness allies, sub crit allies, sub break allies], [hit allies], [effectiveness allies], [crit allies], [posthit allies], [faint allies],
            // Foes    - [sub damage foes, sub effectiveness foes, sub crit foes, sub break foes], [hit foes], [effectiveness foes], [crit foes], [posthit foes], [faint foes],
            // Cleanup - recoil, lifeorb, [colorchange foes], [colorchange allies], [berry allies], [berry foes], [antistatusability allies], [antistatusability foes], exp

            PBEType moveType = user.GetMoveType(mData);
            // DreamEater checks for sleep before gem activates
            // SuckerPunch fails
            Hit_GetVictims(user, targets, mData, moveType, out List<PBEAttackVictim> victims, failFunc: failFunc);
            if (victims.Count == 0)
            {
                return;
            }
            float basePower = CalculateBasePower(user, targets, mData, moveType); // Gem activates here
            float initDamageMultiplier = victims.Count > 1 ? 0.75f : 1;
            int totalDamageDealt = 0;
            void CalcDamage(PBEAttackVictim victim)
            {
                PBEBattlePokemon target = victim.Pkmn;
                PBEResult result = victim.Result;
                float damageMultiplier = initDamageMultiplier * victim.TypeEffectiveness;
                // Brick Break destroys Light Screen and Reflect before doing damage (after gem)
                // Feint destroys protection
                // Pay Day scatters coins
                beforeDoingDamage?.Invoke(target);
                bool crit = CritCheck(user, target, mData);
                damageMultiplier *= CalculateDamageMultiplier(user, target, mData, moveType, result, crit);
                int damage = (int)(damageMultiplier * CalculateDamage(user, target, mData, moveType, basePower, crit));
                victim.Damage = DealDamage(user, target, damage, ignoreSubstitute: false, ignoreSturdy: false);
                totalDamageDealt += victim.Damage;
                victim.Crit = crit;
            }
            void DoSub(List<PBEAttackVictim> subs)
            {
                foreach (PBEAttackVictim victim in subs)
                {
                    CalcDamage(victim);
                    PBEBattlePokemon target = victim.Pkmn;
                    PBEResult result = victim.Result;
                    if (result != PBEResult.Success)
                    {
                        BroadcastMoveResult(user, target, result);
                    }
                    if (victim.Crit)
                    {
                        BroadcastMoveCrit(target);
                    }
                    if (target.SubstituteHP == 0)
                    {
                        BroadcastStatus2(target, user, PBEStatus2.Substitute, PBEStatusAction.Ended);
                    }
                }
            }
            void DoNormal(List<PBEAttackVictim> normals)
            {
                foreach (PBEAttackVictim victim in normals)
                {
                    CalcDamage(victim);
                }
                Hit_DoMoveResult(user, normals);
                Hit_DoCrit(normals);
                foreach (PBEAttackVictim victim in normals)
                {
                    PBEBattlePokemon target = victim.Pkmn;
                    // Stats/statuses are changed before post-hit effects
                    // HP-draining moves restore HP
                    beforePostHit?.Invoke(target, victim.Damage); // TODO: LiquidOoze fainting/healing
                    DoPostHitEffects(user, target, mData, moveType);
                    // ShadowForce destroys protection
                    // SmellingSalt cures paralysis
                    // WakeUpSlap cures sleep
                    afterPostHit?.Invoke(target); // Verified: These happen before Recoil/LifeOrb
                }
                Hit_FaintCheck(normals);
            }

            Hit_HitTargets(user.Team, DoSub, DoNormal, victims, out List<PBEAttackVictim> allies, out List<PBEAttackVictim> foes);
            DoPostAttackedEffects(user, allies, foes, true, recoilDamage: recoilFunc?.Invoke(totalDamageDealt), colorChangeType: moveType);
        }
        // None of these moves are multi-target
        private void FixedDamageHit(PBEBattlePokemon user, PBEBattlePokemon[] targets, IPBEMoveData mData, Func<PBEBattlePokemon, int> damageFunc,
            Func<PBEBattlePokemon, PBEResult>? failFunc = null,
            Action? beforePostHit = null)
        {
            PBEType moveType = user.GetMoveType(mData);
            // Endeavor fails if the target's HP is <= the user's HP
            // One hit knockout moves fail if the target's level is > the user's level
            Hit_GetVictims(user, targets, mData, moveType, out List<PBEAttackVictim> victims, failFunc: failFunc);
            if (victims.Count == 0)
            {
                return;
            }
            // BUG: Gems activate for these moves despite base power not being involved
            if (!Settings.BugFix)
            {
                _ = CalculateBasePower(user, targets, mData, moveType);
            }
            void CalcDamage(PBEAttackVictim victim)
            {
                PBEBattlePokemon target = victim.Pkmn;
                // FinalGambit user faints here
                victim.Damage = DealDamage(user, target, damageFunc.Invoke(target));
            }
            void DoSub(List<PBEAttackVictim> subs)
            {
                foreach (PBEAttackVictim victim in subs)
                {
                    CalcDamage(victim);
                    PBEBattlePokemon target = victim.Pkmn;
                    if (target.SubstituteHP == 0)
                    {
                        BroadcastStatus2(target, user, PBEStatus2.Substitute, PBEStatusAction.Ended);
                    }
                }
            }
            void DoNormal(List<PBEAttackVictim> normals)
            {
                foreach (PBEAttackVictim victim in normals)
                {
                    CalcDamage(victim);
                }
                foreach (PBEAttackVictim victim in normals)
                {
                    PBEBattlePokemon target = victim.Pkmn;
                    // "It's a one-hit KO!"
                    beforePostHit?.Invoke();
                    DoPostHitEffects(user, target, mData, moveType);
                }
                Hit_FaintCheck(normals);
            }

            Hit_HitTargets(user.Team, DoSub, DoNormal, victims, out List<PBEAttackVictim> allies, out List<PBEAttackVictim> foes);
            DoPostAttackedEffects(user, allies, foes, false, colorChangeType: moveType);
        }
        // None of these moves are multi-target
        private void MultiHit(PBEBattlePokemon user, PBEBattlePokemon[] targets, IPBEMoveData mData, byte numHits,
            bool subsequentMissChecks = false,
            Action<PBEBattlePokemon>? beforePostHit = null)
        {
            PBEType moveType = user.GetMoveType(mData);
            Hit_GetVictims(user, targets, mData, moveType, out List<PBEAttackVictim> victims);
            if (victims.Count == 0)
            {
                return;
            }
            float basePower = CalculateBasePower(user, targets, mData, moveType); // Verified: Gem boost applies to all hits
            float initDamageMultiplier = victims.Count > 1 ? 0.75f : 1;
            void CalcDamage(PBEAttackVictim victim)
            {
                PBEBattlePokemon target = victim.Pkmn;
                PBEResult result = victim.Result;
                float damageMultiplier = initDamageMultiplier * victim.TypeEffectiveness;
                bool crit = CritCheck(user, target, mData);
                damageMultiplier *= CalculateDamageMultiplier(user, target, mData, moveType, result, crit);
                int damage = (int)(damageMultiplier * CalculateDamage(user, target, mData, moveType, basePower, crit));
                victim.Damage = DealDamage(user, target, damage, ignoreSubstitute: false, ignoreSturdy: false);
                victim.Crit = crit;
            }
            void DoSub(List<PBEAttackVictim> subs)
            {
                foreach (PBEAttackVictim victim in subs)
                {
                    CalcDamage(victim);
                    PBEBattlePokemon target = victim.Pkmn;
                    if (victim.Crit)
                    {
                        BroadcastMoveCrit(target);
                    }
                    if (target.SubstituteHP == 0)
                    {
                        BroadcastStatus2(target, user, PBEStatus2.Substitute, PBEStatusAction.Ended);
                    }
                }
            }
            void DoNormal(List<PBEAttackVictim> normals)
            {
                normals.RemoveAll(v => v.Pkmn.HP == 0); // Remove ones that fainted from previous hits
                foreach (PBEAttackVictim victim in normals)
                {
                    CalcDamage(victim);
                }
                Hit_DoCrit(normals);
                foreach (PBEAttackVictim victim in normals)
                {
                    PBEBattlePokemon target = victim.Pkmn;
                    // Twineedle has a chance to poison on each strike
                    beforePostHit?.Invoke(target);
                    DoPostHitEffects(user, target, mData, moveType);
                }
            }

            byte hit = 0;
            List<PBEAttackVictim> allies, foes;
            do
            {
                Hit_HitTargets(user.Team, DoSub, DoNormal, victims, out allies, out foes);
                hit++;
            } while (hit < numHits && user.HP > 0 && user.Status1 != PBEStatus1.Asleep && victims.FindIndex(v => v.Pkmn.HP > 0) != -1);
            Hit_DoMoveResult(user, allies);
            Hit_DoMoveResult(user, foes);
            BroadcastMultiHit(hit);
            Hit_FaintCheck(allies);
            Hit_FaintCheck(foes);
            DoPostAttackedEffects(user, allies, foes, true, colorChangeType: moveType);
        }
    }
}
