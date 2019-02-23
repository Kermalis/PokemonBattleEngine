using Ether.Network.Packets;
using Kermalis.PokemonBattleEngine.Data;
using Kermalis.PokemonBattleEngine.Packets;
using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;

namespace Kermalis.PokemonBattleEngine.Battle
{
    public sealed partial class PBEBattle
    {
        const ushort CurrentReplayVersion = 0;

        public void SaveReplay()
        {
            if (BattleState != PBEBattleState.Ended)
            {
                throw new InvalidOperationException($"{nameof(BattleState)} must be {PBEBattleState.Ended} to save a replay.");
            }

            string path = "Test" + ".pbereplay";
            using (var s = new FileStream(path, FileMode.Create, FileAccess.ReadWrite))
            {
                // Write current replay version
                s.Write(BitConverter.GetBytes(CurrentReplayVersion), 0, sizeof(ushort));
                // Write battle format
                s.Write(BitConverter.GetBytes((byte)BattleFormat), 0, sizeof(byte));

                // Write team 0 party
                var team0Size = (sbyte)Teams[0].Party.Count;
                s.Write(BitConverter.GetBytes(team0Size), 0, sizeof(sbyte));
                for (int i = 0; i < team0Size; i++)
                {
                    byte[] shell = Teams[0].Party[i].Shell.ToBytes();
                    s.Write(shell, 0, shell.Length);
                }
                // Write team 1 party
                var team1Size = (sbyte)Teams[1].Party.Count;
                s.Write(BitConverter.GetBytes(team1Size), 0, sizeof(sbyte));
                for (int i = 0; i < team1Size; i++)
                {
                    byte[] shell = Teams[1].Party[i].Shell.ToBytes();
                    s.Write(shell, 0, shell.Length);
                }

                // Write all packets
                foreach (INetPacket packet in Events)
                {
                    byte[] buffer = packet.Buffer.ToArray();
                    s.Write(buffer, 0, buffer.Length);
                }

                // Generate hash
                using (var md5 = MD5.Create())
                {
                    byte[] hash = md5.ComputeHash(s);
                    s.Write(hash, 0, hash.Length);
                }
            }
        }

        public static void LoadReplay(string path)
        {
            using (var s = new FileStream(path, FileMode.Open))
            using (var r = new BinaryReader(s))
            {
                Console.WriteLine("Replay version: {0}", r.ReadUInt16());
                var battleFormat = (PBEBattleFormat)r.ReadByte();
                Console.WriteLine("Battle format: {0}", battleFormat);

                sbyte team0Size = r.ReadSByte();
                Console.WriteLine("Team 0 size: {0}", team0Size);
                for (int i = 0; i < team0Size; i++)
                {
                    var shell = PBEPokemonShell.FromBytes(r);
                    Console.WriteLine("\t{0}/{1}", shell.Nickname, shell.Species);
                }
                sbyte team1Size = r.ReadSByte();
                Console.WriteLine("Team 1 size: {0}", team1Size);
                for (int i = 0; i < team1Size; i++)
                {
                    var shell = PBEPokemonShell.FromBytes(r);
                    Console.WriteLine("\t{0}/{1}", shell.Nickname, shell.Species);
                }

                var processor = new PBEPacketProcessor(new PBEBattle(battleFormat, PBESettings.DefaultSettings));
                INetPacket packet;
                do
                {
                    byte[] buffer = r.ReadBytes(r.ReadInt16());
                    packet = processor.CreatePacket(buffer);
                    Console.WriteLine(packet.GetType().Name);
                } while (!(packet is PBEWinnerPacket));

                Console.WriteLine("Hash: {0}", BitConverter.ToString(r.ReadBytes(16)).Replace("-", string.Empty).ToLower());
            }
        }
    }
}
