using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Kermalis.PokemonBattleEngine.Battle;
using Kermalis.PokemonBattleEngine.Data;
using Kermalis.PokemonBattleEngine.Data.Legality;
using Kermalis.PokemonBattleEngine.DefaultData;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kermalis.PokemonBattleEngineDiscord;

internal static class Matchmaking
{
	public sealed class Challenge
	{
		public SocketUser Challenger { get; }
		public SocketUser Challengee { get; }
		public IGuild Guild { get; }
		public DateTime ChallengeTime { get; }

		public Challenge(SocketUser challenger, SocketUser challengee, IGuild guild)
		{
			Challenger = challenger;
			Challengee = challengee;
			Guild = guild;
			ChallengeTime = DateTime.Now;
		}
	}

	private const int ChallengeMinuteExpiration = 5;
	private static readonly object _matchmakingLockObj = new();
	private static readonly List<Challenge> _challenges = new();

	public static void OnLeftGuild(SocketGuild guild)
	{
		lock (_matchmakingLockObj)
		{
			_challenges.RemoveAll(c => c.Guild.Id == guild.Id);
		}
	}
	public static void OnUserLeft(SocketGuildUser user)
	{
		lock (_matchmakingLockObj)
		{
			_challenges.RemoveAll(c => (user.Id == c.Challenger.Id || user.Id == c.Challengee.Id) && user.Guild.Id == c.Guild.Id);
		}
	}

	private static async Task PrintParticipating(SocketUser tag, SocketUser participant, ISocketMessageChannel channel)
	{
		await channel.SendMessageAsync($"{tag.Mention} ― {participant.Username} is already participating in a battle.");
	}
	private static void RemoveOldChallenges()
	{
		DateTime dt = DateTime.Now;
		_challenges.RemoveAll(c => (dt - c.ChallengeTime).Minutes >= ChallengeMinuteExpiration);
	}
	private static Challenge? GetChallenge(SocketUser challengee)
	{
		foreach (Challenge c in _challenges)
		{
			if (c.Challengee.Id == challengee.Id)
			{
				return c;
			}
		}
		return null;
	}
	private static async Task StartBattle(Challenge c)
	{
		SocketUser a = c.Challenger;
		SocketUser b = c.Challengee;
		IGuild guild = c.Guild;
		_challenges.Remove(c);

		await StartBattle(guild, a, b, a.Username, b.Username, a.Mention, b.Mention);
	}
	private static async Task StartBattle(IGuild guild, SocketUser? battler0, SocketUser? battler1, string team0Name, string team1Name, string team0Mention, string team1Mention)
	{
		PBELegalPokemonCollection p0, p1;
		// Competitively Randomized Pokémon
		p0 = PBEDDRandomTeamGenerator.CreateRandomTeam(3);
		p1 = PBEDDRandomTeamGenerator.CreateRandomTeam(3);

		var battle = PBEBattle.CreateTrainerBattle(PBEBattleFormat.Single, PBESettings.DefaultSettings, new PBETrainerInfo(p0, team0Name, false), new PBETrainerInfo(p1, team1Name, false));

		var bc = new BattleContext(battle, battler0, battler1);
		ITextChannel channel = await ChannelHandler.CreateChannel(guild, $"battle-{bc.BattleId}");
		await channel.SendMessageAsync($"**Battle #{bc.BattleId} ― {team0Mention} vs {team1Mention}**");
		await bc.Begin(channel);
	}

	public static Task AcceptChallenge(SocketCommandContext ctx)
	{
		async Task Do()
		{
			RemoveOldChallenges();

			SocketUser challengee = ctx.User;
			if (BattleContext.GetBattleContext(challengee, out _))
			{
				return;
			}
			Challenge? c = GetChallenge(challengee);
			if (c is null)
			{
				await ctx.Channel.SendMessageAsync($"{challengee.Mention} ― You have no pending challenges.");
				return;
			}
			SocketUser challenger = c.Challenger;
			if (BattleContext.GetBattleContext(challenger, out _))
			{
				await PrintParticipating(challengee, challenger, ctx.Channel);
			}
			else
			{
				await StartBattle(c);
			}
		}
		lock (_matchmakingLockObj)
		{
			return Do();
		}
	}
	public static Task ChallengeAI(SocketCommandContext ctx)
	{
		lock (_matchmakingLockObj)
		{
			const string AIName = "*PBEAI*";
			SocketUser a = ctx.User;
			return StartBattle(ctx.Guild, a, null, a.Username, AIName, a.Mention, AIName);
		}
	}
	public static Task ChallengeUser(SocketCommandContext ctx, SocketUser challengee)
	{
		async Task Do()
		{
			RemoveOldChallenges();

			SocketUser challenger = ctx.User;
			if (challenger.Id == challengee.Id || BattleContext.GetBattleContext(challenger, out _))
			{
				return;
			}
			if (BattleContext.GetBattleContext(challengee, out _))
			{
				await PrintParticipating(challenger, challengee, ctx.Channel);
				return;
			}
			Challenge? c = GetChallenge(challenger);
			if (c is not null && c.Challenger.Id == challengee.Id)
			{
				await StartBattle(c);
				return;
			}
			c = GetChallenge(challengee);
			if (c is not null)
			{
				await ctx.Channel.SendMessageAsync($"{challenger.Mention} ― {challengee.Username} already has a pending challenge.");
				return;
			}
			string msg = $"You were challenged to a Pokémon battle by {challenger.Mention}!\nThe challenge expires in {ChallengeMinuteExpiration} minutes.\nType `!accept` to accept the challenge.";
			try
			{
				await challengee.SendMessageAsync(msg);
			}
			catch (Discord.Net.HttpException ex)
			{
				if (ex.DiscordCode == 50007)
				{
					await ctx.Channel.SendMessageAsync($"{challenger.Mention} ― Cannot challenge {challengee.Mention} because their DMs are closed.");
				}
				Console.WriteLine("Challenge exception:{0}{1}", Environment.NewLine, ex);
				return;
			}
			_challenges.Add(new Challenge(challenger, challengee, ctx.Guild));
			await ctx.Channel.SendMessageAsync($"{challenger.Mention} ― Your challenge has been sent to {challengee.Username}.");
		}
		lock (_matchmakingLockObj)
		{
			return Do();
		}
	}
	public static async Task Forfeit(SocketCommandContext ctx)
	{
		SocketUser sucker = ctx.User;
		if (BattleContext.GetBattleContext(sucker, out BattleContext? bc))
		{
			await bc.Forfeit(sucker);
		}
	}
}
