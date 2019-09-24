using Ether.Network.Packets;
using Kermalis.PokemonBattleEngine.Battle;
using Kermalis.PokemonBattleEngine.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;

namespace Kermalis.PokemonBattleEngine.Packets
{
    public sealed class PBESpecialMessagePacket : INetPacket
    {
        public const short Code = 0x20;
        public ReadOnlyCollection<byte> Buffer { get; }

        public PBESpecialMessage Message { get; }
        public ReadOnlyCollection<object> Params { get; }

        internal PBESpecialMessagePacket(PBESpecialMessage message, params object[] parameters)
        {
            var bytes = new List<byte>();
            bytes.AddRange(BitConverter.GetBytes(Code));
            bytes.Add((byte)(Message = message));
            var par = new List<object>();
            switch (Message)
            {
                case PBESpecialMessage.DraggedOut:
                case PBESpecialMessage.Endure:
                case PBESpecialMessage.HPDrained:
                case PBESpecialMessage.Recoil:
                case PBESpecialMessage.Struggle:
                {
                    var p = (PBEPokemon)parameters[0];
                    par.Add(p.FieldPosition);
                    par.Add(p.Team);
                    bytes.Add((byte)p.FieldPosition);
                    bytes.Add(p.Team.Id);
                    break;
                }
                case PBESpecialMessage.Magnitude:
                {
                    byte p = (byte)parameters[0];
                    par.Add(p);
                    bytes.Add(p);
                    break;
                }
                case PBESpecialMessage.PainSplit:
                {
                    var p0 = (PBEPokemon)parameters[0];
                    var p1 = (PBEPokemon)parameters[1];
                    par.Add(p0.FieldPosition);
                    par.Add(p0.Team);
                    par.Add(p1.FieldPosition);
                    par.Add(p1.Team);
                    bytes.Add((byte)p0.FieldPosition);
                    bytes.Add(p0.Team.Id);
                    bytes.Add((byte)p1.FieldPosition);
                    bytes.Add(p1.Team.Id);
                    break;
                }
            }
            Params = new ReadOnlyCollection<object>(par);
            bytes.InsertRange(0, BitConverter.GetBytes((short)bytes.Count));
            Buffer = new ReadOnlyCollection<byte>(bytes);
        }
        internal PBESpecialMessagePacket(ReadOnlyCollection<byte> buffer, BinaryReader r, PBEBattle battle)
        {
            Buffer = buffer;
            Message = (PBESpecialMessage)r.ReadByte();
            switch (Message)
            {
                case PBESpecialMessage.DraggedOut:
                case PBESpecialMessage.Endure:
                case PBESpecialMessage.HPDrained:
                case PBESpecialMessage.Recoil:
                case PBESpecialMessage.Struggle:
                {
                    Params = new ReadOnlyCollection<object>(new object[] { (PBEFieldPosition)r.ReadByte(), battle.Teams[r.ReadByte()] });
                    break;
                }
                case PBESpecialMessage.Magnitude:
                {
                    Params = new ReadOnlyCollection<object>(new object[] { r.ReadByte() });
                    break;
                }
                case PBESpecialMessage.OneHitKnockout:
                {
                    Params = new ReadOnlyCollection<object>(Array.Empty<object>());
                    break;
                }
                case PBESpecialMessage.PainSplit:
                {
                    Params = new ReadOnlyCollection<object>(new object[] { (PBEFieldPosition)r.ReadByte(), battle.Teams[r.ReadByte()], (PBEFieldPosition)r.ReadByte(), battle.Teams[r.ReadByte()] });
                    break;
                }
                throw new InvalidDataException();
            }
        }

        public void Dispose() { }
    }
}
