using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Kermalis.PokemonBattleEngine.Data
{
    public sealed class PBEPokemonShell
    {
        public PBESpecies Species { get; set; }
        public string Nickname { get; set; }
        public byte Level { get; set; }
        public byte Friendship { get; set; }
        public bool Shiny { get; set; }
        public PBEAbility Ability { get; set; }
        public PBENature Nature { get; set; }
        public PBEGender Gender { get; set; }
        public PBEItem Item { get; set; }
        public byte[] EVs { get; set; }
        public byte[] IVs { get; set; }
        public PBEMove[] Moves { get; set; }
        public byte[] PPUps { get; set; }

        public static IEnumerable<PBEPokemonShell> TeamFromTextFile(string path)
        {
            using (StreamReader r = File.OpenText(path))
            {
                int amt = int.Parse(r.ReadLine());
                var team = new PBEPokemonShell[amt];
                for (int i = 0; i < amt; i++)
                {
                    team[i] = new PBEPokemonShell
                    {
                        Species = (PBESpecies)Enum.Parse(typeof(PBESpecies), r.ReadLine()),
                        Nickname = r.ReadLine(),
                        Level = byte.Parse(r.ReadLine()),
                        Friendship = byte.Parse(r.ReadLine()),
                        Shiny = bool.Parse(r.ReadLine()),
                        Ability = (PBEAbility)Enum.Parse(typeof(PBEAbility), r.ReadLine()),
                        Nature = (PBENature)Enum.Parse(typeof(PBENature), r.ReadLine()),
                        Gender = (PBEGender)Enum.Parse(typeof(PBEGender), r.ReadLine()),
                        Item = (PBEItem)Enum.Parse(typeof(PBEItem), r.ReadLine()),
                    };
                    // Setup for arrays
                    string line;
                    string[] args;
                    void SplitNext()
                    {
                        line = r.ReadLine();
                        args = line.Split(new[] { ", " }, StringSplitOptions.None);
                    }
                    // EVs
                    SplitNext();
                    team[i].EVs = new byte[args.Length];
                    for (int j = 0; j < args.Length; j++)
                    {
                        team[i].EVs[j] = byte.Parse(args[j]);
                    }
                    // IVs
                    SplitNext();
                    team[i].IVs = new byte[args.Length];
                    for (int j = 0; j < args.Length; j++)
                    {
                        team[i].IVs[j] = byte.Parse(args[j]);
                    }
                    // Moves
                    SplitNext();
                    team[i].Moves = new PBEMove[args.Length];
                    for (int j = 0; j < args.Length; j++)
                    {
                        team[i].Moves[j] = (PBEMove)Enum.Parse(typeof(PBEMove), args[j]);
                    }
                    // PPUps
                    SplitNext();
                    team[i].PPUps = new byte[args.Length];
                    for (int j = 0; j < args.Length; j++)
                    {
                        team[i].PPUps[j] = byte.Parse(args[j]);
                    }
                }
                return team;
            }
        }
        public static void TeamToTextFile(string path, IEnumerable<PBEPokemonShell> team)
        {
            using (var w = new StreamWriter(path))
            {
                w.WriteLine(team.Count());
                foreach (PBEPokemonShell pkmn in team)
                {
                    w.WriteLine(pkmn.Species);
                    w.WriteLine(pkmn.Nickname);
                    w.WriteLine(pkmn.Level);
                    w.WriteLine(pkmn.Friendship);
                    w.WriteLine(pkmn.Shiny);
                    w.WriteLine(pkmn.Ability);
                    w.WriteLine(pkmn.Nature);
                    w.WriteLine(pkmn.Gender);
                    w.WriteLine(pkmn.Item);
                    w.WriteLine(string.Join(", ", pkmn.EVs));
                    w.WriteLine(string.Join(", ", pkmn.IVs));
                    w.WriteLine(string.Join(", ", pkmn.Moves));
                    w.WriteLine(string.Join(", ", pkmn.PPUps));
                }
            }
        }

        internal byte[] ToBytes()
        {
            var bytes = new List<byte>();
            bytes.AddRange(BitConverter.GetBytes((uint)Species));
            bytes.AddRange(PBEUtils.StringToBytes(Nickname));
            bytes.Add(Level);
            bytes.Add(Friendship);
            bytes.Add((byte)(Shiny ? 1 : 0));
            bytes.Add((byte)Ability);
            bytes.Add((byte)Nature);
            bytes.Add((byte)Gender);
            bytes.AddRange(BitConverter.GetBytes((ushort)Item));
            bytes.AddRange(EVs);
            bytes.AddRange(IVs);
            bytes.Add((byte)Moves.Length);
            for (int i = 0; i < Moves.Length; i++)
            {
                bytes.AddRange(BitConverter.GetBytes((ushort)Moves[i]));
            }
            bytes.AddRange(PPUps);
            return bytes.ToArray();
        }
        internal static PBEPokemonShell FromBytes(BinaryReader r)
        {
            var pkmn = new PBEPokemonShell
            {
                Species = (PBESpecies)r.ReadUInt32(),
                Nickname = PBEUtils.StringFromBytes(r),
                Level = r.ReadByte(),
                Friendship = r.ReadByte(),
                Shiny = r.ReadBoolean(),
                Ability = (PBEAbility)r.ReadByte(),
                Nature = (PBENature)r.ReadByte(),
                Gender = (PBEGender)r.ReadByte(),
                Item = (PBEItem)r.ReadUInt16(),
                EVs = r.ReadBytes(6),
                IVs = r.ReadBytes(6)
            };
            byte numMoves = r.ReadByte();
            pkmn.Moves = new PBEMove[numMoves];
            for (int i = 0; i < numMoves; i++)
            {
                pkmn.Moves[i] = (PBEMove)r.ReadUInt16();
            }
            pkmn.PPUps = r.ReadBytes(numMoves);
            return pkmn;
        }
    }
}
