using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kermalis.PokemonBattleEngineDiscord
{
    internal static class ReactionListener
    {
        private class Listener
        {
            public readonly BattleContext BattleContext;
            public readonly IUserMessage Message;
            public readonly IEnumerable<IUserMessage> MessagesToRemoveListenersForOnViableClick;
            public readonly IEmote Emote;
            public readonly IEnumerable<IUser> UsersThatWillRunClickFunc;
            public readonly Func<Task> ClickFunc;

            public Listener(BattleContext ctx, IUserMessage msg, IEnumerable<IUserMessage> messages, IEmote emote, IEnumerable<IUser> users, Func<Task> clickFunc)
            {
                BattleContext = ctx;
                Message = msg;
                MessagesToRemoveListenersForOnViableClick = messages;
                Emote = emote;
                UsersThatWillRunClickFunc = users;
                ClickFunc = clickFunc;
            }
        }

        private static readonly object _reactionListenersLockObj = new object();
        private static readonly List<Listener> _reactionListeners = new List<Listener>();

        public static Task Client_ReactionAdded(Cacheable<IUserMessage, ulong> message, ISocketMessageChannel channel, SocketReaction reaction)
        {
            lock (_reactionListenersLockObj)
            {
                foreach (Listener listener in _reactionListeners)
                {
                    if (listener.Message.Id == reaction.MessageId && reaction.Emote.Equals(listener.Emote) && listener.UsersThatWillRunClickFunc.Any(u => u.Id == reaction.UserId))
                    {
                        foreach (IUserMessage removing in listener.MessagesToRemoveListenersForOnViableClick)
                        {
                            _reactionListeners.RemoveAll(l => l.Message.Id == removing.Id);
                        }
                        listener.ClickFunc.Invoke();
                        break;
                    }
                }
            }
            return Task.CompletedTask;
        }

        public static void AddListener(BattleContext ctx, IUserMessage msg, IEnumerable<IUserMessage> messagesToRemoveListenersForOnViableClick, IEmote emote, IEnumerable<IUser> usersThatWillRunClickFunc, Func<Task> clickFunc)
        {
            lock (_reactionListenersLockObj)
            {
                _reactionListeners.Add(new Listener(ctx, msg, messagesToRemoveListenersForOnViableClick, emote, usersThatWillRunClickFunc, clickFunc));
            }
        }
        public static void RemoveListeners(BattleContext ctx)
        {
            lock (_reactionListenersLockObj)
            {
                _reactionListeners.RemoveAll(l => l.BattleContext == ctx);
            }
        }
    }
}
