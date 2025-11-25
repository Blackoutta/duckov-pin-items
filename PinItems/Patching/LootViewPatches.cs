using Duckov;
using Duckov.UI;
using HarmonyLib;
using ItemStatsSystem;
using Duckov.Utilities;

namespace PinItems.Patching
{
    [HarmonyPatch(typeof(LootView))]
    internal static class LootViewPatches
    {
        [HarmonyPatch("OnStoreAllButtonClicked")]
        [HarmonyPrefix]
        private static bool StoreAllPrefix(LootView __instance)
        {
            Inventory targetInventory = __instance.TargetInventory;
            if (targetInventory == null || targetInventory != PlayerStorage.Inventory)
            {
                PinLogger.Info("LootView.OnStoreAllButtonClicked prefix skipped - target inventory invalid.");
                return true;
            }

            CharacterMainControl main = CharacterMainControl.Main;
            Item characterItem = main?.CharacterItem;
            Inventory inventory = characterItem?.Inventory;
            if (inventory == null)
            {
                PinLogger.Info("LootView.OnStoreAllButtonClicked prefix skipped - player inventory unavailable.");
                return true;
            }

            PinLogger.Info("LootView.OnStoreAllButtonClicked prefix executing custom Store All logic.");
            int lastIndex = inventory.GetLastItemPosition();
            bool soundPlayed = false;
            for (int i = 0; i <= lastIndex; i++)
            {
                if (inventory.lockedIndexes.Contains(i))
                {
                    continue;
                }

                Item itemAt = inventory.GetItemAt(i);
                if (itemAt == null)
                {
                    continue;
                }

                if (PinnedItemRegistry.IsPinned(itemAt))
                {
                    PinLogger.Info($"Skipping pinned item '{itemAt.DisplayName}' at slot {i}.");
                    continue;
                }

                if (!targetInventory.AddAndMerge(itemAt, 0))
                {
                    PinLogger.Info($"Failed to store '{itemAt.DisplayName}' at slot {i}; target inventory full.");
                    break;
                }

                if (!soundPlayed)
                {
                    AudioManager.PlayPutItemSFX(itemAt, false);
                    soundPlayed = true;
                }

                PinnedItemRegistry.Unpin(itemAt);
                PinLogger.Info($"Stored '{itemAt.DisplayName}' from slot {i} via Store All.");
            }

            return false;
        }
    }
}
