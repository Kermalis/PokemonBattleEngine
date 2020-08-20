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
                        var p0 = (PBEBattlePokemon)parameters[0];
                        par.Add(p0.Trainer);
                        par.Add(p0.FieldPosition);
                        w.Write(p0.Trainer.Id);
                        w.Write(p0.FieldPosition);
                        break;
                    }
                    case PBESpecialMessage.Magnitude:
                    case PBESpecialMessage.MultiHit:
                    {
                        byte p0 = (byte)parameters[0];
                        par.Add(p0);
                        w.Write(p0);
                        break;
                    }
                    case PBESpecialMessage.PainSplit:
                    {
                        var p0 = (PBEBattlePokemon)parameters[0];
                        var p1 = (PBEBattlePokemon)parameters[1];
                        par.Add(p0.Trainer);
                        par.Add(p0.FieldPosition);
                        par.Add(p1.Trainer);
                        par.Add(p1.FieldPosition);
                        w.Write(p1.Trainer.Id);
                        w.Write(p1.FieldPosition);
                        w.Write(p1.Trainer.Id);
                        w.Write(p1.FieldPosition);
                        break;
                    }
                }
                Params = new ReadOnlyCollection<object>(par);
                Data = new ReadOnlyCollection<byte>(ms.GetBuffer());
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
                    Params = new ReadOnlyCollection<object>(new object[] { battle.Trainers[r.ReadByte()], r.ReadEnum<PBEFieldPosition>() });
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
                    Params = new ReadOnlyCollection<object>(new object[] { battle.Trainers[r.ReadByte()], r.ReadEnum<PBEFieldPosition>(), battle.Trainers[r.ReadByte()], r.ReadEnum<PBEFieldPosition>() });
                    break;
                }
                throw new InvalidDataException();
            }
        }
    }
}
