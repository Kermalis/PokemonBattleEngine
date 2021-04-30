using Kermalis.EndianBinaryIO;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Kermalis.PokemonBattleEngine.Data
{
    public interface IPBEMovesetSlot
    {
        PBEMove Move { get; }
        byte PPUps { get; }
    }
    public interface IPBEPartyMovesetSlot : IPBEMovesetSlot
    {
        int PP { get; }
    }
    public interface IPBEMoveset<T> : IReadOnlyList<T> where T : IPBEMovesetSlot
    {
    }
    public interface IPBEMoveset : IPBEMoveset<IPBEMovesetSlot>
    {
    }
    public interface IPBEPartyMoveset<T> : IPBEMoveset<T>, IReadOnlyList<T> where T : IPBEPartyMovesetSlot
    {
    }
    public interface IPBEPartyMoveset : IPBEMoveset, IPBEPartyMoveset<IPBEPartyMovesetSlot>
    {
    }

    internal static class PBEMovesetInterfaceExtensions
    {
        internal static void ToBytes(this IPBEMoveset moveset, EndianBinaryWriter w)
        {
            byte count = (byte)moveset.Count;
            w.Write(count);
            for (int i = 0; i < count; i++)
            {
                IPBEMovesetSlot slot = moveset[i];
                w.Write(slot.Move);
                w.Write(slot.PPUps);
            }
        }
        internal static void ToBytes(this IPBEPartyMoveset moveset, EndianBinaryWriter w)
        {
            byte count = (byte)moveset.Count;
            w.Write(count);
            for (int i = 0; i < count; i++)
            {
                IPBEPartyMovesetSlot slot = moveset[i];
                w.Write(slot.Move);
                w.Write(slot.PP);
                w.Write(slot.PPUps);
            }
        }
        internal static void ToJson(this IPBEMoveset moveset, JsonTextWriter w)
        {
            w.WriteStartArray();
            for (int i = 0; i < moveset.Count; i++)
            {
                IPBEMovesetSlot slot = moveset[i];
                w.WriteStartObject();
                w.WritePropertyName(nameof(IPBEMovesetSlot.Move));
                w.WriteValue(slot.Move.ToString());
                w.WritePropertyName(nameof(IPBEMovesetSlot.PPUps));
                w.WriteValue(slot.PPUps);
                w.WriteEndObject();
            }
            w.WriteEndArray();
        }
        internal static void ToJson(this IPBEPartyMoveset moveset, JsonTextWriter w)
        {
            w.WriteStartArray();
            for (int i = 0; i < moveset.Count; i++)
            {
                IPBEPartyMovesetSlot slot = moveset[i];
                w.WriteStartObject();
                w.WritePropertyName(nameof(IPBEPartyMovesetSlot.Move));
                w.WriteValue(slot.Move.ToString());
                w.WritePropertyName(nameof(IPBEPartyMovesetSlot.PPUps));
                w.WriteValue(slot.PP);
                w.WritePropertyName(nameof(IPBEPartyMovesetSlot.PPUps));
                w.WriteValue(slot.PPUps);
                w.WriteEndObject();
            }
            w.WriteEndArray();
        }
    }
}
