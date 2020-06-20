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

        public PBEFieldPosition User { get; }
        public PBETeam UserTeam { get; }
        public PBEFieldPosition Target { get; }
        public PBETeam TargetTeam { get; }
        public PBEType Type1 { get; }
        public PBEType Type2 { get; }

        internal PBEReflectTypePacket(PBEBattlePokemon user, PBEBattlePokemon target)
        {
            using (var ms = new MemoryStream())
            using (var w = new EndianBinaryWriter(ms, encoding: EncodingType.UTF16))
            {
                w.Write(Code);
                w.Write(User = user.FieldPosition);
                w.Write((UserTeam = user.Team).Id);
                w.Write(Target = target.FieldPosition);
                w.Write((TargetTeam = target.Team).Id);
                w.Write(Type1 = target.Type1);
                w.Write(Type2 = target.Type2);
                Data = new ReadOnlyCollection<byte>(ms.ToArray());
            }
        }
        internal PBEReflectTypePacket(byte[] data, EndianBinaryReader r, PBEBattle battle)
        {
            Data = new ReadOnlyCollection<byte>(data);
            User = r.ReadEnum<PBEFieldPosition>();
            UserTeam = battle.Teams[r.ReadByte()];
            Target = r.ReadEnum<PBEFieldPosition>();
            TargetTeam = battle.Teams[r.ReadByte()];
            Type1 = r.ReadEnum<PBEType>();
            Type2 = r.ReadEnum<PBEType>();
        }
    }
    public sealed class PBEReflectTypePacket_Hidden : IPBEPacket
    {
        public const ushort Code = 0x33;
        public ReadOnlyCollection<byte> Data { get; }

        public PBEFieldPosition User { get; }
        public PBETeam UserTeam { get; }
        public PBEFieldPosition Target { get; }
        public PBETeam TargetTeam { get; }

        public PBEReflectTypePacket_Hidden(PBEReflectTypePacket other)
        {
            if (other == null)
            {
                throw new ArgumentNullException(nameof(other));
            }
            using (var ms = new MemoryStream())
            using (var w = new EndianBinaryWriter(ms, encoding: EncodingType.UTF16))
            {
                w.Write(Code);
                w.Write(User = other.User);
                w.Write((UserTeam = other.UserTeam).Id);
                w.Write(Target = other.Target);
                w.Write((TargetTeam = other.TargetTeam).Id);
                Data = new ReadOnlyCollection<byte>(ms.ToArray());
            }
        }
        internal PBEReflectTypePacket_Hidden(byte[] data, EndianBinaryReader r, PBEBattle battle)
        {
            Data = new ReadOnlyCollection<byte>(data);
            User = r.ReadEnum<PBEFieldPosition>();
            UserTeam = battle.Teams[r.ReadByte()];
            Target = r.ReadEnum<PBEFieldPosition>();
            TargetTeam = battle.Teams[r.ReadByte()];
        }
    }
}
