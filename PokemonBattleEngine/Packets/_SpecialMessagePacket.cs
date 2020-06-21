using Kermalis.EndianBinaryIO;
using Kermalis.PokemonBattleEngine.Battle;
using Kermalis.PokemonBattleEngine.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;

namespace Kermalis.PokemonBattleEngine.Packets
{
    public sealed class PBESpecialMessagePacket : IPBEPacket
    {
        public const ushort Code = 0x20;
        public ReadOnlyCollection<byte> Data { get; }

        public PBESpecialMessage Message { get; }
        public ReadOnlyCollection<object> Params { get; }

        internal PBESpecialMessagePacket(PBESpecialMessage message, params object[] parameters)
        {
            using (var ms = new MemoryStream())
            using (var w = new EndianBinaryWriter(ms, encoding: EncodingType.UTF16))
            {
                w.Write(Code);
                w.Write(Message = message);
                var par = new List<object>();
                switch (Message)
                {
                    case PBESpecialMessage.DraggedOut:
                    case PBESpecialMessage.Endure:
                    case PBESpecialMessage.HPDrained:
                    case PBESpecialMessage.Recoil:
                    case PBESpecialMessage.Struggle:
                    {
                        var p = (PBEBattlePokemon)parameters[0];
                        par.Add(p.FieldPosition);
                        par.Add(p.Team);
                        w.Write(p.FieldPosition);
                        w.Write(p.Team.Id);
                        break;
                    }
                    case PBESpecialMessage.Magnitude:
                    case PBESpecialMessage.MultiHit:
                    {
                        byte p = (byte)parameters[0];
                        par.Add(p);
                        w.Write(p);
                        break;
                    }
                    case PBESpecialMessage.PainSplit:
                    {
                        var p0 = (PBEBattlePokemon)parameters[0];
                        var p1 = (PBEBattlePokemon)parameters[1];
                        par.Add(p0.FieldPosition);
                        par.Add(p0.Team);
                        par.Add(p1.FieldPosition);
                        par.Add(p1.Team);
                        w.Write(p0.FieldPosition);
                        w.Write(p0.Team.Id);
                        w.Write(p1.FieldPosition);
                        w.Write(p1.Team.Id);
                        break;
                    }
                }
                Params = new ReadOnlyCollection<object>(par);
                Data = new ReadOnlyCollection<byte>(ms.ToArray());
            }
        }
        internal PBESpecialMessagePacket(byte[] data, EndianBinaryReader r, PBEBattle battle)
        {
            Data = new ReadOnlyCollection<byte>(data);
            Message = r.ReadEnum<PBESpecialMessage>();
            switch (Message)
            {
                case PBESpecialMessage.DraggedOut:
                case PBESpecialMessage.Endure:
                case PBESpecialMessage.HPDrained:
                case PBESpecialMessage.Recoil:
                case PBESpecialMessage.Struggle:
                {
                    Params = new ReadOnlyCollection<object>(new object[] { r.ReadEnum<PBEFieldPosition>(), battle.Teams[r.ReadByte()] });
                    break;
                }
                case PBESpecialMessage.Magnitude:
                case PBESpecialMessage.MultiHit:
                {
                    Params = new ReadOnlyCollection<object>(new object[] { r.ReadByte() });
                    break;
                }
                case PBESpecialMessage.NothingHappened:
                case PBESpecialMessage.OneHitKnockout:
                case PBESpecialMessage.PayDay:
                {
                    Params = new ReadOnlyCollection<object>(Array.Empty<object>());
                    break;
                }
                case PBESpecialMessage.PainSplit:
                {
                    Params = new ReadOnlyCollection<object>(new object[] { r.ReadEnum<PBEFieldPosition>(), battle.Teams[r.ReadByte()], r.ReadEnum<PBEFieldPosition>(), battle.Teams[r.ReadByte()] });
                    break;
                }
                throw new InvalidDataException();
            }
        }
    }
}
