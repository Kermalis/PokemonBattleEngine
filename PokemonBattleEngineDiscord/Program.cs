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
    internal class Program
    {
        private DiscordSocketClient client;
        private IServiceProvider services;
        private CommandService commands;
        private const char commandPrefix = '!';

        public static void Main(string[] args)
        {
            new Program().MainAsync(args).GetAwaiter().GetResult();
        }
        private async Task MainAsync(string[] args)
        {
            PBEUtils.CreateDatabaseConnection(string.Empty);

            client = new DiscordSocketClient(new DiscordSocketConfig { WebSocketProvider = Discord.Net.Providers.WS4Net.WS4NetProvider.Instance });

            commands = new CommandService(new CommandServiceConfig { DefaultRunMode = RunMode.Async });
            services = new ServiceCollection()
                .AddSingleton(client)
                .AddSingleton(commands)
                .BuildServiceProvider();
            await commands.AddModulesAsync(Assembly.GetEntryAssembly(), services);

            client.Log += LogMessage;
            client.MessageReceived += CommandMessageReceived;
            client.ReactionAdded += ReactionListener.Client_ReactionAdded;

            await client.LoginAsync(TokenType.Bot, args[0]); // Token is passed in as args[0]
            await client.StartAsync();

            await Task.Delay(-1);
        }

        private async Task CommandMessageReceived(SocketMessage arg)
        {
            int argPos = 0;
            if (!(arg is SocketUserMessage message)
                || message.Author.Id == client.CurrentUser.Id
                || !message.HasCharPrefix(commandPrefix, ref argPos))
            {
                return;
            }
            var context = new SocketCommandContext(client, message);
            IResult result = await commands.ExecuteAsync(context, argPos, services);
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
