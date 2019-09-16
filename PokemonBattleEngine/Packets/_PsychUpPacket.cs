using Ether.Network.Packets;
using Kermalis.PokemonBattleEngine.Battle;
using Kermalis.PokemonBattleEngine.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;

namespace Kermalis.PokemonBattleEngine.Packets
{
    public sealed class PBEPsychUpPacket : INetPacket
    {
        public const short Code = 0x22;
        public ReadOnlyCollection<byte> Buffer { get; }

        public PBEFieldPosition User { get; }
        public PBETeam UserTeam { get; }
        public PBEFieldPosition Target { get; }
        public PBETeam TargetTeam { get; }
        public sbyte AttackChange { get; }
        public sbyte DefenseChange { get; }
        public sbyte SpAttackChange { get; }
        public sbyte SpDefenseChange { get; }
        public sbyte SpeedChange { get; }
        public sbyte AccuracyChange { get; }
        public sbyte EvasionChange { get; }

        internal PBEPsychUpPacket(PBEPokemon user, PBEPokemon target)
        {
            var bytes = new List<byte>();
            bytes.AddRange(BitConverter.GetBytes(Code));
            bytes.Add((byte)(User = user.FieldPosition));
            bytes.Add((UserTeam = user.Team).Id);
            bytes.Add((byte)(Target = target.FieldPosition));
            bytes.Add((TargetTeam = target.Team).Id);
            bytes.Add((byte)(AttackChange = target.AttackChange));
            bytes.Add((byte)(DefenseChange = target.DefenseChange));
            bytes.Add((byte)(SpAttackChange = target.SpAttackChange));
            bytes.Add((byte)(SpDefenseChange = target.SpDefenseChange));
            bytes.Add((byte)(SpeedChange = target.SpeedChange));
            bytes.Add((byte)(AccuracyChange = target.AccuracyChange));
            bytes.Add((byte)(EvasionChange = target.EvasionChange));
            bytes.InsertRange(0, BitConverter.GetBytes((short)bytes.Count));
            Buffer = new ReadOnlyCollection<byte>(bytes);
        }
        internal PBEPsychUpPacket(ReadOnlyCollection<byte> buffer, BinaryReader r, PBEBattle battle)
        {
            Buffer = buffer;
            User = (PBEFieldPosition)r.ReadByte();
            UserTeam = battle.Teams[r.ReadByte()];
            Target = (PBEFieldPosition)r.ReadByte();
            TargetTeam = battle.Teams[r.ReadByte()];
            AttackChange = r.ReadSByte();
            DefenseChange = r.ReadSByte();
            SpAttackChange = r.ReadSByte();
            SpDefenseChange = r.ReadSByte();
            SpeedChange = r.ReadSByte();
            AccuracyChange = r.ReadSByte();
            EvasionChange = r.ReadSByte();
        }

        public void Dispose() { }
    }
}
