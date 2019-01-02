using Ether.Network.Packets;
using Kermalis.PokemonBattleEngine.Battle;
using Kermalis.PokemonBattleEngine.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

namespace Kermalis.PokemonBattleEngine.Packets
{
    public sealed class PBESpecialMessagePacket : INetPacket
    {
        public const short Code = 0x20;
        public IEnumerable<byte> Buffer { get; }

        public PBESpecialMessage Message { get; }
        public ReadOnlyCollection<object> Params { get; }

        public PBESpecialMessagePacket(PBESpecialMessage message, params object[] parameters)
        {
            var bytes = new List<byte>();
            bytes.AddRange(BitConverter.GetBytes(Code));
            bytes.Add((byte)(Message = message));
            Params = Array.AsReadOnly(parameters);
            switch (Message)
            {
                case PBESpecialMessage.Magnitude:
                case PBESpecialMessage.Recoil:
                case PBESpecialMessage.Struggle:
                    bytes.Add((byte)Params[0]);
                    break;
                case PBESpecialMessage.PainSplit:
                    bytes.Add((byte)Params[0]);
                    bytes.Add((byte)Params[1]);
                    break;
            }
            Buffer = BitConverter.GetBytes((short)bytes.Count).Concat(bytes);
        }
        public PBESpecialMessagePacket(byte[] buffer, PBEBattle battle)
        {
            Buffer = buffer;
            using (var r = new BinaryReader(new MemoryStream(buffer)))
            {
                r.ReadInt16(); // Skip Code
                Message = (PBESpecialMessage)r.ReadByte();
                switch (Message)
                {
                    case PBESpecialMessage.Magnitude:
                    case PBESpecialMessage.Recoil:
                    case PBESpecialMessage.Struggle:
                        Params = Array.AsReadOnly(new object[] { r.ReadByte() });
                        break;
                    case PBESpecialMessage.PainSplit:
                        Params = Array.AsReadOnly(new object[] { r.ReadByte(), r.ReadByte() });
                        break;
                }
            }
        }

        public void Dispose() { }
    }
}
