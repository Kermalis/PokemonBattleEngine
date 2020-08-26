using Kermalis.PokemonBattleEngine.Data;
using Kermalis.PokemonBattleEngine.Packets;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Kermalis.PokemonBattleEngine.Battle
{
    public sealed class PBEBattleInventory : IReadOnlyDictionary<PBEItem, PBEBattleInventory.PBEBattleInventorySlot>
    {
        public sealed class PBEBattleInventorySlot
        {
            public PBEItem Item { get; }
            public uint Quantity { get; internal set; }

            internal PBEBattleInventorySlot(PBEItem item, uint quantity)
            {
                Item = item;
                Quantity = quantity;
            }
        }

        private readonly Dictionary<PBEItem, PBEBattleInventorySlot> _slots;

        public PBEBattleInventorySlot this[PBEItem key] => _slots[key];
        public IEnumerable<PBEItem> Keys => _slots.Keys;
        public IEnumerable<PBEBattleInventorySlot> Values => _slots.Values;
        public int Count => _slots.Count;

        internal PBEBattleInventory(IReadOnlyList<(PBEItem item, uint quantity)> items)
        {
            _slots = new Dictionary<PBEItem, PBEBattleInventorySlot>(items.Count);
            foreach ((PBEItem item, uint quantity) in items)
            {
                if (item == PBEItem.None || !Enum.IsDefined(typeof(PBEItem), item))
                {
                    throw new ArgumentOutOfRangeException(nameof(items), $"Invalid item returned: {item}");
                }
                if (!_slots.TryGetValue(item, out PBEBattleInventorySlot slot))
                {
                    slot = new PBEBattleInventorySlot(item, quantity);
                    _slots.Add(item, slot);
                }
                else
                {
                    slot.Quantity += quantity;
                }
            }
        }
        internal PBEBattleInventory(IReadOnlyList<PBEBattlePacket.PBETeamInfo.PBETrainerInfo.PBEInventorySlotInfo> items)
        {
            _slots = new Dictionary<PBEItem, PBEBattleInventorySlot>(items.Count);
            foreach (PBEBattlePacket.PBETeamInfo.PBETrainerInfo.PBEInventorySlotInfo slot in items)
            {
                _slots.Add(slot.Item, new PBEBattleInventorySlot(slot.Item, slot.Quantity));
            }
        }

        public bool ContainsKey(PBEItem key)
        {
            return _slots.ContainsKey(key);
        }
        public bool TryGetValue(PBEItem key, out PBEBattleInventorySlot value)
        {
            return _slots.TryGetValue(key, out value);
        }
        public IEnumerator<KeyValuePair<PBEItem, PBEBattleInventorySlot>> GetEnumerator()
        {
            return _slots.GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return _slots.GetEnumerator();
        }

        private static readonly PBEBattleInventory _empty = new PBEBattleInventory(Array.Empty<(PBEItem, uint)>());
        internal static PBEBattleInventory Empty()
        {
            return _empty;
        }

        internal void Remove(PBEItem item)
        {
            _slots[item].Quantity--;
        }
    }
}
