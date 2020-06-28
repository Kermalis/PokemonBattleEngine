using Kermalis.EndianBinaryIO;
using Kermalis.PokemonBattleEngine.Battle;
using Kermalis.PokemonBattleEngine.Data;
using System.Collections.ObjectModel;
using System.IO;

namespace Kermalis.PokemonBattleEngine.Packets
{
    public sealed class PBEPsychUpPacket : IPBEPacket
    {
        public const ushort Code = 0x22;
        public ReadOnlyCollection<byte> Data { get; }

        public PBETrainer UserTrainer { get; }
        public PBEFieldPosition User { get; }
        public PBETrainer TargetTrainer { get; }
        public PBEFieldPosition Target { get; }
        public sbyte AttackChange { get; }
        public sbyte DefenseChange { get; }
        public sbyte SpAttackChange { get; }
        public sbyte SpDefenseChange { get; }
        public sbyte SpeedChange { get; }
        public sbyte AccuracyChange { get; }
        public sbyte EvasionChange { get; }

        internal PBEPsychUpPacket(PBEBattlePokemon user, PBEBattlePokemon target)
        {
            using (var ms = new MemoryStream())
            using (var w = new EndianBinaryWriter(ms, encoding: EncodingType.UTF16))
            {
                w.Write(Code);
                w.Write((UserTrainer = user.Trainer).Id);
                w.Write(User = user.FieldPosition);
                w.Write((TargetTrainer = target.Trainer).Id);
                w.Write(Target = target.FieldPosition);
                w.Write(AttackChange = target.AttackChange);
                w.Write(DefenseChange = target.DefenseChange);
                w.Write(SpAttackChange = target.SpAttackChange);
                w.Write(SpDefenseChange = target.SpDefenseChange);
                w.Write(SpeedChange = target.SpeedChange);
                w.Write(AccuracyChange = target.AccuracyChange);
                w.Write(EvasionChange = target.EvasionChange);
                Data = new ReadOnlyCollection<byte>(ms.ToArray());
            }
        }
        internal PBEPsychUpPacket(byte[] data, EndianBinaryReader r, PBEBattle battle)
        {
            Data = new ReadOnlyCollection<byte>(data);
            UserTrainer = battle.Trainers[r.ReadByte()];
            User = r.ReadEnum<PBEFieldPosition>();
            TargetTrainer = battle.Trainers[r.ReadByte()];
            Target = r.ReadEnum<PBEFieldPosition>();
            AttackChange = r.ReadSByte();
            DefenseChange = r.ReadSByte();
            SpAttackChange = r.ReadSByte();
            SpDefenseChange = r.ReadSByte();
            SpeedChange = r.ReadSByte();
            AccuracyChange = r.ReadSByte();
            EvasionChange = r.ReadSByte();
        }
    }
}
