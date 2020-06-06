using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Kermalis.PokemonBattleEngineDiscord
{
    internal sealed class ChannelHandler
    {
        private const int NumMinutesUntilChannelDeleted = 60; // 60 is fine for the small userbase now, 15 is probably the target later (add customization)
        private const string CategoryName = "Pokémon Battles";
        private static readonly object _channelHandlerLockObj = new object();
        private static readonly Dictionary<IGuild, ICategoryChannel> _categoryLookup = new Dictionary<IGuild, ICategoryChannel>(DiscordComparers.GuildComparer);
        private static readonly Dictionary<ITextChannel, (DateTime, IUserMessage)> _channelDeletion = new Dictionary<ITextChannel, (DateTime, IUserMessage)>(DiscordComparers.ChannelComparer);
        private static Timer _channelDeletionTimer;

        public static Task OnChannelDeleted(SocketChannel channel)
        {
            async Task Do()
            {
                if (channel is ITextChannel tc)
                {
                    _channelDeletion.Remove(tc);
                }
                else if (channel is ICategoryChannel gc)
                {
                    IGuild guild = gc.Guild;
                    if (_categoryLookup.TryGetValue(guild, out ICategoryChannel c) && c.Id == gc.Id)
                    {
                        _categoryLookup.Remove(guild);
                        await CreateCategory(gc.Guild);
                    }
                }
            }
            lock (_channelHandlerLockObj)
            {
                return Do();
            }
        }
        public static void OnLeftGuild(SocketGuild guild)
        {
            lock (_channelHandlerLockObj)
            {
                _categoryLookup.Remove(guild);
            }
        }
        public static void OnConnected()
        {
            _channelDeletionTimer = new Timer(CheckChannelsForDeletion, null, 0, 1000 * 60);
        }
        public static void OnDisconnected()
        {
            _channelDeletionTimer.Dispose();
        }

        private static Task<ICategoryChannel> CreateCategory(IGuild guild)
        {
            return guild.CreateCategoryAsync(CategoryName);
        }
        public static async Task<ITextChannel> CreateChannel(IGuild guild, string name)
        {
            if (!_categoryLookup.TryGetValue(guild, out ICategoryChannel category))
            {
                IReadOnlyCollection<ICategoryChannel> all = await guild.GetCategoriesAsync();
                bool found = false;
                foreach (ICategoryChannel c in all)
                {
                    if (c.Name == CategoryName)
                    {
                        category = c;
                        _categoryLookup.Add(guild, c);
                        found = true;
                        break;
                    }
                }
                if (!found)
                {
                    category = await CreateCategory(guild);
                    _categoryLookup.Add(guild, category);
                }
            }
            return await guild.CreateTextChannelAsync(name, func: p => p.CategoryId = category.Id);
        }

        private static Embed GetChannelDeletionEmbed(int num)
        {
            string str = string.Format("This channel will be deleted in {0} minute{1}.\nTo prevent this from happening, have a moderator use `!dontdelete`.", num, num == 1 ? string.Empty : "s");
            return new EmbedBuilder()
                .WithUrl(Utils.URL)
                .WithDescription(str)
                .WithColor(Utils.RandomColor())
                .Build();
        }
        private static void CheckChannelsForDeletion(object state)
        {
            lock (_channelHandlerLockObj)
            {
                CheckChannelsForDeletion().GetAwaiter().GetResult();
            }
        }
        private static async Task CheckChannelsForDeletion()
        {
            DateTime dt = DateTime.Now;
            var toRemove = new List<ITextChannel>();
            foreach (KeyValuePair<ITextChannel, (DateTime, IUserMessage)> kvp in _channelDeletion)
            {
                ITextChannel chan = kvp.Key;
                (DateTime endTime, IUserMessage msg) = kvp.Value;
                int mins = (endTime - dt).Minutes;
                if (mins <= 0)
                {
                    toRemove.Add(chan);
                }
                else
                {
                    await msg.ModifyAsync(p => p.Embed = GetChannelDeletionEmbed(mins));
                }
            }
            foreach (ITextChannel chan in toRemove)
            {
                _channelDeletion.Remove(chan);
                await chan.DeleteAsync();
            }
        }
        public static Task DontDelete(SocketCommandContext context)
        {
            if (context.Channel is ITextChannel c)
            {
                async Task Do()
                {
                    if (_channelDeletion.TryGetValue(c, out (DateTime, IUserMessage) tup))
                    {
                        await tup.Item2.DeleteAsync();
                        await c.SendMessageAsync($"{context.User.Mention} ― This channel will no longer be deleted.");
                    }
                }
                lock (_channelHandlerLockObj)
                {
                    return Do();
                }
            }
            return Task.CompletedTask;
        }
        public static Task MarkChannelForDeletion(ITextChannel channel)
        {
            async Task Do()
            {
                IUserMessage msg = await channel.SendMessageAsync(embed: GetChannelDeletionEmbed(NumMinutesUntilChannelDeleted));
                _channelDeletion.Add(channel, (DateTime.Now.AddMinutes(NumMinutesUntilChannelDeleted), msg));
            }
            lock (_channelHandlerLockObj)
            {
                return Do();
            }
        }
    }
}
