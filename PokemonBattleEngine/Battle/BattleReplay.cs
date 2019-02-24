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
            // "12-30-2020 11-59-59 PM - Team 1 vs Team 2.pbereplay"
            string path = PBEUtils.ToSafeFileName(new string(string.Format("{0} - {1} vs {2}", DateTime.Now.ToLocalTime(), Teams[0].TrainerName, Teams[1].TrainerName).Take(200).ToArray())) + ".pbereplay";
            SaveReplay(path);
        }
        public void SaveReplay(string path)
        {
            if (BattleState != PBEBattleState.Ended)
            {
                throw new InvalidOperationException($"{nameof(BattleState)} must be {PBEBattleState.Ended} to save a replay.");
            }

            using (var s = new FileStream(path, FileMode.Create, FileAccess.ReadWrite))
            {
                // Write current replay version
                s.Write(BitConverter.GetBytes(CurrentReplayVersion), 0, sizeof(ushort));
                // Write battle format
                s.Write(BitConverter.GetBytes((byte)BattleFormat), 0, sizeof(byte));

                // Write team 0
                byte[] name = PBEUtils.StringToBytes(Teams[0].TrainerName);
                s.Write(name, 0, name.Length);
                sbyte size = (sbyte)Teams[0].Party.Count;
                s.Write(BitConverter.GetBytes(size), 0, sizeof(sbyte));
                for (int i = 0; i < size; i++)
                {
                    byte[] shell = Teams[0].Party[i].Shell.ToBytes();
                    s.Write(shell, 0, shell.Length);
                }
                // Write team 1
                name = PBEUtils.StringToBytes(Teams[1].TrainerName);
                s.Write(name, 0, name.Length);
                size = (sbyte)Teams[1].Party.Count;
                s.Write(BitConverter.GetBytes(size), 0, sizeof(sbyte));
                for (int i = 0; i < size; i++)
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

        public static PBEBattle LoadReplay(string path)
        {
            using (var s = new FileStream(path, FileMode.Open))
            using (var r = new BinaryReader(s))
            {
                ushort version = r.ReadUInt16();

                var battle = new PBEBattle((PBEBattleFormat)r.ReadByte(), PBESettings.DefaultSettings);
                var processor = new PBEPacketProcessor(battle);

                battle.Teams[0].TrainerName = PBEUtils.StringFromBytes(r);
                var party = new PBEPokemonShell[r.ReadSByte()];
                for (int i = 0; i < party.Length; i++)
                {
                    party[i] = PBEPokemonShell.FromBytes(r);
                }
                battle.Teams[0].CreateParty(party, ref battle.pkmnIdCounter);
                battle.Teams[1].TrainerName = PBEUtils.StringFromBytes(r);
                party = new PBEPokemonShell[r.ReadSByte()];
                for (int i = 0; i < party.Length; i++)
                {
                    party[i] = PBEPokemonShell.FromBytes(r);
                }
                battle.Teams[1].CreateParty(party, ref battle.pkmnIdCounter);

                INetPacket packet;
                do
                {
                    byte[] buffer = r.ReadBytes(r.ReadInt16());
                    packet = processor.CreatePacket(buffer);
                    battle.Events.Add(packet);
                } while (!(packet is PBEWinnerPacket));

                Console.WriteLine("Hash: {0}", BitConverter.ToString(r.ReadBytes(16)).Replace("-", string.Empty).ToLower());

                return battle;
            }
        }
    }
}
