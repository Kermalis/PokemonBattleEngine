﻿using Kermalis.EndianBinaryIO;
using Kermalis.PokemonBattleEngine.Data;
using Kermalis.PokemonBattleEngine.Packets;
using Kermalis.PokemonBattleEngine.Utils;
using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;

namespace Kermalis.PokemonBattleEngine.Battle
{
    public sealed partial class PBEBattle
    {
        private const ushort CurrentReplayVersion = 0;

        public string GetDefaultReplayFileName()
        {
            // "2020-12-30 23-59-59 - Team 1 vs Team 2.pbereplay"
            return PBEUtils.ToSafeFileName(new string(string.Format("{0:yyyy-MM-dd HH-mm-ss} - {1} vs {2}", DateTime.Now, Teams[0].CombinedName, Teams[1].CombinedName).Take(200).ToArray())) + ".pbereplay";
        }

        public void SaveReplay()
        {
            if (_battleState != PBEBattleState.Ended)
            {
                throw new InvalidOperationException($"{nameof(BattleState)} must be {PBEBattleState.Ended} to save a replay.");
            }
            SaveReplay(GetDefaultReplayFileName());
        }
        public void SaveReplayToFolder(string path)
        {
            if (path is null)
            {
                throw new ArgumentNullException(nameof(path));
            }
            if (_battleState != PBEBattleState.Ended)
            {
                throw new InvalidOperationException($"{nameof(BattleState)} must be {PBEBattleState.Ended} to save a replay.");
            }
            SaveReplay(Path.Combine(path, GetDefaultReplayFileName()));
        }
        public void SaveReplay(string path)
        {
            if (path is null)
            {
                throw new ArgumentNullException(nameof(path));
            }
            if (_battleState != PBEBattleState.Ended)
            {
                throw new InvalidOperationException($"{nameof(BattleState)} must be {PBEBattleState.Ended} to save a replay.");
            }

            using (var ms = new MemoryStream())
            using (var w = new EndianBinaryWriter(ms, encoding: EncodingType.UTF16))
            {
                w.Write(CurrentReplayVersion);
                w.Write(_rand.Seed);

                int numEvents = Events.Count;
                w.Write(numEvents);
                for (int i = 0; i < numEvents; i++)
                {
                    byte[] data = Events[i].Data.ToArray();
                    w.Write((ushort)data.Length);
                    w.Write(data);
                }

                using (var md5 = MD5.Create())
                {
                    w.Write(md5.ComputeHash(ms.ToArray()));
                }

                File.WriteAllBytes(path, ms.ToArray());
            }
        }

        public static PBEBattle LoadReplay(string path)
        {
            byte[] fileBytes = File.ReadAllBytes(path);
            using (var s = new MemoryStream(fileBytes))
            using (var r = new EndianBinaryReader(s, encoding: EncodingType.UTF16))
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
                ushort version = r.ReadUInt16(); // Unused for now
                int seed = r.ReadInt32(); // Unused for now
                PBEBattle b = null;
                int numEvents = r.ReadInt32();
                for (int i = 0; i < numEvents; i++)
                {
                    IPBEPacket packet = PBEPacketProcessor.CreatePacket(b, r.ReadBytes(r.ReadUInt16()));
                    if (packet is PBEBattlePacket bp)
                    {
                        b = new PBEBattle(bp);
                    }
                    else if (packet is PBEWildPkmnAppearedPacket wpap)
                    {
                        PBETrainer wildTrainer = b.Teams[1].Trainers[0];
                        foreach (PBEPkmnAppearedInfo info in wpap.Pokemon)
                        {
                            PBEBattlePokemon pkmn = wildTrainer.TryGetPokemon(info.Pokemon);
                            // Process disguise and position now
                            pkmn.FieldPosition = info.FieldPosition;
                            if (info.IsDisguised)
                            {
                                pkmn.Status2 |= PBEStatus2.Disguised;
                                pkmn.KnownCaughtBall = info.CaughtBall;
                                pkmn.KnownGender = info.Gender;
                                pkmn.KnownNickname = info.Nickname;
                                pkmn.KnownShiny = info.Shiny;
                                pkmn.KnownSpecies = info.Species;
                                pkmn.KnownForm = info.Form;
                                IPBEPokemonData pData = PBEDataProvider.Instance.GetPokemonData(info);
                                pkmn.KnownType1 = pData.Type1;
                                pkmn.KnownType2 = pData.Type2;
                            }
                            b.ActiveBattlers.Add(pkmn);
                        }
                    }
                    b.Events.Add(packet);
                }
                b.BattleState = PBEBattleState.Ended;
                return b;
            }
        }
    }
}
