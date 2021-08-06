using Kermalis.EndianBinaryIO;
using Kermalis.PokemonBattleEngine.Battle;
using Kermalis.PokemonBattleEngine.Data;
using System;
using System.Collections.ObjectModel;
using System.IO;

namespace Kermalis.PokemonBattleEngine.Packets
{
    public sealed class PBEReflectTypePacket : IPBEPacket
    {
        public const ushort Code = 0x2E;
        public ReadOnlyCollection<byte> Data { get; }

        public PBETrainer UserTrainer { get; }
        public PBEFieldPosition User { get; }
        public PBETrainer TargetTrainer { get; }
        public PBEFieldPosition Target { get; }
        public PBEType Type1 { get; }
        public PBEType Type2 { get; }

        internal PBEReflectTypePacket(PBEBattlePokemon user, PBEBattlePokemon target)
        {
            using (var ms = new MemoryStream())
            using (var w = new EndianBinaryWriter(ms, encoding: EncodingType.UTF16))
            {
                w.Write(Code);
                w.Write((UserTrainer = user.Trainer).Id);
                w.Write(User = user.FieldPosition);
                w.Write((TargetTrainer = target.Trainer).Id);
                w.Write(Target = target.FieldPosition);
                w.Write(Type1 = target.Type1);
                w.Write(Type2 = target.Type2);
                Data = new ReadOnlyCollection<byte>(ms.ToArray());
            }
        }
        internal PBEReflectTypePacket(byte[] data, EndianBinaryReader r, PBEBattle battle)
        {
            Data = new ReadOnlyCollection<byte>(data);
            UserTrainer = battle.Trainers[r.ReadByte()];
            User = r.ReadEnum<PBEFieldPosition>();
            TargetTrainer = battle.Trainers[r.ReadByte()];
            Target = r.ReadEnum<PBEFieldPosition>();
            Type1 = r.ReadEnum<PBEType>();
            Type2 = r.ReadEnum<PBEType>();
        }
    }
    public sealed class PBEReflectTypePacket_Hidden : IPBEPacket
    {
        public const ushort Code = 0x33;
        public ReadOnlyCollection<byte> Data { get; }

        public PBETrainer UserTrainer { get; }
        public PBEFieldPosition User { get; }
        public PBETrainer TargetTrainer { get; }
        public PBEFieldPosition Target { get; }

        public PBEReflectTypePacket_Hidden(PBEReflectTypePacket other)
        {
            using (var ms = new MemoryStream())
            using (var w = new EndianBinaryWriter(ms, encoding: EncodingType.UTF16))
            {
                w.Write(Code);
                w.Write((UserTrainer = other.UserTrainer).Id);
                w.Write(User = other.User);
                w.Write((TargetTrainer = other.TargetTrainer).Id);
                w.Write(Target = other.Target);
                Data = new ReadOnlyCollection<byte>(ms.ToArray());
            }
        }
        internal PBEReflectTypePacket_Hidden(byte[] data, EndianBinaryReader r, PBEBattle battle)
        {
            Data = new ReadOnlyCollection<byte>(data);
            UserTrainer = battle.Trainers[r.ReadByte()];
            User = r.ReadEnum<PBEFieldPosition>();
            TargetTrainer = battle.Trainers[r.ReadByte()];
            Target = r.ReadEnum<PBEFieldPosition>();
        }
    }
}
