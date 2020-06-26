using Kermalis.EndianBinaryIO;
using Kermalis.PokemonBattleEngine.Battle;
using System.Collections.ObjectModel;
using System.IO;

namespace Kermalis.PokemonBattleEngine.Packets
{
    public sealed class PBEPsychUpPacket : IPBEPacket
    {
        public const ushort Code = 0x22;
        public ReadOnlyCollection<byte> Data { get; }

        public PBEBattlePokemon User { get; }
        public PBEBattlePokemon Target { get; }
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
                (User = user).ToBytes_Position(w);
                (Target = target).ToBytes_Position(w);
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
            User = battle.GetPokemon_Position(r);
            Target = battle.GetPokemon_Position(r);
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
