using Kermalis.PokemonBattleEngine.Data;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kermalis.PokemonBattleEngine.Battle;

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
	private async Task<List<PBEAttackVictim>> Hit_GetVictims(PBEBattlePokemon user, PBEBattlePokemon[] targets, IPBEMoveData mData, PBEType moveType,
		Func<PBEBattlePokemon, Task<PBEResult>>? failFunc = null)
	{
		var victims = new List<PBEAttackVictim>(targets.Length);
		foreach (PBEBattlePokemon target in targets)
		{
			(bool success, PBEResult result, float typeEffectiveness) = await AttackTypeCheck(user, target, moveType);
			if (!success)
			{
				continue;
			}
			// Verified: These fails are after type effectiveness (So SuckerPunch will not affect Ghost types due to Normalize, then it fails due to invalid conditions)
			if (failFunc is not null && await failFunc(target) != PBEResult.Success)
			{
				continue;
			}
			victims.Add(new PBEAttackVictim(target, result, typeEffectiveness));
		}
		if (victims.Count == 0)
		{
			return victims;
		}

		var victims2 = new List<PBEAttackVictim>(victims);
		foreach (PBEAttackVictim victim in victims)
		{
			if (await MissCheck(user, victim.Pkmn, mData))
			{
				victims2.Remove(victim);
			}
		}
		return victims2;
	}
	// Outs are for hit targets that were not behind substitute
	private static async Task<(List<PBEAttackVictim> Allies, List<PBEAttackVictim> Foes)> Hit_HitTargets(PBETeam user, Func<List<PBEAttackVictim>, Task> doSub, Func<List<PBEAttackVictim>, Task> doNormal, List<PBEAttackVictim> victims)
	{
		List<PBEAttackVictim> subAllies = victims.FindAll(v =>
		{
			PBEBattlePokemon pkmn = v.Pkmn;
			return pkmn.Team == user && pkmn.Status2.HasFlag(PBEStatus2.Substitute);
		});
		List<PBEAttackVictim> allies = victims.FindAll(v =>
		{
			PBEBattlePokemon pkmn = v.Pkmn;
			return pkmn.Team == user && !pkmn.Status2.HasFlag(PBEStatus2.Substitute);
		});
		List<PBEAttackVictim> subFoes = victims.FindAll(v =>
		{
			PBEBattlePokemon pkmn = v.Pkmn;
			return pkmn.Team != user && pkmn.Status2.HasFlag(PBEStatus2.Substitute);
		});
		List<PBEAttackVictim> foes = victims.FindAll(v =>
		{
			PBEBattlePokemon pkmn = v.Pkmn;
			return pkmn.Team != user && !pkmn.Status2.HasFlag(PBEStatus2.Substitute);
		});
		await doSub(subAllies);
		await doNormal(allies);
		await doSub(subFoes);
		await doNormal(foes);
		return (allies, foes);
	}
	private async Task Hit_DoCrit(List<PBEAttackVictim> victims)
	{
		foreach (PBEAttackVictim victim in victims)
		{
			if (victim.Crit)
			{
				await BroadcastMoveCrit(victim.Pkmn);
			}
		}
	}
	private async Task Hit_DoMoveResult(PBEBattlePokemon user, List<PBEAttackVictim> victims)
	{
		foreach (PBEAttackVictim victim in victims)
		{
			PBEResult result = victim.Result;
			if (result != PBEResult.Success)
			{
				await BroadcastMoveResult(user, victim.Pkmn, result);
			}
		}
	}
	private async Task Hit_FaintCheck(List<PBEAttackVictim> victims)
	{
		foreach (PBEAttackVictim victim in victims)
		{
			await FaintCheck(victim.Pkmn);
		}
	}

	private async Task BasicHit(PBEBattlePokemon user, PBEBattlePokemon[] targets, IPBEMoveData mData,
		Func<PBEBattlePokemon, Task<PBEResult>>? failFunc = null,
		Func<PBEBattlePokemon, Task>? beforeDoingDamage = null,
		Func<PBEBattlePokemon, ushort, Task>? beforePostHit = null,
		Func<PBEBattlePokemon, Task>? afterPostHit = null,
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
		List<PBEAttackVictim> victims = await Hit_GetVictims(user, targets, mData, moveType, failFunc: failFunc);
		if (victims.Count == 0)
		{
			return;
		}
		float basePower = await CalculateBasePower(user, targets, mData, moveType); // Gem activates here
		float initDamageMultiplier = victims.Count > 1 ? 0.75f : 1;
		int totalDamageDealt = 0;
		async Task CalcDamage(PBEAttackVictim victim)
		{
			PBEBattlePokemon target = victim.Pkmn;
			PBEResult result = victim.Result;
			float damageMultiplier = initDamageMultiplier * victim.TypeEffectiveness;
			// Brick Break destroys Light Screen and Reflect before doing damage (after gem)
			// Feint destroys protection
			// Pay Day scatters coins
			if (beforeDoingDamage is not null)
			{
				await beforeDoingDamage(target);
			}
			bool crit = CritCheck(user, target, mData);
			damageMultiplier *= CalculateDamageMultiplier(user, target, mData, moveType, result, crit);
			int damage = (int)(damageMultiplier * CalculateDamage(user, target, mData, moveType, basePower, crit));
			victim.Damage = await DealDamage(user, target, damage, ignoreSubstitute: false, ignoreSturdy: false);
			totalDamageDealt += victim.Damage;
			victim.Crit = crit;
		}
		async Task DoSub(List<PBEAttackVictim> subs)
		{
			foreach (PBEAttackVictim victim in subs)
			{
				await CalcDamage(victim);
				PBEBattlePokemon target = victim.Pkmn;
				PBEResult result = victim.Result;
				if (result != PBEResult.Success)
				{
					await BroadcastMoveResult(user, target, result);
				}
				if (victim.Crit)
				{
					await BroadcastMoveCrit(target);
				}
				if (target.SubstituteHP == 0)
				{
					await BroadcastStatus2(target, user, PBEStatus2.Substitute, PBEStatusAction.Ended);
				}
			}
		}
		async Task DoNormal(List<PBEAttackVictim> normals)
		{
			foreach (PBEAttackVictim victim in normals)
			{
				await CalcDamage(victim);
			}
			await Hit_DoMoveResult(user, normals);
			await Hit_DoCrit(normals);
			foreach (PBEAttackVictim victim in normals)
			{
				PBEBattlePokemon target = victim.Pkmn;
				// Stats/statuses are changed before post-hit effects
				// HP-draining moves restore HP
				if (beforePostHit is not null)
				{
					await beforePostHit(target, victim.Damage); // TODO: LiquidOoze fainting/healing
				}
				await DoPostHitEffects(user, target, mData, moveType);
				// ShadowForce destroys protection
				// SmellingSalt cures paralysis
				// WakeUpSlap cures sleep
				if (afterPostHit is not null)
				{
					await afterPostHit(target); // Verified: These happen before Recoil/LifeOrb
				}
			}
			await Hit_FaintCheck(normals);
		}

		(List<PBEAttackVictim> allies, List<PBEAttackVictim> foes) = await Hit_HitTargets(user.Team, DoSub, DoNormal, victims);
		await DoPostAttackedEffects(user, allies, foes, true, recoilDamage: recoilFunc?.Invoke(totalDamageDealt), colorChangeType: moveType);
	}
	// None of these moves are multi-target
	private async Task FixedDamageHit(PBEBattlePokemon user, PBEBattlePokemon[] targets, IPBEMoveData mData, Func<PBEBattlePokemon, Task<int>> damageFunc,
		Func<PBEBattlePokemon, Task<PBEResult>>? failFunc = null,
		Func<Task>? beforePostHit = null)
	{
		PBEType moveType = user.GetMoveType(mData);
		// Endeavor fails if the target's HP is <= the user's HP
		// One hit knockout moves fail if the target's level is > the user's level
		List<PBEAttackVictim> victims = await Hit_GetVictims(user, targets, mData, moveType, failFunc: failFunc);
		if (victims.Count == 0)
		{
			return;
		}
		// BUG: Gems activate for these moves despite base power not being involved
		if (!Settings.BugFix)
		{
			_ = CalculateBasePower(user, targets, mData, moveType);
		}
		async Task CalcDamage(PBEAttackVictim victim)
		{
			PBEBattlePokemon target = victim.Pkmn;
			// FinalGambit user faints here
			victim.Damage = await DealDamage(user, target, await damageFunc(target));
		}
		async Task DoSub(List<PBEAttackVictim> subs)
		{
			foreach (PBEAttackVictim victim in subs)
			{
				await CalcDamage(victim);
				PBEBattlePokemon target = victim.Pkmn;
				if (target.SubstituteHP == 0)
				{
					await BroadcastStatus2(target, user, PBEStatus2.Substitute, PBEStatusAction.Ended);
				}
			}
		}
		async Task DoNormal(List<PBEAttackVictim> normals)
		{
			foreach (PBEAttackVictim victim in normals)
			{
				await CalcDamage(victim);
			}
			foreach (PBEAttackVictim victim in normals)
			{
				PBEBattlePokemon target = victim.Pkmn;
				// "It's a one-hit KO!"
				if (beforePostHit is not null)
				{
					await beforePostHit();
				}
				await DoPostHitEffects(user, target, mData, moveType);
			}
			await Hit_FaintCheck(normals);
		}

		(List<PBEAttackVictim> allies, List<PBEAttackVictim> foes) = await Hit_HitTargets(user.Team, DoSub, DoNormal, victims);
		await DoPostAttackedEffects(user, allies, foes, false, colorChangeType: moveType);
	}
	// None of these moves are multi-target
	private async Task MultiHit(PBEBattlePokemon user, PBEBattlePokemon[] targets, IPBEMoveData mData, byte numHits,
		bool subsequentMissChecks = false,
		Func<PBEBattlePokemon, Task>? beforePostHit = null)
	{
		PBEType moveType = user.GetMoveType(mData);
		List<PBEAttackVictim> victims = await Hit_GetVictims(user, targets, mData, moveType);
		if (victims.Count == 0)
		{
			return;
		}

		float basePower = await CalculateBasePower(user, targets, mData, moveType); // Verified: Gem boost applies to all hits
		float initDamageMultiplier = victims.Count > 1 ? 0.75f : 1;
		async Task CalcDamage(PBEAttackVictim victim)
		{
			PBEBattlePokemon target = victim.Pkmn;
			PBEResult result = victim.Result;
			float damageMultiplier = initDamageMultiplier * victim.TypeEffectiveness;
			bool crit = CritCheck(user, target, mData);
			damageMultiplier *= CalculateDamageMultiplier(user, target, mData, moveType, result, crit);
			int damage = (int)(damageMultiplier * CalculateDamage(user, target, mData, moveType, basePower, crit));
			victim.Damage = await DealDamage(user, target, damage, ignoreSubstitute: false, ignoreSturdy: false);
			victim.Crit = crit;
		}
		async Task DoSub(List<PBEAttackVictim> subs)
		{
			foreach (PBEAttackVictim victim in subs)
			{
				await CalcDamage(victim);
				PBEBattlePokemon target = victim.Pkmn;
				if (victim.Crit)
				{
					await BroadcastMoveCrit(target);
				}
				if (target.SubstituteHP == 0)
				{
					await BroadcastStatus2(target, user, PBEStatus2.Substitute, PBEStatusAction.Ended);
				}
			}
		}
		async Task DoNormal(List<PBEAttackVictim> normals)
		{
			normals.RemoveAll(v => v.Pkmn.HP == 0); // Remove ones that fainted from previous hits
			foreach (PBEAttackVictim victim in normals)
			{
				await CalcDamage(victim);
			}
			await Hit_DoCrit(normals);
			foreach (PBEAttackVictim victim in normals)
			{
				PBEBattlePokemon target = victim.Pkmn;
				// Twineedle has a chance to poison on each strike
				if (beforePostHit is not null)
				{
					await beforePostHit(target);
				}
				await DoPostHitEffects(user, target, mData, moveType);
			}
		}

		byte hit = 0;
		List<PBEAttackVictim> allies, foes;
		do
		{
			(allies, foes) = await Hit_HitTargets(user.Team, DoSub, DoNormal, victims);
			hit++;
		} while (hit < numHits && user.HP > 0 && user.Status1 != PBEStatus1.Asleep && victims.FindIndex(v => v.Pkmn.HP > 0) != -1);
		await Hit_DoMoveResult(user, allies);
		await Hit_DoMoveResult(user, foes);
		await BroadcastMultiHit(hit);
		await Hit_FaintCheck(allies);
		await Hit_FaintCheck(foes);
		await DoPostAttackedEffects(user, allies, foes, true, colorChangeType: moveType);
	}
}
