using Kermalis.EndianBinaryIO;
using Kermalis.PokemonBattleEngine.Battle;
using Kermalis.PokemonBattleEngine.Data;
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

        private PBEReflectTypePacket(PBEFieldPosition user, PBETeam userTeam, PBEFieldPosition target, PBETeam targetTeam, PBEType type1, PBEType type2)
        {
            using (var ms = new MemoryStream())
            using (var w = new EndianBinaryWriter(ms, encoding: EncodingType.UTF16))
            {
                w.Write(Code);
                w.Write(User = user);
                w.Write((UserTeam = userTeam).Id);
                w.Write(Target = target);
                w.Write((TargetTeam = targetTeam).Id);
                w.Write(Type1 = type1);
                w.Write(Type2 = type2);
                Data = new ReadOnlyCollection<byte>(ms.ToArray());
            }
        }
        internal PBEReflectTypePacket(PBEBattlePokemon user, PBEBattlePokemon target)
            : this(user.FieldPosition, user.Team, target.FieldPosition, target.Team, target.Type1, target.Type2) { }
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

        public PBEReflectTypePacket MakeHidden()
        {
            return new PBEReflectTypePacket(User, UserTeam, Target, TargetTeam, PBEType.None, PBEType.None);
        }
    }
}
