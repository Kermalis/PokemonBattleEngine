using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kermalis.PokemonBattleEngineDiscord
{
    public static class ReactionListener
    {
        private class Listener
        {
            public IUserMessage Message;
            public IEnumerable<IUserMessage> MessagesToRemoveListenersForOnViableClick;
            public IEmote Emote;
            public IEnumerable<IUser> UsersThatWillRunClickFunc;
            public Func<Task> ClickFunc;
        }

        private static readonly List<Listener> reactionListeners = new List<Listener>();

        public static Task Client_ReactionAdded(Cacheable<IUserMessage, ulong> message, ISocketMessageChannel channel, SocketReaction reaction)
        {
            lock (reactionListeners)
            {
                Listener listener = reactionListeners.SingleOrDefault(l => l.Message.Id == reaction.MessageId && reaction.Emote.Equals(l.Emote) && l.UsersThatWillRunClickFunc.Any(u => u.Id == reaction.UserId));
                if (listener != null)
                {
                    foreach (IUserMessage removing in listener.MessagesToRemoveListenersForOnViableClick)
                    {
                        reactionListeners.RemoveAll(l => l.Message.Id == removing.Id);
                    }
                    listener.ClickFunc.Invoke();
                }
            }
            return Task.CompletedTask;
        }

        public static void AddListener(IUserMessage msg, IEnumerable<IUserMessage> messagesToRemoveListenersForOnViableClick, IEmote emote, IEnumerable<IUser> usersThatWillRunClickFunc, Func<Task> clickFunc)
        {
            lock (reactionListeners)
            {
                reactionListeners.Add(new Listener
                {
                    Message = msg,
                    MessagesToRemoveListenersForOnViableClick = messagesToRemoveListenersForOnViableClick,
                    Emote = emote,
                    UsersThatWillRunClickFunc = usersThatWillRunClickFunc,
                    ClickFunc = clickFunc
                });
            }
        }
    }
}
