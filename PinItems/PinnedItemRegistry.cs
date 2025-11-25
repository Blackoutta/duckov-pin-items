using System;
using System.Collections.Generic;
using ItemStatsSystem;

namespace PinItems
{
    internal static class PinnedItemRegistry
    {
        private static readonly Dictionary<int, Item> PinnedItems = new Dictionary<int, Item>();
        private static readonly Dictionary<int, Action<Item>> DestroyHandlers = new Dictionary<int, Action<Item>>();

        internal static event Action<Item, bool>? PinStateChanged;

        internal static bool Toggle(Item? item)
        {
            if (item == null)
            {
                return false;
            }
            return IsPinned(item) ? Unpin(item) : Pin(item);
        }

        internal static bool Pin(Item? item)
        {
            if (!IsEligible(item) || item == null)
            {
                return false;
            }

            int id = item.GetInstanceID();
            if (PinnedItems.ContainsKey(id))
            {
                return false;
            }

            PinnedItems.Add(id, item);
            Action<Item> handler = OnPinnedItemDestroyed;
            DestroyHandlers[id] = handler;
            item.onDestroy += handler;
            PinStateChanged?.Invoke(item, true);
            PinLogger.Info($"Pinned {item.DisplayName} ({id})");
            return true;
        }

        internal static bool Unpin(Item? item)
        {
            if (item == null)
            {
                return false;
            }
            bool removed = RemoveInternal(item, true);
            if (removed)
            {
                PinLogger.Info($"Unpinned {item.DisplayName} ({item.GetInstanceID()})");
            }
            return removed;
        }

        internal static bool IsPinned(Item? item, bool validateOwnership = true)
        {
            if (item == null)
            {
                return false;
            }

            int id = item.GetInstanceID();
            bool pinned = PinnedItems.ContainsKey(id);
            if (!pinned)
            {
                return false;
            }

            if (validateOwnership && !IsOwnedByPlayer(item))
            {
                RemoveInternal(item, true);
                return false;
            }

            return true;
        }

        internal static bool IsEligible(Item? item)
        {
            if (item == null || item.IsBeingDestroyed)
            {
                return false;
            }

            Item? playerRoot = CharacterMainControl.Main?.CharacterItem;
            if (playerRoot == null)
            {
                return false;
            }

            Item? root = item.GetRoot();
            return root != null && root == playerRoot;
        }

        internal static void Clear()
        {
            foreach (KeyValuePair<int, Item> pair in PinnedItems)
            {
                if (DestroyHandlers.TryGetValue(pair.Key, out Action<Item> handler))
                {
                    pair.Value.onDestroy -= handler;
                }
            }
            PinnedItems.Clear();
            DestroyHandlers.Clear();
        }

        private static bool RemoveInternal(Item? item, bool signal)
        {
            if (item == null)
            {
                return false;
            }

            int id = item.GetInstanceID();
            if (!PinnedItems.Remove(id))
            {
                return false;
            }

            if (DestroyHandlers.TryGetValue(id, out Action<Item> handler))
            {
                item.onDestroy -= handler;
                DestroyHandlers.Remove(id);
            }

            if (signal)
            {
                PinStateChanged?.Invoke(item, false);
            }

            return true;
        }

        private static void OnPinnedItemDestroyed(Item destroyed)
        {
            RemoveInternal(destroyed, true);
        }

        private static bool IsOwnedByPlayer(Item? item)
        {
            if (item == null)
            {
                return false;
            }
            Item? playerRoot = CharacterMainControl.Main?.CharacterItem;
            if (playerRoot == null)
            {
                return false;
            }
            Item? root = item.GetRoot();
            return root != null && root == playerRoot;
        }
    }
}
