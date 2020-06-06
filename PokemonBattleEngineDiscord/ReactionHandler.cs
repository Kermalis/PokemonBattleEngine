using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kermalis.PokemonBattleEngineDiscord
{
    internal static class ReactionHandler
    {
        private class Reaction
        {
            public IUserMessage Message { get; }
            public IEmote Emote { get; }
            public Func<Task> ClickFunc { get; }

            public Reaction(IUserMessage msg, IEmote emote, Func<Task> clickFunc)
            {
                Message = msg;
                Emote = emote;
                ClickFunc = clickFunc;
            }
        }

        private static readonly object _reactionListenersLockObj = new object();
        private static readonly Dictionary<IUser, List<Reaction>> _reactionListeners = new Dictionary<IUser, List<Reaction>>(DiscordComparers.UserComparer);

        public static void OnReactionAdded(SocketReaction inEvent)
        {
            lock (_reactionListenersLockObj)
            {
                IUser user = inEvent.User.Value;
                if (_reactionListeners.TryGetValue(user, out List<Reaction> list))
                {
                    ulong msg = inEvent.MessageId;
                    IEmote emote = inEvent.Emote;
                    foreach (Reaction r in list)
                    {
                        if (r.Message.Id == msg && emote.Equals(r.Emote))
                        {
                            _reactionListeners.Remove(user);
                            r.ClickFunc.Invoke(); // Do not return Task because we do not want to block the main thread
                            return;
                        }
                    }
                }
            }
        }

        public static void AddListener(IUser user, IUserMessage msg, IEmote emote, Func<Task> clickFunc)
        {
            lock (_reactionListenersLockObj)
            {
                if (!_reactionListeners.TryGetValue(user, out List<Reaction> list))
                {
                    list = new List<Reaction>();
                    _reactionListeners.Add(user, list);
                }
                list.Add(new Reaction(msg, emote, clickFunc));
            }
        }
        public static void RemoveListeners(IUser a, IUser b)
        {
            lock (_reactionListenersLockObj)
            {
                if (a != null)
                {
                    _reactionListeners.Remove(a);
                }
                if (b != null)
                {
                    _reactionListeners.Remove(b);
                }
            }
        }
    }
}
