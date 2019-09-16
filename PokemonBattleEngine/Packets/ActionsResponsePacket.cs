using Ether.Network.Packets;
using Kermalis.PokemonBattleEngine.Battle;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

namespace Kermalis.PokemonBattleEngine.Packets
{
    public sealed class PBEActionsResponsePacket : INetPacket
    {
        public const short Code = 0x08;
        public ReadOnlyCollection<byte> Buffer { get; }

        public ReadOnlyCollection<PBETurnAction> Actions { get; }

        public PBEActionsResponsePacket(IList<PBETurnAction> actions)
        {
            if (actions == null)
            {
                throw new ArgumentNullException(nameof(actions));
            }
            if (actions.Count == 0)
            {
                throw new ArgumentOutOfRangeException(nameof(actions));
            }
            if (actions.Any(a => a == null))
            {
                throw new ArgumentNullException(nameof(actions));
            }
            var bytes = new List<byte>();
            bytes.AddRange(BitConverter.GetBytes(Code));
            bytes.Add((byte)(Actions = new ReadOnlyCollection<PBETurnAction>(actions)).Count);
            for (int i = 0; i < (byte)Actions.Count; i++)
            {
                Actions[i].ToBytes(bytes);
            }
            bytes.InsertRange(0, BitConverter.GetBytes((short)bytes.Count));
            Buffer = new ReadOnlyCollection<byte>(bytes);
        }
        internal PBEActionsResponsePacket(ReadOnlyCollection<byte> buffer, BinaryReader r, PBEBattle battle)
        {
            Buffer = buffer;
            var actions = new PBETurnAction[r.ReadByte()];
            for (int i = 0; i < actions.Length; i++)
            {
                actions[i] = new PBETurnAction(r);
            }
            Actions = new ReadOnlyCollection<PBETurnAction>(actions);
        }

        public void Dispose() { }
    }
}
