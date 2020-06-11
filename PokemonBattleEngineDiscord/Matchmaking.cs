using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Kermalis.PokemonBattleEngine.Battle;
using Kermalis.PokemonBattleEngine.Data;
using Kermalis.PokemonBattleEngine.Utils;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kermalis.PokemonBattleEngineDiscord
{
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
        private static readonly object _matchmakingLockObj = new object();
        private static readonly List<Challenge> _challenges = new List<Challenge>();

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
        private static Challenge GetChallenge(SocketUser challengee)
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
        private static async Task StartBattle(IGuild guild, SocketUser battler1, SocketUser battler2, string team1Name, string team2Name, string team1Mention, string team2Mention)
        {
            PBETeamShell team1Shell, team2Shell;
            // Competitively Randomized Pokémon
            team1Shell = PBERandomTeamGenerator.CreateRandomTeam(3);
            team2Shell = PBERandomTeamGenerator.CreateRandomTeam(3);

            var battle = new PBEBattle(PBEBattleTerrain.Plain, PBEBattleFormat.Single, team1Shell, team1Name, team2Shell, team2Name);
            team1Shell.Dispose();
            team2Shell.Dispose();

            var bc = new BattleContext(battle, battler1, battler2);
            ITextChannel channel = await ChannelHandler.CreateChannel(guild, $"battle-{bc.BattleId}");
            await channel.SendMessageAsync($"**Battle #{bc.BattleId} ― {team1Mention} vs {team2Mention}**");
            await bc.Begin(channel);
        }

        public static Task AcceptChallenge(SocketCommandContext ctx)
        {
            async Task Do()
            {
                RemoveOldChallenges();

                SocketUser challengee = ctx.User;
                if (BattleContext.GetBattleContext(challengee) != null)
                {
                    return;
                }
                else
                {
                    Challenge c = GetChallenge(challengee);
                    if (c == null)
                    {
                        await ctx.Channel.SendMessageAsync($"{challengee.Mention} ― You have no pending challenges.");
                    }
                    else
                    {
                        SocketUser challenger = c.Challenger;
                        if (BattleContext.GetBattleContext(challenger) != null)
                        {
                            await PrintParticipating(challengee, challenger, ctx.Channel);
                        }
                        else
                        {
                            await StartBattle(c);
                        }
                    }
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
                if (challenger.Id == challengee.Id || BattleContext.GetBattleContext(challenger) != null)
                {
                    return;
                }
                else if (BattleContext.GetBattleContext(challengee) != null)
                {
                    await PrintParticipating(challenger, challengee, ctx.Channel);
                }
                else
                {
                    Challenge c = GetChallenge(challenger);
                    if (c != null && c.Challenger.Id == challengee.Id)
                    {
                        await StartBattle(c);
                    }
                    else
                    {
                        c = GetChallenge(challengee);
                        if (c == null)
                        {
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
                        else
                        {
                            await ctx.Channel.SendMessageAsync($"{challenger.Mention} ― {challengee.Username} already has a pending challenge.");
                        }
                    }
                }
            }
            lock (_matchmakingLockObj)
            {
                return Do();
            }
        }
        public static async Task Forfeit(SocketCommandContext ctx)
        {
            SocketUser sucker = ctx.User;
            var bc = BattleContext.GetBattleContext(sucker);
            if (bc != null)
            {
                await bc.Forfeit(sucker);
            }
        }
    }
}
