using Ether.Network.Packets;
using Kermalis.PokemonBattleEngine.Battle;
using Kermalis.PokemonBattleEngine.Data;
using System;
using System.IO;
using System.Text;

namespace Kermalis.PokemonBattleEngine.Network
{
    public sealed class PRequestTeamPacket : INetPacketStream
    {
        public const int Code = 0x1;
        byte[] buf;
        public byte[] Buffer => (byte[])buf.Clone();

        public readonly PTeamShell Team;

        public PRequestTeamPacket(PTeamShell team)
        {
            Team = team ?? throw new ArgumentNullException(nameof(team));

            var numPkmn = Math.Min(PConstants.MaxPokemon, (byte)Team.Pokemon.Count);
            byte[] playerNameBytes = Encoding.ASCII.GetBytes(Team.PlayerName);

            // size, Code, name size, name, numPkmn, pkmn array
            int numBytes = 4 + 4 + 1 + playerNameBytes.Length + 1 + (28 * numPkmn);
            buf = new byte[numBytes];

            using (var w = new BinaryWriter(new MemoryStream(buf)))
            {
                w.Write(numBytes - 4);
                w.Write(Code);
                w.Write((byte)playerNameBytes.Length);
                w.Write(playerNameBytes);
                w.Write(numPkmn);
                for (int i = 0; i < numPkmn; i++)
                {
                    var pkmn = Team.Pokemon[i];
                    w.Write((ushort)pkmn.Species);
                    w.Write(pkmn.Level);
                    w.Write(pkmn.Friendship);
                    w.Write((byte)pkmn.Ability);
                    w.Write((byte)pkmn.Nature);
                    w.Write((ushort)pkmn.Item);
                    for (int j = 0; j < 6; j++)
                        w.Write(pkmn.EVs[j]);
                    for (int j = 0; j < 6; j++)
                        w.Write(pkmn.IVs[j]);
                    for (int j = 0; j < PConstants.NumMoves; j++)
                        w.Write((ushort)pkmn.Moves[j]);
                }
            }
        }
        public PRequestTeamPacket(byte[] buffer)
        {
            using (var r = new BinaryReader(new MemoryStream(buf = buffer)))
            {
                r.ReadInt32(); // Skip Code
                Team = new PTeamShell { PlayerName = Encoding.ASCII.GetString(r.ReadBytes(r.ReadByte())) };
                var numPkmn = Math.Min(PConstants.MaxPokemon, r.ReadByte());
                for (int i = 0; i < numPkmn; i++)
                {
                    var pkmn = new PPokemonShell
                    {
                        Species = (PSpecies)r.ReadUInt16(),
                        Level = r.ReadByte(),
                        Friendship = r.ReadByte(),
                        Ability = (PAbility)r.ReadByte(),
                        Nature = (PNature)r.ReadByte(),
                        Item = (PItem)r.ReadUInt16()
                    };
                    for (int j = 0; j < 6; j++)
                        pkmn.EVs[j] = r.ReadByte();
                    for (int j = 0; j < 6; j++)
                        pkmn.IVs[j] = r.ReadByte();
                    for (int j = 0; j < PConstants.NumMoves; j++)
                        pkmn.Moves[j] = (PMove)r.ReadUInt16();
                    Team.Pokemon.Add(pkmn);
                }
            }
        }

        public int Size => throw new NotImplementedException();
        public long Position => throw new NotImplementedException();
        public T Read<T>() => throw new NotImplementedException();
        public T[] ReadArray<T>(int amount) => throw new NotImplementedException();
        public void Write<T>(T value) => throw new NotImplementedException();
        public void Dispose() { }
    }
}
