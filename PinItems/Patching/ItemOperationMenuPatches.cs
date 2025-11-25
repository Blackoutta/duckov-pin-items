using Duckov.UI;
using HarmonyLib;
using PinItems.UI;
using UnityEngine.UI;

namespace PinItems.Patching
{
    [HarmonyPatch(typeof(ItemOperationMenu))]
    internal static class ItemOperationMenuPatches
    {
        [HarmonyPatch("Initialize")]
        [HarmonyPostfix]
        private static void InitializePostfix(ItemOperationMenu __instance, Button ___btn_Wishlist)
        {
            if (__instance == null || ___btn_Wishlist == null)
            {
                if (__instance == null)
                {
                    PinLogger.Warn("ItemOperationMenu.Initialize postfix received null instance, skipping pin button injection.");
                }
                else
                {
                    PinLogger.Warn("ItemOperationMenu.Initialize postfix missing wishlist button reference, cannot clone pin button.");
                }
                return;
            }

            PinMenuButtonController controller =
                __instance.gameObject.GetComponent<PinMenuButtonController>() ??
                __instance.gameObject.AddComponent<PinMenuButtonController>();
            PinLogger.Info($"ItemOperationMenu.Initialize postfix executing for menu '{__instance.name}'.");
            controller.EnsureButton(__instance, ___btn_Wishlist);
        }

        [HarmonyPatch("Setup")]
        [HarmonyPostfix]
        private static void SetupPostfix(ItemOperationMenu __instance)
        {
            PinMenuButtonController controller = __instance.gameObject.GetComponent<PinMenuButtonController>();
            if (controller == null)
            {
                PinLogger.Warn("ItemOperationMenu.Setup postfix could not find PinMenuButtonController; pin button will not update.");
            }
            else
            {
                PinLogger.Info($"ItemOperationMenu.Setup postfix refreshing pin button for '{__instance.name}'.");
            }
            controller?.RefreshButton();
        }
    }
}
