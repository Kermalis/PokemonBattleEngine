using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;

namespace Kermalis.PokemonBattleEngine.Data
{
    public sealed class PBEPokemonShell : INotifyPropertyChanged
    {
        private void OnPropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }
        public event PropertyChangedEventHandler PropertyChanged;

        private PBESpecies species;
        public PBESpecies Species
        {
            get => species;
            set
            {
                species = value;
                OnPropertyChanged(nameof(Species));
            }
        }
        private string nickname;
        public string Nickname
        {
            get => nickname;
            set
            {
                nickname = value;
                OnPropertyChanged(nameof(Nickname));
            }
        }
        private byte level;
        public byte Level
        {
            get => level;
            set
            {
                level = value;
                OnPropertyChanged(nameof(Level));
            }
        }
        private byte friendship;
        public byte Friendship
        {
            get => friendship;
            set
            {
                friendship = value;
                OnPropertyChanged(nameof(Friendship));
            }
        }
        private bool shiny;
        public bool Shiny
        {
            get => shiny;
            set
            {
                shiny = value;
                OnPropertyChanged(nameof(Shiny));
            }
        }
        private PBEAbility ability;
        public PBEAbility Ability
        {
            get => ability;
            set
            {
                ability = value;
                OnPropertyChanged(nameof(Ability));
            }
        }
        private PBENature nature;
        public PBENature Nature
        {
            get => nature;
            set
            {
                nature = value;
                OnPropertyChanged(nameof(Nature));
            }
        }
        private PBEGender gender;
        public PBEGender Gender
        {
            get => gender;
            set
            {
                gender = value;
                OnPropertyChanged(nameof(Gender));
            }
        }
        private PBEItem item;
        public PBEItem Item
        {
            get => item;
            set
            {
                item = value;
                OnPropertyChanged(nameof(Item));
            }
        }
        private byte[] evs;
        public byte[] EVs
        {
            get => evs;
            set
            {
                evs = value;
                OnPropertyChanged(nameof(EVs));
            }
        }
        private byte[] ivs;
        public byte[] IVs
        {
            get => ivs;
            set
            {
                ivs = value;
                OnPropertyChanged(nameof(IVs));
            }
        }
        private PBEMove[] moves;
        public PBEMove[] Moves
        {
            get => moves;
            set
            {
                moves = value;
                OnPropertyChanged(nameof(Moves));
            }
        }
        private byte[] ppups;
        public byte[] PPUps
        {
            get => ppups;
            set
            {
                ppups = value;
                OnPropertyChanged(nameof(PPUps));
            }
        }

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
                        species = (PBESpecies)Enum.Parse(typeof(PBESpecies), r.ReadLine()),
                        nickname = r.ReadLine(),
                        level = byte.Parse(r.ReadLine()),
                        friendship = byte.Parse(r.ReadLine()),
                        shiny = bool.Parse(r.ReadLine()),
                        ability = (PBEAbility)Enum.Parse(typeof(PBEAbility), r.ReadLine()),
                        nature = (PBENature)Enum.Parse(typeof(PBENature), r.ReadLine()),
                        gender = (PBEGender)Enum.Parse(typeof(PBEGender), r.ReadLine()),
                        item = (PBEItem)Enum.Parse(typeof(PBEItem), r.ReadLine()),
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
                    team[i].evs = new byte[args.Length];
                    for (int j = 0; j < args.Length; j++)
                    {
                        team[i].evs[j] = byte.Parse(args[j]);
                    }
                    // IVs
                    SplitNext();
                    team[i].ivs = new byte[args.Length];
                    for (int j = 0; j < args.Length; j++)
                    {
                        team[i].ivs[j] = byte.Parse(args[j]);
                    }
                    // Moves
                    SplitNext();
                    team[i].moves = new PBEMove[args.Length];
                    for (int j = 0; j < args.Length; j++)
                    {
                        team[i].moves[j] = (PBEMove)Enum.Parse(typeof(PBEMove), args[j]);
                    }
                    // PPUps
                    SplitNext();
                    team[i].ppups = new byte[args.Length];
                    for (int j = 0; j < args.Length; j++)
                    {
                        team[i].ppups[j] = byte.Parse(args[j]);
                    }
                }
                return team;
            }
        }
        public static void TeamToTextFile(string path, IEnumerable<PBEPokemonShell> team)
        {
            // TODO: Include settings
            using (var w = new StreamWriter(path))
            {
                w.WriteLine(team.Count());
                foreach (PBEPokemonShell pkmn in team)
                {
                    w.WriteLine(pkmn.species);
                    w.WriteLine(pkmn.nickname);
                    w.WriteLine(pkmn.level);
                    w.WriteLine(pkmn.friendship);
                    w.WriteLine(pkmn.shiny);
                    w.WriteLine(pkmn.ability);
                    w.WriteLine(pkmn.nature);
                    w.WriteLine(pkmn.gender);
                    w.WriteLine(pkmn.item);
                    w.WriteLine(string.Join(", ", pkmn.evs));
                    w.WriteLine(string.Join(", ", pkmn.ivs));
                    w.WriteLine(string.Join(", ", pkmn.moves));
                    w.WriteLine(string.Join(", ", pkmn.ppups));
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
                species = (PBESpecies)r.ReadUInt32(),
                nickname = PBEUtils.StringFromBytes(r),
                level = r.ReadByte(),
                friendship = r.ReadByte(),
                shiny = r.ReadBoolean(),
                ability = (PBEAbility)r.ReadByte(),
                nature = (PBENature)r.ReadByte(),
                gender = (PBEGender)r.ReadByte(),
                item = (PBEItem)r.ReadUInt16(),
                evs = r.ReadBytes(6),
                ivs = r.ReadBytes(6)
            };
            byte numMoves = r.ReadByte();
            pkmn.moves = new PBEMove[numMoves];
            for (int i = 0; i < numMoves; i++)
            {
                pkmn.moves[i] = (PBEMove)r.ReadUInt16();
            }
            pkmn.ppups = r.ReadBytes(numMoves);
            return pkmn;
        }
    }
}
