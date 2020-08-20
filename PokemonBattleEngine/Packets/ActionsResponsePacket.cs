using Kermalis.EndianBinaryIO;
using Kermalis.PokemonBattleEngine.Battle;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

namespace Kermalis.PokemonBattleEngine.Packets
{
    public sealed class PBEActionsResponsePacket : IPBEPacket
    {
        public const ushort Code = 0x08;
        public ReadOnlyCollection<byte> Data { get; }

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
            using (var ms = new MemoryStream())
            using (var w = new EndianBinaryWriter(ms, encoding: EncodingType.UTF16))
            {
                w.Write(Code);
                byte count = (byte)(Actions = new ReadOnlyCollection<PBETurnAction>(actions)).Count;
                w.Write(count);
                for (int i = 0; i < count; i++)
                {
                    Actions[i].ToBytes(w);
                }
                Data = new ReadOnlyCollection<byte>(ms.GetBuffer());
            }
        }
        internal PBEActionsResponsePacket(byte[] data, EndianBinaryReader r)
        {
            Data = new ReadOnlyCollection<byte>(data);
            var actions = new PBETurnAction[r.ReadByte()];
            for (int i = 0; i < actions.Length; i++)
            {
                actions[i] = new PBETurnAction(r);
            }
            Actions = new ReadOnlyCollection<PBETurnAction>(actions);
        }
    }
}
