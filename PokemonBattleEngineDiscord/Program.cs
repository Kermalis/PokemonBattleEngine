using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Kermalis.PokemonBattleEngine.Data;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace Kermalis.PokemonBattleEngineDiscord
{
    internal sealed class Program
    {
        private const char CommandPrefix = '!';
        private DiscordSocketClient _client;
        private CommandService _commands;

        public static void Main(string[] args)
        {
            new Program().MainAsync(args).GetAwaiter().GetResult();
        }
        private async Task MainAsync(string[] args)
        {
            Utils.InitFemaleSpriteLookup();
            ReplaySaver.RemoveOldReplays();
            PBEDataProvider.InitEngine(string.Empty);

            _client = new DiscordSocketClient();

            _commands = new CommandService(new CommandServiceConfig { DefaultRunMode = RunMode.Async });
            await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), null);

            _client.Log += LogMessage;
            _client.MessageReceived += CommandMessageReceived;
            _client.ReactionAdded += OnReactionAdded;
            _client.ChannelDestroyed += OnChannelDeleted;
            _client.LeftGuild += OnLeftGuild;
            _client.UserLeft += OnUserLeft;
            _client.Connected += OnConnected;
            _client.Disconnected += OnDisconnected;
            _client.GuildMemberUpdated += OnGuildMemberUpdated;

            await _client.LoginAsync(TokenType.Bot, args[0]); // Token is passed in as args[0]
            await _client.StartAsync();

            await Task.Delay(-1);
        }

        private Task OnReactionAdded(Cacheable<IUserMessage, ulong> message, ISocketMessageChannel channel, SocketReaction reaction)
        {
            ReactionHandler.OnReactionAdded(reaction);
            return Task.CompletedTask;
        }
        private async Task OnChannelDeleted(SocketChannel arg)
        {
            // TODO: Prevent abuse of constant deletions of our stuff
            await ChannelHandler.OnChannelDeleted(arg);
            BattleContext.OnChannelDeleted(arg);
        }
        private Task OnLeftGuild(SocketGuild arg)
        {
            Matchmaking.OnLeftGuild(arg);
            ChannelHandler.OnLeftGuild(arg);
            BattleContext.OnLeftGuild(arg);
            return Task.CompletedTask;
        }
        private async Task OnUserLeft(SocketGuildUser arg)
        {
            Matchmaking.OnUserLeft(arg);
            await BattleContext.OnUserLeft(arg);
        }
        private Task OnConnected()
        {
            ChannelHandler.OnConnected();
            return Task.CompletedTask;
        }
        private Task OnDisconnected(Exception arg)
        {
            Console.WriteLine(arg);
            ChannelHandler.OnDisconnected();
            return Task.CompletedTask;
        }
        private Task OnGuildMemberUpdated(SocketGuildUser arg1, SocketGuildUser arg2)
        {
            BattleContext.OnGuildMemberUpdated(arg2);
            return Task.CompletedTask;
        }

        private async Task CommandMessageReceived(SocketMessage arg)
        {
            int argPos = 0;
            if (!(arg is SocketUserMessage message)
                || message.Author.Id == _client.CurrentUser.Id
                || !message.HasCharPrefix(CommandPrefix, ref argPos))
            {
                return;
            }
            var context = new SocketCommandContext(_client, message);
            IResult result = await _commands.ExecuteAsync(context, argPos, null);
            if (!result.IsSuccess)
            {
                Console.WriteLine(result.ErrorReason);
            }
        }
        private Task LogMessage(LogMessage arg)
        {
            Console.WriteLine(arg.Message);
            return Task.CompletedTask;
        }
    }
}
