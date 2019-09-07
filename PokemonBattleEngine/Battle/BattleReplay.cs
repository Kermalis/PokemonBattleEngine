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

            bytes.AddRange(Settings.ToBytes());
            bytes.Add((byte)BattleFormat);

            bytes.AddRange(Teams[0].ToBytes());
            bytes.AddRange(Teams[1].ToBytes());

            bytes.AddRange(BitConverter.GetBytes(Events.Count));
            for (int i = 0; i < Events.Count; i++)
            {
                bytes.AddRange(Events[i].Buffer);
            }

            using (var md5 = MD5.Create())
            {
                bytes.AddRange(md5.ComputeHash(bytes.ToArray()));
            }

            File.WriteAllBytes(path, bytes.ToArray());
        }

        public static PBEBattle LoadReplay(string path)
        {
            byte[] fileBytes = File.ReadAllBytes(path);
            using (var s = new MemoryStream(fileBytes))
            using (var r = new BinaryReader(s))
            {
                byte[] hash;
                using (var md5 = MD5.Create())
                {
                    hash = md5.ComputeHash(fileBytes, 0, fileBytes.Length - 16);
                }
                for (int i = 0; i < 16; i++)
                {
                    if (hash[i] != fileBytes[fileBytes.Length - 16 + i])
                    {
                        throw new InvalidDataException();
                    }
                }
                return new PBEBattle(r);
            }
        }
        private PBEBattle(BinaryReader r)
        {
            ushort version = r.ReadUInt16();

            Settings = new PBESettings(r);
            Settings.MakeReadOnly();
            BattleFormat = (PBEBattleFormat)r.ReadByte();

            Teams[0] = new PBETeam(this, 0);
            Teams[0].FromBytes(r);
            Teams[1] = new PBETeam(this, 1);
            Teams[1].FromBytes(r);

            var packetProcessor = new PBEPacketProcessor(this);
            int numEvents = r.ReadInt32();
            for (int i = 0; i < numEvents; i++)
            {
                INetPacket packet = packetProcessor.CreatePacket(r.ReadBytes(r.ReadInt16()));
                if (packet is PBEWinnerPacket wp)
                {
                    Winner = wp.WinningTeam;
                }
                Events.Add(packet);
            }

            BattleState = PBEBattleState.Ended;
            OnStateChanged?.Invoke(this);
        }
    }
}
