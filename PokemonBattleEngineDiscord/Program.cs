using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Kermalis.PokemonBattleEngine;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace Kermalis.PokemonBattleEngineDiscord
{
    internal sealed class Program
    {
        private const char CommandPrefix = '!';
        private DiscordSocketClient _client;
        private IServiceProvider _services;
        private CommandService _commands;

        public static void Main(string[] args)
        {
            new Program().MainAsync(args).GetAwaiter().GetResult();
        }
        private async Task MainAsync(string[] args)
        {
            PBEUtils.CreateDatabaseConnection(string.Empty);

            _client = new DiscordSocketClient(new DiscordSocketConfig { WebSocketProvider = Discord.Net.Providers.WS4Net.WS4NetProvider.Instance });

            _commands = new CommandService(new CommandServiceConfig { DefaultRunMode = RunMode.Async });
            _services = new ServiceCollection()
                .AddSingleton(_client)
                .AddSingleton(_commands)
                .BuildServiceProvider();
            await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _services);

            _client.Log += LogMessage;
            _client.MessageReceived += CommandMessageReceived;
            _client.ReactionAdded += ReactionListener.Client_ReactionAdded;

            await _client.LoginAsync(TokenType.Bot, args[0]); // Token is passed in as args[0]
            await _client.StartAsync();

            await Task.Delay(-1);
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
            IResult result = await _commands.ExecuteAsync(context, argPos, _services);
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
