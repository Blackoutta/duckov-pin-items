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
                return true;
            }

            CharacterMainControl main = CharacterMainControl.Main;
            Item characterItem = main?.CharacterItem;
            Inventory inventory = characterItem?.Inventory;
            if (inventory == null)
            {
                return true;
            }

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
                    continue;
                }

                if (!targetInventory.AddAndMerge(itemAt, 0))
                {
                    break;
                }

                if (!soundPlayed)
                {
                    AudioManager.PlayPutItemSFX(itemAt, false);
                    soundPlayed = true;
                }

            }

            return false;
        }
    }
}
