using Kermalis.EndianBinaryIO;
using Kermalis.PokemonBattleEngine.Battle;
using Kermalis.PokemonBattleEngine.Data;
using Kermalis.PokemonBattleEngine.Utils;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

namespace Kermalis.PokemonBattleEngine.Packets
{
    public sealed class PBEBattlePacket : IPBEPacket
    {
        public const ushort Code = 0x05;
        public ReadOnlyCollection<byte> Data { get; }

        public sealed class PBETeamInfo
        {
            public sealed class PBETrainerInfo
            {
                public sealed class PBEBattlePokemonInfo // SleepTurns would be too much info for a client to have
                {
                    public byte Id { get; }
                    public PBESpecies Species { get; }
                    public PBEForm Form { get; }
                    public string Nickname { get; }
                    public byte Level { get; }
                    public byte Friendship { get; }
                    public bool Shiny { get; }
                    public PBEAbility Ability { get; }
                    public PBENature Nature { get; }
                    public PBEGender Gender { get; }
                    public PBEItem Item { get; }
                    public PBEStatus1 Status1 { get; }
                    public PBEReadOnlyStatCollection EffortValues { get; }
                    public PBEReadOnlyStatCollection IndividualValues { get; }
                    public PBEReadOnlyPartyMoveset Moveset { get; }

                    internal PBEBattlePokemonInfo(PBEBattlePokemon pkmn)
                    {
                        Id = pkmn.Id;
                        Species = pkmn.OriginalSpecies;
                        Form = pkmn.OriginalForm;
                        Nickname = pkmn.Nickname;
                        Level = pkmn.OriginalLevel;
                        Friendship = pkmn.Friendship;
                        Shiny = pkmn.Shiny;
                        Ability = pkmn.OriginalAbility;
                        Nature = pkmn.Nature;
                        Gender = pkmn.Gender;
                        Item = pkmn.OriginalItem;
                        Status1 = pkmn.OriginalStatus1;
                        EffortValues = pkmn.OriginalEffortValues;
                        IndividualValues = pkmn.IndividualValues;
                        Moveset = pkmn.OriginalMoveset;
                    }
                    internal PBEBattlePokemonInfo(EndianBinaryReader r)
                    {
                        Id = r.ReadByte();
                        Species = r.ReadEnum<PBESpecies>();
                        Form = r.ReadEnum<PBEForm>();
                        Nickname = r.ReadStringNullTerminated();
                        Level = r.ReadByte();
                        Friendship = r.ReadByte();
                        Shiny = r.ReadBoolean();
                        Ability = r.ReadEnum<PBEAbility>();
                        Nature = r.ReadEnum<PBENature>();
                        Gender = r.ReadEnum<PBEGender>();
                        Item = r.ReadEnum<PBEItem>();
                        Status1 = r.ReadEnum<PBEStatus1>();
                        EffortValues = new PBEReadOnlyStatCollection(r);
                        IndividualValues = new PBEReadOnlyStatCollection(r);
                        Moveset = new PBEReadOnlyPartyMoveset(r);
                    }

                    internal void ToBytes(EndianBinaryWriter w)
                    {
                        w.Write(Id);
                        w.Write(Species);
                        w.Write(Form);
                        w.Write(Nickname, true);
                        w.Write(Level);
                        w.Write(Friendship);
                        w.Write(Shiny);
                        w.Write(Ability);
                        w.Write(Nature);
                        w.Write(Gender);
                        w.Write(Item);
                        w.Write(Status1);
                        EffortValues.ToBytes(w);
                        IndividualValues.ToBytes(w);
                        Moveset.ToBytes(w);
                    }
                }
                public sealed class PBEInventorySlotInfo
                {
                    public PBEItem Item { get; }
                    public uint Quantity { get; }

                    internal PBEInventorySlotInfo(PBEBattleInventory.PBEBattleInventorySlot slot)
                    {
                        Item = slot.Item;
                        Quantity = slot.Quantity;
                    }
                    internal PBEInventorySlotInfo(EndianBinaryReader r)
                    {
                        Item = r.ReadEnum<PBEItem>();
                        Quantity = r.ReadUInt32();
                    }

                    internal void ToBytes(EndianBinaryWriter w)
                    {
                        w.Write(Item);
                        w.Write(Quantity);
                    }
                }

                public byte Id { get; }
                public string Name { get; }
                public ReadOnlyCollection<PBEInventorySlotInfo> Inventory { get; }
                public ReadOnlyCollection<PBEBattlePokemonInfo> Party { get; }

                internal PBETrainerInfo(PBETrainer trainer)
                {
                    Id = trainer.Id;
                    if (trainer.IsWild)
                    {
                        Name = string.Empty;
                        Inventory = PBEEmptyReadOnlyCollection<PBEInventorySlotInfo>.Value;
                    }
                    else
                    {
                        Name = trainer.Name;
                        Inventory = trainer.Inventory.Count == 0
                            ? PBEEmptyReadOnlyCollection<PBEInventorySlotInfo>.Value
                            : new ReadOnlyCollection<PBEInventorySlotInfo>(trainer.Inventory.Values.Select(s => new PBEInventorySlotInfo(s)).ToArray());
                    }
                    Party = new ReadOnlyCollection<PBEBattlePokemonInfo>(trainer.Party.Select(p => new PBEBattlePokemonInfo(p)).ToArray());
                }
                internal PBETrainerInfo(EndianBinaryReader r)
                {
                    Id = r.ReadByte();
                    Name = r.ReadStringNullTerminated();
                    int count = r.ReadUInt16();
                    if (count == 0)
                    {
                        Inventory = PBEEmptyReadOnlyCollection<PBEInventorySlotInfo>.Value;
                    }
                    else
                    {
                        var inv = new PBEInventorySlotInfo[count];
                        for (int i = 0; i < count; i++)
                        {
                            inv[i] = new PBEInventorySlotInfo(r);
                        }
                        Inventory = new ReadOnlyCollection<PBEInventorySlotInfo>(inv);
                    }
                    count = r.ReadByte();
                    if (count == 0)
                    {
                        Party = PBEEmptyReadOnlyCollection<PBEBattlePokemonInfo>.Value;
                    }
                    else
                    {
                        var party = new PBEBattlePokemonInfo[count];
                        for (int i = 0; i < count; i++)
                        {
                            party[i] = new PBEBattlePokemonInfo(r);
                        }
                        Party = new ReadOnlyCollection<PBEBattlePokemonInfo>(party);
                    }
                }
                internal PBETrainerInfo(PBETrainerInfo other, byte? onlyForTrainer)
                {
                    Id = other.Id;
                    Name = other.Name;
                    if (onlyForTrainer.HasValue && onlyForTrainer.Value == Id)
                    {
                        Inventory = other.Inventory;
                        Party = other.Party;
                    }
                    else
                    {
                        Inventory = PBEEmptyReadOnlyCollection<PBEInventorySlotInfo>.Value;
                        Party = PBEEmptyReadOnlyCollection<PBEBattlePokemonInfo>.Value;
                    }
                }

                internal void ToBytes(EndianBinaryWriter w)
                {
                    w.Write(Id);
                    w.Write(Name, true);
                    ushort icount = (ushort)Inventory.Count;
                    w.Write(icount);
                    for (int i = 0; i < icount; i++)
                    {
                        Inventory[i].ToBytes(w);
                    }
                    byte pcount = (byte)Party.Count;
                    w.Write(pcount);
                    for (int i = 0; i < pcount; i++)
                    {
                        Party[i].ToBytes(w);
                    }
                }
            }

            public byte Id { get; }
            public ReadOnlyCollection<PBETrainerInfo> Trainers { get; }

            internal PBETeamInfo(PBETeam team)
            {
                Id = team.Id;
                Trainers = new ReadOnlyCollection<PBETrainerInfo>(team.Trainers.Select(t => new PBETrainerInfo(t)).ToArray());
            }
            internal PBETeamInfo(EndianBinaryReader r)
            {
                Id = r.ReadByte();
                var trainers = new PBETrainerInfo[r.ReadByte()];
                for (int i = 0; i < trainers.Length; i++)
                {
                    trainers[i] = new PBETrainerInfo(r);
                }
                Trainers = new ReadOnlyCollection<PBETrainerInfo>(trainers);
            }
            internal PBETeamInfo(PBETeamInfo other, byte? onlyForTrainer)
            {
                Id = other.Id;
                Trainers = new ReadOnlyCollection<PBETrainerInfo>(other.Trainers.Select(t => new PBETrainerInfo(t, onlyForTrainer)).ToArray());
            }

            internal void ToBytes(EndianBinaryWriter w)
            {
                w.Write(Id);
                byte count = (byte)Trainers.Count;
                w.Write(count);
                for (int i = 0; i < count; i++)
                {
                    Trainers[i].ToBytes(w);
                }
            }
        }

        public PBEBattleType BattleType { get; }
        public PBEBattleFormat BattleFormat { get; }
        public PBEBattleTerrain BattleTerrain { get; }
        public PBEWeather Weather { get; }
        public PBESettings Settings { get; }
        public ReadOnlyCollection<PBETeamInfo> Teams { get; }

        internal PBEBattlePacket(PBEBattle battle)
        {
            using (var ms = new MemoryStream())
            using (var w = new EndianBinaryWriter(ms, encoding: EncodingType.UTF16))
            {
                w.Write(Code);
                w.Write(BattleType = battle.BattleType);
                w.Write(BattleFormat = battle.BattleFormat);
                w.Write(BattleTerrain = battle.BattleTerrain);
                w.Write(Weather = battle.Weather);
                w.Write((Settings = battle.Settings).ToBytes());
                Teams = new ReadOnlyCollection<PBETeamInfo>(battle.Teams.Select(t => new PBETeamInfo(t)).ToArray());
                for (int i = 0; i < 2; i++)
                {
                    Teams[i].ToBytes(w);
                }
                Data = new ReadOnlyCollection<byte>(ms.GetBuffer());
            }
        }
        internal PBEBattlePacket(byte[] data, EndianBinaryReader r)
        {
            Data = new ReadOnlyCollection<byte>(data);
            BattleType = r.ReadEnum<PBEBattleType>();
            BattleFormat = r.ReadEnum<PBEBattleFormat>();
            BattleTerrain = r.ReadEnum<PBEBattleTerrain>();
            Weather = r.ReadEnum<PBEWeather>();
            Settings = new PBESettings(r);
            Settings.MakeReadOnly();
            var teams = new PBETeamInfo[2];
            for (int i = 0; i < 2; i++)
            {
                teams[i] = new PBETeamInfo(r);
            }
            Teams = new ReadOnlyCollection<PBETeamInfo>(teams);
        }
        public PBEBattlePacket(PBEBattlePacket other, byte? onlyForTrainer)
        {
            using (var ms = new MemoryStream())
            using (var w = new EndianBinaryWriter(ms, encoding: EncodingType.UTF16))
            {
                w.Write(Code);
                w.Write(BattleType = other.BattleType);
                w.Write(BattleFormat = other.BattleFormat);
                w.Write(BattleTerrain = other.BattleTerrain);
                w.Write(Weather = other.Weather);
                w.Write((Settings = other.Settings).ToBytes());
                Teams = new ReadOnlyCollection<PBETeamInfo>(other.Teams.Select(t => new PBETeamInfo(t, onlyForTrainer)).ToArray());
                for (int i = 0; i < 2; i++)
                {
                    Teams[i].ToBytes(w);
                }
                Data = new ReadOnlyCollection<byte>(ms.GetBuffer());
            }
        }
    }
}
