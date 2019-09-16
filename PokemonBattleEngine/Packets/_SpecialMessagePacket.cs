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
                    par.Add(((PBEPokemon)parameters[0]).FieldPosition);
                    par.Add(((PBEPokemon)parameters[0]).Team);
                    bytes.Add((byte)((PBEPokemon)parameters[0]).FieldPosition);
                    bytes.Add(((PBEPokemon)parameters[0]).Team.Id);
                    break;
                }
                case PBESpecialMessage.Magnitude:
                {
                    par.Add(parameters[0]);
                    bytes.Add((byte)parameters[0]);
                    break;
                }
                case PBESpecialMessage.PainSplit:
                {
                    par.Add(((PBEPokemon)parameters[0]).FieldPosition);
                    par.Add(((PBEPokemon)parameters[0]).Team);
                    par.Add(((PBEPokemon)parameters[1]).FieldPosition);
                    par.Add(((PBEPokemon)parameters[1]).Team);
                    bytes.Add((byte)((PBEPokemon)parameters[0]).FieldPosition);
                    bytes.Add(((PBEPokemon)parameters[0]).Team.Id);
                    bytes.Add((byte)((PBEPokemon)parameters[1]).FieldPosition);
                    bytes.Add(((PBEPokemon)parameters[1]).Team.Id);
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
