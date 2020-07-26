using Kermalis.EndianBinaryIO;
using Kermalis.PokemonBattleEngine.Battle;
using Kermalis.PokemonBattleEngine.Data;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;

namespace Kermalis.PokemonBattleEngine.Packets
{
    public sealed class PBEWildPkmnAppearedPacket : IPBEPacket
    {
        public const ushort Code = 0x0D;
        public ReadOnlyCollection<byte> Data { get; }

        public sealed class PBEWildPkmnInfo : IPBEPkmnSwitchInInfo
        {
            public PBESpecies Species { get; }
            public PBEForm Form { get; }
            public string Nickname { get; }
            public byte Level { get; }
            public bool Shiny { get; }
            public PBEGender Gender { get; }
            public double HPPercentage { get; }
            public PBEStatus1 Status1 { get; }
            public PBEFieldPosition FieldPosition { get; }

            internal PBEWildPkmnInfo(PBEBattlePokemon pkmn)
            {
                Species = pkmn.KnownSpecies;
                Form = pkmn.KnownForm;
                Nickname = pkmn.KnownNickname;
                Level = pkmn.Level;
                Shiny = pkmn.KnownShiny;
                Gender = pkmn.KnownGender;
                HPPercentage = pkmn.HPPercentage;
                Status1 = pkmn.Status1;
                FieldPosition = pkmn.FieldPosition;
            }
            internal PBEWildPkmnInfo(EndianBinaryReader r)
            {
                Species = r.ReadEnum<PBESpecies>();
                Form = r.ReadEnum<PBEForm>();
                Nickname = r.ReadStringNullTerminated();
                Level = r.ReadByte();
                Shiny = r.ReadBoolean();
                Gender = r.ReadEnum<PBEGender>();
                HPPercentage = r.ReadDouble();
                Status1 = r.ReadEnum<PBEStatus1>();
                FieldPosition = r.ReadEnum<PBEFieldPosition>();
            }

            internal void ToBytes(EndianBinaryWriter w)
            {
                w.Write(Species);
                w.Write(Form);
                w.Write(Nickname, true);
                w.Write(Level);
                w.Write(Shiny);
                w.Write(Gender);
                w.Write(HPPercentage);
                w.Write(Status1);
                w.Write(FieldPosition);
            }
        }

        public ReadOnlyCollection<PBEWildPkmnInfo> Pokemon { get; }

        internal PBEWildPkmnAppearedPacket(IList<PBEWildPkmnInfo> switchIns)
        {
            using (var ms = new MemoryStream())
            using (var w = new EndianBinaryWriter(ms, encoding: EncodingType.UTF16))
            {
                w.Write(Code);
                byte count = (byte)(Pokemon = new ReadOnlyCollection<PBEWildPkmnInfo>(switchIns)).Count;
                w.Write(count);
                for (int i = 0; i < count; i++)
                {
                    Pokemon[i].ToBytes(w);
                }
                Data = new ReadOnlyCollection<byte>(ms.ToArray());
            }
        }
        internal PBEWildPkmnAppearedPacket(byte[] data, EndianBinaryReader r)
        {
            Data = new ReadOnlyCollection<byte>(data);
            var switches = new PBEWildPkmnInfo[r.ReadByte()];
            for (int i = 0; i < switches.Length; i++)
            {
                switches[i] = new PBEWildPkmnInfo(r);
            }
            Pokemon = new ReadOnlyCollection<PBEWildPkmnInfo>(switches);
        }
    }
}
