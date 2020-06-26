using Kermalis.EndianBinaryIO;
using Kermalis.PokemonBattleEngine.Battle;
using Kermalis.PokemonBattleEngine.Data;
using Kermalis.PokemonBattleEngine.Data.Legality;
using System;
using System.Collections.ObjectModel;
using System.IO;

namespace Kermalis.PokemonBattleEngine.Packets
{
    public sealed class PBEPartyResponsePacket : IPBEPacket
    {
        public const ushort Code = 0x04;
        public ReadOnlyCollection<byte> Data { get; }

        public IPBEPokemonCollection Party { get; }

        public PBEPartyResponsePacket(IPBEPokemonCollection party)
        {
            if (party == null)
            {
                throw new ArgumentNullException(nameof(party));
            }
            using (var ms = new MemoryStream())
            using (var w = new EndianBinaryWriter(ms, encoding: EncodingType.UTF16))
            {
                w.Write(Code);
                (Party = party).ToBytes(w);
                Data = new ReadOnlyCollection<byte>(ms.ToArray());
            }
        }
        internal PBEPartyResponsePacket(byte[] data, EndianBinaryReader r)
        {
            Data = new ReadOnlyCollection<byte>(data);
            Party = new PBEReadOnlyPokemonCollection(r);
        }
    }
    public sealed class PBELegalPartyResponsePacket : IPBEPacket
    {
        public const ushort Code = 0x2D;
        public ReadOnlyCollection<byte> Data { get; }

        public PBELegalPokemonCollection Party { get; }

        public PBELegalPartyResponsePacket(PBELegalPokemonCollection party)
        {
            if (party == null)
            {
                throw new ArgumentNullException(nameof(party));
            }
            using (var ms = new MemoryStream())
            using (var w = new EndianBinaryWriter(ms, encoding: EncodingType.UTF16))
            {
                w.Write(Code);
                (Party = party).ToBytes(w);
                Data = new ReadOnlyCollection<byte>(ms.ToArray());
            }
        }
        internal PBELegalPartyResponsePacket(byte[] data, EndianBinaryReader r, PBEBattle battle)
        {
            Data = new ReadOnlyCollection<byte>(data);
            Party = new PBELegalPokemonCollection(battle.Settings, r); // TODO: Server battle is null here
        }
    }
}
