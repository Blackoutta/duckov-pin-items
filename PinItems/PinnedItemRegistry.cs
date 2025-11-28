using System;
using System.Collections.Generic;
using Duckov.Utilities;
using ItemStatsSystem;

namespace PinItems
{
    internal static class PinnedItemRegistry
    {
        private const string PinnedTypesKey = "PinItems.PinnedTypes";
        private static readonly HashSet<int> PinnedTypeIds = new HashSet<int>();
        private static bool _hydrated;

        internal static event Action<int, bool>? PinStateChanged;

        internal static bool Toggle(Item? item, bool validateOwnership = true)
        {
            if (item == null)
            {
                return false;
            }
            return IsPinned(item, validateOwnership) ? Unpin(item) : Pin(item);
        }

        internal static bool Pin(Item? item)
        {
            if (!IsEligible(item) || item == null)
            {
                return false;
            }

            if (!TryGetTypeId(item, out int typeId))
            {
                return false;
            }

            EnsureHydrated();
            if (!PinnedTypeIds.Add(typeId))
            {
                return false;
            }

            PersistPinnedTypes();
            PinStateChanged?.Invoke(typeId, true);
            return true;
        }

        internal static bool Unpin(Item? item)
        {
            if (!TryGetTypeId(item, out int typeId))
            {
                return false;
            }

            EnsureHydrated();
            if (!PinnedTypeIds.Remove(typeId))
            {
                return false;
            }

            PersistPinnedTypes();
            PinStateChanged?.Invoke(typeId, false);
            return true;
        }

        internal static bool IsPinned(Item? item, bool validateOwnership = true)
        {
            if (!TryGetTypeId(item, out int typeId))
            {
                return false;
            }

            EnsureHydrated();
            if (!PinnedTypeIds.Contains(typeId))
            {
                return false;
            }

            if (validateOwnership && !IsOwnedByPlayer(item))
            {
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
            if (root != null && root == playerRoot)
            {
                return true;
            }

            return item.IsInPlayerStorage();
        }

        internal static void Clear()
        {
            EnsureHydrated();
            if (PinnedTypeIds.Count == 0)
            {
                PersistPinnedTypes();
                return;
            }

            PinnedTypeIds.Clear();
            PersistPinnedTypes();
        }

        private static bool TryGetTypeId(Item? item, out int typeId)
        {
            if (item == null)
            {
                typeId = default;
                return false;
            }

            typeId = item.TypeID;
            return true;
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

        private static void EnsureHydrated()
        {
            if (_hydrated)
            {
                return;
            }

            _hydrated = true;
            string serialized = CommonVariables.GetString(PinnedTypesKey, string.Empty);
            if (string.IsNullOrWhiteSpace(serialized))
            {
                return;
            }

            string[] tokens = serialized.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string token in tokens)
            {
                if (int.TryParse(token, out int typeId))
                {
                    PinnedTypeIds.Add(typeId);
                }
            }
        }

        private static void PersistPinnedTypes()
        {
            if (!_hydrated)
            {
                return;
            }

            if (PinnedTypeIds.Count == 0)
            {
                CommonVariables.SetString(PinnedTypesKey, string.Empty);
                return;
            }

            string serialized = string.Join(",", PinnedTypeIds);
            CommonVariables.SetString(PinnedTypesKey, serialized);
        }
    }
}
