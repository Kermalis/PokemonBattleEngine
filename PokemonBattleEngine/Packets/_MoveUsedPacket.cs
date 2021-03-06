﻿using Kermalis.EndianBinaryIO;
using Kermalis.PokemonBattleEngine.Battle;
using Kermalis.PokemonBattleEngine.Data;
using System.Collections.ObjectModel;
using System.IO;

namespace Kermalis.PokemonBattleEngine.Packets
{
    public sealed class PBEMoveUsedPacket : IPBEPacket
    {
        public const ushort Code = 0x09;
        public ReadOnlyCollection<byte> Data { get; }

        public PBETrainer MoveUserTrainer { get; }
        public PBEFieldPosition MoveUser { get; }
        public PBEMove Move { get; }
        public bool Owned { get; }

        internal PBEMoveUsedPacket(PBEBattlePokemon moveUser, PBEMove move, bool owned)
        {
            using (var ms = new MemoryStream())
            using (var w = new EndianBinaryWriter(ms, encoding: EncodingType.UTF16))
            {
                w.Write(Code);
                w.Write((MoveUserTrainer = moveUser.Trainer).Id);
                w.Write(MoveUser = moveUser.FieldPosition);
                w.Write(Move = move);
                w.Write(Owned = owned);
                Data = new ReadOnlyCollection<byte>(ms.ToArray());
            }
        }
        internal PBEMoveUsedPacket(byte[] data, EndianBinaryReader r, PBEBattle battle)
        {
            Data = new ReadOnlyCollection<byte>(data);
            MoveUserTrainer = battle.Trainers[r.ReadByte()];
            MoveUser = r.ReadEnum<PBEFieldPosition>();
            Move = r.ReadEnum<PBEMove>();
            Owned = r.ReadBoolean();
        }
    }
}
