using Duckov.UI;
using HarmonyLib;
using PinItems.UI;

namespace PinItems.Patching
{
    [HarmonyPatch(typeof(ItemDisplay))]
    internal static class ItemDisplayPatches
    {
        [HarmonyPatch("Setup")]
        [HarmonyPostfix]
        private static void SetupPostfix(ItemDisplay __instance)
        {
            if (__instance == null)
            {
                PinLogger.Warn("ItemDisplay.Setup postfix received null instance.");
                return;
            }

            ItemPinIndicator indicator =
                __instance.gameObject.GetComponent<ItemPinIndicator>() ??
                __instance.gameObject.AddComponent<ItemPinIndicator>();
            indicator.Bind(__instance);
        }
    }
}
