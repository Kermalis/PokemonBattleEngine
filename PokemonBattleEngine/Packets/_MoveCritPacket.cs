using Kermalis.EndianBinaryIO;
using Kermalis.PokemonBattleEngine.Battle;
using Kermalis.PokemonBattleEngine.Data;
using System.Collections.ObjectModel;
using System.IO;

namespace Kermalis.PokemonBattleEngine.Packets
{
    public sealed class PBEMoveCritPacket : IPBEPacket
    {
        public const ushort Code = 0x0F;
        public ReadOnlyCollection<byte> Data { get; }

        public PBEFieldPosition Victim { get; }
        public PBETeam VictimTeam { get; }

        internal PBEMoveCritPacket(PBEPokemon victim)
        {
            using (var ms = new MemoryStream())
            using (var w = new EndianBinaryWriter(ms, encoding: EncodingType.UTF16))
            {
                w.Write(Code);
                w.Write(Victim = victim.FieldPosition);
                w.Write((VictimTeam = victim.Team).Id);
                Data = new ReadOnlyCollection<byte>(ms.ToArray());
            }
        }
        internal PBEMoveCritPacket(byte[] data, EndianBinaryReader r, PBEBattle battle)
        {
            Data = new ReadOnlyCollection<byte>(data);
            Victim = r.ReadEnum<PBEFieldPosition>();
            VictimTeam = battle.Teams[r.ReadByte()];
        }
    }
}
