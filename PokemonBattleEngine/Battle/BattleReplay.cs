using Ether.Network.Packets;
using Kermalis.PokemonBattleEngine.Data;
using Kermalis.PokemonBattleEngine.Packets;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;

namespace Kermalis.PokemonBattleEngine.Battle
{
    public sealed partial class PBEBattle
    {
        private const ushort CurrentReplayVersion = 0;

        public void SaveReplay()
        {
            // "12-30-2020 11-59-59 PM - Team 1 vs Team 2.pbereplay"
            SaveReplay(PBEUtils.ToSafeFileName(new string(string.Format("{0} - {1} vs {2}", DateTime.Now.ToLocalTime(), Teams[0].TrainerName, Teams[1].TrainerName).Take(200).ToArray())) + ".pbereplay");
        }
        public void SaveReplay(string path)
        {
            if (BattleState != PBEBattleState.Ended)
            {
                throw new InvalidOperationException($"{nameof(BattleState)} must be {PBEBattleState.Ended} to save a replay.");
            }

            var bytes = new List<byte>();
            bytes.AddRange(BitConverter.GetBytes(CurrentReplayVersion));
            bytes.Add((byte)BattleFormat);

            bytes.AddRange(PBEUtils.StringToBytes(Teams[0].TrainerName));
            bytes.Add((byte)Teams[0].Party.Count);
            bytes.AddRange(Teams[0].Party.SelectMany(p => p.Shell.ToBytes()));

            bytes.AddRange(PBEUtils.StringToBytes(Teams[1].TrainerName));
            bytes.Add((byte)Teams[1].Party.Count);
            bytes.AddRange(Teams[1].Party.SelectMany(p => p.Shell.ToBytes()));

            foreach (INetPacket packet in Events)
            {
                bytes.AddRange(packet.Buffer.ToArray());
            }

            using (var md5 = MD5.Create())
            {
                bytes.AddRange(md5.ComputeHash(bytes.ToArray()));
            }

            File.WriteAllBytes(path, bytes.ToArray());
        }

        public static PBEBattle LoadReplay(string path)
        {
            PBESettings settings = PBESettings.DefaultSettings;
            byte[] fileBytes = File.ReadAllBytes(path);
            using (var s = new MemoryStream(fileBytes))
            using (var r = new BinaryReader(s))
            {
                using (var md5 = MD5.Create())
                {
                    byte[] hash = md5.ComputeHash(fileBytes, 0, fileBytes.Length - 16);
                    for (int i = 0; i < 16; i++)
                    {
                        if (hash[i] != fileBytes[fileBytes.Length - 16 + i])
                        {
                            throw new InvalidDataException();
                        }
                    }
                }

                ushort version = r.ReadUInt16();

                var battle = new PBEBattle((PBEBattleFormat)r.ReadByte(), settings);

                battle.Teams[0].TrainerName = PBEUtils.StringFromBytes(r);
                var party = new PBEPokemonShell[r.ReadSByte()];
                for (int i = 0; i < party.Length; i++)
                {
                    party[i] = PBEPokemonShell.FromBytes(r, settings);
                }
                battle.Teams[0].CreateParty(party, ref battle.pkmnIdCounter);

                battle.Teams[1].TrainerName = PBEUtils.StringFromBytes(r);
                party = new PBEPokemonShell[r.ReadSByte()];
                for (int i = 0; i < party.Length; i++)
                {
                    party[i] = PBEPokemonShell.FromBytes(r, settings);
                }
                battle.Teams[1].CreateParty(party, ref battle.pkmnIdCounter);

                var packetProcessor = new PBEPacketProcessor(battle);
                INetPacket packet;
                do
                {
                    byte[] buffer = r.ReadBytes(r.ReadInt16());
                    packet = packetProcessor.CreatePacket(buffer);
                    battle.Events.Add(packet);
                } while (!(packet is PBEWinnerPacket));

                return battle;
            }
        }
    }
}
